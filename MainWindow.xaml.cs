using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace Waluty
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel;
        public MainWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            MainViewModel = new MainViewModel();
            DataContext = MainViewModel;
            Loaded += MainWindow_Loaded;
            
        }




        private void UpdateDataButton_Click(object sender, RoutedEventArgs e)
        {
           
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void MainWindow_Exit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainViewModel.SaveUserSettings();
        }
        private void ChartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                MainViewModel.CreateChart(button.Content.ToString());
 
            }
        }

        private void BoxChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                string msg = checkBox.Tag as string;
                MainViewModel.AddItem(msg);

                
            }
        }

        private void BoxUnChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                string msg = checkBox.Tag as string;
                MainViewModel.RemoveItem(msg);
                
            }
        }

    }
}