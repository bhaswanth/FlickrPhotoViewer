using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MetroFlickr8
{
    class CharmCont:Frame
    {
        public CharmCont()
        {
        }
        public object CharmContent
        {
            get { return (object)GetValue(CharmContentProperty); }
            set { SetValue(CharmContentProperty, value); }
        }

        public static readonly DependencyProperty CharmContentProperty =
            DependencyProperty.Register("CharmContent", typeof(object), typeof(CharmCont), new PropertyMetadata(null));
    }
}
