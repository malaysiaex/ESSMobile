using ESSMobile.Shared;

namespace ESSMobile
{
    public partial class App : Application
    {
        private readonly ApiInitializer _apiInitializer;

        public App(ApiInitializer apiInitializer)
        {
            InitializeComponent();
            _apiInitializer = apiInitializer;

            MainPage = new Page.Tabbed_Login(); // app shows immediately
        }

        public ApiInitializer ApiInitializer => _apiInitializer; // property for pages
    }
}
