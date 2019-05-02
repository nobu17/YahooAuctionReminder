using System;
using Prism.Navigation;
using UserNotifications;
using Xamarin.Forms;
using YahooAuctionRemainder.Common;
using YahooAuctionRemainder.Services;
using YahooAuctionRemainder.ViewModels;

namespace YahooAuctionRemainder.iOS
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Tell system to display the notification anyway or use
            // `None` to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert);
        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            var id = response.Notification.Request.Identifier;
            var p = App.Current.MainPage as NavigationPage;
            var vModel = p.CurrentPage.BindingContext as ViewModelBase;
            var param = new NavigationParameters();
            param.Add(StaticInfo.NotificationTransitParamKey, id);
            vModel.NavigateAsync("WebBrowsePage", param);
            //var param = new NavigationParameters();
            //param.Add(StaticInfo.AuctionUrlTransitParamKey, "https://www.yahoo.co.jp");
            //AppDelegate.StaticApp.NavigateAsync("WebBrowsePage", param);

        }

    }
}
