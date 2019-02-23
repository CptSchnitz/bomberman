using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class NewGame : Page
    {
        string[] _playerIconsPath = { "ms-appx:///Assets/PlayerIcons/Angry.png", "ms-appx:///Assets/PlayerIcons/derp.png",
            "ms-appx:///Assets/PlayerIcons/Drool.png", "ms-appx:///Assets/PlayerIcons/Fr.png", "ms-appx:///Assets/PlayerIcons/Great.png",
            "ms-appx:///Assets/PlayerIcons/Obese.png", "ms-appx:///Assets/PlayerIcons/Ree.png", "ms-appx:///Assets/PlayerIcons/thump.png"};

        public NewGame()
        {
            this.InitializeComponent();
            this.Loaded += NewGame_Loaded;
        }

        private void NewGame_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxNumOfPlayers.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView senderGridView = sender as GridView;
            int senderGridSelectedIndex = senderGridView.SelectedIndex;


            if (!ReferenceEquals(senderGridView, gridView1) && (gridView1.SelectedIndex == senderGridSelectedIndex))
                gridView1.SelectedIndex = -1;

            if (!ReferenceEquals(senderGridView, gridView2) && (gridView2.SelectedIndex == senderGridSelectedIndex))
                gridView2.SelectedIndex = -1;

            if (!ReferenceEquals(senderGridView, gridView3) && (gridView3.SelectedIndex == senderGridSelectedIndex))
                gridView3.SelectedIndex = -1;

            btnStartGame.IsEnabled = CheckToEnableStartGameButton();
        }

        private bool CheckToEnableStartGameButton()
        {
            switch (comboBoxNumOfPlayers.SelectedIndex)
            {
                case 2:
                    if (gridView3.SelectedIndex == -1)
                    {
                        return false;
                    }
                    goto case 1;
                case 1:
                    if (gridView2.SelectedIndex == -1)
                    {
                        return false;
                    }
                    goto case 0;
                case 0:
                    if (gridView1.SelectedIndex == -1)
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            switch (comboBox.SelectedIndex)
            {
                case 0:
                    player2Stack.Visibility = Visibility.Collapsed;
                    player3Stack.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    player2Stack.Visibility = Visibility.Visible;
                    player3Stack.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    player2Stack.Visibility = Visibility.Visible;
                    player3Stack.Visibility = Visibility.Visible;
                    break;
            }
            btnStartGame.IsEnabled = CheckToEnableStartGameButton();
        }

        private void BtnStartGame_Click(object sender, RoutedEventArgs e)
        {
            NavigationView navView = Frame.Parent as NavigationView;
            Page page = navView.Parent as MainPage;
            Frame frame = page.Parent as Frame;
            frame.Navigate(typeof(GamePage));
        }
    }
}
