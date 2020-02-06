using System;
using System.Collections.Generic;
using CoreSpatial.Utility;

namespace CoreSpatial.BasicGeometrys
{
    /// <summary>
    /// 点
    /// </summary>
    public class GeoPoint : IBasicGeometry
    {
        public GeoPoint()
        {
            _coordinate = new Coordinate();
        }

        public GeoPoint(double x, double y)
        {
            _coordinate = new Coordinate(x, y);
        }

        private readonly Coordinate _coordinate;

        /// <summary>
        /// X坐标
        /// </summary>
        public double X
        {
            get => _coordinate.X;
            set => _coordinate.X = value;
        }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y
        {
            get => _coordinate.Y;
            set => _coordinate.Y = value;
        }




        /// <summary>
        /// 坐标对
        /// </summary>
        public Coordinate Coordinate => _coordinate;

        #region 接口实现

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public GeometryType GeometryType => GeometryType.Point;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnvelope Envelope => new Envelope(X, Y, X, Y);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<Coordinate> Coordinates => new List<Coordinate>()
        {
            _coordinate
        };

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int PartsNum => 1;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int PointsNum => 1;

        #endregion

        #region 方法重载

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"[{X},{Y}]";
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        #endregion

        #region 操作符重载

        /// <summary>
        /// 相等判断
        /// </summary>
        /// <param name="geoPoint1"></param>
        /// <param name="geoPoint2"></param>
        /// <returns></returns>
        public static bool operator ==(GeoPoint geoPoint1, GeoPoint geoPoint2)
        {
            #region check null

            if (geoPoint1 == null && geoPoint2 == null)
            {
                return true;
            }
            //else
            if (geoPoint1 == null || geoPoint2 == null)
            {
                return false;
            }

            #endregion

            if ((Math.Abs(geoPoint1.X - geoPoint2.X) < ShpUtil.DValue) && (Math.Abs(geoPoint1.Y - geoPoint2.Y) < ShpUtil.DValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 不相等判断
        /// </summary>
        /// <param name="geoPoint1"></param>
        /// <param name="geoPoint2"></param>
        /// <returns></returns>
        public static bool operator !=(GeoPoint geoPoint1, GeoPoint geoPoint2)
        {
            return !(geoPoint1 == geoPoint2);
        }

        /// <summary>
        /// 判断是否为同一个引用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException();
            }
            //else
            return object.ReferenceEquals(this, obj);
        }

        #endregion

    }
}
