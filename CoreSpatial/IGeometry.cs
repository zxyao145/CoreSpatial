using System.Collections.Generic;
using CoreSpatial.BasicGeometrys;

namespace CoreSpatial
{
    public interface IGeometry
    {
        IEnumerable<Coordinate> Coordinates { get; }
        GeometryType GeometryType { get; }
        IBasicGeometry BasicGeometry { get; set; }
    }
}
