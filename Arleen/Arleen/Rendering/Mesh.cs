using OpenTK.Graphics.OpenGL;
using System;

namespace Arleen.Rendering
{
    /// <summary>
    /// Represents a 3D structure.
    /// </summary>
    public sealed class Mesh : IDisposable
    {
        private const int INT_ColorSize = 4;
        private const int INT_NormalSize = 3;
        private const int INT_PositionSize = 3;
        private const int INT_TextureSize = 2;

        private static readonly bool? CoreSupport;

        private readonly PrimitiveType _beginMode;
        private readonly int _bufferData;
        private readonly int _bufferIndexes;

        private readonly int _length;

        private readonly int? _offsetColor;
        private readonly int? _offsetNormal;
        private readonly int? _offsetTexture;

        private readonly Action _render;
        private readonly int _stride;

        static Mesh()
        {
            const string ARB = "GL_ARB_vertex_buffer_object";
            var version = GL.GetString(StringName.Version);
            var exts = GL.GetString(StringName.Extensions);
            if (int.Parse(version.Split(' ')[0].Split('.')[0]) >= 2)
            {
                CoreSupport = true;
            }
            else
            {
                CoreSupport &= !exts.Contains(ARB);
            }
        }

        /// <summary>
        /// Creates a new instance of Mesh.
        /// </summary>
        /// <param name="vextexInfo">The kinds of vertex info contained in this mesh.</param>
        /// <param name="data">The vertexes data.</param>
        /// <param name="beginMode">The way the vertexes are organized.</param>
        /// <param name="indexes">The order of the vertexes.</param>
        public Mesh(VextexInfo vextexInfo, float[] data, PrimitiveType beginMode, byte[] indexes)
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

        /// <summary>
        /// Represents the kinds of vertex information.
        /// </summary>
        [Flags]
        public enum VextexInfo
        {
            Position = 0,
            Normal = 1,
            Color = 2,
            Texture = 4
        }

        /// <summary>
        /// Releases the allocated memory of the current Mesh.
        /// </summary>
        public void Dispose()
        {
            GL.DeleteBuffer(_bufferData);
            GL.DeleteBuffer(_bufferIndexes);
        }

        /// <summary>
        /// Renders the current Mesh.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CC0016:Copy Event To Variable Before Fire", Justification = "False Positive")]
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