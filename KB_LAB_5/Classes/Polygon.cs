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
            for (int i = 1; i < p1.points.Count + 1; ++i)
            {
                f = f && (p2.Inside(new Vector3D(p1.points[(i - 1) % p1.points.Count].X,
                              p1.points[(i - 1) % p1.points.Count].Y, p1.points[(i - 1) % p1.points.Count].Z))
                          || p1.Inside(new Vector3D(p2.points[(i - 1) % p2.points.Count].X,
                              p2.points[(i - 1) % p2.points.Count].Y, p2.points[(i - 1) % p2.points.Count].Z)));
            }

//            var k = true;
//            for (int i = 1; i < p2.points.Count + 1; ++i)
//            {
//                k = k && p1.Inside(new Vector3D(p2.points[(i - 1) % p2.points.Count].X,
//                        p2.points[(i - 1) % p2.points.Count].Y, p2.points[(i - 1) % p2.points.Count].Z));
//            }
            
            if (f)
            {
                bool inter;
                
                for (int i = 1; i < p1.points.Count + 1; ++i)
                {
                    for (int j = 1; j < p2.points.Count + 1; ++j)
                    {
                        double zAB, zCD;
                        inter = Vector3D.intersect(
                            new Vector3D(p1.points[(i - 1) % p1.points.Count].X, p1.points[(i - 1) % p1.points.Count].Y, p1.points[(i - 1) % p1.points.Count].Z), 
                            new Vector3D(p1.points[i % p1.points.Count].X, p1.points[i % p1.points.Count].Y, p1.points[i % p1.points.Count].Z),
                            new Vector3D(p2.points[(j - 1) % p2.points.Count].X, p2.points[(j - 1) % p2.points.Count].Y, p2.points[(j - 1) % p2.points.Count].Z), 
                            new Vector3D(p2.points[j % p2.points.Count].X, p2.points[j % p2.points.Count].Y, p2.points[j % p2.points.Count].Z),
                            out zAB, out zCD, true
                        );

                        if (inter)
                        {
                            return zAB < zCD ? 1 : (Math.Abs(zAB - zCD) < 0.000001f ? 0 : -1);
                        }
                    }
                }
            }
            
            return 0;
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