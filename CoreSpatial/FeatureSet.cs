using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CoreSpatial.GeometryTypes;
using CoreSpatial.ShpOper;
using CoreSpatial.Utility;

namespace CoreSpatial
{
    public class FeatureSet: IFeatureSet
    {
        public FeatureSet(FeatureType featureType)
        {
            FeatureType = featureType;
        }

        /// <summary>
        /// 打开一个shp文件
        /// </summary>
        /// <param name="shpPath">.shp文件的路径</param>
        /// <param name="encoding">编码方式，默认采用GB2312</param>
        /// <returns></returns>
        public static FeatureSet Open(string shpPath, Encoding encoding = null)
        {
            if (encoding == null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                encoding = Encoding.GetEncoding("GB2312");
            }

            var fs = ShpManager.CreateFeatureSet(shpPath, encoding);
            fs._shpFilePath = shpPath;

            return fs;
        }

        public void Save(string newShpFilePath = null)
        {
            if (newShpFilePath == null)
            {
                newShpFilePath = _shpFilePath;
            }

            ShpManager.SaveFeatureSet(this, newShpFilePath);
        }


        #region 属性

        /// <summary>
        /// 坐标系
        /// </summary>
        public Crs.Crs Crs { get; set; }
        
        /// <summary>
        /// 范围
        /// </summary>
        public IEnvelope Envelope { get; set; }

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
        public IFeatureList Features { get; set; }

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
