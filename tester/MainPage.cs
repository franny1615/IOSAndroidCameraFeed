using Maui.NativeCamera;

namespace NativeCameraTester;

public class MainPage : ContentPage
{
	public MainPage()
	{
		Content = new Grid
		{
			Children =
			{
				new NativeCameraView
				{
					CameraPosition = CameraPosition.FrontFacing
				}
			}
		};
	}
}
