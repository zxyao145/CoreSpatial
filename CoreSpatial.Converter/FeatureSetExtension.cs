using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Text;
using CoreSpatial.Converter.GeoJSON;
using CoreSpatial.Converter.KML;

namespace CoreSpatial.Converter
{
    public static class FeatureSetExtension
    {
        /// <summary>
        /// 将IFeatureSet转为GeoJSON
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="prettyPrint">是否输出美观（不压缩），默认false</param>
        /// <returns></returns>
        public static string ToGeoJSON(this IFeatureSet featureSet, bool prettyPrint = false)
        {
            return CoreSpatialConvert.ToGeoJSON(featureSet);
        }

        /// <summary>
        /// 将IFeatureSet转为Kml
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToKML([NotNull]this IFeatureSet featureSet, [NotNull]string name)
        {
            return CoreSpatialConvert.ToKML(featureSet, name);
        }

        /// <summary>
        /// 将IFeatureSet转为KMZ
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="kmlName"></param>
        /// <param name="kmzPath"></param>
        public static void ToKMZ([NotNull]this IFeatureSet featureSet,
            [NotNull]string kmlName, [NotNull]string kmzPath)
        {
            CoreSpatialConvert.ToKMZ(featureSet, kmlName, kmzPath);
        }

        /// <summary>
        /// 将IFeatureSet转为KMZ，并返回KMZ的文件流
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="kmlName"></param>
        /// <returns>KMZ的文件流</returns>
        public static FileStream ToKMZ([NotNull]this IFeatureSet featureSet,
            [NotNull] string kmlName)
        {
            return CoreSpatialConvert.ToKMZ(featureSet, kmlName);
        }
    }
}
