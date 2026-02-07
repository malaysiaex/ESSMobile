using ESSMobile.Models;
using ESSMobile.Shared;
using Microsoft.Maui.Storage;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ESSMobile.Shared
{
    public static class _Function
    {
        static string BaseUrl = "";
        //static string BaseUrl = "http://exacom.koreacentral.cloudapp.azure.com:6100"; //old exacom azure (deleted soon)
        //static string BaseUrl = "http://exacom.com.my:6100"; //exabyte vm (connection dropped, stopped working)
        //static string BaseUrl = "http://192.168.1.147:6100"; //local testing only
        //static HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        //static int clientTimeOut = 20;

        static string JsonUrl = "https://www.exacom.com.my/files/master_url.json";
        public static async Task<string> GetValueByKeyAsync(string key)
        {
            if (key == "ESSMobile_API2")
            {
                return "https://localhost:7220";
            }
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

        public static async Task<(string, string)> jsonCheckIndentifier(string sCompanyIdentifier)
        {
            //company identifier cannot blank or empty
            if (string.IsNullOrEmpty(sCompanyIdentifier) || string.IsNullOrWhiteSpace(sCompanyIdentifier))
            {
                SecureStorage.Remove("login_constring");
                SecureStorage.Remove("login_company");
                SecureStorage.Remove("login_licenseid");
                SecureStorage.Remove("login_api");
                return ("Error", "Invalid Company Identifier");
            }

            BaseUrl = await GetValueByKeyAsync("ESSMobile_API");
            HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            client.Timeout = new TimeSpan(0, 0, 20);
            try
            {
                //check company identifier with backend
                var requestContent = new jsonRequest_Identifier { IdentifierID = sCompanyIdentifier };
                var requestJson = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/Employee/checkCompanyIdentifier", requestJson);
                var responseJson = await response.Content.ReadAsStringAsync();

                ESS_Root responseContent = JsonConvert.DeserializeObject<ESS_Root>(responseJson);

                var responseResult = responseContent.ESS_Master.Resultcode;
                var responseMessage = responseContent.ESS_Master.Detailmsg;

                if (responseResult == "IdentifierInvalid")
                {
                    SecureStorage.Remove("login_constring");
                    SecureStorage.Remove("login_company");
                    SecureStorage.Remove("login_licenseid");
                    SecureStorage.Remove("login_api");
                    return ("Error", responseMessage);
                }
                else
                {
                    await SecureStorage.SetAsync("login_constring", responseContent.Identifier[0].ConString);
                    await SecureStorage.SetAsync("login_company", responseContent.Identifier[0].Company);
                    await SecureStorage.SetAsync("login_licenseid", responseContent.Identifier[0].LicenseID);
                    await SecureStorage.SetAsync("login_api", responseContent.Identifier[0].API);

                    for (int loopCompany = 0; loopCompany <= responseContent.Identifier.Count - 1; loopCompany++)
                    {
                        if (sCompanyIdentifier.Trim().ToUpper() == responseContent.Identifier[loopCompany].IdentifierID.Trim().ToUpper())
                        {
                            await SecureStorage.SetAsync("login_constring", responseContent.Identifier[loopCompany].ConString);
                            await SecureStorage.SetAsync("login_company", responseContent.Identifier[loopCompany].Company);
                            await SecureStorage.SetAsync("login_licenseid", responseContent.Identifier[loopCompany].LicenseID);
                            await SecureStorage.SetAsync("login_api", responseContent.Identifier[loopCompany].API);
                            break;
                        }
                    }
                    return ("Success", responseMessage);
                }
            }
            catch (TaskCanceledException er)
            {
                return ("Maintenance", "Server is offline for maintenance, please try again later.");
                //return ("Maintenance", "Server is offline for maintenance, please try again later." + "\nMsg1: " + er.Message);
            }
            catch (Exception ex)
            {
                SecureStorage.Remove("login_constring");
                SecureStorage.Remove("login_company");
                SecureStorage.Remove("login_licenseid");
                SecureStorage.Remove("login_api");
                return ("Error", ex.Message);
                //return ("Maintenance", "Server is offline for maintenance, please try again later." + "\nMsg2: " + ex.Message);
            }
        }

        public static async Task<(string,string)> jsonCheckLogin(string sUsername, string sPassword)
        {
            //login identifier
            var getIdentifier = await SecureStorage.GetAsync("login_identifier");

            //constring from login identifier
            var login_constring = await SecureStorage.GetAsync("login_constring");
            if (string.IsNullOrEmpty(login_constring) || string.IsNullOrWhiteSpace(login_constring))
            {
                return ("Error", "Invalid Database.");               
            }
            
            //username and password cannot blank or empty
            if (string.IsNullOrEmpty(sUsername) || string.IsNullOrWhiteSpace(sUsername) ||
                string.IsNullOrEmpty(sPassword) || string.IsNullOrWhiteSpace(sPassword))
            {
                return ("Error", "Invalid Username or Password.");
            }

            BaseUrl = await GetValueByKeyAsync("ESSMobile_API");
            HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            client.Timeout = new TimeSpan(0, 0, 20);
            try
            {
                //check username and password with backend
                var requestContent = new jsonRequest_Login { dbPath = "", dbName = "", dbUser = "", dbPass = "", Token = getIdentifier, userID = sUsername, userPassword = sPassword };
                var requestJson = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/Employee/verifypass", requestJson);
                var responseJson = await response.Content.ReadAsStringAsync();

                ESS_Root responseContent = JsonConvert.DeserializeObject<ESS_Root>(responseJson);

                var responseResult = responseContent.ESS_Master.Resultcode;
                var responseMessage = responseContent.ESS_Master.Detailmsg;

                if (responseResult == "Match1")
                {
                    return ("Success", responseMessage);
                }
                else
                {
                    return ("Error", responseMessage);
                }
            }
            catch (TaskCanceledException er)
            {
                return ("Maintenance", "Server is offline for maintenance, please try again later.");
                //return ("Maintenance", "Server is offline for maintenance, please try again later." + "\nMsg3: " + er.Message);
            }
            catch (Exception ex)
            {
                //return ("Error", ex.Message);
                return ("Maintenance", "Server is offline for maintenance, please try again later.");
                //return ("Maintenance", "Server is offline for maintenance, please try again later." + "\nMsg4: " + ex.Message);
            }
        }

        public static async Task<(string, string)> jsonGetServerTime(string getIdentifier)
        {
            //var login_api = await Xamarin.Essentials.SecureStorage.GetAsync("login_api");
            //if (string.IsNullOrEmpty(login_api) || string.IsNullOrWhiteSpace(login_api))
            //{
            //    client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            //}
            //else
            //{
            //    client = new HttpClient { BaseAddress = new Uri(login_api) };
            //}
            BaseUrl = await GetValueByKeyAsync("ESSMobile_API");
            HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            client.Timeout = new TimeSpan(0, 0, 20);
            try
            {
                //get server date time
                var requestContent = new jsonRequest_Login { dbPath = "", dbName = "", dbUser = "", dbPass = "", Token = getIdentifier, userID = "", userPassword = "" };
                var requestJson = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/Employee/GetServerDateTime", requestJson);
                var responseJson = await response.Content.ReadAsStringAsync();

                ESS_Root responseContent = JsonConvert.DeserializeObject<ESS_Root>(responseJson);

                var responseResult = responseContent.ESS_Master.Resultcode;
                var responseMessage = responseContent.ESS_Master.Detailmsg;


                if (responseResult == "Success")
                {
                    return ("Success", responseMessage);
                }
                else
                {
                    return ("Error", responseMessage);
                }
            }
            catch (TaskCanceledException er)
            {
                return ("Maintenance", "Server is offline for maintenance, please try again later.");
                //return ("Maintenance", "Server is offline for maintenance, please try again later." + "\nMsg5: " + er.Message);
            }
            catch (Exception ex)
            {
                //return ("Error", ex.Message);
                return ("Maintenance", "Server is offline for maintenance, please try again later.");
                //return ("Maintenance", "Server is offline for maintenance, please try again later." + "\nMsg6: " + ex.Message);
            }
        }

        public static async Task<(string, string)> jsonGetLastClocking(string getIdentifier, string getUsername)
        {
            //var login_api = await Xamarin.Essentials.SecureStorage.GetAsync("login_api");
            //if (string.IsNullOrEmpty(login_api) || string.IsNullOrWhiteSpace(login_api))
            //{
            //    client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            //}
            //else
            //{
            //    client = new HttpClient { BaseAddress = new Uri(login_api) };
            //}
            BaseUrl = await GetValueByKeyAsync("ESSMobile_API");
            HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            client.Timeout = new TimeSpan(0, 0, 20);
            try
            {
                //get last clocking date time
                var requestContent = new jsonRequest_Login { dbPath = "", dbName = "", dbUser = "", dbPass = "", Token = getIdentifier, userID = getUsername, userPassword = "" };
                var requestJson = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("/api/Employee/GetLastClocking", requestJson);
                var responseJson = await response.Content.ReadAsStringAsync();

                ESS_Root responseContent = JsonConvert.DeserializeObject<ESS_Root>(responseJson);

                var responseResult = responseContent.ESS_Master.Resultcode;
                var responseMessage = responseContent.ESS_Master.Detailmsg;

                if (responseResult == "Success")
                {
                    return ("Success", responseMessage);
                }
                else
                {
                    return ("Error", responseMessage);
                }
            }
            catch (TaskCanceledException er)
            {
                return ("Maintenance", "Server is offline for maintenance, please try again later.");
                //return ("Maintenance", "Server is offline for maintenance, please try again later." + "\nMsg7: " + er.Message);
            }
            catch (Exception ex)
            {
                //return ("Error", ex.Message);
                return ("Maintenance", "Server is offline for maintenance, please try again later.");
                //return ("Maintenance", "Server is offline for maintenance, please try again later." + "\nMsg8: " + ex.Message);
            }
        }

        public static async Task<(string, string)> jsonSubmitClocking(string getIdentifier, string getUsername, string getDateD, string getDateM, string getDateY, string getTimeH, string getTimeM, string getTimeS, string getGeoLa, string getGeoLo, string getGeoAd, string getImg64)
        {
            BaseUrl = await GetValueByKeyAsync("ESSMobile_API");
            HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            client.Timeout = new TimeSpan(0, 0, 20);
            try
            {
                //get last clocking date time
                var requestContent = new jsonRequest_SubmitClocking { dbPath = "", dbName = "", dbUser = "", dbPass = "", Token = getIdentifier, userID = getUsername, userPassword = "", DateD = getDateD, DateM = getDateM, DateY = getDateY, TimeH = getTimeH, TimeM = getTimeM, TimeS = getTimeS, GeoLa = getGeoLa, GeoLo = getGeoLo, GeoAd = getGeoAd, Img64 = getImg64};
                var requestJson = new StringContent(JsonConvert.SerializeObject(requestContent), Encoding.UTF8, "application/json");                
                var response = await client.PostAsync("/api/Employee/SubmitClocking", requestJson);
                var responseJson = await response.Content.ReadAsStringAsync();

                ESS_Root responseContent = JsonConvert.DeserializeObject<ESS_Root>(responseJson);

                var responseResult = responseContent.ESS_Master.Resultcode;
                var responseMessage = responseContent.ESS_Master.Detailmsg;

                if (responseResult == "Success")
                {
                    return ("Success", responseMessage);
                }
                else
                {
                    return ("Error", responseMessage);
                }
            }
            catch (TaskCanceledException er)
            {
                return ("Maintenance", "Server is offline for maintenance, please try again later.");
                //return ("Maintenance", "Server is offline for maintenance, please try again later." + "\nMsg9: " + er.Message);
            }
            catch (Exception ex)
            {
                //return ("Error", ex.Message);
                return ("Maintenance", "Server is offline for maintenance, please try again later.");
                //return ("Maintenance", "Server is offline for maintenance, please try again later." + "\nMsg10: " + ex.Message);
            }
        }

        /// <summary>
        /// Returns (1, 2) as indicated below
        /// <para>
        /// 1. Returns list of LocationCheckInWindows belonging to all locations. 
        /// </para>
        /// <para>
        /// 2. Returns list of CompanyLocation belonging to company identifier.
        /// Can return empty list - indicates user can check in from anywhere.
        /// Returns null if the CompanyLocation database was not found.
        /// Throws exception if error.
        /// </para>
        /// </summary>
        /// <param name="companyIdentifier"></param>
        /// <returns></returns>
        public static async Task<List<CompanyLocation>?> APIGetCompanyLocations(string companyIdentifier, string username)
        {
            BaseUrl = await GetValueByKeyAsync("ESSMobile_API2");

            HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            client.Timeout = new TimeSpan(0, 0, 20);
            try
            {
                // retrieve company locations depending on companyIdentifier
                string url = $"/api/Employee/GetCompanyLocationsWithWindows?identifierId={Uri.EscapeDataString(companyIdentifier)}&userId={Uri.EscapeDataString(username)}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseJson = await response.Content.ReadAsStringAsync();
                var companyLocations = JsonConvert.DeserializeObject<List<CompanyLocation>>(responseJson);
                return companyLocations;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static async Task<DateTime> APIGetServerUtcTime()
        {
            BaseUrl = await GetValueByKeyAsync("ESSMobile_API2");
            // temp hard code to local api
            //BaseUrl = "https://localhost:5002";

            HttpClient client = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            client.Timeout = new TimeSpan(0, 0, 20);

            try
            {
                string url = "/api/Employee/GetUtcTime";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var utcObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseJson);
                if (utcObj == null || !utcObj.ContainsKey("utc"))
                    throw new Exception("Invalid response from server.");

                // Parse the ISO 8601 UTC string into DateTime
                var serverUtcTime = DateTime.Parse(utcObj["utc"], null, System.Globalization.DateTimeStyles.AdjustToUniversal);

                // Ensure DateTimeKind is UTC
                serverUtcTime = DateTime.SpecifyKind(serverUtcTime, DateTimeKind.Utc);

                return serverUtcTime;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
