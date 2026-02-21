using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Xaml;

using ESSMobile.Shared;
using System.Net.Http;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;

namespace ESSMobile.Page;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CompanyIdentifier : ContentPage
{
    string getIdentifier = "";
    string getCompany = "";

    public CompanyIdentifier()
	{
		InitializeComponent();
        lblVersion.Text = AppInfo.VersionString;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await getLocalIdentifier();
    }

    async Task getLocalIdentifier()
    {
        //btnReset.IsVisible = false;
        getIdentifier = "";
        getCompany = "";
        lblCompanyName.Text = "";

        try
        {
            getIdentifier = await SecureStorage.GetAsync("login_identifier") ?? "";

            if (String.IsNullOrEmpty(getIdentifier) == false && String.IsNullOrWhiteSpace(getIdentifier) == false)
            {
                inpCompanyIdentifierID.Text = getIdentifier;
                //await checkIdentifier(getIdentifier);
            }
        }
        catch { }
    }

    private async void btnSet_Clicked(object sender, EventArgs e)
    {
        getIdentifier = inpCompanyIdentifierID.Text;
        getCompany = "";
        lblCompanyName.Text = "";

        //if (inpCompanyIdentifierID.Text != "" )
        if (string.IsNullOrEmpty(getIdentifier) || string.IsNullOrWhiteSpace(getIdentifier))
        {
            await DisplayAlert("Error", "Invalid Company Identifier.", "OK");
        }
        else
        {
            try
            {
                areaLoading.IsVisible = true;

                await checkIdentifier(getIdentifier);

                //await DisplayAlert("identifier/company", getIdentifier + "/" + getCompany, "TEST");
                if (string.IsNullOrEmpty(getCompany) == false && string.IsNullOrWhiteSpace(getCompany) == false)
                {
                    //await Xamarin.Essentials.SecureStorage.SetAsync("login_identifier", inpCompanyIdentifierID.Text);
                    SecureStorage.Remove("login_username");
                    SecureStorage.Remove("login_password");
                    SecureStorage.Remove("inpData");                    //for enable and disable the input field
                    await DisplayAlert("Success!", lblCompanyName.Text, "Go To Login");
                    (this.Parent as TabbedPage).CurrentPage = (this.Parent as TabbedPage).Children[0];
                }
            }
            //catch (TaskCanceledException)
            //{
            //    await DisplayAlert("Error", "Server is offline at the moment, please try again later.", "OK");
            //}
            //catch (Exception ex)
            //{
            //    await DisplayAlert("Error", ex.Message, "OK");
            //}
            finally
            {
                areaLoading.IsVisible = false;
            }
        }
    }

    async Task checkIdentifier(string sCompanyIdentifier)
    {
        try
        {
            var results = await ApiFunctions.jsonCheckIndentifier(sCompanyIdentifier);
            if (results.Item1 == "Success")
            {
                //btnReset.IsVisible = true;
                await SecureStorage.SetAsync("login_identifier", sCompanyIdentifier);
                getCompany = await SecureStorage.GetAsync("login_company") ?? "";
                lblCompanyName.Text = getCompany;
            }
            else
            {
                await DisplayAlert(results.Item1, results.Item2, "OK");
            }
        }
        catch (Exception ex)
        {
            //await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void btnReset_Clicked(object sender, EventArgs e)
    {
        bool ans = await DisplayAlert("Question", "Do you want to reset application data?", "Reset", "No");
        if (ans == true)
        {
            try
            {
                areaLoading.IsVisible = true;

                getIdentifier = "";
                inpCompanyIdentifierID.Text = "";
                getCompany = "";
                lblCompanyName.Text = "";

                SecureStorage.RemoveAll();
                //Preferences.Clear();

                await DisplayAlert("Message", "Application data has beed reset successfully.", "OK");
            }
            catch { }
            finally
            {
                areaLoading.IsVisible = false;
            }
        }
    }

}