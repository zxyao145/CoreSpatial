using System;
using System.IO;

namespace CoreSpatial.Converter.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //you can see geojson result in http://geojson.io/
            GeoJSONTest();
            KmlAndKmzTest();
            Console.WriteLine("Finished!");
            Console.Read();
        }

        #region Kml

        static void KmlAndKmzTest()
        {
            string[] files = Directory.GetFiles("../测试Data", "*.shp");

            foreach (var file in files)
            {
                Console.WriteLine(new string('*', 100));
                Console.WriteLine(Path.GetFileName(file));
                Kml1Test(file);
            }
        }

        static void Kml1Test(string shpPath)
        {
            IFeatureSet fs = FeatureSet.Open(shpPath);
            var kmlName = Path.GetFileNameWithoutExtension(shpPath);
            var kml = fs.ToKML(kmlName);
            fs.ToKMZ(kmlName, Path.ChangeExtension(shpPath,".kmz"));

            Console.WriteLine(kml);
            var kmlPath = Path.ChangeExtension(shpPath, ".kml");
            if (File.Exists(kmlPath))
            {
                File.Delete(kmlPath);
            }

            using var sw = new StreamWriter(kmlPath, false);
            sw.WriteLine(kml);
        }

        #endregion

        #region GeoJSONTest

        static void GeoJSONTest()
        {
            string[] files = Directory.GetFiles("../测试Data", "*.shp");

            foreach (var file in files)
            {
                Console.WriteLine(new string('*', 100));
                Console.WriteLine(Path.GetFileName(file));
                GeoJSON1Test(file);
            }
        }

        static void GeoJSON1Test(string shpPath)
        {
            IFeatureSet fs = FeatureSet.Open(shpPath);
            var geoJson = fs.ToGeoJSON(false);
            Console.WriteLine(geoJson);
            var geojsonPath = Path.ChangeExtension(shpPath, ".json");
            if (File.Exists(geojsonPath))
            {
                File.Delete(geojsonPath);
            }

            using var sw = new StreamWriter(geojsonPath, false);
            sw.WriteLine(geoJson);
        }

        #endregion
    }
}
