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
            if (double.IsNaN(coor.Z))
            {
                return new List<double>() { coor.X, coor.Y };
            }
            else
            {
                return new List<double>() { coor.X, coor.Y, coor.Z };
            }
        }

        public List<double> GetCoords(IFeature feature)
        {
            var coor = feature.Geometry.BasicGeometry.Coordinates.ToList()[0];
            return GetFeatureCoordinates(coor);
        }
    }
}
