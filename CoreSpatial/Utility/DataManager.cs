using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using CoreSpatial.DbfOper;
using CoreSpatial.ShpOper;

namespace CoreSpatial.Utility
{
    internal class DataManager
    {
        private string shpDir;
        private string shpNameWithoutExtension;
        private DbfReader _dbfReader;
        private ShpReader _shpReader;
        private readonly int recordNum;

        public DataManager(string shpFilePath,Encoding encoding)
        {
            shpDir = Path.GetDirectoryName(shpFilePath);
            shpNameWithoutExtension = Path.GetFileNameWithoutExtension(shpFilePath);
            if (!ShpUtility.VerificationShp(shpDir, shpNameWithoutExtension))
            {
                throw new IOException("该shapefile文件不存在或者主文件缺失！");
            }
            //else
            _dbfReader = new DbfReader(Path.Combine(shpDir, shpNameWithoutExtension + ".dbf"), encoding);
            var shxIndexs  = new ShxReader().ReadShx(Path.Combine(shpDir, shpNameWithoutExtension + ".shx"));
            recordNum = shxIndexs.Count;
            _shpReader = new ShpReader(shpFilePath, shxIndexs);
        }


        public FeatureSet CreateFeatureSet()
        {
            GeometryType geometryType = (GeometryType)_shpReader.ShpHeader.ShapeType;
            var featureType = ShpUtility.GeometryType2FeatureType(geometryType);
            FeatureSet fs = new FeatureSet(featureType);
            IFeatureList features = new FeatureList(fs);
            for (int i = 0; i < recordNum; i++)
            {
                var spatialBytes = _shpReader.GetNextRecord();
                _dbfReader.GetNextRow();
                IGeometry geometry;
                switch (geometryType)
                {
                    case GeometryType.Point:
                        geometry = BytesToGeometry.CreatePoint(spatialBytes);
                        break;
                    case GeometryType.MultiPoint:
                        geometry = BytesToGeometry.CreateMultipoint(spatialBytes);
                        break;
                    case GeometryType.PolyLine:
                        geometry = BytesToGeometry.CreatePolyline(spatialBytes);
                        break;
                    case GeometryType.Polygon:
                        geometry = BytesToGeometry.CreatePolygon(spatialBytes);
                        break;
                    default:
                        geometry = null;
                        break;
                }
                IFeature feature = new Feature(fs)
                {
                    Geometry = geometry
                };
                features.Add(feature);
            }

            fs.Features = features;
            fs.AttrTable = _dbfReader.DbfTable;
            return fs;
        }

    }
}
