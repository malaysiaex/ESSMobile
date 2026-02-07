using ESSMobile.Shared;
//using Plugin.Fingerprint.Abstractions;
//using Plugin.Fingerprint;
using Plugin.Maui.Biometric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;

namespace ESSMobile.Page;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class Login : ContentPage
{

    string getIdentifier = "";
    string getCompany = "";
    string getUsername = "";
    string getPassword = "";

    //public readonly IBiometric biometric;

    public Login()
	{
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        lblVersion.Text = AppInfo.VersionString;

        //preload caches
        try
        {
            getUsername = await SecureStorage.GetAsync("login_username") ?? "";
        }
        catch { getUsername = ""; }
        inpUsername.Text = getUsername;
        try
        {
            getPassword = await SecureStorage.GetAsync("login_password") ?? "";
        }
        catch { getPassword = ""; }
        inpPassword.Text = getPassword;

        //autocomplete username & password
        if ((string.IsNullOrEmpty(getUsername) || string.IsNullOrWhiteSpace(getUsername)) && (string.IsNullOrEmpty(getPassword) || string.IsNullOrWhiteSpace(getPassword)))
        {
            inpUsername.IsEnabled = true;
            inpPassword.IsEnabled = true;
            inpUsername.BackgroundColor = Colors.AliceBlue;
            inpPassword.BackgroundColor = Colors.AliceBlue;
            btnLogout.IsVisible = false;
        }
        else
        {
            inpUsername.IsEnabled = false;
            inpPassword.IsEnabled = false;
            inpUsername.BackgroundColor = Colors.LightGray;
            inpPassword.BackgroundColor = Colors.LightGray;
            btnLogout.IsVisible = true;
        }

        //disable login button before verify
        btnLogin.IsEnabled = false;
        btnLogin.BackgroundColor = Colors.LightGray;
        btnLogin.BorderColor = Colors.Gray;
        btnLoginBio.BackgroundColor = Colors.LightGray;
        btnLoginBio.BorderColor = Colors.Gray;
        lblErrorMessage.Text = "";

        //preload identifier
        try
        {
            getIdentifier = await SecureStorage.GetAsync("login_identifier") ?? "";
            try
            {
                getCompany = await SecureStorage.GetAsync("login_company") ?? "";
            }
            catch { getCompany = ""; }
        }
        catch { getIdentifier = ""; getCompany = ""; }
        lblCompanyName.Text = getCompany;

        //verify identifier
        await checkIdentifier();

    }

    async Task checkIdentifier()
    {
        lblErrorMessage.Text = "Verifying...";
        if (string.IsNullOrEmpty(getIdentifier) || string.IsNullOrWhiteSpace(getIdentifier))
        {
            //identifier not blank
            lblErrorMessage.Text = "Company Identifier Not Found.";
            await DisplayAlert("Message", "Company Identifier not found.", "OK");
            (this.Parent as TabbedPage).CurrentPage = (this.Parent as TabbedPage).Children[1];
        }
        else
        {
            try
            {
                var results = await _Function.jsonCheckIndentifier(getIdentifier);
                if (results.Item1 == "Success")
                {
                    //((TabbedPage)this.Parent).Children[0].IsEnabled = true;

                    btnLogin.IsEnabled = true;
                    btnLogin.BackgroundColor = Colors.LightBlue;
                    btnLogin.BorderColor = Colors.Blue;
                    btnLoginBio.BackgroundColor = Colors.LightBlue;
                    btnLoginBio.BorderColor = Colors.Blue;
                    lblErrorMessage.Text = "";

                    // Retrieve the stored switch state
                    //bool isBiometricEnabled = Preferences.Get("IsBiometricEnabled", false);
                    bool isBiometricEnabled = false;
                    if (await SecureStorage.GetAsync("IsBiometricEnabled") == "True")
                    {
                        isBiometricEnabled = true;
                    }

                    if (isBiometricEnabled)
                    {
                        lineLoginBio.IsVisible = isBiometricEnabled;
                        btnLoginBio.IsEnabled = isBiometricEnabled;          //btnLoginBio.IsEnabled = true
                        btnLoginBio.IsVisible = isBiometricEnabled;
                    }
                    else
                    {
                        lineLoginBio.IsVisible = isBiometricEnabled;
                        btnLoginBio.IsEnabled = isBiometricEnabled;          //btnLoginBio.IsEnabled = false
                        btnLoginBio.IsVisible = isBiometricEnabled;
                    }
                }
                else
                {
                    if (results.Item1 == "Maintenance")
                    {
                        //timeout
                        lblErrorMessage.Text = results.Item2;
                        await DisplayAlert(results.Item1, results.Item2, "OK");
                        //(this.Parent as TabbedPage).CurrentPage = (this.Parent as TabbedPage).Children[1];
                    }
                    else
                    {
                        //invalid identifier
                        lblErrorMessage.Text = "Invalid Company Identifier.";
                        await DisplayAlert("Message", "Invalid Company Identifier.", "OK");
                        (this.Parent as TabbedPage).CurrentPage = (this.Parent as TabbedPage).Children[1];
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    //[Obsolete]
    private void inpUsername_Completed(object sender, EventArgs e)
    {
        inpPassword.Focus();
    }
    private void inpPassword_Completed(object sender, EventArgs e)
    {
        if (btnLogin.IsEnabled == true)
        {
            loginclick();
        }
    }
    private void btnLogin_Clicked(object sender, EventArgs e)
    {
        loginclick();
    }
    private async void loginclick()
    {
        if (string.IsNullOrEmpty(inpUsername.Text) || string.IsNullOrWhiteSpace(inpUsername.Text))
        {
            lblErrorMessage.Text = "Username cannot be blank.";
            await DisplayAlert("", lblErrorMessage.Text, "OK");
            return;
        }
        else
        {
            Regex r = new Regex("^[a-zA-Z0-9]*$");
            if (r.IsMatch(inpUsername.Text) == false)
            {
                lblErrorMessage.Text = "Username cannot have symbols.";
                await DisplayAlert("", lblErrorMessage.Text, "OK");
                return;
            }
        }
        if (string.IsNullOrEmpty(inpPassword.Text) || string.IsNullOrWhiteSpace(inpPassword.Text))
        {
            lblErrorMessage.Text = "Password cannot be blank";
            await DisplayAlert("", lblErrorMessage.Text, "OK");
            return;
        }
        else
        {
            Regex r = new Regex("^[a-zA-Z0-9]*$");
            if (r.IsMatch(inpPassword.Text) == false)
            {
                lblErrorMessage.Text = "Password cannot have symbols.";
                await DisplayAlert("", lblErrorMessage.Text, "OK");
                return;
            }
        }

        try
        {
            areaLoading.IsVisible = true;

            //check login
            var results = await _Function.jsonCheckLogin(inpUsername.Text, inpPassword.Text);
            if (results.Item1 == "Success")
            {
                await SecureStorage.SetAsync("login_username", inpUsername.Text.ToUpper());
                await SecureStorage.SetAsync("login_password", inpPassword.Text);

                //update biometric password when needed
                try
                {
                    string bioUser = await SecureStorage.GetAsync("UserSetBio") ?? "";
                    string bioPass = await SecureStorage.GetAsync("UserPassSetBio") ?? "";
                    if (bioUser.ToUpper() == inpUsername.Text.ToUpper() && bioPass != inpPassword.Text)
                    {
                        await SecureStorage.SetAsync("UserPassSetBio", inpPassword.Text);
                        await DisplayAlert("", "Biometric login password updated", "OK");
                    }
                }
                catch { }

                //Preferences.Set("LoginMethod", "Password");
                //Preferences.Set("SettingDisplayCheck", true);
                await SecureStorage.SetAsync("LoginMethod", "Password");
                await SecureStorage.SetAsync("SettingDisplayCheck", "True");
                //await Navigation.PushAsync(new FlyoutPage1());
                Application.Current.MainPage = new AppShell_Main();
            }
            else
            {
                inpPassword.Text = "";
                lblErrorMessage.Text = "Invalid Username or Password.";
                await DisplayAlert("", lblErrorMessage.Text, "OK");

                if (inpUsername.IsEnabled == false)
                {
                    SecureStorage.Remove("login_username");
                    SecureStorage.Remove("login_password");
                    //Xamarin.Essentials.SecureStorage.Remove("inpData");

                    await DisplayAlert("", "You have successfully logged out.", "OK");
                    //await Navigation.PushAsync(new Tabbed_Login());
                    Application.Current.MainPage = new Tabbed_Login();
                }
            }
        }
        finally
        {
            areaLoading.IsVisible = false;
        }

    }

    private async void btnBioLogin_Clicked(object sender, EventArgs e)
    {

        try
        {
            areaLoading.IsVisible = true;

            string usernameSet = await SecureStorage.GetAsync("UserSetBio") ?? "";
            string password = await SecureStorage.GetAsync("UserPassSetBio") ?? "";

            if (usernameSet != null && usernameSet != "" )
            {
                //Preferences.Set("LoginMethod", "Bio");
                await SecureStorage.SetAsync("LoginMethod", "Bio");

                string authenticate = await CheckFpAuthentication(usernameSet, password);

                if (authenticate == "Success")
                {
                    //await Navigation.PushAsync(new FlyoutPage1());
                    Application.Current.MainPage = new AppShell_Main();
                    //Preferences.Set("SettingDisplayCheck", false);
                    await SecureStorage.SetAsync("SettingDisplayCheck", "False");
                }
                else
                {
                    if (authenticate == "Authentication failed")
                    {
                        await DisplayAlert(authenticate, "Please re-login with password.", "OK");
                        //string ans = await DisplayPromptAsync(authenticate, "Re-Enter Password :","Submit");
                        //var results = await _Function.jsonCheckLogin(usernameSet, ans);
                        //if (results.Item1 == "Success")
                        //{
                        //    await Xamarin.Essentials.SecureStorage.SetAsync("UserPassSetBio", ans);
                        //    await Navigation.PushAsync(new FlyoutPage1());
                        //    Preferences.Set("SettingDisplayCheck", false);
                        //}
                        //else
                        //{
                        //    // Authentication successful but login failed
                        //    await DisplayAlert(authenticate, "Invalid password.", "OK");
                        //}
                    }
                    else
                    {
                        await DisplayAlert(authenticate, "Please login with password.", "OK");
                    }
                }
            }
            else
            {
                await DisplayAlert("Biometic login is disabled", "Please login with password.", "OK");
            }

        }
        finally
        {
            areaLoading.IsVisible = false;
        }
    }

    private async void lblLogout_Tapped(object sender, EventArgs e)
    {
        bool Ans = await DisplayAlert("", "Are you sure want to logout current user?", "Logout", "No");
        if (Ans == true)
        {
            SecureStorage.Remove("login_username");
            SecureStorage.Remove("login_password");
            //Xamarin.Essentials.SecureStorage.Remove("inpData");

            await DisplayAlert("", "You have successfully logged out.", "OK");
            //await Navigation.PushAsync(new Tabbed_Login());
            Application.Current.MainPage = new Tabbed_Login();
        }
    }

    //use for checking the fingerprint after set up the fingerprint
    private async Task<string> CheckFpAuthentication(string bioUser, string bioPass)
    {
        //var request = new AuthenticationRequestConfiguration("Authenticate to login to your account.", "");
        //var request = new AuthenticationRequestConfiguration("Authentication request", $"Biometric Login For {bioUser}");
        //var result = await CrossFingerprint.Current.AuthenticateAsync(request); //result.Authenticated
        var request = new AuthenticationRequest() { Title = "Authentication request", Description = $"Biometric Login For {bioUser}", NegativeText = "Cancel", AllowPasswordAuth = false };
        var result = await BiometricAuthenticationService.Default.AuthenticateAsync(request, CancellationToken.None); //result.Status == BiometricResponseStatus.Success

        if (result.Status == BiometricResponseStatus.Success)
        {
            //check bio login
            var results = await _Function.jsonCheckLogin(bioUser, bioPass);
            if (results.Item1 == "Success")
            {
                // Authentication successful
                return "Success";
            }
            else
            {
                // Authentication successful but login failed
                return "Authentication failed";
            }
        }
        else
        {
            // Authentication failed
            return "Biometric not recognized";
        }
    }

}