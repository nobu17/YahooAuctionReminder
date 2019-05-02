using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using YahooAuctionRemainder.Data;
using YahooAuctionRemainder.Services;

namespace YahooAuctionRemainder.Model
{
    public class AuctionListPageModel : BindableBase
    {
        /// <summary>
        /// 設定の読み書きサービス
        /// </summary>
        private readonly ISettingService _settingService;

        private readonly IAlermListService _alermLiistService;

        private readonly INotificationForLimit _notificationService;

        public AuctionListPageModel(ISettingService settingService, IAlermListService alermListService, INotificationForLimit notificationService)
        {
            _settingService = settingService;
            _alermLiistService = alermListService;
            _notificationService = notificationService;
        }

        #region prop

        /// <summary>
        /// The item list.
        /// </summary>
        private List<AuctionInfo> _itemList;
        public List<AuctionInfo> ItemList
        {
            get { return _itemList; }
            set
            {
                SetProperty(ref _itemList, value);
            }
        }



        #endregion


        #region method


        /// <summary>
        /// 期限切れの詳細情報を削除します
        /// </summary>
        private void RemoveOldAuctionDetail()
        {
            //詳細情報を取得
            var stored = _settingService.RestoreAuctionDetailSetting();
            if(stored != null && stored.StoredDetails != null)
            {
                //期限切れを間引く
                stored.StoredDetails = stored.StoredDetails.Where(d => !d.Value.IsExpired()).ToDictionary(d => d.Key, d => d.Value);
                //保存
                _settingService.StoreAuctionDetailSetting(stored);
            }
        }

        /// <summary>
        /// オークション情報をキャッシュとして保存します
        /// </summary>
        private void StoreAuctionDetail()
        {
            var targets = ItemList.Where(a => a.AuctionDetail == null).ToDictionary(i => i.AuctionId, i => i.AuctionDetail);
            if(targets.Any())
            {
                //設定を更新
                var setting = new AuctionDetailSetting();
                setting.StoredDetails = targets;
                _settingService.StoreAuctionDetailSetting(setting);
            }
        }

        /// <summary>
        /// キャッシュしたオークション詳細を一覧へマージします
        /// </summary>
        private void MergeAuctionDetailToItemList()
        {
            //詳細情報を取得
            var stored = _settingService.RestoreAuctionDetailSetting();
            if (stored != null && stored.StoredDetails != null)
            {
                foreach(var item in ItemList)
                {
                    //保存してある設定に詳細があれば設定
                    if(stored.StoredDetails.ContainsKey(item.AuctionId))
                    {
                        item.AuctionDetail = stored.StoredDetails[item.AuctionId];
                    }
                }
            }
        }

        /// <summary>
        /// オークション情報をキャッシュからマージします
        /// </summary>
        public void MergeAuctionDetailInfo()
        {
            //キャッシュ内の期限切れを間引く
            RemoveOldAuctionDetail();
            //設定に保存されている詳細情報を反映
            MergeAuctionDetailToItemList();
        }

        /// <summary>
        /// 詳細情報を持たないオークション情報を取得します
        /// </summary>
        /// <returns>The not have detail item list.</returns>
        public List<AuctionInfo> GetNotHaveDetailItemList()
        {
            return ItemList.Where(a => a.AuctionDetail == null).ToList();
        }

        public void UpdateItemList(IEnumerable<AuctionInfo> updatedList)
        {
            //アップデート対象でないものを取り出す
            var noUpdated = ItemList.Where(i => !updatedList.Any(u => u.IsSameItem(i)));
            ItemList = new List<AuctionInfo>(noUpdated.Concat(updatedList));
            //設定を更新
            StoreAuctionDetail();
        }
        
        /// <summary>
        /// 詳細情報を詠みこむ必要があるかどうか
        /// </summary>
        /// <returns><c>true</c>, if need read detail was ised, <c>false</c> otherwise.</returns>
        public bool IsNeedReadDetail()
        {
            if(ItemList.Any())
            {
                //AuctionDetailがnullなら
                return ItemList.Any(x => x.AuctionDetail == null);
            }
            return false;
        }

        /// <summary>
        /// 対象のオークションIDをreminderから追加もしくは削除します。
        /// </summary>
        /// <param name="targetAuctionId">Target auction identifier.</param>
        public void MakeOrRemoveNotify(string targetAuctionId)
        {
            if (!string.IsNullOrWhiteSpace(targetAuctionId))
            {
                //IDが有ればアラーム更新する
                var targetItem = ItemList.FirstOrDefault(itm => itm.AuctionId == targetAuctionId);
                if (targetItem != null)
                {
                    //オークション情報から通知データを作製
                    var notify = _alermLiistService.MakeNotification(targetItem);
                    //登録の場合
                    if(targetItem.IsReminder)
                    {
                        try
                        {
                            //通知する
                            _notificationService.AddAlerm(notify);
                            //ユーザ操作設定を保存
                            _alermLiistService.StoreToggleState(targetAuctionId, targetItem.IsReminder);
                        }
                        catch(Exception e)
                        {
                            targetItem.IsReminder = false;
                            //ユーザ操作設定を保存
                            _alermLiistService.StoreToggleState(targetAuctionId, targetItem.IsReminder);
                            throw e;
                        }
                    }
                    else
                    {
                        //アラート対象から除外
                        _notificationService.RemoveAlerm(notify);
                    }
                }
            }
        }

        /// <summary>
        /// ユーザ操作によるトリガー有無を反映します。
        /// </summary>
        public void ApplyUserToggleState()
        {
            if(ItemList != null)
            {
                _alermLiistService.ApplyToggleStateToAuctionList(ref _itemList);
            }
        }

        /// <summary>
        /// 現在のアラーム状態を反映させます
        /// </summary>
        public void UpdateAllAlermState()
        {
            if (ItemList != null)
            {
                //現在のリマインダー対象を取得
                var targetList = _alermLiistService.MakeAllAlermList(ItemList);
                //アラームに反映
                _notificationService.UpdateCurrentAlermState(targetList);
            }
        }


        #endregion
    }
}
