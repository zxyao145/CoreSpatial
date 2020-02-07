using System.Collections.Generic;
using System.Linq;
using CoreSpatial.BasicGeometrys;

namespace CoreSpatial
{
    public class Geometry: IGeometry
    {
        public Geometry(IBasicGeometry basicGeometry)
        {
            BasicGeometry = basicGeometry;
        }

        public IEnumerable<Coordinate> Coordinates => BasicGeometry.Coordinates;

        public GeometryType GeometryType => BasicGeometry.GeometryType;

        public IBasicGeometry BasicGeometry { get; set; }
    }
}
