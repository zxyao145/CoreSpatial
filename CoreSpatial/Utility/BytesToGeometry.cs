using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CoreSpatial.GeometryTypes;


namespace CoreSpatial.Utility
{
    /// <summary>
    /// 用户将byte数组解析成几何要素实体类
    /// </summary>
    internal static class BytesToGeometry
    {
        public static IGeometry CreatePoint(byte[] buffer)
        {
            if (buffer.Length >= 20)
            {
                GeoPoint point = new GeoPoint
                {
                    X = BitConverter.ToDouble(buffer, 4),
                    Y = BitConverter.ToDouble(buffer, 12)
                };
                IBasicGeometry basicGeometry = point;
                IGeometry geometry = new Geometry()
                {
                    BasicGeometry = basicGeometry,
                    GeometryType = GeometryType.Point,
                    Points = new List<IGeoPoint>() { point }
                };
                return geometry;
            }
            else
            {
                throw new Exception($"Byte数组大小无效，不能创建点。数组大小为{buffer.Length}");
            }
        }
        
        public static IGeometry CreateMultipoint(byte[] buffer)
        {
            if (buffer.Length >= 56)
            {
                int numPoints = BitConverter.ToInt32(buffer, 36);
                
                List<IGeoPoint> points = new List<IGeoPoint>();
                for (int i = 0; i < numPoints; i++)
                {
                    GeoPoint point = new GeoPoint
                    {
                        X = BitConverter.ToDouble(buffer, (i * 16) + 40),
                        Y = BitConverter.ToDouble(buffer, (i * 16) + 48)
                    };
                    points.Add(point);
                }
                IBasicGeometry basicGeometry = new MultiPoint()
                {
                    Points = points
                };

                IGeometry geometry = new Geometry()
                {
                    BasicGeometry = basicGeometry,
                    GeometryType = GeometryType.MultiPoint,
                    Points =  points
                };
                return geometry;
            }
            else
            {
                throw new ArgumentException("Byte数组大小无效，不能创建多点几何要素。Byte数组大小：" + buffer.Length);
            }
        }

        public static IGeometry CreatePolyline(byte[] buffer)
        {
            if (buffer.Length >= 60)
            {
                return GetMultipointParts<MultiPolyLine>(buffer);
            }
            else
            {
                throw new ArgumentException($"Byte数组大小无效，不能创建线要素。数组大小为{buffer.Length}");
            }
        }

        public static IGeometry CreatePolygon(byte[] buffer)
        {
            if (buffer.Length >= 60)
            {
                return GetMultipointParts<Polygon>(buffer);
            }
            else
            {
                throw new ArgumentException($"Byte数组大小无效，不能创建面几何要素。数组大小为{buffer.Length}");
            }
        }


        #region 解析多点要素相关私有

        /// <summary>
        /// 解析多点要素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="recordContents"></param>
        /// <returns></returns>
        private static IGeometry GetMultipointParts<T>(byte[] recordContents) 
            where T:MultiPolyLine,new()
        {
            //获取几何分了几部分，以及所有坐标点的数量
            PolyRecordFields recordNums = new PolyRecordFields(recordContents);
            //获取记录中的所有的坐标点
            List<IGeoPoint> points = GetPolyPoints(recordNums.NumPoints, recordNums.NumParts, recordContents);

            //按照部分划分这些点
            List<PolyLine> multiparts = new List<PolyLine>();
            //获取每部分点的开始索引
            int[] partIndex = GetRecordPartOffsets(recordNums.NumParts, recordContents);
            var partsNum = partIndex.Length;
            //循环以生成各个部分的点
            for (int n = 0; n < partsNum; n++)
            {
                //判断这是不是最后一部分点，并计算本部分点共有多少个
                int numPointsInPart = 
                    n < partsNum - 1 
                    ? partIndex[n + 1] - partIndex[n] 
                    : recordNums.NumPoints - partIndex[n];

                //取出当前这部分的点
                List<IGeoPoint> partPoints =
                    points.Skip(partIndex[n]).Take(numPointsInPart).ToList();

                PolyLine multiPoint = new PolyLine { Points = partPoints};
                multiparts.Add(multiPoint);
            }

            IBasicGeometry polygon = new T()
            {
                PartsNum = partsNum,
                PolyLines = multiparts
            };

            Type t = typeof(T);
            GeometryType geometryType = t == typeof(Polygon) ? GeometryType.Polygon : (partsNum == 1 ? GeometryType.PolyLine : GeometryType.MultiPolyLine);

            IGeometry geometry = new Geometry()
            {
                BasicGeometry = polygon,
                GeometryType = geometryType,
                Points = points
            };
            return geometry;
        }
        
        /// <summary>
        /// 解析所有的坐标点
        /// </summary>
        /// <param name="numPoints"></param>
        /// <param name="numParts"></param>
        /// <param name="recordContents"></param>
        /// <returns></returns>
        private static List<IGeoPoint> GetPolyPoints(int numPoints, int numParts, byte[] recordContents)
        {
            //计算需要跳过多少个byte
            int bytesToSkip = 44 + (4 * numParts);

            List<IGeoPoint> points = new List<IGeoPoint>();
            for (int j = 0; j < numPoints; j++)
            {
                GeoPoint point = new GeoPoint
                {
                    X = BitConverter.ToDouble(recordContents, (j * 16) + bytesToSkip),
                    Y = BitConverter.ToDouble(recordContents, (j * 16) + bytesToSkip + 8)
                };
                points.Add(point);
            }
            return points;
        }

        private struct PolyRecordFields
        {
            public int NumParts;
            public int NumPoints;

            public PolyRecordFields(byte[] recordContents)
            {
                //该几何包含多少部分
                this.NumParts = BitConverter.ToInt32(recordContents, 36);
                //该几何包含多少个点
                this.NumPoints = BitConverter.ToInt32(recordContents, 40);
            }
        }

        /// <summary>
        /// 读取索引文件，得到当前几何要素各部分第一个点的索引位置
        /// </summary>
        /// <param name="numParts"></param>
        /// <param name="recordContents"></param>
        /// <returns></returns>
        private static int[] GetRecordPartOffsets(int numParts, byte[] recordContents)
        {
            int[] recordOffsets = new int[numParts];
            for (int i = 0; i < numParts; i++)
            {
                recordOffsets[i] = BitConverter.ToInt32(recordContents, (i * 4) + 44);
            }
            return recordOffsets;
        }

        #endregion

    }

    internal static class GeometryToBytes
    {
        /// <summary>
        /// 点
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static IEnumerable<byte> GetPointBytes(IBasicGeometry geometry)
        {
            var point = (GeoPoint)geometry;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(point.X));
            bytes.AddRange(BitConverter.GetBytes(point.Y));
            return bytes;
        }

        /// <summary>
        /// 多点
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static IEnumerable<byte> GetMultiPointBytes(IBasicGeometry geometry)
        {
            var multiPoint = (MultiPoint)geometry;
            var bytes = new List<byte>();
            MultiGeometryPreHandle(bytes, 
                multiPoint.Envelope, multiPoint.PartsNum);

            foreach (var point in multiPoint.Points)
            {
                bytes.AddRange(GetPointBytes(point));
            }

            return bytes;
        }

        /// <summary>
        /// 线
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static IEnumerable<byte> GetPolyLineBytes(IBasicGeometry geometry)
        {
            if (geometry.GetType().Name == "MultiPolyLine")
            {
                //var polyLine = ((MultiPolyLine)geometry).PolyLines[0];
                return GetMultiPolyLineBytes(geometry);
            }
            else
            {
                var line = (PolyLine) geometry;
                var bytes = new List<byte>();
                foreach (var point in line.Points)
                {
                    bytes.AddRange(BitConverter.GetBytes(point.X));
                    bytes.AddRange(BitConverter.GetBytes(point.Y));
                }
                return bytes;
            }
        }

        /// <summary>
        /// 多线
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static IEnumerable<byte> GetMultiPolyLineBytes(IBasicGeometry geometry)
        {
            var bytes = new List<byte>();
            var multiPolyLine = (MultiPolyLine)geometry;

            MultiGeometryPreHandle(bytes,
                multiPolyLine.Envelope,
                multiPolyLine.PartsNum,
                multiPolyLine.PointsNum
                );

            var firstPtIndexOfEveryPart =  new List<int>();
            var curPtIndexOfAllPoints = 0;

            var allPointsBytes = new List<byte>();
            foreach (var polyLine in multiPolyLine.PolyLines)
            {
                firstPtIndexOfEveryPart.Add(curPtIndexOfAllPoints);
                curPtIndexOfAllPoints += polyLine.PointsNum;

                allPointsBytes.AddRange(GetPolyLineBytes(polyLine));
            }

            foreach (var i in firstPtIndexOfEveryPart)
            {
                bytes.AddRange(BitConverter.GetBytes(i));
            }
            bytes.AddRange(allPointsBytes);

            return bytes;
        }

        /// <summary>
        /// 面
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static IEnumerable<byte> GetPolygonBytes(IBasicGeometry geometry)
        {
            return GetMultiPolyLineBytes(geometry);
        }

        /// <summary>
        /// 多面
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static IEnumerable<byte> GetMultiPolygonBytes(IBasicGeometry geometry)
        {
            var bytes = new List<byte>();
            //var multiPolyLine = (MultiPolygon)geometry;

            //MultiGeometryPreHandle(bytes,
            //    multiPolyLine.Envelope, multiPolyLine.PartsNum);

            //foreach (var polyLine in multiPolyLine.PolyLines)
            //{
            //    bytes.AddRange(GetPolyLineBytes(polyLine));
            //}
            return bytes;
        }


        /// <summary>
        /// 多部件几何预处理
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="envelope"></param>
        /// <param name="partNum"></param>
        /// <param name="pointNum"></param>
        private static void MultiGeometryPreHandle(List<byte> bytes,
            IEnvelope envelope,
            int partNum,int? pointNum = null)
        {
            //box
            bytes.AddRange(
                BitConverter.GetBytes(envelope.MinX)
            );
            bytes.AddRange(
                BitConverter.GetBytes(envelope.MinY)
            );
            bytes.AddRange(
                BitConverter.GetBytes(envelope.MaxX)
            );
            bytes.AddRange(
                BitConverter.GetBytes(envelope.MaxY)
            );
            //num of part
            bytes.AddRange(
                BitConverter.GetBytes(partNum)
            );

            if (pointNum != null)
            {
                bytes.AddRange(
                    BitConverter.GetBytes(pointNum.Value)
                );
            }
        }
    }

}
