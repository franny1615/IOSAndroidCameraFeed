using AVFoundation;
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

        AVCaptureDevicePosition position = AVCaptureDevicePosition.Front;
        if (cameraFeedView.CameraPosition == CameraPosition.Rear)
            position = AVCaptureDevicePosition.Back;

        AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, (granted) =>
        {
            if (!granted)
                return;

            SetupCaptureDevice(position);
            SetupCaptureSession();
        });
    }

    private void SetupCaptureDevice(AVCaptureDevicePosition position)
    {
        captureDevice = AVCaptureDevice.GetDefaultDevice(
            AVCaptureDeviceType.BuiltInDualCamera,
            AVMediaTypes.Video,
            position);

        if (captureDevice == null && OperatingSystem.IsIOSVersionAtLeast(13))
            captureDevice = AVCaptureDevice.GetDefaultDevice(
                AVCaptureDeviceType.BuiltInDualWideCamera,
                AVMediaTypes.Video,
                position);
    }

    private void SetupCaptureSession()
    {
        NSError error = null;
        videoInput = new AVCaptureDeviceInput(captureDevice, out error);

        captureSession = new AVCaptureSession();
        captureSession.SessionPreset = AVCaptureSession.Preset1280x720;
        captureSession.AddInput(videoInput);

        videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession);
        videoPreviewLayer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;

        videoPreviewLayer.Frame = Layer.Frame;
        Layer.AddSublayer(videoPreviewLayer);
    }

    private void Start()
    {
        if (captureSession != null && !captureSession.Running)
        {
            captureSession.StartRunning();
        }        
    }

    private void SwitchCameraPosition(CameraPosition newPosition)
    {
        AVCaptureDevicePosition position = AVCaptureDevicePosition.Front;
        if (newPosition == CameraPosition.Rear)
            position = AVCaptureDevicePosition.Back;

        captureSession.StopRunning();
        videoPreviewLayer.RemoveFromSuperLayer();

        SetupCaptureDevice(position);
        SetupCaptureSession();
    }

    public override void LayoutSubviews()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            base.LayoutSubviews();
            Start();
        });
    }
}
