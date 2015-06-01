using Arleen.Geometry;
using Arleen.Rendering;
using Arleen.Rendering.Sources;
using OpenTK;
using System;
using System.Drawing;
using Arleen.Game;
using Arleen;

namespace Experior
{
    public class DefaultRealm : Realm
    {
        private const float FLT_FarPlane = 1000.0f;
        private const float FLT_NearPlane = 0.01f;
        private Camera _camera1;
        private Camera _camera2;
        private TextRenderer _textRenderer1;
        private TextRenderer _textRenderer2;

        protected override Scene Load()
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
            var scene = new Scene();
            var brickwall = Resources.Instance.LoadStream("brickwall.png");
            var sources = new AggregateRenderSource
                (
                    new IRenderable[]
                    {
                        new BackgroundColorRenderSource(Color.LightSkyBlue, 1.0),
						new SkyboxRenderer(Resources.Instance.LoadStream("skybox.png")),
                        new BoxRenderer(brickwall, new Location { Position = new Vector3d(0, 0, -5) }),
                        new BoxRenderer(brickwall, new Location { Position = new Vector3d(1.5, 0, -5) }, Transformation.Identity.Scale(2.0f)),
                        new BoxRenderer(brickwall, new Location { Position = new Vector3d(-2.5, 0, -5) }, Transformation.Identity.Scale(4.0f))
                    }
                );
            scene.RenderTargets.Add
                (
                    new RenderTarget
                        (
                            new RectangleF(0.0f, 0.0f, 1.0f, 0.5f),
                            _camera1,
                            new AggregateRenderSource
                            (
                                new IRenderable[]
                                {
                                    sources,
                                    _textRenderer1
                                }
                            )
                        )
                );
            scene.RenderTargets.Add
                (
                    new RenderTarget
                        (
                            new RectangleF(0.0f, 0.5f, 1.0f, 0.5f),
                            _camera2,
                            new AggregateRenderSource
                            (
                                new IRenderable[]
                                {
                                    sources,
                                    _textRenderer2
                                }
                            )
                        )
                );
            return scene;
        }

        protected override void UpdateFrame(RenderInfo renderInfo)
        {
            UpdateCamera
                (
                    renderInfo,
                    _camera1,
                    QuaterniondHelper.CreateFromEulerAngles(0.004, 0.002, 0.001),
                    new Vector3d(0, 0, -0.001),
                    _textRenderer1
                );
            UpdateCamera
                (
                    renderInfo,
                    _camera2,
                    QuaterniondHelper.CreateFromEulerAngles(-0.004, 0.002, 0.001),
                    new Vector3d(0, 0, -0.001),
                    _textRenderer2
                );
        }

        private void UpdateCamera(RenderInfo renderinfo, Camera camera, Quaterniond rotationPerSecond, Vector3d translationPerSecond, TextRenderer textRenderer)
        {
            camera.Location.Orientation = QuaterniondHelper.Extrapolate(Quaterniond.Identity, rotationPerSecond, TotalTime);
            camera.Location.Position = translationPerSecond * TotalTime;
            //---
            double bearing, elevation, roll;
            QuaterniondHelper.ToEulerAngles(camera.Location.Orientation, out bearing, out elevation, out roll);
            var cameraInfo = "FPS: " + renderinfo.Fps + "\n" +
                             "x:" + camera.Location.Position.X + "\n" +
                             "y:" + camera.Location.Position.Y + "\n" +
                             "z:" + camera.Location.Position.Z + "\n" +
                             "Bearing: " + MathHelper.RadiansToDegrees(bearing).ToString("0.000") + "\n" +
                             "Elevation: " + MathHelper.RadiansToDegrees(elevation).ToString("0.000") + "\n" +
                             "Roll: " + MathHelper.RadiansToDegrees(roll).ToString("0.000") + "\n";
            textRenderer.Text = cameraInfo;
        }
    }
}