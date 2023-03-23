using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp1;

namespace WpfApp2;

internal sealed class ShoppingCartViewModel : ObservableObject
{
    private int _totalQuantity;
    private decimal _grandTotal;

    public ShoppingCartViewModel()
    {
        CartItems = new ObservableCollection<ShoppingCartItem>();
        AddProductCommand = new RelayCommand<Product>(AddProduct);
        RemoveProductCommand = new RelayCommand<Product>(RemoveProduct);
        IncreaseQuantityCommand = new RelayCommand<Product>(IncreaseQuantity);
        DecreaseQuantityCommand = new RelayCommand<Product>(DecreaseQuantity);
        BuyCommand = new RelayCommand(Buy, CanBuy);
    }

    public ObservableCollection<ShoppingCartItem> CartItems { get; }

    public RelayCommand<Product> AddProductCommand { get; }

    public RelayCommand<Product> RemoveProductCommand { get; }

    public RelayCommand<Product> IncreaseQuantityCommand { get; }

    public RelayCommand<Product> DecreaseQuantityCommand { get; }

    public RelayCommand BuyCommand { get; }

    public int TotalQuantity
    {
        get => _totalQuantity;
        private set => SetProperty(ref _totalQuantity, value);
    }

    public decimal GrandTotal
    {
        get => _grandTotal;
        private set => SetProperty(ref _grandTotal, value);
    }

    private void AddProduct(Product? product) =>
        ChangeCartItem(product, i => i.Quantity++, p => CartItems.Add(new ShoppingCartItem(p)));

    private void RemoveProduct(Product? product) => ChangeCartItem(product, i => CartItems.Remove(i));

    private void IncreaseQuantity(Product? product) => ChangeCartItem(product, i => i.Quantity++);

    private void DecreaseQuantity(Product? product) =>
        ChangeCartItem(product, i =>
        {
            i.Quantity--;
            if (i.Quantity == 0)
            {
                CartItems.Remove(i);
            }
        });

    private void ChangeCartItem(
        Product? product,
        Action<ShoppingCartItem> action1,
        Action<Product>? action2 = null)
    {
        if (product == null)
            return;

        var cartItem = CartItems.FirstOrDefault(x => x.Product == product);

        if (cartItem != null)
        {
            action1(cartItem);
        }
        else
        {
            action2?.Invoke(product);
        }

        TotalQuantity = CartItems.Sum(x => x.Quantity);
        GrandTotal = CartItems.Sum(x => x.Total);
        BuyCommand.NotifyCanExecuteChanged();
    }

    private void Buy()
    {
        CartItems.Clear();
        TotalQuantity = 0;
        GrandTotal = 0;
    }

    private bool CanBuy() => CartItems.Count > 0;
}