using System.Collections.Generic;
using System.Linq;

namespace CoreSpatial.BasicGeometrys
{
    public class MultiPolyLine:IBasicGeometry
    {
        public MultiPolyLine():this(new List<PolyLine>())
        {

        }

        public MultiPolyLine(List<PolyLine> polyLines)
        {
            PolyLines = polyLines;
        }

        /// <summary>
        /// 边集合
        /// </summary>
        public List<PolyLine> PolyLines { get; internal set; }

        public PolyLine this[int lineIndex] => PolyLines[lineIndex];

        #region 接口实现

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public GeometryType GeometryType => GeometryType.MultiPolyLine;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnvelope Envelope
        {
            get
            {
                if (PartsNum > 0)
                {
                    var envelope = PolyLines[0].Envelope;

                    for (int i = 1; i < PartsNum; i++)
                    {
                        envelope.Update(PolyLines[i].Envelope);
                    }

                    return envelope;
                }

                return null;
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IEnumerable<Coordinate> Coordinates
        {
            get
            {
                var list = new List<Coordinate>();
                foreach (var polyLine in PolyLines)
                {
                    list.AddRange(polyLine.Coordinates);
                }

                return list;
            }
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int PartsNum => PolyLines.Count;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int PointsNum => PolyLines.Sum(e => e.PartsNum);

        #endregion

    }
}
