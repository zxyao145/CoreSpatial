using CoreSpatial.BasicGeometrys;
using CoreSpatial.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace CoreSpatial.Analysis.Tin
{
    internal class TinBuilder
    {
        public FeatureSet Build(IFeatureSet featureSet)
        {
            if(featureSet.FeatureType != FeatureType.Point)
            {
                return null;
            }
            
            var features = featureSet.Features;
            var vertices = features.Select(e =>
                new TrianglePoint((GeoPoint)e.Geometry.BasicGeometry)
                {
                    DataRow = e.DataRow,
                    Fid = e.Fid
                }).ToList();

            var taiangleFs = new FeatureSet(FeatureType.Polygon);
            var taiangleFes = taiangleFs.Features;

            var dataTable = new DataTable();
            dataTable.Columns.Add("p1_id", typeof(int));
            dataTable.Columns.Add("p2_id", typeof(int));
            dataTable.Columns.Add("p3_id", typeof(int));

            var taiangles = Triangulate(vertices);
            if(taiangles != null)
            {
                foreach (var taiangle in taiangles)
                {
                    var p1 = taiangle.A.Point;
                    var p2 = taiangle.B.Point;
                    var p3 = taiangle.C.Point;

                    Polygon polygon = new Polygon(
                        new PolyLine(new List<GeoPoint>()
                        {
                        p1,p2,p3
                        }));

                    var taiangleFe = new Feature(new Geometry(polygon));
                    taiangleFes.Add(taiangleFe);

                    var newRow = dataTable.NewRow();
                    newRow[0] = taiangle.A.Fid;
                    newRow[1] = taiangle.B.Fid;
                    newRow[2] = taiangle.C.Fid;
                    dataTable.Rows.Add(newRow);
                }

                taiangleFs.AttrTable = dataTable;

                return taiangleFs;
            }

            return null;
        }

        private void Dedup(List<TrianglePoint> edges)
        {
            var j = edges.Count;
            TrianglePoint a;
            TrianglePoint b;
            int i;
            TrianglePoint m;
            TrianglePoint n;

            outer:
            while (j > 0)
            {
                b = edges[--j];
                a = edges[--j];
                i = j;
                while (i > 0)
                {
                    n = edges[--i];
                    m = edges[--i];
                    if ((a == m && b == n) || (a == n && b == m))
                    {
                        edges.RemoveRange(j, 2);
                        edges.RemoveRange(i, 2);
                        j -= 2;
                        goto outer;
                    }
                }
            }
        }

        private List<Triangle> Triangulate(IList<TrianglePoint> vertices)
        {
            if(vertices.Count < 3)
            {
                return null;
            }
            vertices = vertices.OrderBy(e => e.X).ToList();

            var i = vertices.Count - 1;
            var xmin = vertices[i].X;
            var xmax = vertices[0].Y;
            var ymin = vertices[i].Y;
            var ymax = ymin;
            var epsilon = Util.DValue;//1e-12;

            while (i-- > 0)
            {
                if (vertices[i].Y < ymin)
                {
                    ymin = vertices[i].Y;
                }
                if (vertices[i].Y > ymax)
                {
                    ymax = vertices[i].Y;
                }
            }

            var dx = xmax - xmin;
            var dy = ymax - ymin;
            var dmax = (dx > dy) ? dx : dy;
            var xmid = (xmax + xmin) * 0.5;
            var ymid = (ymax + ymin) * 0.5;
            List<Triangle> open = new List<Triangle>()
            {
                 new Triangle(
                        new TrianglePoint(
                            new GeoPoint(xmid - 20 * dmax, ymid - dmax), true) ,
                        new TrianglePoint(
                            new GeoPoint(xmid, ymid + 20 * dmax), true),
                        new TrianglePoint(
                            new GeoPoint(xmid + 20 * dmax, ymid - dmax), true)
                        )
            };

            List<Triangle> closed = new List<Triangle>();
            List<TrianglePoint> edges = new List<TrianglePoint>();
            //const edges: any = [];
            int j;

            i = vertices.Count;

            TrianglePoint a;
            TrianglePoint b;
            TrianglePoint c;
            double A, B, G;

            while (i-- > 0)
            {
                // For each open triangle, check to see if the current point is
                // inside it's circumcircle. If it is, remove the triangle and add
                // it's edges to an edge list.
                edges.Clear();

                j = open.Count;
                while (j-- > 0)
                {
                    // If this point is to the right of this triangle's circumcircle,
                    // then this triangle should never get checked again. Remove it
                    // from the open list, add it to the closed list, and skip.
                    dx = vertices[i].X - open[j].X;
                    if (dx > 0 && dx * dx > open[j].R)
                    {
                        closed.Add(open[j]);
                        open.RemoveAt(j);
                        continue;
                    }

                    // If not, skip this triangle.
                    dy = vertices[i].Y - open[j].Y;
                    if (dx * dx + dy * dy > open[j].R)
                    {
                        continue;
                    }

                    // Remove the triangle and add it's edges to the edge list.
                    edges.Add(
                        open[j].A, open[j].B,
                        open[j].B, open[j].C,
                        open[j].C, open[j].A
                    );
                    open.RemoveAt(j);
                }

                // Remove any doubled edges.
                Dedup(edges);

                // Add a new triangle for each edge.

                j = edges.Count;
                while (j>0)
                {
                    b = edges[--j];
                    a = edges[--j];
                    c = vertices[i];
                    // Avoid adding colinear triangles (which have error-prone
                    // circumcircles)
                    A = b.X - a.X;
                    B = b.Y - a.Y;
                    G = 2 * (A * (c.Y - b.Y) - B * (c.X - b.X));
                    if (Math.Abs(G) > epsilon)
                    {
                        open.Add(new Triangle(a, b, c));
                    }
                }
            }

            // Copy any remaining open triangles to the closed list, and then
            // remove any triangles that share a vertex with the supertriangle.
            closed.AddRange(open);


            i = closed.Count;
            while (i-- > 0)
            {
                if (closed[i].A.Sentinel ||
                    closed[i].B.Sentinel ||
                    closed[i].C.Sentinel)
                {
                    closed.RemoveAt(i);
                }
            }

            return closed;
        }
    }
}

