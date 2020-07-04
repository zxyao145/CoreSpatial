namespace CoreSpatial.Analysis.Tyson
{
    class PointEdgeDistance
    {
        public PointEdgeDistance
            (TysonGeoPoint pt, GraphEdge edge, double distance2)
        {
            Pt = pt;
            Edge = edge;
            Distance2 = distance2;
        }

        public PointEdgeDistance
            (int pointIndex, GraphEdge edge, double distance2)
        {
            PointIndex = pointIndex;
            Edge = edge;
            Distance2 = distance2;
        }

        public int PointIndex { get; private set; }

        public TysonGeoPoint Pt { get; private set; }

        public GraphEdge Edge { get; private set; }


        public double Distance2 { get; private set; }
    }
}
