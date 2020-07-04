using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreSpatial.Extensions;
using CoreSpatial.Utility;

namespace CoreSpatial.ShapeFile
{
    internal class ShxWriter: IDisposable
    {
        private readonly Stream _shxWriterStream;
        private int _fileLength = 100;
        public Stream Stream => _shxWriterStream;

        public ShxWriter(string shpFilePath=null)
        {
            if (string.IsNullOrWhiteSpace(shpFilePath))
            {
                _shxWriterStream = new MemoryStream();
            }
            else
            {
                _shxWriterStream = new FileStream(shpFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            }
        }

        public void Write(IFeatureSet featureSet)
        {
            WriterHeader((FeatureSet)featureSet);
            WriterRecords(featureSet.Features);
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
            _shxWriterStream.Write(headerBytes);
        }

        #endregion


        private void WriterRecords(IFeatureList featureList)
        {
            int offset = 100;
            foreach (var feature in featureList)
            {
                var curContentLength = GetRecordLength(feature);
                WriterRecord(offset, curContentLength);
                offset += curContentLength;
            }
        }

        private int GetRecordLength(IFeature feature)
        {
            var geometryBytes = new List<byte>();
            var geometryType = feature.GeometryType;
            geometryBytes.AddRange(
                BitConverter
                    .GetBytes((int)geometryType)
                    .Reverse()
            );
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
                case GeometryType.Polygon:
                    geometryBytes.AddRange(
                        GeometryToBytes
                            .GetPolygonBytes(feature.Geometry.BasicGeometry)
                    );
                    break;
                default:
                    break;
            }

            var recordLength = geometryBytes.Count + 8;

            return recordLength;
        }

        public void WriterRecord(int offset,int contentByteLength)
        {
            var offsetBytes = BigOrderBitConverter.GetBytes(offset / 2);
            var contentLengthBytes = BigOrderBitConverter.GetBytes((contentByteLength - 8) / 2);
            var recordBytes = new List<byte>();
            recordBytes.AddRange(offsetBytes);
            recordBytes.AddRange(contentLengthBytes);
            
            _shxWriterStream.Write(recordBytes.ToArray());
            _fileLength += 8;
        }



        public void WriteFileLength()
        {
            _shxWriterStream.Position = 24;
            _shxWriterStream.Write(BigOrderBitConverter.GetBytes(_fileLength/2)
                .ToArray());
        }


        public void Dispose()
        {
            _shxWriterStream?.Dispose();
        }
    }
}
