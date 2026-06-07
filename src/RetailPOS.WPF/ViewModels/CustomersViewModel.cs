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
    public class CustomerListItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
    }

    public class CustomersViewModel : ObservableObject
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<CustomerListItem> Items { get; } = new();

        private CustomerListItem? _selected;
        public CustomerListItem? Selected
        {
            get => _selected;
            set
            {
                if (SetProperty(ref _selected, value))
                {
                    EditCommand.NotifyCanExecuteChanged();
                    DeleteCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public IAsyncRelayCommand LoadCommand { get; }
        public IAsyncRelayCommand AddCommand { get; }
        public IAsyncRelayCommand EditCommand { get; }
        public IAsyncRelayCommand DeleteCommand { get; }

        public CustomersViewModel(ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IServiceProvider serviceProvider)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;

            LoadCommand = new AsyncRelayCommand(LoadAsync);
            AddCommand = new AsyncRelayCommand(AddAsync);
            EditCommand = new AsyncRelayCommand(EditAsync, () => Selected != null);
            DeleteCommand = new AsyncRelayCommand(DeleteAsync, () => Selected != null);
        }

        public async Task LoadAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            Items.Clear();
            foreach (var c in customers.Select(c => new CustomerListItem
            {
                Id = c.Id,
                Name = c.Name,
                Mobile = c.Mobile,
                Email = c.Email?.ToString(),
                Address = c.Address
            }))
            {
                Items.Add(c);
            }

            EditCommand.NotifyCanExecuteChanged();
            DeleteCommand.NotifyCanExecuteChanged();
        }

        private async Task AddAsync()
        {
            var window = _serviceProvider.GetService<RetailPOS.WPF.Views.CustomerEditWindow>() ?? new RetailPOS.WPF.Views.CustomerEditWindow();
            var vm = new CustomerEditViewModel();
            window.DataContext = vm;

            var result = window.ShowDialog();
            if (result == true)
            {
                var customer = new Customer(vm.Name, vm.Mobile, vm.Email, vm.Address);
                await _customerRepository.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();
                await LoadAsync();
            }
        }

        private async Task EditAsync()
        {
            if (Selected == null) return;

            var customer = await _customerRepository.GetByIdAsync(Selected.Id);
            if (customer == null) return;

            var window = _serviceProvider.GetService<RetailPOS.WPF.Views.CustomerEditWindow>() ?? new RetailPOS.WPF.Views.CustomerEditWindow();
            var vm = new CustomerEditViewModel();
            vm.LoadFrom(customer);
            window.DataContext = vm;

            var result = window.ShowDialog();
            if (result == true)
            {
                customer.UpdateContact(vm.Name, vm.Mobile, vm.Email, vm.Address);
                _customerRepository.Update(customer);
                await _unitOfWork.SaveChangesAsync();
                await LoadAsync();
            }
        }

        private async Task DeleteAsync()
        {
            if (Selected == null) return;

            if (MessageBox.Show($"Delete {Selected.Name}?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            var customer = await _customerRepository.GetByIdAsync(Selected.Id);
            if (customer == null) return;

            _customerRepository.Remove(customer);
            await _unitOfWork.SaveChangesAsync();
            await LoadAsync();
        }
    }
}
