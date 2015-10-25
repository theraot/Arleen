using Arleen.Geometry;
using OpenTK.Graphics.OpenGL;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

namespace Arleen.Rendering.Sources
{
    public sealed class BoxRenderer : RenderSource, IDisposable, ILocable
    {
        private const float FLT_Height0 = 0.0f;
        private const float FLT_Height1 = 1.0f / 3.0f;
        private const float FLT_Height2 = FLT_Height1 * 2;
        private const float FLT_Length = 0.5f;
        private const float FLT_Width0 = 0.0f;
        private const float FLT_Width1 = 1.0f / 4.0f;
        private const float FLT_Width2 = FLT_Width1 * 2;
        private const float FLT_Width3 = FLT_Width1 * 3;

        private readonly Transformation _transformation;
        private Stream _stream;
        private Mesh _mesh;
        private Action _render;
        private Texture _texture;

        [Obsolete("Use the Create method instead")]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public BoxRenderer(Stream stream, Location location, Transformation transformation)
        {
            _stream = stream;
            Location = location;
            _transformation = transformation;
        }

        [Obsolete("Use the Create method instead")]
        [EditorBrowsableAttribute(EditorBrowsableState.Never)]
        public BoxRenderer(Stream stream, Location location)
        {
            _stream = stream;
            Location = location;
            _transformation = Transformation.Identity;
        }

        public Location Location { get; set; }

        public static BoxRenderer Create(Stream stream, Location location, Transformation transformation)
        {
            return Facade.Create<BoxRenderer>(stream, location, transformation);
        }

        public static BoxRenderer Create(Stream stream, Location location)
        {
            return Facade.Create<BoxRenderer>(stream, location);
        }

        public void Dispose()
        {
            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }
            if (_mesh != null)
            {
                _mesh.Dispose();
                _mesh = null;
            }
        }

        protected override void OnInitilaize()
        {
            if (GL.IsEnabled(EnableCap.DepthTest))
            {
                _render = Draw;
            }
            else
            {
                _render = () =>
                {
                    GL.Enable(EnableCap.DepthTest);
                    GL.Clear(ClearBufferMask.DepthBufferBit);
                    GL.Disable(EnableCap.DepthTest);
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
                // VEXTEX   // TEXTURES
                // 0
                A, A, A,    left.Right, left.Bottom,
                A, B, A,    left.Right, left.Top,
                // 2
                A, B, B,    left.Left, left.Top,
                A, A, B,    left.Left, left.Bottom,
                // 4
                B, A, A,    front.Right, front.Bottom,
                B, B, A,    front.Right, front.Top,
                // 6
                B, A, B,    right.Right, right.Bottom,
                B, B, B,    right.Right, right.Top,
                // 8
                A, A, B,    back.Right, back.Bottom,
                A, B, B,    back.Right, back.Top,
                // 10
                A, A, A,    down.Left, down.Top,
                A, A, B,    down.Left, down.Bottom,
                // 12
                B, A, B,    down.Right, down.Bottom,
                B, A, A,    down.Right, down.Top,
                // 14
                A, B, A,    up.Left, up.Bottom,
                B, B, A,    up.Right, up.Bottom,
                // 16
                B, B, B,    up.Right, up.Top,
                A, B, B,    up.Left, up.Top
            };

            var indexes = new byte[] {
                3, 2, 1, 0,
                0, 1, 5, 4,
                4, 5, 7, 6,
                6, 7, 9, 8,
                13, 12, 11, 10,
                17, 16, 15, 14
            };

            _mesh = new Mesh(Mesh.VextexInfo.Position | Mesh.VextexInfo.Texture, data, PrimitiveType.Quads, indexes);
        }

        private void Draw()
        {
            _transformation.Apply();
            _texture.Bind();
            _mesh.Render();
        }
    }
}