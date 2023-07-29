using IOSAndroidCameraFeed.Controls;
using IOSAndroidCameraFeed.Handlers;
using IOSAndroidCameraFeed.Pages;
using Microsoft.Extensions.Logging;

namespace IOSAndroidCameraFeed;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddTransient<MainPage>();
		builder.ConfigureMauiHandlers(handlers =>
		{
			handlers.AddHandler(typeof(CameraFeedView), typeof(CameraFeedHandler));
		});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
