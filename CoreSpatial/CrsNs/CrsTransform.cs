using CoreSpatial.BasicGeometrys;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace CoreSpatial.CrsNs
{
    /// <summary>
    /// 坐标转换器
    /// </summary>
    public class CrsTransform
    {
        private readonly CoordinateSystem _from;
        private readonly CoordinateSystem _to;
        private readonly ICoordinateTransformation _trans;
        private readonly MathTransform _mathTransform;

        public CrsTransform(Crs from, Crs to)
            : this((CoordinateSystem)from, (CoordinateSystem)to)
        {
        }

        public CrsTransform(CoordinateSystem from, CoordinateSystem to)
        {
            _from = from;
            _to = to;

            CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();
            _trans = ctfac.CreateFromCoordinateSystems(_from, _to);
            _mathTransform = _trans.MathTransform;
        }


        /// <summary>
        /// 根据XY坐标数组进行转换
        /// </summary>
        /// <param name="fromXy"></param>
        /// <returns></returns>
        public double[] Transform(double[] fromXy)
        {
            return _mathTransform.Transform(fromXy);
        }

        /// <summary>
        /// 根据GeoPoint进行转换
        /// </summary>
        /// <param name="fromPoint"></param>
        /// <returns></returns>
        public GeoPoint Transform(GeoPoint fromPoint)
        {
            var xyArr = this.Transform(new double[]
            {
                fromPoint.X,
                fromPoint.Y
            });

            return new GeoPoint(xyArr[0], xyArr[1]);
        }
    }
}
