using System;
using System.Collections.Generic;
using CoreSpatial.BasicGeometrys;

namespace CoreSpatial.Converter.GeoJSON.CoordinatesOper
{
    /// <summary>
    /// 三维的数组，针对多线和面
    /// </summary>
    internal class Coord3 : Coord2
    {
        /// <summary>
        /// 获取多线的坐标点
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        internal new List<List<List<double>>> GetCoords(IFeature feature)
        {
            List<List<List<double>>> coordinates = new List<List<List<double>>>();
            var geometry = feature.Geometry;

            if (geometry.GeometryType  == GeometryType.MultiPolyLine)
            {
                var muiltLineString = (MultiPolyLine) geometry.BasicGeometry;

                for (int i = 0; i < muiltLineString.PartsNum; i++)
                {
                    var oneline = muiltLineString[i];
                    if (oneline != null)
                    {
                        IFeature ife = new Feature(new Geometry(oneline));
                        coordinates.Add(base.GetCoords(ife));
                    }
                }
            }
            
            return coordinates;
        }

        /// <summary>
        /// 获取面的坐标点
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        internal new List<List<List<double>>> GetClosedCoords(IFeature feature)
        {
            var polygon = feature.Geometry.BasicGeometry as Polygon;
            if (polygon == null)
            {
                return null;
            }

            List<List<List<double>>> coordinates;
            if (polygon.PartsNum == 1)
            {
                coordinates = new List<List<List<double>>>()
                {
                    base.GetClosedCoords(feature)
                };
            }
            else
            {
                //带有洞的复杂面
                var lines = polygon.PolyLines;

                coordinates = new List<List<List<double>>>();
                foreach (var polyLine in lines)
                {
                    var coords = base.GetClosedCoords(new Feature(new Geometry(polyLine)));
                    coordinates.Add(coords);
                }
            }
           
            return coordinates;
        }
        /// <summary>
        /// 获取带有洞的复杂面的线点
        /// </summary>
        /// <param name="geometryStr"></param>
        /// <returns></returns>
        private List<List<Coordinate>> GetLinesCoords(string geometryStr)
        {
            var lineList = new List<List<Coordinate>>();
            string[] lineItem = geometryStr.Split(new string[] { "), (" }, StringSplitOptions.RemoveEmptyEntries);
            if (lineItem.Length>0)
            {
                foreach (var line in lineItem)
                {
                    List<Coordinate> oneLine = new List<Coordinate>();
                    var pointStr = line.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var point in pointStr)
                    {
                        string[] xy = point.Split(new char[] { ' ' });
                        if (xy.Length == 2)
                        {
                            Coordinate coord = new Coordinate()
                            {
                                X = Convert.ToDouble(xy[0]),
                                Y = Convert.ToDouble(xy[1]),
                            };
                            oneLine.Add(coord);
                        }
                        else if (xy.Length == 3)
                        {
                            Coordinate coord = new Coordinate()
                            {
                                X = Convert.ToDouble(xy[0]),
                                Y = Convert.ToDouble(xy[1]),
                                Z = Convert.ToDouble(xy[2])
                            };
                            oneLine.Add(coord);
                        }
                    }
                    if (oneLine.Count > 0)
                    {
                        lineList.Add(oneLine);
                    }
                }
            }
            return lineList;
        }
    }
}
