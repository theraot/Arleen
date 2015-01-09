using Arleen.Geometry;
using Arleen.Rendering;
using Arleen.Rendering.Sources;
using OpenTK;
using System;
using System.Drawing;

namespace Arleen.Game
{
    public class DefaultRealm : Realm
    {
        private const float FLT_FarPlane = 1000.0f;
        private const float FLT_NearPlane = 0.01f;
        private Renderer _renderer;
        private Camera _camera1;
        private Camera _camera2;
        private TextRenderer _textRenderer1;
        private TextRenderer _textRenderer2;

        protected override void OnLoad(EventArgs e)
        {
            var viewingVolume = new ViewingVolume.Perspective
            {
                FieldOfView = 45,
                FarPlane = FLT_FarPlane,
                NearPlane = FLT_NearPlane,
                AspectRatio = 1
            };
            _camera1 = new Camera(viewingVolume);
            _camera2 = new Camera(viewingVolume);
            _textRenderer1 = new TextRenderer(new Font("Verdana", 12, FontStyle.Regular), true);
            _textRenderer2 = new TextRenderer(new Font("Verdana", 12, FontStyle.Regular), true);
            //---
            _renderer = new Renderer();
            var brickwall = Resources.LoadBitmap("brickwall.png");
            var sources = new AggregateRenderSource
                (
                    new RenderSource[]
                    {
                        new BackgroundColorRenderSource(Color.LightSkyBlue, 1.0),
                        new SkyboxRenderer(Resources.LoadBitmap("skybox.png")),
                        new BoxRenderer(brickwall, new Location { Position = new Vector3d(0, 0, -5) }),
                        new BoxRenderer(brickwall, new Location { Position = new Vector3d(1.5, 0, -5) }, Transformation.Identity.Scale(2.0f)),
                        new BoxRenderer(brickwall, new Location { Position = new Vector3d(-2.5, 0, -5) }, Transformation.Identity.Scale(4.0f))
                    }
                );
            _renderer.RenderTargets.Add
                (
                    new RenderTarget
                        (
                            new RectangleF(0, 0, 1.0f, 0.5f),
                            _camera1,
                            new AggregateRenderSource
                            (
                                new RenderSource[]
                                {
                                    sources,
                                    _textRenderer1
                                }
                            )
                        )
                );
            _renderer.RenderTargets.Add
                (
                    new RenderTarget
                        (
                            new RectangleF(0, 0.5f, 1.0f, 0.5f),
                            _camera2,
                            new AggregateRenderSource
                            (
                                new RenderSource[]
                                {
                                    sources,
                                    _textRenderer1
                                }
                            )
                        )
                );
            _renderer.Initialize(this);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            UpdateCamera
                (
                    _camera1,
                    QuaterniondHelper.CreateFromEulerAngles(0.004, 0.002, 0.001),
                    _textRenderer1
                );
            UpdateCamera
                (
                    _camera2,
                    QuaterniondHelper.CreateFromEulerAngles(-0.004, 0.002, 0.001),
                    _textRenderer2
                );
        }

        private void UpdateCamera(Camera camera, Quaterniond rotationPerSecond, TextRenderer textRenderer)
        {
            camera.Location.Orientation = QuaterniondHelper.Extrapolate(Quaterniond.Identity, rotationPerSecond, TotalTime);
            //---
            double bearing, elevation, roll;
            QuaterniondHelper.ToEulerAngles(camera.Location.Orientation, out bearing, out elevation, out roll);
            var cameraInfo = "FPS: " + _renderer.Fps + "\n" +
                             "x:" + camera.Location.Position.X + "\n" +
                             "y:" + camera.Location.Position.Y + "\n" +
                             "z:" + camera.Location.Position.Z + "\n" +
                             "Bearing: " + MathHelper.RadiansToDegrees(bearing).ToString("0.000") + "\n" +
                             "Elevation: " + MathHelper.RadiansToDegrees(elevation).ToString("0.000") + "\n" +
                             "Roll: " +
                             MathHelper.RadiansToDegrees(roll).ToString("0.000") + "\n";
            textRenderer.Text = cameraInfo;
        }
    }
}