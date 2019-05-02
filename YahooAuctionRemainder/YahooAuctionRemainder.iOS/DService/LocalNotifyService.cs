using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using UserNotifications;
using Xamarin.Forms;
using YahooAuctionRemainder.DService;
using YahooAuctionRemainder.iOS.DService;
using YahooAuctionRemainder.iOS.Common;

[assembly: Dependency(typeof(LocalNotifyService))]
namespace YahooAuctionRemainder.iOS.DService
{
    public class LocalNotifyService : ILocalNotifyService
    {
        public LocalNotifyService()
        {
        }

        public void AddNotify(LocalNotifyData data)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(delegate
            {
                var content = new UNMutableNotificationContent();
                content.Title = data.Title;
                content.Subtitle = data.Title;
                content.Body = data.Body;
                content.Sound = UNNotificationSound.Default;


                var reserveDateTime = data.ReserveDate;
                var components = new NSDateComponents();
                components.TimeZone = NSTimeZone.DefaultTimeZone;
                components.Year = reserveDateTime.Year;
                components.Month = reserveDateTime.Month;
                components.Day = reserveDateTime.Day;
                components.Hour = reserveDateTime.Hour;
                components.Minute = reserveDateTime.Minute;
                components.Second = reserveDateTime.Second;
                var calendarTrigger = UNCalendarNotificationTrigger.CreateTrigger(components, false);

                var requestID = data.Key;
                //content.UserInfo = NSDictionary.FromObjectAndKey(new NSString("notifyValue"), new NSString("notifyKey"));
                var request = UNNotificationRequest.FromIdentifier(requestID, content, calendarTrigger);

                //通知登録
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();

                //既に同じ通知があれば削除
                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(new string[] { requestID });

                // ローカル通知を予約する
                UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) => {
                    if (err != null)
                    {
                        throw new Exception(err.LocalizedFailureReason + System.Environment.NewLine + err.LocalizedDescription);
                    }
                });
                //UIApplication.SharedApplication.ApplicationIconBadgeNumber += 1; // アイコン上に表示するバッジの数値
            });
        }

        public void CancelNotify(string key)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(delegate
            {
                //全ての送信待ちの通知を削除する場合
                //UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
                //通知時に設定したキーを元に通知情報をキャンセル
                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(new string[] { key });
            });
        }

        public void GetNotifycationList(Action<IEnumerable<LocalNotifyData>> notifyList)
        {
            UIApplication.SharedApplication.InvokeOnMainThread(delegate
            {
                UNUserNotificationCenter.Current.GetPendingNotificationRequests((reqList) =>
                {
                    var nList = new List<LocalNotifyData>();
                    foreach (UNNotificationRequest req in reqList)
                    {
                        var notfy = new LocalNotifyData();
                        notfy.Title = req.Content.Title;
                        notfy.Body = req.Content.Body;

                        notfy.Key = req.Identifier;
                        var tri = req.Trigger as UNCalendarNotificationTrigger;
                        if(tri != null)
                        {
                            notfy.ReserveDate = tri.NextTriggerDate.ToDateTime();
                        }
                        nList.Add(notfy);
                    }

                    notifyList(nList);
                });
            });
        }

    }
}
