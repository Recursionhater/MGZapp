using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp1
{
    
    public partial class Product : ObservableObject
    {
        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private string _name;
        [ObservableProperty]
        private decimal _price;
        [ObservableProperty]
        private string? _description;
        //public int Id { get;  set; }
        //public string Name { get;  set; }
        //public decimal Price { get;  set; }
        //public string Description { get;  set; }
        public Product() { 
        _name= string.Empty;
            _description = string.Empty;
        }
        
    }
}
