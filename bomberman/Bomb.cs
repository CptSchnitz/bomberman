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
        private bool exploded;

        public Bomb(Image image, Player bombOwner) : base(image)
        {
            _image.Source = new BitmapImage(new Uri("ms-appx:///Assets/bomb.png"));
            _image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            isArmed = false;
            ticksToExplode = 20;
            owner = bombOwner;
            exploded = false;
        }

        public int Strength
        {
            get
            {
                return owner.BombStr;
            }
        }

        public bool IsArmed { get { return isArmed; } }

        public bool Tick(Board board)
        {
            if (!IsArmed)
            {
                if (!board.IsPlayerInTile(owner, this))
                    ArmBomb();
            }
            else if (ticksToExplode == 0)
            {
                exploded = true;                
                _image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                ticksToExplode--;
            }
            return exploded;

        }


        public bool Exploded
        {
            get { return exploded; }
        }


        public void ArmBomb()
        {
            isArmed = true;
            _image.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        public void Explode()
        {
            ticksToExplode = 0;
            owner.GiveBomb();
            exploded = true;
        }

        public override bool IsPassable()
        {
            return !IsArmed || ticksToExplode == 0;
        }

    }
}
