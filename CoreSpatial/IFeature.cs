using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CoreSpatial
{
    public interface IFeature
    {
        /// <summary>
        /// 要素的Fid
        /// </summary>
        int Fid { get; }

        /// <summary>
        /// 当前要素的属性信息
        /// </summary>
        DataRow DataRow { get; }

        /// <summary>
        /// 当前要素的几何信息
        /// </summary>
        IGeometry Geometry { get; set; }

        /// <summary>
        /// 当前要素的几何类型
        /// </summary>
        GeometryType GeometryType { get; }

        /// <summary>
        /// 当前要素所属的要素集
        /// </summary>
        IFeatureSet ParentFeatureSet { get;}
    }
}
