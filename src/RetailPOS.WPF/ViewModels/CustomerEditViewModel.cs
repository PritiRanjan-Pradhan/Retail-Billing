using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RetailPOS.WPF.ViewModels
{
    public class CustomerEditViewModel : ObservableObject
    {
        public System.Guid? Id { get; set; }

        private string _name = string.Empty;
        [Required]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _mobile = string.Empty;
        [Required]
        public string Mobile
        {
            get => _mobile;
            set => SetProperty(ref _mobile, value);
        }

        private string? _email;
        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string? _address;
        public string? Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        public void LoadFrom(RetailPOS.Domain.Entities.Customer customer)
        {
            Id = customer.Id;
            Name = customer.Name;
            Mobile = customer.Mobile;
            Email = customer.Email?.ToString();
            Address = customer.Address;
        }
    }
}
