using System;
using System.Linq;
using System.Collections.Generic;

using YahooAuctionRemainder.Data;
using System.Threading.Tasks;
using YahooAuctionRemainder.DService;
using Xamarin.Forms;

namespace YahooAuctionRemainder.Services
{
    public interface INotificationForLimit
    {
        void AddAlerm(AlermTarget target);
        void RemoveAlerm(AlermTarget target);
        //Task ClearAllNotification();
        string GetAuctionUrlFromNotificationId(string key);
        void UpdateCurrentAlermState(IEnumerable<AlermTarget> alermTargetList);
        IEnumerable<AlermTarget> GetCurrentAlemList();
        Task<IEnumerable<LocalNotifyData>> GetCurrentNotifycationList();
        //Task<IEnumerable<Notification>> GetCurrentNotificationList();
    }

    public class NotificationService : INotificationForLimit
    {
        private const string AlermTilte = "オークション終了通知";

        private const string AlermMessage = "もうすぐオークションが終了します。{0}";
            
        private readonly ISettingService _settingService;

        private readonly ILocalNotifyService _localNotifyService;

        private static Dictionary<int, AlermTarget> _alermList = new Dictionary<int, AlermTarget>();

        public static object LockObj = new object();

        private Random _random = new Random();

        public NotificationService(ISettingService settingService)
        {
            _settingService = settingService;
            _localNotifyService = DependencyService.Get<ILocalNotifyService>();
        }

        public string GetAuctionUrlFromNotificationId(string key)
        {
            //オークションIDからURL
            return "https://page.auctions.yahoo.co.jp/jp/auction/"+key;
        }



        /// <summary>
        /// アラームを追加します
        /// </summary>
        /// <param name="target">Target.</param>
        public void AddAlerm(AlermTarget target)
        {
            //リストに無ければ登録
            var existTarge = _alermList.FirstOrDefault(x => x.Value.AuctionId == target.AuctionId);
            //アラームが無ければ追加
            if (existTarge.Value == null)
            {
                //発火可能な日時か?
                var offset = _settingService.RestoreUserSetting().AlermFireOffsetMinutes;
                var fireDate = target.AuctionEndDateTime.AddMinutes(-offset);
                if (fireDate > DateTime.Now)
                {
                    //ユニークなランダムKeyを付与
                    int i = _random.Next();
                    lock (LockObj)
                    {
                        while (true)
                        {
                            if (!_alermList.ContainsKey(i))
                            {
                                target.Id = i;
                                _alermList.Add(i, target);
                                //保存
                                SaveAlermList();
                                break;
                            }
                            i = _random.Next();
                        }
                    }
                    //登録
                    var lData = new LocalNotifyData();
                    lData.Title = AlermTilte;
                    lData.Key = target.AuctionId;
                    lData.Body = string.Format(AlermMessage, target.ItemTitle);
                    lData.ReserveDate = fireDate;

                    _localNotifyService.AddNotify(lData);

                }
                else
                {
                    throw new Exception("直前のため、発火不可能です。");
                }
                ClearExpiredAlermList();
            }
            else
            {
                //変数に保存されているがアラート登録されていない場合があるのでアラート登録だけは実施する
                var lData = new LocalNotifyData();
                lData.Title = AlermTilte;
                lData.Key = existTarge.Value.AuctionId;
                lData.Body = string.Format(AlermMessage, existTarge.Value.ItemTitle);
                lData.ReserveDate = existTarge.Value.AuctionEndDateTime;
            }

        }

        /// <summary>
        /// 現在のアラーム情報を一括で反映します
        /// </summary>
        /// <param name="alermTargetList">Alerm target list.</param>
        public void UpdateCurrentAlermState(IEnumerable<AlermTarget> alermTargetList)
        {
            //アラームはあるが、オークション一覧に無いものを削除する（オークションが終了したものなど）
            var deleteTargets = _alermList.Where(a => !alermTargetList.Any(aL => aL.AuctionId == a.Value.AuctionId));
            foreach(var del in deleteTargets)
            {
                RemoveAlerm(del.Value);
            }

            //アラームに無いがリマインダー入っているものをアラームに登録
            foreach(var alerm in alermTargetList)
            {
                try
                {
                    AddAlerm(alerm);                    
                }
                catch(Exception)
                {
                    //発火不可能なターゲットはここに来る
                }
            }
        }


        /// <summary>
        /// アラームを削除します
        /// </summary>
        /// <param name="target">Target.</param>
        public void RemoveAlerm(AlermTarget target)
        {
            lock(LockObj)
            {
                var rTarget = _alermList.FirstOrDefault(x => x.Value.AuctionId == target.AuctionId);
                //一致すれば削除
                if (!rTarget.Equals(default(Dictionary<int, AlermTarget>)))
                {
                    //一覧から削除して通知を停止
                    //CrossLocalNotifications.Current.Cancel(rTarget.Key);
                    //CrossNotifications.Current.Cancel(rTarget.Key);

                    _localNotifyService.CancelNotify(rTarget.Value.AuctionId);

                    _alermList.Remove(rTarget.Key);
                    //保存
                    SaveAlermList();
                }
            }
            ClearExpiredAlermList();
        }

        /// <summary>
        /// 期限切れのアラーム一覧を削除します
        /// </summary>
        private void ClearExpiredAlermList()
        {
            var now = DateTime.Now;
            lock (LockObj)
            {
                //過去のものは削除
                var expired = _alermList.Where(x => x.Value.AuctionEndDateTime < now);
                foreach(var item in expired)
                {
                    _alermList.Remove(item.Key);
                }
                //保存
                SaveAlermList();
            }
        }

        private void SaveAlermList()
        {
            _settingService.StoreAlermSetting(new AlermSetting() { AlermList = _alermList });
        }

        /// <summary>
        /// 現在のアラーム一覧を取得します
        /// </summary>
        /// <returns>The current alem list.</returns>
        public IEnumerable<AlermTarget> GetCurrentAlemList()
        {
            return _alermList.Select(a => a.Value);
        }

        /// <summary>
        /// 実際の通知予約一覧を取得します
        /// </summary>
        /// <returns>The current notifycation list.</returns>
        public async Task<IEnumerable<LocalNotifyData>> GetCurrentNotifycationList()
        {
            IEnumerable<LocalNotifyData> result = null;
            await Task.Run(() =>
            {
                _localNotifyService.GetNotifycationList((res) =>
                {
                    result = res;
                });
            });

            return result;
        }

    }
}
