using System;
using System.Collections.Generic;
using Prism.Mvvm;
using YahooAuctionRemainder.Data;
using YahooAuctionRemainder.Services;

namespace YahooAuctionRemainder.Model
{
    public class YahooWebPageModel : YahooWebBasePageModel
    {

        /// <summary>
        /// ウオッチリストページ
        /// </summary>
        private const string WatchListPageUrl = @"https://auctions.yahoo.co.jp/openwatchlist/jp";


        /// <summary>
        /// ウォッチリストのベースページ
        /// </summary>
        private const string WatchListPageBaseUrl = @"https://auctions.yahoo.co.jp/openwatchlist/jp/show/mystatus?select=watchlist&watchclosed=0&apg={0}";

        /// <summary>
        /// The yahoo webservice.
        /// </summary>
        private readonly IYahooWebService _yahooWebservice;

        /// <summary>
        /// 現在の読込ページ
        /// </summary>
        private int _currentAuctionPage = 1;

        public YahooWebPageModel(IYahooWebService yahooWebservice)
        {
            _yahooWebservice = yahooWebservice;

            _currentAuctionPage = 1;
            AuctionList = new List<AuctionInfo>();
        }

        #region prop

        /// <summary>
        /// オークション一覧
        /// </summary>
        /// <value>The auction list.</value>
        public List<AuctionInfo> AuctionList { get; set; }

        public string CurrentURL
        {
            get { return string.Format(WatchListPageBaseUrl, _currentAuctionPage); }
        }

        #endregion



        /// <summary>
        /// ウォッチリストかどうか判定
        /// </summary>
        /// <returns><c>true</c>, if watch list page was ised, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        public bool IsWatchListPage(string url)
        {
            if (url.StartsWith(WatchListPageUrl))
            {
                return true;
            }
            return false;
        }

        public string GetProgressMessage()
        {
            return string.Format("{0}ページ目を読込中。", _currentAuctionPage);
        }



        public bool LoadAuctionList(string html)
        {
            var result = _yahooWebservice.LoadAuctionListFromHtml(html);
            //読込無データがある場合
            if(result.Item1)
            {
                AuctionList.AddRange(result.Item2);
                _currentAuctionPage++;
            }

            return result.Item1;
        }
    }
}
