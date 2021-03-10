using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace Directx_Textures
{
    public partial class Form1 : Form
    {
        private Device device = null;
        private VertexBuffer vb = null;
        private float angle = 0f;
        private CustomVertex.PositionNormalTextured[] vertices;
        private IndexBuffer ib = null;
        private int[] indices;
        private Bitmap b;
        private Texture tex1;

        public Form1()
        {
            InitializeComponent();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            InitializeDevice();
            VertexDeclaration();
            CameraPositioning();

            b = (Bitmap)Image.FromFile("Logo.bmp");
            tex1 = new Texture(device, b, 0, Pool.Managed);
        }

        public void InitializeDevice()
        {
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;
            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

            //включаем освещение
            device.RenderState.Lighting = true;

            //режим отбрасывания невидимых граней
            device.RenderState.CullMode = Cull.CounterClockwise;
        }

        public void CameraPositioning()
        {
            float x = Convert.ToSingle(10*2 / (Math.Sqrt(3 - Math.Sqrt(5))) * Math.Cos(angle));
            float y = Convert.ToSingle(20*2 / (Math.Sqrt(3 + Math.Sqrt(5))) * Math.Sin(angle));
            float z = 20;
            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, (float)this.Width / (float)this.Height, 1f, 50f);
            device.Transform.View = Matrix.LookAtLH(new Vector3(x, y, z),
                                        new Vector3(0, 0, 0),
                                        new Vector3(0, 1, 0));

            //включаем направленные источники света и настраиваем их
            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = new Vector3(0f, 1f, -1f);
            device.Lights[0].Enabled = true;
        }

        public void VertexDeclaration()
        {
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), 30, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);

            //из-за введения текстурных координат приходится описывать вершины во всех треугольниках
            vertices = new CustomVertex.PositionNormalTextured[30];
            //дно
            vertices[0] = new CustomVertex.PositionNormalTextured(3f, 0f, 0f, 0f, 0f, -1f, 0.5f, 1f);
            vertices[1] = new CustomVertex.PositionNormalTextured(0f, 3f, 0f, 0f, 0f, -1f, 1f, 0.5f);
            vertices[2] = new CustomVertex.PositionNormalTextured(-3f, 0f, 0f, 0f, 0f, -1f, 0.5f, 0f);
            vertices[3] = new CustomVertex.PositionNormalTextured(-1f, -3f, 0f, 0f, 0f, -1f, 0f, 0.33333f);
            vertices[4] = new CustomVertex.PositionNormalTextured(1f, -3f, 0f, 0f, 0f, -1f, 0f, 0.66666f);

            //крыша
            vertices[5] = new CustomVertex.PositionNormalTextured(4f, 1f, 5f, 0f, 0f, 1f, 0.5f, 1f);
            vertices[6] = new CustomVertex.PositionNormalTextured(1f, 4f, 5f, 0f, 0f, 1f, 1f, 0.5f);
            vertices[7] = new CustomVertex.PositionNormalTextured(-2f, 1f, 5f, 0f, 0f, 1f, 0.5f, 0f);
            vertices[8] = new CustomVertex.PositionNormalTextured(0f, -2f, 5f, 0f, 0f, 1f, 0f, 0.33333f);
            vertices[9] = new CustomVertex.PositionNormalTextured(2f, -2f, 5f, 0f, 0f, 1f, 0f, 0.66666f);

            //первая
            vertices[10] = new CustomVertex.PositionNormalTextured(3f, 0f, 0f, (float)(-15 / Math.Sqrt(486)), (float)(-15 / Math.Sqrt(486)), (float)(6 / Math.Sqrt(486)), 1f, 1f);
            vertices[11] = new CustomVertex.PositionNormalTextured(4f, 1f, 5f, (float)(-15 / Math.Sqrt(486)), (float)(-15 / Math.Sqrt(486)), (float)(6 / Math.Sqrt(486)), 1f, 0f);
            vertices[12] = new CustomVertex.PositionNormalTextured(1f, 4f, 5f, (float)(-15 / Math.Sqrt(486)), (float)(-15 / Math.Sqrt(486)), (float)(6 / Math.Sqrt(486)), 0f, 0f);
            vertices[13] = new CustomVertex.PositionNormalTextured(0f, 3f, 0f, (float)(-15 / Math.Sqrt(486)), (float)(-15 / Math.Sqrt(486)), (float)(6 / Math.Sqrt(486)), 0f, 1f);
            //вторая
            vertices[14] = new CustomVertex.PositionNormalTextured(0f, 3f, 0f, (float)(15 / Math.Sqrt(450)), (float)(-15 / Math.Sqrt(450)), 0f, 1f, 1f);
            vertices[15] = new CustomVertex.PositionNormalTextured(1f, 4f, 5f, (float)(15 / Math.Sqrt(450)), (float)(-15 / Math.Sqrt(450)), 0f, 1f, 0f);
            vertices[16] = new CustomVertex.PositionNormalTextured(-2f, 1f, 5f, (float)(15 / Math.Sqrt(450)), (float)(-15 / Math.Sqrt(450)), 0f, 0f, 0f);
            vertices[17] = new CustomVertex.PositionNormalTextured(-3f, 0f, 0f, (float)(15 / Math.Sqrt(450)), (float)(-15 / Math.Sqrt(450)), 0f, 0f, 1f);

            //третья
            vertices[18] = new CustomVertex.PositionNormalTextured(-3f, 0f, 0f, (float)(15 / Math.Sqrt(350)), (float)(10 / Math.Sqrt(350)), (float)(-5 / Math.Sqrt(350)), 1f, 1f);
            vertices[19] = new CustomVertex.PositionNormalTextured(-2f, 1f, 5f, (float)(15 / Math.Sqrt(350)), (float)(10 / Math.Sqrt(350)), (float)(-5 / Math.Sqrt(350)), 1f, 0f);
            vertices[20] = new CustomVertex.PositionNormalTextured(0f, -2f, 5f, (float)(15 / Math.Sqrt(350)), (float)(10 / Math.Sqrt(350)), (float)(-5 / Math.Sqrt(350)), 0f, 0f);
            vertices[21] = new CustomVertex.PositionNormalTextured(-1f, -3f, 0f, (float)(15 / Math.Sqrt(350)), (float)(10 / Math.Sqrt(350)), (float)(-5 / Math.Sqrt(350)), 0f, 1f);

            //четвертая
            vertices[22] = new CustomVertex.PositionNormalTextured(-1f, -3f, 0f, 0f, (float)(10 / Math.Sqrt(104)), (float)(-2 / Math.Sqrt(104)), 1f, 1f);
            vertices[23] = new CustomVertex.PositionNormalTextured(0f, -2f, 5f, 0f, (float)(10 / Math.Sqrt(104)), (float)(-2 / Math.Sqrt(104)), 1f, 0f);
            vertices[24] = new CustomVertex.PositionNormalTextured(2f, -2f, 5f, 0f, (float)(10 / Math.Sqrt(104)), (float)(-2 / Math.Sqrt(104)), 0f, 0f);
            vertices[25] = new CustomVertex.PositionNormalTextured(1f, -3f, 0f, 0f, (float)(10 / Math.Sqrt(104)), (float)(-2 / Math.Sqrt(104)), 0f, 1f);

            //пятая
            vertices[26] = new CustomVertex.PositionNormalTextured(1f, -3f, 0f, (float)(-15 / Math.Sqrt(326)), (float)(10 / Math.Sqrt(326)), (float)(1 / Math.Sqrt(326)), 1f, 1f);
            vertices[27] = new CustomVertex.PositionNormalTextured(2f, -2f, 5f, (float)(-15 / Math.Sqrt(326)), (float)(10 / Math.Sqrt(326)), (float)(1 / Math.Sqrt(326)), 1f, 0f);
            vertices[28] = new CustomVertex.PositionNormalTextured(4f, 1f, 5f, (float)(-15 / Math.Sqrt(326)), (float)(10 / Math.Sqrt(326)), (float)(1 / Math.Sqrt(326)), 0f, 0f);
            vertices[29] = new CustomVertex.PositionNormalTextured(3f, 0f, 0f, (float)(-15 / Math.Sqrt(326)), (float)(10 / Math.Sqrt(326)), (float)(1 / Math.Sqrt(326)), 0f, 1f);
            vb.SetData(vertices, 0, LockFlags.None);

            //индексы, показывающие, как из вершин составить треугольники
            ib = new IndexBuffer(typeof(int), 48, device, Usage.WriteOnly, Pool.Default);
            indices = new int[48];

            //дно
            indices[0] = 3;
            indices[1] = 2;
            indices[2] = 1;
            indices[3] = 4;
            indices[4] = 3;
            indices[5] = 1;
            indices[6] = 0;
            indices[7] = 4;
            indices[8] = 1;

            //крыша
            indices[9] = 8;
            indices[10] = 6;
            indices[11] = 7;
            indices[12] = 9;
            indices[13] = 6;
            indices[14] = 8;
            indices[15] = 5;
            indices[16] = 6;
            indices[17] = 9;

            //первая
            indices[18] = 13;
            indices[19] = 12;
            indices[20] = 10;
            indices[21] = 10;
            indices[22] = 12;
            indices[23] = 11;

            //вторая
            indices[24] = 17;
            indices[25] = 16;
            indices[26] = 14;
            indices[27] = 14;
            indices[28] = 16;
            indices[29] = 15;

            //третья
            indices[30] = 21;
            indices[31] = 20;
            indices[32] = 18;
            indices[33] = 18;
            indices[34] = 20;
            indices[35] = 19;

            //четвертая
            indices[36] = 25;
            indices[37] = 24;
            indices[38] = 22;
            indices[39] = 22;
            indices[40] = 24;
            indices[41] = 23;

            //пятая
            indices[42] = 29;
            indices[43] = 28;
            indices[44] = 26;
            indices[45] = 26;
            indices[46] = 28;
            indices[47] = 27;



            ib.SetData(indices, 0, LockFlags.None);


        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            device.Clear(ClearFlags.Target, Color.DarkSlateBlue, 1.0f, 0);

            device.BeginScene();
            device.VertexFormat = CustomVertex.PositionNormalTextured.Format;

            //установка вершин, индексов и текстуры
            device.SetStreamSource(0, vb, 0);
            device.Indices = ib;
            device.SetTexture(0, tex1);

            //материал показывает, как свет взаимодействует с поверхностью объекта
            Material M = new Material();
            M.Diffuse = Color.Pink;
            M.Emissive = Color.Purple;
            M.Ambient = Color.Moccasin;
            M.Specular = Color.WhiteSmoke;
            M.SpecularSharpness = 0.5f;
            device.Material = M;

            float x = Convert.ToSingle(10*2 / (Math.Sqrt(3 - Math.Sqrt(5))) * Math.Cos(angle));
            float y = Convert.ToSingle(20*2 / (Math.Sqrt(3 + Math.Sqrt(5))) * Math.Sin(angle));
            float z = 20;

            device.Transform.View = Matrix.LookAtLH(new Vector3(x, y, z),
                                        new Vector3(0, 0, 0),
                                        new Vector3(0, 1, 0));

            //рисуем треугольники с использованием буфера индексов
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 30, 0, 16);

            device.EndScene();

            device.Present();

            this.Invalidate();
            angle += 0.01f;
        }
    }
}
