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
        private static int _ticksToExplode = 12;
        private int _currentTick = TicksToExplode;
        private static int _tickInterval = 250;
        private Player _owner;
        private DispatcherTimer _bombTimer;
        private static readonly string _explodingBombSource = "Assets/ExplodingBomb.png";
        private static readonly string _largeBombSource = "Assets/LargeBomb.png";
        private static readonly string _smallBombSource = "Assets/SmallBomb.png";
        private bool _ownerLeftBombTile;
        private Game _game;
        private static int _maxStr = 5;

        public Bomb(Image image, Player bombOwner, Game game) : base(image, _largeBombSource)
        {
            _ownerLeftBombTile = false;
            _owner = bombOwner;
            _game = game;
            _owner.PlayerMovement += _owner_PlayerMovement;
            _game.GameOver += StopBombWhenGameOver;
            _game.Paused += _game_Paused;
            _game.UnPaused += _game_UnPaused;
            _bombTimer = new DispatcherTimer();
            _bombTimer.Interval = new TimeSpan(0, 0, 0, 0, TickInterval);
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
        public static int MaxStrength
        {
            get
            {
                return _maxStr;
            }
        }
        public static int TicksToExplode
        {
            get
            {
                return _ticksToExplode;
            }
        }
        public static int TickInterval
        {
            get
            {
                return _tickInterval;
            }
        }
        public override bool IsPassable(Player player)
        {
            return ReferenceEquals(_owner, player) && !_ownerLeftBombTile;
        }

        private void _owner_PlayerMovement(object sender, EventArgs e)
        {
            if (!_game.Board.IsPlayerInTile(_owner, this))
            {
                _ownerLeftBombTile = true;
                _owner.PlayerMovement -= _owner_PlayerMovement;
            }
        }
        private void StopBombWhenGameOver(object sender, GameOverEventArgs e)
        {
            _bombTimer.Stop();
        }

        private void _game_UnPaused(object sender, EventArgs e)
        {
            _bombTimer.Start();
        }

        private void _game_Paused(object sender, EventArgs e)
        {
            _bombTimer.Stop();
        }

        public void _bombTimer_Tick(object sender, object e)
        {
            switch (_currentTick)
            {
                case 0:
                    OnBombExplosion();
                    return;
                case 1:
                    _image.Source = new BitmapImage(new Uri($"ms-appx:///{_explodingBombSource}"));
                    break;
                case int num when _currentTick % 2 == 0:
                    _image.Source = new BitmapImage(new Uri($"ms-appx:///{_largeBombSource}"));
                    break;
                case int num when _currentTick % 2 == 1:
                    _image.Source = new BitmapImage(new Uri($"ms-appx:///{_smallBombSource}"));
                    break;
            }
            _currentTick--;
        }

        public void Explode()
        {
            _bombTimer.Stop();
            _owner.GiveBomb();
            _game.GameOver -= StopBombWhenGameOver;
            _game.Paused -= _game_Paused;
            _game.UnPaused -= _game_UnPaused;
        }


        protected virtual void OnBombExplosion()
        {
            if (BombExplosion != null)
                BombExplosion(this, EventArgs.Empty);
        }

        public event EventHandler BombExplosion;
    }
}
