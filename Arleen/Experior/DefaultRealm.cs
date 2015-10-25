using System.Collections.Generic;
using Arleen;
using Arleen.Game;
using Arleen.Geometry;
using Arleen.Rendering;
using Arleen.Rendering.Sources;
using OpenTK;
using System.Drawing;

namespace Experior
{
    public class DefaultRealm : Realm
    {
        private const float FLT_FarPlane = 1000.0f;
        private const float FLT_NearPlane = 0.01f;
        private Font _font;
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
            _font = new Font("Verdana", 12, FontStyle.Regular);
            _textRenderer1 = TextRenderer.Create(string.Empty, _font, true);
            _textRenderer2 = TextRenderer.Create(string.Empty, _font, true);
            //---
            var scene = new Scene();
            var renderSources = new List<RenderSource>
            {
                BackgroundColorRenderSource.Create(Color.LightSkyBlue, 1.0),
                SkyboxRenderer.Create(Facade.Resources.LoadStream("skybox.png")),
            };
            CreateWalls(renderSources);
            var sources = AggregateRenderSource.Create(renderSources);
            scene.RenderTargets.Add(new RenderTarget(new RectangleF(0.0f, 0.0f, 1.0f, 0.5f), _camera1, sources, _textRenderer1));
            scene.RenderTargets.Add(new RenderTarget(new RectangleF(0.0f, 0.5f, 1.0f, 0.5f), _camera2, sources, _textRenderer2));
            return scene;
        }

        private static void CreateWalls(List<RenderSource> renderSources)
        {
            const int Steps = 12;
            var brickwall = Facade.Resources.LoadStream("brickwall.png");
            var length = new Vector3d(5, 0, 0);
            var target = QuaterniondHelper.CreateFromEulerAngles(2 * MathHelper.Pi / Steps, 0, 0);
            for (int index = 0; index < Steps; index++)
            {
                var quaterniond = QuaterniondHelper.Extrapolate(Quaterniond.Identity, target, index);
                var position = Vector3d.Transform(length, Matrix4d.Transpose(Matrix4d.CreateFromQuaternion(quaterniond)));
                renderSources.Add
                    (
                        BoxRenderer.Create
                            (
                                brickwall,
                                new Location
                                {
                                    Position = position + (index == 0 ? new Vector3d(0, 1, 0) : Vector3d.Zero),
                                    Orientation = quaterniond.Inverted()
                                },
                                Transformation.Identity.Scale((1 + index) * 1.0f / Steps)
                            )
                    );
            }
        }

        protected override void UpdateFrame(RenderInfo renderInfo)
        {
            UpdateCamera
            (
                renderInfo,
                _camera1,
                QuaterniondHelper.CreateFromEulerAngles(MathHelper.Pi / 1000, 0, 0),
                new Vector3d(0, 0, -0.001),
                _textRenderer1
            );
            UpdateCamera
            (
                renderInfo,
                _camera2,
                QuaterniondHelper.CreateFromEulerAngles(-MathHelper.Pi / 1000, 0, 0),
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