using System;
namespace YahooAuctionRemainder.Data
{
    /// <summary>
    /// 個別ページで取るオークション情報
    /// </summary>
    public class AuctionDetailInfo
    {
        public AuctionDetailInfo()
        {
            
        }

        public DateTime AuctionEndDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 期限切れかを判定します
        /// </summary>
        /// <returns><c>true</c>, if expired was ised, <c>false</c> otherwise.</returns>
        public bool IsExpired()
        {
            var now = DateTime.Now;
            if(now > AuctionEndDateTime)
            {
                return true;
            }
            return false;
        }
    }
}
