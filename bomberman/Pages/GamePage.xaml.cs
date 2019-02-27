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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace bomberman
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        Game game;
        ((ControlScheme controlScheme, string iconPath)[] playerList, bool botsEnabled) gameParameters;
        int _countdownCount;
        public GamePage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            gameParameters = (((ControlScheme controlScheme, string iconPath)[] playerList, bool botsEnabled))e.Parameter;
            base.OnNavigatedTo(e);
        }
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            InitGame();
        }

        private void GameOver(object sender, GameOverEventArgs e)
        {
            CoreWindow.GetForCurrentThread().KeyDown -= game.OnKeyDown;
            CoreWindow.GetForCurrentThread().KeyUp -= game.OnKeyUp;
            ForegroundGrid.Visibility = Visibility.Visible;
            stackPanelEndGame.Visibility = Visibility.Visible;
            if (e.Winner == -1)
                txtWinner.Text = "Game Over";
            else
            {
                imgWinner.Visibility = Visibility.Visible;
                imgWinner.Source = new BitmapImage(new Uri(gameParameters.playerList[e.Winner].iconPath));
                txtWinner.Text = "Is the winner";
            }
            game.GameOver -= GameOver;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CoreWindow.GetForCurrentThread().KeyDown -= game.OnKeyDown;
            CoreWindow.GetForCurrentThread().KeyUp -= game.OnKeyUp;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {         
            btnStartGame.Visibility = Visibility.Collapsed;
            txtCountdown.Visibility = Visibility.Visible;
            DispatcherTimer countdownTimer = new DispatcherTimer();
            countdownTimer.Tick += CountdownTimer_Tick;
            _countdownCount = 3;
            txtCountdown.Text = _countdownCount.ToString();
            countdownTimer.Interval = new TimeSpan(0, 0, 1);
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, object e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;
            switch (_countdownCount)
            {
                case 0:
                    timer.Stop();
                    StartGame();
                    txtCountdown.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    txtCountdown.Text = "Go!";
                    _countdownCount--;
                    break;
                default:
                    _countdownCount--;
                    txtCountdown.Text = _countdownCount.ToString();
                    break;
            }
        }

        private void StartGame()
        {
            txtCountdown.Visibility = Visibility.Visible;
            ForegroundGrid.Visibility = Visibility.Collapsed;
            CoreWindow.GetForCurrentThread().KeyDown += game.OnKeyDown;
            CoreWindow.GetForCurrentThread().KeyUp += game.OnKeyUp;
            game.StartGame();
        }

        private void BtnToMainPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            imgWinner.Visibility = Visibility.Collapsed;
            stackPanelEndGame.Visibility = Visibility.Collapsed;
            canvas.Children.Clear();
            InitGame();
            btnStartGame.Visibility = Visibility.Visible;
        }

        private void InitGame()
        {
            game = new Game(canvas, gameParameters.playerList, gameParameters.botsEnabled);
            game.GameOver += GameOver;
            game.Paused += Game_Paused;
            game.UnPaused += Game_UnPaused;
        }

        private void Game_UnPaused(object sender, EventArgs e)
        {
            ForegroundGrid.Visibility = Visibility.Collapsed;
            txtPaused.Visibility = Visibility.Collapsed;
        }

        private void Game_Paused(object sender, EventArgs e)
        {
            ForegroundGrid.Visibility = Visibility.Visible;
            txtPaused.Visibility = Visibility.Visible;
        }
    }
}
