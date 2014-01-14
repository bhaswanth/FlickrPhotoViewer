using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace MetroFlickr8
{
    public class ByteToImageSourceValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            InMemoryRandomAccessStream s = new InMemoryRandomAccessStream();

            byte[] bytes = (byte[])value;
            Stream stream = s.AsStreamForWrite();
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);


            BitmapImage source = new BitmapImage();
            source.SetSource(s);

            return source;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
