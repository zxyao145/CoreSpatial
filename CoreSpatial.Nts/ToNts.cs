using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CoreSpatial.BasicGeometrys;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Coordinate = NetTopologySuite.Geometries.Coordinate;
using MultiPoint = NetTopologySuite.Geometries.MultiPoint;
using Polygon = NetTopologySuite.Geometries.Polygon;

namespace CoreSpatial.Nts
{
    public static partial class Convert
    {
        public static Coordinate
            ToNtsCoordinate(this CoreSpatial.BasicGeometrys.Coordinate coordinate)
        {
            var coord = new Coordinate(coordinate.X, coordinate.Y);

            if (!double.IsNaN(coordinate.Z))
            {
                coord.Z = coordinate.Z;
            }

            return coord;
        }

        public static Coordinate
            ToNtsCoordinate(this GeoPoint point)
        {
            return ToNtsCoordinate(point.Coordinate);
        }

        public static Point
            ToNtsPoint(this GeoPoint point)
        {
            if (double.IsNaN(point.Coordinate.Z))
            {
                return new Point(point.X, point.Y);
            }
            else
            {
                return new Point(point.X, point.Y, point.Coordinate.Z);
            }
        }

        public static MultiPoint ToNtsMultiPoint(this BasicGeometrys.MultiPoint multiPoint)
        {
            var pointArr = multiPoint.Points.Select(ToNtsPoint).ToArray();
            return new MultiPoint(pointArr);
        }

        public static LineString ToNtsLineString(this PolyLine polyLine)
        {
            if (polyLine.IsLineRing)
            {
                throw new Exception("this is a line ring object");
            }

            var coordinateArr = polyLine.Points.Select(ToNtsCoordinate).ToArray();

            return new LineString(coordinateArr);
        }

        public static LinearRing ToNtsLinearRing(this PolyLine polyLine)
        {
            if (!polyLine.IsLineRing)
            {
                throw new Exception("this is not a line ring object");
            }

            var coordinateArr = polyLine.Points.Select(ToNtsCoordinate).ToArray();

            return new LinearRing(coordinateArr);
        }


        public static MultiLineString ToNtsMultiLineString
            (this CoreSpatial.BasicGeometrys.MultiPolyLine multiPolyLine)
        {
            if (multiPolyLine.PartsNum < 1)
            {
                throw new Exception($"{nameof(multiPolyLine)} does not contain PolyLine part");
            }
            var isLingRing = multiPolyLine.PolyLines[0].IsLineRing;
            LineString[] linestringArr = null;
            if (isLingRing)
            {
                linestringArr = multiPolyLine.PolyLines.Select(e =>
                        (LineString)ToNtsLinearRing(e))
                    .ToArray();
            }
            else
            {
                linestringArr = multiPolyLine.PolyLines.Select(ToNtsLineString)
                    .ToArray();
            }

            MultiLineString result = new MultiLineString(linestringArr);
            return result;
        }


        public static Polygon ToNtsPolygon(this CoreSpatial.BasicGeometrys.Polygon polygon)
        {
            var edges = polygon.PolyLines.Select(ToNtsLinearRing).ToArray();
            if (edges.Length > 1)
            {
                return new Polygon(edges[0], edges.Skip(1).ToArray());
            }
            return new Polygon(edges[0]);
        }


        private static List<string> GetFieldNames(this IFeatureSet featureSet)
        {
            var fields = new List<string>();
            var originalColumns = featureSet.AttrTable.Columns;

            foreach (DataColumn originalColumn in originalColumns)
            {
                fields.Add(originalColumn.ColumnName);
            }

            return fields;
        }

        private static IAttributesTable ToNtsAttributeTable(this CoreSpatial.IFeature feature, List<string> fieldNames)
        {
            var attrDict = new Dictionary<string, object>();
            var dataRow = feature.DataRow;
            for (int i = 0; i < fieldNames.Count; i++)
            {
                var val = dataRow[i];
                attrDict.Add(fieldNames[i], val);
            }

            IAttributesTable result = new AttributesTable(attrDict);

            return result;
        }

        private static NetTopologySuite.Features.Feature
            ToNtsFeature(this CoreSpatial.IFeature feature, List<string> fieldNames)
        {
            NetTopologySuite.Geometries.Geometry geometry = null;
            var geo = feature.Geometry;
            switch (geo.GeometryType)
            {
                case GeometryType.Point:
                    {
                        var basicGeometry = (GeoPoint)geo.BasicGeometry;
                        geometry = (NetTopologySuite.Geometries.Geometry)ToNtsPoint(basicGeometry);
                        break;
                    }
                case GeometryType.MultiPoint:
                    {
                        var basicGeometry = (CoreSpatial.BasicGeometrys.MultiPoint)geo.BasicGeometry;
                        geometry = ToNtsMultiPoint(basicGeometry);
                        break;
                    }
                case GeometryType.PolyLine:
                    {
                        var basicGeometry = (CoreSpatial.BasicGeometrys.PolyLine)geo.BasicGeometry;
                        geometry = basicGeometry.IsLineRing ? ToNtsLinearRing(basicGeometry) : ToNtsLineString(basicGeometry);
                        break;
                    }
                case GeometryType.MultiPolyLine:
                {
                    var basicGeometry = (CoreSpatial.BasicGeometrys.MultiPolyLine)geo.BasicGeometry;
                    geometry = ToNtsMultiLineString(basicGeometry);
                    break;
                }
                case GeometryType.Polygon:
                {
                    var basicGeometry = (CoreSpatial.BasicGeometrys.Polygon)geo.BasicGeometry;
                    geometry = ToNtsPolygon(basicGeometry);
                    break;
                }
                default:
                    throw new Exception("not support GeometryType");
            }

            var attrTable = feature.ToNtsAttributeTable(fieldNames);
            var result = new NetTopologySuite.Features.Feature(geometry, attrTable);
            return result;
        }

        public static NetTopologySuite.Features.FeatureCollection ToNtsFeatureCollection(this IFeatureSet featureSet)
        {
            var fieldNames = GetFieldNames(featureSet);

            FeatureCollection fc = new FeatureCollection();
            foreach (var featureSetFeature in featureSet.Features)
            {
                fc.Add(featureSetFeature.ToNtsFeature(fieldNames));
            }
            return fc;
        }
    }
}
