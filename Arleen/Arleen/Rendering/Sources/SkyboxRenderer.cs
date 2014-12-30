using Arleen.Geometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;

namespace Arleen.Rendering.Sources
{
    public class SkyboxRenderer : RenderSource
    {
        private const float FLT_height0 = 0.0f;
        private const float FLT_height1 = 1.0f / 3.0f;
        private const float FLT_height2 = FLT_height1 * 2;
        private const float FLT_Length = 2.0f;
        private const float FLT_width0 = 0.0f;
        private const float FLT_width1 = 1.0f / 4.0f;
        private const float FLT_width2 = FLT_width1 * 2;
        private const float FLT_width3 = FLT_width1 * 3;

        private int dataBuffer = -1;
        private int indexBuffer = -1;
        private Action<Camera> _render;

        private Texture _texture;

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

            const float a = -FLT_Length;
            const float b = FLT_Length;

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
            //    aab-----bab
            //   /|      /|
            //  aaa-----baa
            //  | |     | |
            //  | abb---|-bbb
            //  |/      |/
            //  aba-----bba
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