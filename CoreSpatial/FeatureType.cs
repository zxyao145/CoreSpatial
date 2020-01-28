using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSpatial
{
    /// <summary>
    /// 标准shapefile的类型
    /// </summary>
    public enum FeatureType
    {
        NullShape = 0,
        Point =1,
        PolyLine = 3,
        Polygon = 5,
        MultiPoint = 8
    }
}
