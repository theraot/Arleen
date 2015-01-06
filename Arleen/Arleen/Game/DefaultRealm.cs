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
        private Camera _camera;
        private double _last_time;
        private Renderer _renderer;
        private TextRenderer _textRenderer;

        protected override void OnLoad(EventArgs e)
        {
            _last_time = TotalTime;
            _camera = new Camera
                (
                    new ViewingVolume.Perspective
                    {
                        FieldOfView = 45,
                        FarPlane = FLT_FarPlane,
                        NearPlane = FLT_NearPlane,
                        AspectRatio = 1
                    }
                );
            _renderer = new Renderer();
            _renderer.RenderSources.Add
                (
                    new BackgroundColorRenderSource(Color.LightSkyBlue, 1.0)
                );
            _renderer.RenderSources.Add
                (
                    new SkyboxRenderer(Resources.LoadBitmap("skybox.png"))
                );
            _renderer.RenderSources.Add
                (
                    new BoxRenderer(Resources.LoadBitmap("brickwall.png"), new Location { Position = new Vector3d(0, 0, -5) })
                );
            _renderer.RenderSources.Add
                (
                    new BoxRenderer(Resources.LoadBitmap("brickwall.png"), new Location { Position = new Vector3d(1.5, 0, -5) }, 2.0f)
                );
            _renderer.RenderSources.Add
                (
                    new BoxRenderer(Resources.LoadBitmap("brickwall.png"), new Location { Position = new Vector3d(-2.5, 0, -5) }, 4.0f)
                );
            _renderer.RenderSources.Add
                (
                    _textRenderer = new TextRenderer(new Font("Verdana", 12, FontStyle.Regular), true)
                );
            _renderer.RenderTargets.Add
                (
                    new RenderTarget
                        (
                            new RectangleF(0, 0, 1.0f, 0.5f),
                            _camera
                        )
                );
            _renderer.RenderTargets.Add
                (
                    new RenderTarget
                        (
                            new RectangleF(0, 0.5f, 1.0f, 0.5f),
                            _camera
                        )
                );
            _renderer.Initialize(this);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var rotationPerSecond = QuaterniondHelper.CreateFromEulerAngles(bearing: 0.004, elevation: 0.002, roll: 0.001);
            _camera.Location.Orientation = QuaterniondHelper.Extrapolate(Quaterniond.Identity, rotationPerSecond, TotalTime);
            //---
            double bearing, elevation, roll;
            QuaterniondHelper.ToEulerAngles(_camera.Location.Orientation, out bearing, out elevation, out roll);
            var cameraInfo = "FPS: " + _renderer.Fps + "\n" +
                             "x:" + _camera.Location.Position.X + "\n" +
                             "y:" + _camera.Location.Position.Y + "\n" +
                             "z:" + _camera.Location.Position.Z + "\n" +
                             "Bearing: " + MathHelper.RadiansToDegrees(bearing).ToString("0.000") + "\n" +
                             "Elevation: " + MathHelper.RadiansToDegrees(elevation).ToString("0.000") + "\n" +
                             "Roll: " +
                             MathHelper.RadiansToDegrees(roll).ToString("0.000") + "\n";
            _textRenderer.Text = cameraInfo;
        }
    }
}