using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static OneAppAway.ServiceDay;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=402347&clcid=0x409

namespace OneAppAway
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Allows tracking page views, exceptions and other telemetry through the Microsoft Application Insights service.
        /// </summary>
        public static Microsoft.ApplicationInsights.TelemetryClient TelemetryClient;
        private HamburgerBar MainHamburgerBar = new HamburgerBar();
        public Frame RootFrame;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            TelemetryClient = new Microsoft.ApplicationInsights.TelemetryClient();

            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(300, 300));
            Common.SuspensionManager.KnownTypes.Add(typeof(string[]));
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            RootFrame = MainHamburgerBar.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                RootFrame = new Frame();
                Common.SuspensionManager.RegisterFrame(RootFrame, "appFrame");

                RootFrame.NavigationFailed += OnNavigationFailed;

                TransitionCollection transitions = new TransitionCollection();
                transitions.Add(new EntranceThemeTransition() { FromHorizontalOffset = 200 });
                RootFrame.ContentTransitions = transitions;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    await Common.SuspensionManager.RestoreAsync();
                }

                // Place the frame in the current Window
                MainHamburgerBar.Content = RootFrame;
            }

            if (RootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                RootFrame.Navigate(typeof(BusMapPage), "CurrentLocation");
            }
            // Ensure the current window is active
            Window.Current.Content = MainHamburgerBar;
            Window.Current.Activate();
            SetTitleBar();
            //DaySchedule sch = new DaySchedule();
            //sch.LoadFromVerboseString(await ApiLayer.SendRequest("schedule-for-stop/1_320", new Dictionary<string, string>() {["date"] = "2015-07-13" }));
            //string monday = await ApiLayer.SendRequest("schedule-for-stop/1_61378", new Dictionary<string, string>() {["date"] = "2015-07-13" });
            //string tuesday = await ApiLayer.SendRequest("schedule-for-stop/1_431", new Dictionary<string, string>() {["date"] = "2015-07-14" });
            //tuesday.ToString();
            //sch.ToString();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await Common.SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        internal void SetTitleBar()
        {
            Func<Color, Color> darken = clr => Color.FromArgb(clr.A, (byte)(clr.R / 2), (byte)(clr.G / 2), (byte)(clr.B / 2));
            Func<Color, Color> lighten = clr => Color.FromArgb(clr.A, (byte)(128 + clr.R / 2), (byte)(128 + clr.G / 2), (byte)(1287 + clr.B / 2));
            Color accentColor = ((Color)App.Current.Resources["SystemColorControlAccentColor"]);
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Color.FromArgb(255, byte.Parse("05", System.Globalization.NumberStyles.HexNumber), byte.Parse("05", System.Globalization.NumberStyles.HexNumber), byte.Parse("05", System.Globalization.NumberStyles.HexNumber));
            titleBar.InactiveBackgroundColor = titleBar.BackgroundColor;
            //titleBar.BackgroundColor = darken(accentColor);
            titleBar.ForegroundColor = accentColor;
            titleBar.InactiveForegroundColor = darken(accentColor);
            titleBar.ButtonBackgroundColor = titleBar.BackgroundColor;
            titleBar.ButtonForegroundColor = titleBar.ForegroundColor;
            //titleBar.InactiveBackgroundColor = Color.FromArgb(255, byte.Parse("20", System.Globalization.NumberStyles.HexNumber), byte.Parse("20", System.Globalization.NumberStyles.HexNumber), byte.Parse("20", System.Globalization.NumberStyles.HexNumber));
            //titleBar.InactiveBackgroundColor = accentColor;
            //titleBar.InactiveForegroundColor = Colors.White;
            titleBar.ButtonInactiveBackgroundColor = titleBar.InactiveBackgroundColor;
            titleBar.ButtonInactiveForegroundColor = titleBar.InactiveForegroundColor;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (RootFrame.Content is NavigationFriendlyPage)
            {
                e.Handled = ((NavigationFriendlyPage)RootFrame.Content).GoBack();
            }
        }
    }
}
