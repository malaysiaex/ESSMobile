namespace ESSMobile
{
    using Microsoft.Maui.Controls;
    using System.Linq;

#if IOS
    using WebKit;
    using UIKit;
    using Foundation;
    using PhotosUI;
#endif

    public class CustomWebView : WebView
    {
#if IOS
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            if (Handler?.PlatformView is WKWebView wkWebView)
            {
                var msgHandler = new FilePickerMessageHandler(wkWebView);
                wkWebView.Configuration.UserContentController
                    .AddScriptMessageHandler(msgHandler, "fileInputTapped");
            }
        }

        private class FilePickerMessageHandler : NSObject, IWKScriptMessageHandler
        {
            private readonly WKWebView _webView;

            public FilePickerMessageHandler(WKWebView webView)
            {
                _webView = webView;
            }

            public void DidReceiveScriptMessage(
                WKUserContentController userContentController,
                WKScriptMessage message)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var rootVC = UIApplication.SharedApplication
                        .ConnectedScenes
                        .ToArray()
                        .OfType<UIWindowScene>()
                        .FirstOrDefault()
                        ?.Windows
                        .FirstOrDefault(w => w.IsKeyWindow)
                        ?.RootViewController;

                    if (rootVC == null) return;
                    while (rootVC.PresentedViewController != null)
                        rootVC = rootVC.PresentedViewController;

                    var config = new PHPickerConfiguration();
                    config.SelectionLimit = 1;
                    config.Filter = PHPickerFilter.ImagesFilter;

                    var picker = new PHPickerViewController(config);
                    picker.Delegate = new PickerDelegate(_webView);

                    rootVC.PresentViewController(picker, true, null);
                });
            }
        }

        private class PickerDelegate : PHPickerViewControllerDelegate
        {
            private readonly WKWebView _webView;

            public PickerDelegate(WKWebView webView)
            {
                _webView = webView;
            }

            public override void DidFinishPicking(
                PHPickerViewController picker,
                PHPickerResult[] results)
            {
                picker.DismissViewController(true, null);

                // User cancelled — send empty result back
                if (results == null || results.Length == 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _webView.EvaluateJavaScript("receiveFileFromNative('', '');", null);
                    });
                    return;
                }

                results[0].ItemProvider.LoadObject<UIImage>((obj, err) =>
                {
                    if (obj is not UIImage image)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            _webView.EvaluateJavaScript("receiveFileFromNative('', '');", null);
                        });
                        return;
                    }

                    // Compress progressively until under 200KB
                    NSData jpegData = null;
                    float quality = 0.7f;
                    do
                    {
                        jpegData = image.AsJPEG(quality);
                        quality -= 0.1f;
                    }
                    while (jpegData != null && jpegData.Length > 200 * 1024 && quality > 0.1f);

                    if (jpegData == null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            _webView.EvaluateJavaScript("receiveFileFromNative('', '');", null);
                        });
                        return;
                    }

                    var base64 = jpegData.GetBase64EncodedString(
                        NSDataBase64EncodingOptions.None);
                    var fileName = "photo.jpg";

                    var js = $"receiveFileFromNative('{base64}', '{fileName}');";

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _webView.EvaluateJavaScript(js, null);
                    });
                });
            }
        }
#endif
    }
}