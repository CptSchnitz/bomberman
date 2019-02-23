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
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void SetNavViewHeader(String header)
        {
            NavView.Header = header;
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            TextBlock itemContent = (TextBlock)args.InvokedItem;
            if (itemContent != null)
            {
                switch (itemContent.Tag)
                {
                    case "Nav_Home":
                        ContentFrame.Navigate(typeof(HomePage));
                        SetNavViewHeader("Home");
                        break;
                    case "Nav_NewGame":
                        ContentFrame.Navigate(typeof(NewGame));
                        SetNavViewHeader("New Game");
                        break;
                    case "Nav_Help":
                        ContentFrame.Navigate(typeof(HelpPage));
                        SetNavViewHeader("Help");
                        break;
                    case "Nav_About":
                        ContentFrame.Navigate(typeof(AboutPage));
                        SetNavViewHeader("About");
                        break;
                }
            }
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            NavView.SelectedItem = NavView.MenuItems[0];
            SetNavViewHeader("Home");
            ContentFrame.Navigate(typeof(HomePage));
        }
    }
}


