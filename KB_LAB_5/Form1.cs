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
        private Vector3D _figureCenter;
        private float size = 100f;
        
        private float angleX = 0f;
        private float angleY = 0f;
        private float angleZ = 0f;

        private Point currentLocation = new Point(0, 0);

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            angleY += (e.Location.X - currentLocation.X) / 1.0f;
            angleX += (e.Location.Y - currentLocation.Y) / 1.0f;
            currentLocation = e.Location;
            
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            currentLocation = e.Location;
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
            var fileStream = new FileStream("G:\\универ\\4 курс\\компьютерная графика\\Kompyuteraya_grafika\\Компьютерая графика\\obj файлы\\Hammer.obj",
//            var fileStream = new FileStream("G:\\универ\\4 курс\\компьютерная графика\\Kompyuteraya_grafika\\Компьютерая графика\\obj файлы\\Toilet.obj",
                FileMode.Open);
            var loadedObj = objLoader.Load(fileStream);

            foreach (var g in loadedObj.Groups)
            {
                foreach (var f in g.Faces)
                {
                    Polygon p = new Polygon();
                    p.color = Color.FromArgb(255, 255, 128, 64);
//                    values.Add(f.Count);
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
            var Ox = 0f;
            var Oy = 0f;
            var Oz = 0f;
            var count = 0;
            
            foreach (var polygon in _polygons)
            {
                foreach (var point in polygon.points)
                {
                    Ox += point.X;
                    Oy += point.Y;
                    Oz += point.Z;
                    count++;
                }
            }
            
            return new Vector3D(-Ox / count, -Oy / count, -Oz / count);
        }
        
        private List<Polygon> View3D(float scale, float width, float height)
        {
            _figureCenter = GetCenter();
            
            var T = Matrix3D.TranslateMatrix(width, height, 0);
            var S = Matrix3D.ScaleMatrix(scale / 2);
            var T2 = Matrix3D.TranslateMatrix(_figureCenter);
            var Rx = Matrix3D.XRotateMatrix(angleX);
            var Ry = Matrix3D.ZRotateMatrix(angleY);
            var P = Matrix3D.CentralProjection(10000, 10000, 500);
           
            var m = T * P * S * Rx * Ry * T2;
            
            List<Polygon> sortedPolygonList = new List<Polygon>();
            foreach (Polygon polygon in _polygons)
            {
                Polygon mutatePolygon = new Polygon {color = polygon.color};

                foreach (Vector3D point in polygon.points)
                {
                    Vector3D mutatePoint = m * point;
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
            var size = w * 0.025f;
            var wid = b.Width / 2f;
            var hei = b.Height / 2f;
            
            var p = View3D(size, wid, hei);
            DrawObj(e.Graphics, p);
        }

        private void DrawObj(Graphics g, List<Polygon> polygons)
        {
            Pen pen = new Pen(Brushes.Black, 0.3F);
            foreach (Polygon polygon in polygons)
            {
                PointF[] pointFArray = new PointF[polygon.points.Count];

                for (int i = 0; i < pointFArray.Length; i++)
                {
                    pointFArray[i] = new PointF(polygon.points[i].X, polygon.points[i].Y);
                }

                g.FillPolygon(new SolidBrush(polygon.color), pointFArray);
                g.DrawPolygon(pen, pointFArray);
            }
            
            
        }
       
    }
}
