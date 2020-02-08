using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using CoreSpatial.Converter.GeoJSON;
using CoreSpatial.Converter.KML;

namespace CoreSpatial.Converter
{
    /// <summary>
    /// 文件转换器
    /// </summary>
    public class CoreSpatialConvert
    {
        /// <summary>
        /// 将IFeatureSet转为GeoJSON
        /// </summary>
        /// <param name="prettyPrint">是否输出美观（不压缩），默认false</param>
        /// <returns>GeoJSON字符串</returns>
        public static string ToGeoJSON([NotNull] IFeatureSet featureSet,bool prettyPrint = false)
        {
            CheckFeatureSet(featureSet);
            return GeoJSONBuilder.Build(featureSet, prettyPrint);
        }

        /// <summary>
        /// 检查IFeatureSet是否合格
        /// </summary>
        /// <param name="featureSet"></param>
        private static void CheckFeatureSet(IFeatureSet featureSet)
        {
            if (featureSet == null)
            {
                throw new Exception("shapefile文件为空！");
            }
            if (featureSet.AttrTable == null)
            {
                throw new Exception("shapefile文件缺少属性表！");
            }
        }

        /// <summary>
        /// 将IFeatureSet转为KML
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToKML([NotNull] IFeatureSet featureSet,[NotNull]string name)
        {
            CheckFeatureSet(featureSet);
            var kmlBuilder = new KmlBuilder(featureSet);
            return kmlBuilder.Build(name);
        }

        /// <summary>
        /// 将IFeatureSet转为KMZ
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="kmlName"></param>
        /// <param name="kmzPath"></param>
        /// <returns></returns>
        public static void ToKMZ([NotNull]IFeatureSet featureSet,
            [NotNull]string kmlName, [NotNull]string kmzPath)
        {
            CheckFeatureSet(featureSet);
            var kmlBuilder = new KmlBuilder(featureSet);
            kmlBuilder.BuildKMZ(kmlName, kmzPath);
        }

        /// <summary>
        /// 将IFeatureSet转为KMZ
        /// </summary>
        /// <param name="featureSet"></param>
        /// <param name="kmlName"></param>
        /// <returns>kmz FileStream</returns>
        public static FileStream ToKMZ([NotNull]IFeatureSet featureSet,
            [NotNull]string kmlName)
        {
            var kmzPath = Path.Combine(Path.GetTempPath(),
                $"{DateTime.Now.Ticks.ToString()}_{kmlName}.kmz");
            ToKMZ(featureSet, kmlName, kmzPath);

            return new FileStream(kmzPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// 将KML文件转为KMZ文件
        /// </summary>
        /// <param name="kmlPath">KML文件路径</param>
        /// <param name="kmzPath">KMZ文件路径，若为空，则与KML文件同一目录</param>
        /// <returns></returns>
        public static void KMLToKMZ([NotNull]string kmlPath, string kmzPath = null)
        {
            if (!File.Exists(kmlPath))
            {
                throw new IOException("KML文件不存在");
            }
            if (string.IsNullOrEmpty(kmzPath))
            {
                kmzPath = Path.ChangeExtension(kmzPath, ".kmz");
            }
            if (File.Exists(kmzPath))
            {
                File.Delete(kmzPath);
            }

            var bytes = File.ReadAllBytes(kmlPath);
            try
            {
                using FileStream zipToOpen = new FileStream(kmzPath, FileMode.CreateNew);
                using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create);
                ZipArchiveEntry readmeEntry = archive.CreateEntry("doc.kml");
                using StreamWriter writer = new StreamWriter(readmeEntry.Open());
                writer.Write(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }


}
