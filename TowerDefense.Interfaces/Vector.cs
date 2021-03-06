﻿using System;

namespace TowerDefense.Interfaces
{
    public class Vector
    {
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector()
            : this(0, 0)
        {
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Total {
            get { return Math.Sqrt(X*X + Y*Y); }
            set
            {
                var multiplier = value/Total;
                X *= multiplier;
                Y *= multiplier;
            }
        }

        public static Vector operator +(Vector c1, Vector c2)
        {
            return new Vector(c1.X + c2.X, c1.Y + c2.Y);
        }
    }
}