using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MetroFlickr.Model
{
    public class FlickrImage : NotificationObject
    {
        public DateTime Date { get; private set; }

        public string Title { get; set; }

        public string Description { get; set; }        

        public byte[] Image { get; private set; }

        public string ImageUri { get; private set; }

        public string LargeImageUri { get; private set; }       

        public byte[] LargeImage
        {
            get
            {
                return this.LoadFromResource(this.LargeImageUri);
            }
        }

        public FlickrImageSet ImageSet { get; private set; }

        public FlickrImage(FlickrImageSet imageSet, string smallImageUri, string largeImageUri, string title, DateTime date, string Description)
        {
            this.ImageUri = smallImageUri;
            this.LargeImageUri = largeImageUri;

            this.ImageSet = imageSet;
            this.Image = this.LoadFromResource(smallImageUri);
            this.Title = title;
            this.Date = date;
            this.Description = Description;
        }

        public override string ToString()
        {
            return string.Format("[Img] {0} - {1}", this.ImageUri, this.Title);
        }

        public byte[] LoadFromResource(string name)
        {
            WebRequest req = HttpWebRequest.Create(name);

            using (Stream stream = req.GetResponse().GetResponseStream())
            {
                MemoryStream buffer = new MemoryStream();
                stream.CopyTo(buffer);

                return buffer.ToArray();
            }
        }
    }
}
