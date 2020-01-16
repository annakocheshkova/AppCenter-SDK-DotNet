// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Push;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Geolocation;
using Windows.Globalization;
using Windows.Services.Maps;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Contoso.Forms.Demo.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private const string LogTag = "AppCenterXamarinDemo";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // Set the country before initialization occurs so App Center can send the field to the backend.
            // We do not use await here because we don't need to wait for this task to complete,
            // and await would block the UI in App constructor.
            _ = SetCountryCode();
            EventFilterHolder.Implementation = new EventFilterWrapper();
            InitializeComponent();
            Suspending += OnSuspending;
        }

        private static async Task SetCountryCode()
        {
            // The following country code is used only as a fallback for the main implementation.
            // This fallback country code does not reflect the physical device location, but rather the
            // country that corresponds to the culture it uses.
            var countryCode = new GeographicRegion().CodeTwoLetter;
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    var geoLocator = new Geolocator
                    {
                        DesiredAccuracyInMeters = 100
                    };
                    var position = await geoLocator.GetGeopositionAsync();
                    var myLocation = new BasicGeoposition
                    {
                        Longitude = position.Coordinate.Point.Position.Longitude,
                        Latitude = position.Coordinate.Point.Position.Latitude
                    };
                    var pointToReverseGeocode = new Geopoint(myLocation);
                    try
                    {
                        MapService.ServiceToken = Constants.BingMapsAuthKey;
                    }
                    catch (SEHException)
                    {
                        AppCenterLog.Info(LogTag, "Please provide a valid Bing Maps authentication key. For more info see: https://docs.microsoft.com/en-us/windows/uwp/maps-and-location/authentication-key");
                    }
                    var result = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);
                    if (result.Status != MapLocationFinderStatus.Success || result.Locations == null || result.Locations.Count == 0)
                    {
                        break;
                    }

                    // The returned country code is in 3-letter format (ISO 3166-1 alpha-3).
                    // Below we convert it to ISO 3166-1 alpha-2 (two letter).
                    var country = result.Locations[0].Address.CountryCode;
                    countryCode = new GeographicRegion(country).CodeTwoLetter;
                    break;
                case GeolocationAccessStatus.Denied:
                    AppCenterLog.Info(LogTag, "Geolocation access denied. In order to set country code in App Center, enable location service in Windows 10.");
                    break;
                case GeolocationAccessStatus.Unspecified:
                    break;
            }
            AppCenter.SetCountryCode(countryCode);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                Xamarin.Forms.Forms.Init(e);

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }

            Push.CheckLaunchedFromNotification(e);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
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
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
