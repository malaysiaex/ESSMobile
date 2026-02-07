using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System.Data.SqlClient;
using SkiaSharp;
using Microsoft.Maui.Controls.Xaml;
using System.Web;
//using SkiaSharp.Views.Maui.Controls;
//using SkiaSharp.Views.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;

namespace ESSMobile.Page;

public partial class WebContent : ContentPage
{
    string BaseUrl = "";
    //string BaseUrl = "http://exacom.com.my:6221";
    //string BaseUrl = "https://exacom.com.my:6222";
    //string BaseUrl = "http://exacom.koreacentral.cloudapp.azure.com:6221"; 
    //string BaseUrl = "https://exacom.koreacentral.cloudapp.azure.com:6222"; 

    static string JsonUrl = "https://www.exacom.com.my/files/master_url.json";
    public static async Task<string> GetValueByKeyAsync(string key)
    {
        try
        {
            HttpClient masterClient = new HttpClient();
            var json = await masterClient.GetStringAsync(JsonUrl);
            var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            if (data != null && data.TryGetValue(key, out string value))
            {
                return value;
            }
            return null; // Key not found
        }
        catch (Exception ex)
        {
            // Optional: log or handle errors
            return null;
        }
    }

    public WebContent()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            areaLoading.IsVisible = true;

            //Get username either via password or biometric login
            _ = GetName();


            // Remove first if already attached
            myWebView1.Navigating -= MyWebView1_Navigating;
            myWebView1.Navigated -= MyWebView1_Navigated;

            // Now attach
            myWebView1.Navigating += MyWebView1_Navigating;
            myWebView1.Navigated += MyWebView1_Navigated;
        }
        catch { }
        finally
        {
            areaLoading.IsVisible = false;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        myWebView1.Navigating -= MyWebView1_Navigating;
        myWebView1.Navigated -= MyWebView1_Navigated;
    }

    private void MyWebView1_Navigating(object sender, WebNavigatingEventArgs e)
    {
        areaLoading.IsVisible = true;
    }

    private async void MyWebView1_Navigated(object sender, WebNavigatedEventArgs e)
    {
        try
        {
            // Get the URL from the WebView
            var url = e.Url;

            // Use Uri class to parse the URL and extract parameters
            var uri = new Uri(url);

            // Retrieve the value of the "Mode" and "LoginMethod" parameter
            var modeParameter = System.Web.HttpUtility.ParseQueryString(uri.Query)["Mode"];
            var LogMethod = System.Web.HttpUtility.ParseQueryString(uri.Query)["LoginMethod"];

            if (!string.IsNullOrEmpty(modeParameter))
            {
                if (modeParameter == "eAttendance") //old mode=SignAttendance
                {
                    await Navigation.PushAsync(new ESS_eAttendance());
                    areaLoading.IsVisible = false;
                }
                else
                {
                    areaLoading.IsVisible = false; //unknown mode
                }
            }
            else
            {
                areaLoading.IsVisible = false; //empty mode
            }

        } 
        catch
        {
            areaLoading.IsVisible = false; //error
        }

    }

    public async Task GetName()
    {

        //string LoginMethod = Preferences.Get("LoginMethod", "Bio");
        string LoginMethod = await SecureStorage.GetAsync("LoginMethod") ?? "";

        BaseUrl = await GetValueByKeyAsync("ESSMobile_WebPage");
        // TEMP hard code the mobile web page locally later

        if (LoginMethod != "Bio")
        {
            string getUsername = await SecureStorage.GetAsync("login_username") ?? "";
            string getIdentifier = await SecureStorage.GetAsync("login_identifier") ?? "";

            myWebView1.Source = $"{BaseUrl.ToString().Trim()}/Page/Dashboard.aspx?Identifier={getIdentifier.ToString().Trim()}&UserID={getUsername.ToString().Trim()}&LoginMethod={LoginMethod.ToString().Trim()}";            
        }
        else
        {
            string getUsername = await SecureStorage.GetAsync("UserSetBio") ?? "";
            string getIdentifier = await SecureStorage.GetAsync("login_identifier") ?? "";

            myWebView1.Source = $"{BaseUrl.ToString().Trim()}/Page/Dashboard.aspx?Identifier={getIdentifier.ToString().Trim()}&UserID={getUsername.ToString().Trim()}&LoginMethod={LoginMethod.ToString().Trim()}";
        }

    }


}