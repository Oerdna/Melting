using Melting.ModelView;
using Melting.View;
using System.Windows;

namespace Melting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MainWinViewModel mainViewModel;

        public MainWindow()
        {
            // Попробуем свзяать список портов
            InitializeComponent();
            mainViewModel = new MainWinViewModel();
            this.DataContext = mainViewModel;
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            Window about = new About();
            about.ShowDialog();
        }

        private void MenuItem_NormalWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 720;
            this.Height = 480;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
