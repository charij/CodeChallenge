namespace PlanetWars.Common.Game.Data
{
    using System;

    public class Point
    {
        public int X { get; set; }

        public int Y { get; set; }

        public double Distance(Point p)
        {
            return Math.Sqrt(Math.Pow(X - p.X, 2) + Math.Pow(Y - p.Y, 2));
        }
    }
}