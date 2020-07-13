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

        public GeoPoint(double x, double y, double z = double.NaN)
        {
            _coordinate = new Coordinate(x, y, z);
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
        /// Z坐标
        /// </summary>
        public double Z
        {
            get => _coordinate.Z;
            set => _coordinate.Z = value;
        }
        
        public void SetPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public void SetPoint(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public bool IsPointZ => this.Z == double.NaN;

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

            var point1IsNull = object.ReferenceEquals(geoPoint1, null);
            var point2IsNull = object.ReferenceEquals(geoPoint2, null);

            if (point1IsNull && point2IsNull)
            {
                return true;
            }
            //else
            if (point1IsNull || point2IsNull) 
            { 
                return false;
            }

            #endregion

            if ((Math.Abs(geoPoint1.X - geoPoint2.X) < Util.DValue) && (Math.Abs(geoPoint1.Y - geoPoint2.Y) < Util.DValue))
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
            if (object.ReferenceEquals(obj, null))
            {
                throw new ArgumentNullException();
            }
            //else
            return object.ReferenceEquals(this, obj);
        }

        #endregion


        public GeoPoint DeepClone()
        {
            return new GeoPoint(X, Y);
        }
    }
}
