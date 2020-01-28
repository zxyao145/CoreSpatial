using System;

namespace CoreSpatial.Dbf
{
    internal struct DbfFieldInfo
    {
        /// <summary>
        /// 字段名
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string FieldName;

        /// <summary>
        /// 字段类型
        /// </summary>
        public DbfFieldType FieldType;

        /// <summary>
        /// 字段长度
        /// </summary>
        public byte FieldLength;

        /// <summary>
        /// 字段精度
        /// </summary>
        public byte Accuracy;

       
        public override string ToString()
        {
            return String.Format("{0} {1}", FieldName, FieldType);
        }
    }

}
