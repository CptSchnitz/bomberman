using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class Crate : Tile
    {
        public Crate (Image image) : base (image, "Assets/crate.png")
        {

        }

        public override bool IsDestructable()
        {
            return true;
        }
        public override bool IsPassable(Player player)
        {
            return false;
        }
    }
}
