using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class Wall : Tile
    {
        public Wall(Image image) : base (image, "Assets/wall.png")
        {
            
        }

        public override bool IsPassable(Player player)
        {
            return false;
        }

        public override bool BlocksExplosion => true;
    }
}
