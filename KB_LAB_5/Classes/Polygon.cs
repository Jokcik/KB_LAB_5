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

//        public bool inside(Vector3D A)
//        {
//            return true;
//        }
        
    }
}