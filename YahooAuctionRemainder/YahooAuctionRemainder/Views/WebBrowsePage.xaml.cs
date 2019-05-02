using System;
using System.Collections.Generic;

using Xamarin.Forms;
using YahooAuctionRemainder.ViewModels;

namespace YahooAuctionRemainder.Views
{
    public partial class WebBrowsePage : ContentPage
    {

        public WebBrowsePage()
        {
            InitializeComponent();

        }

        private void GoBack<T>(T sender)
        {
            webView.GoBack();
        }

        private void GoFoward<T>(T sender)
        {
            webView.GoForward();
        }

        void Handle_Disappearing(object sender, System.EventArgs e)
        {
            MessagingCenter.Unsubscribe<WebBrowsePageViewModel>(this, "GoBack");
            MessagingCenter.Unsubscribe<WebBrowsePageViewModel>(this, "GoForward");
        }

        void Handle_Appearing(object sender, System.EventArgs e)
        {
            MessagingCenter.Subscribe<WebBrowsePageViewModel>(this, "GoBack", GoBack);
            MessagingCenter.Subscribe<WebBrowsePageViewModel>(this, "GoForward", GoFoward);
        }

    }
}
