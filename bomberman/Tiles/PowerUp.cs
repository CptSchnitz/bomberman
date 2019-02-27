using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class PowerUp : Tile
    {
        public PowerUp(Image image, string imageSource) : base (image, imageSource)
        {

        }

        public virtual void PickPowerUp(Player player)
        {

        }

        public override bool Destructable
        {
            get
            {
                return true;
            }
        }
    }
}
