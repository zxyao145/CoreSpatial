using System.Collections.Generic;
using System.Linq;

namespace CoreSpatial.GeometryTypes
{
    public class MultiPolyLine:IBasicGeometry
    {
        public int PartsNum { get; internal set; }

        public int PointsNum => PolyLines.Sum(e => e.PartsNum);

        public List<PolyLine> PolyLines { get; set; }

        public PolyLine this[int lineIndex] => PolyLines[lineIndex];

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

                return new Envelope(0,0,0,0);
            }
        }

    }
}
