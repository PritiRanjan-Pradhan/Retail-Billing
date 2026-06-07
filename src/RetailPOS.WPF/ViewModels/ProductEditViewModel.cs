using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RetailPOS.WPF.ViewModels
{
    public class ProductEditViewModel : ObservableObject
    {
        public System.Guid? Id { get; set; }

        private string _name = string.Empty;
        [Required]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _sku = string.Empty;
        [Required]
        public string SKU
        {
            get => _sku;
            set => SetProperty(ref _sku, value);
        }

        private decimal _price;
        public decimal Price
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        private int _stockQuantity;
        public int StockQuantity
        {
            get => _stockQuantity;
            set => SetProperty(ref _stockQuantity, value);
        }

        public IAsyncRelayCommand SaveCommand { get; }

        public ProductEditViewModel()
        {
            SaveCommand = new AsyncRelayCommand(() => Task.CompletedTask);
        }

        public void LoadFrom(RetailPOS.Domain.Entities.Product p)
        {
            Id = p.Id;
            Name = p.Name;
            SKU = p.SKU;
            Price = p.Price;
            StockQuantity = p.StockQuantity;
        }
    }
}
