using System;
using System.Collections.Generic;

namespace YahooAuctionRemainder.Data
{
    /// <summary>
    /// ユーザ設定
    /// </summary>
    public class UserSetting
    {
        public UserSetting()
        {
            AlermFireOffsetMinutes = 5;
        }

        /// <summary>
        /// アラーム発火開始前時間
        /// </summary>
        /// <value>The alerm fire offset minutes.</value>
        public int AlermFireOffsetMinutes
        {
            get;
            set;
        }
    }
}
