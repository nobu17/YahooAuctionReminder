using System;
using System.Collections.Generic;

namespace YahooAuctionRemainder.DService
{
    public interface ILocalNotifyService
    {
        void AddNotify(LocalNotifyData notifyData);
        void CancelNotify(string key);
        void GetNotifycationList(Action<IEnumerable<LocalNotifyData>> notifyList);
    }
}
