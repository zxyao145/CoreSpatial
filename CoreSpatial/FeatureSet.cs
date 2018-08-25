using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CoreSpatial.DbfOper;
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
        /// 属性表
        /// </summary>
        public DataTable AttrTable { get; set; }

        /// <summary>
        /// shapefile文件类型
        /// </summary>
        public FeatureType FeatureType { get;private set; }

        /// <summary>
        /// 所有要素
        /// </summary>
        public IFeatureList Features { get; set; }

        /// <summary>
        /// 打开一个shp文件
        /// </summary>
        /// <param name="shpPath">.shp文件的路径</param>
        /// <param name="encoding">编码方式，默认采用GB2312</param>
        /// <returns></returns>
        public static FeatureSet Open(string shpPath, Encoding encoding = null)
        {
            if (encoding==null)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                encoding = Encoding.GetEncoding("GB2312");
            }
            DataManager dataManager = new DataManager(shpPath, encoding);
            return dataManager.CreateFeatureSet();
        }
    }
}
