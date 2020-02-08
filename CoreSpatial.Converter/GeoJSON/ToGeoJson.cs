using System.Collections.Generic;
using CoreSpatial.Converter.Extension;
using Jil;

namespace CoreSpatial.Converter.GeoJSON
{
    /// <summary>
    /// 将shapefile转成GeoJSON的具体类
    /// </summary>
    internal class GeoJSONBuilder
    {
        internal static string Build(IFeatureSet fs, bool prettyPrint = false)
        {
            if (fs.Features.Count==0)
            {
                return GeoJSON.Empty;
            }
            //else
            
            var type = fs.Features[0].GetGeoJsonType();
            if (type == GeoJSONType.Unspecified)
            {
                return string.Empty;
            }
            //else

            GeoJSON geoJson = new GeoJSON
            {
                Bbox = fs.GetBbox()
            };
            var features = new List<FeatureGeoJson>();

            //创建该feature对应的GeometryGeoJson对象
            foreach (var feature in fs.Features)
            {
                if (feature != null)
                {
                    var basicGeometry = feature.Geometry.BasicGeometry;

                    if (basicGeometry == null || basicGeometry.PointsNum == 0)
                    {
                        continue;
                    }
                    //else

                    
                    //获取该feature的坐标点
                    var coords = feature.GetCoordinates(type);
                    if (coords == null)
                    {
                        continue;
                    }
                    //else

                    //获取该feature的属性信息
                    var properties = feature.GetPropertieJson();
                    GeometryGeoJson geometryGeoJson = new GeometryGeoJson(type)
                    {
                        Coordinates = coords
                    };
                    FeatureGeoJson featureGeoJson = new FeatureGeoJson()
                    {
                        GeometryGeoJson = geometryGeoJson,
                        Properties = properties
                    };
                    features.Add(featureGeoJson);
                }
            }
            geoJson.Features = features;
            return JSON.SerializeDynamic(geoJson,new Options(prettyPrint: prettyPrint));
        }
    }
}
