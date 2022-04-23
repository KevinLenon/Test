using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using LearnOpenTK.Common;
using OpenTK.Windowing.Common;
using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing.Imaging;

namespace Test
{
    internal class Assets_3D
    {
        List<Vector3> _vertices = new List<Vector3>();
        List<uint> _indices = new List<uint>();
        float[] _color = new float[4];
        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject;
        Shader _shader;
        int index;
        int[] _pascal;
        int indexGaris = 2;
        Matrix4 _view;
        Matrix4 _projection;
        Matrix4 _model;

        public Vector3 colors;
        public Vector3 _centerPosition = new Vector3(0, 0, 0);
        public List<Vector3> _euler = new List<Vector3>();
        public List<Assets_3D> Child;

        public Assets_3D(float r, float g, float b, float alpha)
        {
            _color[0] = r;
            _color[1] = g;
            _color[2] = b;
            _color[3] = alpha;

            _vertices = new List<Vector3>();
            setdefault();
        }

        public Assets_3D()
        {
            _vertices = new List<Vector3>();
            setdefault();
        }
        public Assets_3D(Vector3 color)
        {
            this.colors = color;
            setdefault();
        }


        public void setdefault()
        {
            _euler = new List<Vector3>();
            //sumbu X
            _euler.Add(new Vector3(1, 0, 0));
            //sumbu y
            _euler.Add(new Vector3(0, 1, 0));
            //sumbu z
            _euler.Add(new Vector3(0, 0, 1));
            _model = Matrix4.Identity;
            _centerPosition = new Vector3(0, 0, 0);
            Child = new List<Assets_3D>();
        }

        public void load(string shadervert, string shaderfrag, float Size_x, float Size_y)
        {
            //inisialisasi
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            //Untuk segitiga berwarna
            /*_vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);*/

            if (_indices.Count != 0)
            {
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Count
                    * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);
            }

            _shader = new Shader(shadervert, shaderfrag);
            _shader.Use();

            _view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);

            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size_x / (float)Size_y, 0.01f, 100.0f);

            foreach (var item in Child)
            {
                item.load(shadervert, shaderfrag, Size_x, Size_y);
            }
        }

        public void render(int pilihan,Matrix4 temp, double time, Matrix4 camera_view, Matrix4 camera_projection)
        {
            _shader.Use();

            int vertexColorLocation = GL.GetUniformLocation(_shader.Handle, "unicolor");
            GL.Uniform4(vertexColorLocation, _color[0], _color[1], _color[2], _color[3]);

            GL.BindVertexArray(_vertexArrayObject);

            _model = temp;

            _shader.SetMatrix4("model", _model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);

            //Matrix4 model = Matrix4.Identity;
            //model += model * Matrix4.CreateTranslation(0.0f, 0.3f, 0.0f);
            if (_indices.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else
            {

                if (pilihan == 0)
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
                }
                else if (pilihan == 1)
                {
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);
                }
                else if (pilihan == 2)
                {

                }
                else if (pilihan == 3)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertices.Count);
                }
            }
            foreach (var item in Child)
            {
                item.render(pilihan,  temp, time, camera_view, camera_projection);
            }


        }

        public void createBoxVertices(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }
        public void createReversePyramidBox(float x, float y, float z, float length, float wide, float depth, float slope)
        {

            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //1 2 5 6 -> Tutup (Atas)
            //3 4 7 8 -> Alas (Bawah)


            //TITIK 1 
            temp_vector.X = x - (slope + length) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + (slope + length) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - (length + slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + (length + slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createBoxVertices2(float x, float y, float z, float xLength, float yLength, float zLength)
        {
            //biar lebih fleksibel jangan inisialiasi posisi dan 
            //panjang kotak didalam tapi ditaruh ke parameter
            float _positionX = x;
            float _positionY = y;
            float _positionZ = z;

            //Buat temporary vector
            Vector3 temp_vector;
            //1. Inisialisasi vertex
            // Titik 1
            temp_vector.X = _positionX - xLength / 2.0f; // x 
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 2
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z

            _vertices.Add(temp_vector);
            // Titik 3
            temp_vector.X = _positionX - xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z
            _vertices.Add(temp_vector);

            // Titik 4
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 5
            temp_vector.X = _positionX - xLength / 2.0f; // x
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 6
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 7
            temp_vector.X = _positionX - xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 8
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);
            //2. Inisialisasi index vertex
            _indices = new List<uint> {
                // Segitiga Depan 1
                0, 1, 2,
                // Segitiga Depan 2
                1, 2, 3,
                // Segitiga Atas 1
                0, 4, 5,
                // Segitiga Atas 2
                0, 1, 5,
                // Segitiga Kanan 1
                1, 3, 5,
                // Segitiga Kanan 2
                3, 5, 7,
                // Segitiga Kiri 1
                0, 2, 4,
                // Segitiga Kiri 2
                2, 4, 6,
                // Segitiga Belakang 1
                4, 5, 6,
                // Segitiga Belakang 2
                5, 6, 7,
                // Segitiga Bawah 1
                2, 3, 6,
                // Segitiga Bawah 2
                3, 6, 7
            };
        }

        public void createPararelogram(float x, float y, float z, float length, float wide, float depth, float slope)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - (length - slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + (length + slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - (length - slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + (length + slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }
        public void createPyramidBox(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createJajarBox(float x, float y, float z, float length)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //TITIK 1
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z - length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / 4.0f;
            temp_vector.Y = y + length / 4.0f;
            temp_vector.Z = z + length / 4.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }

        public void createEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 300)
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 300)
                {
                    temp_vector.X = _x + (float)Math.Cos(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Cos(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (float)Math.Sin(v) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }
        public void createNewEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Sin(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }
        public void createEllipsoid2(float x, float y, float z, float radX, float radY, float radZ, float sectorCount, float stackCount)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * pi / sectorCount;
            float stackStep = pi / stackCount;
            float sectorAngle, stackAngle, tempX, tempY, tempZ;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = pi / 2 - i * stackStep;
                tempX = radX * (float)Math.Cos(stackAngle);
                tempY = radY * (float)Math.Sin(stackAngle);
                tempZ = radZ * (float)Math.Cos(stackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x + tempX * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = y + tempY;
                    temp_vector.Z = z + tempZ * (float)Math.Sin(sectorAngle);

                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);

                    }

                    if (i != stackCount - 1)
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }

        public void createHalfEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float u = 0; u <= pi; u += pi / 360)
            {
                for (float v = -pi / 2; v <= 0; v += pi / 360)
                {
                    temp_vector.X = _x + (float)Math.Cos(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Cos(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (float)Math.Sin(v) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createCylinder(float radius, float height, float _x, float _y, float _z)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float i = -pi / 2; i <= pi / 2; i += pi / 360)
            {
                for (float j = -pi; j <= pi; j += pi / 360)
                {
                    temp_vector.X = radius * (float)Math.Cos(i) * (float)Math.Cos(j) + _centerPosition.X;
                    temp_vector.Y = radius * (float)Math.Cos(i) * (float)Math.Sin(j) + _centerPosition.Y;
                    if (temp_vector.Y < _centerPosition.Y)
                        temp_vector.Y = _centerPosition.Y - height * 0.5f;
                    else
                        temp_vector.Y = _centerPosition.Y + height * 0.5f;
                    temp_vector.Z = radius * (float)Math.Sin(i) + _centerPosition.Z;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createEllipticParaboloid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float v = -pi; v <= pi; v += pi / 300)
            {
                for (float u = 0; u <= 15; u += pi / 300)
                {
                    temp_vector.X = _x + v * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + v * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (v * v) * radiusZ;
                    _vertices.Add(temp_vector);

                }
            }
        }

        public void createTorus(float x, float y, float z, float radMajor, float radMinor, float sectorCount, float stackCount)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            stackCount *= 2;
            float sectorStep = 2 * pi / sectorCount;
            float stackStep = 2 * pi / stackCount;
            float sectorAngle, stackAngle, tempX, tempY, tempZ;

            for (int i = 0; i <= stackCount; ++i)
            {
                stackAngle = pi / 2 - i * stackStep;
                tempX = radMajor + radMinor * (float)Math.Cos(stackAngle);
                tempY = radMinor * (float)Math.Sin(stackAngle);
                tempZ = radMajor + radMinor * (float)Math.Cos(stackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x + tempX * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = y + tempY;
                    temp_vector.Z = z + tempZ * (float)Math.Sin(sectorAngle);

                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);

                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    _indices.Add(k1);
                    _indices.Add(k2);
                    _indices.Add(k1 + 1);

                    _indices.Add(k1 + 1);
                    _indices.Add(k2);
                    _indices.Add(k2 + 1);
                }
            }
        }
        public void rotate(Vector3 pivot, Vector3 vector, float angle)
        {
            //pivot -> mau rotate di titik mana
            //vector -> mau rotate di sumbu apa? (x,y,z)
            //angle -> rotatenya berapa derajat?
            var real_angle = angle;
            angle = MathHelper.DegreesToRadians(angle);

            //mulai ngerotasi
            for (int i = 0; i < _vertices.Count; i++)
            {
                _vertices[i] = getRotationResult(pivot, vector, angle, _vertices[i]);
            }
            //rotate the euler direction
            for (int i = 0; i < 3; i++)
            {
                _euler[i] = getRotationResult(pivot, vector, angle, _euler[i], true);

                //NORMALIZE
                //LANGKAH - LANGKAH
                //length = akar(x^2+y^2+z^2)
                float length = (float)Math.Pow(Math.Pow(_euler[i].X, 2.0f) + Math.Pow(_euler[i].Y, 2.0f) + Math.Pow(_euler[i].Z, 2.0f), 0.5f);
                Vector3 temporary = new Vector3(0, 0, 0);
                temporary.X = _euler[i].X / length;
                temporary.Y = _euler[i].Y / length;
                temporary.Z = _euler[i].Z / length;
                _euler[i] = temporary;
            }
            _centerPosition = getRotationResult(pivot, vector, angle, _centerPosition);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes,
                _vertices.ToArray(), BufferUsageHint.StaticDraw);
            foreach (var item in Child)
            {
                item.rotate(pivot, vector, real_angle);
            }
        }

        Vector3 getRotationResult(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false)
        {
            Vector3 temp, newPosition;
            if (isEuler)
            {
                temp = point;
            }
            else
            {
                temp = point - pivot;
            }

            newPosition.X =
                (float)temp.X * (float)(Math.Cos(angle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(angle))) +
                (float)temp.Y * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) - vector.Z * Math.Sin(angle)) +
                (float)temp.Z * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) + vector.Y * Math.Sin(angle));
            newPosition.Y =
                (float)temp.X * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) + vector.Z * Math.Sin(angle)) +
                (float)temp.Y * (float)(Math.Cos(angle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(angle))) +
                (float)temp.Z * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) - vector.X * Math.Sin(angle));
            newPosition.Z =
                (float)temp.X * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) - vector.Y * Math.Sin(angle)) +
                (float)temp.Y * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) + vector.X * Math.Sin(angle)) +
                (float)temp.Z * (float)(Math.Cos(angle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(angle)));

            if (isEuler)
            {
                temp = newPosition;
            }
            else
            {
                temp = newPosition + pivot;
            }
            return temp;
        }

        public void resetEuler()
        {
            _euler[0] = new Vector3(1, 0, 0);
            _euler[1] = new Vector3(0, 1, 0);
            _euler[2] = new Vector3(0, 0, 1);
        }

        public void createHyperboloid(float _positionX, float _positionY, float _positionZ, float _radius)
        {
            Vector3 temp_vector;
            float _pi = (float)Math.PI;


            for (float v = -_pi / 2; v <= _pi / 2; v += 0.01f)
            {
                for (float u = -_pi; u <= _pi; u += (_pi / 30))
                {
                    temp_vector.X = _positionX + _radius * (1 / (float)Math.Cos(v)) * (float)Math.Cos(u);
                    temp_vector.Y = _positionY + _radius * (1 / (float)Math.Cos(v)) * (float)Math.Sin(u);
                    temp_vector.Z = _positionZ + _radius * (float)Math.Tan(v);
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createReversePyramidBox(bool createObj, float x, float y, float z, float length, float wide, float depth, float slope)
        {

            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;
            Vector3 temp_vector;

            //1 2 5 6 -> Tutup (Atas)
            //3 4 7 8 -> Alas (Bawah)


            //TITIK 1 
            temp_vector.X = x - (slope + length) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + (slope + length) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z - wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - (length + slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + (length + slope) / 2.0f;
            temp_vector.Y = y + depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - depth / 2.0f;
            temp_vector.Z = z + wide / 2.0f;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //SEGITIGA DEPAN 1
                0,1,2,
                //SEGITIGA DEPAN 2
                1,2,3,
                //SEGITIGA ATAS 1
                0,4,5,
                //SEGITIGA ATAS 2
                0,1,5,
                //SEGITIGA KANAN 1
                1,3,5,
                //SEGITIGA KANAN 2
                3,5,7,
                //SEGITIGA KIRI 1
                0,2,4,
                //SEGITIGA KIRI 2
                2,4,6,
                //SEGITIGA BELAKANG 1
                4,5,6,
                //SEGITIGA BELAKANG 2
                5,6,7,
                //SEGITIGA BAWAH 1
                2,3,6,
                //SEGITIGA BAWAH 2
                3,6,7
            };
        }
        public void createBoxVertices(float x, float y, float z, float xLength, float yLength, float zLength)
        {
            //biar lebih fleksibel jangan inisialiasi posisi dan 
            //panjang kotak didalam tapi ditaruh ke parameter
            float _positionX = x;
            float _positionY = y;
            float _positionZ = z;

            //Buat temporary vector
            Vector3 temp_vector;
            //1. Inisialisasi vertex
            // Titik 1
            temp_vector.X = _positionX - xLength / 2.0f; // x 
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 2
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z

            _vertices.Add(temp_vector);
            // Titik 3
            temp_vector.X = _positionX - xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z
            _vertices.Add(temp_vector);

            // Titik 4
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ - zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 5
            temp_vector.X = _positionX - xLength / 2.0f; // x
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 6
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY + yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 7
            temp_vector.X = _positionX - xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);

            // Titik 8
            temp_vector.X = _positionX + xLength / 2.0f; // x
            temp_vector.Y = _positionY - yLength / 2.0f; // y
            temp_vector.Z = _positionZ + zLength / 2.0f; // z

            _vertices.Add(temp_vector);
            //2. Inisialisasi index vertex
            _indices = new List<uint> {
                // Segitiga Depan 1
                0, 1, 2,
                // Segitiga Depan 2
                1, 2, 3,
                // Segitiga Atas 1
                0, 4, 5,
                // Segitiga Atas 2
                0, 1, 5,
                // Segitiga Kanan 1
                1, 3, 5,
                // Segitiga Kanan 2
                3, 5, 7,
                // Segitiga Kiri 1
                0, 2, 4,
                // Segitiga Kiri 2
                2, 4, 6,
                // Segitiga Belakang 1
                4, 5, 6,
                // Segitiga Belakang 2
                5, 6, 7,
                // Segitiga Bawah 1
                2, 3, 6,
                // Segitiga Bawah 2
                3, 6, 7
            };
        }
        public void createEllipsoidV2(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Sin(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }

       

        public void createStaircase(float x, float y, float z, float wide, int steps)
        {
            float stepHeigth = 0.0125f * 1.5f;
            //true heigth = 0.025f

            createBoxVertices(x, y, z, 0.03f, stepHeigth, wide);

            for (int i = 1; i <= steps + 1; i++)
            {
                x += 0.03f;
                stepHeigth += (0.0125f * 1.5f);
                y += (0.00625f * 1.5f);

                createBoxChild(x, y, z, 0.03f, stepHeigth, wide);

                /*
                if(i % 2 == 0)
                {
                    createBoxChild(x, y, z, 0.03f, stepHeigth, wide,0.5f,0.5f,0.5f,1f);
                }
                else
                {
                    createBoxChild(x, y, z, 0.03f, stepHeigth, wide);
                }
                */
            }
        }

        public void addChild(float x, float y, float z, float length,int pilihan,Vector3 colors)
        {
            Assets_3D newChild = new Assets_3D(colors);
            if(pilihan == 0)
            {
                newChild.createBoxVertices(x, y, z, length);
            }
            else if(pilihan == 1)
            {
                newChild.createPyramidBox(x, y, z, length);
            }
            else if (pilihan == 2)
            {
                newChild.createJajarBox(x, y, z, length);            
            }

            Child.Add(newChild);
        }

        public void addChild(float x, float y, float z, float length, int pilihan, float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);
            if (pilihan == 0)
            {
                newChild.createBoxVertices(x, y, z, length);
            }
            else if (pilihan == 1)
            {
                newChild.createPyramidBox(x, y, z, length);
            }
            else if (pilihan == 2)
            {
                newChild.createJajarBox(x, y, z, length);
            }

            Child.Add(newChild);
        }

        public void addChildBalok(float x, float y, float z, float length, float wide, float depth,int pilihan,Vector3 colors)
        {
            Assets_3D newChild = new Assets_3D(colors);
            if (pilihan == 1)
            {
                newChild.createBoxVertices(x, y, z, length, wide, depth);
            }

            Child.Add(newChild);
        }

        public void addChildBalok(float x, float y, float z, float length, float wide, float depth, int pilihan, float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);
            if (pilihan == 1)
            {
                newChild.createBoxVertices(x, y, z, length, wide, depth);
            }

            Child.Add(newChild);
        }



        public void addChildElipsoid(float x, float y, float z, float radX, float radY, float radZ, int sectorCount, int stackCount,Vector3 colors)
        {
            Assets_3D newChild = new Assets_3D(colors);
            newChild.createEllipsoid2(x, y, z, radX, radY, radZ, sectorCount, stackCount);
            Child.Add(newChild);
        }

        public void addChildElipsoid(float x, float y, float z, float radX, float radY, float radZ, int sectorCount, int stackCount,float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);
            newChild.createEllipsoid2(x, y, z, radX, radY, radZ, sectorCount, stackCount);
            Child.Add(newChild);
        }

        public void createBoxChild(float x, float y, float z, float lx, float ly, float lz)
        {
            Assets_3D newChild = new Assets_3D(_color[0], _color[1], _color[2], _color[3]);

            newChild.createBoxVertices(x, y, z, lx, ly, lz);

            Child.Add(newChild);
        }

        public void createBoxChild(float x, float y, float z, float lx, float ly, float lz, float r, float g, float b, float alpha)
        {
            Assets_3D newChild = new Assets_3D(r, g, b, alpha);

            newChild.createBoxVertices(x, y, z, lx, ly, lz);

            Child.Add(newChild);
        }

        public void createPararelogramChild(float x, float y, float z, float lx, float ly, float lz, float slope)
        {
            Assets_3D newChild = new Assets_3D(_color[0], _color[1], _color[2], _color[3]);

            newChild.createBoxVertices(x, y, z, lx, ly, lz);

            Child.Add(newChild);
        }

    }
}
