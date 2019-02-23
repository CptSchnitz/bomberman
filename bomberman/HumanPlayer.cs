using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class HumanPlayer : Player
    {
        public HumanPlayer(XYCoordinates spawnCoordinates, Canvas canvas) : base(spawnCoordinates, canvas, "Assets/redcatface.png")
        {

        }
    }
}
