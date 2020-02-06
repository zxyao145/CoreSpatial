namespace CoreSpatial.Converter
{
    /// <summary>
    /// shapefile文件转换器的借口
    /// </summary>
    public interface ICoreSpatialConvert
    {
        /// <summary>
        /// 将shapefile文件转为GeoJSON
        /// </summary>
        /// <param name="prettyPrint">是否输出美观（不压缩），默认false</param>
        /// <returns>GeoJSON字符串</returns>
        string ToGeoJSON(bool prettyPrint = false);
    }
}
