using System.Security.Permissions;

namespace Arleen.Rendering
{
    public interface IRenderable
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        void Render();
    }
}