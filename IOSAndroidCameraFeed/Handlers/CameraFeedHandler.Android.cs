using IOSAndroidCameraFeed.Controls;
using IOSAndroidCameraFeed.Platforms.Android.Controls;
using Microsoft.Maui.Handlers;

namespace IOSAndroidCameraFeed.Handlers;

public partial class CameraFeedHandler : ViewHandler<CameraFeedView, CameraFeedPlatformView>
{
    protected override CameraFeedPlatformView CreatePlatformView()
    {
        return new CameraFeedPlatformView(Context, VirtualView);
    }

    protected override void ConnectHandler(CameraFeedPlatformView platformView)
    {
        base.ConnectHandler(platformView);
    }

    protected override void DisconnectHandler(CameraFeedPlatformView platformView)
    {
        platformView.Dispose();
        base.DisconnectHandler(platformView);
    }
}
