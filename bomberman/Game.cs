using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace bomberman
{
    class Game
    {
        Canvas _canvas;
        private Board _board;
        Player[] _players;
        DispatcherTimer _gameTimer;
        private static int _tickInterval = 175;
        private bool _paused;


        public Game(Canvas canvas, (ControlScheme controlScheme, string iconPath)[] HumanPlayerInfo)
        {
            _canvas = canvas;
            _board = new Board(_canvas);
            Point[] SpawnLocations = _board.SpawnLocation;
            _players = new Player[SpawnLocations.Length];
            _paused = false;
            for (int i = 0; i < _players.Length; i++)
            {
                if (i < HumanPlayerInfo.Length)
                {
                    _players[i] = new HumanPlayer(SpawnLocations[i], HumanPlayerInfo[i].controlScheme, HumanPlayerInfo[i].iconPath, _canvas, this);
                }
                else
                {
                    _players[i] = new AiPlayer(SpawnLocations[i], canvas, "ms-appx:///Assets/PlayerIcons/thump.png", this);
                }
                _players[i].PlayerMovement += _board.OnPlayerMovement;
            }
            _gameTimer = new DispatcherTimer();
            _gameTimer.Interval = new TimeSpan(0, 0, 0, 0, TickInterval);
            _gameTimer.Tick += _gameTimer_Tick;
        }

        public Board Board
        {
            get
            {
                return _board;
            }
        }

        public Player[] Players
        {
            get
            {
                return _players;
            }
        }

        public static int TickInterval
        {
            get
            {
                return _tickInterval;
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
            else
            {
                foreach (Player player in Players)
                {
                    if (player.Alive)
                        player.DoAction();
                }
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

        public void OnBombExplosion(object sender, EventArgs e)
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
                foreach (Player player in Players)
                {
                    if (_board.IsPlayerInTile(player, tile))
                        player.Hit();
                }
            }
        }

        public void OnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.KeyStatus.RepeatCount == 1)
            {
                Windows.System.VirtualKey key = args.VirtualKey;
                if (!_paused)
                {
                    foreach (Player player in _players)
                    {
                        if (player is HumanPlayer humanPlayer && humanPlayer.Alive && humanPlayer.ControlScheme.IsKeyInScheme(key))
                            humanPlayer.OnKeyDown(key);
                    }

                }
            }
        }

        public void OnKeyUp(CoreWindow sender, KeyEventArgs args)
        {
            VirtualKey key = args.VirtualKey;
            if (_paused && key == VirtualKey.P)
            {
                _gameTimer.Start();
                OnUnPaused();
                _paused = false;
                return;
            }
            else
            {
                if (key == VirtualKey.P)
                {
                    _gameTimer.Stop();
                    OnPaused();
                    _paused = true;
                    return;
                }
                foreach (Player player in _players)
                {
                    if (player is HumanPlayer humanPlayer && humanPlayer.Alive && humanPlayer.ControlScheme.IsKeyInScheme(key))
                        humanPlayer.OnKeyUp(key);
                }
            }

        }
        protected virtual void OnGameOver(GameOverEventArgs e)
        {
            if (GameOver != null)
                GameOver(this, e);
        }
        protected virtual void OnPaused()
        {
            if (Paused != null)
                Paused(this, EventArgs.Empty);
        }
        protected virtual void OnUnPaused()
        {
            if (UnPaused != null)
                UnPaused(this, EventArgs.Empty);
        }

        public event EventHandler<GameOverEventArgs> GameOver;
        public event EventHandler Paused;
        public event EventHandler UnPaused;
    }

    class GameOverEventArgs : EventArgs
    {
        public int Winner { get; set; }
    }
}
