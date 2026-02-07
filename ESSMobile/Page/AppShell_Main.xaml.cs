namespace ESSMobile.Page;

public partial class AppShell_Main : Shell
{
	public AppShell_Main()
	{
		InitializeComponent();
	}

    protected override bool OnBackButtonPressed()
    {
        Application.Current.MainPage = new Tabbed_Login();
        return true; // cancel default back behavior
    }

    private void btnLogout_Clicked(object sender, EventArgs e)
    {
        // Clear SecureStorage data
        SecureStorage.Remove("login_username");
        SecureStorage.Remove("login_password");
        SecureStorage.Remove("inpData");                    //for enable and disable the input field
        Application.Current.MainPage = new Tabbed_Login();  // Navigate back to login

    }

}