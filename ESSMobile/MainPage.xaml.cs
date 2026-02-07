namespace ESSMobile
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)namespace ESSMobile;

    public partial class MainPage : TabbedPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnLoginClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            // TODO: Validate credentials
            if (username == "admin" && password == "1234")
            {
                DisplayAlert("Login", "Login successful!", "OK");
            }
            else
            {
                DisplayAlert("Login Failed", "Invalid username or password.", "OK");
            }
        }
    }

    CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
