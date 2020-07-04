using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CoreSpatial
{
    public class ShapeFileBytes
    {
        public byte[] ShpBytes { get; set; }
        public byte[] ShxBytes { get; set; }
        public byte[] DbfBytes { get; set; }
        public byte[] PrjBytes { get; set; } = null;
    }
}
