using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class Game
    {
        Canvas _canvas;
        private Board _board;
        HumanPlayer[] _players;
        DispatcherTimer _gameTimer;


        public Game(Canvas canvas, int NumOfPlayers)
        {
            _canvas = canvas;
            _board = new Board(_canvas);
            XYCoordinates[] SpawnLocations = _board.SpawnLocation;
            _players = new HumanPlayer[2];
            for (int i = 0; i < _players.Length; i++)
            {
                _players[i] = new HumanPlayer(SpawnLocations[i], _canvas);
                _players[i].PlayerMovement += _board.OnPlayerMovement;
            }
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            _gameTimer.Tick += _gameTimer_Tick;
        }

        private void _gameTimer_Tick(object sender, object e)
        {
            if (IsGameOver(out int winner))
            {
                _gameTimer.Stop();
                GameOverEventArgs args = new GameOverEventArgs();
                args.Winner = winner;
                OnGameOver(args);
            }
        }

        private bool IsGameOver(out int winner)
        {
            int playersAlive = _players.Length;
            winner = -1;
            for (int i = 0; i < _players.Length; i++)
            {
                Player player = _players[i];
                if (!player.Alive)
                    playersAlive--;
                else
                    winner = i;
            }

            switch (playersAlive)
            {
                case 0:
                case 1:
                    return true;
                default:
                    winner = -1;
                    return false;
            }

        }

        public void StartGame()
        {
            _gameTimer.Start();
        }

        private void Bomb_BombExplosion(object sender, EventArgs e)
        {
            Bomb bomb = sender as Bomb;
            List<Tile> explodedTiles = _board.ExplodeBomb(bomb);
            new Explosion(explodedTiles, _canvas);
            CheckForPlayersHitByBomb(explodedTiles); ;
        }

        private void CheckForPlayersHitByBomb(List<Tile> explodedTiles)
        {
            foreach (Tile tile in explodedTiles)
            {
                foreach (Player player in _players)
                {
                    if (_board.IsPlayerInTile(player, tile))
                        player.Hit();
                }
            }
        }


        public void KeyUp(CoreWindow sender, KeyEventArgs args)
        {

            Windows.System.VirtualKey key = args.VirtualKey;
            switch (key)
            {
                case Windows.System.VirtualKey.W when _players[0].Alive: // up
                    _players[0].MoveUp(_board);
                    break;
                case Windows.System.VirtualKey.A when _players[0].Alive: // left
                    _players[0].MoveLeft(_board);
                    break;
                case Windows.System.VirtualKey.S when _players[0].Alive: // down
                    _players[0].MoveDown(_board);
                    break;
                case Windows.System.VirtualKey.D when _players[0].Alive: // right
                    _players[0].MoveRight(_board);
                    break;
                case Windows.System.VirtualKey.Space when _players[0].Alive:
                    if (_players[0].DropBomb(_board, out Bomb bomb))
                        bomb.BombExplosion += Bomb_BombExplosion;
                    break;
                case Windows.System.VirtualKey.GamepadDPadUp when _players[1].Alive: // up
                    _players[1].MoveUp(_board);
                    break;
                case Windows.System.VirtualKey.GamepadDPadLeft when _players[1].Alive: // left
                    _players[1].MoveLeft(_board);
                    break;
                case Windows.System.VirtualKey.GamepadDPadDown when _players[1].Alive: // down
                    _players[1].MoveDown(_board);
                    break;
                case Windows.System.VirtualKey.GamepadDPadRight when _players[1].Alive: // right
                    _players[1].MoveRight(_board);
                    break;
                case Windows.System.VirtualKey.GamepadA when _players[1].Alive:
                    if (_players[1].DropBomb(_board, out Bomb bomb2))
                        bomb2.BombExplosion += Bomb_BombExplosion;
                    break;
            }
        }
        protected virtual void OnGameOver(GameOverEventArgs e)
        {
            if (GameOver != null)
                GameOver(this, e);
        }

        public event EventHandler<GameOverEventArgs> GameOver;
    }

    class GameOverEventArgs : EventArgs
    {
        public int Winner { get; set; }

    }
}
