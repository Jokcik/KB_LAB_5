using System;

namespace KB_LAB_5.Classes
{
    public class Vector3D
    {
        private readonly float[] _point = new float[4];

        public float X
        {
            get { return _point[0]; }
            set { _point[0] = value; }
        }

        public float Y
        {
            get { return _point[1]; }
            set { _point[1] = value; }
        }

        public float Z
        {
            get { return _point[2]; }
            set { _point[2] = value; }
        }


        private float this[int i]
        {
            get { return _point[i]; }
            set { _point[i] = value; }
        }

        public Vector3D()
        {
        }

        public Vector3D(float x, float y, float z)
        {
            _point[0] = x;
            _point[1] = y;
            _point[2] = z;
            _point[3] = 1;
        }

        public Vector3D(Vector3D point)
        {
            _point[0] = point._point[0];
            _point[1] = point._point[1];
            _point[2] = point._point[2];
            _point[3] = point._point[3];
        }
        
        // Умножение вектора на матрицу
        public static Vector3D operator *( Matrix3D matrix, Vector3D vector )
        {
            var result = new Vector3D();
            result[0] = matrix[0, 0] * vector[0] + matrix[0, 1] * vector[1] + matrix[0, 2] * vector[2] + matrix[0, 3] * vector[3];
            result[1] = matrix[1, 0] * vector[0] + matrix[1, 1] * vector[1] + matrix[1, 2] * vector[2] + matrix[1, 3] * vector[3];
            result[2] = matrix[2, 0] * vector[0] + matrix[2, 1] * vector[1] + matrix[2, 2] * vector[2] + matrix[2, 3] * vector[3];
            result[3] = matrix[3, 0] * vector[0] + matrix[3, 1] * vector[1] + matrix[3, 2] * vector[2] + matrix[3, 3] * vector[3];

            return result;
        }

        private static double getZ(double Ox, double Oy, double Ax, double Ay, double Az,
            double Bx, double By, double Bz)
        {
            if (Math.Abs(Bx - Ax) > Math.Abs(By - Ay))
            {
                return (Ox - Ax) / (Bx - Ax) * (Bz - Az) + Az;
            }
            if (Math.Abs(Bx - Ax) < Math.Abs(By - Ay))
            {
                return (Oy - Ay) / (By - Ay) * (Bz - Az) + Az;
            }
            if (Math.Abs(Bx - Ax) < 0.000001)
            {
                return 0;
            }

            return (Ox - Ax) / (Bx - Ax) * (Bz - Az) + Az;
        }
        
        public static bool intersect(Vector3D a, Vector3D b, Vector3D c, Vector3D d, out double zAB, out double zCD, bool isLine = false)
        {
            zAB = 0;
            zCD = 0;
            var ab = new Vector3D(b.X - a.X, b.Y - a.Y, 0);
            var cd = new Vector3D(d.X - c.X, d.Y - c.Y, 0);
            double A1 = -ab.Y, B1 = ab.X, C1 = -A1 * a.X - B1 * a.Y;
            double A2 = -cd.Y, B2 = cd.X, C2 = -A2 * c.X - B2 * c.Y;

            var D = A1 * B2 - A2 * B1;
            var Dx = -C1 * B2 + C2 * B1;
            var Dy = -C2 * A1 + C1 * A2;

            if (Math.Abs(D) < 0.000001) return false;
            
            var ox = Dx / D;
            var oy = Dy / D;

            if (!isLine)
            {
                if (!(Math.Min(a.X, b.X) <= ox && ox <= Math.Max(a.X, b.X)))
                    return false;

                if (!(Math.Min(a.Y, b.Y) <= oy && oy <= Math.Max(a.Y, b.Y)))
                    return false;

                if (!(Math.Min(c.X, d.X) <= ox && ox <= Math.Max(c.X, d.X)))
                    return false;

                if (!(Math.Min(c.Y, d.Y) <= oy && oy <= Math.Max(c.Y, d.Y)))
                    return false;
            }
            
            zAB = getZ(ox, oy, a.X, a.Y, a.Z, b.X, b.Y, b.Z);
            zCD = getZ(ox, oy, c.X, c.Y, c.Z, d.X, d.Y, d.Z);

            return true;
        }

        // Нормализация точки
        public void Normalize()
        {
            X /= _point[3];
            Y /= _point[3];
            Z /= _point[3];
            _point[3] = 1;
        }
    }
}