using Arleen.Geometry;
using Arleen.Rendering;
using Arleen.Rendering.Sources;
using Arleen.Rendering.Utility;
using OpenTK;
using OpenTK.Graphics.OpenGL;
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
                    new CustomRenderer
                        (
                        info =>
                        {
                            GL.Enable(EnableCap.Blend);
                            GL.BlendEquation(BlendEquationMode.FuncAdd);
                            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                            GL.Disable(EnableCap.DepthTest);
                            GL.LoadIdentity();
                            ViewingVolumeHelper.PlaceOthogonalProjection(info.ClipArea.Width, info.ClipArea.Height, FLT_NearPlane, FLT_FarPlane);

                            double bearing, elevation, roll;
                            QuaterniondHelper.ToEulerAngles(_camera.Location.Orientation, out bearing, out elevation, out roll);

                            Rendering.Utility.TextDrawer.Draw
                                (
                                    Program._["Hello, my name is {name}."].FormatWith(new { name = Program.DisplayName }) + "\n" +
                                    "FPS: " + info.Fps + "\n" +
                                    "x:" + _camera.Location.Position.X + "\n" +
                                    "y:" + _camera.Location.Position.Y + "\n" +
                                    "z:" + _camera.Location.Position.Z + "\n" +
                                    "Bearing: " + (MathHelper.RadiansToDegrees(bearing) % 360).ToString("0.000") + "\n" +
                                    "Elevation: " + (MathHelper.RadiansToDegrees(elevation) % 360).ToString("0.000") + "\n" +
                                    "Roll: " + (MathHelper.RadiansToDegrees(roll) % 360).ToString("0.000") + "\n",
                                    new Font("Verdana", 12, FontStyle.Regular),
                                    true,
                                    Color.White,
                                    info.ClipArea,
                                    TextAlign.Left,
                                    TextAlign.Top
                                );
                            GL.Enable(EnableCap.DepthTest);
                        }
                        )
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
        }
    }
}