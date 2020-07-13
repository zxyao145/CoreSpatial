using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSpatial.Utility
{
    public class Util
    {
        /// <summary>
        /// epsilon 判断两个double是否相等的差值极限，此用以判断坐标的X、Y值
        /// </summary>
        public static double DValue => 1e-8d;
    }
}
