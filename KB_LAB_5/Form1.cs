using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using KB_LAB_4.Classes;
using ObjLoader.Loader.Loaders;

namespace KB_LAB_4
{
    public class Form1 : Form
    {
        private List<Vector3D> vectors = new List<Vector3D>();
        private List<int> values = new List<int>();

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
                    values.Add(f.Count);
                    for (var i = 0; i < f.Count; i++)
                    {
                        vectors.Add(new Vector3D(
                            loadedObj.Vertices[f[i].VertexIndex - 1].X,
                            loadedObj.Vertices[f[i].VertexIndex - 1].Y,
                            loadedObj.Vertices[f[i].VertexIndex - 1].Z
                        ));
                    }
                }
            }

            fileStream.Close();
        }

        private Vector3D[] getObj()
        {
//            var newObj = new Vector3D[8];
//            newObj[0] = new Vector3D( 1,  1,  1);
//            newObj[1] = new Vector3D(-1,  1,  1);
//            newObj[2] = new Vector3D(-1, -1,  1);
//            newObj[3] = new Vector3D( 1, -1,  1);
//            newObj[4] = new Vector3D( 1,  1, -1);
//            newObj[5] = new Vector3D(-1,  1, -1);
//            newObj[6] = new Vector3D(-1, -1, -1);
//            newObj[7] = new Vector3D( 1, -1, -1);

            return vectors.ToArray();
        }

        private Vector3D GetCenter(Vector3D[] vector)
        {
            var Ox = vector.Select(d => d.X).Sum() / vector.Length;
            var Oy = vector.Select(d => d.Y).Sum() / vector.Length;
            var Oz = vector.Select(d => d.Z).Sum() / vector.Length;
            
            return new Vector3D(-Ox, -Oy, -Oz);
        }
        
        private Vector3D[] FrontView(float scale, float width, float height)
        {
            var obj = getObj();
            var center = GetCenter(obj);
            
            var T = Matrix3D.TranslateMatrix(width, height, 0);
            var S = Matrix3D.ScaleMatrix(scale / 2);
            var T2 = Matrix3D.TranslateMatrix(center);

            var newObj = new Vector3D[obj.Length];
            var m = T * Matrix3D.ZOrthogonalMatrix() * S * T2;
            for (var i = 0; i < newObj.Length; i++)
            {
                newObj[i] = m*obj[i];
                newObj[i].Normalize();                
            }
            
            return newObj;
        }

        private Vector3D[] SideView(float scale, float width, float height)
        {
            var obj = getObj();
            var center = GetCenter(obj);
            
            var T = Matrix3D.TranslateMatrix(width, height, 0);
            var S = Matrix3D.ScaleMatrix(scale / 2);
            var T2 = Matrix3D.TranslateMatrix(center);
            var R = Matrix3D.YRotateMatrix(90);

            var newObj = new Vector3D[obj.Length];
            var m = T * Matrix3D.ZOrthogonalMatrix() * S * T2 * R;
            for (var i = 0; i < newObj.Length; i++)
            {
                newObj[i] = m*obj[i];
                newObj[i].Normalize();                
            }
            
            return newObj;
        }

        private Vector3D[] View3D(float scale, float width, float height)
        {
            var obj = getObj();
            var center = GetCenter(obj);
            
            var T = Matrix3D.TranslateMatrix(width, height, 0);
            var S = Matrix3D.ScaleMatrix(scale / 2);
            var T2 = Matrix3D.TranslateMatrix(center);
            var Rx = Matrix3D.XRotateMatrix(angleX);
            var Ry = Matrix3D.ZRotateMatrix(angleY);
            var P = Matrix3D.CentralProjection(10000, 10000, 500);

            var newObj = new Vector3D[obj.Length];
            var m = T * P * S * Rx * Ry * T2;
            for (int i = 0; i < newObj.Length; i++)
            {
                newObj[i] = m*obj[i];
                newObj[i].Normalize();                
            }
            
            return newObj;
        }
        
        private Vector3D[] BottomView(float scale, float width, float height)
        {
            var obj = getObj();
            var center = GetCenter(obj);
            
            var T = Matrix3D.TranslateMatrix(width, height, 0);
            var S = Matrix3D.ScaleMatrix(scale / 2);
            var T2 = Matrix3D.TranslateMatrix(center);
            var R = Matrix3D.XRotateMatrix(90);

            var newObj = new Vector3D[obj.Length];
            var m = T * S * T2 * R;
            for (int i = 0; i < newObj.Length; i++)
            {
                newObj[i] = m*obj[i];
                newObj[i].Normalize();                
            }
            
            return newObj;
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            var b = e.Graphics.ClipBounds;
            var w = Math.Min(b.Width, b.Height);
            var size = w * 0.025f;
            var wid = b.Width / 4f;
            var hei = b.Height / 4f;
            
            var p = FrontView(size, wid, hei);
            DrawObj(e.Graphics, p);
            
            p = SideView(size, 3 * wid, hei);
            DrawObj(e.Graphics, p);
           
            p = BottomView(size, wid, 3 * hei);
            DrawObj(e.Graphics, p);
            
            p = View3D(size, 3 * wid, 3 * hei);
            DrawObj(e.Graphics, p);
        }

        private void DrawObj(Graphics g, IEnumerable<Vector3D> points)
        {
            var sum = 0;
            var ps = points.Select(d => new PointF(d.X, d.Y)).ToArray();
            List<PointF[]> fs = new List<PointF[]>();
            
            foreach (var value in values)
            {
                g.DrawPolygon(Pens.Black, new []{ps[sum], ps[sum + 1], ps[sum + 2], ps[sum + 3]});                
                sum += value;
            }
        }
       
    }
}
