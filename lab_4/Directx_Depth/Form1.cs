using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace Directx_Depth
{
    public partial class Form1 : Form
    {
        private Device device = null;
        private VertexBuffer vb = null;
        private float angle = 0f;
        private CustomVertex.PositionNormalColored[] vertices;
        private IndexBuffer ib = null;
        private int[] indices;
        private int[] indices1;

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
            presentParams.EnableAutoDepthStencil = true;
            presentParams.AutoDepthStencilFormat = DepthFormat.D16;
            device = new Device(0, DeviceType.Hardware, this, CreateFlags.SoftwareVertexProcessing, presentParams);

            device.RenderState.Lighting = true;

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

            device.Lights[0].Type = LightType.Directional;
            device.Lights[0].Diffuse = Color.White;
            device.Lights[0].Direction = new Vector3(0f, 1f, -1f);
            device.Lights[0].Enabled = true;
        }

        public void VertexDeclaration()
        {

            vertices = new CustomVertex.PositionNormalColored[20];

            vb = new VertexBuffer(typeof(CustomVertex.PositionNormalColored), 20, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalColored.Format, Pool.Default);

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

            indices = new int[]{
            //нижняя грань
            3, 2, 1,
            4, 3, 1,
            0, 4, 1,
            //верхняя
            8, 6, 7,
            9, 6, 8,
            5, 6, 9,
            //первая
            1, 6, 0,
            0, 6, 5,
            //вторая
            0, 5, 4,
            4, 5, 9,
            //третья
            4, 9, 3,
            3, 9, 8,
            //четвертая
            3, 8, 2,
            2, 8, 7,
            //пятая
            2, 7, 1,
            1, 7, 6
            };

            for (int i = 10; i < 20; i++)
            {
                vertices[i] = new CustomVertex.PositionNormalColored(vertices[i - 10].X + 3, vertices[i - 10].Y , vertices[i - 10].Z + 3, vertices[i - 10].Nx, vertices[i - 10].Ny, vertices[i - 10].Nz, vertices[i - 10].Color);
            }

            indices1 = new int[96];
            for (int i = 0; i < 48; i++) indices1[i] = indices[i];
            for (int i = 48; i < 96; i++) indices1[i] = indices[i - 48] + 10;

            ib = new IndexBuffer(typeof(int), indices1.Length, device,
                     Usage.WriteOnly, Pool.Default);

            ib.SetData(indices1, 0, LockFlags.None);

            vb.SetData(vertices, 0, LockFlags.None);
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
            device.RenderState.StencilEnable = true;
            device.Clear(ClearFlags.Target, Color.DarkSlateBlue, 1.0f, 0);

            device.BeginScene();
            device.VertexFormat = CustomVertex.PositionNormalColored.Format;
            device.SetStreamSource(0, vb, 0);

            Material M = new Material();
            M.Diffuse = Color.Pink;
            M.Emissive = Color.Purple;
            M.Ambient = Color.Moccasin;
            M.Specular = Color.WhiteSmoke;
            M.SpecularSharpness = 0.5f;
            device.Material = M;

            device.Indices = ib;

            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 20, 0, 32);

            float x = Convert.ToSingle(10*2 / (Math.Sqrt(3 - Math.Sqrt(5))) * Math.Cos(angle));
            float y = Convert.ToSingle(20*2 / (Math.Sqrt(3 + Math.Sqrt(5))) * Math.Sin(angle));
            float z = 20;
            
            device.Transform.View = Matrix.LookAtLH(new Vector3(x, y, z),
                                        new Vector3(0, 0, 0),
                                        new Vector3(0, 1, 0));

            device.EndScene();

            device.Present();

            this.Invalidate();
            angle += 0.01f;
        }
    }
}
