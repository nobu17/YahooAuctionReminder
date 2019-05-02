using Foundation;
using Prism;
using Prism.Ioc;
using UIKit;
using UserNotifications;

namespace YahooAuctionRemainder.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Ask the user for permission to get notifications on iOS 10.0+
                UNUserNotificationCenter.Current.RequestAuthorization(
                    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (approved, error) => { });

                // Watch for notifications while app is active
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // Ask the user for permission to get notifications on iOS 8.0+
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            global::Xamarin.Forms.Forms.Init();
            StaticApp = new App(new iOSInitializer());
            LoadApplication(StaticApp);

            return base.FinishedLaunching(app, options);
        }

        public static App StaticApp;

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            //Debug.WriteLine("Location Notification: {0}:{1}", notification.AlertAction, notification.AlertBody);
            //Debug.WriteLine("Location Notification: " + notification.AlertBody);

            if (UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active)
            {
                //new UIAlertView(notification.AlertAction, notification.AlertBody, null, "OK", null).Show();

                //var alert = UIAlertController.Create(notification.AlertAction, notification.AlertBody, UIAlertControllerStyle.Alert);
                //UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
            }
        }


    }

    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {

        }
    }
}
