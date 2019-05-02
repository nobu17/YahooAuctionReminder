using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class YahooWebForDetailPageViewModel: YahooWebBaseViewModel
    {
        private IPageDialogService _pageDialogService;

        public YahooWebForDetailPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IYahooWebService yahooWebService, ISettingService settingService)
            : base(navigationService)
        {
            //navigationService.NavigateAsync()
            _pageDialogService = pageDialogService;
            _model = new YahooWebForDetailPageModel(yahooWebService, settingService);
            LoadingMessage = "商品情報を取得します。";
        }

        /// <summary>
        /// Model.
        /// </summary>
        private YahooWebForDetailPageModel _model;
        public YahooWebForDetailPageModel Model
        {
            get { return _model; }
            set
            {
                SetProperty(ref _model, value);
            }
        }


        #region coomand

        public ICommand Appearing
        {
            get
            {
                return new Command<WebNavigatingEventArgs>((e) =>
                {

                    IsWebViewVisible = true;
                    IsLoading = false;
                });
            }
        }

        public ICommand Navigating
        {
            get
            {
                return new Command<WebNavigatingEventArgs>((e) =>
                {
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
                    if(e.Result == WebNavigationResult.Success && e.Url == Model.CurrentURL)
                    {
                        Debug.WriteLine("Success:" +e.Url);
                        //詳細ページならば
                        if (Model.IsItemDetailPage(e.Url))
                        {
                            try
                            {
                                var result = await EvaluateJavascript("document.body.innerHTML");
                                LoadingMessage = Model.GetProgressMessage();
                                if (Model.LoadAuctionDetailInfo(result))
                                {
                                    //URL変更して読込
                                    var url = Model.GetNextUrl();
                                    if (!string.IsNullOrWhiteSpace(url))
                                    {
                                        Debug.WriteLine("NextURL:" + url);
                                        SourceUrl = url;
                                    }
                                    else
                                    {
                                        GoBackAsync();
                                    }
                                }
                                else
                                {
                                    GoBackAsync();
                                }
                            }
                            catch(Exception ex)
                            {
                                await _pageDialogService.DisplayAlertAsync("Error", "解析に失敗しました。"+ ex.Message, "OK");
                            }
 
                        }                
                    }
                    else
                    {
                        //リトライ
                        Debug.WriteLine("Retry:" + e.Url);
                        Debug.WriteLine("RetryResult:" + e.Result);
                        System.Threading.Thread.Sleep(500);
                        if(e.Url == Model.CurrentURL)
                        {
                            Debug.WriteLine("RetrySameUrl:"+e.Url);
                            SourceUrl = e.Url;                      
                        }
                        else
                        {
                            Debug.WriteLine("RetryChangeUrl:" + Model.CurrentURL);
                            SourceUrl = Model.CurrentURL;
                        }

                    }

                });
            }
        }

        #endregion

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
                //IsWebViewVisible = true;
                //IsLoading = false;
                IsWebViewVisible = false;
                IsLoading = true;
            }
        }

        /// <summary>
        /// 元のベースへ戻ります
        /// </summary>
        private void GoBackAsync()
        {
            Model.StoreAuctionDetail();

            IsLoading = false;
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add(StaticInfo.YahooWebPageTransitParamKey, Model.AuctionList ?? new List<AuctionInfo>());
            NavigationService.GoBackAsync(navigationParameters);
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {

            //個別の商品取得か
            if (parameters.ContainsKey(StaticInfo.YahooWebPageTransitParamKey))
            {
                var aucList = parameters[StaticInfo.YahooWebPageTransitParamKey] as List<AuctionInfo>;
                if (aucList != null && aucList.Any())
                {
                    _model.AuctionList = aucList;
                    //保存済みの設定があれば反映
                    _model.ApplyStoredDetail();
                    //URL変更して読込
                    var url = Model.GetNextUrl();
                    if(!string.IsNullOrWhiteSpace(url))
                    {
                        SourceUrl = url;
                    }
                    else
                    {
                        GoBackAsync();
                    }

                    //SourceUrl = Model.CurrentURL;
                }
            }
            else
            {
                GoBackAsync();
            }
            base.OnNavigatingTo(parameters);
      
        }

    }
}
