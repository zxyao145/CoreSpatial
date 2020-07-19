using CoreSpatial.Analysis.Tin;
using CoreSpatial.Converter;
using System;
using CoreSpatial.Analysis;

namespace CoreSpatial.Analysis.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Convex();
            Console.WriteLine();
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }

        static void Convex()
        {
            var fs = FeatureSet.Open(@"./data/points.shp");
            Console.WriteLine(fs.ToGeoJSON());

            Console.WriteLine();
            Console.WriteLine(new string('=',50));
            Console.WriteLine();

            var convex = fs.Convex();
            var convexJson = convex.ToGeoJSON();

            var a = convexJson == @"{""features"":[{""geometry"":{""coordinates"":[[[115,30],[110,30],[110,33],[113,35],[115,30],[115,30]]],""type"":""Polygon""},""properties"":{},""type"":""Feature""}],""bbox"":[110,30,115,35],""type"":""FeatureCollection""}";
            Console.WriteLine(convexJson);
            Console.WriteLine(a);
        }

        static void Tin()
        {
            var fs = FeatureSet.Open(@"./data/points.shp");

            var tin = fs.Tin();
            var fecc = tin.ToGeoJSON();
            Console.WriteLine(fs.ToGeoJSON());
            Console.WriteLine(fecc);

        }
    }
}
