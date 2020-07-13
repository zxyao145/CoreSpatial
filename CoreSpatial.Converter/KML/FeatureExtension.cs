using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using CoreSpatial.BasicGeometrys;

namespace CoreSpatial.Converter.KML
{
    internal class KmlUtil
    {
        public static bool IsMultiGeometry(GeometryType geometryType)
        {
            return geometryType == GeometryType.MultiPoint 
                   || geometryType == GeometryType.MultiPolyLine;
        }

        public static List<XElement> GetCoordinates(IBasicGeometry basicGeometry)
        {
            var geometryType = basicGeometry.GeometryType;
            List<XElement> result = null;
            XElement coordinatesRoot = null;
            switch (geometryType)
            {
                case GeometryType.MultiPoint:
                {
                    result = new List<XElement>();
                        var multiPoint = (MultiPoint)basicGeometry;
                        foreach (var point in multiPoint.Points)
                        {
                            var strText = GetText(point);
                            var coordinatesEleTemp = new XElement("coordinates");
                            coordinatesEleTemp.SetValue(strText);
                            var pointEle = new XElement("Point");
                            pointEle.Add(coordinatesEleTemp);
                            result.Add(pointEle);
                        }
                        break;
                    }

                case GeometryType.MultiPolyLine:
                    {
                        result = new List<XElement>();
                        var multiPolyLine = (MultiPolyLine)basicGeometry;
                        foreach (var polyLine in multiPolyLine.PolyLines)
                        {
                            result.AddRange(GetCoordinates(polyLine));
                        }
                        break;
                    }
                case GeometryType.Point:
                    coordinatesRoot = new XElement("Point");
                    goto case GeometryType.PolyLine;
                case GeometryType.PolyLine:
                    {
                        if (coordinatesRoot == null)
                        {
                            var polyLine = (PolyLine)basicGeometry;
                            coordinatesRoot =
                                polyLine.IsLineRing 
                                    ? new XElement("LinearRing") 
                                    : new XElement("LineString");
                        }
                        result = new List<XElement>(1);
                        var coords = basicGeometry.Coordinates;
                        var points = new List<string>();
                        foreach (var coordinate in coords)
                        {
                            points.Add(GetText(coordinate));
                        }
                        var coordinatesEle = new XElement("coordinates");
                        coordinatesEle.SetValue(string.Join(" ", points));
                        coordinatesRoot.Add(coordinatesEle);
                        result.Add(coordinatesRoot);
                        break;
                    }

                case GeometryType.Polygon:
                    {
                        result = new List<XElement>();
                        var polygon = (Polygon)basicGeometry;
                        var lines = polygon.PolyLines;
                        var outerBoundaryIsEle = new XElement("outerBoundaryIs");
                        var innerBoundaryIsEle = new XElement("innerBoundaryIs");
                        foreach (var polyLine in lines)
                        {
                            var lineCoords = GetCoordinates(polyLine);

                            if (polyLine.ClockDirection == ClockDirection.Clockwise)
                            {
                                foreach (var lineCoord in lineCoords)
                                {
                                    outerBoundaryIsEle.Add(lineCoord);
                                }
                            }
                            else
                            {
                                foreach (var lineCoord in lineCoords)
                                {
                                    innerBoundaryIsEle.Add(lineCoord);
                                }
                            }
                        }
                        
                        var polygonEle = new XElement("Polygon");
                        polygonEle.Add(outerBoundaryIsEle);
                        polygonEle.Add(innerBoundaryIsEle);

                        result.Add(polygonEle);
                        break;
                    }
                default:
                    throw new Exception("Not Support Geometry Type");
            }
            
            return result;
        }

        private static string GetText(GeoPoint point)
        {
            return GetText(point.Coordinate);
        }

        private static string GetText(Coordinate coordinate)
        {
            if (double.IsNaN(coordinate.Z))
            {
                return $"{coordinate.X},{coordinate.Y}";
            }
            return $"{coordinate.X},{coordinate.Y},{coordinate.Z}";
        }

    }

}
