using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
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


        public Game(Canvas canvas, (ControlScheme controlScheme, string iconPath)[] HumanPlayerInfo)
        {
            _canvas = canvas;
            _board = new Board(_canvas);
            Point[] SpawnLocations = _board.SpawnLocation;
            _players = new HumanPlayer[HumanPlayerInfo.Length];
            for (int i = 0; i < _players.Length; i++)
            {
                _players[i] = new HumanPlayer(SpawnLocations[i], HumanPlayerInfo[i].controlScheme, HumanPlayerInfo[i].iconPath, _canvas);
                _players[i].PlayerMovement += _board.OnPlayerMovement;
            }
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            _gameTimer.Tick += _gameTimer_Tick;
        }

        public Board Board
        {
            get
            {
                return _board;
            }
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

        public void Bomb_BombExplosion(object sender, EventArgs e)
        {
            Bomb bomb = sender as Bomb;
            List<Tile> explodedTiles = _board.ExplodeBomb(bomb);
            new Explosion(explodedTiles, _canvas);
            CheckForPlayersHitByBomb(explodedTiles);
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
            foreach (HumanPlayer humanPlayer in _players)
            {
                if (humanPlayer.Alive && humanPlayer.ControlScheme.IsKeyInScheme(key))
                    humanPlayer.MoveBasedOnKey(key, this);
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
