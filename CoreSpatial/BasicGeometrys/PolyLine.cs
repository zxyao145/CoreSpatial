using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CoreSpatial.BasicGeometrys
{
    public class PolyLine: MultiPoint,IBasicGeometry
    {
        public PolyLine():base()
        {
            
        }
        public PolyLine(List<GeoPoint> points) : base(points)
        {

        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public new GeometryType GeometryType => GeometryType.PolyLine;

        /// <summary>
        /// 闭合线
        /// </summary>
        public void SafeCloseLine()
        {
            if (!IsLineRing)
            {
                Points.Add(Points[0]);
            }
        }

        #region 线方向

        /// <summary>
        /// 将变为线顺时针方向
        /// </summary>
        public void Clockwise()
        {
            var clockDirection = ClockDirection;
            if (clockDirection == ClockDirection.Counterclockwise)
            {
                Points.Reverse();
            }
        }

        /// <summary>
        /// 将变为线逆时针方向
        /// </summary>
        public void Counterclockwise()
        {
            var clockDirection = ClockDirection;
            if (clockDirection == ClockDirection.Clockwise)
            {
                Points.Reverse();
            }
        }

        /// <summary>
        /// 线的方向
        /// </summary>
        public ClockDirection ClockDirection => GetClockDirection();

        /// <summary>
        /// 获取多边形的方向
        /// </summary>
        /// <returns></returns>
        protected ClockDirection GetClockDirection()
        {
            //所有的点
            var points = this.Points;
            //true:Y轴向下为正(屏幕坐标系),false:Y轴向上为正(一般的坐标系)
            var isYAxixToDown = false;
            if (points == null || points.Count < 3)
            {
                return (0);
            }

            int i, j, k;
            int count = 0;
            double z;
            int yTrans = isYAxixToDown ? (-1) : (1);
            int n = points.Count;
            for (i = 0; i < n; i++)
            {
                j = (i + 1) % n;
                k = (i + 2) % n;
                z = (points[j].X - points[i].X) * (points[k].Y * yTrans - points[j].Y * yTrans);
                z -= (points[j].Y * yTrans - points[i].Y * yTrans) * (points[k].X - points[j].X);
                if (z < 0)
                {
                    count--;
                }
                else if (z > 0)
                {
                    count++;
                }
            }
            if (count > 0)
            {
                return (ClockDirection.Counterclockwise);
            }
            else if (count < 0)
            {
                return (ClockDirection.Clockwise);
            }
            else
            {
                return (ClockDirection.None);
            }
        }

        #endregion

        #region 凹凸性

        /// <summary>
        /// 判断多边形的凹凸性，仅针对线环有效，若不是线环，则返回PolygonType.None
        /// </summary>
        public PolygonType PolygonType
        {
            get
            {
                if (IsLineRing)
                {
                    return GetPolygonType();
                }
                else
                {
                    return PolygonType.None;
                }
            }
        }

        /// <summary>
        /// 计算多边形的凹凸性
        /// </summary>
        /// <returns></returns>
        protected PolygonType GetPolygonType()
        {
            //所有的点
            var points = this.Points;
            //true:Y轴向下为正(屏幕坐标系),false:Y轴向上为正(一般的坐标系)
            var isYAxixToDown = false;

            int i, j, k;
            int flag = 0;
            double z;

            if (points == null || points.Count < 3)
            {
                return (0);
            }
            int n = points.Count;
            int yTrans = isYAxixToDown ? (-1) : (1);
            for (i = 0; i < n; i++)
            {
                j = (i + 1) % n;
                k = (i + 2) % n;
                z = (points[j].X - points[i].X) * (points[k].Y * yTrans - points[j].Y * yTrans);
                z -= (points[j].Y * yTrans - points[i].Y * yTrans) * (points[k].X - points[j].X);
                if (z < 0)
                {
                    flag |= 1;
                }
                else if (z > 0)
                {
                    flag |= 2;
                }
                if (flag == 3)
                {
                    return (PolygonType.Concave);
                }
            }
            if (flag != 0)
            {
                return (PolygonType.Convex);
            }
            else
            {
                return (PolygonType.None);
            }

        }

        #endregion

    }
}
