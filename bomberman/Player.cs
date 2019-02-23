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
    class Player
    {
        protected XYCoordinates _centerCoordinate;
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


        protected Player(XYCoordinates spawnCoordinates, Canvas canvas, string imageSource)
        {
            _centerCoordinate = spawnCoordinates;
            _image = new Image
            {
                Height = 50,
                Width = 50,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Canvas.SetTop(_image, _centerCoordinate.Y - 25);
            Canvas.SetLeft(_image, _centerCoordinate.X - 25);
            canvas.Children.Add(_image);
            _image.Source = new BitmapImage(new Uri($"ms-appx:///{imageSource}"));
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

        public XYCoordinates GetCenter()
        {
            return _centerCoordinate;
        }

        public void MoveLeft(Board board)
        {
            XYCoordinates newCenter = new XYCoordinates(_centerCoordinate.X - MoveSpeed, _centerCoordinate.Y);
            if (board.IsMovePossible(newCenter, this))
            {
                _centerCoordinate = newCenter;
                Canvas.SetLeft(_image, Canvas.GetLeft(_image) - MoveSpeed);
                UpgradeMoveSpeedIfNeeded();
                OnPlayerMovement();
                //board.OnPlayerMovement(this);
            }
        }
        public void MoveUp(Board board)
        {
            XYCoordinates newCenter = new XYCoordinates(_centerCoordinate.X, _centerCoordinate.Y - MoveSpeed);
            if (board.IsMovePossible(newCenter, this))
            {
                _centerCoordinate = newCenter;
                Canvas.SetTop(_image, Canvas.GetTop(_image) - MoveSpeed);
                UpgradeMoveSpeedIfNeeded();
                OnPlayerMovement();
                //board.OnPlayerMovement(this);
            }
        }
        public void MoveRight(Board board)
        {
            XYCoordinates newCenter = new XYCoordinates(_centerCoordinate.X + MoveSpeed, _centerCoordinate.Y);
            if (board.IsMovePossible(newCenter, this))
            {
                _centerCoordinate = newCenter;
                Canvas.SetLeft(_image, Canvas.GetLeft(_image) + MoveSpeed);
                UpgradeMoveSpeedIfNeeded();
                OnPlayerMovement();
                //board.OnPlayerMovement(this);
            }
        }
        public void MoveDown(Board board)
        {
            XYCoordinates newCenter = new XYCoordinates(_centerCoordinate.X, _centerCoordinate.Y + MoveSpeed);
            if (board.IsMovePossible(newCenter, this))
            {
                _centerCoordinate = newCenter;
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
