using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;

namespace IOSAndroidCameraFeed;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static readonly int CAMERA_PERMISSIONS = 200;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        GetPermissions();
    }

    private void GetPermissions()
    {
        var permissions = new List<String>();

        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) != Permission.Granted)
        {
            permissions.Add(Manifest.Permission.Camera);
        }

        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != Permission.Granted)
        {
            permissions.Add(Manifest.Permission.ReadExternalStorage);
        }

        if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted)
        {
            permissions.Add(Manifest.Permission.WriteExternalStorage);
        }

        if (permissions.Count > 0)
        {
            ActivityCompat.RequestPermissions(
                this,
                permissions.ToArray(),
                MainActivity.CAMERA_PERMISSIONS);
        }
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        if (requestCode == CAMERA_PERMISSIONS)
        {
            List<bool> granted = new();
            foreach(var grantedPermission in grantResults)
            {
                if (grantedPermission == Permission.Granted)
                    granted.Add(true);
            }

            if (granted.Count == permissions.Length)
            {
                // TODO: through the WeakReferenceMessenger send a notificatoin up
                // that the camera permissions were granted. 
            }
        }

        if (OperatingSystem.IsAndroidVersionAtLeast(23))
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
