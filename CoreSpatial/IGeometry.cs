using System.Collections.Generic;
using CoreSpatial.BasicGeometrys;

namespace CoreSpatial
{
    public interface IGeometry
    {
        List<GeoPoint> Points { get; }
        GeometryType GeometryType { get; }

        IBasicGeometry BasicGeometry { get; set; }
    }
}
