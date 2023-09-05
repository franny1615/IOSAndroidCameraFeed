using CommunityToolkit.Maui.Markup;
using Maui.NativeCamera;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace NativeCameraTester;

public class MainPage : ContentPage
{
    private NativeCameraView _cameraView = new()
    {
        CameraPosition = CameraPosition.FrontFacing
    };

    private Button _switchCameraPosition = new()
    {
        Text = "Rear"
    };

    private Button _takePhoto = new()
    {
        Text = "Photo"
    };

    private Button _takeVideo = new()
    {
        Text = "Record"
    };

    public MainPage()
    {
        _switchCameraPosition.Clicked += _switchCameraPosition_Clicked;
        _takePhoto.Clicked += _takePhoto_Clicked;
        _takeVideo.Clicked += _takeVideo_Clicked;

        Content = new Grid
        {
            RowDefinitions = Rows.Define(Star, 80),
            ColumnDefinitions = Columns.Define(Star, Star, Star),
            RowSpacing = 8,
            ColumnSpacing = 8,
            Padding = 8,
            Children =
            {
                _cameraView.Row(0).ColumnSpan(3),
                _switchCameraPosition.Row(1).Column(0),
                _takePhoto.Row(1).Column(1),
                _takeVideo.Row(1).Column(2)
            }
        };
    }

    private void _takeVideo_Clicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void _takePhoto_Clicked(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void _switchCameraPosition_Clicked(object sender, EventArgs e)
    {
        if (_switchCameraPosition.Text == "Rear")
        {
            _cameraView.SwitchCameraPosition(CameraPosition.RearFacing);
        }
        else if (_switchCameraPosition.Text == "Front")
        {
            _cameraView.SwitchCameraPosition(CameraPosition.FrontFacing);
        }
    }
}
