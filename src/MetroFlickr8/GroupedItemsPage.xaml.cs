using System;
using System.Collections.Generic;
using MetroFlickr8.Controllers;
using MetroFlickr.Model;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace MetroFlickr8
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : MetroFlickr8.Common.LayoutAwarePage
    {
        public NavigationController NavigationController { get; private set; }

        public GroupedItemsPage()
        {
            this.InitializeComponent();
        }
        public GroupedItemsPage(NavigationController navigationController)
            : this()
        {
            this.NavigationController = navigationController;

            //setting DataContext in code-behind as a bug exists in dev preview
            //http://social.msdn.microsoft.com/Forums/en-US/winappswithcsharp/thread/fab270de-6a35-46f4-bbe1-4fb33dc9b5dc
            pageTitle.DataContext = this.NavigationController;           
         }

        private IEnumerable<Object> _items;
        public IEnumerable<Object> Items
        {
            get
            {
                return this._items;
            }

            set
            {
                this._items = value;
                groupedItemsViewSource.Source = value;
            }
        }

        // View state management for switching among Full, Fill, Snapped, and Portrait states

        private DisplayPropertiesEventHandler _displayHandler;
        private WindowSizeChangedEventHandler _layoutHandler;

        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BackButton.Visibility = (this.DataContext is FlickrDataSource) ? Visibility.Collapsed : Visibility.Visible;

            if (_displayHandler == null)
            {
                _displayHandler = Page_OrientationChanged;
                _layoutHandler = Page_LayoutChanged;
            }

            //get a nasty COM exception when attempting to register these event handlers under the file picker view or calling SetCurrentViewState below
            DisplayProperties.OrientationChanged += _displayHandler;
            Window.Current.SizeChanged += _layoutHandler;

            SetCurrentOrientation(this);
        }

        private void Page_OrientationChanged(object sender)
        {
            SetCurrentOrientation(this);
        }

        private void Page_LayoutChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //back is only enabled when looking at a specific imageset, so go back home
            this.NavigationController.SetView("MetroFlickr", ViewType.Home, null, null, null);
        }

        public void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DisplayProperties.OrientationChanged -= _displayHandler;
            Window.Current.SizeChanged += _layoutHandler;
        }
     

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            
        }

        
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            
              //display the selected image in detail view
                var image = e.ClickedItem as FlickrImage;
                this.NavigationController.SetView(image.Title, ViewType.Detail, image, image.ImageSet.Collection, image);
            
        }

         void ItemView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //display the selected image in detail view
            var image = e.AddedItems[0] as FlickrImage;
            this.NavigationController.SetView(image.Title, ViewType.Detail, image, image.ImageSet.Collection, image);
        }

               
    }
}
