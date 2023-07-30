using AVFoundation;
using CommunityToolkit.Maui.Alerts;
using Foundation;
using IOSAndroidCameraFeed.Controls;
using UIKit;

namespace IOSAndroidCameraFeed.Platforms.iOS.Controls;

public class CameraFeedPlatformView : UIView 
{
    AVCaptureSession captureSession;
    AVCaptureVideoPreviewLayer videoPreviewLayer;
    AVCaptureDevice captureDevice;
    AVCaptureDeviceInput videoInput; 

    public CameraFeedPlatformView(CameraFeedView cameraFeedView)
    {
        cameraFeedView.StartFeed = this.Start;
        cameraFeedView.SwitchCameraPosition = this.SwitchCameraPosition;

        AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

        AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, (granted) =>
        {
            if (!granted)
                return;

            Setup(cameraFeedView.CameraPosition);
        });
    }

    #region Setup
    private void Setup(CameraPosition position)
    {
        AVCaptureDevice device = GetCameraForPosition(position);

        if (device == null)
        {
            Toast.Make("Feed Failure").Show();
            return;
        }

        NSError error = null;
        AVCaptureDeviceInput input = new(device, out error);

        if (error != null)
        {
            Toast.Make($"Feed Failure {error.LocalizedDescription}").Show();
            return;
        }

        AVCaptureSession session = new();
        session.SessionPreset = AVCaptureSession.Preset1280x720;
        session.AddInput(input);

        AVCaptureVideoPreviewLayer layer = new(session);
        layer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
        layer.Frame = Layer.Frame;

        captureDevice = device;
        captureSession = session;
        videoPreviewLayer = layer;
        videoInput = input;

        Layer.AddSublayer(layer);
    }

    private AVCaptureDevice GetCameraForPosition(CameraPosition position)
    {
        var desiredPosition = position == CameraPosition.Front ?
            AVCaptureDevicePosition.Front :
            AVCaptureDevicePosition.Back;

        AVCaptureDeviceDiscoverySession discoverySession =
                    AVCaptureDeviceDiscoverySession.Create(
                        new AVCaptureDeviceType[] { AVCaptureDeviceType.BuiltInWideAngleCamera },
                        AVMediaTypes.Video,
                        AVCaptureDevicePosition.Unspecified);

        foreach (var device in discoverySession.Devices)
        {
            if (device.Position == desiredPosition)
                return device;
        }

        return null;
    }

    public override void LayoutSubviews()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            base.LayoutSubviews();
            Start();
        });
    }
    #endregion

    #region API 
    private void Start()
    {
        if (captureSession != null && !captureSession.Running)
        {
            videoPreviewLayer.Frame = Layer.Frame;
            captureSession.StartRunning();
        }        
    }

    private void SwitchCameraPosition(CameraPosition newPosition)
    {
        AVCaptureDevice device = GetCameraForPosition(newPosition);

        if (captureSession == null || videoInput == null || device == null)
            return;

        captureSession.BeginConfiguration();
        captureSession.RemoveInput(videoInput);

        NSError error = null;
        AVCaptureDeviceInput input = new(device, out error);

        if (error != null)
        {
            Toast.Make($"Feed Failure {error.LocalizedDescription}").Show();
            return;
        }

        videoInput = input;

        captureSession.AddInput(videoInput);
        captureSession.CommitConfiguration();
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        captureSession = null;
        videoInput = null;
        videoPreviewLayer = null;
        captureDevice = null;

        base.Dispose(disposing);
    }
}
