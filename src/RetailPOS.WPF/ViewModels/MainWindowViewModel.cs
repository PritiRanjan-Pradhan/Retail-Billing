using CommunityToolkit.Mvvm.ComponentModel;

namespace RetailPOS.WPF.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public string Title => "RetailPOS";

        public ProductsViewModel ProductsViewModel { get; }
        public CustomersViewModel CustomersViewModel { get; }
        public SalesViewModel SalesViewModel { get; }

        public MainWindowViewModel(ProductsViewModel productsViewModel, CustomersViewModel customersViewModel, SalesViewModel salesViewModel)
        {
            ProductsViewModel = productsViewModel;
            CustomersViewModel = customersViewModel;
            SalesViewModel = salesViewModel;
        }
    }
}
