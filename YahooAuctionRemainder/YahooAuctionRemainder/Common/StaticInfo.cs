using System;
namespace YahooAuctionRemainder.Common
{
    public static class StaticInfo
    {
        public const string AucListTransitParamKey = "aucList";

        /// <summary>
        /// オークションのURLをブラウザに渡すためのパラメータKey
        /// </summary>
        public const string AuctionUrlTransitParamKey = "aucUrl";

        /// <summary>
        /// ローカル通知からの遷移パラーメータキー
        /// </summary>
        public const string NotificationTransitParamKey = "notification";

        /// <summary>
        /// 詳細ページからの取得データを渡すためのKey
        /// </summary>
        public const string YahooWebPageTransitParamKey = "ReadDetail";


        public const string UserSettingKey = "UserSetting";

        public const string AlermSettingKey = "AlermSetting";

        public const string AuctionDetailSettingKey = "AuctionDetail";

        public const string ToggleSettingKey = "ToggleSetting";
    }
}
