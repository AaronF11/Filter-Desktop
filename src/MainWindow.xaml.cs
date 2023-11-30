using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Filter_Desktop.src
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public ObservableCollection<Category> Categories { get; set; }
		//public ObservableCollection<Category> PermittedCategories { get; set; }
		//public ObservableCollection<Category> BlockedCategories { get; set; }
		public ObservableCollection<URL> PermittedUrls { get; set; }
		public ObservableCollection<URL> BlockedUrls { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			CmbCategories.IsEnabled = false;
			//CbxAddCmbCategories.IsEnabled = false;

			Categories = new ObservableCollection<Category>();

			//PermittedCategories = new ObservableCollection<Category>();
			//BlockedCategories = new ObservableCollection<Category>();
			PermittedUrls = new ObservableCollection<URL>();
			BlockedUrls = new ObservableCollection<URL>();

			// Inicializar categorías de ejemplo
			//PermittedCategories.Add(new Category { Name = "Noticias" });
			//PermittedCategories.Add(new Category { Name = "Juegos" });
			//BlockedCategories.Add(new Category { Name = "Redes Sociales" });
			//BlockedCategories.Add(new Category { Name = "Comercio" });

			// Inicializar URLs de ejemplo
			DataContext = this;

            Categories.CollectionChanged += Categories_CollectionChanged;

            WebView.NavigationCompleted += WebView_NavigationCompleted;

			//ListViewPermittedUrls.ItemsSource = 

		}

        private async void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
			{
                await WebView.CoreWebView2.ExecuteScriptAsync($"document.getElementById('url').innerText = '{TxtSearch.Text} has been blocked';");
                await WebView.CoreWebView2.ExecuteScriptAsync($"document.getElementById('url').href = '{TxtSearch.Text}';");
            }
        }

        private void Categories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CmbCategories.ItemsSource = Categories.Select(c => c.CategoryName).ToList();
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
            if (string.IsNullOrEmpty(TxtSearch.Text))
            {
                MessageBox.Show("La URL a navegar no puede estar vacia.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidUrl(TxtSearch.Text))
            {
                MessageBox.Show("Introduce una URL válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

			// Validation goes here

			string url = TxtSearch.Text;

			if (Categories.Count != 0)
			{
                Categories.ToList().ForEach(cat =>
                {
                    if (cat.Forbid)
                    {
                        string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                        string htmlFilePath = Path.Combine(appDirectory, "src", "docs", "blocked.html");

                        WebView.CoreWebView2.Navigate($"file:///{htmlFilePath.Replace("\\", "/")}");
                    }
					else
					{
                        WebView.CoreWebView2.Navigate(TxtSearch.Text);
                    }
                });

				return;
            }

            if (BlockedUrls.Any(_url => _url.Source.Equals(url, StringComparison.OrdinalIgnoreCase)))
			{
                // Navegar a la vista de URL bloqueada
                string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string htmlFilePath = Path.Combine(appDirectory, "src", "docs", "blocked.html");

				WebView.CoreWebView2.Navigate($"file:///{htmlFilePath.Replace("\\", "/")}");
            }
			else
			{
				WebView.CoreWebView2.Navigate(TxtSearch.Text);
			}


        }

		private void BtnReload_Click(object sender, RoutedEventArgs e)
		{
            if (WebView != null && WebView.CoreWebView2 != null)
            {
                // Obtén la URL actual del documento cargado.
                var currentUri = WebView.CoreWebView2.Source;

                // Verifica si es una ruta de archivo.
                if (currentUri.StartsWith("file://"))
                {
                    // Navega de nuevo a la URL actual para recargar el contenido.
                    WebView.CoreWebView2.Navigate(currentUri);
                }
            }
        }

		// Eventos para categorías y URLs
		private void BtnAddCategory_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(TxtNewCategory.Text))
			{
                MessageBox.Show("La categoría no puede ser vacia.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
            }

            /*if (!IsValidUrl(TxtNewCategory.Text))
			{
                MessageBox.Show("Introduce una URL válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }*/

            Category ca = new Category(TxtNewCategory.Text)
            {
                ParentCategories = Categories
            };

            Categories.Add(ca);

            CmbCategories.ItemsSource = Categories.Select(c => c.CategoryName).ToList();

            TxtNewCategory.Clear();
        }

        private void BtnAddPermitted_Click(object sender, RoutedEventArgs e)
		{
            if (string.IsNullOrEmpty(TxtAddUrl.Text))
            {
                MessageBox.Show("La URL no puede ser vacia.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

			if (!IsValidUrl(TxtAddUrl.Text))
			{
                MessageBox.Show("Introduce una URL válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

			if ((bool)CbxAddCmbCategories.IsChecked)
			{
				// get selected category
				string category = CmbCategories.SelectedValue.ToString();

				Category selectedCategory = Categories.FirstOrDefault(c => c.CategoryName == category);

                URL url = new URL(TxtAddUrl.Text);

				selectedCategory.URLS.Add(url);

				return;
			}

			URL _url = new URL(TxtAddUrl.Text)
			{
				ParentURLs = PermittedUrls,
				ParentFlipURLs = BlockedUrls,
			};

			PermittedUrls.Add(_url);

			TxtAddUrl.Clear();
        }

        private void BtnAddBlocked_Click(object sender, RoutedEventArgs e)
		{
            if (string.IsNullOrEmpty(TxtAddUrl.Text))
            {
                MessageBox.Show("La URL no puede ser vacia.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsValidUrl(TxtAddUrl.Text))
			{
                MessageBox.Show("Introduce una URL válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            /*if ((bool)CbxAddCmbCategories.IsChecked)
            {
                string category = CmbCategories.SelectedValue.ToString();

                Category selectedCategory = Categories.FirstOrDefault(c => c.CategoryName == category);

                URL url = new URL(TxtAddUrl.Text);

                selectedCategory.URLS.Add(url);

				return;
			}*/

			URL _url = new URL(TxtAddUrl.Text)
			{
				ParentURLs = BlockedUrls,
				ParentFlipURLs = PermittedUrls
			};

			BlockedUrls.Add(_url);

            TxtAddUrl.Clear();
        }

        private void CategoryCheckBox_Changed(object sender, RoutedEventArgs e)
		{

			/*var checkBox = sender as CheckBox;
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
			}*/
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

        public static bool IsValidUrl(string url)
        {
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url; // Agregar un esquema predeterminado
            }

            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private void CbxAddCmbCategories_Checked(object sender, RoutedEventArgs e)
        {
			CheckBox box = sender as CheckBox;

			CmbCategories.IsEnabled = (bool)box.IsChecked;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string htmlFilePath = Path.Combine(appDirectory, "src", "docs", "index.html");

            await WebView.EnsureCoreWebView2Async();

			WebView.CoreWebView2.Navigate($"file:///{htmlFilePath.Replace("\\", "/")}");
        }

        private void BtnAddCategoryAction_Click(object sender, RoutedEventArgs e)
        {
			if ((bool)!CbxAddCmbCategories.IsChecked)
			{
                MessageBox.Show("Habilita el modo para seleccionar una categoria", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

			if (Categories.Count == 0)
			{
                MessageBox.Show("Crea una categoría para continuar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

			string category = CmbCategories.SelectedValue.ToString();

			Category selectedCategory = Categories.FirstOrDefault(c => c.CategoryName
			== category);


			URL url = new URL(TxtAddUrl.Text);

			selectedCategory.URLS.Add(url);

			TxtAddUrl.Clear();

            MessageBox.Show("Se ha añadido la URL a la categoría", "¡Exito!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

/*
 * 
 * 
 * <StackPanel Orientation="Horizontal">
                                                    <TextBlock x:Name="TxtPermittedUrl1" Text="https://example.com" Margin="5"/>
                                                    <Button x:Name="BtnDeletePermittedUrl1" Style="{DynamicResource MaterialDesignFlatButton}">
                                                        <materialDesign:PackIcon Kind="Delete" />
                                                    </Button>
                                                    <Button x:Name="BtnChangePermittedUrl1" Style="{DynamicResource MaterialDesignFlatButton}">
                                                        <materialDesign:PackIcon Kind="Exchange" />
                                                    </Button>
                                                </StackPanel>
 * 
 * */