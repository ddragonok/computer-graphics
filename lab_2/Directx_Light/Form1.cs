using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using Microsoft.VisualC;


namespace Directx_Light
{
    public partial class Form1 : Form
    {
        private Device device = null;
        private VertexBuffer vb = null;
        private float angle = 0f;
        private CustomVertex.PositionNormalColored[] vertices;
        private IndexBuffer ib = null;
        private int[] indices;

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

            InitializeDevice();
            VertexDeclaration();
            CameraPositioning();
        }

        public void InitializeDevice()
        {
            PresentParameters presentParams = new PresentParameters();
            presentParams.Windowed = true;
            presentParams.SwapEffect = SwapEffect.Discard;

            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

            //включаем режим освещения и обработку невидимых граней
            device.RenderState.Lighting = true;

            device.RenderState.CullMode = Cull.CounterClockwise;
        }

        public void CameraPositioning()
        {

            float x = Convert.ToSingle(10*2 / (Math.Sqrt(3 - Math.Sqrt(5))) * Math.Cos(angle));
            float y = Convert.ToSingle(20*2 / (Math.Sqrt(3 + Math.Sqrt(5))) * Math.Sin(angle));
            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, (float)this.Width / (float)this.Height, 1f, 50f);
            device.Transform.View = Matrix.LookAtLH(new Vector3(x, y, 20f),
                                        new Vector3(0, 0, 0),
                                        new Vector3(0, -1, 0));

            //включаем направленные источники света
            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = new Vector3(0f, 1f, -1f);
            device.Lights[0].Enabled = true;

        }

        public void VertexDeclaration()
        {
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 10, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);

            //вершины содержат координаты, нормаль и цвет
            vertices = new CustomVertex.PositionNormalColored[10];
            vertices[0] = new CustomVertex.PositionNormalColored(3f, 0f, 0f, 0f, 0f, -1f, Color.Cyan.ToArgb());
            vertices[1] = new CustomVertex.PositionNormalColored(0f, 3f, 0f, 0f, 0f, -1f, Color.Red.ToArgb());
            vertices[2] = new CustomVertex.PositionNormalColored(-3f, 0f, 0f, 0f, 0f, -1f, Color.Blue.ToArgb());
            vertices[3] = new CustomVertex.PositionNormalColored(-1f, -3f, 0f, 0f, 0f, -1f, Color.Magenta.ToArgb());
            vertices[4] = new CustomVertex.PositionNormalColored(1f, -3f, 0f, 0f, 0f, -1f, Color.Yellow.ToArgb());

            vertices[5] = new CustomVertex.PositionNormalColored(4f, 1f, 5f, 0f, 0f, 1f, Color.Green.ToArgb());
            vertices[6] = new CustomVertex.PositionNormalColored(1f, 4f, 5f, 0f, 0f, 1f, Color.Yellow.ToArgb());
            vertices[7] = new CustomVertex.PositionNormalColored(-2f, 1f, 5f, 0f, 0f, 1f, Color.Orange.ToArgb());
            vertices[8] = new CustomVertex.PositionNormalColored(0f, -2f, 5f, 0f, 0f, 1f, Color.Purple.ToArgb());
            vertices[9] = new CustomVertex.PositionNormalColored(2f, -2f, 5f, 0f, 0f, 1f, Color.Violet.ToArgb());

            vb.SetData(vertices, 0, LockFlags.None);

            //индексный буфер показывает, как вершины объединить в треугольники
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
            indices[18] = 1;
            indices[19] = 6;
            indices[20] = 0;
            indices[21] = 0;
            indices[22] = 6;
            indices[23] = 5;

            //вторая
            indices[24] = 0;
            indices[25] = 5;
            indices[26] = 4;
            indices[27] = 4;
            indices[28] = 5;
            indices[29] = 9;

            //третья
            indices[30] = 4;
            indices[31] = 9;
            indices[32] = 3;
            indices[33] = 3;
            indices[34] = 9;
            indices[35] = 8;

            //четвертая
            indices[36] = 3;
            indices[37] = 8;
            indices[38] = 2;
            indices[39] = 2;
            indices[40] = 8;
            indices[41] = 7;

            //пятая
            indices[42] = 2;
            indices[43] = 7;
            indices[44] = 1;
            indices[45] = 1;
            indices[46] = 7;
            indices[47] = 6;


            ib.SetData(indices, 0, LockFlags.None);

            ib = new IndexBuffer(typeof(int), indices.Length, device,
                     Usage.WriteOnly, Pool.Default);

            ib.SetData(indices, 0, LockFlags.None);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            device.Clear(ClearFlags.Target, Color.DarkSlateBlue, 1.0f, 0);

            device.BeginScene();
            device.VertexFormat = CustomVertex.PositionNormalColored.Format;

            //установка вершин и индексов, показывающих как из них построить поверхность
            device.SetStreamSource(0, vb, 0);
            device.Indices = ib;

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 10, 0, 16);

            float x = Convert.ToSingle(10*2 / (Math.Sqrt(3 - Math.Sqrt(5))) * Math.Cos(angle));
            float y = Convert.ToSingle(20*2 / (Math.Sqrt(3 + Math.Sqrt(5))) * Math.Sin(angle));
            float z = 20;

            device.Transform.View = Matrix.LookAtLH(new Vector3(x, y, z),
                                        new Vector3(0, 0, 0),
                                        new Vector3(0, 1, 0));
            //отрисовка индексированных фигур
            device.EndScene();

            device.Present();

            this.Invalidate();
            angle += 0.01f;
            if (angle >= 360f)
            {
                angle -= 360f;
            }
        }
    }
}
