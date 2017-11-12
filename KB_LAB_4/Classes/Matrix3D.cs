using System;

namespace KB_LAB_4.Classes
{
    public class Matrix3D
    {
        private readonly float[,] _matrix = new float[4, 4];

        public float this[int i, int j]
        {
            get { return _matrix[i, j]; }
            private set { _matrix[i, j] = value; }
        }

        public Matrix3D()
        {
        }

        public Matrix3D(float[,] matrix)
        {
            if (matrix.GetLength(1) != 4 && matrix.GetLength(0) != 4)
            {
                throw new ArgumentException("Число строк или столбцов не равно 4");
            }
            
            for (var i = 0; i < 4; i++)
            {
                for (var j = 0; j < 4; j++)
                {
                    _matrix[i, j] = matrix[i, j];
                }
            }
        }

        // Умножение двух матриц
        public static Matrix3D operator *(Matrix3D matrix1, Matrix3D matrix2)
        {
             var m = new Matrix3D();
 
             m[0,0] = matrix1[0,0] * matrix2[0,0] + matrix1[0,1] * matrix2[1,0] + matrix1[0,2] * matrix2[2,0] + matrix1[0,3] * matrix2[3,0];
             m[0,1] = matrix1[0,0] * matrix2[0,1] + matrix1[0,1] * matrix2[1,1] + matrix1[0,2] * matrix2[2,1] + matrix1[0,3] * matrix2[3,1];
             m[0,2] = matrix1[0,0] * matrix2[0,2] + matrix1[0,1] * matrix2[1,2] + matrix1[0,2] * matrix2[2,2] + matrix1[0,3] * matrix2[3,2];
             m[0,3] = matrix1[0,0] * matrix2[0,3] + matrix1[0,1] * matrix2[1,3] + matrix1[0,2] * matrix2[2,3] + matrix1[0,3] * matrix2[3,3];
 
             m[1,0] = matrix1[1,0] * matrix2[0,0] + matrix1[1,1] * matrix2[1,0] + matrix1[1,2] * matrix2[2,0] + matrix1[1,3] * matrix2[3,0];
             m[1,1] = matrix1[1,0] * matrix2[0,1] + matrix1[1,1] * matrix2[1,1] + matrix1[1,2] * matrix2[2,1] + matrix1[1,3] * matrix2[3,1];
             m[1,2] = matrix1[1,0] * matrix2[0,2] + matrix1[1,1] * matrix2[1,2] + matrix1[1,2] * matrix2[2,2] + matrix1[1,3] * matrix2[3,2];
             m[1,3] = matrix1[1,0] * matrix2[0,3] + matrix1[1,1] * matrix2[1,3] + matrix1[1,2] * matrix2[2,3] + matrix1[1,3] * matrix2[3,3];
 
             m[2,0] = matrix1[2,0] * matrix2[0,0] + matrix1[2,1] * matrix2[1,0] + matrix1[2,2] * matrix2[2,0] + matrix1[2,3] * matrix2[3,0];
             m[2,1] = matrix1[2,0] * matrix2[0,1] + matrix1[2,1] * matrix2[1,1] + matrix1[2,2] * matrix2[2,1] + matrix1[2,3] * matrix2[3,1];
             m[2,2] = matrix1[2,0] * matrix2[0,2] + matrix1[2,1] * matrix2[1,2] + matrix1[2,2] * matrix2[2,2] + matrix1[2,3] * matrix2[3,2];
             m[2,3] = matrix1[2,0] * matrix2[0,3] + matrix1[2,1] * matrix2[1,3] + matrix1[2,2] * matrix2[2,3] + matrix1[2,3] * matrix2[3,3];
 
             m[3,0] = matrix1[3,0] * matrix2[0,0] + matrix1[3,1] * matrix2[1,0] + matrix1[3,2] * matrix2[2,0] + matrix1[3,3] * matrix2[3,0];
             m[3,1] = matrix1[3,0] * matrix2[0,1] + matrix1[3,1] * matrix2[1,1] + matrix1[3,2] * matrix2[2,1] + matrix1[3,3] * matrix2[3,1];
             m[3,2] = matrix1[3,0] * matrix2[0,2] + matrix1[3,1] * matrix2[1,2] + matrix1[3,2] * matrix2[2,2] + matrix1[3,3] * matrix2[3,2];
             m[3,3] = matrix1[3,0] * matrix2[0,3] + matrix1[3,1] * matrix2[1,3] + matrix1[3,2] * matrix2[2,3] + matrix1[3,3] * matrix2[3,3];

            return m;
        }
        #region Афинные преобразования в пространстве

        // Матрица вращения вокруг оси абцисс на угол a
        public static Matrix3D XRotateMatrix(float a)
        {
            // Угол в радианах
            float rad = (float) (a * Math.PI / 180);
            return new Matrix3D(
                new[,]
                {
                    {1, 0, 0, 0},
                    {0, (float) Math.Cos(rad), (float) Math.Sin(rad), 0},
                    {0, -(float) Math.Sin(rad), (float) Math.Cos(rad), 0},
                    {0, 0, 0, 1}
                }
            );
        }

        // Матрица вращения вокруг оси ординат на угол a
        public static Matrix3D YRotateMatrix(float a)
        {
            // Угол в радианах
            float rad = (float) (a * Math.PI / 180);
            return new Matrix3D(
                new[,]
                {
                    {(float) Math.Cos(rad), 0, (float) Math.Sin(rad), 0},
                    {0, 1, 0, 0},
                    {-(float) Math.Sin(rad), 0, (float) Math.Cos(rad), 0},
                    {0, 0, 0, 1}
                }
            );
        }

        // Матрица вращения вокруг оси аппликат на угол a
        public static Matrix3D ZRotateMatrix(float a)
        {
            // Угол в радианах
            float rad = (float) (a * Math.PI / 180);
            return new Matrix3D(
                new[,]
                {
                    {(float) Math.Cos(rad), (float) Math.Sin(rad), 0, 0},
                    {-(float) Math.Sin(rad), (float) Math.Cos(rad), 0, 0},
                    {0, 0, 1, 0},
                    {0, 0, 0, 1}
                }
            );
        }

        // Матрица масштабирования
        public static Matrix3D ScaleMatrix(float scale)
        {
            return ScaleMatrix(scale / 2, scale / 2, scale / 2);
        }
        
        public static Matrix3D ScaleMatrix(float sX, float sY, float sZ)
        {
            return new Matrix3D(
                new[,]
                {
                    {sX, 0, 0, 0},
                    {0, sY, 0, 0},
                    {0, 0, sZ, 0},
                    {0, 0, 0, 1}
                }
            );
        }

        // Матрица отражения относительно плоскости YOZ
        public static Matrix3D XReflectMatrix()
        {
            return new Matrix3D(
                new float[,]
                {
                    {-1, 0, 0, 0},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0},
                    {0, 0, 0, 1}
                }
            );
        }

        // Матрица отражения относительно плоскости XOZ
        public static Matrix3D YReflectMatrix()
        {
            return new Matrix3D(
                new float[,]
                {
                    {1, 0, 0, 0},
                    {0, -1, 0, 0},
                    {0, 0, 1, 0},
                    {0, 0, 0, 1}
                }
            );
        }

        // Матрица отражения относительно плоскости XOY
        public static Matrix3D ZReflectMatrix()
        {
            return new Matrix3D(
                new float[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 0, 0},
                    {0, 0, -1, 0},
                    {0, 0, 0, 1}
                }
            );
        }

        // Матрица переноса
        public static Matrix3D TranslateMatrix(Vector3D position)
        {
            return TranslateMatrix(position.X, position.Y, position.Z);
        }
        
        public static Matrix3D TranslateMatrix(float tX, float tY, float tZ)
        {
            return new Matrix3D(
                new[,]
                {
                    {1, 0, 0, tX},
                    {0, 1, 0, tY},
                    {0, 0, 1, tZ},
                    {0, 0, 0, 1}
                }
            );
        }

        #endregion

        #region Проекции

        // Матрица общего вида для ортогональной проекции
        public static Matrix3D OrthogonalMatrix(float x, float y, float z)
        {
            return new Matrix3D(
                new[,]
                {
                    {x, 0, 0, 0},
                    {0, y, 0, 0},
                    {0, 0, z, 0},
                    {0, 0, 0, 1}
                }
            );
        }

        // Матрица для ортогональной прекции относительно оси XOY
        public static Matrix3D ZOrthogonalMatrix()
        {
            return OrthogonalMatrix(1, 1, 0);
        }

        // Матрица для ортогональной прекции относительно оси XOZ
        public static Matrix3D YOrthogonalMatrix()
        {
            return OrthogonalMatrix(1, 0, 1);
        }

        // Матрица для ортогональной прекции относительно оси ZOY
        public static Matrix3D XOrthogonalMatrix()
        {
            return OrthogonalMatrix(0, 1, 1);
        }

        // Матрица для центральной проекции
        public static Matrix3D CentralProjection(float x, float y, float z)
        {
            return new Matrix3D(
                new[,]
                {
                    {1, 0, 0, -1 / x},
                    {0, 1, 0, -1 / y},
                    {0, 0, 1, -1 / z},
                    {0, 0, 0, 1}
                }
            );
        }

        // Косоугольная кабинетная проекция
        public static Matrix3D ObliqueCabinetProjection()
        {
            return new Matrix3D(
                new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 0, 0},
                    {(float) Math.Cos(Math.PI / 4) / 2, (float) Math.Cos(Math.PI / 4) / 2, 1, 0},
                    {0, 0, 0, 1}
                }
            );
        }

        // Косоугольная свободная проекция
        public static Matrix3D ObliqueFreeProjection()
        {
            return new Matrix3D(
                new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 0, 0},
                    {(float) Math.Cos(Math.PI / 4), (float) Math.Cos(Math.PI / 4), 1, 0},
                    {0, 0, 0, 1}
                }
            );
        }

        #endregion
    }
}