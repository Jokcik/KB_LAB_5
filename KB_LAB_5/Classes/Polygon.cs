using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KB_LAB_5.Classes
{
    public class Polygon
    {
        public List<Vector3D> points;
        public Color color;
        public float midleZDepthValue;
        public int id;

        public Polygon(List<Vector3D> points, Color color, int id = 0)
        {
            this.points = points;
            this.color = color;
            this.id = id;
        }

        public Polygon()
        {
            points = new List<Vector3D>();
        }
        
        public void AddPoint(Vector3D point)
        {
            points.Add(point);
        }
        
        public void FindMidleZValue()
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

        public bool Inside(Vector3D a)
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

        public static int Cover(Polygon p1, Polygon p2)
        {
            for (int i = 1; i < p1.points.Count + 1; ++i)
            {
                for (int j = 1; j < p2.points.Count + 1; ++j)
                {
                    double zAB, zCD;
                    var inter = Vector3D.intersect(
                        new Vector3D(p1.points[(i - 1) % p1.points.Count].X, p1.points[(i - 1) % p1.points.Count].Y, p1.points[(i - 1) % p1.points.Count].Z), 
                        new Vector3D(p1.points[i % p1.points.Count].X, p1.points[i % p1.points.Count].Y, p1.points[i % p1.points.Count].Z),
                        new Vector3D(p2.points[(j - 1) % p2.points.Count].X, p2.points[(j - 1) % p2.points.Count].Y, p2.points[(j - 1) % p2.points.Count].Z), 
                        new Vector3D(p2.points[j % p2.points.Count].X, p2.points[j % p2.points.Count].Y, p2.points[j % p2.points.Count].Z),
                        out zAB, out zCD
                    );

                    if (inter)
                    {
                        return zAB < zCD ? 1 : (Math.Abs(zAB - zCD) < 0.000001f ? 0 : -1);
                    }
                }
            }

            var f = true;
            var m = true;
            for (int i = 1; i < p1.points.Count + 1; ++i)
            {
                f = f && p2.Inside(new Vector3D(p1.points[(i - 1) % p1.points.Count].X,
                        p1.points[(i - 1) % p1.points.Count].Y, p1.points[(i - 1) % p1.points.Count].Z));
                m = m && p1.Inside(new Vector3D(p2.points[(i - 1) % p2.points.Count].X,
                              p2.points[(i - 1) % p2.points.Count].Y, p2.points[(i - 1) % p2.points.Count].Z));
            }

            if (f || m)
            {
                var center = m ? p1 : p2;
                return intersectPInP(Form1.GetCenter(new[] {f ? p1 : p2}.ToList()), center.points[0], center.points[1],
                    center.points[2]);
            }
            
            return 0;
        }

        private static int intersectPInP(Vector3D E, Vector3D v0, Vector3D v1, Vector3D v2)
        {
            var Ex0 = E.X - v0.X;
            var Ey0 = E.Y - v0.Y;

            var x10 = v1.X - v0.X;
            var x20 = v2.X - v0.X;

            var y10 = v1.Y - v0.Y;
            var y20 = v2.Y - v0.Y;
            
            var z10 = v1.Z - v0.Z;
            var z20 = v2.Z - v0.Z;

            var A = Ex0 * y10 * z20 + Ey0 * z10 * x20;
            var B = Ex0 * z10 * y20 + Ey0 * x10 * z20;
            var Z = v0.Z + (B - A)/(x10 * y20 - y10 * x20);

            return E.Z < Z ? 1 : (Math.Abs(E.Z - Z) < 0.000001f ? 0 : -1);
        }

        public static List<Polygon> Sort(List<Polygon> polygons)
        {
            var list = new List<Polygon>();

            while (polygons.Count > 0)
            {

                var p = polygons[0];
                foreach (var polygon in polygons)
                {
                    if (Cover(p, polygon) == -1)
                    {
                        p = polygon;
                    }
                }
                list.Add(p);
                polygons.Remove(p);
            }
            
            return list;
        }
    }
}