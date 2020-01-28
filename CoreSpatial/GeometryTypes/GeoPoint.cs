using System;
using CoreSpatial.Utility;

namespace CoreSpatial.GeometryTypes
{
    public class GeoPoint: IGeoPoint
    {
        public GeoPoint()
        {
            
        }

        public GeoPoint(double x,double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y { get; set; }


        public static bool operator==(GeoPoint geoPoint1, GeoPoint geoPoint2)
        {
            if ((Math.Abs(geoPoint1.X - geoPoint2.X) < ShpUtil.DValue)&& (Math.Abs(geoPoint1.Y - geoPoint2.Y) < ShpUtil.DValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(GeoPoint geoPoint1, GeoPoint geoPoint2)
        {
            return !(geoPoint1 == geoPoint2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj is GeoPoint point)
            {
                return this == point;
            }
            else
            {
                throw new Exception("obj不是GeoPoint类型，不能进行Equals比较!");
            }
        }

        public override string ToString()
        {
            return $"[{X},{Y}]";
        }

        public IEnvelope Envelope => new Envelope(X, Y, X, Y);
    }
}
