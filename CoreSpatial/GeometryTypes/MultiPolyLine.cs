using System.Collections.Generic;

namespace CoreSpatial.GeometryTypes
{
    public class MultiPolyLine:IBasicGeometry
    {
        public int PartsNum { get; set; }
        public List<PolyLine> PolyLines { get; set; }

        public PolyLine this[int lineIndex] => PolyLines[lineIndex];
    }
}
