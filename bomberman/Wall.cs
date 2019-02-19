using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace bomberman
{
    class Wall : Tile
    {
        public Wall(Image image) : base (image)
        {
            _image.Source = new BitmapImage(new Uri("ms-appx:///Assets/wall1.jpg"));
        }

        public override bool IsPassable()
        {
            return false;
        }

        public override bool BlocksExplosion
        {
            get
            {
                return true;
            }
        }
    }
}
