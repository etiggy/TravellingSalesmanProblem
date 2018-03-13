using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanProblem.Model
{
    internal class Coordinate
    {
        internal double X { get; private set; }
        internal double Y { get; private set; }
        internal double Z { get; private set; }

        public Coordinate(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public Coordinate(double X, double Y) : this(X, Y, 0) { }

        public Coordinate(double X) : this(X, 0, 0) { }

        public double GetDistance(Coordinate otherCoordinate)
        {
            return Math.Sqrt(Math.Pow((this.X - otherCoordinate.X), 2) + Math.Pow((this.Y - otherCoordinate.Y), 2) +
                Math.Pow((this.Z - otherCoordinate.Z), 2));
        }
    }
}
