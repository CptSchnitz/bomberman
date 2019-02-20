using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace bomberman
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Board board;
        HumanPlayer player;
        List<Bomb> bombsList;
        DispatcherTimer bombTimer;

        public MainPage()
        {
            this.InitializeComponent();
            bombsList = new List<Bomb>();
            bombTimer = new DispatcherTimer();
            bombTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            bombTimer.Tick += BombTimer_Tick;
            bombTimer.Start();
        }

        private void BombTimer_Tick(object sender, object e)
        {
            for (int i = bombsList.Count - 1; i >= 0; i--)
            {
                if (bombsList[i].Exploded)
                    bombsList.RemoveAt(i);
                else if (bombsList[i].Tick(board))
                {
                    List<XYCoordinates> explosionCoordinates = board.ExplodeBomb(bombsList[i]);
                    new Explosion(explosionCoordinates, canvas);
                }
            }
        }

        private void DrawGame()
        {           
            board = new Board(canvas);

            Image image = new Image
            {
                Height = 50,
                Width = 50,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                //RenderTransformOrigin = new Point(0.5, 0.5)
            };
            player = new HumanPlayer(25, 25, image);
            Canvas.SetTop(image, 0);
            Canvas.SetLeft(image, 0);
            canvas.Children.Add(image);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            CoreWindow.GetForCurrentThread().KeyDown += MainPage_KeyDown;
            DrawGame();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CoreWindow.GetForCurrentThread().KeyDown -= MainPage_KeyDown;
        }

        private void MainPage_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            Windows.System.VirtualKey key = args.VirtualKey;
            switch (key)
            {
                case Windows.System.VirtualKey.W: // up

                    player.MoveUp(board);
                    break;
                case Windows.System.VirtualKey.A: // left
                    player.MoveLeft(board);
                    break;
                case Windows.System.VirtualKey.S: // down
                    player.MoveDown(board);
                    break;
                case Windows.System.VirtualKey.D: // right
                    player.MoveRight(board);
                    break;
                case Windows.System.VirtualKey.Space:
                    if (player.DropBomb(board, out Bomb bomb))
                        bombsList.Add(bomb);
                    break;

            }
        }
    }
}
