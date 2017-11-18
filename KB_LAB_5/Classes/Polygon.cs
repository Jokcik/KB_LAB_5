using System;
using System.Collections.Generic;
using System.Drawing;

namespace KB_LAB_5.Classes
{
    public class Polygon
    {
        public List<Vector3D> points;
        public Color color;
        public float midleZDepthValue;

        public Polygon(List<Vector3D> points, Color color)
        {
            this.points = points;
            this.color = color;
        }

        public Polygon()
        {
            points = new List<Vector3D>();
        }
        
        public void AddPoint(Vector3D point)
        {
            points.Add(point);
        }
        
        public void findMidleZValue()
        {
            foreach(Vector3D point in points)
            {
                midleZDepthValue += point.Z;
            }
            midleZDepthValue /= points.Count;
        }

        public static int ZDepthComparer(Polygon p1, Polygon p2)
        {
            return p1.midleZDepthValue.CompareTo(p2.midleZDepthValue);
        }

        public bool inside(Vector3D a)
        {
            var a1 = new Vector3D(a.X - points[0].X, a.Y - points[0].Y, a.Z - points[0].Z);
            var a2 = new Vector3D(a.X - points[1].X, a.Y - points[1].Y, a.Z - points[1].Z);
            var r1 = a1.X * a2.Y - a1.Y * a2.X;
            a1 = a2;
            var r2 = 0f;
            for (int i = 2; i < points.Count + 1; ++i)
            {
                a2 = new Vector3D(a.X - points[i % points.Count].X, a.Y - points[i % points.Count].Y, a.Z - points[i % points.Count].Z);                    
                r2 = a1.X * a2.Y - a1.Y * a2.X ;
                if (Math.Abs(r2) < 0.0000001 || Math.Abs(r1) < 0.0000001) return false;
                if (r1 > 0 != r2 > 0) return false;
                a1 = a2;
            }
            
            return true;
        }
        
    }
}