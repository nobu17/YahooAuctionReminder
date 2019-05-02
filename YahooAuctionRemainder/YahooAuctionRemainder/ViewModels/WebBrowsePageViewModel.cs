using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YahooAuctionRemainder.Common;
using YahooAuctionRemainder.Services;

namespace YahooAuctionRemainder.ViewModels
{
    public class WebBrowsePageViewModel : ViewModelBase
    {
        private readonly INotificationForLimit _notificationService;

        public WebBrowsePageViewModel(INavigationService navigationService, INotificationForLimit notificationService)
            : base(navigationService)
        {
            _notificationService = notificationService;
        }

        #region prop

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

        #endregion

        #region Command

        public Command GoBack
        {
            get
            {
                return new Command(() =>
                {
                    MessagingCenter.Send(this, "GoBack");
                });
            }
        }

        public Command GoForward
        {
            get
            {
                return new Command(() =>
                {
                    MessagingCenter.Send(this, "GoForward");
                });
            }
        }

        public ICommand Navigated
        {
            get
            {
                return new Command(() =>
                {

                });
            }
        }

        public ICommand Navigating
        {
            get
            {
                return new Command(() =>
                {

                });
            }
        }


        #endregion

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey(StaticInfo.AuctionUrlTransitParamKey))
            {
                SourceUrl = parameters[StaticInfo.AuctionUrlTransitParamKey] as string;
            } 
            else if(parameters.ContainsKey(StaticInfo.NotificationTransitParamKey))
            {
                //オークションIDがKey
                var id = parameters[StaticInfo.NotificationTransitParamKey] as string;
                //通知からきた場合はIDからURL取得
                SourceUrl = _notificationService.GetAuctionUrlFromNotificationId(id);
            }


            base.OnNavigatingTo(parameters);
        }
    }
}
