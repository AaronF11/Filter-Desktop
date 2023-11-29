using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Filter_Desktop
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public ObservableCollection<Category> PermittedCategories { get; set; }
		public ObservableCollection<Category> BlockedCategories { get; set; }
		public ObservableCollection<string> PermittedUrls { get; set; }
		public ObservableCollection<string> BlockedUrls { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			PermittedCategories = new ObservableCollection<Category>();
			BlockedCategories = new ObservableCollection<Category>();
			PermittedUrls = new ObservableCollection<string>();
			BlockedUrls = new ObservableCollection<string>();

			// Inicializar categorías de ejemplo
			PermittedCategories.Add(new Category { Name = "Noticias" });
			PermittedCategories.Add(new Category { Name = "Juegos" });
			BlockedCategories.Add(new Category { Name = "Redes Sociales" });
			BlockedCategories.Add(new Category { Name = "Comercio" });

			// Inicializar URLs de ejemplo
			PermittedUrls.Add("https://example-permitted.com");
			BlockedUrls.Add("https://example-blocked.org");

			DataContext = this;
		}

		// INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		// Eventos de la barra de herramientas
		private void BtnBack_Click(object sender, RoutedEventArgs e)
		{
			// Navegar atrás en el WebBrowser
		}

		private void BtnForward_Click(object sender, RoutedEventArgs e)
		{
			// Navegar adelante en el WebBrowser
		}

		private void BtnGo_Click(object sender, RoutedEventArgs e)
		{
			// Navegar a la URL en el TextBox de búsqueda
		}

		private void BtnReload_Click(object sender, RoutedEventArgs e)
		{
			// Recargar la página actual en el WebBrowser
		}

		// Eventos para categorías y URLs
		private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(TxtNewCategory.Text))
			{
				PermittedCategories.Add(new Category { Name = TxtNewCategory.Text });
				TxtNewCategory.Clear();
			}
		}

		private void BtnAddPermitted_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(TxtAddUrl.Text))
			{
				PermittedUrls.Add(TxtAddUrl.Text);
				TxtAddUrl.Clear();
			}
		}

		private void BtnAddBlocked_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(TxtAddUrl.Text))
			{
				BlockedUrls.Add(TxtAddUrl.Text);
				TxtAddUrl.Clear();
			}
		}

		private void CategoryCheckBox_Changed(object sender, RoutedEventArgs e)
		{
			var checkBox = sender as CheckBox;
			var category = checkBox?.DataContext as Category;
			if (category != null)
			{
				if (checkBox.IsChecked ?? false)
				{
					BlockedCategories.Remove(category);
					if (!PermittedCategories.Contains(category))
					{
						PermittedCategories.Add(category);
					}
				}
				else
				{
					PermittedCategories.Remove(category);
					if (!BlockedCategories.Contains(category))
					{
						BlockedCategories.Add(category);
					}
				}
			}
		}

		// Cerrar, minimizar y maximizar eventos
		private void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void BtnMinimize_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void BtnMaximizeRestore_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
		}
	}

	public class Category : INotifyPropertyChanged
{
    private string _name;
    public string Name
    {
        get { return _name; }
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return _name; // Mantén ToString() para devolver solo el nombre
    }

    // Método para crear un CheckBox con el evento vinculado
    public CheckBox CreateCheckBoxWithEvent()
    {
        CheckBox checkBox = new CheckBox
        {
            Content = _name,
            IsChecked = true // O el valor inicial que necesites
        };

        // Aquí puedes vincular el evento Checked y Unchecked del CheckBox
        checkBox.Checked += CheckBox_Checked;
        checkBox.Unchecked += CheckBox_Unchecked;

        return checkBox;
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        // Lógica para cuando el CheckBox se marca
    }

    private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        // Lógica para cuando el CheckBox se desmarca
    }
}

}
