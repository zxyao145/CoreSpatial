using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CoreSpatial.CrsNs;
using CoreSpatial.ShapeFile;
using CoreSpatial.Utility;

namespace CoreSpatial
{
    public class FeatureSet: IFeatureSet
    {
        public FeatureSet(FeatureType featureType)
        {
            FeatureType = featureType;
            Features = new FeatureList(this);
        }

        /// <summary>
        /// 打开一个shp文件
        /// </summary>
        /// <param name="shpPath">.shp文件的路径</param>
        /// <returns></returns>
        public static FeatureSet Open(string shpPath)
        {
            var fs = ShpManager.CreateFeatureSet(shpPath);
            fs._shpFilePath = shpPath;
            return fs;
        }

        /// <summary>
        /// 通过文件流打开shapefile
        /// </summary>
        /// <param name="shpFileStream">.shp文件流</param>
        /// <param name="shxFileStream">.shx文件流</param>
        /// <param name="dbfFileStream">.dbf文件流</param>
        /// <param name="prjFileStream">.prj文件流</param>
        /// <returns></returns>
        public static FeatureSet Open(Stream shpFileStream, Stream shxFileStream,
            Stream dbfFileStream, Stream prjFileStream = null)
        {
            var fs = ShpManager
                .CreateFeatureSet(shpFileStream, shxFileStream, dbfFileStream, prjFileStream);
            fs._shpFilePath = null;
            return fs;
        }

        /// <summary>
        /// 保存FeatureSet到硬盘，如果原来存在则先删除
        /// </summary>
        /// <param name="newShpFilePath"></param>
        /// <returns>shapefile在硬盘上的保存目录</returns>
        public string Save(string newShpFilePath = null)
        {
            if (newShpFilePath == null)
            {
                newShpFilePath = string.IsNullOrEmpty(_shpFilePath) 
                    ? Path.Combine(Path.GetTempPath(), "CoreSpatial") 
                    : _shpFilePath;
            }

            ShpManager.SaveFeatureSet(this, newShpFilePath);
            return newShpFilePath;
        }

        public ShapeFileBytes GetShapeFileBytes()
        {
            return ShpManager.GetShapeFileStreams(this);
        }

        #region 属性

        /// <summary>
        /// 坐标系
        /// </summary>
        public Crs Crs { get; set; }

        /// <summary>
        /// 范围
        /// </summary>
        public IEnvelope Envelope
        {
            get
            {
                if (Features.Count > 0)
                {
                    var envelope = Features[0].Geometry.BasicGeometry.Envelope;

                    bool firstFlag = true;
                    foreach (var feature in Features)
                    {
                        if (firstFlag)
                        {
                            firstFlag = false;
                            continue;
                        }
                        //else
                        envelope.Update(feature.Geometry.BasicGeometry.Envelope);
                    }

                    return envelope;
                }

                return null;
            }
        }

        /// <summary>
        /// 属性表
        /// </summary>
        public DataTable AttrTable { get; set; }

        /// <summary>
        /// shapefile文件类型
        /// </summary>
        public FeatureType FeatureType { get; private set; }

        /// <summary>
        /// 所有要素
        /// </summary>
        public IFeatureList Features { get; }

        #endregion

        /// <summary>
        /// shp文件路径
        /// </summary>
        private string _shpFilePath;
        
        /// <summary>
        /// 文件头
        /// </summary>
        private byte[] _header = null;

        /// <summary>
        /// 当前feature set对应的文件头
        /// </summary>
        /// <returns></returns>
        internal byte[] GetHeader()
        {
            return _header ??= ShpUtil.BuildHeader(this);
        }
    }
}
