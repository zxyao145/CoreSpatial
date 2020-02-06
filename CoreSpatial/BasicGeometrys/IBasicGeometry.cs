using System.Collections.Generic;

namespace CoreSpatial.BasicGeometrys
{
    /// <summary>
    /// 几何信息
    /// </summary>
    public interface IBasicGeometry
    {
        /// <summary>
        /// MBR
        /// </summary>
        IEnvelope Envelope { get; }

        /// <summary>
        /// 所有坐标点对
        /// </summary>
        IEnumerable<Coordinate> Coordinates { get; }

        /// <summary>
        /// 几何部件数，对于单部件几何为1
        /// </summary>
        int PartsNum { get; }

        /// <summary>
        /// 几何中所有点的数目
        /// </summary>
        int PointsNum { get; }

        /// <summary>
        /// 几何类型
        /// </summary>
        GeometryType GeometryType { get; }
    }
}
