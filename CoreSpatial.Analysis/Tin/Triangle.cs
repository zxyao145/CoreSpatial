using CoreSpatial.BasicGeometrys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

/// <summary>
/// translated from turf
/// </summary>
namespace CoreSpatial.Analysis.Tin
{
    public class Triangle
    {
        public TrianglePoint A { get; private set; }
        public TrianglePoint B { get; private set; }
        public TrianglePoint C { get; private set; }

        public double X { get; private set; }
        public double Y { get; private set; }
        public double R { get; private set; }

        public Triangle(TrianglePoint a, TrianglePoint b, TrianglePoint c)
        {
            this.A = a;
            this.B = b;
            this.C = c;

            var baX = B.X - A.X;
            var baY = B.Y - A.Y;
            var caZ = C.X - A.X;
            var caY = C.Y - A.Y;
            var e = baX * (A.X + B.X) + baY * (A.Y + B.Y);
            var f = caZ * (A.X + C.X) + caY * (A.Y + C.Y);
            var g = 2 * (baX * (C.Y - B.Y) - baY * (C.X - B.X));

            // If the points of the triangle are collinear, then just find the
            // extremes and use the midpoint as the center of the circumcircle.
            this.X = (caY * e - baY * f) / g;
            this.Y = (baX * f - caZ * e) / g;
            var dx = this.X - A.X;
            var dy = this.Y - A.Y;
            this.R = dx * dx + dy * dy;
        }
    }

    public class TrianglePoint
    {
        public TrianglePoint(GeoPoint geoPoint)
        {
            this.Point = geoPoint;
        }

        public TrianglePoint(GeoPoint geoPoint,bool sentinel):this(geoPoint)
        {
            this.Sentinel = sentinel;
        }

        public GeoPoint Point {  get; private set; }

        public DataRow DataRow { get; set; }

        public bool Sentinel { get; set; }

        /// <summary>
        /// X坐标
        /// </summary>
        public double X
        {
            get => Point.X;
            set => Point.X = value;
        }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y
        {
            get => Point.Y;
            set => Point.Y = value;
        }

        /// <summary>
        /// Z坐标
        /// </summary>
        public double Z
        {
            get => Point.Z;
            set => Point.Z = value;
        }

        public int Fid { get; internal set; }
    }

    public class Edge
    {
        public GeoPoint Point1 { get; set; }

        public GeoPoint Point2 { get; set; }

        public Edge(GeoPoint point1,GeoPoint point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
    }
}
