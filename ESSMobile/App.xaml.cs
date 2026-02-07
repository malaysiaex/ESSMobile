namespace ESSMobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
            //MainPage = new NavigationPage(new Page.Tabbed_Login());
            //Application.Current.MainPage = new Page.Tabbed_Login(); // Navigate back to login
            MainPage = new Page.Tabbed_Login(); // Navigate back to login
        }
    }
}
