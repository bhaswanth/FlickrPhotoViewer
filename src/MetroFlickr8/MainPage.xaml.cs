using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroFlickr8.Controllers;
using MetroFlickr.Model;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;


namespace MetroFlickr8
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : MetroFlickr8.Common.LayoutAwarePage
    {
        private NavigationController _NavigationController;

        public MainPage()
        {
            this.InitializeComponent();
            SettingsPane.GetForCurrentView().CommandsRequested += CommandsRequested;
        }
        private void CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            UICommandInvokedHandler handler = new UICommandInvokedHandler(onSettingsCommand);
            args.Request.ApplicationCommands.Add(new SettingsCommand("a", "Preferences", handler));
            //args.Request.ApplicationCommands.Add(new SettingsCommand("s", "My Settings",handler));
        }
        void onSettingsCommand(IUICommand command)
        {
            //SettingsCommand settingsCommand = (SettingsCommand)command;


            SettingsView.Visibility = Visibility.Visible;


        }

        public void show1()
        {
        }
        private DisplayPropertiesEventHandler _displayHandler;

        private WindowSizeChangedEventHandler _layoutHandler;


        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_displayHandler == null)
            {
                _displayHandler = Page_OrientationChanged;
                _layoutHandler = Page_LayoutChanged;
            }
            DisplayProperties.OrientationChanged += _displayHandler;

            Window.Current.SizeChanged += _layoutHandler;
            SetCurrentOrientation(this);
        }

        private ViewType _RequestedView;

        public void SetView(ViewType viewType)
        {
            _RequestedView = viewType;

            object username = null;
            object apiKey = null;

            var settingsViewRequired = !Windows.Storage.ApplicationData.Current.RoamingSettings.Values.TryGetValue("FlickrUsername", out username) || !Windows.Storage.ApplicationData.Current.RoamingSettings.Values.TryGetValue("FlickrApiKey", out apiKey);

            if (string.IsNullOrWhiteSpace((string)username) || string.IsNullOrWhiteSpace((string)apiKey))
            {
                settingsViewRequired = true;
            }

            if (settingsViewRequired)
            {
                SettingsPane.Show();
            }
            else
            {
                _RunApp((string)username, (string)apiKey);
            }
        }

        public void LayoutRoot_PointerPressed(object sender, PointerRoutedEventArgs args)
        {
            //verify the settings have been entered and if so, launch the conventional navigation
            var canRun = true;
            object apiKey = null;

            if (!Windows.Storage.ApplicationData.Current.RoamingSettings.Values.TryGetValue("FlickrApiKey", out apiKey) || string.IsNullOrWhiteSpace((string)apiKey))
            {
                canRun = false;
            }

            object username = null;

            if (!Windows.Storage.ApplicationData.Current.RoamingSettings.Values.TryGetValue("FlickrUsername", out username) || string.IsNullOrWhiteSpace((string)username))
            {
                canRun = false;
            }

            if (canRun)
            {
                _RunApp((string)username, (string)apiKey);
            }
            else
            {
                //  var dialog = new MessageDialog("MetroFlickr requires an API key and a username in order to run. Please enter these in the settings charm under Preferences");
                // dialog.ShowAsync();
                SettingsView.Visibility = Visibility.Collapsed;
            }
        }



        private void Page_LayoutChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            SetCurrentOrientation(this);
        }

        private void Page_OrientationChanged(object sender)
        {
            SetCurrentOrientation(this);
        }

        private void SetCurrentOrientation(Control viewStateAwareControl)
        {
            VisualStateManager.GoToState(viewStateAwareControl, this.GetViewState(), false);
        }

        private String GetViewState()
        {
            var orientation = DisplayProperties.CurrentOrientation;
            if (orientation == DisplayOrientations.Portrait ||
                orientation == DisplayOrientations.PortraitFlipped) return "Portrait";
            var layout = ApplicationView.Value;
            if (layout == ApplicationViewState.Filled) return "Fill";
            if (layout == ApplicationViewState.Snapped) return "Snapped";
            return "Full";
        }

        private void _RunApp(string username, string apiKey)
        {
            _NavigationController = new NavigationController(username, apiKey);
            _NavigationController.SetView("MetroFlickr", _RequestedView, null, null, null);
        }

        public void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DisplayProperties.OrientationChanged -= _displayHandler;
            Window.Current.SizeChanged += _layoutHandler;
        }



    }
}
