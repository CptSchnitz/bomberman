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
        public GamePage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            game = new Game(canvas, 4);
            game.GameOver += GameOver;
        }

        private void GameOver(object sender, GameOverEventArgs e)
        {
            CoreWindow.GetForCurrentThread().KeyUp -= game.KeyUp;
            ForegroundGrid.Visibility = Visibility.Visible;
            stackPanelEndGame.Visibility = Visibility.Visible;
            if (e.Winner == -1)
                txtWinner.Text = "Draw";
            else
                txtWinner.Text = $"Player {(e.Winner + 1).ToString()} Won";
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            CoreWindow.GetForCurrentThread().KeyUp -= game.KeyUp;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            CoreWindow.GetForCurrentThread().KeyUp += game.KeyUp;
            ForegroundGrid.Visibility = Visibility.Collapsed;
            StartGame.Visibility = Visibility.Collapsed;
            game.StartGame();
        }

        private void BtnToMainPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
