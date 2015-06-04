using System;
using System.Security.Permissions;

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

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public void Render()
        {
            _mesh.Render();
        }
    }
}