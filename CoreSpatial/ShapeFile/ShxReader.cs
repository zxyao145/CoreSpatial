using System;
using System.Collections.Generic;
using System.IO;
using CoreSpatial.ShpOper.ShapefileModel;
using CoreSpatial.Utility;

namespace CoreSpatial.ShpOper
{
    internal class ShxReader
    {
        /// <summary>
        /// 解析shx文件
        /// </summary>
        /// <returns></returns>
        public List<ShxRecord> ReadShx(string shxFilePath)
        {
            if (!File.Exists(shxFilePath))
            {
                throw new Exception("索引文件不存在！");
            }

            using (var readStream = new FileStream(shxFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (readStream.CanRead)
                {
                    var shxIndex = GetAllIndexRecords(readStream);
                    return shxIndex;
                }
                else
                {
                    throw new Exception("索引文件不可读！");
                }
            }
        }

        /// <summary>
        /// 读取shx索引信息
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private static List<ShxRecord> GetAllIndexRecords(FileStream stream)
        {
            List<ShxRecord> records = new List<ShxRecord>();
            try
            {
                stream.Position = 100;
                byte[] streamBuffer;
                do
                {
                    streamBuffer = ShpUtil.ReadBytesFromStream(stream, 8);
                    if (streamBuffer.Length < 8)
                    {
                        break;
                    }
                    else
                    {
                        ShxRecord myRecord = new ShxRecord(streamBuffer);
                        records.Add(myRecord);
                    }
                } while (streamBuffer.Length > 0);
            }
            catch
            {
                records.Clear();
            }
            return records;
        }
    }
}
