using Maui.NativeCamera.Views;
using Microsoft.Maui.Handlers;

namespace Maui.NativeCamera.Handlers;

public partial class NativeCameraHandler : ViewHandler<NativeCameraView, NativeCameraPlatformView>
{
    protected override NativeCameraPlatformView CreatePlatformView()
    {
        return new NativeCameraPlatformView(Context, VirtualView);
    }

    protected override void ConnectHandler(NativeCameraPlatformView platformView)
    {
        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(NativeCameraPlatformView platformView)
    {
        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }
}
