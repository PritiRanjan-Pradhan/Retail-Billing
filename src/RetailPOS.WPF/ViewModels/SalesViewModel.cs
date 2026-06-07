using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using RetailPOS.Application.Interfaces;
using RetailPOS.Domain.Entities;
using RetailPOS.Domain.Enums;

namespace RetailPOS.WPF.ViewModels
{
    public class SaleCartItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    public class SalesViewModel : ObservableObject
    {
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RetailPOS.Application.Interfaces.IReceiptPrinter _receiptPrinter;

        public ObservableCollection<ProductListItem> AvailableProducts { get; } = new();
        public ObservableCollection<CustomerListItem> AvailableCustomers { get; } = new();
        public ObservableCollection<SaleCartItem> CartItems { get; } = new();

        private ProductListItem? _selectedProduct;
        public ProductListItem? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        private CustomerListItem? _selectedCustomer;
        public CustomerListItem? SelectedCustomer
        {
            get => _selectedCustomer;
            set => SetProperty(ref _selectedCustomer, value);
        }

        private int _quantity = 1;
        public int Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        public decimal Total => CartItems.Sum(i => i.LineTotal);

        public IAsyncRelayCommand LoadCommand { get; }
        public IAsyncRelayCommand AddToCartCommand { get; }
        public IAsyncRelayCommand RemoveFromCartCommand { get; }
        public IAsyncRelayCommand SaveSaleCommand { get; }
        public IAsyncRelayCommand PrintReceiptCommand { get; }

        private SaleCartItem? _selectedCartItem;
        public SaleCartItem? SelectedCartItem
        {
            get => _selectedCartItem;
            set
            {
                if (SetProperty(ref _selectedCartItem, value))
                {
                    RemoveFromCartCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public SalesViewModel(
            IProductRepository productRepository,
            ICustomerRepository customerRepository,
            ISaleRepository saleRepository,
            IUnitOfWork unitOfWork,
            RetailPOS.Application.Interfaces.IReceiptPrinter receiptPrinter)
        {
            _productRepository = productRepository;
            _customerRepository = customerRepository;
            _saleRepository = saleRepository;
            _unitOfWork = unitOfWork;
            _receiptPrinter = receiptPrinter;

            LoadCommand = new AsyncRelayCommand(LoadAsync);
            AddToCartCommand = new AsyncRelayCommand(AddToCartAsync);
            RemoveFromCartCommand = new AsyncRelayCommand(RemoveFromCartAsync, () => SelectedCartItem != null);
            SaveSaleCommand = new AsyncRelayCommand(SaveSaleAsync, () => CartItems.Any());
            PrintReceiptCommand = new AsyncRelayCommand(PrintReceiptAsync, () => CartItems.Any());
        }

        public async Task LoadAsync()
        {
            AvailableProducts.Clear();
            AvailableCustomers.Clear();
            CartItems.Clear();

            var products = await _productRepository.GetAllAsync();
            foreach (var product in products.Select(p => new ProductListItem
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            }))
            {
                AvailableProducts.Add(product);
            }

            var customers = await _customerRepository.GetAllAsync();
            foreach (var customer in customers.Select(c => new CustomerListItem
            {
                Id = c.Id,
                Name = c.Name,
                Mobile = c.Mobile,
                Email = c.Email?.ToString(),
                Address = c.Address
            }))
            {
                AvailableCustomers.Add(customer);
            }

            OnPropertyChanged(nameof(Total));
            SaveSaleCommand.NotifyCanExecuteChanged();
            PrintReceiptCommand.NotifyCanExecuteChanged();
            RemoveFromCartCommand.NotifyCanExecuteChanged();
        }

        private Task AddToCartAsync()
        {
            if (SelectedProduct == null || Quantity <= 0)
            {
                MessageBox.Show("Select a product and quantity first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return Task.CompletedTask;
            }

            var existing = CartItems.FirstOrDefault(i => i.ProductId == SelectedProduct.Id);
            if (existing != null)
            {
                existing.Quantity += Quantity;
                OnPropertyChanged(nameof(Total));
                return Task.CompletedTask;
            }

            CartItems.Add(new SaleCartItem
            {
                ProductId = SelectedProduct.Id,
                ProductName = SelectedProduct.Name,
                SKU = SelectedProduct.SKU,
                UnitPrice = SelectedProduct.Price,
                Quantity = Quantity
            });
            OnPropertyChanged(nameof(Total));
            SaveSaleCommand.NotifyCanExecuteChanged();
            PrintReceiptCommand.NotifyCanExecuteChanged();
            return Task.CompletedTask;
        }

        private Task RemoveFromCartAsync()
        {
            if (SelectedCartItem != null)
            {
                CartItems.Remove(SelectedCartItem);
                SelectedCartItem = null;
                OnPropertyChanged(nameof(Total));
                SaveSaleCommand.NotifyCanExecuteChanged();
                PrintReceiptCommand.NotifyCanExecuteChanged();
            }
            return Task.CompletedTask;
        }

        private async Task SaveSaleAsync()
        {
            if (!CartItems.Any())
            {
                MessageBox.Show("Add products to the cart before saving.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var sale = new Sale(SelectedCustomer?.Id);
            foreach (var item in CartItems)
            {
                sale.AddItem(new SaleItem(item.ProductId, item.ProductName, item.SKU, item.UnitPrice, item.Quantity));
            }

            sale.Validate();
            await _saleRepository.AddAsync(sale);
            await _unitOfWork.SaveChangesAsync();

            MessageBox.Show($"Sale saved with invoice {sale.InvoiceNumber}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadAsync();
        }

        private Task PrintReceiptAsync()
        {
            if (!CartItems.Any())
            {
                MessageBox.Show("Add products to the cart before printing receipt.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return Task.CompletedTask;
            }

            var receipt = _receiptPrinter.GenerateReceipt(SelectedCustomer?.Name, CartItems.Select(i => (i.ProductName, i.Quantity, i.UnitPrice, i.LineTotal)).ToList(), Total);
            _receiptPrinter.PrintReceipt(receipt);
            return Task.CompletedTask;
        }
    }
}
