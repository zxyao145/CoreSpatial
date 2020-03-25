using System;
using System.Collections.Generic;
using System.IO;
using CoreSpatial.ShapeFile.ShapefileModel;
using CoreSpatial.Utility;

namespace CoreSpatial.ShapeFile
{
    /// <summary>
    /// 读取shp文件
    /// </summary>
    internal class ShpReader:IDisposable
    {
        public ShpReader(string shpFilePath, List<ShxRecord> shxRecords)
        :this(new FileStream(shpFilePath, FileMode.Open, FileAccess.Read, FileShare.Read), shxRecords)
        {
            
        }

        public ShpReader(Stream shpFileStream, List<ShxRecord> shxRecords)
        {
            _shxRecords = shxRecords;
            _readStream = shpFileStream;
            this.ShpHeader = ShpUtil.GetHeader(_readStream);
        }

        public ShpReader(string shpFilePath)
        {
            string shxFilePath = shpFilePath.Substring(0, shpFilePath.Length - 3) + ".shx";
            _shxRecords = new ShxReader().ReadShx(shxFilePath);
            _readStream = new FileStream(shpFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            this.ShpHeader = ShpUtil.GetHeader(_readStream);
        }

        /// <summary>
        ///  当前读取的空间实体记录的索引+1
        /// </summary>
        private int _currentRecord = 0;

        /// <summary>
        /// shp文件流
        /// </summary>
        private readonly Stream _readStream;

        /// <summary>
        /// 索引文件的记录内容
        /// </summary>
        private readonly List<ShxRecord> _shxRecords;

        /// <summary>
        /// shp文件的头
        /// </summary>
        internal ShpOrShxHeader ShpHeader { get; set; }

        #region 读取所有的空间实体信息
        /// <summary>
        /// 读取所有的空间实体信息
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        //private List<ShapefileRecord> GetAllMainRecords(FileStream stream)
        //{
        //    List<ShapefileRecord> records = new List<ShapefileRecord>();
        //    byte[] streamBuffer;
        //    try
        //    {
        //        stream.Position = HeaderLengthInBytes;
        //        do
        //        {
        //            //前8byte：
        //            //【0,3】：当前记录的索引
        //            //【4,7】：当前记录记录内容的长度
        //            streamBuffer = ShpUtility.ReadBytesFromStream(stream, 8);
        //            if (streamBuffer.Length < 8)
        //            {
        //                break;
        //            }
        //            else
        //            {
        //                ShapefileRecord myRecord = new ShapefileRecord(streamBuffer);
        //                //读取当前记录的记录内容，并将其添加到当前记录中
        //                myRecord.SetRecordContents(ShpUtility.ReadBytesFromStream(stream, myRecord.ContentLength * 2));
        //                records.Add(myRecord);
        //            }
        //        } while (streamBuffer.Length > 0);
        //    }
        //    catch
        //    {
        //        records.Clear();
        //    }
        //    return records;
        //} 
        #endregion

        /// <summary>
        /// 读取下一条空间实体记录的所有byte
        /// </summary>
        /// <returns></returns>
        public byte[] GetNextRecord()
        {
            //shx索引文件存在
            if (this._shxRecords != null && this._shxRecords.Count > 0)
            {
                this._currentRecord++;
                if (_currentRecord > _shxRecords.Count)
                {
                    return null;
                }

                var shxRecord = _shxRecords[_currentRecord - 1];
                return GetNextRecord(shxRecord.Offset * 2 + 8, shxRecord.Length * 2);
            }
            else
            {
                if (this._readStream.Position < 100)
                {
                    this._readStream.Position = 100;
                }
                byte[] recordHeader = this.ReadBytesFromOpenStream((int)_readStream.Position, 8);
                if (recordHeader == null || recordHeader.Length < 8)
                {
                    return null;
                }
                Array.Reverse(recordHeader);

                int recordLength = BitConverter.ToInt32(recordHeader, 0);
                return GetNextRecord(_readStream.Position, recordLength * 2);
            }
        }

        /// <summary>
        /// 从shp文件读取要素记录
        /// </summary>
        /// <param name="recordOffset"></param>
        /// <param name="recordLength"></param>
        /// <returns></returns>
        private byte[] GetNextRecord(long recordOffset, int recordLength)
        {
            if (this._readStream.CanRead)
            {
                if (this._readStream.Position != recordOffset)
                {
                    this._readStream.Position = recordOffset;
                }
                byte[] record = this.ReadBytesFromOpenStream(recordOffset, recordLength);
                if (record != null && record.Length == recordLength)
                {
                    return record;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        private byte[] ReadBytesFromOpenStream(long startPosition, int numBytesRequested)
        {
            byte[] bufferBytes = new byte[numBytesRequested];
            int totalBytesRead = 0;
            int bytesRead = 0;
            do
            {
                bytesRead = this._readStream.Read(bufferBytes, totalBytesRead, numBytesRequested - totalBytesRead);
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
            return bufferBytes;
        }

        public void Dispose()
        {
            _readStream?.Dispose();
        }
    }
}
