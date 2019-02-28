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
        protected int _currentBombCount;
        protected int _bombStr = 1;
        protected bool _upgradeSpeedFlag = false;
        private bool _alive = true;
        private readonly double _maxStepSize = 25;
        private double _stepSize = 12.5;
        protected Game _game;
        protected Direction _currentMoveDirection = Direction.Null;

        protected Player(Point spawnPoint, Canvas canvas, string imageSource, Game game)
        {
            _centerPoint = spawnPoint;
            _image = new Image
            {
                Height = Game.PlayerSize,
                Width = Game.PlayerSize,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Canvas.SetTop(_image, _centerPoint.Y - Game.PlayerSize / 2);
            Canvas.SetLeft(_image, _centerPoint.X - Game.PlayerSize / 2);
            canvas.Children.Add(_image);
            _image.Source = new BitmapImage(new Uri($"{imageSource}"));
            _currentBombCount = _currentMaxBombCount;
            _game = game;
        }

        public double StepSize
        {
            get
            {
                return _stepSize;
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
        public Point Center
        {
            get
            {
                return _centerPoint;
            }
        }

        public virtual void DoAction()
        {
            MoveBasedOnDirection();
        }
        private void MoveBasedOnDirection()
        {
            
            double step = StepSize;
            if (_currentMoveDirection == Direction.Null)
                return;
            else if (_currentMoveDirection == Direction.Up || _currentMoveDirection == Direction.Left)
                step = -step;
            Point newCenter = GetNewCenterPointBasedOnMoveDirection(_currentMoveDirection, _centerPoint);
            if (_game.Board.IsMovePossible(newCenter, this))
            {
                _centerPoint = newCenter;
                if (_currentMoveDirection == Direction.Left || _currentMoveDirection == Direction.Right)
                    Canvas.SetLeft(_image, Canvas.GetLeft(_image) + step);
                else
                    Canvas.SetTop(_image, Canvas.GetTop(_image) + step);
                UpgradeMoveSpeedIfNeeded();
                OnPlayerMovement();
            }
        }

        protected Point GetNewCenterPointBasedOnMoveDirection(Direction direction, Point centerPoint)
        {
            Point newCenter = new Point();
            switch (direction)
            {                
                case Direction.Up:
                    newCenter = new Point(centerPoint.X, centerPoint.Y - StepSize);
                    break;
                case Direction.Down:
                    newCenter = new Point(centerPoint.X, centerPoint.Y + StepSize);
                    break;
                case Direction.Left:
                    newCenter = new Point(centerPoint.X - StepSize, centerPoint.Y);
                    break;
                case Direction.Right:
                    newCenter = new Point(centerPoint.X + StepSize, centerPoint.Y);
                    break;
            }
            return newCenter;
        }
        public bool DropBomb()
        {
            if (_currentBombCount > 0 && _game.Board.PlaceBomb(this, BombStr, _game))
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
            if (_bombStr < Bomb.MaxStrength)
                _bombStr += 1;
        }
        public void UpgradeMoveSpeed()
        {
            if (_stepSize < _maxStepSize)
            {
                _upgradeSpeedFlag = true;
            }

        }
        protected void UpgradeMoveSpeedIfNeeded()
        {
            if (_upgradeSpeedFlag)
            {
                _stepSize *= 2;
                _upgradeSpeedFlag = false;
            }
        }
        public void Hit()
        {
            _image.Visibility = Visibility.Collapsed;
            _alive = false;
        }

        protected void OnPlayerMovement()
        {
            if (PlayerMovement != null)
                PlayerMovement(this, EventArgs.Empty);
        }
        public event EventHandler PlayerMovement;
    }
}
