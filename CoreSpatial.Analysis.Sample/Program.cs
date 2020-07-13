using CoreSpatial.Analysis.Tin;
using CoreSpatial.Converter;
using System;

namespace CoreSpatial.Analysis.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var fs = FeatureSet.Open(@"D:\Users\admin\Documents\1111测试Data\point-tyson.shp");
            var tinBuilder = new TinBuilder();

            var tin = tinBuilder.Build(fs);
            var fecc = tin.ToGeoJSON();
            Console.WriteLine(fs.ToGeoJSON());

            Console.WriteLine();

            Console.WriteLine(fecc);
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
