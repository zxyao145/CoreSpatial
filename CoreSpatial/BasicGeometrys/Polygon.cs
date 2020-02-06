using System.Collections.Generic;

namespace CoreSpatial.BasicGeometrys
{
    public class Polygon : MultiPolyLine,IBasicGeometry
    {
        public Polygon() : base()
        {
            
        }

        public Polygon(List<PolyLine> polyLines):base(polyLines)
        {
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public new GeometryType GeometryType => GeometryType.Polygon;
    }
}