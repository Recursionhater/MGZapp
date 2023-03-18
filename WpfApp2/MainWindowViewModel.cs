using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using WpfApp1;

namespace WpfApp2;

internal sealed class MainWindowViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    public AsyncRelayCommand RefreshCommand { get; }
    public LoginViewModel LoginViewModel { get; }

    public CreateAccountViewModel CreateAccountViewModel { get; }

    public MainWindowViewModel(IDbContextFactory<AppDbContext> factory, LoginViewModel lwm, CreateAccountViewModel cavm)
    {
        Products = new ObservableCollection<Product>();
        _factory = factory;
        LoginViewModel = lwm;
        CreateAccountViewModel = cavm;
        LoginViewModel.PropertyChanged += LoginViewModelOnPropertyChanged;
        RefreshCommand = new AsyncRelayCommand(LoadData);

    }

    private void LoginViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(LoginViewModel.IsLoggedIn))
            return;

        if (LoginViewModel.IsLoggedIn)
        {
            _ = LoadData();
        }
        else {
            Products.Clear();
        }
    }
    private async Task LoadData()
    {
        Products.Clear();
        using var dbContext = _factory.CreateDbContext();
        var products = await dbContext.Products.AsNoTracking().ToListAsync();
        foreach (var i in products)
        {
            Products.Add(i);
        }

    }
   

    public ObservableCollection<Product> Products { get; }
}
