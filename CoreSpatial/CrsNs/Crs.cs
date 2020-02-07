using ProjNet.CoordinateSystems;
using ProjNet.IO.CoordinateSystems;

namespace CoreSpatial.CrsNs
{
    /// <summary>
    /// 坐标系
    /// </summary>
    public class Crs
    {
        internal Crs(CoordinateSystem coordinateSystem)
        {
            _coordinateSystem = coordinateSystem;
        }

        private CoordinateSystem _coordinateSystem;

        /// <summary>
        /// 坐标系名
        /// </summary>
        public string Name => _coordinateSystem.Name;

        /// <summary>
        /// WKT字符串
        /// </summary>
        public string Wkt => _coordinateSystem.WKT;

        /// <summary>
        /// 收否为地理坐标系
        /// </summary>
        public bool IsGcs => _coordinateSystem
                                 .GetType()
                                 .Name == "GeographicCoordinateSystem";

        #region 创建Projection

        /// <summary>
        /// 根据坐标系的Wkt字符串创建Projection
        /// </summary>
        /// <param name="wktPrj"></param>
        /// <returns></returns>
        public static Crs CreateFromWkt(string wktPrj)
        {
            var iInfo = CoordinateSystemWktReader.Parse(wktPrj);
            if (iInfo.GetType().Name == "GeographicCoordinateSystem")
            {
                return new Crs(iInfo
                    as GeographicCoordinateSystem);
            }
            else
            {
                return new Crs(iInfo
                    as ProjectedCoordinateSystem);
            }
        }

        /// <summary>
        /// 根据srid创建Projection
        /// </summary>
        /// <param name="srid"></param>
        /// <returns></returns>
        public static Crs CreateBySrid(int srid)
        {
            var coordSys = SridReader.GetCSbyID(srid);
            if (coordSys != null)
            {
                return new Crs(coordSys);
            }
            return null;
        }

        #endregion
        
        #region 常见坐标系

        private static Crs _wgs84Gcs = null;
        /// <summary>
        /// EPSG:4326，Wgs84坐标系
        /// </summary>
        public static Crs Wgs84Gcs => _wgs84Gcs ??= Crs.CreateBySrid(4326);

        private static Crs _webMercator = null;

        /// <summary>
        /// EPSG:3857，Web墨卡托投影
        /// </summary>
        public static Crs WebMercator => _webMercator ??= Crs.CreateBySrid(3857);

        #endregion

        #region 显示转换

        ///// <summary>
        ///// 隐式转换
        ///// </summary>
        ///// <param name="projection"></param>
        //public static implicit operator GeographicCoordinateSystem(Projection projection)
        //{
        //    return projection._coordinateSystem as GeographicCoordinateSystem;
        //}

        /// <summary>
        /// 显示转换为CoordinateSystem
        /// </summary>
        /// <param name="crs"></param>
        public static explicit operator CoordinateSystem(Crs crs)
        {
            return crs._coordinateSystem;
        }

        /// <summary>
        /// 显示转换为GeographicCoordinateSystem 
        /// </summary>
        /// <param name="crs"></param>
        public static explicit operator GeographicCoordinateSystem(Crs crs)
        {
            return crs._coordinateSystem as GeographicCoordinateSystem;
        }

        /// <summary>
        /// 显示转换为ProjectedCoordinateSystem
        /// </summary>
        /// <param name="crs"></param>
        public static explicit operator ProjectedCoordinateSystem(Crs crs)
        {
            return crs._coordinateSystem as ProjectedCoordinateSystem;
        }

        #endregion

    }
}
