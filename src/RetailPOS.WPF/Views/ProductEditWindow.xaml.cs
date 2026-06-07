using System.Windows;
using RetailPOS.WPF.ViewModels;

namespace RetailPOS.WPF.Views
{
    public partial class ProductEditWindow : Window
    {
        public ProductEditWindow()
        {
            InitializeComponent();
        }

        public ProductEditViewModel ViewModel => (ProductEditViewModel)DataContext;

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
