using OpenTK.Graphics.OpenGL;
using System;

namespace Arleen.Rendering
{
    public sealed class Model : IDisposable
    {
        private const int INT_ColorSize = 4;
        private const int INT_NormalSize = 3;
        private const int INT_PositionSize = 3;
        private const int INT_TextureSize = 2;

        private static readonly bool? CoreSupport;

        private readonly BeginMode _beginMode;
        private readonly int _bufferData = -1;
        private readonly int _bufferIndexes = -1;

        private readonly int _length;

        private readonly int? _offsetColor;
        private readonly int? _offsetNormal;
        private readonly int? _offsetTexture;

        private readonly Action _render;
        private readonly int _stride;

        static Model()
        {
            const string ARB = "GL_ARB_vertex_buffer_object";
            var version = GL.GetString(StringName.Version);
            var exts = GL.GetString(StringName.Extensions);
            if (int.Parse(version.Split(' ')[0].Split('.')[0]) >= 2)
            {
                CoreSupport = true;
            }
            else if (exts.Contains(ARB))
            {
                CoreSupport = false;
            }
        }

        public Model(VextexInfo vextexInfo, float[] data, BeginMode beginMode, byte[] indexes)
        {
            _stride = INT_PositionSize;
            if ((int)(vextexInfo & VextexInfo.Color) != 0)
            {
                _offsetColor = _stride;
                _stride += INT_ColorSize;
            }

            if ((int)(vextexInfo & VextexInfo.Normal) != 0)
            {
                _offsetNormal = _stride;
                _stride += INT_NormalSize;
            }
            if ((int)(vextexInfo & VextexInfo.Texture) != 0)
            {
                _offsetTexture = _stride;
                _stride += INT_TextureSize;
            }

            _beginMode = beginMode;
            _length = indexes.Length;

            if (CoreSupport.HasValue)
            {
                if (CoreSupport.Value)
                {
                    GL.GenBuffers(1, out _bufferData);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _bufferData);
                    GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * sizeof(float)), data, BufferUsageHint.StaticDraw);

                    GL.GenBuffers(1, out _bufferIndexes);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, _bufferIndexes);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_length * sizeof(byte)), indexes, BufferUsageHint.StaticDraw);

                    _render = () =>
                    {
                        GL.BindBuffer(BufferTarget.ArrayBuffer, _bufferData);
                        RenderExtracted();
                        //---
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _bufferIndexes);
                        GL.DrawElements(_beginMode, _length, DrawElementsType.UnsignedByte, new IntPtr(0));
                    };
                }
                else
                {
                    GL.Arb.GenBuffers(1, out _bufferData);
                    GL.Arb.BindBuffer(BufferTargetArb.ArrayBuffer, _bufferData);
                    GL.Arb.BufferData(BufferTargetArb.ArrayBuffer, (IntPtr)(data.Length * sizeof(float)), data, BufferUsageArb.StaticDraw);

                    GL.Arb.GenBuffers(1, out _bufferIndexes);
                    GL.Arb.BindBuffer(BufferTargetArb.ElementArrayBuffer, _bufferIndexes);
                    GL.Arb.BufferData(BufferTargetArb.ElementArrayBuffer, (IntPtr)(_length * sizeof(byte)), indexes, BufferUsageArb.StaticDraw);

                    _render = () =>
                    {
                        GL.Arb.BindBuffer(BufferTargetArb.ArrayBuffer, _bufferData);
                        RenderExtracted();
                        //---
                        GL.Arb.BindBuffer(BufferTargetArb.ElementArrayBuffer, _bufferIndexes);
                        GL.DrawElements(_beginMode, _length, DrawElementsType.UnsignedByte, new IntPtr(0));
                    };
                }
            }
            else
            {
                throw new PlatformNotSupportedException("The video driver does not provide vertext buffers.");
            }
        }

        [Flags]
        public enum VextexInfo
        {
            Position = 0,
            Normal = 1,
            Color = 2,
            Texture = 4
        }

        public void Dispose()
        {
            GL.DeleteBuffer(_bufferData);
            GL.DeleteBuffer(_bufferIndexes);
        }

        public void Render()
        {
            _render();
        }

        private void RenderExtracted()
        {
            GL.VertexPointer(INT_PositionSize, VertexPointerType.Float, sizeof(float) * _stride, new IntPtr(0));
            if (_offsetColor.HasValue)
            {
                GL.ColorPointer(INT_ColorSize, ColorPointerType.Float, sizeof(float) * _stride, new IntPtr(sizeof(float) * _offsetColor.Value));
            }
            if (_offsetNormal.HasValue)
            {
                GL.NormalPointer(NormalPointerType.Float, sizeof(float) * _stride, new IntPtr(sizeof(float) * _offsetNormal.Value));
            }
            if (_offsetTexture.HasValue)
            {
                GL.TexCoordPointer(INT_TextureSize, TexCoordPointerType.Float, sizeof(float) * _stride, new IntPtr(sizeof(float) * _offsetTexture.Value));
            }
        }
    }
}