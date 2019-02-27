using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace bomberman
{
    class Tile
    {
        protected Image _image;

        public Tile(Image image)
        {
            _image = image;
            _image.Source = null;
        }

        protected Tile (Image image, string ImageSource) : this (image)
        {
            _image.Source = _image.Source = new BitmapImage(new Uri($"ms-appx:///{ImageSource}"));
        }

        public Image Image
        {
            get
            {
                return _image;
            }
        }

        public virtual bool Destructable
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsBlocksExplosion
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsPassable(Player player)
        {
            return true;
        }

    }
}
