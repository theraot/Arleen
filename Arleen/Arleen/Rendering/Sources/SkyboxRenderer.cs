using Arleen.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace Arleen.Rendering.Sources
{
    public sealed class SkyboxRenderer : RenderSource, IDisposable
    {
        private const float FLT_height0 = 0.0f;
        private const float FLT_height1 = 1.0f / 3.0f;
        private const float FLT_height2 = FLT_height1 * 2;
        private const float FLT_Length = 2.0f;
        private const float FLT_width0 = 0.0f;
        private const float FLT_width1 = 1.0f / 4.0f;
        private const float FLT_width2 = FLT_width1 * 2;
        private const float FLT_width3 = FLT_width1 * 3;

        private Action<Camera> _render;
        private Texture _texture;
        private int dataBuffer = -1;
        private int indexBuffer = -1;

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
                _render = camera =>
                {
                    GL.Disable(EnableCap.DepthTest);
                    Draw(camera);
                    GL.Enable(EnableCap.DepthTest);
                    GL.Clear(ClearBufferMask.DepthBufferBit);
                };
            }
            else
            {
                _render = camera =>
                {
                    GL.Clear(ClearBufferMask.DepthBufferBit);
                    Draw(camera);
                };
            }
            Build();
            var bitmap = Resources.LoadBitmap("skybox.png");
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            _texture = new Texture(bitmap);
        }

        protected override void OnRender(RenderInfo info)
        {
            _render(info.Camera);
        }

        private void Build()
        {
            var back = new RectangleF(FLT_width3, FLT_height1, FLT_width1, FLT_height1);
            var up = new RectangleF(FLT_width1, FLT_height2, FLT_width1, FLT_height1);
            var front = new RectangleF(FLT_width1, FLT_height1, FLT_width1, FLT_height1);
            var left = new RectangleF(FLT_width0, FLT_height1, FLT_width1, FLT_height1);
            var right = new RectangleF(FLT_width2, FLT_height1, FLT_width1, FLT_height1);
            var down = new RectangleF(FLT_width1, FLT_height0, FLT_width1, FLT_height1);

            const float A = -FLT_Length;
            const float B = FLT_Length;

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
                A, A, A,
                A, B, A,
                //2
                A, B, B,
                A, A, B,
                //4
                B, A, A,
                B, B, A,
                //6
                B, A, B,
                B, B, B,
                //8
                A, A, B,
                A, B, B,
                //10
                A, A, A,
                A, A, B,
                //12
                B, A, B,
                B, A, A,
                //14
                A, B, A,
                B, B, A,
                //16
                B, B, B,
                A, B, B,

                //TEXTURES
                //0
                left.Right, left.Top,
                left.Right, left.Bottom,
                //2
                left.Left, left.Bottom,
                left.Left, left.Top,
                //4
                front.Right, front.Top,
                front.Right, front.Bottom,
                //6
                right.Right, right.Top,
                right.Right, right.Bottom,
                //8
                back.Right, back.Top,
                back.Right, back.Bottom,
                //10
                down.Left, down.Bottom,
                down.Left, down.Top,
                //12
                down.Right, down.Top,
                down.Right, down.Bottom,
                //14
                up.Left, up.Top,
                up.Right, up.Top,
                //16
                up.Right, up.Bottom,
                up.Left, up.Bottom
            };

            var _indexes = new byte[]
            {
                0, 1, 2, 3,
                4, 5, 1, 0,
                6, 7, 5, 4,
                8, 9, 7, 6,
                10, 11, 12, 13,
                14, 15, 16, 17
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
            Matrix4d matrix = Matrix4d.Identity;
            camera.Location.Apply(matrix, Location.PlaceMode.OrientationOnly);
            GL.LoadMatrix(ref matrix);
            _texture.Bind();
            GL.Arb.BindBuffer(BufferTargetArb.ArrayBuffer, dataBuffer);
            GL.VertexPointer(3, VertexPointerType.Float, 0, new IntPtr(0));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, new IntPtr(sizeof(float) * 3 * 18));
            GL.Arb.BindBuffer(BufferTargetArb.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(BeginMode.Quads, 24, DrawElementsType.UnsignedByte, new IntPtr(0));
        }
    }
}