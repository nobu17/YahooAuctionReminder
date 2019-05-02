using System;
using System.Collections.Generic;

namespace YahooAuctionRemainder.Data
{
    /// <summary>
    /// アラーム設定
    /// </summary>
    public class AlermSetting
    {
        public AlermSetting()
        {
        }

        /// <summary>
        /// アラーム設定
        /// </summary>
        /// <value>The alerm list.</value>
        public Dictionary<int, AlermTarget> AlermList
        {
            get;
            set;
        }
    }
}
