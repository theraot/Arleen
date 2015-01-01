using Arleen.Rendering;
using Arleen.Rendering.Sources;
using Arleen.Rendering.Utility;
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

        protected override void OnLoad(EventArgs e)
        {
            var _camera = new Camera
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
                            ViewingVolumeHelper.PlaceOthogonalProjection(info.ClipArea.Width, info.ClipArea.Height,
                                FLT_NearPlane, FLT_FarPlane);
                            Rendering.Utility.TextDrawer.Draw
                                (
                                    Program._["Hello, my name is {name}."].FormatWith(new { name = Program.DisplayName }) + "\n" +
                                    "FPS:" + info.Fps,
                                    new Font("Verdana", 12, FontStyle.Regular),
                                    true,
                                    Color.White,
                                    info.ClipArea,
                                    TextAlign.Left,
                                    TextAlign.Top
                                );
                            GL.LoadIdentity();
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
    }
}