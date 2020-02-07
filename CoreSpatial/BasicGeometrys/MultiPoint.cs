using System.Collections.Generic;
using System.Linq;

namespace CoreSpatial.BasicGeometrys
{
    /// <summary>
    /// 多点
    /// </summary>
    public class MultiPoint: IBasicGeometry
    {
        public MultiPoint():this(new List<GeoPoint>())
        {
        }

        public MultiPoint(List<GeoPoint> points)
        {
            Points = points;
        }

        public List<GeoPoint> Points { get; protected set; }

        public GeoPoint this[int index] => Points[index];

        /// <summary>
        /// 是否为线环
        /// </summary>
        public bool IsLineRing
        {
            get
            {
                if (Points.Count > 2)
                {
                    var startPoint = Points[0];
                    var endPoint = Points[Points.Count - 1];
                    return startPoint == endPoint;
                }
                else
                {
                    return false;
                }
            }
        }

        #region 接口

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public GeometryType GeometryType => GeometryType.MultiPoint;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<Coordinate> Coordinates => Points.Select(e => e.Coordinate);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int PartsNum => Points.Count;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int PointsNum => PartsNum;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnvelope Envelope
        {
            get
            {
                if (Points.Count > 0)
                {
                    var firstPoint = Points[0];

                    var minX = firstPoint.X;
                    var minY = firstPoint.Y;
                    var maxX = firstPoint.X;
                    var maxY = firstPoint.Y;

                    foreach (var geoPoint in Points)
                    {
                        var curX = geoPoint.X;
                        var curY = geoPoint.Y;

                        if (curX < minX)
                        {
                            minX = curX;
                        }
                        else
                        {
                            if (curX > maxX)
                            {
                                maxX = curX;
                            }
                        }

                        if (curY < minY)
                        {
                            minY = curY;
                        }
                        else
                        {
                            if (curY > maxY)
                            {
                                maxY = curY;
                            }
                        }
                    }

                    return new Envelope(minX, minY, maxX, maxY);
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

    }
}
