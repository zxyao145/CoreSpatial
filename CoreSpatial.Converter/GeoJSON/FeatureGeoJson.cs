using System.Collections.Generic;
using Jil;

namespace CoreSpatial.Converter.GeoJSON
{
    /// <summary>
    /// shapefile中每个Feature对应的GeoJSON实体类，包括Feature类型、几何类型和属性信息
    /// </summary>
    public class FeatureGeoJson
    {
        /// <summary>
        /// GeoJSON类型，
        /// </summary>
        [JilDirective(Name = "type")]
        public GeoJSONType Type => GeoJSONType.Feature;

        /// <summary>
        /// shapefile中每个Feature对应的几何类型的geojson，包括一个几何类型和空间坐标点
        /// </summary>
        [JilDirective(Name = "geometry")]
        public GeometryGeoJson GeometryGeoJson { get; set; }
        /// <summary>
        ///  属性信息
        /// </summary>
        [JilDirective(Name = "properties")]
        public Dictionary<string,string> Properties { get; set; }
    }
}
