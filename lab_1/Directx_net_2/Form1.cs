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

            device.Transform.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4, (float)this.Width / (float)this.Height, 1f, 50f);
            device.Transform.View = Matrix.LookAtLH(new Vector3(0f, 0f, 20f),
                                        new Vector3(-1, -1, -1),
                                        new Vector3(0, 1, 0));

            //включаем направленные источники света

            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Position = new Vector3(0f, 0f, -5f);
            device.Lights[0].Direction = new Vector3(0f, 0f, -7f);
            device.Lights[0].Enabled = true;
        }

        public void VertexDeclaration()
        {
            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 5, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);

            //вершины содержат координаты, нормаль и цвет
            vertices = new CustomVertex.PositionNormalColored[5];
            vertices[0] = new CustomVertex.PositionNormalColored(3f, 0f, 0f, 0f, 0f, 1f, Color.Cyan.ToArgb());
            vertices[1] = new CustomVertex.PositionNormalColored(0f, 3f, 0f, 0f, 0f, 1f, Color.Red.ToArgb());
            vertices[2] = new CustomVertex.PositionNormalColored(-3f, 0f, 0f, 0f, 0f, 1f, Color.Blue.ToArgb());
            vertices[3] = new CustomVertex.PositionNormalColored(-1f, -3f, 0f, 0f, 0f, 1f, Color.Magenta.ToArgb());
            vertices[4] = new CustomVertex.PositionNormalColored(1f, -3f, 0f, 0f, 0f, 1f, Color.Yellow.ToArgb());

            vb.SetData(vertices, 0, LockFlags.None);

            //индексный буфер показывает, как вершины объединить в треугольники
            ib = new IndexBuffer(typeof(int), 9, device, Usage.WriteOnly, Pool.Default);
            indices = new int[9];

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 4;
            indices[4] = 0;
            indices[5] = 2;
            indices[6] = 3;
            indices[7] = 4;
            indices[8] = 2;
            
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

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 5, 0, 3);

            device.EndScene();

            device.Present();

            this.Invalidate();
            angle += 0.01f;
        }
    }
}
