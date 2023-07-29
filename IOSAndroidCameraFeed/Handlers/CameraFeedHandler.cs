using IOSAndroidCameraFeed.Controls;
using Microsoft.Maui.Handlers;

namespace IOSAndroidCameraFeed.Handlers;

public partial class CameraFeedHandler
{
    public static IPropertyMapper<CameraFeedView, CameraFeedHandler> PropertyMapper 
        = new PropertyMapper<CameraFeedView, CameraFeedHandler>(ViewHandler.ViewMapper)
        {
        };

    public static CommandMapper<CameraFeedView, CameraFeedHandler> CommandMapper 
        = new(ViewCommandMapper)
        {
        };

    public CameraFeedHandler() : base(PropertyMapper, CommandMapper)
    {
    }
}
