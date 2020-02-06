namespace CoreSpatial.Converter.GeoJSON
{
    /// <summary>
    /// GeoJSON对应的类型
    /// </summary>
    public enum GeoJSONType
    {
        /// <summary>
        /// 类型不在GeoJSON规定的类型中
        /// </summary>
        Unspecified,

        #region 几何对象


        /// <summary>
        /// 点
        /// </summary>
        Point,

        /// <summary>
        /// 多点
        /// </summary>
        MultiPoint,

        /// <summary>
        /// 线
        /// </summary>
        LineString,

        /// <summary>
        /// 多线
        /// </summary>
        MultiLineString,

        /// <summary>
        /// 面
        /// </summary>
        Polygon,

        /// <summary>
        /// 多面
        /// </summary>
        MultiPolygon,

        /// <summary>
        /// GeometryCollection
        /// </summary>
        GeometryCollection,


        #endregion
        
        /// <summary>
        /// 特征对象
        /// </summary>
        Feature,

        /// <summary>
        /// 特征集合对象
        /// </summary>
        FeatureCollection
    }
}
