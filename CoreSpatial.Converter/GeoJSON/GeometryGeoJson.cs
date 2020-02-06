using Jil;

namespace CoreSpatial.Converter.GeoJSON
{
    /// <summary>
    /// shapefile中每个Feature对应的几何类型实体类，包括一个几何类型和空间坐标点
    /// </summary>
    public class GeometryGeoJson
    {
        /// <summary>
        /// 本构造函数是为了适应与Jil类库进行序列化。如果使用此构造函数，需要在创建实例后制定Type的类型，否则默认的Type为GeoJSONType.Point。
        /// </summary>
        public GeometryGeoJson()
        {

        }

        /// <summary>
        /// 创建GeometryGeoJson对象，并对Type进行赋值
        /// </summary>
        /// <param name="type"></param>
        public GeometryGeoJson(GeoJSONType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// GeoJSON的类型，对于shapefile中种几何对象，有种，包括点、多点、线、多线、面
        /// </summary>
        [JilDirective(Name = "type")]
        public GeoJSONType Type { get; set; } = GeoJSONType.Point;
        /// <summary>
        /// 坐标点
        /// </summary>
        [JilDirective(Name = "coordinates")]
        public dynamic Coordinates { get; set; }
    }
}
