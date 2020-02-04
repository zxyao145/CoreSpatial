using System;

namespace CoreSpatial.ShapeFile.ShapefileModel
{
    /// <summary>
    /// 一条空间实体信息的索引记录
    /// </summary>
    internal class ShxRecord
    {
        /// <summary>
        /// 位移量（ Offset ）,表示坐标文件中的对应记录的起始位置相对于坐标文件起始位置的位移量。big位序
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// 记录长度（ Content Length ）,表示坐标文件中的对应记录的长度。big位序
        /// </summary>
        public int Length { get; private set; }

        public ShxRecord(byte[] recordHeader)
        {
            if (recordHeader.Length == 8)
            {
                Array.Reverse(recordHeader);
                this.Length = BitConverter.ToInt32(recordHeader, 0);
                this.Offset = BitConverter.ToInt32(recordHeader, 4);
            }
        }
    }
}
