using Prism;
using Prism.Ioc;
using YahooAuctionRemainder.ViewModels;
using YahooAuctionRemainder.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.DryIoc;
using YahooAuctionRemainder.Services;
using Prism.Navigation;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace YahooAuctionRemainder
{
    public partial class App : PrismApplication
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<YahooWebPage>();
            containerRegistry.RegisterForNavigation<WebBrowsePage>();
            containerRegistry.RegisterForNavigation<UserSettingPage>();
            containerRegistry.RegisterForNavigation<YahooWebForDetailPage>();
            containerRegistry.RegisterForNavigation<AlermStatePage>();

            containerRegistry.RegisterForNavigation<TestPage2>();

            containerRegistry.Register(typeof(IYahooWebService), typeof(YahooWebService));
            containerRegistry.Register(typeof(ISettingService), typeof(SettingService));
            containerRegistry.Register(typeof(IAlermListService), typeof(AlermListService));
            containerRegistry.Register(typeof(INotificationForLimit), typeof(NotificationService));

        }


    }
}
