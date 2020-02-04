using System;

namespace CoreSpatial.ShapeFile.ShapefileModel
{
    /// <summary>
    /// shp或shx文件的文件头，除了FileLength外，两者是相同的
    /// </summary>
    internal class ShpOrShxHeader
    {
        /// <summary>
        /// 32-bit words的长度，bytes长度/2
        /// </summary>
        public int FileLength { get; set; }
        public int Version { get; set; }
        public int ShapeType { get; set; }
        public double XMin { get; set; }
        public double YMin { get; set; }
        public double XMax { get; set; }
        public double YMax { get; set; }
        public double ZMin { get; set; }
        public double ZMax { get; set; }
        public double MMin { get; set; }
        public double MMax { get; set; }

        public ShpOrShxHeader()
        {
            //Empty constructor
        }

        public ShpOrShxHeader(byte[] headerBytes)
        {
            if (headerBytes.Length >= 100)
            {
                //文件长度，大字节序
                this.FileLength = BitConverter.ToInt32(new byte[4]
                {
                    headerBytes[27],
                    headerBytes[26], 
                    headerBytes[25],
                    headerBytes[24]
                }, 0);

                //从28之后小字节序
                this.Version = BitConverter.ToInt32(headerBytes, 28);
                this.ShapeType = BitConverter.ToInt32(headerBytes, 32);
                this.XMin = BitConverter.ToDouble(headerBytes, 36);
                this.YMin = BitConverter.ToDouble(headerBytes, 44);
                this.XMax = BitConverter.ToDouble(headerBytes, 52);
                this.YMax = BitConverter.ToDouble(headerBytes, 60);
                this.ZMin = BitConverter.ToDouble(headerBytes, 68);
                this.ZMax = BitConverter.ToDouble(headerBytes, 76);
                this.MMin = BitConverter.ToDouble(headerBytes, 84);
                this.MMax = BitConverter.ToDouble(headerBytes, 92);
            }
        }

    }
}
