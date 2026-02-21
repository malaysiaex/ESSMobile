
//using AndroidX.Startup;
using ESSMobile.Shared;

namespace ESSMobile.Page;

public partial class Tabbed_Login : TabbedPage
{
    private readonly ApiInitializer _apiInitializer;
    public Tabbed_Login()
    {
        InitializeComponent();
        _apiInitializer = (Application.Current as App)?.ApiInitializer;
    }

    protected override async void OnAppearing()
    {
        var apiInitializer = (Application.Current as App)?.ApiInitializer;
        if (apiInitializer != null)
        {
            await apiInitializer.InitializationTask;
        }
        base.OnAppearing();



    }
}