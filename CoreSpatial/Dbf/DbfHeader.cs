using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSpatial.Dbf
{
    internal class DbfHeader
    {
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// 当前版本
        /// </summary>
        public DbfVersion Version { get; private set; } = DbfVersion.FoxBaseDBase3NoMemo;

        /// <summary>
        /// 最近更新日期
        /// </summary>
        public DateTime LastUpdated { get; private set; } = DateTime.Now;

        /// <summary>
        /// 文件中的记录条数
        /// </summary>
        public int RecordCount { get; private set; }

        /// <summary>
        /// 文件头的长度
        /// </summary>
        public short HeaderLength { get; private set; }

        /// <summary>
        /// 字段的个数
        /// </summary>
        public int FieldCount => (HeaderLength - 33) / 32;

        /// <summary>
        /// 一条记录的长度
        /// </summary>
        public short RecordLength { get; private set; }

        /// <summary>
        /// LangDriverId 29
        /// </summary>
        public byte LangDriverId { get; private set; }

        public List<DbfFieldInfo> FieldInfos { get; private set; } = new List<DbfFieldInfo>();

        /// <summary>
        /// 从头中解析出字段名及字段类型
        /// </summary>
        /// <param name="tableHeader"></param>
        private void GetFieldsFromHeader(byte[] tableHeader)
        {
            if (tableHeader.Length >= this.HeaderLength)
            {
                for (int i = 1; i <= this.FieldCount; i++)
                {
                    var offset = i * 32;

                    var fieldInfo = new DbfFieldInfo
                    {
                        FieldName = Encoding
                            .GetString(tableHeader, offset, 10)
                            .Replace("\0",""),
                        FieldType = (DbfFieldType)
                            (Encoding
                                .GetChars(tableHeader, offset + 11, 1)[0]),
                        FieldLength = tableHeader[offset + 16],
                        Accuracy = tableHeader[offset + 17]
                    };
                    FieldInfos.Add(fieldInfo);
                }
            }
        }

        public DbfHeader(byte[] tableHeader)
        {
            if (tableHeader != null && tableHeader.Length >= 32)
            {
                this.Version = (DbfVersion)tableHeader[0];
                //string year = tableHeader[1].ToString();
                //string month = tableHeader[2].ToString("00");
                //string day = tableHeader[3].ToString("00");
                //string lastUpdatedStr = (year.Length == 2 ? year : year.Substring(year.Length - 2, 2)) + month + day;
                //this.LastUpdated = DateTime.ParseExact(lastUpdatedStr, "yyMMdd", CultureInfo.CurrentCulture);

                this.LastUpdated = new DateTime(tableHeader[1] + 1900, tableHeader[2], tableHeader[3]);
                this.RecordCount = BitConverter.ToInt32(tableHeader, 4);
                this.HeaderLength = BitConverter.ToInt16(tableHeader, 8);
                this.RecordLength = BitConverter.ToInt16(tableHeader, 10);
                this.LangDriverId = tableHeader[29];
                this.Encoding = DbfEncodingUtil.GetEncoding(this.LangDriverId);
                this.GetFieldsFromHeader(tableHeader);
            }
        }

    }
}
