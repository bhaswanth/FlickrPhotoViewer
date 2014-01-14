using System;
using System.Collections.Generic;
using System.Linq;
using MetroFlickr8.Controllers;
using MetroFlickr.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Printing;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Printing;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace MetroFlickr8
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage : MetroFlickr8.Common.LayoutAwarePage
    {
        public NavigationController NavigationController { get; private set; }

        private PropertySet _flipState = new PropertySet();
        

        public ItemDetailPage()
        {
            InitializeComponent();
            _flipState["CanFlipNext"] = false;
            _flipState["CanFlipPrevious"] = false;

        }

        public ItemDetailPage(NavigationController navigationController)
            : this()
        {
            this.NavigationController = navigationController;
            pageTitle.DataContext = this.NavigationController; 
        }

        void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationController.SetView("MetroFlickr", ViewType.Home, null, null, null);
        }

        

       
        void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var associatedImageSet = (this.DataContext as FlickrImage).ImageSet;
            this.NavigationController.SetView(associatedImageSet.Title, ViewType.Collection, associatedImageSet, associatedImageSet.Collection, null);
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
                itemsViewSource.Source = value;
                pageTitle.DataContext = value;
            }
        }

        private FlickrImage _CurrentImage;

        public Object Item
        {
            get
            {
                return flipView.SelectedItem;
            }

            set
            {
                flipView.SelectedItem = value;

                if (value is FlickrImage)
                {
                    _CurrentImage = value as FlickrImage;
                }
            }
        }

        // Mirror the flipper controls in the application bar

        void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _flipState["CanFlipNext"] = CanFlipNext;
            _flipState["CanFlipPrevious"] = CanFlipPrevious;

            if (e.AddedItems.Count == 1 && e.AddedItems[0] is FlickrImage)
            {
                _CurrentImage = e.AddedItems[0] as FlickrImage;
            }
        }

        bool CanFlipPrevious
        {
            get { return flipView.SelectedIndex > 0; }
        }

        bool CanFlipNext
        {
            get { return flipView.SelectedIndex < (flipView.Items.Count - 1); }
        }

        void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanFlipPrevious) flipView.SelectedIndex -= 1;
        }

        void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanFlipNext) flipView.SelectedIndex += 1;
        }

        private DisplayPropertiesEventHandler _displayHandler;

        private WindowSizeChangedEventHandler _layoutHandler;

        private List<Control> viewStateAwareControls = new List<Control>();

        public void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Workaround: the initial selection for a FlipView isn't respected when it's made before the page is loaded,
            // so it's necessary to clear its state and try again once the page is loaded.  Clearly this is not intended
            // to be necessary.
            if (sender == this)
            {
                var originallySelectedItem = Item;
                Item = null;
                UpdateLayout();
                Item = originallySelectedItem;
            }

            var control = sender as Control;
            if (viewStateAwareControls.Count == 0)
            {
                if (_displayHandler == null)
                {
                    _displayHandler = Page_OrientationChanged;
                    _layoutHandler = Page_LayoutChanged;
                }
                DisplayProperties.OrientationChanged += _displayHandler;
                Window.Current.SizeChanged += _layoutHandler;
            }
            viewStateAwareControls.Add(control);
            SetCurrentViewState(control);
        }

        public void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            viewStateAwareControls.Remove(sender as Control);
            if (viewStateAwareControls.Count == 0)
            {
                DisplayProperties.OrientationChanged -= _displayHandler;
                Window.Current.SizeChanged += _layoutHandler;
            }
        }

        private void Page_LayoutChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            foreach (var control in viewStateAwareControls)
            {
                SetCurrentViewState(control);
            }
        }

        private void Page_OrientationChanged(object sender)
        {
            foreach (var control in viewStateAwareControls)
            {
                SetCurrentViewState(control);
            }
        }

        private void SetCurrentViewState(Control viewStateAwareControl)
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
