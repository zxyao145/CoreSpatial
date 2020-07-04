using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CoreSpatial.CrsNs;

namespace CoreSpatial
{
    public interface IFeatureSet
    {
        /// <summary>
        /// 属性表
        /// </summary>
        DataTable AttrTable { get; set; }

        /// <summary>
        /// 要素类型
        /// </summary>
        FeatureType FeatureType { get; }

        /// <summary>
        /// 所有要素
        /// </summary>
        IFeatureList Features { get; }

        /// <summary>
        /// 坐标系
        /// </summary>
        Crs Crs { get; set; }

        /// <summary>
        /// 范围
        /// </summary>
        IEnvelope Envelope { get; }

        /// <summary>
        /// 保存FeatureSet到硬盘
        /// </summary>
        /// <param name="newShpFilePath"></param>
        /// <returns>shapefile在硬盘上的保存目录</returns>
        string Save(string newShpFilePath = null);

        ShapeFileBytes GetShapeFileBytes();
    }
}
