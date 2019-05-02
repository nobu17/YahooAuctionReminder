using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YahooAuctionRemainder.Common;
using YahooAuctionRemainder.Data;
using YahooAuctionRemainder.Model;
using YahooAuctionRemainder.Services;

namespace YahooAuctionRemainder.ViewModels
{
    public class YahooWebPageViewModel : YahooWebBaseViewModel
    {
        //後でセキュリティ設定を変えること
        //https://qiita.com/akatsuki174/items/176886ac9f695e2f3d29

        public YahooWebPageViewModel(INavigationService navigationService, IYahooWebService yahooWebService)
            : base(navigationService)
        {
            //navigationService.NavigateAsync()
            _model = new YahooWebPageModel(yahooWebService);
            LoadingMessage = "オークション情報を読み込んでいます。";
            Init();
        }


        #region prop


        /// <summary>
        /// Model.
        /// </summary>
        private YahooWebPageModel _model;
        public YahooWebPageModel Model
        {
            get { return _model; }
            set
            {
                SetProperty(ref _model, value);
            }
        }

        #endregion

        #region coomand

        public ICommand Navigating
        {
            get
            {
                return new Command<WebNavigatingEventArgs>((e) =>
                {
                    //URLに応じて表示を切り替え
                    ChangeStateByUrl(e.Url);
                });
            }
        }

        /// <summary>
        /// ページ遷移後
        /// </summary>
        /// <value>The navigated.</value>
        public ICommand Navigated
        {
            get
            {
                return new Command<WebNavigatedEventArgs>(async (e) =>
                {
                    if(e.Result == WebNavigationResult.Success)
                    {
                        //ウォッチリストならば
                        if (Model.IsWatchListPage(e.Url))
                        {
                            var result = await EvaluateJavascript("document.body.innerHTML");
                            LoadingMessage = Model.GetProgressMessage();
                            if (Model.LoadAuctionList(result))
                            {
                                //URL変更して読込
                                SourceUrl = Model.CurrentURL;
                            }
                            else
                            {
                                IsLoading = false;
                                var navigationParameters = new NavigationParameters();
                                navigationParameters.Add(StaticInfo.AucListTransitParamKey, Model.AuctionList);
                                await NavigationService.GoBackAsync(navigationParameters);
                            }
                        }                        
                    }
                    else
                    {
                        //リトライ
                        LoadingMessage = "読み込み失敗のため、リトライ";
                        System.Threading.Thread.Sleep(500);
                        SourceUrl = e.Url;
                    }

                });
            }
        }

        #endregion


        #region method

        private void ChangeStateByUrl(string url)
        {
            //ログインページならば表示する
            if (Model.IsLoginPage(url))
            {
                IsWebViewVisible = true;
                IsLoading = false;
            }
            else
            {
                IsWebViewVisible = false;
                IsLoading = true;
            }
        }



        private void Init()
        {
            //読込を試して
            SourceUrl = Model.CurrentURL;
        }


        //public override void OnNavigatedTo(NavigationParameters parameters)
        //{
        //    //個別の商品取得か
        //    if (parameters.ContainsKey(StaticInfo.YahooWebPageTransitParamKey))
        //    {
        //        var aucList = parameters[StaticInfo.YahooWebPageTransitParamKey] as List<AuctionInfo>;
        //        if(aucList != null && aucList.Any())
        //        {
                    
        //        }
    
        //    }
        //    else
        //    {
                
        //    }
        //    base.OnNavigatingTo(parameters);
        //}

        #endregion
    }
}
