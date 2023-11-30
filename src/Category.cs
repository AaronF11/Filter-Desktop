using MaterialDesignThemes.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Filter_Desktop.src
{
    public class Category : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<Category> _parentCategories;
        private string _name;
        private ObservableCollection<URL> _urls;
        private bool _exception;
        private CheckBox _checkBox;
        private Button _deleteButton;
        private Button _showURLsButton;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Category> ParentCategories
        {
            get
            {
                return _parentCategories;   
            }
            set
            {
                if (_parentCategories != value)
                {
                    _parentCategories = value;
                }
            }
        }

        public string CategoryName
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

        public ObservableCollection<URL> URLS
        {
            get
            {
                return _urls;
            }
            set
            {
                if (_urls != value)
                {
                    _urls = value;
                }
            }
        }


        public bool Forbid
        {
            get { return _exception; }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    OnPropertyChanged();
                }
            }
        }

        public Category()
        {
            _urls = new ObservableCollection<URL>();
            //_urls = new ObservableCollection<string>();
            
            InitializeComponents();
        }

        public Category(string categoryName) : this()
        {
            this.CategoryName = categoryName;
        }

        private void InitializeComponents()
        {
            _checkBox = new CheckBox
            {
                Margin = new Thickness(5)
            };

            _checkBox.Checked += CheckBox_Checked;
            _checkBox.Unchecked += CheckBox_Checked;

            _checkBox.SetBinding(ContentProperty, new Binding("CategoryName")
            {
                Source = this
            });

            _deleteButton = new Button
            {
                Content = new TextBlock { Text = "🗑️" }, // Aquí podrías usar un icono más complejo como un PackIcon
                Style = (Style)FindResource("MaterialDesignFlatButton"),
                Margin = new Thickness(5)
            };

            _showURLsButton = new Button
            {
                Style = (Style)FindResource("MaterialDesignFlatButton"),
                Margin = new Thickness(5)
            };

            PackIcon changeIcon = new PackIcon { Kind = PackIconKind.TableEye };

            _showURLsButton.Content = changeIcon;

            _showURLsButton.Click += _showURLsButton_Click;

            _deleteButton.Click += DeleteButton_Click;

            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            panel.Children.Add(_checkBox);
            panel.Children.Add(_deleteButton);
            panel.Children.Add(_showURLsButton);

            this.Content = panel;
        }

        private void _showURLsButton_Click(object sender, RoutedEventArgs e)
        {
            Modal m = new Modal(URLS);

            m.ShowDialog();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            _exception = (bool)checkBox.IsChecked;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            _parentCategories.Remove(this);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return CategoryName;
        }
    }
}
