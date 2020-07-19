using CoreSpatial.BasicGeometrys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


/// <summary>
/// deatils see https://github.com/AndriiHeonia/hull/
/// </summary>
namespace CoreSpatial.Analysis.Convex
{
    internal class ConvexAnalysis
    {
        // angle = 90 deg
        private static double MaxConvaveAngleCos = Math.Cos(90 / (180 / Math.PI));
        private const double MaxSearchExtentSizePercent = 0.6;

        public IFeatureSet Build(IEnumerable<Coordinate> points,
            int concavity = 20)
        {
            var pointList = points.OrderBy(e => e.X).Distinct().ToList();

            if (pointList.Count < 4)
            {
                return CreatePolygon(pointList);
            }

            var wh = GetWh(pointList);

            var searchArea = new double[] {
                wh[0] * MaxSearchExtentSizePercent,
                wh[1] * MaxSearchExtentSizePercent };


            var convex = ConvexHull(pointList);
            var innerPoints = pointList.Where(pt => convex.Contains(pt));

            var cellSize = Math.Ceiling(1 / (pointList.Count / (wh[0] * wh[1])));

            var concave = _concave(
                convex, Math.Pow(concavity, 2),
                searchArea, new Grid(innerPoints, cellSize), new HashSet<string>());

            return CreatePolygon(concave);
        }

        private IFeatureSet CreatePolygon(List<Coordinate> coordinates)
        {
            var fs = new FeatureSet(FeatureType.Polygon);
            //TODO
            fs.Features.Add(
                new Feature(
                    new Geometry(
                        new Polygon(
                            new PolyLine(
                                coordinates.Select(e => new GeoPoint(e.X, e.Y)).ToList()
                                )
                            )
                        )
                    )
                );
            fs.AttrTable = new DataTable();
            fs.AttrTable.Rows.Add(fs.AttrTable.NewRow());

            return fs;
        }

        private double[] GetWh(List<Coordinate> pointList)
        {
            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var maxX = double.MinValue;
            var maxY = double.MinValue;
            foreach (var point in pointList)
            {
                var curX = point.X;
                var curY = point.Y;
                if(curX < minX)
                {
                    minX = curX;
                }
                else if(curX > maxX)
                {
                    maxX = curX;
                }

                if (curY < minY)
                {
                    minY = curY;
                }
                else if (curY > maxY)
                {
                    maxY = curY;
                }
            }
            return new []
            {
                maxX - minX,maxY - minY
            };
        }

        #region Core

        private List<Coordinate> ConvexHull(List<Coordinate> pointList)
        {
            var upper = UpperTangent(pointList);
            var lower = LowerTangent(pointList);
            lower.AddRange(upper);
            lower.Add(pointList[0]);
            return lower;
        }

        private double Cross(Coordinate o, Coordinate a, Coordinate b)
        {
            return (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);
        }

        private List<Coordinate> UpperTangent(List<Coordinate> pointset)
        {
            List<Coordinate> lower = new List<Coordinate>();
            foreach (var item in pointset)
            {
                var lowerCount = lower.Count;
                while (lowerCount >= 2 &&
                    (Cross(lower[lowerCount - 2], lower[lowerCount - 1], item) <= 0))
                {
                    lower.RemoveAt(lowerCount - 1);
                    lowerCount -= 1;
                }
                lower.Add(item);
            }

            lower.RemoveAt(lower.Count - 1);
            return lower;
        }

        private List<Coordinate> LowerTangent(List<Coordinate> pointset)
        {
            pointset.Reverse();
            List<Coordinate> upper = new List<Coordinate>();

            foreach (var item in pointset)
            {
                var upperCount = upper.Count;
                while (upperCount >= 2 && (Cross(upper[upperCount - 2], upper[upperCount - 1], item) <= 0))
                {
                    upper.RemoveAt(upperCount - 1);
                    upperCount -= 1;
                }
                upper.Add(item);
            }

            upper.RemoveAt(upper.Count - 1);
            return upper;
        }

        #endregion

        #region Intersect

        private bool Ccw(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            var cw = ((y3 - y1) * (x2 - x1)) - ((y2 - y1) * (x3 - x1));
            return cw > 0 ? true : cw < 0 ? false : true; // colinear
        }

        private bool Intersect(Coordinate[] seg1, Coordinate[] seg2)
        {
            double x1 = seg1[0].X, y1 = seg1[0].Y,
                x2 = seg1[1].X, y2 = seg1[1].Y,
                x3 = seg2[0].X, y3 = seg2[0].Y,
                x4 = seg2[1].X, y4 = seg2[1].Y;

            return Ccw(x1, y1, x3, y3, x4, y4) != Ccw(x2, y2, x3, y3, x4, y4) 
                && Ccw(x1, y1, x2, y2, x3, y3) != Ccw(x1, y1, x2, y2, x4, y4);
        } 
       
        #endregion


        private double SqLength(Coordinate a, Coordinate b)
        {
            return Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2);
        }

        private double Cos(Coordinate o, Coordinate a, Coordinate b)
        {
            double[] aShifted =new double[] { a.X - o.X, a.Y - o.Y },
                bShifted = new double[] { b.X - o.X, b.Y - o.Y };

            double sqALen = SqLength(o, a),
                sqBLen = SqLength(o, b);

            double dot = aShifted[0] * bShifted[0] + aShifted[1] * bShifted[1];

            return dot / Math.Sqrt(sqALen * sqBLen);
        }

        private bool IntersectCheck(Coordinate[] segment, List<Coordinate> pointset)
        {
            var maxCount = pointset.Count - 1;
            for (var i = 0; i < maxCount; i++)
            {
                var seg = new Coordinate[] { pointset[i], pointset[i + 1] };

                if (segment[0] == seg[0] || segment[0] == segment[1])
                {
                    continue;
                }
                if (Intersect(segment, seg))
                {
                    return true;
                }
            }
            return false;
        }

        private Coordinate MidPoint(Coordinate[] edge, List<Coordinate> innerPoints, List<Coordinate> convex)
        {
            Coordinate point = null;
            var angle1Cos = MaxConvaveAngleCos;
            var angle2Cos = MaxConvaveAngleCos;
            double a1Cos, a2Cos;

            for (var i = 0; i < innerPoints.Count; i++)
            {
                a1Cos = Cos(edge[0], edge[1], innerPoints[i]);
                a2Cos = Cos(edge[1], edge[0], innerPoints[i]);

                if (a1Cos > angle1Cos && a2Cos > angle2Cos &&
                    !IntersectCheck(new Coordinate[] { edge[0], innerPoints[i] }, convex) &&
                    !IntersectCheck(new Coordinate[] { edge[1], innerPoints[i] }, convex))
                {

                    angle1Cos = a1Cos;
                    angle2Cos = a2Cos;
                    point = innerPoints[i];
                }
            }

            return point;
        }

        private double[] BBoxAround(Coordinate[] edge)
        {
            return new double[]{ 
                Math.Min(edge[0].X, edge[1].X), // left
                Math.Min(edge[0].Y, edge[1].Y), // top
                Math.Max(edge[0].X, edge[1].X), // right
                Math.Max(edge[0].Y, edge[1].Y)  // bottom
            };
        }

        private List<Coordinate> _concave(List<Coordinate>  convex, 
            double maxSqEdgeLen, double[] maxSearchArea, Grid grid, HashSet<string> edgeSkipList)
        {
            var midPointInserted = false;


            var convexCount = convex.Count - 1;

            for (var i = 0; i < convexCount; i++)
            {
                var edge = new Coordinate[] { convex[i], convex[i + 1] };
                // generate a key in the format X0,Y0,X1,Y1
                string keyInSkipList = edge[0].X + "," + edge[0].Y + "," + edge[1].X + "," + edge[1].Y;

                if (SqLength(edge[0], edge[1]) < maxSqEdgeLen ||
                    edgeSkipList.Contains(keyInSkipList)) { continue; }

                var scaleFactor = 0;
                var bBoxAround = BBoxAround(edge);
                double bBoxWidth;
                double bBoxHeight;
                Coordinate midPoint;
                do
                {
                    bBoxAround = grid.ExtendBbox(bBoxAround, scaleFactor);
                    bBoxWidth = bBoxAround[2] - bBoxAround[0];
                    bBoxHeight = bBoxAround[3] - bBoxAround[1];

                    midPoint = MidPoint(edge, grid.RangePoints(bBoxAround), convex);
                    scaleFactor++;
                } while (midPoint == null && (maxSearchArea[0] > bBoxWidth || maxSearchArea[1] > bBoxHeight));

                if (bBoxWidth >= maxSearchArea[0] && bBoxHeight >= maxSearchArea[1])
                {
                    edgeSkipList.Add(keyInSkipList);
                }

                if (midPoint != null)
                {
                    convex.Insert(i + 1, midPoint);
                    grid.RemovePoint(midPoint);
                    midPointInserted = true;
                }
            }

            if (midPointInserted)
            {
                return _concave(convex, maxSqEdgeLen, maxSearchArea, grid, edgeSkipList);
            }

            return convex;
        }
    
    }
}
