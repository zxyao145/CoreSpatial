using System;
using System.Collections.Generic;
using System.Linq;
using CoreSpatial.Analysis.Tyson.Core;
using CoreSpatial.BasicGeometrys;

namespace CoreSpatial.Analysis.Tyson
{
    internal class VoronoiBuilder
    {
        /// <summary>
        /// 获取Voronoi多边形的线
        /// </summary>
        /// <param name="points"></param>
        /// <param name="envelope"></param>
        /// <param name="minDistanceTolerance"></param>
        /// <returns></returns>
        public static List<GraphEdge> BuildLine(List<TysonGeoPoint> points,
            double[] envelope = null, double minDistanceTolerance = 1e-8)
        {
            envelope ??= GetEnvelope(points);

            double[] xVal = new double[points.Count];
            double[] yVal = new double[points.Count];

            var i = 0;
            foreach (var point in points)
            {
                xVal[i] = point.X;
                yVal[i] = point.Y;
                i += 1;
            }
            
            var voronoiObject = new Voronoi(minDistanceTolerance);
            var graphEdgeList = voronoiObject.GenerateVoronoi(xVal, yVal,
                envelope[0], envelope[2],
                envelope[1], envelope[3]);
            var graphEdges = new HashSet<GraphEdge>(graphEdgeList).ToList();
            var index = 0;
            var removeIndexs = new HashSet<int>();
            foreach (var graphEdge in graphEdges)
            {
                var edgeLinePt1 = graphEdge.Start;
                var edgeLinePt2 = graphEdge.End;
                if (edgeLinePt1 == edgeLinePt2)
                {
                    removeIndexs.Add(index);
                }

                index++;
            }


            removeIndexs.Reverse();
            foreach (var i1 in removeIndexs)
            {
                graphEdges.RemoveAt(i1);
            }

            return graphEdges;
        }

        /// <summary>
        /// 获取Voronoi多边形, 返回结果
        /// key: point, 
        /// value: 多边形闭合点集
        /// </summary>
        /// <param name="points"></param>
        /// <param name="envelope"></param>
        /// <param name="minDistanceTolerance"></param>
        /// <returns>
        /// key: point, 
        /// value: 多边形闭合点集
        /// </returns>
        public static Dictionary<TysonGeoPoint, List<TysonGeoPoint>>
            BuildCell(List<GeoPoint> points,
                double[] envelope = null,
                double minDistanceTolerance = 1e-8)
        {
            var tysonPts =
                points.Select(e => new TysonGeoPoint(e)).ToList();

            envelope ??= GetEnvelope(tysonPts);

            var polygonBuilder = new PolygonBuilder();
            var result = polygonBuilder.Build(tysonPts, envelope);

            return result;
        }

        /// <summary>
        /// 获取Voronoi多边形, 返回结果
        /// key: point在<paramref name="points"/>中的索引,
        /// value: 多边形闭合点集
        /// </summary>
        /// <param name="points"></param>
        /// <param name="envelope"></param>
        /// <param name="minDistanceTolerance">误差，默认为1e-8</param>
        /// <returns>
        /// key: point在<paramref name="points"/>中的索引, 
        /// value: 多边形闭合点集
        /// </returns>
        public static Dictionary<int, Polygon>
            BuildCellWithIndex(List<GeoPoint> points,
            double[] envelope = null,
            double minDistanceTolerance = 1e-8)
        {
            var tysonPts =
                points.Select(e => new TysonGeoPoint(e)).ToList();

            envelope ??= GetEnvelope(tysonPts);

            var polygonBuilder = new PolygonBuilder();
            var result = polygonBuilder
                .BuildWithIndex(tysonPts, envelope, minDistanceTolerance);

            var polygonRes = new Dictionary<int, Polygon>();

            foreach (var item in result)
            {
                var geoPoints = new List<GeoPoint>();
                foreach (var tysonGeoPoint in item.Value)
                {
                    geoPoints.Add(tysonGeoPoint);
                }
                PolyLine polyLine = new PolyLine(geoPoints);
                var polygon = new Polygon(polyLine);
                polygonRes.Add(item.Key,polygon);
            }

            return polygonRes;
        }


        private static double[] GetEnvelope(
            List<TysonGeoPoint> allPoints, int envelopeEpsilon = 0)
        {
            
            var firstPoint = allPoints[0];
            double[] envelopeArr = new double[4];
            envelopeArr[0] = firstPoint.X;//min x
            envelopeArr[1] = firstPoint.Y;//min y
            envelopeArr[2] = firstPoint.X;//max x
            envelopeArr[3] = firstPoint.Y;//max y

            foreach (var point in allPoints)
            {
                var curX = point.X;
                var curY = point.Y;

                if (curX < envelopeArr[0])
                {
                    envelopeArr[0] = curX;
                }
                else if (curX > envelopeArr[2])
                {
                    envelopeArr[2] = curX;
                }

                if (curY < envelopeArr[1])
                {
                    envelopeArr[1] = curY;
                }
                else if (curY > envelopeArr[3])
                {
                    envelopeArr[3] = curY;
                }
            }

            double envelopeEpsilonD = envelopeEpsilon;
            if (envelopeEpsilon == 0)
            {
                //var width = envelopeArr[2] - envelopeArr[0];
                //var height = envelopeArr[3] - envelopeArr[1];

                //if (width > height)
                //{
                //    envelopeEpsilon = (int)Math.Abs(Math.Log10(width + 1));
                //}
                //else
                //{
                //    envelopeEpsilon = (int)Math.Abs(Math.Log10(height + 1));
                //}

                envelopeEpsilonD = 0.5;
            }

            envelopeArr[0] = envelopeArr[0] - envelopeEpsilonD;
            envelopeArr[1] = envelopeArr[1] - envelopeEpsilonD;
            envelopeArr[2] = envelopeArr[2] + envelopeEpsilonD;
            envelopeArr[3] = envelopeArr[3] + envelopeEpsilonD;

            return envelopeArr;
        }
    }
}
