using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using MetroFlickr.Model;
using Windows.UI.ApplicationSettings;
using MetroFlickr8.Controllers;
using Windows.UI.Popups;

// The Grid App template is documented at http://go.microsoft.com/fwlink/?LinkId=234226

namespace MetroFlickr8
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        

        public App()
        {
            InitializeComponent();
        }

           

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {            
            var page = new MainPage();
            
            Window.Current.Content = page;
            Window.Current.Activate();

            page.SetView(ViewType.Collection);
        }        
    }
    }


