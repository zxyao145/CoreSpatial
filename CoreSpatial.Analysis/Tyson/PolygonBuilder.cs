/**
 * create by zxyao
 * 2020/2/14
 */

using System;
using System.Collections.Generic;
using System.Linq;
using CoreSpatial.BasicGeometrys;
using CoreSpatial.Utility;

namespace CoreSpatial.Analysis.Tyson
{
    internal class PolygonBuilder
    {
        private double _tolerance;

        public double[] Envelope { get; private set; }

        private TysonGeoPoint _lbPt;
        private TysonGeoPoint _ltPt;
        private TysonGeoPoint _rbPt;
        private TysonGeoPoint _rtPt;
        

        private List<TysonGeoPoint> _allEdgePoints;
        private List<GraphEdge> _envelopeEdges;
        
        public PolygonBuilder()
        {
            _tolerance = Util.DValue;
            _allEdgePoints= new List<TysonGeoPoint>();
            _envelopeEdges = new List<GraphEdge>();
        }

        public Dictionary<TysonGeoPoint, List<TysonGeoPoint>> Build(
            List<TysonGeoPoint> points,
            double[] envelope)
        {
            Envelope = envelope;

            var ge = VoronoiBuilder.BuildLine(points, envelope);

            //x→，y↑
            _lbPt = new TysonGeoPoint(Envelope[0], Envelope[1]);
            _ltPt = new TysonGeoPoint(Envelope[0], Envelope[3]);

            _rbPt = new TysonGeoPoint(Envelope[2], Envelope[1]);
            _rtPt = new TysonGeoPoint(Envelope[2], Envelope[3]);


            var pointWithLines
                = new Dictionary<int, List<GraphEdge>>();
            var edgeLines = new List<GraphEdge>();

            void SafeAddToDict(int pointIndex, GraphEdge edgeLine)
            {
                if (pointWithLines.ContainsKey(pointIndex))
                {
                    pointWithLines[pointIndex].Add(edgeLine);
                }
                else
                {
                    pointWithLines.Add(pointIndex,
                        new List<GraphEdge>()
                        {
                            edgeLine
                        }
                    );
                }
            }
            
            foreach (var graphEdge in ge)
            {
                SafeAddToDict(graphEdge.Point1Index, graphEdge);
                SafeAddToDict(graphEdge.Point2Index, graphEdge);
                
                edgeLines.Add(graphEdge);
            }

            //处理边界点
            EdgePointHandle(points, edgeLines, pointWithLines);


            //遍历所有的EdgeLine，生成Polygon

            var pointPolygonDict = new Dictionary<TysonGeoPoint, List<TysonGeoPoint>>();
            foreach (var kv in pointWithLines)
            {
                var edges = kv.Value;
                var linearRing = BuildLinearRingWithEdgeLine(edges); //BuildLinearRing(edges);
                if (linearRing != null)
                {
                    pointPolygonDict.Add(
                        points[kv.Key],
                        linearRing
                        );
                }
            }

            return pointPolygonDict;
        }

        public Dictionary<int, List<TysonGeoPoint>> BuildWithIndex(
            List<TysonGeoPoint> points,
            double[] envelope,
            double minDistanceTolerance)
        {
            Envelope = envelope;

            var ge = VoronoiBuilder.BuildLine(points, envelope, minDistanceTolerance);

            //x→，y↑
            _lbPt = new TysonGeoPoint(Envelope[0], Envelope[1]);
            _ltPt = new TysonGeoPoint(Envelope[0], Envelope[3]);

            _rbPt = new TysonGeoPoint(Envelope[2], Envelope[1]);
            _rtPt = new TysonGeoPoint(Envelope[2], Envelope[3]);


            var pointWithLines
                = new Dictionary<int, List<GraphEdge>>();
            var edgeLines = new List<GraphEdge>();

            void SafeAddToDict(int pointIndex, GraphEdge edgeLine)
            {
                if (pointWithLines.ContainsKey(pointIndex))
                {
                    pointWithLines[pointIndex].Add(edgeLine);
                }
                else
                {
                    pointWithLines.Add(pointIndex,
                        new List<GraphEdge>()
                        {
                            edgeLine
                        }
                    );
                }
            }

            foreach (var graphEdge in ge)
            {
                SafeAddToDict(graphEdge.Point1Index, graphEdge);
                SafeAddToDict(graphEdge.Point2Index, graphEdge);

                edgeLines.Add(graphEdge);
            }

            //处理边界点
            EdgePointHandle(points, edgeLines, pointWithLines);


            //遍历所有的EdgeLine，生成Polygon

            var pointPolygonDict = new Dictionary<int, List<TysonGeoPoint>>();
            foreach (var kv in pointWithLines)
            {
                var edges = kv.Value;
                var linearRing = BuildLinearRingWithEdgeLine(edges); //BuildLinearRing(edges);
                if (linearRing != null)
                {
                    pointPolygonDict.Add(
                        kv.Key,
                        linearRing
                        );
                }
            }

            return pointPolygonDict;
        }

        /// <summary>
        /// 处理边界点生成闭合线
        /// </summary>
        /// <param name="points"></param>
        /// <param name="edgeLines"></param>
        /// <param name="pointWithLines"></param>
        private void EdgePointHandle(List<TysonGeoPoint> points,
            List<GraphEdge> edgeLines,
            Dictionary<int, List<GraphEdge>> pointWithLines)
        {
            //获取边界点
            _allEdgePoints = GetEdgePoints(edgeLines);

            //边界边
            _envelopeEdges = new List<GraphEdge>();

            #region 边界点处理

            var minXPoints = _allEdgePoints
                .Where(e => e.EdgePointType == EdgePointType.MinX)
                .OrderBy(e => e.Y)
                .ToList();
            var maxXPoints = _allEdgePoints
                .Where(e => e.EdgePointType == EdgePointType.MaxX)
                .OrderByDescending(e => e.Y)
                .ToList();
            var minYPoints = _allEdgePoints
                .Where(e => e.EdgePointType == EdgePointType.MinY)
                .OrderByDescending(e => e.X)
                .ToList();
            var maxYPoints = _allEdgePoints
                .Where(e => e.EdgePointType == EdgePointType.MaxY)
                .OrderBy(e => e.X)
                .ToList();

            _envelopeEdges.Clear();
            _envelopeEdges.AddRange(
                InsertPoint(minXPoints, _lbPt, _ltPt)
            );
            _envelopeEdges.AddRange(
                InsertPoint(maxXPoints, _rtPt, _rbPt)
            );
            _envelopeEdges.AddRange(
                InsertPoint(minYPoints, _rbPt, _lbPt)
            );
            _envelopeEdges.AddRange(
                InsertPoint(maxYPoints, _ltPt, _rtPt)
            );

            //为边界边绑定一个最近的点
            foreach (var edgeLine in _envelopeEdges)
            {
                var ptEdgeDistances = new List<PointEdgeDistance>();

                var index = 0;
                foreach (var point in points)
                {
                    var distance2 =
                        MathUtil.Distance2ToMidPoint(point, edgeLine);

                    ptEdgeDistances.Add(
                        new PointEdgeDistance(index,
                            edgeLine, distance2));

                    index++;
                }

                var order = ptEdgeDistances
                    .OrderBy(e => e.Distance2).ToList();
                var ptIndex = order[0].PointIndex;
                if (pointWithLines.ContainsKey(ptIndex))
                {
                    pointWithLines[ptIndex].Add(edgeLine);
                }
                else
                {
                    pointWithLines.Add(ptIndex, new List<GraphEdge>()
                    {
                        edgeLine
                    });
                }
            }

            #endregion
        }

        /// <summary>
        /// 构建一条线
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        List<TysonGeoPoint> BuildLinearRingWithEdgeLine(List<GraphEdge> edges)
        {
            var linearRingPoints = new LinkedList<TysonGeoPoint>();
            var hashSet = new HashSet<TysonGeoPoint>();

            var firstEdge = edges[0];
            var startPoint = firstEdge.Start;
            var endPoint = firstEdge.End;
            linearRingPoints.AddFirst(startPoint);
            linearRingPoints.AddLast(endPoint);
            hashSet.Add(startPoint);
            hashSet.Add(endPoint);

            edges[0] = null;

            void AddStart(TysonGeoPoint pt, int index)
            {
                if (!hashSet.Contains(pt))
                {
                    startPoint = pt;
                    linearRingPoints.AddFirst(startPoint);
                }
                edges[index] = null;
            }

            void AddLast(TysonGeoPoint pt, int index)
            {
                if (!hashSet.Contains(pt))
                {
                    endPoint = pt;
                    linearRingPoints.AddLast(endPoint);
                }

                edges[index] = null;
            }

            Tag:
            var edgesCount = edges.Count;
            for (int i = 0; i < edgesCount; i++)
            {
                var edge = edges[i];
                if (edge == null)
                {
                    continue;
                }
                else
                {
                    var curStart = edge.Start;
                    var curEnd = edge.End;


                    if (curStart == startPoint)
                    {
                        AddStart(curEnd, i);
                    }
                    else if (curEnd == startPoint)
                    {
                        AddStart(curStart, i);
                    }

                    else if (curStart == endPoint)
                    {
                        AddLast(curEnd, i);
                    }
                    else if (curEnd == endPoint)
                    {
                        AddLast(curStart, i);
                    }
                    else
                    {
                        if (hashSet.Contains(curStart) && hashSet.Contains(curEnd))
                        {
                            edges[i] = null;
                        }
                    }
                }

            }

            edges = edges.Where(e => e != null).ToList();
            if (edges.Count != 0)
            {
                if (edges.Count < edgesCount)
                {
                    goto Tag;
                }
            }

            if (startPoint != endPoint)
            {
                linearRingPoints.AddLast(startPoint);
            }

            return linearRingPoints.ToList();



            //var firstCoord = linearRingPoints.First.Value;
            //var endCoord = linearRingPoints.Last.Value;
            //if (firstCoord
            //    != endCoord)
            //{
            //    linearRingPoints.AddLast(firstCoord);
            //}

            //if (linearRingPoints.Count < 4 && linearRingPoints.Count > 0)
            //{
            //    return null;
            //}

            //var linearRing = new LinearRing(linearRingPoints.ToArray());

            //return linearRing;
        }

        #region 构建线方法2

        ///// <summary>
        ///// 构建一条线
        ///// </summary>
        ///// <param name="edges"></param>
        ///// <returns></returns>
        //LinearRing BuildLinearRing(List<GraphEdge> edges)
        //{

        //    var linearRingPoints = GetEdgeLineCoordinates(edges);

        //    var linearRing = new LinearRing(linearRingPoints.ToArray());

        //    return linearRing;
        //}

        //List<Coordinate> GetEdgeLinePartCoordinates(List<GraphEdge> edges)
        //{
        //    var linearRingPoints = new LinkedList<Coordinate>();
        //    var hashSet = new HashSet<TysonGeoPoint>();

        //    var firstEdge = edges[0];
        //    var startPoint = firstEdge.Start;
        //    var endPoint = firstEdge.End;
        //    linearRingPoints.AddFirst(startPoint.ToCoordinate());
        //    linearRingPoints.AddLast(endPoint.ToCoordinate());
        //    hashSet.Add(startPoint);
        //    hashSet.Add(endPoint);

        //    edges[0] = null;

        //    void AddStart(TysonGeoPoint pt, int index)
        //    {
        //        if (!hashSet.Contains(pt))
        //        {
        //            startPoint = pt;
        //            linearRingPoints.AddFirst(startPoint.ToCoordinate());
        //        }
        //        edges[index] = null;
        //    }

        //    void AddLast(TysonGeoPoint pt, int index)
        //    {
        //        if (!hashSet.Contains(pt))
        //        {
        //            endPoint = pt;
        //            linearRingPoints.AddLast(endPoint.ToCoordinate());
        //        }

        //        edges[index] = null;
        //    }

        //    Tag:
        //    var edgesCount = edges.Count;
        //    for (int i = 0; i < edgesCount; i++)
        //    {
        //        var edge = edges[i];
        //        if (edge == null)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            var curStart = edge.Start;
        //            var curEnd = edge.End;


        //            if (curStart == startPoint)
        //            {
        //                AddStart(curEnd, i);
        //            }
        //            else if (curEnd == startPoint)
        //            {
        //                AddStart(curStart, i);
        //            }

        //            else if (curStart == endPoint)
        //            {
        //                AddLast(curEnd, i);
        //            }
        //            else if (curEnd == endPoint)
        //            {
        //                AddLast(curStart, i);
        //            }
        //            else
        //            {
        //                if (hashSet.Contains(curStart) && hashSet.Contains(curEnd))
        //                {
        //                    edges[i] = null;
        //                }
        //            }
        //        }
        //    }

        //    edges = edges.Where(e => e != null).ToList();
        //    if (edges.Count != 0)
        //    {
        //        if (edges.Count < edgesCount)
        //        {
        //            goto Tag;
        //        }
        //        //有边与其他边隔离
        //        else
        //        {
        //            var otherPoints = GetEdgeLinePartCoordinates(edges);
        //            var otherStartPoint = otherPoints[0];
        //            if (Math.Abs(startPoint.X - otherStartPoint.X) < _tolerance
        //                || Math.Abs(startPoint.Y - otherStartPoint.Y) < _tolerance)
        //            {
        //                foreach (var coordinate in otherPoints)
        //                {
        //                    linearRingPoints.AddFirst(coordinate);
        //                }
        //            }
        //            else
        //            {
        //                foreach (var coordinate in otherPoints)
        //                {
        //                    linearRingPoints.AddLast(coordinate);
        //                }
        //            }
        //        }
        //    }

        //    return linearRingPoints.ToList();
        //}

        //List<Coordinate> GetEdgeLineCoordinates(List<GraphEdge> edges)
        //{
        //    var linearRingPoints = GetEdgeLinePartCoordinates(edges);
        //    var start = linearRingPoints[0];
        //    var end = linearRingPoints[linearRingPoints.Count - 1];

        //    var startPoint = new TysonGeoPoint(start.X, start.Y);
        //    var endPoint = new TysonGeoPoint(end.X, end.Y);

        //    var nextPoints = Close(startPoint, endPoint);

        //    foreach (var coordinate in nextPoints)
        //    {
        //        //AddLast
        //        linearRingPoints.Add(coordinate);
        //    }

        //    return linearRingPoints;
        //}

        ///// <summary>
        ///// 闭合polygon
        ///// </summary>
        ///// <param name="startGeoPoint"></param>
        ///// <param name="endGeoPoint"></param>
        ///// <returns></returns>
        //List<Coordinate> Close(TysonGeoPoint startGeoPoint, TysonGeoPoint endGeoPoint)
        //{
        //    var startPtType = CheckEdgePointTypeWithCorner(startGeoPoint);
        //    var endPtType = CheckEdgePointTypeWithCorner(endGeoPoint);

        //    var result = new List<Coordinate>();
        //    //两者都不是内部点
        //    if (!(startPtType == EdgePointType.Inner
        //        && endPtType == EdgePointType.Inner))
        //    {
        //        var startX = startGeoPoint.X;
        //        var startY = startGeoPoint.Y;
        //        var endX = endGeoPoint.X;
        //        var endY = endGeoPoint.Y;

        //        //两点共边，只需要添加起点封闭即可
        //        //两点不共边，才需要添加其他点
        //        if (startPtType != endPtType)
        //        {
        //            //两点左右平行
        //            if (
        //                startPtType == EdgePointType.MinX
        //                && endPtType == EdgePointType.MaxX
        //            )
        //            {
        //                var edgeEdgeLines = new List<GraphEdge>()
        //                {
        //                    new GraphEdge(startGeoPoint,_ltPt),
        //                    new GraphEdge(_ltPt,_rtPt),
        //                    new GraphEdge(_rtPt,endGeoPoint)
        //                };
        //                int maxYPointCount = 0;
        //                foreach (var edgeEdgeLine in edgeEdgeLines)
        //                {
        //                    foreach (var allEdgePoint in _allEdgePoints)
        //                    {
        //                        if (edgeEdgeLine.ContainPoint(allEdgePoint))
        //                        {
        //                            maxYPointCount++;
        //                        }
        //                    }
        //                }

        //                //两者需要上面的边
        //                if (maxYPointCount == 0)
        //                {
        //                    result.Add(_rtPt.ToCoordinate());
        //                    result.Add(_ltPt.ToCoordinate());
        //                }
        //                else
        //                {
        //                    result.Add(_rbPt.ToCoordinate());
        //                    result.Add(_lbPt.ToCoordinate());
        //                }
        //            }
        //            else if (
        //                startPtType == EdgePointType.MaxX
        //                && endPtType == EdgePointType.MinX
        //            )
        //            {
        //                var edgeEdgeLines = new List<GraphEdge>()
        //                {
        //                    new GraphEdge(startGeoPoint,_rtPt),
        //                    new GraphEdge(_ltPt,_rtPt),
        //                    new GraphEdge(_ltPt,endGeoPoint)
        //                };
        //                int maxYPointCount = 0;
        //                foreach (var edgeEdgeLine in edgeEdgeLines)
        //                {
        //                    foreach (var allEdgePoint in _allEdgePoints)
        //                    {
        //                        if (edgeEdgeLine.ContainPoint(allEdgePoint))
        //                        {
        //                            maxYPointCount++;
        //                        }
        //                    }
        //                }

        //                //两者需要上面的边
        //                if (maxYPointCount == 0)
        //                {
        //                    result.Add(_ltPt.ToCoordinate());
        //                    result.Add(_rtPt.ToCoordinate());
        //                }
        //                else
        //                {
        //                    result.Add(_lbPt.ToCoordinate());
        //                    result.Add(_rbPt.ToCoordinate());
        //                }
        //            }

        //            //两点上下平行
        //            else if (startPtType == EdgePointType.MinY
        //                     && endPtType == EdgePointType.MaxY
        //            )
        //            {
        //                var edgeEdgeLines = new List<GraphEdge>()
        //                {
        //                    new GraphEdge(startGeoPoint,_lbPt),
        //                    new GraphEdge(_lbPt,_ltPt),
        //                    new GraphEdge(_ltPt,endGeoPoint)
        //                };
        //                int minXPointCount = 0;
        //                foreach (var edgeEdgeLine in edgeEdgeLines)
        //                {
        //                    foreach (var allEdgePoint in _allEdgePoints)
        //                    {
        //                        if (edgeEdgeLine.ContainPoint(allEdgePoint))
        //                        {
        //                            minXPointCount++;
        //                        }
        //                    }
        //                }

        //                //两者需要左边的边
        //                if (minXPointCount == 0)
        //                {
        //                    result.Add(_ltPt.ToCoordinate());
        //                    result.Add(_lbPt.ToCoordinate());
        //                }
        //                else
        //                {
        //                    result.Add(_rtPt.ToCoordinate());
        //                    result.Add(_rbPt.ToCoordinate());
        //                }
        //            }
        //            else if (startPtType == EdgePointType.MaxY
        //                     && endPtType == EdgePointType.MinY
        //            )
        //            {
        //                var edgeEdgeLines = new List<GraphEdge>()
        //                {
        //                    new GraphEdge(startGeoPoint,_ltPt),
        //                    new GraphEdge(_lbPt,_ltPt),
        //                    new GraphEdge(_lbPt,endGeoPoint)
        //                };
        //                int minXPointCount = 0;
        //                foreach (var edgeEdgeLine in edgeEdgeLines)
        //                {
        //                    foreach (var allEdgePoint in _allEdgePoints)
        //                    {
        //                        if (edgeEdgeLine.ContainPoint(allEdgePoint))
        //                        {
        //                            minXPointCount++;
        //                        }
        //                    }
        //                }

        //                //两者需要左边的边
        //                if (minXPointCount == 0)
        //                {
        //                    result.Add(_lbPt.ToCoordinate());
        //                    result.Add(_ltPt.ToCoordinate());
        //                }
        //                else
        //                {
        //                    result.Add(_rbPt.ToCoordinate());
        //                    result.Add(_rtPt.ToCoordinate());
        //                }
        //            }
        //            //两点共一个角
        //            else
        //            {
        //                var newPoint = new TysonGeoPoint();

        //                if (Math.Abs(startX - Envelope[0]) < _tolerance
        //                    || Math.Abs(startX - Envelope[2]) < _tolerance)
        //                {
        //                    newPoint.X = startX;
        //                    newPoint.Y = endY;
        //                }
        //                else
        //                {
        //                    newPoint.X = endX;
        //                    newPoint.Y = startY;
        //                }
        //                result.Add(newPoint.ToCoordinate());
        //            }

        //        }
        //    }

        //    result.Add(startGeoPoint.ToCoordinate());
        //    return result;
        //}
        
        #endregion

        #region CheckEdgePointType

        int PositionX(double x)
        {
            if (Math.Abs(x - Envelope[0]) < _tolerance)
            {
                return -1;
            }
            else if (Math.Abs(x - Envelope[2]) < _tolerance)
            {
                return 1;
            }
            return 0;
        }

        int PositionY(double y)
        {
            if (Math.Abs(y - Envelope[1]) < _tolerance)
            {
                return -1;
            }
            else if (Math.Abs(y - Envelope[3]) < _tolerance)
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 边界点类型，检测角点
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        EdgePointType CheckEdgePointTypeWithCorner(TysonGeoPoint pt)
        {
            var xPos = PositionX(pt.X);
            var yPos = PositionY(pt.Y);
            if (xPos == -1)
            {
                if (yPos == -1)
                {
                    return EdgePointType.MinXMinY;
                }
                else if (yPos == 1)
                {
                    return EdgePointType.MinXMaxY;
                }
                return EdgePointType.MinX;
            }
            else if (xPos == 1)
            {
                if (yPos == -1)
                {
                    return EdgePointType.MaxXMinY;
                }
                else if (yPos == 1)
                {
                    return EdgePointType.MaxXMaxY;
                }
                return EdgePointType.MaxX;
            }
            else
            {
                if (yPos == -1)
                {
                    return EdgePointType.MinY;
                }
                else if (yPos == 1)
                {
                    return EdgePointType.MaxY;
                }
                else
                {
                    return EdgePointType.Inner;
                }
            }
        }

        /// <summary>
        /// 边界点类型，不检测角点
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="xieLv"></param>
        /// <returns></returns>
        EdgePointType CheckEdgePointType(TysonGeoPoint pt, double xieLv)
        {
            if (pt.EdgePointType != EdgePointType.NotCalc)
            {
                return pt.EdgePointType;
            }
            EdgePointType type;
            var pos = PointPosition(pt);
            //内部
            if (pos == 0)
            {
                type = EdgePointType.Inner;
            }
            //下部
            else if (pos == 1)
            {
                type = EdgePointType.MinY;
            }
            //上部
            else if (pos == 2)
            {
                type = EdgePointType.MaxY;
            }
            //左部
            else if (pos == 4)
            {
                type = EdgePointType.MinX;
            }
            //右部
            else if (pos == 8)
            {
                type = EdgePointType.MaxX;
            }
            else
            {
                if (pos == 5)
                {
                    type = Pos5EdgePointType(pt, xieLv);
                }
                else if (pos == 6)
                {
                    type = Pos6EdgePointType(pt, xieLv);
                }
                else if (pos == 9)
                {
                    type = Pos9EdgePointType(pt, xieLv);
                }
                else //if (pos == 10)
                {
                    type = Pos10EdgePointType(pt, xieLv);
                }
            }

            pt.EdgePointType = type;
            return type;
        }
        

        EdgePointType Pos5EdgePointType(TysonGeoPoint pt, double oLineXieLv)
        {
            var xieLv = MathUtil.GetXieLv(pt, _lbPt);
            if (xieLv > oLineXieLv)
            {
                return EdgePointType.MinY;
            }
            else
            {
                return EdgePointType.MinX;
            }
        }

        EdgePointType Pos10EdgePointType(TysonGeoPoint pt, double oLineXieLv)
        {
            var xieLv = MathUtil.GetXieLv(pt, _rtPt);
            if (xieLv > oLineXieLv)
            {
                return EdgePointType.MaxY;
            }
            else
            {
                return EdgePointType.MaxX;
            }
        }

        EdgePointType Pos6EdgePointType(TysonGeoPoint pt, double oLineXieLv)
        {
            var xieLv = MathUtil.GetXieLv(pt, _ltPt);
            if (xieLv > oLineXieLv)
            {
                return EdgePointType.MinX;
            }
            else
            {
                return EdgePointType.MaxY;
            }
        }

        EdgePointType Pos9EdgePointType(TysonGeoPoint pt, double oLineXieLv)
        {
            var xieLv = MathUtil.GetXieLv(pt, _rbPt);
            if (xieLv > oLineXieLv)
            {
                return EdgePointType.MinY;
            }
            else
            {
                return EdgePointType.MaxX;
            }
        }
        
        /// <summary>
        /// ↑
        /// |     0110 |  0010 |  1010         
        /// |      6   |   2   |    10 
        /// |   -------|-------|--------   
        /// |     0100 |  0000 |  1000 
        /// |      4   |    0  |    8  
        /// |   -------|-------|--------  
        /// |     0101 |  0001 |  1001 
        /// |      5   |   1   |    9  
        /// ---------------------------------→
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        int PointPosition(TysonGeoPoint pt)
        {
            var code1 = "00";
            var code2 = "00";

            if (pt.X <= Envelope[0])
            {
                code1 = "01";
            }
            else if (pt.X >= Envelope[2])
            {
                code1 = "10";
            }

            if (pt.Y <= Envelope[1])
            {
                code2 = "01";
            }
            else if (pt.Y >= Envelope[3])
            {
                code2 = "10";
            }

            return Convert.ToInt32(code1 + code2, 2);
        }

        #endregion

        #region 边界点相关

        /// <summary>
        /// 获取Voronoi Edge中位于边界的点
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        List<TysonGeoPoint> GetEdgePoints(IEnumerable<GraphEdge> edges)
        {
            var result = new HashSet<TysonGeoPoint>();
            foreach (var voronoiEdge in edges)
            {
                var start = voronoiEdge.Start;
                var end = voronoiEdge.End;
                var xieLv = MathUtil.GetXieLv(start, end);
                var type = CheckEdgePointType(start, xieLv);
                if (type != EdgePointType.Inner)
                {
                    start.EdgePointType = type;
                    result.Add(start);
                }
                type = CheckEdgePointType(end, xieLv);
                if (type != EdgePointType.Inner)
                {
                    end.EdgePointType = type;
                    result.Add(end);
                }
            }

            return result.ToList();
        }

        /// <summary>
        /// /更改边界点的坐标，使其位于边界上
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="otherTysonGeoPoint"></param>
        /// <param name="type"></param>
        void ChangePtPosToEdge(GeoPoint pt, GeoPoint otherTysonGeoPoint, EdgePointType type)
        {
            GeoPoint intersection = null;
            switch (type)
            {
                case EdgePointType.MinX:
                    {
                        intersection = MathUtil.GetIntersection
                        (_lbPt, _ltPt,
                            pt, otherTysonGeoPoint);
                        break;
                    }
                case EdgePointType.MinY:
                    {
                        intersection = MathUtil.GetIntersection
                        (_lbPt, _rbPt,
                            pt, otherTysonGeoPoint);
                        break;
                    }
                case EdgePointType.MaxX:
                    {
                        intersection = MathUtil.GetIntersection
                        (_rtPt, _rbPt,
                            pt, otherTysonGeoPoint);
                        break;
                    }
                case EdgePointType.MaxY:
                    {
                        intersection = MathUtil.GetIntersection
                        (_ltPt, _rtPt,
                            pt, otherTysonGeoPoint);
                        break;
                    }
                default: break;
            }

            if (intersection != null)
            {
                pt.X = intersection.X;
                pt.Y = intersection.Y;
            }
        }

        /// <summary>
        /// 根据起点、重点和中间点构建多个线段
        /// </summary>
        /// <param name="innerPoints"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        List<GraphEdge> InsertPoint(IEnumerable<TysonGeoPoint> innerPoints,
            TysonGeoPoint start, TysonGeoPoint end)
        {
            var lastPt = start;
            var result = new List<GraphEdge>();
            foreach (var innerPoint in innerPoints)
            {
                result.Add(new GraphEdge(lastPt, innerPoint));
                lastPt = innerPoint;
            }
            result.Add(new GraphEdge(lastPt, end));
            return result;
        }
        
        #endregion
    }
}
