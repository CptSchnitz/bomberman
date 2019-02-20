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
        Canvas canvas;
        Image[] explosions;
        int explosionTimeMiliSec = 2000;
        DispatcherTimer timer;


        public Explosion(List<XYCoordinates> explosionLocation, Canvas canvas)
        {
            this.canvas = canvas;
            explosions = new Image[explosionLocation.Count];
            for (int i = 0; i < explosionLocation.Count; i++)
            {
                Image image = new Image
                {
                    Height = 50,
                    Width = 50,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/explosion.png"))
                };
                Canvas.SetTop(image, explosionLocation[i].Y);
                Canvas.SetLeft(image, explosionLocation[i].X);
                Canvas.SetZIndex(image, 2);
                canvas.Children.Add(image);
                explosions[i] = image;
            }

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, explosionTimeMiliSec);
            timer.Tick += Timer_Tick;
            timer.Start();
        }
        

        private void Timer_Tick(object sender, object e)
        {
            UIElementCollection canvasChildrens = canvas.Children;
            int canvasChildIndex = canvasChildrens.Count - 1;

            for (int i = explosions.GetLength(0) - 1; i >= 0; i--)
            {
                while (!ReferenceEquals(explosions[i], canvasChildrens[canvasChildIndex]))
                {
                    canvasChildIndex--;
                }
                canvasChildrens.RemoveAt(canvasChildIndex--);
            }
            timer.Stop();
        }
    }
}
