using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class SpeedPowerUp : PowerUp
    {
        public SpeedPowerUp(Image image) : base(image, "Assets/SpeedUpgrade.png")
        {

        }

        public override void PickPowerUp(Player player)
        {
            player.UpgradeMoveSpeed();
        }
    }
}