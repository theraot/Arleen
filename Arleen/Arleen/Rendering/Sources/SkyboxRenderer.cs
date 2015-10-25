using Arleen.Geometry;
using OpenTK.Graphics.OpenGL;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace Arleen.Rendering.Sources
{
    public sealed class SkyboxRenderer : RenderSource, IDisposable, ICameraRelative
    {
        private const float FLT_Height0 = 0.0f;
        private const float FLT_Height1 = 1.0f / 3.0f;
        private const float FLT_Height2 = FLT_Height1 * 2;
        private const float FLT_Length = 2.0f;
        private const float FLT_Width0 = 0.0f;
        private const float FLT_Width1 = 1.0f / 4.0f;
        private const float FLT_Width2 = FLT_Width1 * 2;
        private const float FLT_Width3 = FLT_Width1 * 3;

        private Stream _stream;
        private Action _render;
        private Texture _texture;
        private int _dataBuffer = -1;
        private int _indexBuffer = -1;

        [Obsolete("Use the Create method instead")]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public SkyboxRenderer(Stream stream)
        {
            _stream = stream;
        }

        public static SkyboxRenderer Create(Stream stream)
        {
            return Facade.Create<SkyboxRenderer>(stream);
        }

        public void Dispose()
        {
            _texture.Dispose();
            GL.DeleteBuffer(_dataBuffer);
            GL.DeleteBuffer(_indexBuffer);
        }

        protected override void OnInitilaize()
        {
            if (GL.IsEnabled(EnableCap.DepthTest))
            {
                _render = () =>
                {
                    GL.Disable(EnableCap.DepthTest);
                    Draw();
                    GL.Enable(EnableCap.DepthTest);
                    GL.Clear(ClearBufferMask.DepthBufferBit);
                };
            }
            else
            {
                _render = () =>
                {
                    GL.Clear(ClearBufferMask.DepthBufferBit);
                    Draw();
                };
            }
            Build();
            _texture = new Texture(_stream);
            _stream = null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CC0016:Copy Event To Variable Before Fire", Justification = "False Positive")]
        protected override void OnRender()
        {
            _render();
        }

        private void Build()
        {
            var back = new RectangleF(FLT_Width3, FLT_Height1, FLT_Width1, FLT_Height1);
            var up = new RectangleF(FLT_Width1, FLT_Height0, FLT_Width1, FLT_Height1);
            var front = new RectangleF(FLT_Width1, FLT_Height1, FLT_Width1, FLT_Height1);
            var left = new RectangleF(FLT_Width0, FLT_Height1, FLT_Width1, FLT_Height1);
            var right = new RectangleF(FLT_Width2, FLT_Height1, FLT_Width1, FLT_Height1);
            var down = new RectangleF(FLT_Width1, FLT_Height2, FLT_Width1, FLT_Height1);

            const float A = -FLT_Length;
            const float B = FLT_Length;

            /*    3-------6
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
            */

            var data = new[] {
                // VEXTEX
                // 0
                A, A, A,
                A, B, A,
                // 2
                A, B, B,
                A, A, B,
                // 4
                B, A, A,
                B, B, A,
                // 6
                B, A, B,
                B, B, B,
                // 8
                A, A, B,
                A, B, B,
                // 10
                A, A, A,
                A, A, B,
                // 12
                B, A, B,
                B, A, A,
                // 14
                A, B, A,
                B, B, A,
                // 16
                B, B, B,
                A, B, B,

                // TEXTURES
                // 0
                left.Right, left.Bottom,
                left.Right, left.Top,
                // 2
                left.Left, left.Top,
                left.Left, left.Bottom,
                // 4
                front.Right, front.Bottom,
                front.Right, front.Top,
                // 6
                right.Right, right.Bottom,
                right.Right, right.Top,
                // 8
                back.Right, back.Bottom,
                back.Right, back.Top,
                // 10
                down.Left, down.Top,
                down.Left, down.Bottom,
                // 12
                down.Right, down.Bottom,
                down.Right, down.Top,
                // 14
                up.Left, up.Bottom,
                up.Right, up.Bottom,
                // 16
                up.Right, up.Top,
                up.Left, up.Top
            };

            var indexes = new byte[] {
                0, 1, 2, 3,
                4, 5, 1, 0,
                6, 7, 5, 4,
                8, 9, 7, 6,
                10, 11, 12, 13,
                14, 15, 16, 17
            };

            GL.Arb.GenBuffers(1, out _dataBuffer);
            GL.Arb.BindBuffer(BufferTargetArb.ArrayBuffer, _dataBuffer);
            GL.Arb.BufferData(BufferTargetArb.ArrayBuffer, (IntPtr)(data.Length * sizeof(float)), data, BufferUsageArb.StaticDraw);

            GL.Arb.GenBuffers(1, out _indexBuffer);
            GL.Arb.BindBuffer(BufferTargetArb.ElementArrayBuffer, _indexBuffer);
            GL.Arb.BufferData(BufferTargetArb.ElementArrayBuffer, (IntPtr)(indexes.Length * sizeof(byte)), indexes, BufferUsageArb.StaticDraw);
        }

        private void Draw()
        {
            RenderTarget.Current.Camera.Location.PlaceInverted(Location.Mode.OrientationOnly);
            _texture.Bind();
            GL.Arb.BindBuffer(BufferTargetArb.ArrayBuffer, _dataBuffer);
            GL.VertexPointer(3, VertexPointerType.Float, 0, new IntPtr(0));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, new IntPtr(sizeof(float) * 3 * 18));
            GL.Arb.BindBuffer(BufferTargetArb.ElementArrayBuffer, _indexBuffer);
            GL.DrawElements(PrimitiveType.Quads, 24, DrawElementsType.UnsignedByte, new IntPtr(0));
        }
    }
}