using ESSMobile.Shared;

public class ApiInitializer
{
    private readonly IHttpClientFactory _factory;
    static string JsonUrl = "https://www.exacom.com.my/files/master_url.json";
    public static string ESSMobileApi { get; private set; }
    public static string ESSMobileApi2 { get; private set; }
    public static string ESSMobileWeb { get; private set; }
    public static string ESSMobile_DisableTakePhoto { get; private set; }

    public bool IsInitialized { get; private set; } = false;
    private Task _initializationTask;   

    public Task InitializationTask => _initializationTask ??= InitializeAsync();
    public ApiInitializer(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
            return;

        using var tempClient = new HttpClient();
        tempClient.DefaultRequestHeaders.UserAgent.ParseAdd("ESSMobile/1.0");
        var json = await tempClient.GetStringAsync(JsonUrl);
        var apiUrls = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        ESSMobileApi = apiUrls["ESSMobile_API"];
        ESSMobileApi2 = apiUrls["ESSMobile_API2"];
        // temp hard code for local testing
        //ESSMobileApi2 = "https://localhost:7220";
        ESSMobileWeb = apiUrls["ESSMobile_WebPage"];
        ESSMobile_DisableTakePhoto = apiUrls["ESSMobile_DisableTakePhoto"];
        // Initialize ApiFunctions after URLs are resolved
        ApiFunctions.Initialize(this, _factory);
        IsInitialized = true;
    }
}