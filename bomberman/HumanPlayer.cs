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
        ControlScheme _controlScheme;
        public HumanPlayer(Point spawnPoint,ControlScheme controlScheme, string iconPath, Canvas canvas) : base(spawnPoint, canvas, iconPath)
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

        public void MoveBasedOnKey(Windows.System.VirtualKey virtualKey, Game game)
        {
            KeyAction keyAction = _controlScheme.GetAction(virtualKey);
            if (keyAction == KeyAction.PlaceBomb && DropBomb(game.Board, out Bomb bomb))
                bomb.BombExplosion += game.Bomb_BombExplosion;
            Move(keyAction, game.Board);

        }
    }
}
