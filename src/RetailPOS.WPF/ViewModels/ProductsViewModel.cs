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

namespace RetailPOS.WPF.ViewModels
{
    public class ProductListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class ProductsViewModel : ObservableObject
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<ProductListItem> Items { get; } = new();

        private ProductListItem? _selected;
        public ProductListItem? Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public IAsyncRelayCommand LoadCommand { get; }
        public IAsyncRelayCommand AddCommand { get; }
        public IAsyncRelayCommand EditCommand { get; }
        public IAsyncRelayCommand DeleteCommand { get; }

        public ProductsViewModel(IProductRepository productRepository, IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;

            LoadCommand = new AsyncRelayCommand(LoadAsync);
            AddCommand = new AsyncRelayCommand(AddAsync);
            EditCommand = new AsyncRelayCommand(EditAsync, () => Selected != null);
            DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => Selected != null);
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

            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        private async Task AddAsync()
        {
            var window = _serviceProvider.GetService<RetailPOS.WPF.Views.ProductEditWindow>() ?? new RetailPOS.WPF.Views.ProductEditWindow();
            var vm = new ProductEditViewModel();
            window.DataContext = vm;

            var result = window.ShowDialog();
            if (result == true)
            {
                var product = new Product(vm.Name, vm.SKU, vm.Price, vm.StockQuantity, 0, null, null);
                await _productRepository.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                await LoadAsync();
            }
        }

        private async Task EditAsync()
        {
            if (Selected == null) return;

            var product = await _productRepository.GetByIdAsync(Selected.Id);
            if (product == null) return;

            var window = _serviceProvider.GetService<RetailPOS.WPF.Views.ProductEditWindow>() ?? new RetailPOS.WPF.Views.ProductEditWindow();
            var vm = new ProductEditViewModel();
            vm.LoadFrom(product);
            window.DataContext = vm;

            var result = window.ShowDialog();
            if (result == true)
            {
                product.Update(vm.Name, vm.SKU, vm.Price, product.ReorderLevel, null, product.CategoryId);
                _productRepository.Update(product);
                await _unitOfWork.SaveChangesAsync();
                await LoadAsync();
            }
        }

        private async Task DeleteAsync()
        {
            if (Selected == null) return;

            if (MessageBox.Show($"Delete {Selected.Name}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            var product = await _productRepository.GetByIdAsync(Selected.Id);
            if (product == null) return;

            _productRepository.Remove(product);
            await _unitOfWork.SaveChangesAsync();
            await LoadAsync();
        }
    }
}
