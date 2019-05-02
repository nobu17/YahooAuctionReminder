using System;
using System.Windows.Input;
using Prism.Navigation;
using Prism.Services;
using Xamarin.Forms;
using YahooAuctionRemainder.DService;
using YahooAuctionRemainder.Model;
using YahooAuctionRemainder.Services;

namespace YahooAuctionRemainder.ViewModels
{
    public class UserSettingPageViewModel : ViewModelBase
    {
        private readonly IPageDialogService _pageDialogService;

        public UserSettingPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, ISettingService settingService)
            : base(navigationService)
        {
            _pageDialogService = pageDialogService;
            _model = new UserSettingPageModel(settingService);
        }

        /// <summary>
        /// Model.
        /// </summary>
        private UserSettingPageModel _model;
        public UserSettingPageModel Model
        {
            get { return _model; }
            set
            {
                SetProperty(ref _model, value);
            }
        }

        /// <summary>
        /// オークションのキャッシュクリア
        /// </summary>
        /// <value>The clear auction cache.</value>
        public ICommand ClearAuctionCache
        {
            get
            {
                return new Command(() =>
                {
                    _model.ClearAuctionCache();
                    _pageDialogService.DisplayAlertAsync("", "キャッシュをクリアしました。", "OK");
                });
            }
        }

        /// <summary>
        /// トグル情報のキャッシュクリア
        /// </summary>
        /// <value>The clear auction cache.</value>
        public ICommand ClearToggleCache
        {
            get
            {
                return new Command(() =>
                {
                    _model.ClearToglleCache();
                    _pageDialogService.DisplayAlertAsync("", "キャッシュをクリアしました。リロードします。", "OK");
                });
            }
        }


        /// <summary>
        /// アラーム一覧ページへ移動
        /// </summary>
        /// <value>The goto alerm state page.</value>
        public ICommand GotoAlermStatePage
        {
            get
            {
                return new Command(() =>
                {
                    NavigationService.NavigateAsync("AlermStatePage"); 
                });
            }
        }


        public ICommand MakeTestAlerm
        {
            get
            {
                return new Command(() =>
                {
                    var noty = new LocalNotifyData();
                    noty.Title = "test";
                    noty.Body = "body";
                    noty.ReserveDate = DateTime.Now.AddSeconds(30);
                    noty.Key = "v564278464";
                    Xamarin.Forms.DependencyService.Get<ILocalNotifyService>().AddNotify(noty);
                });
            }
        }

    }
}
