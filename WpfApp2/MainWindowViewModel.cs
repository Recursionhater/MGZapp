using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using ClientApp.Properties;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using WpfApp1;

namespace WpfApp2;

internal partial class MainWindowViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly IConnectionStringProvider _csp;
    private readonly ICollectionView _productsView;
    private int _transitionIndex;
    private string _filter = "";
    private string _categoryFilter = "";
    private ICollectionView _categoriesView;
    private Category? _selectedCategory;
    

    public int TransitionIndex
    {
        get => _transitionIndex;
        private set => SetProperty(ref _transitionIndex, value);
    }
    public ObservableCollection<Product> Products { get; }
    public ObservableCollection<Category> Categories { get; }
    public AsyncRelayCommand RefreshCommand { get; }
    public LoginViewModel LoginViewModel { get; }
    public CreateAccountViewModel CreateAccountViewModel { get; }
    public ConnectViewModel ConnectViewModel { get; }
    public ShoppingCartViewModel ShoppingCartViewModel { get; }
    public AsyncRelayCommand OpenShoppingCartCommand { get; }
    public Category? SelectedCategory
    {
        get=>_selectedCategory;
        set 
        {
            if(SetProperty(ref _selectedCategory, value)) 
            {
                _productsView.Refresh();
            }
                
        } 
    }
    public string Filter
    {
        get => _filter;
        set
        {
            if (SetProperty(ref _filter, value))
            {
                _productsView.Refresh();
            }
        }
    }
    public string CategoryFilter
    {
        get => _categoryFilter;
        set
        {
            if (SetProperty(ref _categoryFilter, value))
            {
                _categoriesView.Refresh();
            }
        }
    }
    public MainWindowViewModel(IDbContextFactory<AppDbContext> factory, LoginViewModel lwm, CreateAccountViewModel cavm,
        IConnectionStringProvider csp , ConnectViewModel cvm, ShoppingCartViewModel shoppingCartViewModel)
    {
        Products = new ObservableCollection<Product>();
        Categories = new ObservableCollection<Category>();
        _factory = factory;
        LoginViewModel = lwm;
        CreateAccountViewModel = cavm;
        _csp = csp;
        _transitionIndex = string.IsNullOrEmpty(csp.ConnectionString) ? 0 : 1;
        LoginViewModel.PropertyChanged += LoginViewModelOnPropertyChanged;
        RefreshCommand = new AsyncRelayCommand(LoadData);
        ConnectViewModel = cvm;
        ConnectViewModel.PropertyChanged += ConnectViewModel_PropertyChanged;
        ShoppingCartViewModel = shoppingCartViewModel;
        _productsView = CollectionViewSource.GetDefaultView(Products);
        _productsView.Filter = x => x is Product p &&
            (string.IsNullOrEmpty(Filter) || p.Name.Contains(Filter)) &&
            (SelectedCategory == null || p.Category.Id == SelectedCategory.Id);
        _categoriesView = CollectionViewSource.GetDefaultView(Categories);
        _categoriesView.Filter = x => string.IsNullOrEmpty(CategoryFilter) ||
             x is Category c && c.Name.Contains(CategoryFilter);
        OpenShoppingCartCommand = new AsyncRelayCommand(OpenShopingCart);
    }

    private async Task OpenShopingCart()
    {
        await DialogHost.Show(ShoppingCartViewModel,"iden");
    }

    private void ConnectViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {

        if (e.PropertyName == nameof(ConnectViewModel.IsConnected) && ConnectViewModel.IsConnected)
        {
            TransitionIndex = 1;
        }
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
            Categories.Clear();
        }
    }
    private async Task LoadData()
    {
        Categories.Clear();
        Products.Clear();
        using var dbContext = _factory.CreateDbContext();
        var products = await dbContext.Products.Include(p=>p.Category).AsNoTracking().ToListAsync();
        foreach (var i in products)
        {
            Products.Add(i);
        }
        var categories = await dbContext.Categories.AsNoTracking().ToListAsync();
        foreach (var i in categories)
        {
            Categories.Add(i);
        }
    }
   

}
