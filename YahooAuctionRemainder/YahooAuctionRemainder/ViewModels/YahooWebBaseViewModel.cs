using System;
using Prism.Navigation;
using System.Threading.Tasks;

namespace YahooAuctionRemainder.ViewModels
{
    /// <summary>
    /// YahooWebページ読み込み基底クラス
    /// </summary>
    public class YahooWebBaseViewModel : ViewModelBase
    {
        public YahooWebBaseViewModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public Func<string, Task<string>> EvaluateJavascript { get; set; }

        /// <summary>
        /// The source URL.
        /// </summary>
        private string _sourceUrl;
        public string SourceUrl
        {
            get { return _sourceUrl; }
            set
            {
                SetProperty(ref _sourceUrl, value);
            }
        }

        private bool _isWebViewVisible = true;
        public bool IsWebViewVisible
        {
            get { return _isWebViewVisible; }
            set
            {
                SetProperty(ref _isWebViewVisible, value);
            }
        }

        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                SetProperty(ref _isLoading, value);
            }
        }

        /// <summary>
        /// 読込中メッセージ
        /// </summary>
        private string _loadingMessage;
        public string LoadingMessage
        {
            get { return _loadingMessage; }
            set
            {
                SetProperty(ref _loadingMessage, value);
            }
        }
    }
}
