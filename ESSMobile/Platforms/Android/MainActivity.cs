using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

[assembly: UsesPermission(Android.Manifest.Permission.UseBiometric)]
[assembly: UsesPermission(Android.Manifest.Permission.UseFingerprint)]

namespace ESSMobile;

[Activity(Theme = "@style/Maui.SplashTheme", 
    MainLauncher = true, 
    LaunchMode = LaunchMode.SingleTop, 
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
    ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity
{

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Window.SetStatusBarColor(Android.Graphics.Color.Orange);
    }

}
