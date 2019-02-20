using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace bomberman
{
    class Player
    {
        protected XYCoordinates centerCoordinate;
        protected Image _image;
        protected int maxBombCount;
        protected int currentBombCount;
        private int _bombStr;



        public int MoveSpeed { get; set; }
        public int BombStr
        {
            get
            {
                return _bombStr;
            }
        }

        protected Player(int x, int y, Image image, string imageSource)
        {
            centerCoordinate = new XYCoordinates(x, y);
            MoveSpeed = 25;
            _image = image;
            _image.Source = new BitmapImage(new Uri($"ms-appx:///{imageSource}"));
            currentBombCount = maxBombCount = 2;
            _bombStr = 2;
        }

        public XYCoordinates GetCenter ()
        {
            return centerCoordinate;
        }

        public bool DropBomb(Board board, out Bomb bomb)
        {
            bomb = null;
            if (currentBombCount > 0 && board.PlaceBomb(this, BombStr, out bomb))
            {
                currentBombCount--;
                return true;
            }
            return false;
        }

        public void GiveBomb()
        {
            currentBombCount++;
        }
    }
}
