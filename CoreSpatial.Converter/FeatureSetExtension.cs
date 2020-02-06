using System;
using System.Collections.Generic;
using System.Text;
using CoreSpatial.Converter.GeoJSON;

namespace CoreSpatial.Converter
{
    public static class FeatureSetExtension
    {
        /// <summary>
        /// 将shapefile文件转为GeoJSON
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="prettyPrint">是否输出美观（不压缩），默认false</param>
        /// <returns></returns>
        public static string ToGeoJSON(this IFeatureSet featureSet, bool prettyPrint = false)
        {
            var coreSpatialConvert = new CoreSpatialConvert(featureSet);
            return coreSpatialConvert.ToGeoJSON();
        }
    }
}
