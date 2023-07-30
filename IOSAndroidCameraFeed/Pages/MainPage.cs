using CommunityToolkit.Maui.Markup;
using IOSAndroidCameraFeed.Controls;
using System.Diagnostics;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace IOSAndroidCameraFeed.Pages;

public class MainPage : ContentPage
{
	private CameraFeedView cameraFeed;

	private Button recordButton = new Button
	{
		Text = "Record",
	};

	private Button switchCameraPosition = new Button
	{
		Text = "Switch CP"
	};

	private Button takePhoto = new Button
	{
		Text = "Photo"
	};

	public MainPage()
	{
		cameraFeed = new CameraFeedView();
		cameraFeed.CameraPosition = CameraPosition.Rear;

		switchCameraPosition.TapGesture(SwitchCameraPosition, 1);
		takePhoto.TapGesture(GetImage, 1);

		Content = new Grid 
		{
			RowDefinitions = Rows.Define(Star, 44),
			ColumnDefinitions = Columns.Define(Star, Star, Star, Star, Star),
			Children =
			{
                cameraFeed
                    .Row(0)
					.RowSpan(2)
					.ColumnSpan(5),
				switchCameraPosition
					.Row(1)
					.Column(0),
				recordButton
					.Row(1)
					.Column(2),
				takePhoto
					.Row(1)
					.Column(4)
			}
		};

		FetchFrameEveryTenSecond();
	}

    private void SwitchCameraPosition()
	{
		if (cameraFeed.CameraPosition == CameraPosition.Rear)
		{
			cameraFeed.CameraPosition = CameraPosition.Front;
		}
		else
		{
			cameraFeed.CameraPosition = CameraPosition.Rear;
		}
	}

	private void GetImage()
	{
		cameraFeed.GetImage((byteArray) =>
		{
            Debug.WriteLine($"Current Image as BASE64 >>> {Convert.ToBase64String(byteArray)}");
        });
	}

	private void FetchFrameEveryTenSecond()
	{
		Task.Run(async () =>
		{
			while(true)
			{
				await Task.Delay(10000);
                cameraFeed.GetLatestVideoFrame((byteArray) =>
                {
                    Debug.WriteLine($"Current VIDEO FRAME as BASE64 >>> {Convert.ToBase64String(byteArray)}");
                });
            }
		});
	}
}