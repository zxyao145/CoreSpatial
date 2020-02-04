using System.Collections.Generic;

namespace CoreSpatial.ShapeFile.ShapefileModel
{
    internal class Shapefile
    {
        public ShpOrShxHeader Header { get; private set; }
        public List<ShapefileRecord> Records { get; private set; }

        /// <summary>
        /// 获取文件的长度
        /// </summary>
        public int SizeInBytes
        {
            get
            {
                if (Header != null)
                {
                    return Header.FileLength * 2;
                }
                else
                {
                    return -1;
                }
            }
        }

        public bool SetHeader(ShpOrShxHeader header)
        {
            if (header != null && this.Header == null)
            {
                this.Header = header;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetRecords(List<ShapefileRecord> records)
        {
            if (records != null && records.Count > 0)
            {
                this.Records = records;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
