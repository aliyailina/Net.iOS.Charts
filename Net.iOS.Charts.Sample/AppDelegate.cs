namespace Net.iOS.Charts.Sample;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    public override UIWindow? Window { get; set; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        Window = new UIWindow(UIScreen.MainScreen.Bounds);

        var vc = new DemoListViewController();
        var nvc = new UINavigationController(vc);
        if(UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
        {
            var appearance = new UINavigationBarAppearance();
            appearance.ConfigureWithOpaqueBackground();
            nvc.NavigationBar.StandardAppearance = appearance;
            nvc.NavigationBar.ScrollEdgeAppearance = appearance;
        }
    
        Window.RootViewController = nvc;
        Window.MakeKeyAndVisible();

        return true;
    }
}