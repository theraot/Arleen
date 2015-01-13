using System;

namespace Arleen.Rendering
{
    public sealed class Model : IDisposable
    {
        // TODO: Multiple mesh per model
        private readonly Mesh _mesh;

        public Model(Mesh mesh)
        {
            _mesh = mesh;
        }

        public void Dispose()
        {
            _mesh.Dispose();
        }

        public void Render()
        {
            _mesh.Render();
        }
    }
}