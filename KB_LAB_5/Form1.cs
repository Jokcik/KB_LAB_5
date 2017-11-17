using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using KB_LAB_5.Classes;
using ObjLoader.Loader.Loaders;

namespace KB_LAB_5
{
    public class Form1 : Form
    {
        private List<Polygon> _polygons = new List<Polygon>();
        private Vector3D _figureCenter;

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
            var fileStream = new FileStream("G:\\универ\\4 курс\\компьютерная графика\\Kompyuteraya_grafika\\Компьютерая графика\\obj файлы\\Toilet.obj",
                FileMode.Open);
            var loadedObj = objLoader.Load(fileStream);

            foreach (var g in loadedObj.Groups)
            {
                foreach (var f in g.Faces)
                {
                    var p = new Polygon {color = Color.FromArgb(255, 255, 128, 64)};
                    for (var i = 0; i < f.Count; i++)
                    {
                        p.AddPoint(new Vector3D(
                            loadedObj.Vertices[f[i].VertexIndex - 1].X,
                            loadedObj.Vertices[f[i].VertexIndex - 1].Y,
                            loadedObj.Vertices[f[i].VertexIndex - 1].Z
                        ));
                    }
                    polygons.Add(p);
                }
            }

            fileStream.Close();
            _polygons = polygons;
        }

        private Vector3D GetCenter()
        {
            var ox = 0f;
            var oy = 0f;
            var oz = 0f;
            var count = 0;
            
            foreach (var polygon in _polygons)
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
        
        private IEnumerable<Polygon> View3D(float scale, float width, float height)
        {
            _figureCenter = GetCenter();
            
            var T = Matrix3D.TranslateMatrix(width, height, 0);
            var s = Matrix3D.ScaleMatrix(scale / 2);
            var t2 = Matrix3D.TranslateMatrix(_figureCenter);
            var rx = Matrix3D.XRotateMatrix(_angleX);
            var ry = Matrix3D.ZRotateMatrix(_angleY);
            var p = Matrix3D.CentralProjection(10000, 10000, 500);
           
            var m = T * p * s * rx * ry * t2;
            
            var sortedPolygonList = new List<Polygon>();
            foreach (var polygon in _polygons)
            {
                var mutatePolygon = new Polygon {color = polygon.color};

                foreach (var point in polygon.points)
                {
                    var mutatePoint = m * point;
                    mutatePoint.Normalize();
                    mutatePolygon.points.Add(mutatePoint);
                }

                mutatePolygon.findMidleZValue();
                sortedPolygonList.Add(mutatePolygon);
            }
            
            sortedPolygonList.Sort(Polygon.ZDepthComparer);

            return sortedPolygonList;
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            var b = e.Graphics.ClipBounds;
            var w = Math.Min(b.Width, b.Height);
            var sizeObj = w * 0.025f;
            var wid = b.Width / 2f;
            var hei = b.Height / 2f;
            
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
