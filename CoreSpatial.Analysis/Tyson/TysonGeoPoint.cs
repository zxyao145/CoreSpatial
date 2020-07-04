using CoreSpatial.BasicGeometrys;

namespace CoreSpatial.Analysis.Tyson
{
    public class TysonGeoPoint:GeoPoint
    {
        public TysonGeoPoint()
        {
            
        }

        public TysonGeoPoint(double x, double y):base(x,y)
        {
            
        }

        public TysonGeoPoint(GeoPoint pt) : this(pt.X, pt.Y)
        {

        }

        public EdgePointType EdgePointType { get; internal set; }
    }
}
