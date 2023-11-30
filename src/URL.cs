using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf; // Asegúrate de tener este paquete NuGet

public class URL : UserControl, INotifyPropertyChanged
{
    private string _URL;
    private bool toggler;

    public string Source
    {
        get
        {
            return _URL;
        }
        set
        {
            _URL = value;
            OnPropertyChanged();
        }
    }

    public TextBlock TxtPermittedUrl;
    public Button BtnDeletePermittedUrl;
    public Button BtnChangeURLList;

    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<URL> ParentURLs { get; set; }
    public ObservableCollection<URL> ParentFlipURLs { get; set; }

    public URL(string url)
    {
        // Crear StackPanel
        StackPanel stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };

        // Crear TextBlock para la URL
        TxtPermittedUrl = new TextBlock
        {
            Text = url,
            Margin = new Thickness(5)
        };

        // Crear botón de eliminar
        BtnDeletePermittedUrl = new Button
        {
            Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"]
        };
        PackIcon deleteIcon = new PackIcon { Kind = PackIconKind.Delete };
        BtnDeletePermittedUrl.Content = deleteIcon;

        BtnDeletePermittedUrl.Click += BtnDeletePermittedUrl_Click;

        // Crear botón de cambiar
        BtnChangeURLList = new Button
        {
            Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"]
        };
        PackIcon changeIcon = new PackIcon { Kind = PackIconKind.Exchange };
        BtnChangeURLList.Content = changeIcon;

        BtnChangeURLList.Click += BtnChangeURLList_Click;

        // Añadir los componentes al StackPanel
        stackPanel.Children.Add(TxtPermittedUrl);
        stackPanel.Children.Add(BtnDeletePermittedUrl);
        stackPanel.Children.Add(BtnChangeURLList);

        // Establecer el StackPanel como el contenido del UserControl
        Content = stackPanel;

        toggler = false;

        Source = url;
    }

    private void BtnChangeURLList_Click(object sender, RoutedEventArgs e)
    {
        toggler = !toggler;

        if (toggler)
        {
            ParentURLs.Remove(this);
            ParentFlipURLs.Add(this);
        }

        if (!toggler)
        {
            ParentFlipURLs.Remove(this);
            ParentURLs.Add(this);
        }
    }

    private void BtnDeletePermittedUrl_Click(object sender, RoutedEventArgs e)
    {
        ParentURLs.Remove(this);
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
