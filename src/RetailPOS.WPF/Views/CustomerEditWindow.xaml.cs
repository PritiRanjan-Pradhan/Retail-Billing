using System.Windows;
using RetailPOS.WPF.ViewModels;

namespace RetailPOS.WPF.Views
{
    public partial class CustomerEditWindow : Window
    {
        public CustomerEditWindow()
        {
            InitializeComponent();
        }

        public CustomerEditViewModel ViewModel => (CustomerEditViewModel)DataContext;

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
