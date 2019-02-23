using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class BombCountPowerUp : PowerUp
    {
        public BombCountPowerUp(Image image) : base(image, "Assets/CountUpgrade.png")
        {

        }

        public override void PickPowerUp(Player player)
        {
            player.UpgradeBombCount();
        }
    }
}