using AVFoundation;
using CommunityToolkit.Maui.Alerts;
using Foundation;
using IOSAndroidCameraFeed.Controls;
using UIKit;
using CoreFoundation;
using CoreMedia;
using CoreImage;

namespace IOSAndroidCameraFeed.Platforms.iOS.Controls;

public class CameraFeedPlatformView :
    UIView,
    IAVCapturePhotoCaptureDelegate,
    IAVCaptureVideoDataOutputSampleBufferDelegate,
    IAVCaptureFileOutputRecordingDelegate
{
    AVCaptureSession captureSession;
    AVCaptureVideoPreviewLayer videoPreviewLayer;
    AVCaptureDevice captureDevice;
    AVCaptureDeviceInput videoInput;
    AVCaptureVideoDataOutput videoOutput;
    AVCapturePhotoOutput imageOutput;
    AVCaptureMovieFileOutput videoRecordOutput;

    byte[] latestVideoFrame;

    Action<byte[]> takePhotoCompletion;
    Action<byte[]> takeVideoCompletion;

    public CameraFeedPlatformView(CameraFeedView cameraFeedView)
    {
        cameraFeedView.StartFeed = this.Start;
        cameraFeedView.SwitchCameraPosition = this.SwitchCameraPosition;
        cameraFeedView.GetImage = this.GetImage;
        cameraFeedView.GetLatestVideoFrame = this.GetLatestVideoFrame;
        cameraFeedView.StartRecording = this.StartRecording;
        cameraFeedView.EndRecording = this.EndRecording;

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

        AVCapturePhotoOutput imgOutput = new();
        session.AddOutput(imgOutput);

        AVCaptureVideoDataOutput vidOutput = new();
        vidOutput.AlwaysDiscardsLateVideoFrames = true;
        vidOutput.SetSampleBufferDelegate(this, DispatchQueue.DefaultGlobalQueue);
        session.AddOutput(vidOutput);

        AVCaptureMovieFileOutput vidRecordOutput = new();
        session.AddOutput(vidRecordOutput);

        AVCaptureVideoPreviewLayer layer = new(session);
        layer.VideoGravity = AVLayerVideoGravity.ResizeAspectFill;
        layer.Frame = Layer.Frame;

        captureDevice = device;
        captureSession = session;
        videoPreviewLayer = layer;
        videoInput = input;
        videoOutput = vidOutput;
        imageOutput = imgOutput;
        videoRecordOutput = vidRecordOutput;

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

    private void GetImage(Action<byte[]> completion)
    {
        var settings = AVCapturePhotoSettings.Create();
        if (OperatingSystem.IsIOSVersionAtLeast(13))
        {
            settings.PhotoQualityPrioritization = AVCapturePhotoQualityPrioritization.Speed;
        }
        else
        {
            settings.IsAutoStillImageStabilizationEnabled = true;
            settings.IsHighResolutionPhotoEnabled = false;
        }

        takePhotoCompletion = completion;
        imageOutput.CapturePhoto(
            AVCapturePhotoSettings.Create(),
            this);
    }

    [Export("captureOutput:didFinishProcessingPhoto:error:")]
    public void DidFinishProcessingPhoto(
        AVCapturePhotoOutput output,
        AVCapturePhoto photo,
        NSError error)
    {
        if (error != null)
        {
            takePhotoCompletion?.Invoke(new byte[] { });
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"DidFinishProcessingPhoto error >>> {error.LocalizedDescription}");
#endif
            return;
        }

        NSData data = photo.FileDataRepresentation;
        if (data == null)
            System.Diagnostics.Debug.WriteLine($"DidFinishProcessingPhoto error >>> data was null for photo");

        takePhotoCompletion?.Invoke(data.ToArray());
    }

    private void GetLatestVideoFrame(Action<byte[]> completion)
    {
        if (latestVideoFrame == null)
        {
            completion(new byte[] { });
            return;
        }

        completion(latestVideoFrame);
    }

    [Export("captureOutput:didOutputSampleBuffer:fromConnection:")]
    public void DidOutputSampleBuffer(
        AVCaptureOutput captureOutput,
        CMSampleBuffer sampleBuffer,
        AVCaptureConnection connection)
    {
        var imageBuffer = sampleBuffer.GetImageBuffer();
        if (imageBuffer == null)
            return;

        var ciImage = CIImage.FromImageBuffer(imageBuffer);
        var context = new CIContext();

        if (ciImage == null)
            return;

        var cgImage = context.CreateCGImage(ciImage, ciImage.Extent);
        if (cgImage == null)
            return;

        var uiImage = new UIImage(cgImage: cgImage);
        NSData data = uiImage.AsJPEG();

        latestVideoFrame = data.ToArray();
    }

    private void StartRecording()
    {
        if (videoRecordOutput.Recording)
            return;

        var urls = NSFileManager.DefaultManager.GetUrls(
            NSSearchPathDirectory.DocumentDirectory,
            NSSearchPathDomain.User);

        var url = urls.First()?.Append("video.mp4", false);

        if (url == null)
            return;

        NSError error;
        if (NSFileManager.DefaultManager.FileExists(url.Path))
            NSFileManager.DefaultManager.Remove(url.Path, out error); // clean up previous recording

        videoRecordOutput.StartRecordingToOutputFile(url, this);   
    }

    private void EndRecording(Action<byte[]> completion)
    {
        this.takeVideoCompletion = completion;
        videoRecordOutput.StopRecording();
    }

    public void FinishedRecording(
        AVCaptureFileOutput captureOutput,
        NSUrl outputFileUrl,
        NSObject[] connections,
        NSError error)
    {
        if (error != null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"FinishRecording error >>> {error.LocalizedDescription}");
#endif
            takeVideoCompletion?.Invoke(new byte[] { });
            return;
        }

        var data = NSData.FromUrl(outputFileUrl);
        if (data == null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"FinishRecording error >>> data at url was empty/null");
#endif
            takeVideoCompletion?.Invoke(new byte[] { });
            return;
        }

        takeVideoCompletion?.Invoke(data.ToArray());
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        captureSession = null;
        videoInput = null;
        videoPreviewLayer = null;
        captureDevice = null;
        imageOutput = null;
        videoOutput = null;
        videoRecordOutput = null;

        base.Dispose(disposing);
    }
}
