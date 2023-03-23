using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace WpfApp2;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LoginCommand))] private string _username = "";
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LoginCommand))] private string _password = "";
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(LoginCommand))] private string _error = "";
    private bool _isLoggedIn;
    private readonly IDbContextFactory<AppDbContext> _factory;

    public LoginViewModel(IDbContextFactory<AppDbContext> factory)
    {
        LoginCommand = new AsyncRelayCommand(Login, CanLogin);
        LogoutCommand = new RelayCommand(Logout, () => IsLoggedIn);
        _factory = factory;
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        private set
        {
            SetProperty(ref _isLoggedIn, value);
            LogoutCommand.NotifyCanExecuteChanged();
        }
    }

    public AsyncRelayCommand LoginCommand { get; }

    public RelayCommand LogoutCommand { get; }

    private async Task Login()
    {
        Error = "";
        using var db = _factory.CreateDbContext();
        if (await db.Users.AnyAsync(u => u.Login == Username && EF.Functions.Collate(u.Password, "Latin1_General_CS_AS") == Password))
        {
            Username = "";
            Password = "";
            IsLoggedIn = true;
        }
        else {
            Error = "Invalid password.";
        }
    }

    private bool CanLogin() => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);

    private void Logout() => IsLoggedIn = false;
}