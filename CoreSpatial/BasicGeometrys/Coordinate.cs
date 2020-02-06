namespace CoreSpatial.BasicGeometrys
{
    public class Coordinate
    {
        #region 构造函数

        public Coordinate()
        {
            this.X = double.NaN;
            this.Y = double.NaN;
            this.Z = double.NaN;
            this.M = double.NaN;
        }

        public Coordinate(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.Z = double.NaN;
            this.M = double.NaN;
        }

        public Coordinate(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.M = double.NaN;
        }

        public Coordinate(double x, double y, double z, double m)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.M = m;
        }

        public Coordinate(Coordinate c)
        {
            this.X = c.X;
            this.Y = c.Y;
            this.Z = c.Z;
            this.M = c.M;
        }

        #endregion
        
        public override bool Equals(object obj)
        {
            if (obj is Coordinate c)
            {
                if (double.IsNaN(this.Z) || double.IsNaN(c.Z))
                {
                    return c.X == this.X && c.Y == this.Y;
                }
                return c.X == this.X && c.Y == this.Y && c.Z == this.Z;


            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public double[] ToArray()
        {
            if (!double.IsNaN(this.Z))
            {
                return new double[]
                {
                    this.X,
                    this.Y,
                    this.Z
                };
            }
            return new double[]
            {
                this.X,
                this.Y
            };
        }

        public static bool operator ==(Coordinate obj1, Coordinate obj2)
		{
			return object.Equals(obj1, obj2);
		}

        public static bool operator !=(Coordinate obj1, Coordinate obj2)
		{
			return !(obj1 == obj2);
		}
        
		
		public double X { get; set; }


		public double Y { get; set; }


        public double Z { get; set; }

        public double M { get; set; }

    }
}
