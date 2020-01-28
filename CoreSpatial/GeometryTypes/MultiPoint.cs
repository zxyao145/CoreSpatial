using System.Collections.Generic;

namespace CoreSpatial.GeometryTypes
{
    public class MultiPoint: IBasicGeometry
    {
        public List<IGeoPoint> Points { get; set; }

        public int PartsNum => Points.Count;

        public int PointsNum => PartsNum;

        public IGeoPoint this[int index] => Points[index];

        public bool IsLineRing => (GeoPoint)Points[0] == (GeoPoint)Points[Points.Count - 1];

        public IEnvelope Envelope
        {
            get
            {
                if (Points.Count > 1)
                {
                    var firstPoint = Points[0];

                    var minX = firstPoint.X;
                    var minY = firstPoint.Y;
                    var maxX = firstPoint.X;
                    var maxY = firstPoint.Y;

                    foreach (var geoPoint in Points)
                    {
                        var curX = geoPoint.X;
                        var curY = geoPoint.Y;

                        if (curX < minX)
                        {
                            minX = curX;
                        }
                        else
                        {
                            if (curX > maxX)
                            {
                                maxX = curX;
                            }
                        }

                        if (curY < minY)
                        {
                            minY = curY;
                        }
                        else
                        {
                            if (curY > maxY)
                            {
                                maxY = curY;
                            }
                        }
                    }

                    return new Envelope(minX, minY, maxX, maxY);
                }
                else
                {
                    return new Envelope(0,0,0,0);
                }
            }
        }
    }
}
