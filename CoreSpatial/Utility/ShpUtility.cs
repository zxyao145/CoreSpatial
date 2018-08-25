using System;
using System.IO;
using CoreSpatial.GeometryTypes;
using CoreSpatial.ShpOper;
using CoreSpatial.ShpOper.ShapefileModel;

namespace CoreSpatial.Utility
{
    internal static class ShpUtility
    {
        /// <summary>
        /// 判断两个double是否相等的差值极限，此用以判断坐标的X、Y值
        /// </summary>
        public static double DValue => 0.0000000000d;

        /// <summary>
        /// 文件头的长度，100 byte
        /// </summary>
        public static int HeaderLengthInBytes = 100;
        public static byte[] ReadBytesFromStream(FileStream readStream, int numBytesRequested)
        {
            byte[] bufferBytes = new byte[numBytesRequested];
            if (readStream != null)
            {
                if (readStream.CanRead)
                {
                    int totalBytesRead = 0;
                    do
                    {
                        var bytesRead = readStream.Read(bufferBytes, totalBytesRead, numBytesRequested - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            byte[] resizedBufferBytes = new byte[totalBytesRead];
                            Array.Copy(bufferBytes, resizedBufferBytes, totalBytesRead);
                            return resizedBufferBytes;
                        }
                        else
                        {
                            totalBytesRead += bytesRead;
                        }
                    } while (totalBytesRead < numBytesRequested);

                }
                else
                {
                    throw new IOException("不能读取FileStream: " + readStream.Name);
                }
            }
            else
            {
                throw new NullReferenceException("FileStream为空！");
            }
            return bufferBytes;
        }

        /// <summary>
        /// 获取shp/shx文件的文件头
        /// </summary>
        /// <param name="stream">shp文件流或者shx文件的文件流</param>
        /// <returns>根据流返回对应的文件的文件头</returns>
        internal static ShpOrShxHeader GetHeader(FileStream stream)
        {
            ShpOrShxHeader shpfileHeader = null;
            try
            {
                stream.Position = 0;
                shpfileHeader = new ShpOrShxHeader(ShpUtility.ReadBytesFromStream(stream, HeaderLengthInBytes));
            }
            catch (Exception e)
            {
                shpfileHeader = null;
                throw new Exception("error", e);
            }
            return shpfileHeader;
        }

        /// <summary>
        /// 验证shp文件是否有效，shp、shx、dbf三个文件是否存在
        /// </summary>
        /// <param name="dir">shp文件的目录</param>
        /// <param name="fileNameWithoutExtension">shp文件不带扩展名的文件名</param>
        /// <returns></returns>
        internal static bool VerificationShp(string dir,string fileNameWithoutExtension)
        {
            string shpFile = Path.Combine(dir, fileNameWithoutExtension + ".shp");
            string shxFile = Path.Combine(dir, fileNameWithoutExtension + ".shx");
            string dbfFile = Path.Combine(dir, fileNameWithoutExtension + ".dbf");
            if (File.Exists(shpFile) && File.Exists(shxFile) && File.Exists(dbfFile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据GeometryType判断shp的类型
        /// </summary>
        /// <param name="geometryType"></param>
        /// <returns></returns>
        internal static FeatureType GeometryType2FeatureType(GeometryType geometryType)
        {
            FeatureType featureType;
            switch (geometryType)
            {
                case GeometryType.Point:
                case GeometryType.PointM:
                case GeometryType.PointZ:
                    featureType = FeatureType.Point;
                    break;

                case GeometryType.MultiPoint:
                case GeometryType.MultiPointM:
                case GeometryType.MultiPointZ:
                    featureType = FeatureType.MultiPoint;
                    break;

                case GeometryType.PolyLine:
                case GeometryType.PolyLineM:
                case GeometryType.PolyLineZ:
                case GeometryType.MultiPolyLine:
                    featureType = FeatureType.PolyLine;
                    break;
                case GeometryType.Polygon:
                case GeometryType.PolygonM:
                case GeometryType.PolygonZ:
                case GeometryType.MultiPolygon:
                    featureType = FeatureType.Polygon;
                    break;
                
                default:
                    featureType = FeatureType.NullShape;
                    break;
            }
            return featureType;
        }
    }
}
