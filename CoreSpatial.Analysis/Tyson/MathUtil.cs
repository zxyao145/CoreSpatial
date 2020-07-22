using System;
using CoreSpatial.BasicGeometrys;

namespace CoreSpatial.Analysis.Tyson
{
    public class MathUtil
    {
        /// <summary>
        /// 计算两点的斜率
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static double GetXieLv(GeoPoint pt1, GeoPoint pt2)
        {
            if (Math.Abs(pt2.X - pt1.X) < double.Epsilon)
            {
                return double.NaN;
            }
            double pl = (pt2.Y - pt1.Y) / (pt2.X - pt1.X);

            return pl;
        }

        /// <summary>
        /// 计算两条直线的交点
        /// </summary>
        /// <param name="lineFirstStar">L1的点1坐标</param>
        /// <param name="lineFirstEnd">L1的点2坐标</param>
        /// <param name="lineSecondStar">L2的点1坐标</param>
        /// <param name="lineSecondEnd">L2的点2坐标</param>
        /// <returns></returns>
        public static GeoPoint GetIntersection
        (GeoPoint lineFirstStar, GeoPoint lineFirstEnd,
            GeoPoint lineSecondStar, GeoPoint lineSecondEnd)
        {
            /*
             * L1，L2都存在斜率的情况：
             * 直线方程L1: ( y - y1 ) / ( y2 - y1 ) = ( x - x1 ) / ( x2 - x1 ) 
             * => y = [ ( y2 - y1 ) / ( x2 - x1 ) ]( x - x1 ) + y1
             * 令 a = ( y2 - y1 ) / ( x2 - x1 )
             * 有 y = a * x - a * x1 + y1   .........1
             * 直线方程L2: ( y - y3 ) / ( y4 - y3 ) = ( x - x3 ) / ( x4 - x3 )
             * 令 b = ( y4 - y3 ) / ( x4 - x3 )
             * 有 y = b * x - b * x3 + y3 ..........2
             * 
             * 如果 a = b，则两直线平等，否则， 联解方程 1,2，得:
             * x = ( a * x1 - b * x3 - y1 + y3 ) / ( a - b )
             * y = a * x - a * x1 + y1
             * 
             * L1存在斜率, L2平行Y轴的情况：
             * x = x3
             * y = a * x3 - a * x1 + y1
             * 
             * L1 平行Y轴，L2存在斜率的情况：
             * x = x1
             * y = b * x - b * x3 + y3
             * 
             * L1与L2都平行Y轴的情况：
             * 如果 x1 = x3，那么L1与L2重合，否则平等
             * 
            */
            double a = 0, b = 0;
            int state = 0;
            if (Math.Abs(lineFirstStar.X - lineFirstEnd.X) > double.Epsilon)
            {
                a = (lineFirstEnd.Y - lineFirstStar.Y)
                    / (lineFirstEnd.X - lineFirstStar.X);
                state |= 1;
            }
            if (Math.Abs(lineSecondStar.X - lineSecondEnd.X) > double.Epsilon)
            {
                b = (lineSecondEnd.Y - lineSecondStar.Y)
                    / (lineSecondEnd.X - lineSecondStar.X);
                state |= 2;
            }
            switch (state)
            {
                case 0: //L1与L2都平行Y轴
                    {
                        if (Math.Abs(lineFirstStar.X - lineSecondStar.X)
                            < double.Epsilon)
                        {
                            //throw new Exception("两条直线互相重合，且平行于Y轴，无法计算交点。");
                            return new GeoPoint(0, 0);
                        }
                        else
                        {
                            //throw new Exception("两条直线互相平行，且平行于Y轴，无法计算交点。");
                            return new GeoPoint(0, 0);
                        }
                    }
                case 1: //L1存在斜率, L2平行Y轴
                    {
                        double x = lineSecondStar.X;
                        double y = (lineFirstStar.X - x) * (-a) + lineFirstStar.Y;
                        return new GeoPoint(x, y);
                    }
                case 2: //L1 平行Y轴，L2存在斜率
                    {
                        var x = lineFirstStar.X;
                        //网上有相似代码的，这一处是错误的。你可以对比case 1 的逻辑 进行分析
                        //源code:lineSecondStar * x + lineSecondStar * lineSecondStar.X + p3.Y;
                        var y = (lineSecondStar.X - x) * (-b) + lineSecondStar.Y;
                        return new GeoPoint(x, y);
                    }
                case 3: //L1，L2都存在斜率
                    {
                        if (Math.Abs(a - b) < double.Epsilon)
                        {
                            // throw new Exception("两条直线平行或重合，无法计算交点。");
                            return new GeoPoint(0, 0);
                        }
                        var x = (a * lineFirstStar.X - b * lineSecondStar.X - lineFirstStar.Y + lineSecondStar.Y) / (a - b);
                        var y = a * x - a * lineFirstStar.X + lineFirstStar.Y;
                        return new GeoPoint(x, y);
                    }
            }
            // throw new Exception("不可能发生的情况");
            return new GeoPoint(0, 0);
        }


        #region 距离

        /// <summary>
        /// 垂线距离
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        internal virtual double GetVerticalDistance
            (GeoPoint pt, GraphEdge line) //所在点到AB线段的垂线长度
        {
            var x = pt.X;
            var y = pt.Y;
            var x1 = line.Start.X;
            var y1 = line.Start.Y;
            var x2 = line.End.X;
            var y2 = line.End.Y;


            var reVal = 0.0;
            bool retData = false;

            var cross = (x2 - x1) * (x - x1) + (y2 - y1) * (y - y1);
            if (cross <= 0)
            {
                reVal = Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1));
                retData = true;
            }

            var d2 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
            if (cross >= d2)
            {
                reVal = Math.Sqrt((x - x2) * (x - x2) + (y - y2) * (y - y2));
                retData = true;
            }

            if (!retData)
            {
                var r = cross / d2;
                var px = x1 + (x2 - x1) * r;
                var py = y1 + (y2 - y1) * r;
                reVal = Math.Sqrt((x - px) * (x - px) + (py - y) * (py - y));
            }

            return reVal;
        }

        /// <summary>
        /// 线段距离最短
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double PointToSegDist(double x, double y,
            double x1, double y1, double x2, double y2)
        {

            double cross = (x2 - x1) * (x - x1) + (y2 - y1) * (y - y1);
            if (cross <= 0) return Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1));

            double d2 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);

            if (cross >= d2) return Math.Sqrt((x - x2) * (x - x2) + (y - y2) * (y - y2));


            double r = cross / d2;

            double px = x1 + (x2 - x1) * r;

            double py = y1 + (y2 - y1) * r;

            return Math.Sqrt((x - px) * (x - px) + (py - y1) * (py - y1));

        }

        /// <summary>
        /// 计算点到Voronoi Edge的距离
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        internal static double Distance2ToMidPoint(GeoPoint pt, GraphEdge edge)
        {
            var midPt = new GeoPoint(
                (edge.Start.X + edge.End.X) / 2,
                (edge.Start.Y + edge.End.Y) / 2
            );

            var distance2 = Math.Pow((pt.X - midPt.X), 2)
                            + Math.Pow((pt.Y - midPt.Y), 2);
            return distance2;
        }

        #endregion
    }
}
