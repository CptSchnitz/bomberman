using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace bomberman
{
    class Crate : Tile
    {
        public Crate (Image image) : base (image)
        {
            _image.Source = new BitmapImage(new Uri("ms-appx:///Assets/crate.png"));
        }

        public override bool IsDestructable
        {
            get
            {
                return true;
            }
        }
        public override bool IsPassable()
        {
            return false;
        }
    }
}
