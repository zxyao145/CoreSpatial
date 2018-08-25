using System;
using System.Collections.Generic;
using System.Text;
using CoreSpatial.GeometryTypes;
using CoreSpatial.ShpOper;

namespace CoreSpatial
{
    public interface IGeometry
    {
        List<IGeoPoint> Points { get;}
        GeometryType GeometryType { get; set; }

        IBasicGeometry BasicGeometry { get; set; }
    }
}
