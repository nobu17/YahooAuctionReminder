using System;
using System.Collections.Generic;

namespace YahooAuctionRemainder.Data
{
    /// <summary>
    /// オークションの詳細情報（毎回詠みこむ事を避けるために保存
    /// </summary>
    public class AuctionDetailSetting
    {
        public AuctionDetailSetting()
        {
            StoredDetails = new Dictionary<string, AuctionDetailInfo>();
        }

        /// <summary>
        /// オークションの詳細情報 Key:AutionId
        /// </summary>
        /// <value>The stored details.</value>
        public Dictionary<string,AuctionDetailInfo> StoredDetails
        {
            get;
            set;
        }
    }
}
