using System.Collections.Generic;
using Jil;

namespace CoreSpatial.Converter.GeoJSON
{
    /// <summary>
    /// shapefile对应的GeoJSON实体类
    /// </summary>
    public class GeoJSON
    {
        /// <summary>
        /// GeoJSON的类型，对应根属性而言为FeatureCollection
        /// </summary>
        [JilDirective(Name = "type")]
        public GeoJSONType Type => GeoJSONType.FeatureCollection;

        /// <summary>
        /// shapefile中的feature对应的GeoJSON实体类的几何
        /// </summary>
        [JilDirective(Name = "features")]
        public List<FeatureGeoJson> Features { get; set; }

        /// <summary>
        /// 边界框数组
        /// </summary>
        [JilDirective(Name = "bbox" )]
        public List<double> Bbox { get; set; }

        /// <summary>
        /// 返回空的FeatureCollection
        /// </summary>
        [JilDirective(Ignore = true)]
        internal static string Empty => "{\"type\":\"FeatureCollection\",\"features\":[]}";
    }
}
