using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSpatial.Analysis
{
   public static  class Extensions
    {
        public static void Add<T>(this List<T> list, params T[] objs)
        {
            foreach (var item in objs)
            {
                list.Add(item);
            }
        }
    }
}
