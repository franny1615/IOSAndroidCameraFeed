using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace Maui.NativeCamera.Views;

public class NativeCameraPlatformView : RelativeLayout, TextureView.ISurfaceTextureListener
{
	public NativeCameraPlatformView(
        Context context,
        NativeCameraView nativeCameraView) : base(context)
	{

	}

    public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
    {
        // TODO:
    }

    public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
    {
        // TODO:
        return false;
    }

    public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
    {
        // TODO:
    }

    public void OnSurfaceTextureUpdated(SurfaceTexture surface)
    {
        // TODO: 
    }
}
