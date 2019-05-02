using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using YahooAuctionRemainder.Common;
using YahooAuctionRemainder.Data;
using YahooAuctionRemainder.Model;
using YahooAuctionRemainder.Services;

namespace YahooAuctionRemainder.ViewModels
{
    public class AuctionListPageViewModel : ViewModelBase
    {
        /// <summary>
        /// 初期化されたかどうか
        /// </summary>
        private bool _isInitilized = false;

        private readonly IPageDialogService _pageDialogService;

        public AuctionListPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, ISettingService settingService, IAlermListService alermListService, INotificationForLimit notificationService)
            : base(navigationService)
        {
            _model = new AuctionListPageModel(settingService, alermListService, notificationService);
            _pageDialogService = pageDialogService;
        }

        #region prop

        /// <summary>
        /// Model.
        /// </summary>
        private AuctionListPageModel _model;
        public AuctionListPageModel Model
        {
            get { return _model; }
            set
            {
                SetProperty(ref _model, value);
            }
        }


        /// <summary>
        /// The Item List is refreshing.
        /// </summary>
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                SetProperty(ref _isRefreshing, value);
            }
        }

        /// <summary>
        /// Selected Auction Item
        /// </summary>
        private AuctionInfo _selectedItem;
        public AuctionInfo SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }


        #endregion

        /// <summary>
        /// Webサイトからオークション情報を取得します
        /// </summary>
        public void LoadAuctionInfoFromWeb()
        {
            //Web画面へ遷移して読込
            NavigationService.NavigateAsync("YahooWebPage");
        }

        /// <summary>
        /// 各オークションの詳細情報を取得します
        /// </summary>
        public void LoadAuctionDetailInfoFromWeb()
        {
            //詠みこむ必要があれば
            if(Model.IsNeedReadDetail())
            {
                //Web画面へ遷移して読込(Detailが無いものだけ)
                var param = new NavigationParameters();
                param.Add(StaticInfo.YahooWebPageTransitParamKey, new List<AuctionInfo>(Model.GetNotHaveDetailItemList()));
                NavigationService.NavigateAsync("YahooWebForDetailPage", param);             
            }
        }

        public void LoadAuctionDetailFromWeb(IEnumerable<AuctionInfo> auctions)
        {
            //Web画面へ遷移して読込(Detailが無いものだけ)
            var param = new NavigationParameters();
            param.Add(StaticInfo.YahooWebPageTransitParamKey, new List<AuctionInfo>(auctions));
            NavigationService.NavigateAsync("YahooWebForDetailPage", param); 
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            IsRefreshing = false;
            if (parameters.ContainsKey(StaticInfo.AucListTransitParamKey))
            {
                //オークションの情報取得ページから戻ってきた場合
                //取得したオークション情報を表示
                //このタイミングで反映はだめ
                //Model.ItemList = new List<AuctionInfo>(parameters[StaticInfo.AucListTransitParamKey] as IEnumerable<AuctionInfo>);
                //ItemListに対して保存された詳細情報を反映
                //Model.MergeAuctionDetailInfo();
                //ユーザ操作のリマインダ設定を反映
                //Model.ApplyUserToggleState();
                //オークションの詳細情報を取得する
                LoadAuctionDetailFromWeb(parameters[StaticInfo.AucListTransitParamKey] as IEnumerable<AuctionInfo>);
            }
            else if (parameters.ContainsKey(StaticInfo.YahooWebPageTransitParamKey))
            {
                //オークション情報一覧を取り出して、個別ページからオークションを詳細から取り出す

                //オークション詳細情報取得ページから取得したパラメータ
                var updated = parameters[StaticInfo.YahooWebPageTransitParamKey] as IEnumerable<AuctionInfo>;
                //一覧へ反映
                Model.ItemList = new List<AuctionInfo>(updated);
                //保存してあるユーザ操作の反映（トグル状態）
                Model.ApplyUserToggleState();
                //現在のアラームを更新
                Model.UpdateAllAlermState();
                //Model.UpdateItemList(updated);
            }

            base.OnNavigatingTo(parameters);
        }

        #region coomand

        public ICommand Appearing
        {
            get
            {
                return new Command(() =>
                {
                    //初回のみ初期化
                    if(!_isInitilized)
                    {
                        LoadAuctionInfoFromWeb();
                        _isInitilized = true;
                    }
                });
            }
        }

        /// <summary>
        /// 更新タップ時
        /// </summary>
        /// <value>The refreshing.</value>
        public ICommand Refreshing
        {
            get
            {
                return new Command(() =>
                {
                    LoadAuctionInfoFromWeb();
                });
            }
        }

        /// <summary>
        /// トグルボタン押下時
        /// </summary>
        /// <value>The toggled.</value>
        public ICommand Toggled
        {
            get
            {
                return new Command<string>((auctionId) =>
                {
                    try
                    {
                        //対象のオークションのアラーム設定を変更する
                        //Model.UpdateTargetAlert(auctionId);
                        Model.MakeOrRemoveNotify(auctionId);
                    }
                    catch(Exception e)
                    {
                        _pageDialogService.DisplayAlertAsync("エラー", "登録に失敗しました。" + e.Message, "OK");
                    }
                });
            }
        }

        /// <summary>
        /// オークションのアイテムを選択時
        /// </summary>
        /// <value>The item selected.</value>
        public ICommand ItemSelected
        {
            get
            {
                return new Command<AuctionInfo>((item) =>
                {
                    if(item != null)
                    {
                        //オークションの商品ページへ移動して表示する
                        var naviParam = new NavigationParameters();
                        naviParam.Add(StaticInfo.AuctionUrlTransitParamKey, item.AuctionUrl);
                        NavigationService.NavigateAsync("WebBrowsePage", naviParam);
                        SelectedItem = null;
                    }
                });
            }
        }

        #endregion
    }
}
