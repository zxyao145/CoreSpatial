﻿using System.Collections.Generic;
using System.Linq;
using CoreSpatial.BasicGeometrys;

namespace CoreSpatial
{
    public class Geometry: IGeometry
    {
        public List<GeoPoint> Points
        {
            #region MyRegion
            //get
            //{
            //    if (BasicGeometry is GeoPoint point)
            //    {
            //        return new List<GeoPoint>()
            //        {
            //           point
            //        };
            //    }
            //    else if (BasicGeometry is MultiPoint multiPoint)
            //    {
            //        return multiPoint.Points;
            //    }
            //    else
            //    {
            //        if (BasicGeometry is MultiPolyLine multiPolyLine)
            //        {
            //            var points =
            //                (from line in multiPolyLine.PolyLines
            //                 from pointTemp in line.Points
            //                 select pointTemp).ToList();
            //            return points;
            //        }
            //        return new List<GeoPoint>();
            //    }
            //} 
            #endregion

            get;
            set;
        }

        public GeometryType GeometryType => BasicGeometry.GeometryType;

        public IBasicGeometry BasicGeometry { get; set; }
    }
}
