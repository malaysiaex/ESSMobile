using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml;
//using Plugin.Fingerprint;
//using Plugin.Fingerprint.Abstractions;
using Plugin.Maui.Biometric;
using System.Runtime.InteropServices;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Storage;

namespace ESSMobile.Page;

public partial class Setting : ContentPage
{
    //public readonly IBiometric biometric;
	public Setting()
	{
        InitializeComponent();
        //InitializeSettings();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        InitializeSettings();
    }

    private async void InitializeSettings()
    {
        //bool settingDisplay = Preferences.Get("SettingDisplayCheck", false);
        bool settingDisplay = false;
        if (await SecureStorage.GetAsync("SettingDisplayCheck") == "True")
        {
            settingDisplay = true;
        }

        string usernameSet = await SecureStorage.GetAsync("UserSetBio") ?? "";
        string passwordSet = await SecureStorage.GetAsync("UserPassSetBio") ?? "";
        string username = await SecureStorage.GetAsync("login_username") ?? "";
        //bool isCheck = Preferences.Get("IsBiometricEnabled", true);
        bool isCheck = true;
        if (await SecureStorage.GetAsync("IsBiometricEnabled") == "False")
        {
            isCheck = false;
        }

        if (settingDisplay == true)
        {
            if (isCheck)
            {
                if (username == usernameSet || usernameSet == null || usernameSet == "")
                {
                    enableBiometricLabel.IsVisible = true;
                    Switch.IsVisible = true;
                    //enableBiometricLabel.IsEnabled = true;
                    //Switch.IsEnabled = true;

                    //bool isBiometricEnabled = Preferences.Get("IsBiometricEnabled", false);
                    bool isBiometricEnabled = false;
                    if (await SecureStorage.GetAsync("IsBiometricEnabled") == "True")
                    {
                        isBiometricEnabled = true;
                    }
                    Switch.IsToggled = isBiometricEnabled;
                }
                else
                {
                    enableBiometricLabel.IsVisible = false;
                    Switch.IsVisible = false;
                    await DisplayAlert("", "Biometric login is already enabled for another user. Please disable it first.", "OK");
                }
            }
            else
            {
                enableBiometricLabel.IsVisible = true;
                Switch.IsVisible = true;
                //enableBiometricLabel.IsEnabled = true;
                //Switch.IsEnabled = true;
            }

        }
        else
        {
            enableBiometricLabel.IsVisible = true;
            Switch.IsVisible = true;
            //enableBiometricLabel.IsEnabled = true;
            //Switch.IsEnabled = true;

            //bool isBiometricEnabled = Preferences.Get("IsBiometricEnabled", false);
            bool isBiometricEnabled = false;
            if (await SecureStorage.GetAsync("IsBiometricEnabled") == "True")
            {
                isBiometricEnabled = true;
            }
            Switch.IsToggled = isBiometricEnabled;
        }

    }


    private async void Switch_Toggled(object sender, EventArgs e)
    {
        bool isCheck = Switch.IsToggled;     //return true or false
        if (isCheck)
        {

            //await DisplayAlert("Debug", "CheckFpAvailableAsync()", "OK");
            bool available = await CheckFpAvailableAsync();
            if (!available)
            {
                SecureStorage.Remove("UserSetBio");
                SecureStorage.Remove("UserPassSetBio");
                SecureStorage.Remove("IsBiometricEnabled");
                SecureStorage.Remove("Actived");
                await DisplayAlert("", "Device does not support biometirc login.", "Undo");
                Switch.IsToggled = false;
            }
            else
            {
                //string actived = Preferences.Get("Actived", "No");
                string actived = "No";
                if (await Microsoft.Maui.Storage.SecureStorage.GetAsync("Actived") == "Yes")
                {
                    actived = "Yes";
                }
            
                if (actived != "Yes")
                {
                    bool authenticate = await CheckFpAuthentication();
                    if (authenticate == true)
                    {
                        string username = await SecureStorage.GetAsync("login_username") ?? "";
                        string password = await SecureStorage.GetAsync("login_password") ?? "";
            
                        //store the username which user turned on the biometric feature
                        await SecureStorage.SetAsync("UserSetBio", username);
                        await SecureStorage.SetAsync("UserPassSetBio", password);
                        await SecureStorage.SetAsync("IsBiometricEnabled", "True");
                        await SecureStorage.SetAsync("Actived", "Yes");
                        await DisplayAlert("", "Biometic login enabled.", "OK");
                    }
                    else
                    {
                        SecureStorage.Remove("UserSetBio");
                        SecureStorage.Remove("UserPassSetBio");
                        SecureStorage.Remove("IsBiometricEnabled");
                        SecureStorage.Remove("Actived");
                        await DisplayAlert("", "Failed to unable biometic login.", "Undo");
                        Switch.IsToggled = false;
                    }
                }
            }          
        }
        else
        {
            SecureStorage.Remove("UserSetBio");
            SecureStorage.Remove("UserPassSetBio");
            SecureStorage.Remove("IsBiometricEnabled");
            SecureStorage.Remove("Actived");
        }
    }

    private async Task<Boolean> CheckFpAvailableAsync()
    {


        //var availability = await CrossFingerprint.Current.GetAvailabilityAsync(true); //FingerprintAvailability.Available
        var availability = await BiometricAuthenticationService.Default.GetAuthenticationStatusAsync();  //BiometricHwStatus.Success

        if (availability == BiometricHwStatus.Success)
        {
            // Fingerprint is available, you can use it.
            //Console.WriteLine("Fingerprint for this mobile device is available.");
            return true;
        }
        else
        {
            // Fingerprint is not available or not configured.
            //await DisplayAlert("", "Fingerprint is not available in your mobile", "OK");
            return false;
        }       

    }

    private async Task<bool> CheckFpAuthentication()
    {
        //var request = new AuthenticationRequestConfiguration("Authentication request", "Enable biometric login method.");
        //var result = await CrossFingerprint.Current.AuthenticateAsync(request); //result.Authenticated

        var request = new AuthenticationRequest() { Title = "Authentication request", Description = "Enable biometric login method.", NegativeText = "Cancel", AllowPasswordAuth = false };
        var result = await BiometricAuthenticationService.Default.AuthenticateAsync(request,CancellationToken.None); //result.Status == BiometricResponseStatus.Success

        if (result.Status == BiometricResponseStatus.Success)
        {
            // Authentication successful
            //return "Success Authenticate.";
            return true;
        }
        else
        {
            // Authentication failed
            //return "Authentication failed.";
            return false;
        }

    }
}