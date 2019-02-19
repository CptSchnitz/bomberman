using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace bomberman
{
    class Bomb : Tile
    {
        private int ticksToExplode;
        private bool isArmed;
        private Player owner;

        public Bomb(Image image, Player bombOwner) : base(image)
        {
            _image.Source = new BitmapImage(new Uri("ms-appx:///Assets/bomb.png"));
            _image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            isArmed = false;
            ticksToExplode = 10;
            owner = bombOwner;
        }

        public int Strength
        {
            get
            {
                return owner.BombStr;
            }
        }


        public bool Tick(Board board)
        {
            bool exploded = false;
            if (!isArmed)
            {
                if (!board.IsPlayerInTile(owner, this))
                    ArmBomb();
            }
            else if (ticksToExplode == 0)
            {
                exploded = true;
                owner.GiveBomb();
                _image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                ticksToExplode--;
            }
            return exploded;

        }


        public void ArmBomb()
        {
            isArmed = true;
            _image.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        public override bool IsPassable()
        {
            return !isArmed || ticksToExplode == 0;
        }

    }
}
