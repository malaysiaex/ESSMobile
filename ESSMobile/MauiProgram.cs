using Microsoft.Extensions.Logging;
//using Plugin.Fingerprint;
using Plugin.Maui.Biometric;

namespace ESSMobile;

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

#if DEBUG
        builder.Logging.AddDebug();
#endif

//#if ANDROID
//        CrossFingerprint.SetCurrentActivityResolver(() =>
//            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity);
//#endif

        return builder.Build();


    }
}


