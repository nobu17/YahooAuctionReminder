using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace YahooAuctionRemainder
{
    /// <summary>
    /// JS実行結果と各種イベントをコマンドとして登録可能なWebView
    /// </summary>
    public class WebView2 : WebView
    {
        public static readonly BindableProperty NavigatingCommandProperty =
            BindableProperty.Create(nameof(NavigatingCommand), typeof(ICommand), typeof(WebView2), null);

        public static readonly BindableProperty NavigatedCommandProperty =
            BindableProperty.Create(nameof(NavigatedCommand), typeof(ICommand), typeof(WebView2), null);


        public static BindableProperty EvaluateJavascriptProperty =
            BindableProperty.Create(nameof(EvaluateJavascript), typeof(Func<string, Task<string>>), typeof(WebView2), null, BindingMode.OneWayToSource);

        public WebView2()
        {
            Navigating += (s, e) =>
            {
                if (NavigatingCommand?.CanExecute(e) ?? false)
                    NavigatingCommand.Execute(e);
            };

            Navigated += (s, e) =>
            {
                if (NavigatedCommand?.CanExecute(e) ?? false)
                    NavigatedCommand.Execute(e);
            };
        }

        public ICommand NavigatingCommand
        {
            get { return (ICommand)GetValue(NavigatingCommandProperty); }
            set { SetValue(NavigatingCommandProperty, value); }
        }

        public ICommand NavigatedCommand
        {
            get { return (ICommand)GetValue(NavigatedCommandProperty); }
            set { SetValue(NavigatedCommandProperty, value); }
        }

        public Func<string, Task<string>> EvaluateJavascript
        {
            get { return (Func<string, Task<string>>)GetValue(EvaluateJavascriptProperty); }
            set { SetValue(EvaluateJavascriptProperty, value); }
        }
    }
}
