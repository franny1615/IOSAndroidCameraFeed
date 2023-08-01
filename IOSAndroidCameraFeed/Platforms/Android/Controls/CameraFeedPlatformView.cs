using Android;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using IOSAndroidCameraFeed.Controls;
using Java.Lang;

namespace IOSAndroidCameraFeed.Platforms.Android.Controls;

public class CameraFeedPlatformView : RelativeLayout, TextureView.ISurfaceTextureListener
{
    // https://developer.android.com/reference/android/hardware/camera2/CameraCharacteristics#LENS_FACING
    private static readonly int LENS_FORWARD = 0;
    private static readonly int LENS_BACKWARD = 1;

    private readonly Context context;
    private readonly CameraFeedView cameraFeedView;

    private TextureView textureView;

    private CameraManager cameraManager;
    private CameraCaptureSession cameraCaptureSession;
    private CameraDevice cameraDevice;
    private CaptureRequest captureRequest;

    private CaptureRequest.Builder captureRequestBuilder;

    private Handler handler;
    private HandlerThread handlerThread;

    public CameraFeedPlatformView(
        Context context,
        CameraFeedView cameraFeedView) : base(context)
    {
        this.cameraFeedView = cameraFeedView;
        this.context = context;

        SetupLayout();
        RegisterListeners();
        if (HasPermissions())
            SetupCamera(cameraFeedView.CameraPosition);
    }

    #region Setup
    private void SetupLayout()
    {
        LayoutParameters = new LayoutParams(
            LayoutParams.MatchParent,
            LayoutParams.MatchParent);

        textureView = new TextureView(context);
        textureView.LayoutParameters = new LayoutParams(
            LayoutParams.MatchParent,
            LayoutParams.MatchParent);

        textureView.SurfaceTextureListener = this;
        AddView(textureView);
    }

    private void RegisterListeners()
    {
        // TODO: register WeakReferenceMessenger listener
    }

    private bool HasPermissions()
    {
        var permissions = new List<string>();

        if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.Camera) != Permission.Granted)
        {
            permissions.Add(Manifest.Permission.Camera);
        }

        if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
        {
            permissions.Add(Manifest.Permission.ReadExternalStorage);
        }

        if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
        {
            permissions.Add(Manifest.Permission.WriteExternalStorage);
        }

        if (permissions.Count > 0)
        {
            ActivityCompat.RequestPermissions(
                Platform.CurrentActivity,
                permissions.ToArray(),
                MainActivity.CAMERA_PERMISSIONS);

            return false;
        }

        return true;
    }

    private void SetupCamera(CameraPosition position)
    {
        cameraManager = (CameraManager) Platform.CurrentActivity.GetSystemService(Context.CameraService);
        handlerThread = new HandlerThread("videoThread");
        handlerThread.Start();

        handler = new Handler(handlerThread.Looper);
    }

    [Obsolete]
    private void OpenCamera(CameraPosition position)
    {
        string cameraId = GetCameraForPosition(position);
        if (string.IsNullOrEmpty(cameraId))
            return;

        cameraManager.OpenCamera(
            cameraId,
            new CameraStateCallback(
                disconnected: (camera) => { },
                gotError: (camera, error) => { },
                connected: (camera) =>
                {
                    cameraDevice = camera;
                    captureRequestBuilder = cameraDevice.CreateCaptureRequest(CameraTemplate.Preview);

                    var surface = new Surface(textureView.SurfaceTexture);
                    captureRequestBuilder.AddTarget(surface);

                    cameraDevice.CreateCaptureSession(
                        new List<Surface> { surface },
                        new CameraCaptureSessionCallback(
                            configured: (session) =>
                            {
                                cameraCaptureSession = session;
                                cameraCaptureSession.SetRepeatingRequest(
                                    captureRequestBuilder.Build(),
                                    null,
                                    null);
                            },
                            configFailed: (session) => { }),
                        handler);
                }),
            handler);
    }

    private string GetCameraForPosition(CameraPosition position)
    {
        string[] _ids = cameraManager.GetCameraIdList();
        foreach(string id in _ids)
        {
            var properties = cameraManager.GetCameraCharacteristics(id);
            int facingProperty = ((Integer)properties.Get(CameraCharacteristics.LensFacing)).IntValue();
            if (facingProperty == LENS_FORWARD && position == CameraPosition.Front)
            {
                return id;
            }

            if (facingProperty == LENS_BACKWARD && position == CameraPosition.Rear)
            {
                return id; 
            }
        }

        return "";
    }
    #endregion

    #region API
    private void StartFeed()
    {

    }

    private void SwitchCameraPosition(CameraPosition position)
    {

    }

    private void GetImage(Action<byte[]> completion)
    {

    }

    private void GetLatestVideoFrame(Action<byte[]> completion)
    {

    }

    private void StartRecording()
    {

    }

    private void EndRecording(Action<byte[]> completion)
    {

    }
    #endregion

    #region Surface Listener
    public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
    {
        return false;
    }

    public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
    {
        OpenCamera(cameraFeedView.CameraPosition);
    }

    public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height) { }
    public void OnSurfaceTextureUpdated(SurfaceTexture surface) { }
    #endregion
}

public class CameraStateCallback : CameraDevice.StateCallback
{
    private Action<CameraDevice> Disconnected;
    private Action<CameraDevice, CameraError> GotError;
    private Action<CameraDevice> Connected; 

    public CameraStateCallback(
        Action<CameraDevice> disconnected,
        Action<CameraDevice, CameraError> gotError,
        Action<CameraDevice> connected)
    {
        Disconnected = disconnected;
        GotError = gotError;
        Connected = connected; 
    }

    public override void OnDisconnected(CameraDevice camera)
    {
        Disconnected?.Invoke(camera);
    }

    public override void OnError(CameraDevice camera, [GeneratedEnum] CameraError error)
    {
        GotError?.Invoke(camera, error);
    }

    public override void OnOpened(CameraDevice camera)
    {
        Connected?.Invoke(camera);
    }
}

public class CameraCaptureSessionCallback : CameraCaptureSession.StateCallback
{
    private Action<CameraCaptureSession> Configured;
    private Action<CameraCaptureSession> ConfigFailed;

    public CameraCaptureSessionCallback(
        Action<CameraCaptureSession> configured,
        Action<CameraCaptureSession> configFailed)
    {
        Configured = configured;
        ConfigFailed = configFailed;
    }

    public override void OnConfigured(CameraCaptureSession session)
    {
        Configured?.Invoke(session);
    }

    public override void OnConfigureFailed(CameraCaptureSession session)
    {
        ConfigFailed?.Invoke(session);
    }
}