using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace bomberman
{
    class Explosion
    {
        private Canvas _canvas;
        private Image[] _explosions;
        private int _explosionTimeMiliSec = 500;
        private DispatcherTimer _timer;

        public Explosion(List<Tile> explodedTiles, Canvas canvas)
        {
            _canvas = canvas;
            _explosions = new Image[explodedTiles.Count];
            for (int i = 0; i < explodedTiles.Count; i++)
            {
                Image image = new Image
                {
                    Height = Game.TileSize,
                    Width = Game.TileSize,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/explosion.png"))
                };
                Canvas.SetTop(image, Canvas.GetTop(explodedTiles[i].Image));
                Canvas.SetLeft(image, Canvas.GetLeft(explodedTiles[i].Image));
                Canvas.SetZIndex(image, 2);
                canvas.Children.Add(image);
                _explosions[i] = image;
            }

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, _explosionTimeMiliSec);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        
        private void Timer_Tick(object sender, object e)
        {
            UIElementCollection canvasChildrens = _canvas.Children;
            foreach (Image image in _explosions)
            {
                canvasChildrens.Remove(image);
            }
            _timer.Stop();
        }
    }
}
