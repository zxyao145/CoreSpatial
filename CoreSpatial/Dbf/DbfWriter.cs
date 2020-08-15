using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace CoreSpatial.Dbf
{
    internal class DbfWriter : IDisposable
    {
        private readonly Stream _writerStream;
        private readonly Encoding _encoding;
        private readonly DataTable _dataTable;
        private readonly DataColumnCollection _dataColumns;
        private List<DbfFieldInfo> _fieldInfos;

        public Stream Stream => _writerStream;

        public DbfWriter(DataTable dataTable, string path=null, 
            Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                _writerStream = new MemoryStream();
            }
            else
            {
                var ext = Path.GetExtension(path);
                if (ext != ".dbf")
                {
                    throw new Exception("扩展名不是dbf");
                }

                _writerStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

            }
            _dataTable = dataTable;
            _dataColumns = _dataTable.Columns;
            _encoding = encoding ?? Encoding.ASCII;
        }


        public void Write()
        {
            var (headerLength, recordLength) = WriteHeader();
            //写入文件记录
            foreach (var dataRow in _dataTable.AsEnumerable())
            {
                WriteRow(dataRow);
            }
            //写入文件尾部
            WriteEnd();

            _writerStream.Flush();
        }

        /// <summary>
        /// 写入一条记录
        /// </summary>
        /// <param name="dataRow"></param>
        private void WriteRow(DataRow dataRow)
        {
            var columnIndex = 0;
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            //写入控制位
            _writerStream.WriteByte(0x20);
            //写入字段
            foreach (DataColumn dataColumn in _dataColumns)
            {
                var curFieldInfo = _fieldInfos[columnIndex];
                columnIndex++;
                var dataObj = dataRow[dataColumn.ColumnName];
                string strValue = dataObj.ToString();
                
                if (!string.IsNullOrEmpty(strValue)) 
                {
                    switch (curFieldInfo.FieldType)
                    {
                        case DbfFieldType.Logical:
                            strValue = bool.Parse(strValue).ToString();
                            break;
                        case DbfFieldType.Numeric:
                            strValue = decimal.Parse(strValue).ToString(nfi);
                            break;
                  
                        case DbfFieldType.Date:
                            strValue = DateTime.Parse(strValue).ToString("yyyyMMdd");
                            break;
                        default:
                            strValue = strValue.Trim() + "";
                            break;
                    }
                }
                else
                {
                    //if (curFieldInfo.FieldType == DbfFieldType.Character
                    //    || curFieldInfo.FieldType == DbfFieldType.Date)
                    {
                        strValue = "";
                    }
                }
                
                //while (strValue.Length < curFieldInfo.FieldLength)
                //{
                //    strValue = strValue + (char)0x00;
                //}

                var originalValBytes = _encoding.GetBytes(strValue);
                var writeBytes = new byte[curFieldInfo.FieldLength];
                if (originalValBytes.Length >= curFieldInfo.FieldLength)
                {
                    Buffer.BlockCopy(originalValBytes, 0,
                        writeBytes,0, curFieldInfo.FieldLength);
                }
                else
                {
                    var reminderLength =
                        curFieldInfo.FieldLength - originalValBytes.Length;
                    for (int i = originalValBytes.Length; i < curFieldInfo.FieldLength; i++)
                    {
                        writeBytes[i] = 0x20;
                    }

                    Buffer.BlockCopy(originalValBytes, 0,
                        writeBytes, 0, originalValBytes.Length);
                    
                }

                //if (buffer.Length > curFieldInfo.FieldLength)
                //{
                //    buffer[curFieldInfo.FieldLength - 1] = 0x00;
                //}

                _writerStream.Write(writeBytes);
            }

        }

        /// <summary>
        /// 写入文件尾部
        /// </summary>
        private void WriteEnd()
        {
            _writerStream.WriteByte(0x1A);
        }

        #region 头文件

        /// <summary>
        /// 写头文件
        /// </summary>
        /// <returns>(头文件的长度，记录数量)</returns>
        private (int, int) WriteHeader()
        {
            _fieldInfos = new List<DbfFieldInfo>();
            var (fieldInfoBytes, recordLength) = BuildFieldType();

            var allHeaderLength = 33 + fieldInfoBytes.Length;
            var headerMetaBytes = new byte[allHeaderLength];
            //版本号
            headerMetaBytes[0] = (byte)DbfVersion.FoxBaseDBase3NoMemo;

            //更新日期
            var updateDate = DateTime.Now;
            headerMetaBytes[1] = Convert.ToByte(updateDate.Year-1900);
            headerMetaBytes[2] = Convert.ToByte(updateDate.Month);
            headerMetaBytes[3] = Convert.ToByte(updateDate.Day);

            //记录条数4-7
            var recordNum = BitConverter.GetBytes(_dataTable.Rows.Count);
            Buffer.BlockCopy(recordNum, 0, headerMetaBytes, 4, 4);
            //文件头字节数8-9
            short headerLength = (short)allHeaderLength;
            Buffer.BlockCopy(BitConverter.GetBytes(headerLength), 0, headerMetaBytes, 8, 2);

            //10-11一条记录长度
            Buffer.BlockCopy(BitConverter.GetBytes(recordLength), 0, headerMetaBytes, 10, 2);

            //12-13保留字节
            //14未完成操作
            //15 dBASE IV密码
            //16-27保留字节
            //28 DBF MDX标识
            //29 language driver ID
            //30-31 保留字节
            for (int i = 12; i < 32; i++)
            {
                headerMetaBytes[i] = 0;
            }
            headerMetaBytes[29] = 0x4d;
            //填充记录项信息
            Buffer.BlockCopy(fieldInfoBytes, 0, headerMetaBytes, 32, fieldInfoBytes.Length);
            //记录项目终止标识
            headerMetaBytes[allHeaderLength - 1] = 0x0D;

            //写入文件头
            _writerStream.Write(headerMetaBytes);
            _writerStream.Flush();
            return (headerMetaBytes.Length, recordLength);
        }

        /// <summary>
        /// 构建文件头字节数组
        /// </summary>
        /// <returns></returns>
        private (byte[], short) BuildFieldType()
        {
            var result = new byte[_dataColumns.Count * 32];
            int oneRecordLength = 0;
            for (int i = 0; i < _dataColumns.Count; i++)
            {
                //当前列信息
                var column = _dataColumns[i];
                var curColumnInfo = new byte[32];

                //列名
                var fieldNameLength = 11;
                var fieldName = _encoding.GetBytes(column.ColumnName + "\0");
                if (fieldName.Length >= fieldNameLength)
                {
                    Buffer.BlockCopy(fieldName, 0, curColumnInfo, 0, fieldNameLength);
                }
                else
                {
                    Buffer.BlockCopy(fieldName, 0, curColumnInfo, 0, fieldName.Length);
                    var remainderLength = fieldNameLength - fieldName.Length;
                    var remainderBytes = new byte[remainderLength];
                    for (int j = 0; j < remainderLength; j++)
                    {
                        remainderBytes[j] = 0;
                    }
                    Buffer.BlockCopy(remainderBytes, 0, 
                        curColumnInfo, fieldName.Length,
                        remainderBytes.Length);
                }

                //列类型
                var dbfFieldInfo
                    = FieldTypeTransform
                    .CSharpType2DbfFieldInfoWithoutName(column.DataType);
                var dbfFieldType = dbfFieldInfo.FieldType;
                var fieldLength = dbfFieldInfo.FieldLength;
                var accuracy = dbfFieldInfo.Accuracy;

                _fieldInfos.Add(dbfFieldInfo);

                var dbfType = _encoding.GetBytes(dbfFieldType.ToString())[0];
                curColumnInfo[11] = dbfType;

                //保留字节 12-15
                Buffer.BlockCopy(BitConverter.GetBytes(0), 0, 
                    curColumnInfo, 12, 4);

                curColumnInfo[16] = fieldLength; //fieldLength
                curColumnInfo[17] = accuracy; //accuracy

                oneRecordLength += fieldLength;

                //18-31 填充0
                for (int j = 18; j < 32; j++)
                {
                    curColumnInfo[j] = 0;
                }
                var offset = i * 32;
                Buffer.BlockCopy(curColumnInfo, 0, result, offset, 32);
            }

            oneRecordLength += 1;//控制位
            return (result, (short)oneRecordLength);
        }

        #endregion

        public void Dispose()
        {
            this._writerStream.Dispose();
        }

    }
}
