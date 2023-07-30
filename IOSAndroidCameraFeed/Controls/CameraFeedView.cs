namespace IOSAndroidCameraFeed.Controls;

public class CameraFeedView : View
{
    public static readonly BindableProperty CameraPositionProperty
        = BindableProperty.Create(
            nameof(CameraPositionProperty),
            typeof(CameraPosition),
            typeof(CameraFeedView),
            propertyChanged: (bindable, oldval, newval) =>
            {
                if (bindable == null)
                    return;

                CameraFeedView view = (CameraFeedView)bindable;
                view.SwitchCameraPosition?.Invoke((CameraPosition) newval);
            });

    public CameraPosition CameraPosition
    {
        get => (CameraPosition)GetValue(CameraPositionProperty);
        set => SetValue(CameraPositionProperty, value);
    }

    public Action StartFeed;
    public Action<CameraPosition> SwitchCameraPosition;
    public Action<Action<byte[]>> GetImage;
    public Action<Action<byte[]>> GetLatestVideoFrame;
}

public enum CameraPosition
{
    Front = 0,
    Rear = 1
}