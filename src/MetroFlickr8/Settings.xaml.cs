using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Popups;
using Windows.UI.ApplicationSettings;

using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MetroFlickr;
using System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MetroFlickr8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings
    {

        public Settings()
        {
            this.InitializeComponent();
            //SettingsPane.GetForCurrentView().CommandsRequested += CommandsRequested;
        }


        private DisplayPropertiesEventHandler _displayHandler;

        private WindowSizeChangedEventHandler _layoutHandler;

        public void Page_Loaded(object sender, RoutedEventArgs e)
        {

            object apiKey = null;

            if (Windows.Storage.ApplicationData.Current.RoamingSettings.Values.TryGetValue("FlickrApiKey", out apiKey))
            {
                FlickrApiKey.Text = (string)apiKey;
            }

            object username = null;

            if (Windows.Storage.ApplicationData.Current.RoamingSettings.Values.TryGetValue("FlickrUsername", out username))
            {
                FlickrUsername.Text = (string)username;
            }

            if (_displayHandler == null)
            {
                _displayHandler = Page_OrientationChanged;
                _layoutHandler = Page_LayoutChanged;
            }
            DisplayProperties.OrientationChanged += _displayHandler;
            Window.Current.SizeChanged += _layoutHandler;
            SetCurrentOrientation(this);
        }

        private void TextBox_TextChanged(object sender, RoutedEventArgs e)
        {
            //persist the settings
            if (!Windows.Storage.ApplicationData.Current.RoamingSettings.Values.ContainsKey("FlickrApiKey"))
            {
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values.Add("FlickrApiKey", FlickrApiKey.Text);
            }
            else
            {
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["FlickrApiKey"] = FlickrApiKey.Text;
            }

            if (!Windows.Storage.ApplicationData.Current.RoamingSettings.Values.ContainsKey("FlickrUsername"))
            {
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values.Add("FlickrUsername", FlickrUsername.Text);
            }
            else
            {
                Windows.Storage.ApplicationData.Current.RoamingSettings.Values["FlickrUsername"] = FlickrUsername.Text;
            }
        }

        public void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DisplayProperties.OrientationChanged -= _displayHandler;
            Window.Current.SizeChanged += _layoutHandler;
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
    }
}
