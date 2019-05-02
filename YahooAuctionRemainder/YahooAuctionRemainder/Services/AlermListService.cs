using System;
using System.Linq;
using System.Collections.Generic;
using YahooAuctionRemainder.Data;


namespace YahooAuctionRemainder.Services
{
    public interface IAlermListService
    {
        List<AlermTarget> MakeAlermList(List<AlermTarget> currentAlerm, IEnumerable<AuctionInfo> auctionList);
        AlermTarget MakeNotification(AuctionInfo auction);
        IEnumerable<AlermTarget> MakeAllAlermList(IEnumerable<AuctionInfo> auctionList);

        void StoreToggleState(string auctionID, bool isTotggled);
        void ApplyToggleStateToAuctionList(ref List<AuctionInfo> auctionList);
    }

    public class AlermListService : IAlermListService
    {
        private readonly ISettingService _settingService;

        public AlermListService(ISettingService settingService)
        {
            _settingService = settingService;
        }

        /// <summary>
        /// トグル状態を保持します
        /// </summary>
        /// <param name="auctionID">Auction identifier.</param>
        /// <param name="isTotggled">If set to <c>true</c> is totggled.</param>
        public void StoreToggleState(string auctionID, bool isTotggled)
        {
            var toggleState = _settingService.RestoreToggleSetting();
            if(!toggleState.AuctionToggleList.ContainsKey(auctionID))
            {
                toggleState.AuctionToggleList.Add(auctionID, isTotggled);
            }
            else
            {
                toggleState.AuctionToggleList[auctionID] = isTotggled;
            }
            _settingService.StoreToggleSetting(toggleState);
        }

        /// <summary>
        /// トグル状態をオークションリストに反映します
        /// </summary>
        /// <param name="auctionList">Auction list.</param>
        public void ApplyToggleStateToAuctionList(ref List<AuctionInfo> auctionList)
        {
            //トグル状態を反映
            var toggleState = _settingService.RestoreToggleSetting();
            foreach(var auc in auctionList)
            {
                if(toggleState.AuctionToggleList.ContainsKey(auc.AuctionId))
                {
                    auc.IsReminder = toggleState.AuctionToggleList[auc.AuctionId];
                }
            }
            //保存状態に反映
            RemoveNotListedToggleState(auctionList);
        }
        /// <summary>
        /// オークション一覧に無いトグル設定を削除します
        /// </summary>
        /// <param name="auctionList">Auction list.</param>
        public void RemoveNotListedToggleState(IEnumerable<AuctionInfo> auctionList)
        {
            var toggleState = _settingService.RestoreToggleSetting();
            //保存した状態でリストに無いものを削除
            toggleState.AuctionToggleList = toggleState.AuctionToggleList.Where(tog => auctionList.Any(auc => auc.AuctionId == tog.Key)).ToDictionary(x => x.Key, x => x.Value);

            _settingService.StoreToggleSetting(toggleState);
        }

        public AlermTarget MakeNotification(AuctionInfo auction)
        {
            var notify = new AlermTarget(auction);
            return notify;
        }

        /// <summary>
        /// アラームリストを作成します
        /// </summary>
        /// <returns>The alerm list.</returns>
        /// <param name="currentAlerm">Current alerm.</param>
        /// <param name="auctionList">Auction list.</param>
        public List<AlermTarget> MakeAlermList(List<AlermTarget> currentAlerm, IEnumerable<AuctionInfo> auctionList)
        {
            var alermList = new List<AlermTarget>();

            foreach(var auc in auctionList)
            {
                //リマインダーに有ればアラーム対象
                if(auc.IsReminder)
                {
                    var newAlerm = new AlermTarget() { AuctionId = auc.AuctionId };
                    alermList.Add(newAlerm);
                }
                else
                {
                    //リマインダーに無くても既存のアラーム対象に有れば対象とする
                    if(currentAlerm != null)
                    {
                        var alerm = currentAlerm.FirstOrDefault(al => al.AuctionId == auc.AuctionId);
                        if(alerm != null)
                        {
                            alermList.Add(alerm);
                        }
                    }
                }
            }

            return alermList;
        }

        /// <summary>
        /// オークション一覧からアラーム情報一覧を作成します
        /// </summary>
        /// <returns>The all alerm list.</returns>
        /// <param name="auctionList">Auction list.</param>
        public IEnumerable<AlermTarget> MakeAllAlermList(IEnumerable<AuctionInfo> auctionList)
        {
            return auctionList.Where(x => x.IsReminder).Select(x => new AlermTarget(x));
        }
    }
}
