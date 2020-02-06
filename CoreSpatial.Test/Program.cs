using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using CoreSpatial;
using CoreSpatial.BasicGeometrys;

namespace CoreSpatial.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadTest();

            SaveTest();

            Console.WriteLine();
            Console.WriteLine("Finish!");
            Console.ReadLine();
        }

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

        static void ReadTest()
        {
            string[] files = Directory.GetFiles("../测试Data", "*.shp");

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
                foreach (var point in fe.Geometry.Points)
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
    }
}
