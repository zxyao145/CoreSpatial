using System.Collections.Generic;

namespace CoreSpatial.BasicGeometrys
{
    public class PolyLine: MultiPoint,IBasicGeometry
    {
        public PolyLine():base()
        {
            
        }
        public PolyLine(List<GeoPoint> points) : base(points)
        {

        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public new GeometryType GeometryType => GeometryType.PolyLine;
    }
}
