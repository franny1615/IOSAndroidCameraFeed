using Microsoft.Extensions.Logging;
using Maui.NativeCamera;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Markup;

namespace NativeCameraTester;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiNativeCamera()
			.UseMauiCommunityToolkit()
			.UseMauiCommunityToolkitCore()
            .UseMauiCommunityToolkitMarkup();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

