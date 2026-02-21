using ESSMobile.Shared;
//using Plugin.Fingerprint;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        // <---------------- api initialization ------------------>
        builder.Services.AddSingleton<ApiInitializer>();
        builder.Services.AddHttpClient("ESSMobile");
        builder.Services.AddHttpClient("ESSMobile2");
        builder.Services.AddHttpClient("ESSMobileWeb");
        var app = builder.Build();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        //#if ANDROID
        //        CrossFingerprint.SetCurrentActivityResolver(() =>
        //            Microsoft.Maui.ApplicationModel.Platform.CurrentActivity);
        //#endif
        return app;


    }
}


