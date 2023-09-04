using Microsoft.Extensions.Logging;
using Maui.NativeCamera;

namespace NativeCameraTester;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiNativeCamera();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

