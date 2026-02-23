using ESSMobile.Shared;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
//using Plugin.Fingerprint;
using Plugin.Maui.Biometric;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
//using System.Data.SqlClient;
//using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.XPath;
using static System.Net.WebRequestMethods;
using ESSMobile.Models;
using Microsoft.Maui.Layouts;
using GeoTimeZone;
namespace ESSMobile.Page;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ESS_eAttendance : ContentPage
{
    string getIdentifier = "";
    string getUsername = "";
    string getConString = "";
    string LoginMethod = "";
    // time between clocks in mins
    long minTimeBetweenClocks = 5;
    bool _permissionGeo = false;
    bool _permissionCam = false;
    // system timers are not used anymore, replaced with dispatcher 
    System.Timers.Timer _timerNow = new System.Timers.Timer(1000);
    System.Timers.Timer _timerGeo = new System.Timers.Timer(5000);
    int _timerNowTicks = 0;
    bool validGeolocation = false;
    bool clockedRecently = true;
    bool nearWorkplace = false;
    bool withinWorkplaceTimeWindow = false;
    DateTime _serverTime = DateTime.Now;
    DateTime _utcTime;
    DateTime _lastRecord = DateTime.MinValue;
    // concatenation of lat/long into one string
    string _userLocationStr = "";
    Location? _userLocation = null;
    string _userAddress = "";
    string _picBase64 = "";
    CompanyLocation selectedCompanyLocation; 
    List<CompanyLocation>? companyLocations = new List<CompanyLocation>();
    private bool _geoBusy;




    bool _timer1 = true;
    bool _disableTakePhoto = false;

    string JsonValue = "";
    static string JsonUrl = "https://www.exacom.com.my/files/master_url.json";

    //[Obsolete]
    public ESS_eAttendance()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Title = "e-Attendance";

        JsonValue = ApiInitializer.ESSMobile_DisableTakePhoto;
        if (JsonValue.ToString().Trim().ToUpper() == "TRUE")
        {
            areaPhoto.IsVisible = false;
            _disableTakePhoto = true;
        } else
        {
            areaPhoto.IsVisible = true;
            _disableTakePhoto = false;
        }

        //_timerNow.Elapsed -= _timerNow_OnTimedEvent; //remove event
        //_timerNow.Elapsed += _timerNow_OnTimedEvent; //trigger event
        //_timerNow.Start();

        //_timerGeo.Elapsed -= _timerGeo_OnTimedEvent; //remove event
        //_timerGeo.Elapsed += _timerGeo_OnTimedEvent; //trigger event
        //_timerGeo.Start();

        areaLoading.IsVisible = true;
        try
        {
            await Task.Delay(500);
            await Preload(); //company identifier, user login, etc from storage
            await CheckPermissions();//GPS, camera, etc
            var utcTimeTask = GetUtcTime();
            var serverTimeTask = GetServerTime();
            var lastClockingTask = GetLastClocking();
            var companyLocationsTask = GetCompanyLocations();
            await Task.WhenAll(serverTimeTask, lastClockingTask, companyLocationsTask, utcTimeTask);
            //await getServerTime(); //read server time instead of localtime
            //await getLastClocking(); //read user's last clocking time
            //await getCompanyLocations(); // read lat long of company from storage

            // initial geolocation retrieval call
            await UpdateGeolocationAsync();

            // start timers
            StartGeoTimer();
            StartClockTimer();

            //_timerNow.Elapsed -= _timerNow_OnTimedEvent; //remove event
            //_timerNow.Elapsed += _timerNow_OnTimedEvent; //trigger event
            //_timerNow.Start();

            //_timerGeo.Elapsed -= _timerGeo_OnTimedEvent; //remove event
            //_timerGeo.Elapsed += _timerGeo_OnTimedEvent; //trigger event
            //_timerGeo.Start();
        }
        finally
        {
            await Task.Delay(500);
            areaLoading.IsVisible = false;
        }

        //_timerNow.Elapsed += _timerNow_OnTimedEvent; //trigger event
        //_timerNow.Start();
        //
        //if (_permissionGeo == true)
        //{
        //    _timerGeo.Elapsed += _timerGeo_OnTimedEvent; //trigger event
        //    _timerGeo.Start();            
        //}
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        //_timerNow.Stop();
        //_timerGeo.Stop();
    }
    private void StartGeoTimer()
    {
        Dispatcher.StartTimer(TimeSpan.FromSeconds(5), () =>
        {
            if (_geoBusy)
                return true;

            _geoBusy = true;

            try
            {
                _ = UpdateGeolocationAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                _geoBusy = false;
            }

            return true;
        });
    }
    private void StartClockTimer()
    {
        Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            UpdateClockUi();
            return true;
        });
    }

    /// <summary>
    /// converts single coordinate to a string
    /// </summary>
    /// <param name="coordinate"></param>
    /// <param name="decimals"></param>
    /// <returns></returns>
    public static string CoordToString(double coordinate, int decimals = 6)
    {
        return coordinate.ToString($"F{decimals}", CultureInfo.InvariantCulture);
    }
    /// <summary>
    /// converts string lat or long to double, null if fails
    /// </summary>
    /// <param name="LatOrLong"></param>
    /// <returns></returns>
    private double? StringCoordToDouble(string LatOrLong)
    {
        double? convertedCoord = null;
        if (!string.IsNullOrWhiteSpace(LatOrLong) &&
            double.TryParse(LatOrLong, NumberStyles.Float, CultureInfo.InvariantCulture, out double lat))
        {
            convertedCoord = lat;
        }
        return convertedCoord;
    }


    /// <summary>
    /// Retrieves company locations.
    /// If failed, return user to home page and display error.
    /// </summary>
    /// <returns></returns>
    private async Task GetCompanyLocations()
    {
        try
        {
            companyLocations = await ApiFunctions.APIGetCompanyLocations(getIdentifier, getUsername);
            // disable top box if null meaning db does not exist
            if (companyLocations == null)
            {
                SetRowVisible(false, lblSelectLocation, CompanyPicker);
            }
            else
            {
                SetRowVisible(true, lblSelectLocation, CompanyPicker);
                CompanyPicker.ItemsSource = companyLocations;
                CompanyPicker.SelectedIndexChanged += CompanyPicker_SelectedIndexChanged;
                CompanyPicker.ItemDisplayBinding = new Binding("Name");
            }
        }
        catch(Exception ex)
        {
            //await DisplayAlert("e-Attendance", $"Error retrieving company locations\n{ex.Message}", "Return");
            await DisplayAlert("e-Attendance", $"Error retrieving company locations\n{ex.ToString()}", "Return");
            await Navigation.PopAsync();
        }
    }
    private async Task Preload()
    {
        //LoginMethod = Preferences.Get("LoginMethod", "");
        LoginMethod = await SecureStorage.GetAsync("LoginMethod") ?? "";
        getIdentifier = await SecureStorage.GetAsync("login_identifier") ?? "";
        if (LoginMethod != "Bio")
        {
            getUsername = await SecureStorage.GetAsync("login_username") ?? "";
        }
        else
        {
            getUsername = await SecureStorage.GetAsync("UserSetBio") ?? "";
        }
        getConString = await SecureStorage.GetAsync("login_constring") ?? "";
    }
    private async Task CheckPermissions()
    {
        PermissionStatus status;

        //Geolocation
        status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                //permission denied.
                _permissionGeo = false;
                lblMessage.BackgroundColor = Colors.Red;
                lblMessage.Text = "GPS Permission Denied";
                await DisplayAlert("e-Attendance", "GPS Permission Denied.", "Return");
                await Navigation.PopAsync();
            }
            else
            {
                _permissionGeo = true;
                lblMessage.BackgroundColor = Colors.DarkOrange;
                lblMessage.Text = "Retrieving GPS Location...";
                //await DisplayAlert("Geolocation", "Permission granted", "OK");
            }
        }
        else
        {
            _permissionGeo = true;
            lblMessage.BackgroundColor = Colors.DarkOrange;
            lblMessage.Text = "Retrieving GPS Location...";
            //await DisplayAlert("Geolocation", "Permission already granted", "OK");
        }

        //Camera
        status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                //permission denied.
                _permissionCam = false;
                //await DisplayAlert("Camera", "Permission denied.", "OK");
            }
            else
            {
                _permissionCam = true;
                //await DisplayAlert("Camera", "Permission granted", "OK");
            }
        }
        else
        {
            _permissionCam = true;
            //await DisplayAlert("Camera", "Permission already granted", "OK");
        }

    }

    private async Task GetUtcTime()
    {
        try
        {
            _utcTime = await ApiFunctions.APIGetServerUtcTime();
        }
        catch(Exception ex)
        {
            await DisplayAlert("Error retrieving utc time", ex.Message, "OK");
            await Navigation.PopAsync();
        }
    }
    private async Task GetServerTime()
    {
        try
        {
            var results = await ApiFunctions.jsonGetServerTime(getIdentifier);
            if (results.Item1 == "Success")
            {
                DateTime rebuildTime = new DateTime(
                    int.Parse(results.Item2.ToString().Substring(6, 4)),
                    int.Parse(results.Item2.ToString().Substring(3, 2)),
                    int.Parse(results.Item2.ToString().Substring(0, 2)),
                    int.Parse(results.Item2.ToString().Substring(11, 2)),
                    int.Parse(results.Item2.ToString().Substring(14, 2)),
                    int.Parse(results.Item2.ToString().Substring(17, 2)));
                _serverTime = rebuildTime;
            }
            else
            {
                _serverTime = DateTime.Now;
                await DisplayAlert("Sync server time failed", "Please try again", "OK");
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            //await DisplayAlert("Error", ex.Message, "OK");
            _serverTime = DateTime.Now;
            //await DisplayAlert("Sync server time failed", "Please try again", "OK");
            await DisplayAlert("Error", ex.Message, "OK");
            await Navigation.PopAsync();
        }

        //update datetime and ticks counts
        lblTimeNow.Text = _serverTime.ToString("dd/MM/yyyy HH:mm:ss");
        _timerNowTicks = 0;
    }

    private async Task GetLastClocking()
    {
        try
        {
            var results = await ApiFunctions.jsonGetLastClocking(getIdentifier, getUsername);
            if (results.Item1 == "Success")
            {
                DateTime rebuildTime = new DateTime(
                    int.Parse(results.Item2.ToString().Substring(6, 4)),
                    int.Parse(results.Item2.ToString().Substring(3, 2)),
                    int.Parse(results.Item2.ToString().Substring(0, 2)),
                    int.Parse(results.Item2.ToString().Substring(11, 2)),
                    int.Parse(results.Item2.ToString().Substring(14, 2)),
                    int.Parse(results.Item2.ToString().Substring(17, 2)));
                _lastRecord = rebuildTime;
                lblLastRecord.Text = $"Last Clocking Time: {_lastRecord.ToString("dd/MM/yyyy HH:mm")}";
            }
            else
            {
                _lastRecord = DateTime.MinValue;
                lblLastRecord.Text = "";
            }
        }
        catch (Exception ex)
        {
            _lastRecord = DateTime.MinValue;
            lblLastRecord.Text = "";
        }
    }

    //[Obsolete]
    private void _timerNow_OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
    {
        //Get total ticks count (time)
        _timerNowTicks += 1;

        //Update UI
        MainThread.BeginInvokeOnMainThread(() => {
            lblTimeNow.Text = _serverTime.AddSeconds(_timerNowTicks).ToString("dd/MM/yyyy HH:mm:ss");
            // check if last clock in exists
            // if clocked recently make submit button invisible
            clockedRecently = false;
            if (lblLastRecord.Text != ""){
                TimeSpan timeDiff = _serverTime.AddSeconds(_timerNowTicks) - _lastRecord;   
                TimeSpan timeBals = TimeSpan.FromMinutes(minTimeBetweenClocks) - timeDiff;
                if (timeBals.TotalMilliseconds > 0) {
                    lblCountDown.Text = $"{timeBals.Minutes} minutes {timeBals.Seconds} seconds";
                    clockedRecently = true;
                }
            }
            // if no clocking within last 5 minutes, make submit button visible
            if(!clockedRecently){
                lblCountDown.Text = "0 minutes 0 seconds";
            }
            RefreshCompanyUI();
        });
    }
    // return address string given lat long
    private async Task<string> GetAddressAsync(double latitude, double longitude) {
        string address = "";
        var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
        var placemark = placemarks?.FirstOrDefault();
        if (placemark != null)
        {
            try
            {
                //street
                if (placemark.Thoroughfare.ToString().Trim() != "")
                {
                    address = $"{placemark.Thoroughfare}";
                }
            }
            catch { }
            try
            {
                //area
                if (placemark.SubLocality.ToString().Trim() != "")
                {
                    if (address != "") { address += ", "; }
                    address += $"{placemark.SubLocality}";
                }
            }
            catch { }
            try
            {
                //postcode
                if (placemark.PostalCode.ToString().Trim() != "")
                {
                    if (address != "") { address += ", "; }
                    address += $"{placemark.PostalCode}";
                }
            }
            catch { }
            try
            {
                //city
                if (placemark.Locality.ToString().Trim() != "")
                {
                    if (address != "") { address += " "; }
                    address += $"{placemark.Locality}";
                }
            }
            catch { }
            try
            {
                //state
                if (placemark.AdminArea.ToString().Trim() != "")
                {
                    if (address != "") { address += " "; }
                    address += $"{placemark.AdminArea}";
                }
            }
            catch { }
            try
            {
                //country
                if (placemark.CountryName.ToString().Trim() != "")
                {
                    if (address != "") { address += " "; }
                    address += $"{placemark.CountryName}";
                }
            }
            catch { }

            //_geoAddress =
            //    $"{placemark.Thoroughfare}, " +
            //    $"{placemark.SubLocality}, " +
            //    $"{placemark.PostalCode} " +
            //    $"{placemark.Locality} " +
            //    $"{placemark.AdminArea} " +
            //    $"{placemark.CountryName}";
            //_geoAddress = _geoAddress.Replace(", ,", ","); //
            //_geoAddress = _geoAddress.Replace("  ", " ");
            //if (_geoAddress.Substring(0, 1) == ",")
            //{
            //    _geoAddress = _geoAddress.Substring(1, _geoAddress.Length - 2).ToString().Trim();
            //}

            //_geoAddress += "\n" +
            //    $"AdminArea:       {placemark.AdminArea}\n" +           //Perak
            //    $"CountryCode:     {placemark.CountryCode}\n" +         //MY
            //    $"CountryName:     {placemark.CountryName}\n" +         //Malaysia
            //    $"FeatureName:     {placemark.FeatureName}\n" +         //12 ?
            //    $"Locality:        {placemark.Locality}\n" +            //Ipoh
            //    $"PostalCode:      {placemark.PostalCode}\n" +          //31400
            //    $"SubAdminArea:    {placemark.SubAdminArea}\n" +        //   ?
            //    $"SubLocality:     {placemark.SubLocality}\n" +         //Taman Ipoh Timur
            //    $"SubThoroughfare: {placemark.SubThoroughfare}\n" +     //12 ?
            //    $"Thoroughfare:    {placemark.Thoroughfare}\n";         //Jalan Medan Ipoh 4

        }
        else
        {
            address = "n/a";
        }
        return address;
    }

    /// <summary>
    /// Updates clock and the timer under the clock-in/out button.
    /// Calls RefreshCompanyUI
    /// </summary>
    private void UpdateClockUi()
    {
        // Get total ticks count (time)
        _timerNowTicks += 1;

        lblTimeNow.Text = _serverTime
            .AddSeconds(_timerNowTicks)
            .ToString("dd/MM/yyyy HH:mm:ss");

        clockedRecently = false;

        if (!string.IsNullOrEmpty(lblLastRecord.Text))
        {
            TimeSpan timeDiff = _serverTime.AddSeconds(_timerNowTicks) - _lastRecord;
            TimeSpan timeBals = TimeSpan.FromMinutes(5) - timeDiff;

            if (timeBals.TotalMilliseconds > 0)
            {
                lblCountDown.Text =
                    $"{timeBals.Minutes} minutes {timeBals.Seconds} seconds";

                clockedRecently = true;
                btnSubmit.IsVisible = false;
            }
        }

        if (!clockedRecently)
        {
            lblCountDown.Text = "0 minutes 0 seconds";
            btnSubmit.IsVisible = true;
        }

        RefreshCompanyUI();
    }

    private async Task UpdateGeolocationAsync()
    {
        //Get geolocationf
        _userLocationStr = "";
        _userAddress = "";

        string _error = "";
        if (_permissionGeo == true)
        {
            try
            {   
                //GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(30));
                    _userLocation = await Geolocation.GetLocationAsync(request, CancellationToken.None);
                    if (_userLocation != null)
                    {
                        validGeolocation = true;
                        _userLocationStr = $"{_userLocation.Latitude}, {_userLocation.Longitude}";
                        try
                        {
                            _userAddress = await GetAddressAsync(_userLocation.Latitude, _userLocation.Longitude);
                        }
                        catch (Exception ex)
                        {
                            //_geoAddress = $"{ex.Message}";
                            _userAddress = "n/a";
                        }

                    }
                    // if location is null
                    else
                    {
                        validGeolocation = false;
                        _userLocationStr = $"Failed to get location";
                        _error = $"{_userLocationStr}";
                    }
                });
            }
            catch (Exception ex)
            {
                // Unable to get location
                _userLocationStr = $"Failed to get location";
                _error = $"{ex.Message}";
                validGeolocation = false;
            }

            //Update UI
            MainThread.BeginInvokeOnMainThread(() => {
                lblGeoLocation.Text = _userLocationStr;
                lblAddress.Text = $"{_userAddress}";
                RefreshCompanyUI();
                if (!string.IsNullOrWhiteSpace(_error)) { 
                    lblMessage.BackgroundColor = Colors.Red;
                    lblMessage.Text = _error;
                }
            });
        }
    }
    //[Obsolete]
    private async void _timerGeo_OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
    {
        await UpdateGeolocationAsync();
    }

    /// <summary>
    /// Sets nearWorkplace. 
    /// Uses validGeolocation, nearWorkplace, recentlyClocked to
    /// refresh the notification at the top and disable/enable the clock-in/out button.
    /// Called on dropdown select, geoLoc retrieval, timer update
    /// </summary>
    private void RefreshCompanyUI()
    {
        if (!validGeolocation)
        {
            lblMessage.BackgroundColor = Colors.Red;
            lblMessage.Text = "Unable to retrieve user coordinates";
            btnSubmit.BackgroundColor = Colors.LightGray;
            btnSubmit.IsEnabled = false;
            return;
        }
        else
        {
            lblMessage.BackgroundColor = Colors.Green;
            lblMessage.Text = "GPS Location Service Running.";
        }

        // ----- nearWorkplace/withinWorkplaceTimeWindow calculation + notification for not near workplace ---------

        //  database does not exist or list is empty
        if (companyLocations == null || companyLocations.Count == 0)
        {
            nearWorkplace = true;
            withinWorkplaceTimeWindow = true;
            layoutCompanyInformation.IsVisible = false;
        }
        // user has been restricted to clock into certain locations
        else
        {
            CompanyLocation? selectedCompany = CompanyPicker.SelectedItem as CompanyLocation;
            SortCompanyLocations();
            // if have not selected a location yet
            if (selectedCompany == null)
            {
                lblMessage.BackgroundColor = Colors.Green;
                lblMessage.Text = "GPS Location Service Running. Please select company location.";
                nearWorkplace = false;
                withinWorkplaceTimeWindow = false;
            }
            // nearworkplace + withinworkplacetimewindow display stuff
            else
            {
                // display distance if boundary exists
                if (selectedCompany.BoundaryDistanceM.HasValue)
                {
                    SetRowVisible(true, lblCompanyDistance, lblCompanyDistanceValue);
                    lblCompanyDistanceValue.Text = $"{selectedCompany.DistanceM:F2} m away";
                }
                else
                {
                    SetRowVisible(false, lblCompanyDistance, lblCompanyDistanceValue);
                }
                lblCompanyDistanceValue.Text = $"{selectedCompany.DistanceM:F2} m away";
                // display address if available
                if (!string.IsNullOrWhiteSpace(selectedCompany.Address))
                {
                    SetRowVisible(true, lblCompanyAddress, lblCompanyAddressValue);
                    lblCompanyAddressValue.Text = selectedCompany.Address;
                }
                else
                {
                    SetRowVisible(false, lblCompanyAddress, lblCompanyAddressValue);
                }

                // set near workplace
                nearWorkplace = selectedCompany.IsNearWorkplace(); // true if boundary has no value
                // set withinworkplacetimewindow by converting _utctime to time at companyLocation
                DateTime serverUtc = DateTime.SpecifyKind(
                    _utcTime.AddSeconds(_timerNowTicks),
                    DateTimeKind.Utc
                );
                DateTime companyLocalTime = selectedCompany.GetCompanyLocalTime(serverUtc);
                // day of week check
                List<DayOfWeekWindow> matchedDays = selectedCompany.CheckInWindows
                    .Where(w => w.DayOfWeek == (byte)companyLocalTime.DayOfWeek).ToList();
                bool withinWeeklyWindow = selectedCompany.CheckInWindows
                    .Where(w => w.DayOfWeek == (byte)companyLocalTime.DayOfWeek)
                    .Any(w =>
                        companyLocalTime.TimeOfDay >= w.StartTime &&
                        companyLocalTime.TimeOfDay <= w.EndTime
                    );  

                // date range check 
                bool withinDateRange = selectedCompany.AssignedDateRanges
                    .Any(r =>
                        companyLocalTime.Date >= r.StartDate.Date &&
                        companyLocalTime.Date <= r.EndDate.Date &&
                        companyLocalTime.TimeOfDay >= r.StartTime &&
                        companyLocalTime.TimeOfDay <= r.EndTime
                    );
                // both daterange and dayofweekwindow must be satisfied
                withinWorkplaceTimeWindow = withinWeeklyWindow && withinDateRange;
                // ---------------- button text feedback if not allowed -------------------
                // find time overlaps between the window and dateranges
                if (!withinWorkplaceTimeWindow)
                {
                    var matchedDateRanges = selectedCompany.AssignedDateRanges
                        .Where(r => companyLocalTime.Date >= r.StartDate.Date &&
                                    companyLocalTime.Date <= r.EndDate.Date)
                        .ToList();
                    var rawOverlaps =
                        from day in matchedDays
                        from range in matchedDateRanges
                        let overlapStart = day.StartTime > range.StartTime ? day.StartTime : range.StartTime
                        let overlapEnd = day.EndTime < range.EndTime ? day.EndTime : range.EndTime
                        where overlapStart <= overlapEnd
                        select (start: overlapStart, end: overlapEnd);
                    var mergedOverlaps = MergeOverlaps(rawOverlaps);
                    if (mergedOverlaps.Any())
                    {
                        // order overlaps by start time
                        var orderedOverlaps = mergedOverlaps.OrderBy(o => o.start).ToList();
                        // Display the next available window
                        var currentTime = companyLocalTime.TimeOfDay;
                        var windowToDisplay = orderedOverlaps.FirstOrDefault(o => o.start > currentTime);
                        if (windowToDisplay != default)
                        {
                            btnSubmit.Text = $"Next available check-in/out window: {windowToDisplay.start} to {windowToDisplay.end}.";
                        }
                        // if no available window later today, display the last window
                        else
                        {
                            windowToDisplay = orderedOverlaps.LastOrDefault();   
                            btnSubmit.Text = $"Last available check-in/out window today: {windowToDisplay.start} to {windowToDisplay.end}.";
                        }
                    }
                    else if (!matchedDays.Any())
                    {
                        btnSubmit.Text = "You are not allowed to check in on this day of the week.";
                    }
                    else
                    {
                        lblMessage.Text = "You are not allowed to check in at this time.";
                    }
                }
                else if (!nearWorkplace)
                {
                    btnSubmit.Text = $"Please move {(selectedCompany.DistanceM - selectedCompany.BoundaryDistanceM):F2}m closer to the selected company location";
                }
            }
        }

        // --------- clock notifications + button permissions ----------------
        if (nearWorkplace && withinWorkplaceTimeWindow && clockedRecently)
        {
            lblMessage.BackgroundColor = Colors.Red;
            lblMessage.Text = "GPS Location Service Running. Please wait to clock again.";
        }
        // Allowed to clock, enable
        if (nearWorkplace && !clockedRecently && withinWorkplaceTimeWindow )
        {
            lblMessage.BackgroundColor = Colors.Green;
            lblMessage.Text = "GPS Location Service Running. Clock in or out below.";
            btnSubmit.BackgroundColor = Colors.Green;
            btnSubmit.IsEnabled = true;
            btnSubmit.Text = "Clock-In / Clock-Out";
        }
        // button color + disable
        else
        {
            btnSubmit.BackgroundColor = Colors.LightGray;
            btnSubmit.IsEnabled = false;
        }
    }

    // Merge overlapping timespans into one
    List<(TimeSpan start, TimeSpan end)> MergeOverlaps(IEnumerable<(TimeSpan start, TimeSpan end)> periods)
    {
        var sorted = periods
            .OrderBy(p => p.start)
            .ThenByDescending(p => p.end) // ensures longer periods first if same start
            .ToList();

        var merged = new List<(TimeSpan start, TimeSpan end)>();

        foreach (var period in sorted)
        {
            if (!merged.Any())
            {
                merged.Add(period);
            }
            else
            {
                var last = merged.Last();
                if (period.start <= last.end) // overlap
                {
                    // merge
                    merged[merged.Count - 1] = (last.start, period.end > last.end ? period.end : last.end);
                }
                else
                {
                    merged.Add(period);
                }
            }
        }

        return merged;
    }
    /// <summary>
    /// Re-check distances when new item selected in dropdown
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param
    private void CompanyPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshCompanyUI();
    }
    /// <summary>
    /// Recalculates distance from userLocation to each company location and
    /// sorts companyLocations by this distance.
    /// </summary>
    /// <returns></returns>
    /// 
    private void SortCompanyLocations()
    {
        // set distances to userLoc
        foreach (var companyLocObj in companyLocations)
        {
            var companyLoc = new Location(companyLocObj.Latitude, companyLocObj.Longitude);
            if (companyLoc == null)
            {
                companyLocObj.DistanceM = double.MaxValue;
                continue;
            }
            companyLocObj.DistanceM = (Location.CalculateDistance(
                        _userLocation,
                        companyLoc,
                        DistanceUnits.Kilometers)*1000);
        }
        companyLocations = companyLocations
            .OrderBy(c => c.DistanceM)
            .ToList();
    }
    //[Obsolete]
    private async void btnSubmit_Clicked(object sender, EventArgs e)
    {
        areaLoading.IsVisible = true;
        bool submitContinue = true;

        //Clear Photo
        imgPicture.Source = null;
        lblDefaultPhoto.Text = "[Photo]";
        _picBase64 = "";
        lblPicBase64.Text = "";

        //get geolocation
        if (submitContinue == true)
        {
            if (_userLocation == null)
            {
                submitContinue = false;
                await DisplayAlert("Action Failed", "Missing location.", "Try Again");
            }
        }

        //get last record
        if (submitContinue == true)
        {
            await GetLastClocking();
            TimeSpan timeDiff = _serverTime.AddSeconds(_timerNowTicks) - _lastRecord;
            TimeSpan timeBals = TimeSpan.FromMinutes(5) - timeDiff;
            if (timeBals.TotalMilliseconds > 0) // Ensure remaining time is positive
            {
                submitContinue = false;
                await DisplayAlert("Duplicated Clocking", $"You already clock-in/clock-out at {_lastRecord.ToString("dd/MM/yyyy HH:mm")}", "OK");
            }
        }

        //take photo if needed
        if (submitContinue == true)
        {
            if (await TakePhoto() == true)
            {
                //await DisplayAlert("Debug", "Skip Submit Clocking", "Ok");
                await SubmitClocking();
            }
            else
            {
                await DisplayAlert("Action Failed", "Missing photo.", "Try Again");
            }
        }

        areaLoading.IsVisible = false;
    }

    //[Obsolete]
    public async Task<bool> TakePhoto()
    {
        //disable photo temporary
        if (_disableTakePhoto == true)
        {
            return true;
        }

        //optional with biometric login
        if (LoginMethod == "Bio")
        {
            bool Ans = await DisplayAlert("Optional", "Do you want to take photo?", "Take Photo", "Skip");
            if (Ans != true)
            {
                return true;
            }
        }

        //take photo
        try
        {
            //check device camera
            if (MediaPicker.IsCaptureSupported)
            {
                //turn on camera to take photo
                FileResult photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {

                    //open captured photo
                    Stream stream = await photo.OpenReadAsync();

                    //convert photo stream as image
                    using (var skImage = SKBitmap.Decode(stream))
                    {
                        //int desireW = (int)imgPicture.Height; int desireH = (int)imgPicture.Height;
                        //int desireW = 120; int desireH = 160;
                        int desireW = 160; int desireH = 200;
                        decimal scaleDiff1;
                        decimal scaleDiff2;
                        decimal scaleFactor;
                        string photoOrientation;
                        if (skImage.Height >= skImage.Width)
                        {
                            //portrait
                            photoOrientation = "portrait";
                            //await DisplayAlert("Debug", $"Is portrait. {skImage.Width}x{skImage.Height}", "OK");
                            scaleDiff1 = (decimal)desireW / (decimal)skImage.Width;
                            scaleDiff2 = (decimal)desireH / (decimal)skImage.Height;
                        }
                        else
                        {
                            //landscape
                            photoOrientation = "landscape";
                            //await DisplayAlert("Debug", $"Is lanscape. {skImage.Width}x{skImage.Height}", "OK");
                            scaleDiff1 = (decimal)desireH / (decimal)skImage.Width;
                            scaleDiff2 = (decimal)desireW / (decimal)skImage.Height;
                        }
                        if (scaleDiff1 >= scaleDiff2)
                        {
                            scaleFactor = scaleDiff1;
                        }
                        else
                        {
                            scaleFactor = scaleDiff2;
                        }
                        int newWidth = (int)(skImage.Width * scaleFactor);
                        int newHeight = (int)(skImage.Height * scaleFactor);
                        using (var bitmap_scale = skImage.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High))
                        {
                            //rotate photo
                            SKBitmap bitmap_rotate;
                            //await DisplayAlert("Debug", $"Photo taken as {photoOrientation}", "Ok");
                            if (photoOrientation == "landscape")
                            {
                                bitmap_rotate = RotateBitmap(bitmap_scale);
                            }
                            else
                            {
                                bitmap_rotate = bitmap_scale;
                            }

                            using (MemoryStream memory = new MemoryStream())
                            {
                                Stream stream_scale = SKImage.FromBitmap(bitmap_rotate).Encode().AsStream();
                                stream_scale.CopyTo(memory);
                                byte[] imageBytes = memory.ToArray();
                                imgPicture.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                                lblDefaultPhoto.Text = "";

                                string base64String = Convert.ToBase64String(imageBytes);
                                while (base64String.Length > 40000)
                                {
                                    newWidth = (int)(newWidth * 0.8);
                                    newHeight = (int)(newHeight * 0.8);
                                    //await DisplayAlert("Base64 Length", $"{base64String.Length}. NewH/W:{imgW}/{imgH}", "OK");

                                    byte[] imageBytes_smaller = ResizeBitmap(imageBytes, newWidth, newHeight);
                                    base64String = Convert.ToBase64String(imageBytes_smaller);
                                }
                                _picBase64 = $"{base64String}";
                                lblPicBase64.Text = $"LEN:{base64String.Length}";
                                //imgPicture.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                                //lblDefaultPhoto.Text = "";
                                return true;
                            }
                        }
                    }

                }
                else
                {
                    //no picture capture
                    await DisplayAlert("Error", $"No photo found.", "OK");
                    return false;
                }
            }
            else
            {
                //no camera
                await DisplayAlert("Error", $"No camera detect.", "OK");
                return false;
            }
        }
        catch (Exception ex)
        {
            imgPicture.Source = null;
            lblDefaultPhoto.Text = "[Photo]";
            _picBase64 = "";
            lblPicBase64.Text = "";
            await DisplayAlert("Error", $"{ex.Message}", "OK");
            return false;
        }
    }

    SKBitmap RotateBitmap(SKBitmap bitmap)
    {
        SKBitmap rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width);

        using (var surface = new SKCanvas(rotatedBitmap))
        {
            surface.Translate(rotatedBitmap.Width, 0);
            surface.RotateDegrees(90);
            surface.DrawBitmap(bitmap, 0, 0);
        }

        return rotatedBitmap;
    }

    //[Obsolete]
    private byte[] ResizeBitmap(byte[] originalImageBytes, int widthToScale, int heightToScale)
    {
        try
        {
            // Convert byte array to SKBitmap
            using (SKBitmap originalImage = SKBitmap.Decode(originalImageBytes))
            {
                // Calculate scaling factors
                float xScale = (float)widthToScale / originalImage.Width;
                float yScale = (float)heightToScale / originalImage.Height;
                float xyScale = xScale;
                if (yScale < xScale)
                {
                    xyScale = yScale;
                }
                int newW = (int)(originalImage.Width * xyScale);
                int newH = (int)(originalImage.Height * xyScale);

                // Create a new bitmap with the desired dimensions
                //SKBitmap resizedBitmap = new SKBitmap(widthToScale, heightToScale);
                SKBitmap resizedBitmap = new SKBitmap(newW, newH);

                // Resize the original image using the transformation matrix
                using (SKCanvas canvas = new SKCanvas(resizedBitmap))
                {
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.FilterQuality = SKFilterQuality.High;
                        canvas.Clear(SKColors.Transparent); // Clear canvas before drawing

                        // Apply scaling factors directly to the canvas matrix
                        canvas.Scale(xyScale, xyScale);

                        // Draw the original image onto the resized canvas
                        canvas.DrawBitmap(originalImage, 0, 0, paint);
                    }
                }

                // Convert the resized SKBitmap to a byte array
                using (SKImage image = SKImage.FromBitmap(resizedBitmap))
                using (SKData data = image.Encode())
                {
                    //DisplayAlert("Resized", $"{image.Height}x{image.Width}","OK");
                    return data.ToArray();
                }
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    private void imgPictureTap_Tapped(object sender, EventArgs e)
    {
        if (imgPicture.Source != null)
        {
            if (imgPicture.Aspect == Aspect.AspectFill)
            {
                imgPicture.Aspect = Aspect.AspectFit;
            }
            else
            {
                imgPicture.Aspect = Aspect.AspectFill;
            }
        }
        else
        {
            imgPicture.Aspect = Aspect.AspectFill;
        }
    }

    public async Task SubmitClocking()
    {
        string _gDateNowD = lblTimeNow.Text.Substring(0, 2);
        string _gDateNowM = lblTimeNow.Text.Substring(3, 2);
        string _gDateNowY = lblTimeNow.Text.Substring(6, 4);
        string _gTimeNowH = lblTimeNow.Text.Substring(11, 2);
        string _gTimeNowM = lblTimeNow.Text.Substring(14, 2);
        string _gTimeNowS = lblTimeNow.Text.Substring(17, 2);
        //string _gDateTimeNow = lblTimeNow.Text;
        DateTime _gDateTimeNow = new DateTime(int.Parse(_gDateNowY), int.Parse(_gDateNowM), int.Parse(_gDateNowD), int.Parse(_gTimeNowH), int.Parse(_gTimeNowM), int.Parse(_gTimeNowS));
        string _gGeoLatitude = CoordToString(_userLocation.Latitude);
        string _gGeoLongitude = CoordToString(_userLocation.Longitude);
        string _gGeoAddress = lblAddress.Text;

        //await DisplayAlert("Insert Data", $"{_gDateTimeNow}\nLat:{_gGeoLatitude}\nLon:{_gGeoLongitude}\nAddress\n{_gGeoAddress}", "OK");

        try
        {
            var results = await ApiFunctions.jsonSubmitClocking(getIdentifier, getUsername, _gDateNowD, _gDateNowM, _gDateNowY, _gTimeNowH, _gTimeNowM, _gTimeNowS, _gGeoLatitude, _gGeoLongitude, _gGeoAddress, _picBase64);
            //await DisplayAlert("Debug", $"Results: {results.Item1},{results.Item2}", "OK");
            if (results.Item1 == "Success")
            {
                _lastRecord = _gDateTimeNow;
                lblLastRecord.Text = $"Last Clocking Time: {_lastRecord.ToString("dd/MM/yyyy HH:mm")}";
                await DisplayAlert("Success", $"Clocking Time: {_lastRecord.ToString("dd/MM/yyyy HH:mm")}\n\nAddress: {_gGeoAddress}", "OK");
            }
            else
            {
                await DisplayAlert("Action Failed", $"Please try again.", "OK");
            }
        }
        catch (Exception ex)
        {
            //await DisplayAlert(ex.Message, "Please try again.", "OK");
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            lblPicBase64.Text = "";
        }
    }
    private void SetRowVisible(bool visible, params VisualElement[] elements)
    {
        foreach (var el in elements)
            el.IsVisible = visible;
    }



}