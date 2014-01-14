using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;

namespace MetroFlickr.Model
{
    public class FlickrImageSet : NotificationObject//, IGroupInfo
    {
        public string Title { get; set; }

        public string Subtitle
        {
            get { return this.Title; }
        }

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public byte[] Image { get; set; }

        public IList<FlickrImage> Collection { get; set; }
        
        public FlickrImageSet(string imageUri, string title, DateTime date, string description)
        {
            this.Collection = new List<FlickrImage>();

            this.Image = this.LoadFromResource(imageUri);
            this.Title = title;
            this.Date = date;
            this.Description = string.IsNullOrWhiteSpace(description) ? title : description;
        }

        public override string ToString()
        {
            return string.Format("[ImgSet] {0} - {1} images", this.Title, this.Collection.Count);
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

       
        //public object Key
        //{
        //    get { return this.Title; }
        //}

        //public IEnumerator<object> GetEnumerator()
        //{
        //    return this.Collection.GetEnumerator();
        //}

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    return this.Collection.GetEnumerator();
        //}
    }
}
