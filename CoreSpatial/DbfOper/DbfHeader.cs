using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CoreSpatial.DbfOper
{
    internal class DbfHeader
    {
        private Encoding _encoding;

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
                    var fieldInfo = new DbfFieldInfo
                    {
                        FieldName = _encoding.GetString(tableHeader, i * 32, 10).Replace("\0",""),
                        FieldType = (DbfFieldType)(_encoding.GetChars(tableHeader, (i * 32) + 11, 1)[0]),
                        FieldLength = tableHeader[(i * 32) + 16],
                        Accuracy = tableHeader[(i * 32) + 17]
                    };
                    FieldInfos.Add(fieldInfo);
                }
            }
        }

        public DbfHeader(byte[] tableHeader, Encoding encoding)
        {
            if (tableHeader != null && tableHeader.Length >= 32)
            {
                _encoding = encoding;

                this.Version = (DbfVersion)tableHeader[0];
                string year = tableHeader[1].ToString();
                string month = tableHeader[2].ToString("00");
                string day = tableHeader[3].ToString("00");
                string lastUpdatedStr = (year.Length == 2 ? year : year.Substring(year.Length - 2, 2)) + month + day;
                this.LastUpdated = DateTime.ParseExact(lastUpdatedStr, "yyMMdd", CultureInfo.CurrentCulture);
                this.RecordCount = BitConverter.ToInt32(tableHeader, 4);
                this.HeaderLength = BitConverter.ToInt16(tableHeader, 8);
                this.RecordLength = BitConverter.ToInt16(tableHeader, 10);
                this.GetFieldsFromHeader(tableHeader);
            }
        }

    }
}
