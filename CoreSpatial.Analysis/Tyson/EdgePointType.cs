namespace CoreSpatial.Analysis.Tyson
{
    public enum EdgePointType
    {
        NotCalc,
        Inner,
        MinX,
        MinY,
        MaxX,
        MaxY,

        /// <summary>
        /// 左下角角点
        /// </summary>
        MinXMinY,

        /// <summary>
        /// 左上角角点
        /// </summary>
        MinXMaxY,

        /// <summary>
        /// 右上角角点
        /// </summary>
        MaxXMaxY,

        /// <summary>
        /// 右下角角点
        /// </summary>
        MaxXMinY
    }
}
