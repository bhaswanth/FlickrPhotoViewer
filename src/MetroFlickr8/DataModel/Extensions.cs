using MetroFlickr.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetroFlickr8
{
    public static class Extensions
    {
        public static IEnumerable<FlickrImage> SelectAllImages(this FlickrDataSource datasource)
        {
            return datasource.ImageSets.SelectMany(s => s.Collection).ToList();
        }
    }
}
