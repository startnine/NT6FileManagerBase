using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NT6FileManagerBase
{
    /// <summary>
    /// Interaction logic for NavigationBar.xaml
    /// </summary>
    public partial class NavigationBar : UserControl
    {
        public event EventHandler<RoutedEventArgs> NavBackButtonClick;
        public event EventHandler<RoutedEventArgs> NavForwardButtonClick;
        public event EventHandler<RoutedEventArgs> NavUpButtonClick;
        public event EventHandler<KeyEventArgs> NavAddressBarKeyDown;

        public NavigationBar()
        {
            InitializeComponent();
        }

        public void ValidateNavButtonStates(bool historyIndexZero, bool IndexLessThan, bool listCount, bool dirExists)
        {
            if (historyIndexZero)
                NavBackButton.IsEnabled = false;
            else
                NavBackButton.IsEnabled = true;

            if (IndexLessThan)
                NavForwardButton.IsEnabled = false;
            else
                NavForwardButton.IsEnabled = true;

            try
            {
                if (listCount)
                    NavHistoryButton.IsEnabled = true;
                else
                    NavHistoryButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                NavHistoryButton.IsEnabled = false;
            }

            try
            {
                if (dirExists)
                    NavUpButton.IsEnabled = true;
                else
                    NavUpButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                NavUpButton.IsEnabled = false;
            }
        }

        private void NavBackButton_Click(object sender, RoutedEventArgs e)
        {
            NavBackButtonClick?.Invoke(sender, e);
        }

        private void NavForwardButton_Click(object sender, RoutedEventArgs e)
        {
            NavForwardButtonClick?.Invoke(sender, e);
        }

        private void NavHistoryButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NavUpButton_Click(object sender, RoutedEventArgs e)
        {
            NavUpButtonClick?.Invoke(sender, e);
        }

        private void AddressBox_KeyDown(object sender, KeyEventArgs e)
        {
            NavAddressBarKeyDown?.Invoke(sender, e);
        }
    }
}
