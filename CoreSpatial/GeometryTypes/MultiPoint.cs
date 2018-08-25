using System.Collections.Generic;

namespace CoreSpatial.GeometryTypes
{
    public class MultiPoint: IBasicGeometry
    {
        public List<IGeoPoint> Points { get; set; }

        public IGeoPoint this[int index] => Points[index];

        public bool IsLineRing => (GeoPoint)Points[0] == (GeoPoint)Points[Points.Count - 1];
    }
}
