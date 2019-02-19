using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

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

        public Image GetImage
        {
            get
            {
                return _image;
            }
        }



        public virtual bool IsPassable()
        {
            return true;
        }

        public virtual bool BlocksExplosion
        {
            get
            {
                return false;
            }
        }


    }
}
