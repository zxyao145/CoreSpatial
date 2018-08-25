using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CoreSpatial
{
    public interface IFeatureSet
    {
        DataTable AttrTable { get; set; }
        FeatureType FeatureType { get; }
        IFeatureList Features { get; set; }
    }
}
