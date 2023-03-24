using CommunityToolkit.Mvvm.ComponentModel;
using Shared;

namespace WpfApp1
{

    public partial class Product:ObservableObject
    {
        private int _id;
        private string _name=null!;
        private decimal _price;
        private string? _description;
        private Category _category = null!;
        [ObservableProperty]
        private byte[]? _image;
        public int Id 
        { 
            get=>_id;
            set=>SetProperty(ref _id, value);
        }
        public string Name 
        {
            get => _name;
            set => SetProperty(ref _name, value);
        } 

        public decimal Price 
        {
            get => _price;
            set => SetProperty(ref _price, value);
        }

        public string? Description 
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }


        public virtual Category Category 
        {
            get => _category;
            set => SetProperty(ref _category, value);
        } 

    }
}
