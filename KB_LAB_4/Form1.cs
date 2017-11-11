using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using KB_LAB_4.Classes;

namespace KB_LAB_4
{
    public class Form1 : Form
    {
        private float size = 100f;
        
        private float angleX = 0f;
        private float angleY = 0f;

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
            
//            update();
        }

        private Vector3D[] getObj()
        {
            var newObj = new Vector3D[8];
            newObj[0] = new Vector3D( 1,  1,  1);
            newObj[1] = new Vector3D(-1,  1,  1);
            newObj[2] = new Vector3D(-1, -1,  1);
            newObj[3] = new Vector3D( 1, -1,  1);
            newObj[4] = new Vector3D( 1,  1, -1);
            newObj[5] = new Vector3D(-1,  1, -1);
            newObj[6] = new Vector3D(-1, -1, -1);
            newObj[7] = new Vector3D( 1, -1, -1);

            return newObj;
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
            
            var T = Matrix3D.TranslateMatrix(-width, -height, 0);
            var S = Matrix3D.ScaleMatrix(scale / 2);
            var T2 = Matrix3D.TranslateMatrix(center);

            var newObj = new Vector3D[obj.Length];
            var m = T * S * T2;
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
            
            var T = Matrix3D.TranslateMatrix(-width, -height, 0);
            var S = Matrix3D.ScaleMatrix(scale / 2);
            var T2 = Matrix3D.TranslateMatrix(center);
            var R = Matrix3D.YRotateMatrix(90);

            var newObj = new Vector3D[obj.Length];
            var m = T * S * T2 * R;
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
            
            var T = Matrix3D.TranslateMatrix(-width, -height, 0);
            var S = Matrix3D.ScaleMatrix(scale / 2);
            var T2 = Matrix3D.TranslateMatrix(center);
            var Rx = Matrix3D.XRotateMatrix(angleX);
            var Ry = Matrix3D.YRotateMatrix(angleY);
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
            
            var T = Matrix3D.TranslateMatrix(-width, -height, 0);
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
            var size = w * 0.5f;
            
            var p = FrontView(size, w / 4f, w / 4f);
            DrawObj(e.Graphics, p);
            
            p = SideView(size, 3 * w / 4f, w / 4f);
            DrawObj(e.Graphics, p);
           
            p = BottomView(size, w / 4f, 3 * w / 4f);
            DrawObj(e.Graphics, p);
            
            p = View3D(size, 3 * w / 4f, 3 * w / 4f);
            DrawObj(e.Graphics, p);
        }

        private void DrawObj(Graphics g, Vector3D[] p)
        {
            var path = new GraphicsPath();
            AddLine(path, p[0], p[1], p[2], p[3], p[0], p[4], p[5], p[6], p[7], p[4]);
            AddLine(path, p[5], p[1]);
            AddLine(path, p[2], p[6]);
            AddLine(path, p[6], p[7]);
            AddLine(path, p[7], p[3]);
            //рисуем
            g.DrawPath(Pens.Red, path);
        }
        
        void AddLine(GraphicsPath path, params Vector3D[] points)
        {
            foreach(var p in points)
                path.AddLines(new [] {new PointF(p.X, p.Y)});
        }
    }
}
