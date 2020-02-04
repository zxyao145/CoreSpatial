using System;

namespace CoreSpatial.ShapeFile.ShapefileModel
{
    /// <summary>
    /// 一个记录段
    /// </summary>
    internal class ShapefileRecord
    {
        #region 记录头
        /// <summary>
        /// 记录号（ Record Number ），位序big
        /// </summary>
        public int RecordNumber { get; set; }

        /// <summary>
        /// 坐标记录长度(Content Length) 位序big
        /// </summary>
        public int ContentLength { get; set; } 
        #endregion
        
        /// <summary>
        /// 记录内容
        /// </summary>
        public byte[] RecordContents { get; private set; }

        public bool SetRecordContents(byte[] contents)
        {
            if (contents.Length == ContentLength * 2)
            {
                RecordContents = contents;
                return true;
            }
            else
            {
                return false;
            }
        }

        public ShapefileRecord(byte[] recordContents, bool noHeader)
        {
            this.RecordContents = recordContents;
        }

        /// <summary>
        /// 通过当前记录的记录头构造当前记录的实例
        /// </summary>
        /// <param name="recordHeader"></param>
        public ShapefileRecord(byte[] recordHeader)
        {
            if (recordHeader.Length == 8)
            {
                //Byte 0 Record Number Record Number Integer Big
                //Byte 4 Content Length Content Length Integer Big


                //翻转字节数组，因为record headers是大字节序
                Array.Reverse(recordHeader);

                //Content length is measured in 16-bit words.
                this.ContentLength = BitConverter.ToInt32(recordHeader, 0);
                this.RecordNumber = BitConverter.ToInt32(recordHeader, 4);
                this.RecordContents = new byte[ContentLength * 2];
            }

        }
    }
}
