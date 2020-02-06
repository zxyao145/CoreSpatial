using System;
using CoreSpatial.Converter.GeoJSON;

namespace CoreSpatial.Converter
{
    /// <summary>
    /// shapefile文件转换器
    /// </summary>
    public class CoreSpatialConvert : ICoreSpatialConvert
    {
        /// <summary>
        /// 通过shapefile对应的IFeatureSet对象，创建一个ShpConvert实例
        /// </summary>
        /// <param name="fs">shapefile对应的IFeatureSet对象</param>
        public CoreSpatialConvert(IFeatureSet fs)
        {
            this.Fs = fs;
        }

        /// <summary>
        /// 创建一个ShpConvert实例
        /// </summary>
        public CoreSpatialConvert()
        {

        }
        /// <summary>
        /// shapefile对应的IFeatureSet对象
        /// </summary>
        public IFeatureSet Fs { get; set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="prettyPrint"></param>
        /// <returns></returns>
        public string ToGeoJSON(bool prettyPrint = false)
        {
            CheckFs();
            ToGeoJson toGeoJson = new ToGeoJson();
            return toGeoJson.GetGeoJson(Fs, prettyPrint);
        }

        private void CheckFs()
        {
            if (Fs == null)
            {
                throw new Exception("shapefile文件为空！");
            }
            if (Fs.AttrTable == null)
            {
                throw new Exception("shapefile文件缺少属性表！");
            }
        }
    }

    
}
