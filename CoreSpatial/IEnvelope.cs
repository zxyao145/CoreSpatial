using System;
using System.Collections.Generic;
using System.Text;

namespace CoreSpatial
{
    public interface IEnvelope
    {
        double MinX { get; set; }
        double MaxX { get; set; }


        double MinY { get; set; }
        double MaxY { get; set; }


        double MinZ { get; set; }
        double MaxZ { get; set; }


        double MinM { get; set; }
        double MaxM { get; set; }

        void Update(IEnvelope envelope);

        IEnvelope DeepClone();
    }

    public class Envelope: IEnvelope
    {
        public Envelope(double minX,double minY,
            double maxX,double maxY)
        {
            this.MinX = minX;
            this.MinY = minY;
            this.MaxX = maxX;
            this.MaxY = maxY;
        }

        public Envelope(double minX, double minY,
            double maxX, double maxY, double minZ, double maxZ)
            : this(minX, minY, maxX, maxY)
        {
            this.MinZ = minZ;
            this.MaxZ = maxZ;
        }
        
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }
        public double MinZ { get; set; } = 0;
        public double MaxZ { get; set; } = 0;
        public double MinM { get; set; } = 0;
        public double MaxM { get; set; } = 0;

        /// <summary>
        /// 更新本Envelope
        /// </summary>
        /// <param name="envelope"></param>
        public void Update(IEnvelope envelope)
        {
            MinX = MinX < envelope.MinX ? MinX : envelope.MinX;
            MinY = MinY < envelope.MinY ? MinY : envelope.MinY;
            MinZ = MinZ < envelope.MinZ ? MinZ : envelope.MinZ;

            MaxX = MaxX > envelope.MaxX ? MaxX : envelope.MaxX;
            MaxY = MaxY > envelope.MaxY ? MaxY : envelope.MaxY;
            MaxZ = MaxZ > envelope.MaxZ ? MaxZ : envelope.MaxZ;
        }

        public IEnvelope DeepClone()
        {
            return new Envelope(MinX,MinY,MaxX,MaxY,MinZ,MaxZ);
        }
    }
}
