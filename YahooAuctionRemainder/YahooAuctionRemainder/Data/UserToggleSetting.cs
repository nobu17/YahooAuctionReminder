using System;
using System.Collections.Generic;

namespace YahooAuctionRemainder.Data
{
    /// <summary>
    /// ユーザーがトグルで保存した設定
    /// </summary>
    public class UserToggleSetting
    {
        public UserToggleSetting()
        {
            AuctionToggleList = new Dictionary<string, bool>();   
        }

        public Dictionary<string,bool> AuctionToggleList
        {
            get;
            set;
        }
    }
}
