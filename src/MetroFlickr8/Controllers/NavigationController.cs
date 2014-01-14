﻿using System.Collections.Generic;
using MetroFlickr.Model;
using Windows.UI.ApplicationSettings;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using System.Linq;
using System;
namespace MetroFlickr8.Controllers
{
    public class NavigationController //: NotificationObject <- get a compile error when this code is enabled, temporarily disabling for now
    {
        private string _Breadcrumb;

        public string Breadcrumb
        {
            get { return _Breadcrumb; }
            set
            {
                if (_Breadcrumb == value)
                {
                    return;
                }

                _Breadcrumb = value;

                //base.OnPropertyChanged("Breadcrumb");
            }
        }

        public ViewType CurrentViewType { get; private set; }

        public FlickrDataSource DataSource { get; private set; }

        public NavigationController(string username, string apiKey)
        {
            this.DataSource = new FlickrDataSource(username, apiKey);
        }

        public async void SetView(string title, ViewType type, object context, IEnumerable<object> items, object selectedItem)
        {
            switch (type)
            {
                case ViewType.Collection:
                    {
                        this.Breadcrumb = title;

                        var page = new GroupedItemsPage(this);
                        page.DataContext = this.DataSource;

                        Window.Current.Content = page;
                        Window.Current.Activate();

                        await this.DataSource.LoadAsync(page.Dispatcher);
                        _UpdateTile();
                        page.Items = this.DataSource.ImageSets;

                        break;
                    }
                case ViewType.Home:
                    {
                        var page = new MainPage();
                        page.DataContext = context;

                        Window.Current.Content = page;
                        Window.Current.Activate();

                        break;
                    }
                case ViewType.Detail:
                    {
                        this.Breadcrumb = string.Format("{0} -> {1}", "MetroFlickr", title);

                        var page = new ItemDetailPage(this);
                        page.DataContext = context;
                        page.Items = items;
                        page.Item = selectedItem;

                        Window.Current.Content = page;
                        Window.Current.Activate();

                        break;
                    }

            }

            this.CurrentViewType = type;

        }

        private void _UpdateTile()
        {
            var tileTitle = "MetroFlickr";

            if (!string.IsNullOrWhiteSpace(this.DataSource.Username))
            {
                tileTitle = string.Format("{0}'s photos", this.DataSource.Username);
            }

            var template = Windows.UI.Notifications.TileUpdateManager.GetTemplateContent(Windows.UI.Notifications.TileTemplateType.TileWidePeekImageCollection01);

            var images = template.GetElementsByTagName("image");

            var orderedImagesFromSource = this.DataSource.SelectAllImages().OrderBy(o => o.Date);

            if (orderedImagesFromSource.Count() < images.Length)
            {
                template = Windows.UI.Notifications.TileUpdateManager.GetTemplateContent(Windows.UI.Notifications.TileTemplateType.TileWideImageAndText01);
            }

            var imageSourceArray = orderedImagesFromSource.ToArray();

            if (imageSourceArray.Count() != 0)
            {

                for (int i = 0; i < images.Length; i++)
                {
                    images[i].Attributes.GetNamedItem("src").NodeValue = imageSourceArray[i].ImageUri;
                }
            }

            var text = template.GetElementsByTagName("text");
            text[0].AppendChild(template.CreateTextNode(tileTitle));

            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.Update(new TileNotification(template));
        }
    }
}
