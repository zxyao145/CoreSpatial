namespace CoreSpatial.Analysis.Tyson
{
    public class GraphEdge
    {
        public GraphEdge()
            : this(new TysonGeoPoint(), new TysonGeoPoint())
        {

        }

        public GraphEdge(TysonGeoPoint start, TysonGeoPoint end)
        {
            Start = start;
            End = end;
        }

        #region xy

        public double X1
        {
            get => Start.X;
            set => Start.X = value;
        }

        public double Y1
        {
            get => Start.Y;
            set => Start.Y = value;
        }

        public double X2
        {
            get => End.X;
            set => End.X = value;
        }
        public double Y2
        {
            get => End.Y;
            set => End.Y = value;
        }

        #endregion

        /// <summary>
        /// 起点
        /// </summary>
        public TysonGeoPoint Start { get; }

        /// <summary>
        /// 终点
        /// </summary>
        public TysonGeoPoint End { get; }


        public bool ContainPoint(TysonGeoPoint pt)
        {
            return Start == pt || End == pt;
        }

        /// <summary>
        /// 与这条边相对应的点1的索引
        /// </summary>
        public int Point1Index { get; set; }

        /// <summary>
        /// 与这条边相对应的点2的索引
        /// </summary>
        public int Point2Index { get; set; }
    }
}
