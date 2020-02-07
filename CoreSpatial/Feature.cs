using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CoreSpatial
{
    public class Feature: IFeature
    {
        public Feature(IGeometry geometry)
        {
            Geometry = geometry;
        }
        
        /// <summary>
        /// 要素的Fid
        /// </summary>
        public int Fid=> ParentFeatureSet.Features.IndexOf(this);

        /// <summary>
        /// 当前要素的属性信息
        /// </summary>
        public DataRow DataRow => ParentFeatureSet.AttrTable.Rows[Fid];

        /// <summary>
        /// 当前要素的几何信息
        /// </summary>
        public IGeometry Geometry { get; set; }

        /// <summary>
        /// 当前要素的几何类型
        /// </summary>
        public GeometryType GeometryType => Geometry.GeometryType;

        /// <summary>
        /// 当前要素所属的要素集
        /// </summary>
        public IFeatureSet ParentFeatureSet { get; internal set; }
    }
}
