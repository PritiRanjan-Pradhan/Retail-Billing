using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailPOS.Application.Interfaces;

namespace RetailPOS.WPF.ViewModels
{
    public class ProductListItem
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class ProductsViewModel : ObservableObject
    {
        private readonly IProductRepository _productRepository;

        public ObservableCollection<ProductListItem> Items { get; } = new();

        public IAsyncRelayCommand LoadCommand { get; }

        public ProductsViewModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
            LoadCommand = new AsyncRelayCommand(LoadAsync);
        }

        public async Task LoadAsync()
        {
            var products = await _productRepository.GetAllAsync();
            Items.Clear();
            foreach (var p in products.Select(p => new ProductListItem
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            }))
            {
                Items.Add(p);
            }
        }
    }
}
