using System;
using System.Linq;
using Prism.Mvvm;

namespace YahooAuctionRemainder.Data
{
    public class AuctionInfo : BindableBase
    {
        /// <summary>
        /// 商品名
        /// </summary>
        private string _itemTitle;
        public string ItemTitle
        {
            get { return _itemTitle; }
            set
            {
                SetProperty(ref _itemTitle, value);
            }
        }

        /// <summary>
        /// 現在価格
        /// </summary>
        private string _currentPrice;
        public string CurrentPrice
        {
            get { return _currentPrice; }
            set
            {
                SetProperty(ref _currentPrice, value);
            }
        }

        /// <summary>
        /// 残り時間文字
        /// </summary>
        private string _leftTimeString;
        public string LeftTimeString
        {
            get { return _leftTimeString; }
            set
            {
                SetProperty(ref _leftTimeString, value);
            }
        }

        /// <summary>
        /// 入札状況
        /// </summary>
        private string _bidStutas;
        public string BidStutas
        {
            get { return _bidStutas; }
            set
            {
                SetProperty(ref _bidStutas, value);
            }
        }

        /// <summary>
        /// 画像URL
        /// </summary>
        private string _imageUrl;
        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                SetProperty(ref _imageUrl, value);
            }
        }

        /// <summary>
        /// オークションURL
        /// </summary>
        private string _auctionUrl;
        public string AuctionUrl
        {
            get { return _auctionUrl; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _auctionId = value.Split('/').Last();
                else
                    _auctionId = string.Empty;
                                       
                SetProperty(ref _auctionUrl, value);
            }
        }

        /// <summary>
        /// リマインダー対象かどうか
        /// </summary>
        private bool _isReminder;
        public bool IsReminder
        {
            get { return _isReminder; }
            set
            {
                SetProperty(ref _isReminder, value);
            }
        }

        /// <summary>
        /// オークションID
        /// </summary>
        private string _auctionId;
        public string AuctionId
        {
            get{ return _auctionId; }
        }

        /// <summary>
        /// オークション詳細
        /// </summary>
        private AuctionDetailInfo _auctionDetail;
        public AuctionDetailInfo AuctionDetail
        {
            get { return _auctionDetail; }
            set
            {
                SetProperty(ref _auctionDetail, value);
            }
        }

        /// <summary>
        /// 同じアイテムか判定します
        /// </summary>
        /// <returns><c>true</c>, if same item was ised, <c>false</c> otherwise.</returns>
        /// <param name="auc">Auc.</param>
        public bool IsSameItem(AuctionInfo auc)
        {
            if(AuctionId == auc.AuctionId)
            {
                return true;
            }
            return false;
        }

    }
}
