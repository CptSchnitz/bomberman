using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class AiPlayer : Player
    {
        private Random _random;
        private Direction _directionToLastTile;
        private static Direction[] _directions = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        public AiPlayer(Point spawnPoint, Canvas canvas, string imageSource, Game game) : base(spawnPoint, canvas, imageSource, game)
        {
            _random = new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds() / imageSource.GetHashCode());
            _directionToLastTile = Direction.Null;
        }

        public override void DoAction()
        {
            if (!_game.Board.IsLocationSafe(_centerPoint))
            {
                MoveToSafety();
            }
            else
            {
                bool isInMiddleOfTile = (_centerPoint.X - 25) % 50 == 0 && (_centerPoint.Y - 25) % 50 == 0;
                if (isInMiddleOfTile && (_game.Board.NumOfCrates == 0 || _game.Board.IsNextToCrate(this)) && SafePlaceToBomb())
                {
                    if (DropBomb())
                        MoveToSafety();
                    else
                    {
                        if (_currentBombCount >= 1)
                            MoveRandomly();
                    }
                }
                else
                {
                    MoveRandomly();
                }
            }
            base.DoAction();
            if (_currentMoveDirection != Direction.Null)
                _directionToLastTile = InvertDirection(_currentMoveDirection);
            _currentMoveDirection = Direction.Null;
        }

        private static Direction InvertDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    return Direction.Null;
            }
        }

        private void MoveRandomly()
        {
            List<Direction> directions = PossibleMoveOptions();
            directions.Remove(_directionToLastTile);
            if (directions.Count == 0)
                _currentMoveDirection = Direction.Null;
            else
            {
                _currentMoveDirection = directions[_random.Next() % directions.Count];
            }
        }
        private List<Direction> PossibleMoveOptions()
        {
            List<Direction> directions = new List<Direction>();
            Point upCenter = new Point(_centerPoint.X, _centerPoint.Y - StepSize);
            Point downCenter = new Point(_centerPoint.X, _centerPoint.Y + StepSize);
            Point leftCenter = new Point(_centerPoint.X - StepSize, _centerPoint.Y);
            Point rightCenter = new Point(_centerPoint.X + StepSize, _centerPoint.Y);
            if (_game.Board.IsMovePossible(upCenter, this) && _game.Board.IsLocationSafe(upCenter))
                directions.Add(Direction.Up);
            if (_game.Board.IsMovePossible(downCenter, this) && _game.Board.IsLocationSafe(downCenter))
                directions.Add(Direction.Down);
            if (_game.Board.IsMovePossible(leftCenter, this) && _game.Board.IsLocationSafe(leftCenter))
                directions.Add(Direction.Left);
            if (_game.Board.IsMovePossible(rightCenter, this) && _game.Board.IsLocationSafe(rightCenter))
                directions.Add(Direction.Right);
            return directions;

        }

        private void MoveToSafety()
        {
            List<Direction> stepsToSafety = new List<Direction>();
            int minSteps = int.MaxValue;
            foreach (Direction direction in _directions)
            {
                int steps = FindSafePlace(_centerPoint, direction, 1, true, false);
                if (steps != int.MaxValue && steps <= minSteps)
                {
                    if (steps < minSteps)
                    {
                        minSteps = steps;
                        stepsToSafety.Clear();
                    }
                    stepsToSafety.Add(direction);
                }
            }

            switch (stepsToSafety.Count)
            {
                case 0:
                    _currentMoveDirection = Direction.Null;
                    break;
                case 1:
                    _currentMoveDirection = stepsToSafety[0];
                    break;
                default:
                    _currentMoveDirection = stepsToSafety[_random.Next() % stepsToSafety.Count];
                    break;
            }
        }
        private int FindSafePlace(Point centerPoint, Direction direction, int numOfSteps, bool checkForDiversions, bool checkForOptionalBomb, Point optionalBomb = new Point())
        {
            while (numOfSteps < (Bomb.TicksToExplode * Bomb.TickInterval) / Game.TickInterval)
            {
                centerPoint = GetNewCenterPointBasedOnMoveDirection(direction, centerPoint);
                if (!_game.Board.IsMovePossible(centerPoint, this))
                    return int.MaxValue;
                if ((!checkForOptionalBomb || !IsInBombHitRange(optionalBomb, centerPoint)) && _game.Board.IsLocationSafe(centerPoint))
                    return numOfSteps;
                numOfSteps++;
                if (checkForDiversions)
                {
                    Direction[] perpendicularDirections = GetPerpendicularDirections(direction);
                    int minStepsToSafety = Math.Min(FindSafePlace(centerPoint, perpendicularDirections[0], numOfSteps, false, checkForOptionalBomb, optionalBomb),
                        FindSafePlace(centerPoint, perpendicularDirections[1], numOfSteps, false, checkForOptionalBomb, optionalBomb));
                    if (minStepsToSafety != int.MaxValue)
                        return minStepsToSafety;
                }
            }
            return int.MaxValue;
        }

        private bool SafePlaceToBomb()
        {
            Point bombLocation = new Point((int)_centerPoint.X / 50 * 50 + 25, (int)_centerPoint.Y / 50 * 50 + 25);
            foreach (Direction direction in _directions)
            {
                if (FindSafePlace(_centerPoint, direction, 1, true, true, bombLocation) != int.MaxValue)
                    return true;
            }
            return false;
        }
        private static Direction[] GetPerpendicularDirections(Direction direction)
        {
            Direction[] directions = new Direction[1];
            switch (direction)
            {
                case Direction.Up:
                case Direction.Down:
                    directions = new Direction[] { Direction.Left, Direction.Right };
                    break;
                case Direction.Left:
                case Direction.Right:
                    directions = new Direction[] { Direction.Up, Direction.Down };
                    break;
            }
            return directions;
        }
        private bool IsInBombHitRange(Point bombLocation, Point playerLocation)
        {
            double bombRight = bombLocation.X + (Game.TileSize / 2);
            double bombLeft = bombLocation.X - (Game.TileSize / 2);
            double bombTop = bombLocation.Y - (Game.TileSize / 2);
            double bombBottom = bombLocation.Y + (Game.TileSize / 2);
            double playerRight = playerLocation.X + (Game.PlayerSize / 2 - 1);
            double playerLeft = playerLocation.X - (Game.PlayerSize / 2 - 1);
            double playerTop = playerLocation.Y - (Game.PlayerSize / 2 - 1);
            double playerBottom = playerLocation.Y + (Game.PlayerSize / 2 - 1);
            if ((playerRight > bombLeft && playerRight < bombRight) || (playerLeft > bombLeft && playerLeft < bombRight))
                if (Math.Abs(bombLocation.Y - playerLocation.Y) <= Game.TileSize * (_bombStr + 1))
                    return true;
            if ((playerBottom < bombTop && playerBottom > bombBottom) || (playerTop > bombTop && playerTop < bombBottom))
                if (Math.Abs(bombLocation.X - playerLocation.X) <= Game.TileSize * (_bombStr + 1))
                    return true;
            return false;
        }
    }
}
