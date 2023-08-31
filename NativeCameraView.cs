namespace Maui.NativeCamera;

public class NativeCameraView : View 
{
    /// <summary>
    /// backing store for the <see cref="CameraPosition" /> bindable property
    /// </summary>
    public static readonly BindableProperty CameraPositionProperty = BindableProperty.Create(
        nameof(CameraPositionProperty),
        typeof(CameraPosition),
        typeof(NativeCameraView),
        propertyChanged: (bindable, oldval, newval) =>
        {
            if (bindable == null)
                return;

            NativeCameraView view = (NativeCameraView)bindable;
            view.SwitchCameraPosition?.Invoke((CameraPosition)newval);
        });

    /// <summary>
    /// Gets or Sets the desired camera (front or rear facing) position. 
    /// </summary>
    public CameraPosition CameraPosition
    {
        get => (CameraPosition)GetValue(CameraPositionProperty);
        set => SetValue(CameraPositionProperty, value);
    }

    /// <summary>
    /// Switches the current camera position to desired one.
    /// </summary>
    public Action<CameraPosition> SwitchCameraPosition;

    /// <summary>
    /// Returns a byte array representation of an image taken from a native view.
    /// </summary>
    public Action<Action<byte[]>> TakePhoto;

    /// <summary>
    /// Returns a byte array representation of a frame of the current native camera feed.
    /// </summary>
    public Action<Action<byte[]>> GetCameraFeedFrame;

    /// <summary>
    /// Starts a video recording.
    /// </summary>
    public Action StartVideoRecording;

    /// <summary>
    /// Ends a video recording. Returning byte array representation of file. 
    /// </summary>
    public Action<Action<byte[]>> EndVideoRecording;
}

public enum CameraPosition
{
    FrontFacing = 0,
    RearFacing = 1
}

