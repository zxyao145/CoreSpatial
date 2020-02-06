using System.Collections.Generic;

namespace CoreSpatial.Converter.GeoJSON.CoordinatesOper
{
    /// <summary>
    /// 二维数组，针对多点、线
    /// </summary>
    internal class Coord2 : Coord1
    {
        /// <summary>
        /// 获取一条直线的坐标点
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        internal new List<List<double>> GetCoords(IFeature feature)
        {
            var coords = feature.Geometry.BasicGeometry.Coordinates;
            List<List<double>> coord2S = new List<List<double>>();
            foreach (var coor in coords)
            {
                coord2S.Add(base.GetFeatureCoordinates(coor));
            }
            return coord2S;
        }


        /// <summary>
        /// 获取一条闭合线环的坐标点
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        public List<List<double>> GetClosedCoords(IFeature feature)
        {
            var coords = feature.Geometry.BasicGeometry.Coordinates;
            List<List<double>> coord2S = new List<List<double>>();
            foreach (var coor in coords)
            {
                coord2S.Add(base.GetFeatureCoordinates(coor));
            }
            coord2S.Add(coord2S[coord2S.Count - 1]);
            return coord2S;
        }
    }


}
