using System.Collections.Generic;
using System.Linq;
using CoreSpatial.BasicGeometrys;

namespace CoreSpatial.Converter.GeoJSON.CoordinatesOper
{

    /// <summary>
    /// 一维坐标集合，针对单个点
    /// </summary>
    internal class Coord1
    {
        protected List<double> GetFeatureCoordinates(Coordinate coor)
        {
            return new List<double>() { coor.X, coor.Y };
        }
        public List<double> GetCoords(IFeature feature)
        {
            var coor = feature.Geometry.BasicGeometry.Coordinates.ToList()[0];
            return new List<double>() { coor.X, coor.Y };
        }
    }
}
