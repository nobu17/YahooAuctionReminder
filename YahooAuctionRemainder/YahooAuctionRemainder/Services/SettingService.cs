using System;
using Xamarin.Forms;
using YahooAuctionRemainder.Common;
using YahooAuctionRemainder.Data;

namespace YahooAuctionRemainder.Services
{
    public interface ISettingService
    {
        void StoreUserSetting(UserSetting userSetting);
        UserSetting RestoreUserSetting();

        void StoreAlermSetting(AlermSetting alermSetting);
        AlermSetting RestoreAlermSetting();

        void StoreAuctionDetailSetting(AuctionDetailSetting detailSetting);
        AuctionDetailSetting RestoreAuctionDetailSetting();
        void ClearAuctionDetailSetting();

        void StoreToggleSetting(UserToggleSetting userToggleSetting);
        UserToggleSetting RestoreToggleSetting();
        void ClearToggleSetting();

        void ClearSetting();
    }

    /// <summary>
    /// ユーザ設定処理サービス
    /// </summary>
    public class SettingService : ISettingService
    {
        public SettingService()
        {
        }

        /// <summary>
        /// ユーザ設定をリストアして取得します
        /// </summary>
        /// <returns>The setting.</returns>
        public UserSetting RestoreUserSetting()
        {
            var usr = GetStoredData<UserSetting>(StaticInfo.UserSettingKey);
            if(usr == null)
            {
                var newMake = new UserSetting();
                StoreUserSetting(newMake);
                return newMake;
            }
            return usr;
        }

        /// <summary>
        /// ユーザ設定を保存します
        /// </summary>
        /// <param name="userSetting">User setting.</param>
        public void StoreUserSetting(UserSetting userSetting)
        {
            StoreSetting<UserSetting>(StaticInfo.UserSettingKey, userSetting);
        }


        /// <summary>
        /// アラーム設定を取得します
        /// </summary>
        /// <returns>The alerm setting.</returns>
        public AlermSetting RestoreAlermSetting()
        {
            var usr = GetStoredData<AlermSetting>(StaticInfo.AlermSettingKey);
            if (usr == null)
            {
                var newMake = new AlermSetting();
                StoreAlermSetting(newMake);
                return newMake;
            }
            return usr;
        }

        /// <summary>
        /// アラーム設定を保存します
        /// </summary>
        /// <param name="alermSetting">Alerm setting.</param>
        public void StoreAlermSetting(AlermSetting alermSetting)
        {
            StoreSetting<AlermSetting>(StaticInfo.AlermSettingKey, alermSetting);
        }

        /// <summary>
        /// オークション詳細情報を保存します
        /// </summary>
        /// <param name="detailSetting">Detail setting.</param>
        public void StoreAuctionDetailSetting(AuctionDetailSetting detailSetting)
        {
            StoreSetting<AuctionDetailSetting>(StaticInfo.AuctionDetailSettingKey, detailSetting);
        }

        /// <summary>
        /// オークション詳細情報を取得します
        /// </summary>
        /// <returns>The auction detail setting.</returns>
        public AuctionDetailSetting RestoreAuctionDetailSetting()
        {
            var usr = GetStoredData<AuctionDetailSetting>(StaticInfo.AuctionDetailSettingKey);
            if (usr == null)
            {
                var newMake = new AuctionDetailSetting();
                StoreAuctionDetailSetting(newMake);
                return newMake;
            }
            return usr;
        }

        /// <summary>
        /// Clears the auction detail setting.
        /// </summary>
        public void ClearAuctionDetailSetting()
        {
            if (Application.Current.Properties.ContainsKey(StaticInfo.AuctionDetailSettingKey))
            {
                Application.Current.Properties.Remove(StaticInfo.AuctionDetailSettingKey);
            }
        }

        /// <summary>
        /// Stores the toggle setting.
        /// </summary>
        /// <param name="userToggleSetting">User toggle setting.</param>
        public void StoreToggleSetting(UserToggleSetting userToggleSetting)
        {
            StoreSetting<UserToggleSetting>(StaticInfo.ToggleSettingKey, userToggleSetting);
        }

        /// <summary>
        /// Restores the toggle setting.
        /// </summary>
        /// <returns>The toggle setting.</returns>
        public UserToggleSetting RestoreToggleSetting()
        {
            var usr = GetStoredData<UserToggleSetting>(StaticInfo.ToggleSettingKey);
            if (usr == null)
            {
                var newMake = new UserToggleSetting();
                StoreToggleSetting(newMake);
                return newMake;
            }
            return usr;
        }

        public void ClearToggleSetting()
        {
            if (Application.Current.Properties.ContainsKey(StaticInfo.ToggleSettingKey))
            {
                Application.Current.Properties.Remove(StaticInfo.ToggleSettingKey);
            }
        }


        /// <summary>
        /// 設定をクリアします
        /// </summary>
        /// <returns>The setting.</returns>
        public void ClearSetting()
        {
            if (Application.Current.Properties.ContainsKey(StaticInfo.UserSettingKey))
            {
                Application.Current.Properties.Remove(StaticInfo.UserSettingKey);
            }
            if (Application.Current.Properties.ContainsKey(StaticInfo.AlermSettingKey))
            {
                Application.Current.Properties.Remove(StaticInfo.AlermSettingKey);
            }
            if (Application.Current.Properties.ContainsKey(StaticInfo.AuctionDetailSettingKey))
            {
                Application.Current.Properties.Remove(StaticInfo.AuctionDetailSettingKey);
            }
            if (Application.Current.Properties.ContainsKey(StaticInfo.UserSettingKey))
            {
                Application.Current.Properties.Remove(StaticInfo.UserSettingKey);
            }
            if (Application.Current.Properties.ContainsKey(StaticInfo.ToggleSettingKey))
            {
                Application.Current.Properties.Remove(StaticInfo.ToggleSettingKey);
            }
        }



        /// <summary>
        /// Stores the setting.
        /// </summary>
        /// <param name="storeKey">Store key.</param>
        /// <param name="storeObj">Store object.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private void StoreSetting<T>(string storeKey, T storeObj)
        {
            var json = JSonUtil.SerializeToJson(storeObj);
            if (Application.Current.Properties.ContainsKey(storeKey))
            {
                Application.Current.Properties[storeKey] = json;
            }
            else
            {
                Application.Current.Properties.Add(storeKey, json);
            }
        }

        /// <summary>
        /// Gets the stored data.
        /// </summary>
        /// <returns>The stored data.</returns>
        /// <param name="storeKey">Store key.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private T GetStoredData<T>(string storeKey) where T : class
        {
            if (Application.Current.Properties.ContainsKey(storeKey))
            {
                var json = Application.Current.Properties[storeKey] as string;
                return JSonUtil.DeserializeFromJson<T>(json);
            }
            else
            {
                return null;
            }
        }
    }
}
