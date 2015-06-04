using Arleen;
using Arleen.Game;
using Arleen.Geometry;
using Arleen.Rendering;
using Arleen.Rendering.Sources;
using OpenTK;
using System.Collections.Generic;
using System.Drawing;

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
            var viewingVolume = new ViewingVolume.Perspective {
                FieldOfView = 45,
                FarPlane = FLT_FarPlane,
                NearPlane = FLT_NearPlane,
                AspectRatio = 1
            };
            _camera1 = new Camera(viewingVolume);
            _camera2 = new Camera(viewingVolume);
            _textRenderer1 = Facade.Create<TextRenderer>(new Font("Verdana", 12, FontStyle.Regular), true);
            _textRenderer2 = Facade.Create<TextRenderer>(new Font("Verdana", 12, FontStyle.Regular), true);
            //---
            var scene = new Scene();
            var brickwall = Facade.Resources.LoadStream("brickwall.png");
            var sources = Facade.Create<AggregateRenderSource>((IList<RenderSource>)new RenderSource[] {
                Facade.Create<BackgroundColorRenderSource>(Color.LightSkyBlue, 1.0),
                Facade.Create<SkyboxRenderer>(Facade.Resources.LoadStream("skybox.png")),
                Facade.Create<BoxRenderer>(brickwall, new Location { Position = new Vector3d(0, 0, -5) }),
                Facade.Create<BoxRenderer>(brickwall, new Location { Position = new Vector3d(1.5, 0, -5) }, Transformation.Identity.Scale(2.0f)),
                Facade.Create<BoxRenderer>(brickwall, new Location { Position = new Vector3d(-2.5, 0, -5) }, Transformation.Identity.Scale(4.0f))
            });
            scene.RenderTargets.Add(new RenderTarget(new RectangleF(0.0f, 0.0f, 1.0f, 0.5f), _camera1, sources, _textRenderer1));
            scene.RenderTargets.Add(new RenderTarget(new RectangleF(0.0f, 0.5f, 1.0f, 0.5f), _camera2, sources, _textRenderer2));
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

        private void UpdateCamera(RenderInfo renderinfo, ILocable locable, Quaterniond rotationPerSecond, Vector3d translationPerSecond, TextRenderer textRenderer)
        {
            locable.Location.Orientation = QuaterniondHelper.Extrapolate(Quaterniond.Identity, rotationPerSecond, TotalTime);
            locable.Location.Position = translationPerSecond * TotalTime;
            //---
            double bearing, elevation, roll;
            QuaterniondHelper.ToEulerAngles(locable.Location.Orientation, out bearing, out elevation, out roll);
            var cameraInfo = "FPS: " + renderinfo.Fps + "\n" +
                             "x:" + locable.Location.Position.X + "\n" +
                             "y:" + locable.Location.Position.Y + "\n" +
                             "z:" + locable.Location.Position.Z + "\n" +
                             "Bearing: " + MathHelper.RadiansToDegrees(bearing).ToString("0.000") + "\n" +
                             "Elevation: " + MathHelper.RadiansToDegrees(elevation).ToString("0.000") + "\n" +
                             "Roll: " + MathHelper.RadiansToDegrees(roll).ToString("0.000") + "\n";
            textRenderer.Text = cameraInfo;
        }
    }
}