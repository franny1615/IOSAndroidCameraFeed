using Maui.NativeCamera.Handlers;

namespace Maui.NativeCamera;

public static class AppHostBuilderExtensions
{
    public static MauiAppBuilder UseMauiNativeCamera(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler(typeof(NativeCameraView), typeof(NativeCameraHandler));
        });

        return builder;
    }
}

