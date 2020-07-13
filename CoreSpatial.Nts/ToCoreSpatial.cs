using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        public static CoreSpatial.BasicGeometrys.GeoPoint
            TcsGeometry(this Point point)
        {
            return point.Coordinate.TcsGeometry();
        }

        public static CoreSpatial.BasicGeometrys.GeoPoint
            TcsGeometry(this Coordinate coordinate)
        {
            if (double.IsNaN(coordinate.Z))
            {
                var gp = new GeoPoint(coordinate.X, coordinate.Y);
                return gp;
            }
            else
            {
                var gp = new GeoPoint(coordinate.X, coordinate.Y, coordinate.Z);
                return gp;
            }
        }

        public static CoreSpatial.BasicGeometrys.MultiPoint
            TcsGeometry(this MultiPoint multiPoint)
        {
            var goePoints = multiPoint.Coordinates.Select(TcsGeometry).ToList();

            var result = new CoreSpatial.BasicGeometrys.MultiPoint(goePoints);

            return result;
        }


        public static CoreSpatial.BasicGeometrys.PolyLine
            TcsGeometry(this LineString lineString)
        {
            var goePoints = lineString.Coordinates.Select(TcsGeometry).ToList();
            var polyLine = new PolyLine(goePoints);

            return polyLine;
        }

        public static CoreSpatial.BasicGeometrys.PolyLine
            TcsGeometry(this LinearRing lineString)
        {
            var goePoints = lineString.Coordinates.Select(TcsGeometry).ToList();
            var polyLine = new PolyLine(goePoints);

            return polyLine;
        }

        

        public static CoreSpatial.BasicGeometrys.MultiPolyLine
            TcsGeometry(this MultiLineString multiLineString)
        {
            var lines = new List<CoreSpatial.BasicGeometrys.PolyLine>();

            foreach (var lineGeom in multiLineString)
            {
                var lineNum = lineGeom.NumGeometries;

                for (int i = 0; i < lineNum; i++)
                {
                    var line = lineGeom.GetGeometryN(i).Coordinates
                        .Select(TcsGeometry).ToList();

                    var polyLine = new PolyLine(line);
                    lines.Add(polyLine);
                }
            }

            MultiPolyLine res = new MultiPolyLine(lines);
            return res;
        }

        public static CoreSpatial.BasicGeometrys.Polygon
            TcsGeometry(this Polygon polygon)
        {
            var shell = polygon.Shell.TcsGeometry();
            var holes = polygon.Holes.Select(TcsGeometry).ToList();

            var result =
                new CoreSpatial.BasicGeometrys.Polygon(shell, holes);

            return result;
        }



        private static CoreSpatial.IFeature ToCsFeature(this NetTopologySuite.Features.IFeature feature)
        {
            var originalGeom = feature.Geometry;
            IBasicGeometry basicGeometry = null;
            switch (originalGeom.OgcGeometryType)
            {
                case OgcGeometryType.Point:
                    {
                        basicGeometry = ((Point)originalGeom).TcsGeometry();
                        break;
                    }

                case OgcGeometryType.MultiPoint:
                    {
                        basicGeometry = ((MultiPoint)originalGeom).TcsGeometry();
                        break;
                    }
                case OgcGeometryType.LineString:
                    {
                        if (originalGeom.GeometryType == "LinearRing")
                        {
                            basicGeometry = ((LinearRing)originalGeom).TcsGeometry();
                        }
                        else
                        {
                            basicGeometry = ((LineString)originalGeom).TcsGeometry();
                        }
                        break;
                    }
                case OgcGeometryType.MultiLineString:
                    {
                        basicGeometry = ((MultiLineString)originalGeom).TcsGeometry();

                        break;
                    }
                case OgcGeometryType.Polygon:
                    {
                        basicGeometry = ((Polygon)originalGeom).TcsGeometry();
                        break;
                    }
                default:
                    throw new Exception("not support GeometryType: " + originalGeom.GeometryType);
            }

            IGeometry geometry = new Geometry(basicGeometry);
            var result = new Feature(geometry);
            return result;
        }

        public static FeatureSet ToCsFeatureSet(this FeatureCollection featureCollection)
        {
            if (featureCollection.Count < 1)
            {
                throw new Exception($"{nameof(featureCollection)} does not contain any feature");
            }

            var firstFeature = featureCollection[0];
            FeatureType featureType = FeatureType.NullShape;
            switch (firstFeature.Geometry.OgcGeometryType)
            {
                case OgcGeometryType.Point:
                    featureType = FeatureType.Point;
                    break;
                case OgcGeometryType.MultiPoint:
                    featureType = FeatureType.MultiPoint;
                    break;
                case OgcGeometryType.LineString:
                case OgcGeometryType.MultiLineString:
                    featureType = FeatureType.PolyLine;
                    break;
                case OgcGeometryType.Polygon:
                    featureType = FeatureType.Polygon;
                    break;
                default:
                    throw new Exception("not support OgcGeometryType");
            }
            
            var fs = new FeatureSet(featureType);
            List<IFeature> features = new List<IFeature>(); 
            var attrTable  = new DataTable();
            var columns = firstFeature.Attributes.GetNames();
            foreach (var column in columns)
            {
                //var field = firstFeature.Attributes[column];
                var val = firstFeature.Attributes[column];
                var getType = val.GetType();
                attrTable.Columns.Add(column, getType);
            }

            foreach (var feature in featureCollection)
            {
                features.Add(feature.ToCsFeature());

                var newRow = attrTable.NewRow();
                foreach (var column in columns)
                {
                    var val = feature.Attributes[column];
                    var getType = val.GetType();
                    newRow[column] = val;
                }

                attrTable.Rows.Add(newRow);
            }

            fs.Features.AddRange(features);
            fs.AttrTable = attrTable;
            return fs;
        }


    }
}
