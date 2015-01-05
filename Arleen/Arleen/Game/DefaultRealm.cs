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
        private Camera _camera;
        private TextRenderer _textRenderer;

        protected override void OnLoad(EventArgs e)
        {
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
                    _textRenderer = new TextRenderer(new Font("Verdana", 12, FontStyle.Regular), true)
                );
            _renderer.RenderTargets.Add
                (
                    new RenderTarget
                        (
                        new RectangleF(0, 0, 1.0f, 1.0f),
                        _camera
                        )
                );
            _renderer.Initialize(this);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            _camera.Location.Orientation *= QuaterniondHelper.CreateFromEulerAngles(bearing: 0.0000004, elevation: 0.0000002, roll: 0.0000001);
            //---
            double bearing, elevation, roll;
            QuaterniondHelper.ToEulerAngles(_camera.Location.Orientation, out bearing, out elevation, out roll);

            _textRenderer.Text = Program._["Hello, my name is {name}."].FormatWith(new { name = Program.DisplayName }) +
                                 "\n" +
                                 "FPS: " + _renderer.Fps + "\n" +
                                 "x:" + _camera.Location.Position.X + "\n" +
                                 "y:" + _camera.Location.Position.Y + "\n" +
                                 "z:" + _camera.Location.Position.Z + "\n" +
                                 "Bearing: " + (MathHelper.RadiansToDegrees(bearing) % 360).ToString("0.000") + "\n" +
                                 "Elevation: " + (MathHelper.RadiansToDegrees(elevation) % 360).ToString("0.000") + "\n" +
                                 "Roll: " + (MathHelper.RadiansToDegrees(roll) % 360).ToString("0.000") + "\n";
        }
    }
}