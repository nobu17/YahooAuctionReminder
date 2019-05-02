using System;
using System.Threading.Tasks;
using YahooAuctionRemainder;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Foundation;

[assembly: ExportRenderer(typeof(WebView2), typeof(YahooAuctionRemainder.iOS.WebView2Renderer))]
namespace YahooAuctionRemainder.iOS
{

    public class WebView2Renderer : WebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            var webView = e.NewElement as WebView2;
            if (webView != null)
            {
                //NSUrlCache.SharedCache.RemoveAllCachedResponses();
                //NSUrlCache.SharedCache.MemoryCapacity = 0;
                //NSUrlCache.SharedCache.DiskCapacity = 0;

                webView.EvaluateJavascript = (js) =>
                {
                    return Task.FromResult(this.EvaluateJavascript(js));
                };
            }

        }
    }
}

