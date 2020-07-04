using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreSpatial.Extensions;
using CoreSpatial.ShapeFile.ShapefileModel;

namespace CoreSpatial.Utility
{
    internal static class ShpUtil
    {
        /// <summary>
        /// 文件头的长度，100 byte
        /// </summary>
        public static int HeaderLengthInBytes = 100;
        public static byte[] ReadBytesFromStream(Stream readStream, int numBytesRequested)
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
                    if (readStream is FileStream fs)
                    {
                        throw new IOException("不能读取FileStream: " + fs.Name);
                    }
                    throw new IOException("不能读取Stream");
                }
            }
            else
            {
                throw new NullReferenceException("Stream为空！");
            }
            return bufferBytes;
        }

        /// <summary>
        /// 从IFeatureSet中构建builder
        /// </summary>
        /// <param name="featureSet"></param>
        /// <returns></returns>
        internal static byte[] BuildHeader(IFeatureSet featureSet)
        {
            var header = new List<byte>(100);
            //0-3
            header.AddRange(BigOrderBitConverter.GetBytes(0x0000270a));
            var notUseValue = BigOrderBitConverter.GetBytes(0).ToArray();
            //4-7、8-11、12-15、16-19、20-23
            header.AddRange(notUseValue);
            header.AddRange(notUseValue);
            header.AddRange(notUseValue);
            header.AddRange(notUseValue);
            header.AddRange(notUseValue);
            //24-27 文件长度，包括文件头。
            header.AddRange(notUseValue);
            // 以下小端序
            // 版本28-31
            var version = BitConverter.GetBytes(1000);//new byte[] {0xE8, 0x03, 0, 0};
            header.AddRange(version);
            // 图形类型（参见下面）
            var geometryType = (int)featureSet.FeatureType;
            var geometryTypeBytes = BitConverter.GetBytes(geometryType);
            
            header.AddRange(geometryTypeBytes);
            //MBR
            var envelope = featureSet.Envelope;
            header.AddRange(
                BitConverter.GetBytes(envelope.MinX)
                );
            header.AddRange(
                BitConverter.GetBytes(envelope.MinY)
            );
            header.AddRange(
                BitConverter.GetBytes(envelope.MaxX)
            );
            header.AddRange(
                BitConverter.GetBytes(envelope.MaxY)
            );
            //Z坐标值的范围
            var minZ = envelope.MinZ;
            var maxZ = envelope.MaxZ;
            //M坐标值的范围
            header.AddRange(
                BitConverter.GetBytes(minZ)
            );
            header.AddRange(
                BitConverter.GetBytes(maxZ)
            );

            var minM = envelope.MinZ;
            var maxM = envelope.MaxZ;
            header.AddRange(
                BitConverter.GetBytes(minM)
            );
            header.AddRange(
                BitConverter.GetBytes(maxM)
            );
            return header.ToArray();
        }

        /// <summary>
        /// 获取shp/shx文件的文件头
        /// </summary>
        /// <param name="stream">shp文件流或者shx文件的文件流</param>
        /// <returns>根据流返回对应的文件的文件头</returns>
        internal static ShpOrShxHeader GetHeader(Stream stream)
        {
            ShpOrShxHeader shpfileHeader = null;
            try
            {
                stream.Position = 0;
                shpfileHeader = new ShpOrShxHeader(ShpUtil.ReadBytesFromStream(stream, HeaderLengthInBytes));
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
        /// 验证shp文件是否有效，shp、shx、dbf三个文件是否存在
        /// </summary>
        /// <param name="shpFilePath"></param>
        /// <param name="subFiles"></param>
        /// <returns></returns>
        internal static bool VerificationShp(string shpFilePath,out Tuple<string,string,string> subFiles)
        {
            subFiles = GetSubFileName(shpFilePath);
            var (shxFile, dbfFile, pfj) = subFiles;
            if (File.Exists(shpFilePath) && File.Exists(shxFile) && File.Exists(dbfFile))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据shp文件获取.shx文件、.dbf文件、.prj文件路径
        /// </summary>
        /// <param name="shpFilePath"></param>
        /// <returns></returns>
        internal static Tuple<string, string, string> GetSubFileName(string shpFilePath)
        {
            string shxFile = Path.ChangeExtension(shpFilePath, ".shx");
            string dbfFile = Path.ChangeExtension(shpFilePath, ".dbf");
            string prj = Path.ChangeExtension(shpFilePath, ".prj");

            return new Tuple<string, string, string>(shxFile, dbfFile, prj);
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

        /// <summary>
        /// 删除shapefile
        /// </summary>
        /// <param name="shpPath"></param>
        internal static void DeleteShp(string shpPath)
        {
            var (shxPath, dbfPath, prjPath) = GetSubFileName(shpPath);
            var fileList = new string[]
            {
                shpPath,shxPath,dbfPath,prjPath
            };

            foreach (var file in fileList)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }

    }
}
