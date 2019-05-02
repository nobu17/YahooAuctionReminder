using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace YahooAuctionRemainder.ViewModels
{
    public class TestPageViewModel : ViewModelBase
    {
        public TestPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "OK Page";
            //navigationService.NavigateAsync()
        }


        public ICommand Call
        {
            get
            {
                return new Command(() =>
                {
                    NavigationService.NavigateAsync("TestPage2");
                });
            }
        }
    }
}
