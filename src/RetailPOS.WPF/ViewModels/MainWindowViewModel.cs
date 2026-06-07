using CommunityToolkit.Mvvm.ComponentModel;

namespace RetailPOS.WPF.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public string Title => "RetailPOS";

        public ProductsViewModel ProductsViewModel { get; }

        public MainWindowViewModel(ProductsViewModel productsViewModel)
        {
            ProductsViewModel = productsViewModel;
        }
    }
}
