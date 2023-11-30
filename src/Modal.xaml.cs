using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace Filter_Desktop.src
{
    /// <summary>
    /// Lógica de interacción para Modal.xaml
    /// </summary>
    public partial class Modal : Window
    {
        public Modal(ObservableCollection<URL> urls)
        {
            InitializeComponent();

            ObservableCollection<string> urlsStr = new ObservableCollection<string>(urls.Select(url => url.Source));

            UrlsListView.ItemsSource = urlsStr;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
