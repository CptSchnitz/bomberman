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
        public AiPlayer(Point spawnPoint, Canvas canvas, string imageSource, Game game) : base(spawnPoint, canvas, imageSource, game)
        {
            _random = new Random((int)DateTimeOffset.Now.ToUnixTimeMilliseconds() / imageSource.GetHashCode());
            _directionToLastTile = Direction.Null;
        }

        /* 
         * if current position safe
         *      if good place to place bomb
         *          place bomb
         *      else 
         *          move to a safe place (dont go to previous direction)
         * else current position not safe
         *      if there is a path to safety
         *          move that direction
         *      else
         *          do nothing 
         */
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
                }
                else
                {
                    Move();
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

        private void Move()
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

            int steps = FindSafePlaceUp(_centerPoint, 1, true, false);
            if (steps != int.MaxValue)
            {
                stepsToSafety.Add((Direction.Up));
                minSteps = steps;
            }
            steps = FindSafePlaceDown(_centerPoint, 1, true, false);
            if (steps != int.MaxValue && steps <= minSteps)
            {
                if (steps < minSteps)
                {
                    minSteps = steps;
                    stepsToSafety.Clear();
                }
                stepsToSafety.Add((Direction.Down));
            }
            steps = FindSafePlaceLeft(_centerPoint, 1, true, false);
            if (steps != int.MaxValue && steps <= minSteps)
            {
                if (steps < minSteps)
                {
                    minSteps = steps;
                    stepsToSafety.Clear();
                }
                stepsToSafety.Add((Direction.Left));
            }
            steps = FindSafePlaceRight(_centerPoint, 1, true, false);
            if (steps != int.MaxValue && steps <= minSteps)
            {
                if (steps < minSteps)
                {
                    minSteps = steps;
                    stepsToSafety.Clear();
                }
                stepsToSafety.Add((Direction.Right));
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
        private int FindSafePlaceUp(Point centerPoint, int numOfSteps, bool checkForDiversions, bool checkForOptionalBomb, Point optionalBomb = new Point())
        {
            while (numOfSteps < (Bomb.TicksToExplode * Bomb.TickInterval) / Game.TickInterval)
            {
                centerPoint = new Point(centerPoint.X, centerPoint.Y - StepSize);
                if (!_game.Board.IsMovePossible(centerPoint, this))
                    return int.MaxValue;
                if ((!checkForOptionalBomb || !IsInBombHitRange(optionalBomb, centerPoint)) && _game.Board.IsLocationSafe(centerPoint))
                    return numOfSteps;
                numOfSteps++;
                if (checkForDiversions)
                {
                    int minStepsToSafety = Math.Min(FindSafePlaceLeft(centerPoint, numOfSteps, false, checkForOptionalBomb, optionalBomb), FindSafePlaceRight(centerPoint, numOfSteps, false, checkForOptionalBomb, optionalBomb));
                    if (minStepsToSafety != int.MaxValue)
                        return minStepsToSafety;
                }
            }
            return int.MaxValue;
        }
        private int FindSafePlaceDown(Point centerPoint, int numOfSteps, bool checkForDiversions, bool checkForOptionalBomb, Point optionalBomb = new Point())
        {
            while (numOfSteps < (Bomb.TicksToExplode * Bomb.TickInterval) / Game.TickInterval)
            {
                centerPoint = new Point(centerPoint.X, centerPoint.Y + StepSize);
                if (!_game.Board.IsMovePossible(centerPoint, this))
                    return int.MaxValue;
                if ((!checkForOptionalBomb || !IsInBombHitRange(optionalBomb, centerPoint)) && _game.Board.IsLocationSafe(centerPoint))
                    return numOfSteps;
                numOfSteps++;
                if (checkForDiversions)
                {
                    int minStepsToSafety = Math.Min(FindSafePlaceLeft(centerPoint, numOfSteps, false, checkForOptionalBomb, optionalBomb), FindSafePlaceRight(centerPoint, numOfSteps, false, checkForOptionalBomb, optionalBomb));
                    if (minStepsToSafety != int.MaxValue)
                        return minStepsToSafety;
                }
            }
            return int.MaxValue;
        }
        private int FindSafePlaceLeft(Point centerPoint, int numOfSteps, bool checkForDiversions, bool checkForOptionalBomb, Point optionalBomb = new Point())
        {
            while (numOfSteps < (Bomb.TicksToExplode * Bomb.TickInterval) / Game.TickInterval)
            {
                centerPoint = new Point(centerPoint.X - StepSize, centerPoint.Y);
                if (!_game.Board.IsMovePossible(centerPoint, this))
                    return int.MaxValue;
                if ((!checkForOptionalBomb || !IsInBombHitRange(optionalBomb, centerPoint)) && _game.Board.IsLocationSafe(centerPoint))
                    return numOfSteps;
                numOfSteps++;
                if (checkForDiversions)
                {
                    int minStepsToSafety = Math.Min(FindSafePlaceUp(centerPoint, numOfSteps, false, checkForOptionalBomb, optionalBomb), FindSafePlaceDown(centerPoint, numOfSteps, false, checkForOptionalBomb, optionalBomb));
                    if (minStepsToSafety != int.MaxValue)
                        return minStepsToSafety;
                }
            }
            return int.MaxValue;
        }
        private int FindSafePlaceRight(Point centerPoint, int numOfSteps, bool checkForDiversions, bool checkForOptionalBomb, Point optionalBomb = new Point())
        {
            while (numOfSteps < (Bomb.TicksToExplode * Bomb.TickInterval) / Game.TickInterval)
            {
                centerPoint = new Point(centerPoint.X + StepSize, centerPoint.Y);
                if (!_game.Board.IsMovePossible(centerPoint, this))
                    return int.MaxValue;
                if ((!checkForOptionalBomb || !IsInBombHitRange(optionalBomb, centerPoint)) && _game.Board.IsLocationSafe(centerPoint))
                    return numOfSteps;
                numOfSteps++;
                if (checkForDiversions)
                {
                    int minStepsToSafety = Math.Min(FindSafePlaceUp(centerPoint, numOfSteps, false, checkForOptionalBomb, optionalBomb), FindSafePlaceDown(centerPoint, numOfSteps, false, checkForOptionalBomb, optionalBomb));
                    if (minStepsToSafety != int.MaxValue)
                        return minStepsToSafety;
                }
            }
            return int.MaxValue;
        }

        private bool SafePlaceToBomb()
        {
            Point bombLocation = new Point((int)_centerPoint.X / 50 * 50 + 25, (int)_centerPoint.Y / 50 * 50 + 25);

            if (FindSafePlaceUp(_centerPoint, 1, true, true, bombLocation) != int.MaxValue)
                return true;
            if (FindSafePlaceDown(_centerPoint, 1, true, true, bombLocation) != int.MaxValue)
                return true;
            if (FindSafePlaceLeft(_centerPoint, 1, true, true, bombLocation) != int.MaxValue)
                return true;
            if (FindSafePlaceRight(_centerPoint, 1, true, true, bombLocation) != int.MaxValue)
                return true;
            return false;
        }

        private bool IsInBombHitRange(Point bombLocation, Point playerLocation)
        {
            double bombRight = bombLocation.X + 25;
            double bombLeft = bombLocation.X - 25;
            double bombTop = bombLocation.Y - 25;
            double bombBottom = bombLocation.Y + 25;
            double playerRight = playerLocation.X + 24;
            double playerLeft = playerLocation.X - 24;
            double playerTop = playerLocation.Y - 24;
            double playerBottom = playerLocation.Y + 24;
            if ((playerRight > bombLeft && playerRight < bombRight) || (playerLeft > bombLeft && playerLeft < bombRight))
                if (Math.Abs(bombLocation.Y - playerLocation.Y) <= 50 * (_bombStr + 1))
                    return true;
            if ((playerBottom < bombTop && playerBottom > bombBottom) || (playerTop > bombTop && playerTop < bombBottom))
                if (Math.Abs(bombLocation.X - playerLocation.X) <= 50 * (_bombStr + 1))
                    return true;
            return false;

        }
        private async static void ShowMessageDialog()
        {
            var messegeDialog = new MessageDialog("DANGER");
            await messegeDialog.ShowAsync();
        }
    }
}
