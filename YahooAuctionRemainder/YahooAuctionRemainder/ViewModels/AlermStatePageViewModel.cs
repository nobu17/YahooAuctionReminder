using System;
using Prism.Navigation;
using YahooAuctionRemainder.Model;
using YahooAuctionRemainder.Services;

namespace YahooAuctionRemainder.ViewModels
{
    public class AlermStatePageViewModel : ViewModelBase
    {
        public AlermStatePageViewModel(INavigationService navigationService, INotificationForLimit notificationService)
            : base(navigationService)
        {
            _model = new AlermStatePageModel(notificationService);
        }


        /// <summary>
        /// Model.
        /// </summary>
        private AlermStatePageModel _model;
        public AlermStatePageModel Model
        {
            get { return _model; }
            set
            {
                SetProperty(ref _model, value);
            }
        }


        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            //アラーム一覧を更新
            _model.UpdateCurrentAlermList();
            
            base.OnNavigatedTo(parameters);
        }
    }
}
