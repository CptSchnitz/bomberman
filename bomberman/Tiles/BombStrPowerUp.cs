using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace bomberman
{
    class BombStrPowerUp : PowerUp
    {
        public BombStrPowerUp(Image image) : base(image, "Assets/BombStrUpgrade.png")
        {

        }

        public override void PickPowerUp(Player player)
        {
            player.UpgradeBombStrength();
        }
    }
}