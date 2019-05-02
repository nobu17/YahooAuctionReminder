using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using YahooAuctionRemainder.Data;
using YahooAuctionRemainder.Services;
using YahooAuctionRemainder.DService;

namespace YahooAuctionRemainder.Model
{
	public class AlermStatePageModel : BindableBase
    {
        private INotificationForLimit _notificationService;

		public AlermStatePageModel(INotificationForLimit notificationService)
        {
            _notificationService = notificationService;
        }

        public async void UpdateCurrentAlermList()
        {
            AlermList = _notificationService.GetCurrentAlemList().OrderBy(x => x.AuctionEndDateTime).ToList();

            var nFyList = await _notificationService.GetCurrentNotifycationList();
            if(nFyList != null)
                NotificationList = nFyList.ToList();
        }


        /// <summary>
        /// The Alerm list.
        /// </summary>
        private List<AlermTarget> _alermList;
        public List<AlermTarget> AlermList
        {
            get { return _alermList; }
            set
            {
                SetProperty(ref _alermList, value);
            }
        }

        /// <summary>
        /// The Notifucation list.
        /// </summary>
        private List<LocalNotifyData> _notificationList;
        public List<LocalNotifyData> NotificationList
        {
            get { return _notificationList; }
            set
            {
                SetProperty(ref _notificationList, value);
            }
        }
    }
}
