using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1;

namespace WpfApp2;

internal partial class ShoppingCartItem : ObservableObject
{
    [ObservableProperty, NotifyPropertyChangedFor(nameof(Total))] private int _quantity = 1;

    public ShoppingCartItem(Product product)
    {
        Product = product;
    }

    public Product Product { get; }

    public decimal Total => Product.Price * Quantity;
}