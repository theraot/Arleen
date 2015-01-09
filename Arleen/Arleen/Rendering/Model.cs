using OpenTK.Graphics.OpenGL;
using System;

namespace Arleen.Rendering
{
    public sealed class Model : IDisposable
    {
        private readonly int dataBuffer = -1;
        private readonly int indexBuffer = -1;

        public Model(float[] data, byte[] indexes)
        {
            GL.Arb.GenBuffers(1, out dataBuffer);
            GL.Arb.BindBuffer(BufferTargetArb.ArrayBuffer, dataBuffer);
            GL.Arb.BufferData(BufferTargetArb.ArrayBuffer, (IntPtr)(data.Length * sizeof(float)), data, BufferUsageArb.StaticDraw);

            GL.Arb.GenBuffers(1, out indexBuffer);
            GL.Arb.BindBuffer(BufferTargetArb.ElementArrayBuffer, indexBuffer);
            GL.Arb.BufferData(BufferTargetArb.ElementArrayBuffer, (IntPtr)(indexes.Length * sizeof(byte)), indexes, BufferUsageArb.StaticDraw);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(dataBuffer);
            GL.DeleteBuffer(indexBuffer);
        }

        public void Render()
        {
            //---
            GL.Arb.BindBuffer(BufferTargetArb.ArrayBuffer, dataBuffer);
            GL.VertexPointer(3, VertexPointerType.Float, sizeof(float) * 5, new IntPtr(0));
            GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(float) * 5, new IntPtr(sizeof(float) * 3));
            //---
            GL.Arb.BindBuffer(BufferTargetArb.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(BeginMode.Quads, 24, DrawElementsType.UnsignedByte, new IntPtr(0));
        }
    }
}