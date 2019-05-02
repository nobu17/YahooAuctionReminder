using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Prism.Mvvm;
using YahooAuctionRemainder.Data;
using YahooAuctionRemainder.Services;

namespace YahooAuctionRemainder.Model
{
    public class YahooWebForDetailPageModel : YahooWebBasePageModel
    {
        /// <summary>
        /// 設定の読み書きサービス
        /// </summary>
        private readonly ISettingService _settingService;

        /// <summary>
        /// 個別商品のページ
        /// </summary>
        private const string ItemDetailPageUrl = @"https://page.auctions.yahoo.co.jp/jp/auction/";

        /// <summary>
        /// 個別商品ベースページ
        /// </summary>
        private const string ItemDetailPageBaseUrl = @"https://page.auctions.yahoo.co.jp/jp/auction/{0}";

        /// <summary>
        /// The yahoo webservice.
        /// </summary>
        private readonly IYahooWebService _yahooWebservice;

        /// <summary>
        /// 現在読み込み中のオークションのインデックス
        /// </summary>
        private int _currentIndex;

        public YahooWebForDetailPageModel(IYahooWebService yahooWebservice, ISettingService settingService)
        {
            _yahooWebservice = yahooWebservice;
            _settingService = settingService;
        }

        /// <summary>
        /// オークション一覧
        /// </summary>
        /// <value>The auction list.</value>
        public List<AuctionInfo> AuctionList { get; set; }


        public string CurrentURL
        {
            get { return string.Format(ItemDetailPageBaseUrl, AuctionList[_currentIndex].AuctionId); }
        }

        /// <summary>
        /// 商品詳細ページかどうか
        /// </summary>
        /// <returns><c>true</c>, if item detail page was ised, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        public bool IsItemDetailPage(string url)
        {
            if (url.StartsWith(ItemDetailPageUrl))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 進捗メッセージを取得します
        /// </summary>
        /// <returns>The progress message.</returns>
        public string GetProgressMessage()
        {
            return string.Format("{0}/{1}", _currentIndex+1, AuctionList.Count);
        }

        /// <summary>
        /// HTMLからオークション情報を取得します
        /// </summary>
        /// <returns><c>true</c>, if auction detail info was loaded, <c>false</c> otherwise.</returns>
        /// <param name="html">Html.</param>
        public bool LoadAuctionDetailInfo(string html)
        {
            //詳細情報を取得
            if(AuctionList.Count > (_currentIndex))
            {
                var result = _yahooWebservice.GetDetailedAuctionInfo(html);
                AuctionList[_currentIndex].AuctionDetail = result;
                Debug.WriteLine("EndTime:" + result.AuctionEndDateTime);
                //_currentIndex++;   
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetNextUrl()
        {
            while (AuctionList[_currentIndex].AuctionDetail != null)
            {
                //詳細情報を取得
                if (AuctionList.Count > (_currentIndex + 1))
                {
                    _currentIndex++;
                }
                else
                {
                    return string.Empty;
                }
            }
            return string.Format(ItemDetailPageBaseUrl, AuctionList[_currentIndex].AuctionId);
        }

        public void ApplyStoredDetail()
        {
            RemoveOldAuctionDetail();

            //詳細情報を取得
            var stored = _settingService.RestoreAuctionDetailSetting();
            if (stored != null && stored.StoredDetails != null)
            {
                //保存された情報があるオークション情報を取り出す
                var matchAuc = AuctionList.Where(auc => stored.StoredDetails.Any(st => st.Key == auc.AuctionId));
                if(matchAuc.Any())
                {
                    foreach(var auc in matchAuc)
                    {
                        //詳細を反映
                        auc.AuctionDetail = stored.StoredDetails[auc.AuctionId];
                    }
                }
            }
        }

        /// <summary>
        /// 期限切れの詳細情報を削除します
        /// </summary>
        private void RemoveOldAuctionDetail()
        {
            //詳細情報を取得
            var stored = _settingService.RestoreAuctionDetailSetting();
            if (stored != null && stored.StoredDetails != null)
            {
                //期限切れを間引く
                stored.StoredDetails = stored.StoredDetails.Where(d => !d.Value.IsExpired()).ToDictionary(d => d.Key, d => d.Value);
                //保存
                _settingService.StoreAuctionDetailSetting(stored);
            }
        }

        public void StoreAuctionDetail()
        {
            var targets = AuctionList.Where(a => a.AuctionDetail != null).ToDictionary(i => i.AuctionId, i => i.AuctionDetail);
            if (targets.Any())
            {
                //設定を更新
                var setting = new AuctionDetailSetting();
                setting.StoredDetails = targets;
                _settingService.StoreAuctionDetailSetting(setting);
            }
        }
    }
}
