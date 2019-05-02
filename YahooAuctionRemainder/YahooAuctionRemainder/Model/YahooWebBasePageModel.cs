using System;
using Prism.Mvvm;

namespace YahooAuctionRemainder.Model
{
    public class YahooWebBasePageModel : BindableBase
    {

        /// <summary>
        /// ログインページ
        /// </summary>
        private const string LoginPageUrl = @"https://login.yahoo.co.jp/config/login?";

        public YahooWebBasePageModel()
        {
            
        }

        /// <summary>
        /// ログインページかどうか
        /// </summary>
        /// <returns><c>true</c>, if login page was ised, <c>false</c> otherwise.</returns>
        /// <param name="url">URL.</param>
        public bool IsLoginPage(string url)
        {
            if (url.StartsWith(LoginPageUrl))
            {
                return true;
            }
            return false;
        }
    }
}
