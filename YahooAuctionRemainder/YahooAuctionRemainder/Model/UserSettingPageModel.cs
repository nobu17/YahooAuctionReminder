using System;
using Prism.Mvvm;
using YahooAuctionRemainder.Services;

namespace YahooAuctionRemainder.Model
{
    public class UserSettingPageModel : BindableBase
    {
        private readonly ISettingService _settingService;

        public UserSettingPageModel(ISettingService settingService)
        {
            _settingService = settingService;
        }

        /// <summary>
        /// オークション情報のキャッシュをクリアします
        /// </summary>
        public void ClearAuctionCache()
        {
            _settingService.ClearAuctionDetailSetting();
        }

        /// <summary>
        /// トグル情報のキャッシュをクリアします
        /// </summary>
        public void ClearToglleCache()
        {
            _settingService.ClearToggleSetting();
        }
    }
}
