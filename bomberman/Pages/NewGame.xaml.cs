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
        string[] _playerIconsPath = Game.PlayerIconsPath;
        string[] _controlSchemes = { "Arrows", "WASD","Gamepad"};
        Selector[] _gridViews;
        Selector[] _comboBoxes;

        public NewGame()
        {
            this.InitializeComponent();
            this.Loaded += NewGame_Loaded;
        }

        private void NewGame_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxNumOfPlayers.SelectionChanged += ComboBoxNumOfPlayers_SelectionChanged;
            _gridViews = new Selector[3];
            _comboBoxes = new Selector[3];
            _gridViews[0] = gridView1;
            _gridViews[1] = gridView2;
            _gridViews[2] = gridView3;
            _comboBoxes[0] = comboBoxPlayerScheme1;
            _comboBoxes[1] = comboBoxPlayerScheme2;
            _comboBoxes[2] = comboBoxPlayerScheme3;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector selector = sender as Selector;
            if (selector is GridView)
                CheckForSameSelections(selector,_gridViews);
            else
                CheckForSameSelections(selector, _comboBoxes);

            btnStartGame.IsEnabled = CheckToEnableStartGameButton();
        }

        private void CheckForSameSelections(Selector selectedControl, Selector[] controlsWithSelection)
        {
            int senderSelectedIndex = selectedControl.SelectedIndex;
            for (int i = 0; i < controlsWithSelection.Length; i++)
            {
                if (!ReferenceEquals(selectedControl, controlsWithSelection[i]) && (controlsWithSelection[i].SelectedIndex == senderSelectedIndex))
                    controlsWithSelection[i].SelectedIndex = -1;
            }
        }

        private bool CheckToEnableStartGameButton()
        {
            for (int i = comboBoxNumOfPlayers.SelectedIndex; i >= 0; i--)
            {
                if (_gridViews[i].SelectedIndex == -1 || _comboBoxes[i].SelectedIndex == -1)
                    return false;
            }
            return true;
        }

        private void ComboBoxNumOfPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
            (ControlScheme controlScheme, string iconPath)[] playerList = 
                new (ControlScheme controlScheme, string iconPath)[comboBoxNumOfPlayers.SelectedIndex+1];

            for (int i = 0; i < playerList.Length; i++)
            {
                ControlScheme scheme = null;
                switch(_comboBoxes[i].SelectedIndex)
                {
                    case 0:
                        scheme = ControlScheme.Arrows;
                        break;
                    case 1:
                        scheme = ControlScheme.WASD;
                        break;
                    case 2:
                        scheme = ControlScheme.Gamepad;
                        break;

                }
                playerList[i] = (scheme, _gridViews[i].SelectedValue.ToString());
            }
            ((ControlScheme controlScheme, string iconPath)[] playerList, bool botsEnabled) parameters = (playerList, switchBots.IsOn);

            NavigationView navView = Frame.Parent as NavigationView;
            Page page = navView.Parent as MainPage;
            Frame frame = page.Parent as Frame;
            frame.Navigate(typeof(GamePage), parameters);
        }

        private void SwitchBots_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                ComboBoxItem item = comboBoxNumOfPlayers.Items[0] as ComboBoxItem;
                if (toggleSwitch.IsOn == true)
                    item.IsEnabled = true;
                else
                {
                    item.IsEnabled = false;
                    comboBoxNumOfPlayers.SelectedIndex = 1;
                }
            }
        }
    }
}
