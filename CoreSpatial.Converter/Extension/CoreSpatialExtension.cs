using System.Collections.Generic;
using System.Data;
using CoreSpatial.Converter.GeoJSON;
using CoreSpatial.Converter.GeoJSON.CoordinatesOper;

namespace CoreSpatial.Converter.Extension
{
    internal static class CoreSpatialExtension
    {

        /// <summary>
        /// 获取shapefile的边界信息
        /// </summary>
        /// <param name="fs">shapefile对应的IFeatureSet对象</param>
        /// <returns></returns>
        internal static List<double> GetBbox(this IFeatureSet fs)
        {
            var extent = fs.Envelope;
            List<double> bboxList = new List<double>
            {
                extent.MinX,
                extent.MinY,
                extent.MaxX,
                extent.MaxY
            };
            return bboxList;
        }

        /// <summary>
        /// 获取当前这一个feature的propertie对象
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> GetPropertieJson(this IFeature feature)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            DataRow dr = feature.DataRow;
            if (dr == null)
            {
                return dict;
            }
            var cols = dr.Table.Columns;
            var dataArr = dr.ItemArray;

            for (int i = 0; i < cols.Count; i++)
            {
                string key = cols[i].ColumnName;

                if (dict.ContainsKey(key))
                {
                    dict[key] = dataArr[i].ToString();
                }

                else
                {
                    dict.Add(key, dataArr[i].ToString());

                }
            }
            return dict;
        }

        /// <summary>
        /// 获取IFeature对应的GeoJSON类型
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        internal static GeoJSONType GetGeoJsonType(this IFeature feature)
        {
            GeoJSONType returnGeoJsonType = GeoJSONType.Unspecified;
            var featureType = feature.GeometryType;
            if (featureType == GeometryType.Point)
            {
                returnGeoJsonType = GeoJSONType.Point;
            }
            else if (featureType == GeometryType.MultiPoint)
            {
                returnGeoJsonType = GeoJSONType.MultiPoint;
            }
            else if (featureType == GeometryType.PolyLine)
            {
                returnGeoJsonType = GeoJSONType.LineString;
            }
            else if (featureType == GeometryType.MultiPolyLine)
            {
                returnGeoJsonType =  GeoJSONType.MultiLineString;
            }
            else if (featureType == GeometryType.Polygon)
            {
                returnGeoJsonType = GeoJSONType.Polygon;
            }

            return returnGeoJsonType;
        }

        /// <summary>
        /// 获取几何对象的坐标点信息
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static dynamic GetCoordinates(this IFeature feature, GeoJSONType type)
        {
            dynamic coords = null;
            switch (type)
            {
                case GeoJSONType.Point:
                    Coord1 coord1 = new Coord1();
                    coords = coord1.GetCoords(feature);
                    break;
                case GeoJSONType.MultiPoint:
                case GeoJSONType.LineString:
                    Coord2 coord2 = new Coord2();
                    coords = coord2.GetCoords(feature);
                    break;
                case GeoJSONType.MultiLineString:
                    Coord3 coord31 = new Coord3();
                    coords = coord31.GetCoords(feature);
                    break;
                case GeoJSONType.Polygon:
                    Coord3 coord32 = new Coord3();
                    coords = coord32.GetClosedCoords(feature);
                    break;
                default:
                    return null;
            }
            return coords;
        }
       
    }
}
