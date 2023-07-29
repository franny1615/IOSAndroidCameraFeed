using Android.Content;
using AndroidX.CoordinatorLayout.Widget;
using IOSAndroidCameraFeed.Controls;

namespace IOSAndroidCameraFeed.Platforms.Android.Controls;

public class CameraFeedPlatformView : CoordinatorLayout
{
    private readonly CameraFeedView _cameraFeedView;

    public CameraFeedPlatformView(
        Context context,
        CameraFeedView cameraFeedView) : base(context)
    {
        _cameraFeedView = cameraFeedView;
    }
}
