namespace CoreSpatial.GeometryTypes
{
    public interface IGeoPoint : IBasicGeometry
    {
        /// <summary>
        /// X坐标
        /// </summary>
        double X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        double Y { get; set; }

    }
}
