using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using CoreSpatial;
using CoreSpatial.BasicGeometrys;
using CoreSpatial.CrsNs;

namespace CoreSpatial.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //ReadTest();
            //SaveTest();
            //CreateNew();

            SaveByStream();

            Console.WriteLine();
            Console.WriteLine("Finish!");
            Console.ReadLine();
        }

        static void SaveByStream()
        {
            void CreatePoint()
            {
                IFeatureSet fs = new FeatureSet(FeatureType.Point);
                var point1 = new GeoPoint(133, 30);
                var point2 = new GeoPoint(133, 32);
                var point3 = new GeoPoint(134, 30);

                var feature1 = new Feature(new Geometry(point1));
                var feature2 = new Feature(new Geometry(point2));
                var feature3 = new Feature(new Geometry(point3));

                fs.Features.Add(feature1);
                fs.Features.Add(feature2);
                fs.Features.Add(feature3);
                fs.Crs = Crs.Wgs84Gcs;

                var dataTable = new DataTable();
                dataTable.Columns.Add("名称", typeof(string));
                dataTable.Columns.Add("id", typeof(int));
                var row1 = dataTable.NewRow();
                var row2 = dataTable.NewRow();
                var row3 = dataTable.NewRow();

                row1[0] = "点1";
                row1[1] = 1;

                row2[0] = "点2";
                row2[1] = 2;

                row3[0] = "点3";
                row3[1] = 3;

                dataTable.Rows.Add(row1);
                dataTable.Rows.Add(row2);
                dataTable.Rows.Add(row3);

                fs.AttrTable = dataTable;

                var fileBytes = fs.GetShapeFileBytes();
                using var shpFile = new FileStream("../createNew/point_stream.shp",FileMode.OpenOrCreate,FileAccess.Write,FileShare.Read);
                using var shxFile = new FileStream("../createNew/point_stream.shx", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                using var dbfFile = new FileStream("../createNew/point_stream.dbf", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                using var prjFile = new FileStream("../createNew/point_stream.prj", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                shpFile.Write(fileBytes.ShpBytes);
                shxFile.Write(fileBytes.ShxBytes);
                dbfFile.Write(fileBytes.DbfBytes);
                prjFile.Write(fileBytes.PrjBytes);
            }

            CreatePoint();
        }


        public static void CreateNew()
        {
            void CreatePoint()
            {
                IFeatureSet fs = new FeatureSet(FeatureType.Point);
                var point1 = new GeoPoint(133, 30);
                var point2 = new GeoPoint(133, 32);
                var point3 = new GeoPoint(134, 30);

                var feature1 = new Feature(new Geometry(point1));
                var feature2 = new Feature(new Geometry(point2));
                var feature3 = new Feature(new Geometry(point3));

                fs.Features.Add(feature1);
                fs.Features.Add(feature2);
                fs.Features.Add(feature3);
                fs.Crs = Crs.Wgs84Gcs;

                var dataTable = new DataTable();
                dataTable.Columns.Add("名称", typeof(string));
                dataTable.Columns.Add("id", typeof(int));
                var row1 = dataTable.NewRow();
                var row2 = dataTable.NewRow();
                var row3 = dataTable.NewRow();

                row1[0] = "点1";
                row1[1] = 1;

                row2[0] = "点2";
                row2[1] = 2;
                
                row3[0] = "点3";
                row3[1] = 3;

                dataTable.Rows.Add(row1);
                dataTable.Rows.Add(row2);
                dataTable.Rows.Add(row3);

                fs.AttrTable = dataTable;

                fs.Save("../createNew/point.shp");
            }

            void CreateMultiPoint()
            {
                IFeatureSet fs = new FeatureSet(FeatureType.MultiPoint);

                var point1 = new GeoPoint(133, 30);
                var point2 = new GeoPoint(133, 32);
                var point3 = new GeoPoint(134, 30);

                var point4 = new GeoPoint(134, 34);
                var point5 = new GeoPoint(135, 35);

                var multiPoint1 = new MultiPoint(new List<GeoPoint>()
                {
                    point1,point2,point3
                });

                var multiPoint2 = new MultiPoint();
                multiPoint2.Points.Add(point4);
                multiPoint2.Points.Add(point5);
                

                var feature1 = new Feature(new Geometry(multiPoint1));
                var feature2 = new Feature(new Geometry(multiPoint2));

                fs.Features.Add(feature1);
                fs.Features.Add(feature2);
                fs.Crs = Crs.Wgs84Gcs;

                var dataTable = new DataTable();
                dataTable.Columns.Add("名称", typeof(string));
                dataTable.Columns.Add("id", typeof(int));
                var row1 = dataTable.NewRow();
                var row2 = dataTable.NewRow();

                row1[0] = "多点1";
                row1[1] = 1;

                row2[0] = "多点2";
                row2[1] = 2;

                dataTable.Rows.Add(row1);
                dataTable.Rows.Add(row2);

                fs.AttrTable = dataTable;

                fs.Save("../createNew/multipoint.shp");
            }

            void CreatePolyLine()
            {
                
                IFeatureSet fs = new FeatureSet(FeatureType.PolyLine);

                var point1 = new GeoPoint(133, 30);
                var point2 = new GeoPoint(133, 32);
                var point3 = new GeoPoint(134, 30);

                var point4 = new GeoPoint(134, 34);
                var point5 = new GeoPoint(135, 35);

                //line ring
                var polyLine1 = new PolyLine(new List<GeoPoint>()
                {
                    point1,point2,point3,point1
                });
                var isLineRing = polyLine1.IsLineRing;

                var polyLine2 = new PolyLine(new List<GeoPoint>()
                {
                    point4,point5
                });
                
                var feature1 = new Feature(new Geometry(polyLine1));
                var feature2 = new Feature(new Geometry(polyLine2));

                fs.Features.Add(feature1);
                fs.Features.Add(feature2);
                fs.Crs = Crs.Wgs84Gcs;

                var dataTable = new DataTable();
                dataTable.Columns.Add("名称", typeof(string));
                dataTable.Columns.Add("id", typeof(int));
                var row1 = dataTable.NewRow();
                var row2 = dataTable.NewRow();

                row1[0] = "线1";
                row1[1] = 1;

                row2[0] = "线2";
                row2[1] = 2;

                dataTable.Rows.Add(row1);
                dataTable.Rows.Add(row2);

                fs.AttrTable = dataTable;

                fs.Save("../createNew/polyline.shp");
            }

            void CreateMultiPolyLine()
            {
                IFeatureSet fs = new FeatureSet(FeatureType.PolyLine);

                var point1 = new GeoPoint(132, 30);
                var point2 = new GeoPoint(136, 30);
                var point3 = new GeoPoint(134, 28);
                var point4 = new GeoPoint(134, 32);

                var polyLine1 = new PolyLine(new List<GeoPoint>()
                {
                    point1,point2
                });

                var polyLine2 = new PolyLine(new List<GeoPoint>()
                {
                    point3,point4
                });

                var multiPolyLine1 = new MultiPolyLine(new List<PolyLine>()
                {
                    polyLine1,polyLine2
                });
               
                var feature1 = new Feature(new Geometry(multiPolyLine1));

                fs.Features.Add(feature1);
                fs.Crs = Crs.Wgs84Gcs;

                var dataTable = new DataTable();
                dataTable.Columns.Add("名称", typeof(string));
                dataTable.Columns.Add("id", typeof(int));
                var row1 = dataTable.NewRow();

                row1[0] = "十字多线";
                row1[1] = 1;

                dataTable.Rows.Add(row1);

                fs.AttrTable = dataTable;

                fs.Save("../createNew/multiPolyLine.shp");
            }

            void CreatePolygon()
            {
                IFeatureSet fs = new FeatureSet(FeatureType.Polygon);

                var point1 = new GeoPoint(132, 30);
                var point2 = new GeoPoint(136, 30);
                var point3 = new GeoPoint(132, 35);
                
                var edge = new PolyLine(new List<GeoPoint>()
                {
                    point1, point2, point3, point1
                });
                
                var polygon = new Polygon(edge);

                //edge，Clockwise
                var point4 = new GeoPoint(140, 30);
                var point5 = new GeoPoint(140, 35);
                var point6 = new GeoPoint(150, 35);
                var point7 = new GeoPoint(150, 30);
                var edge2 = new PolyLine(new List<GeoPoint>()
                {
                    point4, point5, point6, point7, point4
                });

                //points sketch 
                //              .(point9)
                //
                //  .(point8)          .(point10)
                //
                var point8 = new GeoPoint(142, 31);
                var point9 = new GeoPoint(145, 33);
                var point10 = new GeoPoint(148, 31);

                //hole, need Counterclockwise,
                //if Counterclockwise, sequence should be:
                //point8, point10, point9
                //It will be handled inside the library.
                //It will be a line ring inside the library too
                var hole = new PolyLine(new List<GeoPoint>()
                {
                    point8,point9,point10
                });
                var polygon2 = new Polygon(edge2, hole);


                var feature1 = new Feature(new Geometry(polygon));
                var feature2 = new Feature(new Geometry(polygon2));

                fs.Features.Set(new List<IFeature>()
                {
                    feature1,
                    feature2
                });
                fs.Crs = Crs.Wgs84Gcs;


                var dataTable = new DataTable();
                dataTable.Columns.Add("名称", typeof(string));
                dataTable.Columns.Add("id", typeof(int));
                var row1 = dataTable.NewRow();
                var row2 = dataTable.NewRow();

                row1[0] = "简单面";
                row1[1] = 1;

                row2[0] = "带洞面";
                row2[1] = 2;

                dataTable.Rows.Add(row1);
                dataTable.Rows.Add(row2);

                fs.AttrTable = dataTable;

                fs.Save("../createNew/polygon.shp");
            }

            CreatePoint();
            CreateMultiPoint();
            CreatePolyLine();
            CreateMultiPolyLine();
            CreatePolygon();

        }

        #region Read and Save

        static void SaveTest()
        {
            string[] files = Directory.GetFiles("../测试Data",
                "*.shp");

            foreach (var shpPath in files)
            {
                Console.WriteLine(Path.GetFileName(shpPath));

                IFeatureSet fs = FeatureSet.Open(shpPath);
                Console.WriteLine(new string('*', 100));

                var newFile = Path.Combine(Path.GetDirectoryName(shpPath), "../saveTest/" + Path.GetFileName(shpPath));

                fs.Save(newFile);
            }
        }

        #endregion

        #region Read

        static void ReadTest()
        {
            string[] files = Directory.GetFiles("../测试Data", "*.shp");

            Console.WriteLine("Read From File");
            foreach (var file in files)
            {
                Console.WriteLine(new string('*', 100));
                Console.WriteLine(Path.GetFileName(file));
                ReadTest1Shp(file);
                foreach (var item in Enumerable.Range(1, 5))
                {
                    Console.Write(Environment.NewLine);
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Read From Stream");
            ReadTest1Shp(files[0]);
            ReadFromStream(files[0]);


        }

        static void ReadTest1Shp(string shpPath)
        {
            IFeatureSet fs = FeatureSet.Open(shpPath);
            Console.WriteLine("FeatureType：" + fs.FeatureType);
            Console.WriteLine();

            var dataTable = fs.AttrTable;

            Console.WriteLine("属性表字段：");
            foreach (DataColumn col in dataTable.Columns)
            {
                Console.Write(col.ColumnName + "\t");
            }
            Console.WriteLine();
            Console.WriteLine(new string('=', 50));

            Console.WriteLine();
            foreach (var fe in fs.Features)
            {
                Console.WriteLine();
                Console.WriteLine("GeometryType：" + fe.GeometryType);
                if (fe.GeometryType == GeometryType.MultiPolyLine)
                {
                    MultiPolyLine multiPolyLine = fe.Geometry.BasicGeometry as MultiPolyLine;
                    if (multiPolyLine != null)
                        Console.WriteLine("PartsNum：" + multiPolyLine.PartsNum);
                    Console.WriteLine("起始点：");
                    if (multiPolyLine != null)
                    {
                        foreach (var line in multiPolyLine.PolyLines)
                        {
                            Console.WriteLine(line[0]);
                        }
                    }
                }

                Console.WriteLine("\r\n所有点信息：");
                foreach (var point in fe.Geometry.Coordinates)
                {
                    Console.Write($"{point.X},{point.Y}\t");
                }
                Console.WriteLine("\r\n属性信息：");

                var datarow = fe.DataRow.ItemArray;
                foreach (var o in datarow)
                {
                    Console.Write(o + "\t");
                }
                Console.WriteLine();
                Console.WriteLine(new string('-', 20));
                Console.WriteLine();

            }
        }

        static void ReadFromStream(string shpPath)
        {
            var shx = Path.ChangeExtension(shpPath, ".shx");
            var dbf = Path.ChangeExtension(shpPath, ".dbf");

            var shpStream =
                new FileStream(shpPath, FileMode.Open,
                    FileAccess.Read, FileShare.Read);

            var shxStream =
                new FileStream(shx, FileMode.Open,
                    FileAccess.Read, FileShare.Read);
            var dbfStream =
                new FileStream(dbf, FileMode.Open,
                    FileAccess.Read, FileShare.Read);

            IFeatureSet fs = FeatureSet.Open(shpStream, shxStream, dbfStream);
            Console.WriteLine("FeatureType：" + fs.FeatureType);
            Console.WriteLine();

            var dataTable = fs.AttrTable;

            Console.WriteLine("属性表字段：");
            foreach (DataColumn col in dataTable.Columns)
            {
                Console.Write(col.ColumnName + "\t");
            }
            Console.WriteLine();
            Console.WriteLine(new string('=', 50));

            Console.WriteLine();
            foreach (var fe in fs.Features)
            {
                Console.WriteLine();
                Console.WriteLine("GeometryType：" + fe.GeometryType);
                if (fe.GeometryType == GeometryType.MultiPolyLine)
                {
                    MultiPolyLine multiPolyLine = fe.Geometry.BasicGeometry as MultiPolyLine;
                    if (multiPolyLine != null)
                        Console.WriteLine("PartsNum：" + multiPolyLine.PartsNum);
                    Console.WriteLine("起始点：");
                    if (multiPolyLine != null)
                    {
                        foreach (var line in multiPolyLine.PolyLines)
                        {
                            Console.WriteLine(line[0]);
                        }
                    }
                }

                Console.WriteLine("\r\n所有点信息：");
                foreach (var point in fe.Geometry.Coordinates)
                {
                    Console.Write($"{point.X},{point.Y}\t");
                }
                Console.WriteLine("\r\n属性信息：");

                var datarow = fe.DataRow.ItemArray;
                foreach (var o in datarow)
                {
                    Console.Write(o + "\t");
                }
                Console.WriteLine();
                Console.WriteLine(new string('-', 20));
                Console.WriteLine();

            }
        }
        #endregion
    }
}
