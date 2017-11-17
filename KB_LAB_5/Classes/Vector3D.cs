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