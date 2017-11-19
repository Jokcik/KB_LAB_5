using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KB_LAB_5.Classes;
using ObjLoader.Loader.Loaders;

namespace KB_LAB_5
{
    public class Form1 : Form
    {
        private List<Polygon> _polygons = new List<Polygon>();
        private Point _locationSelect;
        private Vector3D _figureCenter;

        private int polygonId = -1;

        private float _angleX;
        private float _angleY;

        private Point _currentLocation = new Point(0, 0);

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _angleY += (e.Location.X - _currentLocation.X) / 1.0f;
            _angleX += (e.Location.Y - _currentLocation.Y) / 1.0f;
            _currentLocation = e.Location;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _currentLocation = e.Location;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            _locationSelect = e.Location;
            polygonId = -1;
            Invalidate();
        }

        private static Polygon getPolygon(List<Polygon> polygons, Point location)
        {
            if (location == null) return null;
            var vector = new Vector3D(location.X, location.Y, 0);
            for (int i = polygons.Count - 1; i >= 0; --i)
            {
                if (polygons[i].Inside(vector))
                {
                    return polygons[i];
                }
            }

            return null;
        }


        public Form1()
        {
            // Включаем двойную буферизацию
            SetStyle(ControlStyles.DoubleBuffer |
                     ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.ResizeRedraw, // Перерисовывать при изменении размера окна
                true);
            UpdateStyles();

            LoadObj();
        }

        private void LoadObj()
        {
            List<Polygon> polygons = new List<Polygon>();
            
            var objLoaderFactory = new ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
//            var fileStream = new FileStream("G:\\универ\\4 курс\\компьютерная графика\\Kompyuteraya_grafika\\Компьютерая графика\\obj файлы\\Hammer.obj",
            var fileStream = new FileStream("G:\\универ\\4 курс\\компьютерная графика\\Kompyuteraya_grafika\\Компьютерая графика\\obj файлы\\Hammer.obj",
                FileMode.Open);
            var loadedObj = objLoader.Load(fileStream);

            var k = 0;
            foreach (var g in loadedObj.Groups)
            {
                foreach (var f in g.Faces)
                {
                    var p = new Polygon {color = Color.FromArgb(255, 255, 128, 64), id = k};
                    for (var i = 0; i < f.Count; i++)
                    {
                        p.AddPoint(new Vector3D(
                            loadedObj.Vertices[f[i].VertexIndex - 1].X,
                            loadedObj.Vertices[f[i].VertexIndex - 1].Y,
                            loadedObj.Vertices[f[i].VertexIndex - 1].Z
                        ));
                    }

                    polygons.Add(p);
                    k += 1;
                }
            }

            fileStream.Close();
            _polygons = polygons;
        }

        public static Vector3D GetCenter(List<Polygon> polygons)
        {
            var ox = 0f;
            var oy = 0f;
            var oz = 0f;
            var count = 0;
            
            foreach (var polygon in polygons)
            {
                foreach (var point in polygon.points)
                {
                    ox += point.X;
                    oy += point.Y;
                    oz += point.Z;
                    count++;
                }
            }
            
            return new Vector3D(-ox / count, -oy / count, -oz / count);
        }
        
        private List<Polygon> View3D(float scale, float width, float height)
        {
            _figureCenter = GetCenter(_polygons);
            
            var T = Matrix3D.TranslateMatrix(width, height, 0);
            var s = Matrix3D.ScaleMatrix(scale / 4);                
            var t2 = Matrix3D.TranslateMatrix(_figureCenter);
            var rx = Matrix3D.XRotateMatrix(_angleX);
            var ry = Matrix3D.ZRotateMatrix(_angleY);
            var p = Matrix3D.CentralProjection(10000, 10000, 500);
           
            var m = T * s * p * rx * ry * t2;
            
            var sortedPolygonList = new List<Polygon>();
            foreach (var polygon in _polygons)
            {
                var mutatePolygon = new Polygon {color = polygon.color, id = polygon.id};
               
                foreach (var point in polygon.points)
                {
                    var mutatePoint = m * point;
                    mutatePoint.Normalize();
                    mutatePolygon.points.Add(mutatePoint);
                }

                if (polygonId == mutatePolygon.id)
                {
                    mutatePolygon.color = Color.Brown;
                }

                mutatePolygon.FindMidleZValue();
                sortedPolygonList.Add(mutatePolygon);
            }
            
//            sortedPolygonList.Sort(Polygon.ZDepthComparer);
            sortedPolygonList = Polygon.Sort(sortedPolygonList);
            
            var pol = getPolygon(sortedPolygonList, _locationSelect);
            if (pol != null && polygonId == -1)
            {
                polygonId = pol.id;
                pol.color = Color.Brown;
            }
            
            return sortedPolygonList;
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
//            var p0 = new Vector3D(100, 100, 100);
//            var p1 = new Vector3D(100, 100, 200);
//            var p2 = new Vector3D(100, 200, 100);
//            var p3 = new Vector3D(100, 200, 200);
//            var p4 = new Vector3D(200, 100, 100);
//            var p5 = new Vector3D(200, 100, 200);
//            var p6 = new Vector3D(200, 200, 100);
//            var p7 = new Vector3D(200, 200, 200);
//            
//            var pl1 = new Polygon(new []{p0, p1, p3, p2}.ToList(), Color.Aquamarine, 0);
//            var pl4 = new Polygon(new []{p4, p5, p7, p6}.ToList(), Color.Aquamarine, 1);
//
//            var pl2 = new Polygon(new []{p0, p1, p5, p4}.ToList(), Color.Aquamarine, 2);
//            var pl5 = new Polygon(new []{p2, p3, p7, p6}.ToList(), Color.Aquamarine, 3);
//            
//            var pl3 = new Polygon(new []{p0, p2, p6, p4}.ToList(), Color.Aquamarine, 4);
//            var pl6 = new Polygon(new []{p1, p3, p7, p5}.ToList(), Color.Aquamarine, 5);
//            
//            var p = Polygon.Sort(new []{pl1, pl2, pl3, pl4, pl5, pl6}.ToList());
            
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            var b = e.Graphics.ClipBounds;
            var w = Math.Min(b.Width, b.Height);
            var sizeObj = w * 0.025f;
            var wid = b.Width / 2f;
            var hei = b.Height / 2f;
//            _polygons = p;
            
            var p = View3D(sizeObj, wid, hei);

            DrawObj(e.Graphics, p);
        }

        private static void DrawObj(Graphics g, IEnumerable<Polygon> polygons)
        {
            var pen = new Pen(Brushes.Black, 0.3F);
            foreach (var polygon in polygons)
            {
                var pointFArray = new PointF[polygon.points.Count];

                for (var i = 0; i < pointFArray.Length; i++)
                {
                    pointFArray[i] = new PointF(polygon.points[i].X, polygon.points[i].Y);
                }

                g.FillPolygon(new SolidBrush(polygon.color), pointFArray);
                g.DrawPolygon(pen, pointFArray);
            }
        }
    }
}
