using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace CoreSpatial.Dbf
{
    internal class DbfReader:IDisposable
    {
        private Stream _readStream;
        private BinaryReader _reader;

        public Encoding Encoding { get; private set; }

        private DbfHeader _header;
        private DataTable _dbfTable;
        private int _curRowIndex = 0;

        public DataTable DbfTable => _dbfTable;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbfFilePath">dbf文件路径</param>
        public DbfReader(string dbfFilePath)
        {
            if (!File.Exists(dbfFilePath))
            {
                throw new IOException($"Dbf文件不存在：{dbfFilePath}");
            }

            var fs = new FileStream(dbfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            Init(fs);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbfFileStream">dbf FileStream</param>
        public DbfReader(Stream dbfFileStream)
        {
            Init(dbfFileStream);
        }

        private void Init(Stream dbfFileStream)
        {
            _readStream = dbfFileStream;
            _reader = new BinaryReader(_readStream);
            ReadHeader();
        }

        /// <summary>
        /// 读取头文件
        /// </summary>
        private void ReadHeader()
        {
            _reader.BaseStream.Seek(8, SeekOrigin.Begin);
            byte[] hearderLengthBs = new byte[2];
            _reader.Read(hearderLengthBs);
            var hearderLength = BitConverter.ToInt16(hearderLengthBs);
            var headerBytes = new  byte[hearderLength];
            _reader.BaseStream.Seek(0, SeekOrigin.Begin);
            _reader.Read(headerBytes);
            _header = new DbfHeader(headerBytes);
            Encoding = _header.Encoding;
        }

        #region 读取文件的所有记录信息
        ///// <summary>
        ///// 读取文件的所有记录信息
        ///// </summary>
        //public List<Dictionary<string, object>> ReadToDictionary()
        //{
        //    var records = new List<Dictionary<string, object>>();
        //    var fieldInfos = _header.FieldInfos;

        //    //跳过文件头
        //    _reader.BaseStream.Seek(_header.HeaderLength+1, SeekOrigin.Begin);
        //    for (int i = 0; i < _header.RecordCount; i++)
        //    {
        //        if (_reader.PeekChar() == '*') // DELETED
        //        {
        //            continue;
        //        }

        //        var record = new Dictionary<string, object>();
        //        var fieldStartIndex = 0;
        //        var row = _reader.ReadBytes(_header.RecordLength);
        //        foreach (var field in fieldInfos)
        //        {
        //            byte[] buffer = new byte[field.FieldLength];
        //            //(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
        //            Array.Copy(row, fieldStartIndex, buffer, 0, field.FieldLength);
        //            fieldStartIndex += field.FieldLength;
        //            string text = (_encoding.GetString(buffer) ?? String.Empty).Trim();

        //           var valueObj=  GetValueObj(field, text, buffer);
        //            record.Add(field.FieldName, valueObj);
        //        }

        //        records.Add(record);
        //    }

        //    return records;
        //}


        public DataTable ReadAllDataTable()
        {
            if (_dbfTable == null)
            {
                var recordCount = _header.RecordCount;
                for (int i = 0; i < recordCount; i++)
                {
                    GetNextRow();
                }
            }
          
            return _dbfTable;
        }
        #endregion

        /// <summary>
        /// 逐行读取属性记录信息
        /// </summary>
        /// <returns></returns>
        public DataRow GetNextRow()
        {
            var fieldInfos = _header.FieldInfos;
            if (_dbfTable == null) 
            {
                _dbfTable = new DataTable();
                foreach (var fieldInfo in fieldInfos)
                {
                    _dbfTable.Columns.Add(fieldInfo.FieldName, FieldTypeTransform.DbfFieldType2CSharpType(fieldInfo));
                }

                //跳过文件头
                _reader.BaseStream.Seek(_header.HeaderLength , SeekOrigin.Begin);
            }

            var dtRow = _dbfTable.NewRow();
            _curRowIndex++;
            if (_curRowIndex > _header.RecordCount)
            {
                 dtRow = null;
            }
            else
            {
                if (_reader.PeekChar() == '*') // DELETED
                {
                    dtRow = null;
                }
                else
                {
                    var fieldStartIndex = 1;
                    var row = _reader.ReadBytes(_header.RecordLength);

                    foreach (var field in fieldInfos)
                    {
                        byte[] buffer = new byte[field.FieldLength];
                        //(Array sourceArray, int sourceIndex, Array destinationArray, int destinationIndex, int length)
                        Array.Copy(row, fieldStartIndex, buffer, 0, field.FieldLength);
                        fieldStartIndex += field.FieldLength;
                        string text = (Encoding.GetString(buffer) ?? String.Empty).Trim().Replace("\0", "");

                        var valueObj = GetValueObj(field, text, buffer);
                        dtRow[field.FieldName] = valueObj;
                    }
                    _dbfTable.Rows.Add(dtRow);
                }
            }

            return dtRow;
        }

        /// <summary>
        /// 根据数据类型，进行类型转换获取数据值
        /// </summary>
        /// <param name="fieldInfo">字段信息</param>
        /// <param name="text">从dbf表中获取得字符串值</param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private object GetValueObj(DbfFieldInfo fieldInfo, string text, byte[] buffer)
        {
            object obj;
            switch ((DbfFieldType)fieldInfo.FieldType)
            {
                case DbfFieldType.Character:
                    obj = text;
                    break;

                case DbfFieldType.Numeric:
                    if (String.IsNullOrWhiteSpace(text))
                    {
                        obj = 0.0;
                    }
                    else
                    {
                        obj = Convert.ToDouble(text);
                    }
                    break;

                case DbfFieldType.Date:
                    if (String.IsNullOrWhiteSpace(text))
                    {
                        obj = null;
                    }
                    else
                    {
                        obj = DateTime.ParseExact(text, "yyyyMMdd", CultureInfo.InvariantCulture);
                    }
                    break;


                case DbfFieldType.Logical:
                    if (String.IsNullOrWhiteSpace(text))
                    {
                        obj = false;
                    }
                    else
                    {
                        obj = (buffer[0] == 'Y' || buffer[0] == 'T');
                    }
                    break;

                case DbfFieldType.General:
                case DbfFieldType.Byte:
                default:
                    obj = buffer;
                    break;
            }

            return obj;
        }
        
        /// <summary>
        /// 儒略日转DateTime
        /// </summary>
        /// <param name="julianDateAsLong"></param>
        /// <returns></returns>
        private static DateTime JulianToDateTime(long julianDateAsLong)
        {
            if (julianDateAsLong == 0) return DateTime.MinValue;
            double p = Convert.ToDouble(julianDateAsLong);
            double s1 = p + 68569;
            double n = Math.Floor(4 * s1 / 146097);
            double s2 = s1 - Math.Floor(((146097 * n) + 3) / 4);
            double i = Math.Floor(4000 * (s2 + 1) / 1461001);
            double s3 = s2 - Math.Floor(1461 * i / 4) + 31;
            double q = Math.Floor(80 * s3 / 2447);
            double d = s3 - Math.Floor(2447 * q / 80);
            double s4 = Math.Floor(q / 11);
            double m = q + 2 - (12 * s4);
            double j = (100 * (n - 49)) + i + s4;
            return new DateTime(Convert.ToInt32(j), Convert.ToInt32(m), Convert.ToInt32(d));
        }

        public void Dispose()
        {
            _readStream.Dispose();
            _reader.Dispose();
        }
    }
}
