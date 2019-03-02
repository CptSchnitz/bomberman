using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class HumanPlayer : Player
    {
        private ControlScheme _controlScheme;       
        private bool _resetDirectionFlag = false;
        private bool _firstMove = false;

        public HumanPlayer(Point spawnPoint, ControlScheme controlScheme, string iconPath, Canvas canvas, Game game) : base(spawnPoint, canvas, iconPath, game)
        {
            _controlScheme = controlScheme;
        }

        public ControlScheme ControlScheme
        {
            get
            {
                return _controlScheme;
            }
        }

        public override void DoAction()
        {
            if (!_firstMove && _resetDirectionFlag)
            {
                _currentMoveDirection = Direction.Null;
                _resetDirectionFlag = false;
            }
            _firstMove = false;
            base.DoAction();
        }

        public void OnKeyDown(Windows.System.VirtualKey virtualKey)
        {
            KeyAction keyAction = _controlScheme.GetAction(virtualKey);
            if (keyAction == KeyAction.PlaceBomb)
            {
                DropBomb();
                return;
            }
            Direction direction = KeyActionToDirection(keyAction);
            _currentMoveDirection = KeyActionToDirection(keyAction);
            _firstMove = true;

        }
        public void OnKeyUp(Windows.System.VirtualKey virtualKey)
        {
            KeyAction keyAction = _controlScheme.GetAction(virtualKey);
            if (KeyActionToDirection(keyAction) == _currentMoveDirection)
                _resetDirectionFlag = true;
        }
        private static Direction KeyActionToDirection(KeyAction keyAction)
        {
            Direction direction = Direction.Null;
            switch (keyAction)
            {
                case KeyAction.Up:
                    direction = Direction.Up;
                    break;
                case KeyAction.Down:
                    direction = Direction.Down;
                    break;
                case KeyAction.Left:
                    direction = Direction.Left;
                    break;
                case KeyAction.Right:
                    direction = Direction.Right;
                    break;
            }
            return direction;
        }
    }
}
