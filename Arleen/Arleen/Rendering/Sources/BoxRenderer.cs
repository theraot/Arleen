using Arleen.Geometry;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace Arleen.Rendering.Sources
{
    public sealed class BoxRenderer : RenderSource, IDisposable
    {
        private const float FLT_height0 = 0.0f;
        private const float FLT_height1 = 1.0f / 3.0f;
        private const float FLT_height2 = FLT_height1 * 2;
        private const float FLT_Length = 0.5f;
        private const float FLT_width0 = 0.0f;
        private const float FLT_width1 = 1.0f / 4.0f;
        private const float FLT_width2 = FLT_width1 * 2;
        private const float FLT_width3 = FLT_width1 * 3;

        private readonly Location _location;
        private readonly float _size;
        private Bitmap _bitmap;
        private Action<Camera> _render;
        private Texture _texture;
        private int dataBuffer = -1;
        private int indexBuffer = -1;

        public BoxRenderer(Bitmap bitmap, Location location, float size)
        {
            _bitmap = bitmap;
            _location = location;
            _size = size;
        }

        public BoxRenderer(Bitmap bitmap, Location location)
        {
            _bitmap = bitmap;
            _location = location;
            _size = 1.0f;
        }

        public void Dispose()
        {
            _texture.Dispose();
            GL.DeleteBuffer(dataBuffer);
            GL.DeleteBuffer(indexBuffer);
        }

        protected override void OnInitilaize()
        {
            if (GL.IsEnabled(EnableCap.DepthTest))
            {
                _render = Draw;
            }
            else
            {
                _render = camera =>
                {
                    GL.Enable(EnableCap.DepthTest);
                    GL.Clear(ClearBufferMask.DepthBufferBit);
                    GL.Disable(EnableCap.DepthTest);
                    Draw(camera);
                };
            }
            Build();
            _texture = new Texture(_bitmap);
            _bitmap = null;
        }

        protected override void OnRender(RenderInfo info)
        {
            _render(info.Camera);
        }

        private void Build()
        {
            var back = new RectangleF(FLT_width3, FLT_height1, FLT_width1, FLT_height1);
            var up = new RectangleF(FLT_width1, FLT_height0, FLT_width1, FLT_height1);
            var front = new RectangleF(FLT_width1, FLT_height1, FLT_width1, FLT_height1);
            var left = new RectangleF(FLT_width0, FLT_height1, FLT_width1, FLT_height1);
            var right = new RectangleF(FLT_width2, FLT_height1, FLT_width1, FLT_height1);
            var down = new RectangleF(FLT_width1, FLT_height2, FLT_width1, FLT_height1);

            float a = -FLT_Length * _size;
            float b = FLT_Length * _size;

            //    3-------6
            //   /|      /|
            //  0-------4 |
            //  | |     | |
            //  | 2-----|-7
            //  |/      |/
            //  1-------5
            //
            //    8-------?
            //   /|      /|
            //  ?-------? |
            //  | |     | |
            //  | 9-----|-?
            //  |/      |/
            //  ?-------?
            //
            //    11------12
            //   /       /
            //  10------13
            //
            //    17------16
            //   /       /
            //  14------15
            //
            //    __!
            //   /
            //  0----!__
            //  |
            //  |
            //  _!_
            //
            //    AAB-----BAB
            //   /|      /|
            //  AAA-----BAA
            //  | |     | |
            //  | ABB---|-BBB
            //  |/      |/
            //  ABA-----BBA
            //
            //  _!_
            //  |
            //  | __!
            //  |/
            //  0----!__

            var _data = new[]
            {
                //VEXTEX
                //0
                a, a, a,
                a, b, a,
                //2
                a, b, b,
                a, a, b,
                //4
                b, a, a,
                b, b, a,
                //6
                b, a, b,
                b, b, b,
                //8
                a, a, b,
                a, b, b,
                //10
                a, a, a,
                a, a, b,
                //12
                b, a, b,
                b, a, a,
                //14
                a, b, a,
                b, b, a,
                //16
                b, b, b,
                a, b, b,

                //TEXTURES
                //0
                left.Right, left.Bottom,
                left.Right, left.Top,
                //2
                left.Left, left.Top,
                left.Left, left.Bottom,
                //4
                front.Right, front.Bottom,
                front.Right, front.Top,
                //6
                right.Right, right.Bottom,
                right.Right, right.Top,
                //8
                back.Right, back.Bottom,
                back.Right, back.Top,
                //10
                down.Left, down.Top,
                down.Left, down.Bottom,
                //12
                down.Right, down.Bottom,
                down.Right, down.Top,
                //14
                up.Left, up.Bottom,
                up.Right, up.Bottom,
                //16
                up.Right, up.Top,
                up.Left, up.Top
            };

            var _indexes = new byte[]
            {
                3, 2, 1, 0,
                0, 1, 5, 4,
                4, 5, 7, 6,
                6, 7, 9, 8,
                13, 12, 11, 10,
                17, 16, 15, 14
            };

            GL.Arb.GenBuffers(1, out dataBuffer);
            GL.Arb.BindBuffer(BufferTargetArb.ArrayBuffer, dataBuffer);
            GL.Arb.BufferData(BufferTargetArb.ArrayBuffer, (IntPtr)(_data.Length * sizeof(float)), _data, BufferUsageArb.StaticDraw);

            GL.Arb.GenBuffers(1, out indexBuffer);
            GL.Arb.BindBuffer(BufferTargetArb.ElementArrayBuffer, indexBuffer);
            GL.Arb.BufferData(BufferTargetArb.ElementArrayBuffer, (IntPtr)(_indexes.Length * sizeof(byte)), _indexes, BufferUsageArb.StaticDraw);
        }

        private void Draw(Camera camera)
        {
            camera.Place(Location.PlaceMode.Full);
            _location.Apply(Location.PlaceMode.Full);
            _texture.Bind();
            GL.Arb.BindBuffer(BufferTargetArb.ArrayBuffer, dataBuffer);
            GL.VertexPointer(3, VertexPointerType.Float, 0, new IntPtr(0));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, new IntPtr(sizeof(float) * 3 * 18));
            GL.Arb.BindBuffer(BufferTargetArb.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(BeginMode.Quads, 24, DrawElementsType.UnsignedByte, new IntPtr(0));
        }
    }
}