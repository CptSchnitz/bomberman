using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace bomberman
{
    class Bomb : Tile
    {
        private int _ticksToExplode = 12;
        private Player _owner;
        private DispatcherTimer _bombTimer;
        private static readonly string _explodingBombSource = "Assets/ExplodingBomb.png";
        private static readonly string _largeBombSource = "Assets/LargeBomb.png";
        private static readonly string _smallBombSource = "Assets/SmallBomb.png";
        private bool _ownerLeftBombTile;
        private Board _board;

        public Bomb(Image image, Player bombOwner, Board board) : base(image, _largeBombSource)
        {
            _ownerLeftBombTile = false;
            _owner = bombOwner;
            _board = board;
            _owner.PlayerMovement += _owner_PlayerMovement;
            _bombTimer = new DispatcherTimer();
            _bombTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            _bombTimer.Tick += _bombTimer_Tick;
            _bombTimer.Start();
        }

        public int Strength
        {
            get
            {
                return _owner.BombStr;
            }
        }

        private void _owner_PlayerMovement(object sender, EventArgs e)
        {
            if (!_board.IsPlayerInTile(_owner, this))
            {
                _ownerLeftBombTile = true;
                _owner.PlayerMovement -= _owner_PlayerMovement;
            }
        }


        public void _bombTimer_Tick(object sender, object e)
        {
            switch (_ticksToExplode)
            {
                case 0:
                    OnBombExplosion();
                    return;
                case 1:
                    _image.Source = new BitmapImage(new Uri($"ms-appx:///{_explodingBombSource}"));
                    break;
                case int num when _ticksToExplode % 2 == 0:
                    _image.Source = new BitmapImage(new Uri($"ms-appx:///{_largeBombSource}"));
                    break;

                case int num when _ticksToExplode % 2 == 1:
                    _image.Source = new BitmapImage(new Uri($"ms-appx:///{_smallBombSource}"));
                    break;
            }
            _ticksToExplode--;
        }

        public void Explode()
        {
            _bombTimer.Stop();
            _owner.GiveBomb();
        }

        public override bool IsPassable(Player player)
        {
            return ReferenceEquals(_owner, player) && !_ownerLeftBombTile;
        }

        protected virtual void OnBombExplosion()
        {
            if (BombExplosion != null)
                BombExplosion(this, EventArgs.Empty);
        }

        public event EventHandler BombExplosion;
    }
}
