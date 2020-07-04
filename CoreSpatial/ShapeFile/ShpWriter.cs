using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreSpatial.Extensions;
using CoreSpatial.Utility;

namespace CoreSpatial.ShapeFile
{
    internal class ShpWriter:IDisposable
    {
        private readonly Stream _shpWriterStream;
        private int _fileLength = 100;

        public Stream Stream => _shpWriterStream;

        public  ShpWriter(string shpFilePath = null)
        {
            if (string.IsNullOrWhiteSpace(shpFilePath))
            {
                _shpWriterStream = new MemoryStream();
            }
            else
            {
                _shpWriterStream = new FileStream(shpFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            }
        }

        public void Write(IFeatureSet featureSet)
        {
            WriterHeader((FeatureSet)featureSet);

            foreach (var feature in featureSet.Features)
            {
                WriterRecord(feature);
            }

            WriteFileLength();
        }

        #region 写入文件头

        private void WriterHeader(FeatureSet featureSet)
        {
            var header = featureSet.GetHeader();
            WriterHeader(header);
        }

        public void WriterHeader(byte[] headerBytes)
        {
            _shpWriterStream.Write(headerBytes);
        }

        #endregion

        public int WriterRecord(IFeature feature)
        {
            var geometryBytes = new List<byte>();
            var geometryType = feature.GeometryType;
            if(geometryType == GeometryType.MultiPolyLine)
            {
                geometryBytes.AddRange(
                BitConverter
                    .GetBytes((int)GeometryType.PolyLine)
                );
            }
            else if (geometryType == GeometryType.MultiPolygon)
            {
                geometryBytes.AddRange(
                BitConverter
                    .GetBytes((int)GeometryType.Polygon)
                );
            }
            else
            {
                geometryBytes.AddRange(
                BitConverter
                    .GetBytes((int)geometryType)
                );
            }
                
            switch (geometryType)
            {
                case GeometryType.Point:
                    geometryBytes.AddRange(
                        GeometryToBytes
                            .GetPointBytes(feature.Geometry.BasicGeometry)
                        );
                    break;
                case GeometryType.MultiPoint:
                    geometryBytes.AddRange(
                        GeometryToBytes
                            .GetMultiPointBytes(feature.Geometry.BasicGeometry)
                    );
                    break;

                case GeometryType.PolyLine:
                    geometryBytes.AddRange(
                        GeometryToBytes
                        .GetPolyLineBytes(feature.Geometry.BasicGeometry)
                    );
                    break;
                case GeometryType.MultiPolyLine:
                    geometryBytes.AddRange(
                        GeometryToBytes
                            .GetMultiPolyLineBytes(feature.Geometry.BasicGeometry)
                    );
                    break;
                case GeometryType.Polygon:
                    geometryBytes.AddRange(
                        GeometryToBytes
                            .GetPolygonBytes(feature.Geometry.BasicGeometry)
                    );
                    break;
                default:
                    break;
            }

            var recordList = new List<byte>();
            recordList.AddRange(BigOrderBitConverter.GetBytes(feature.Fid+1));
            var recordLength = geometryBytes.Count + 8;
            recordList.AddRange(BigOrderBitConverter.GetBytes(geometryBytes.Count / 2));
            recordList.AddRange(geometryBytes);

            _shpWriterStream.Write(recordList.ToArray());
            _fileLength += recordList.Count;
            return recordList.Count;
        }

        public void WriteFileLength()
        {
            _shpWriterStream.Position = 24;
            _shpWriterStream.Write(BigOrderBitConverter.GetBytes(_fileLength/2)
                .ToArray());
        }


        public void Dispose()
        {
            _shpWriterStream?.Dispose();
        }
    }
}
