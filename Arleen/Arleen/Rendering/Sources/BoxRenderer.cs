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
        private readonly Transformation _transformation;
        private Bitmap _bitmap;
        private Mesh _mesh;
        private Action<Camera> _render;
        private Texture _texture;

        public BoxRenderer(Bitmap bitmap, Location location, Transformation transformation)
        {
            _bitmap = bitmap;
            _location = location;
            _transformation = transformation;
        }

        public BoxRenderer(Bitmap bitmap, Location location)
        {
            _bitmap = bitmap;
            _location = location;
            _transformation = Transformation.Identity;
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
                //VEXTEX    //TEXTURES
                //0
                A, A, A,    left.Right, left.Bottom,
                A, B, A,    left.Right, left.Top,
                //2
                A, B, B,    left.Left, left.Top,
                A, A, B,    left.Left, left.Bottom,
                //4
                B, A, A,    front.Right, front.Bottom,
                B, B, A,    front.Right, front.Top,
                //6
                B, A, B,    right.Right, right.Bottom,
                B, B, B,    right.Right, right.Top,
                //8
                A, A, B,    back.Right, back.Bottom,
                A, B, B,    back.Right, back.Top,
                //10
                A, A, A,    down.Left, down.Top,
                A, A, B,    down.Left, down.Bottom,
                //12
                B, A, B,    down.Right, down.Bottom,
                B, A, A,    down.Right, down.Top,
                //14
                A, B, A,    up.Left, up.Bottom,
                B, B, A,    up.Right, up.Bottom,
                //16
                B, B, B,    up.Right, up.Top,
                A, B, B,    up.Left, up.Top
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

            _mesh = new Mesh(Mesh.VextexInfo.Position | Mesh.VextexInfo.Texture, _data, BeginMode.Quads, _indexes);
        }

        private void Draw(Camera camera)
        {
            camera.Place(Location.Mode.All);
            _location.Apply(Location.Mode.All);
            _transformation.Apply();
            _texture.Bind();
            _mesh.Render();
        }
    }
}