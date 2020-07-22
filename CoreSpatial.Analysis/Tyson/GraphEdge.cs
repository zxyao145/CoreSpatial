namespace CoreSpatial.Analysis.Tyson
{
    internal class GraphEdge
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


        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }
            if(obj is GraphEdge otherGraphEdge)
            {
                if(!ReferenceEquals(this, otherGraphEdge))
                {
                    return IsEquals(otherGraphEdge, false);
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Start.GetHashCode() & this.End.GetHashCode();
        }

        public bool IsEquals(GraphEdge otherGraphEdge, bool ignoreOrder = false)
        {
            var result = this.Start == otherGraphEdge.Start && this.End == otherGraphEdge.End;
            if (ignoreOrder)
            {
                result = result || this.Start == otherGraphEdge.End && this.End == otherGraphEdge.Start;
            }

            return result;
        }
    }
}
