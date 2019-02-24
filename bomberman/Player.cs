using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace bomberman
{
    class Player
    {
        protected Point _centerPoint;
        protected Image _image;
        private readonly int _maxBombCount = 5;
        private int _currentMaxBombCount = 1;
        private int _currentBombCount;
        private int _bombStr = 1;
        protected bool _upgradeSpeedFlag = false;
        private bool _alive = true;

        private readonly double _maxMoveSpeed = 50;
        private readonly int _maxBombStr = 5;
        private double _moveSpeed = 12.5;


        protected Player(Point spawnPoint, Canvas canvas, string imageSource)
        {
            _centerPoint = spawnPoint;
            _image = new Image
            {
                Height = 50,
                Width = 50,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Canvas.SetTop(_image, _centerPoint.Y - 25);
            Canvas.SetLeft(_image, _centerPoint.X - 25);
            canvas.Children.Add(_image);
            _image.Source = new BitmapImage(new Uri($"{imageSource}"));
            _currentBombCount = _currentMaxBombCount;
        }

        public double MoveSpeed
        {
            get
            {
                return _moveSpeed;
            }
        }

        public int BombStr
        {
            get
            {
                return _bombStr;
            }
        }

        public bool Alive
        {
            get
            {
                return _alive;
            }
        }

        public Point GetCenter()
        {
            return _centerPoint;
        }

        protected void Move(KeyAction keyAction, Board board)
        {
            switch (keyAction)
            {
                case KeyAction.Up:
                    MoveUp(board);
                    break;
                case KeyAction.Down:
                    MoveDown(board);
                    break;
                case KeyAction.Left:
                    MoveLeft(board);
                    break;
                case KeyAction.Right:
                    MoveRight(board);
                    break;
            }
        }

        private void MoveLeft(Board board)
        {
            Point newCenter = new Point(_centerPoint.X - MoveSpeed, _centerPoint.Y);
            if (board.IsMovePossible(newCenter, this))
            {
                _centerPoint = newCenter;
                Canvas.SetLeft(_image, Canvas.GetLeft(_image) - MoveSpeed);
                UpgradeMoveSpeedIfNeeded();
                OnPlayerMovement();
                //board.OnPlayerMovement(this);
            }
        }
        private void MoveUp(Board board)
        {
            Point newCenter = new Point(_centerPoint.X, _centerPoint.Y - MoveSpeed);
            if (board.IsMovePossible(newCenter, this))
            {
                _centerPoint = newCenter;
                Canvas.SetTop(_image, Canvas.GetTop(_image) - MoveSpeed);
                UpgradeMoveSpeedIfNeeded();
                OnPlayerMovement();
                //board.OnPlayerMovement(this);
            }
        }
        private void MoveRight(Board board)
        {
            Point newCenter = new Point(_centerPoint.X + MoveSpeed, _centerPoint.Y);
            if (board.IsMovePossible(newCenter, this))
            {
                _centerPoint = newCenter;
                Canvas.SetLeft(_image, Canvas.GetLeft(_image) + MoveSpeed);
                UpgradeMoveSpeedIfNeeded();
                OnPlayerMovement();
                //board.OnPlayerMovement(this);
            }
        }
        private void MoveDown(Board board)
        {
            Point newCenter = new Point(_centerPoint.X, _centerPoint.Y + MoveSpeed);
            if (board.IsMovePossible(newCenter, this))
            {
                _centerPoint = newCenter;
                Canvas.SetTop(_image, Canvas.GetTop(_image) + MoveSpeed);
                UpgradeMoveSpeedIfNeeded();
                OnPlayerMovement();
                //board.OnPlayerMovement(this);
            }
        }

        public bool DropBomb(Board board, out Bomb bomb)
        {
            bomb = null;
            if (_currentBombCount > 0 && board.PlaceBomb(this, BombStr, out bomb))
            {
                _currentBombCount--;
                return true;
            }
            return false;
        }

        public void GiveBomb()
        {
            _currentBombCount++;
        }

        public void UpgradeBombCount()
        {
            if (_currentMaxBombCount < _maxBombCount)
            {
                _currentBombCount += 1;
                _currentMaxBombCount += 1;
            }
        }

        public void UpgradeBombStrength()
        {
            if (_bombStr < _maxBombStr)
                _bombStr += 1;
        }

        public void UpgradeMoveSpeed()
        {
            if (_moveSpeed < _maxMoveSpeed)
            {
                _upgradeSpeedFlag = true;
            }

        }

        protected void UpgradeMoveSpeedIfNeeded()
        {
            if (_upgradeSpeedFlag)
            {
                _moveSpeed *= 2;
                _upgradeSpeedFlag = false;
            }
        }

        public void Hit()
        {
            _image.Visibility = Visibility.Collapsed;
            _alive = false;
        }

        protected virtual void OnPlayerMovement()
        {
            if (PlayerMovement != null)
                PlayerMovement(this, EventArgs.Empty);
        }

        public event EventHandler PlayerMovement;
    }
}
