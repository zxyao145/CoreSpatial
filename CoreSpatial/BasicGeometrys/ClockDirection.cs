using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSpatial.BasicGeometrys
{
    /// <summary>
    /// 时钟方向
    /// </summary>
    public enum ClockDirection
    {
        /// <summary>
        /// 无.可能是不可计算的图形，比如多点共线
        /// </summary>
        None,

        /// <summary>
        /// 顺时针方向
        /// </summary>
        Clockwise,

        /// <summary>
        /// 逆时针方向
        /// </summary>
        Counterclockwise
    }


    /// <summary>
    /// 多边形凹凸性
    /// </summary>
    public enum PolygonType
    {
        /// <summary>
        /// 无，不可计算的多边形(比如多点共线)
        /// </summary>
        None,

        /// <summary>
        /// 凸多边形
        /// </summary>
        Convex,

        /// <summary>
        /// 凹多边形
        /// </summary>
        Concave

    }
}
