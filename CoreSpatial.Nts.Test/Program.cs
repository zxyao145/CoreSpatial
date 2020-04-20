using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using CoreSpatial.BasicGeometrys;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace CoreSpatial.Nts.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadTest();
            Console.WriteLine("Hello World!");
        }

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
        }

        static void ReadTest1Shp(string shpPath)
        {
            IFeatureSet fs = FeatureSet.Open(shpPath);

            var ntsFec = fs.ToNtsFeatureCollection();

            var newShpPath = Path.Combine(
                Path.GetDirectoryName(shpPath),
                Path.GetFileNameWithoutExtension(shpPath) + "_new.shp");

            var newFs = ntsFec.ToCsFeatureSet();
            newFs.Save(newShpPath);



            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            var serializer = GeoJsonSerializer.CreateDefault();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Serialize(writer, ntsFec);
            writer.Flush();


            Console.WriteLine(writer.ToString());
            writer.Dispose();
            Console.WriteLine();
        }
    }
}
