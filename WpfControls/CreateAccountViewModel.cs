using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace WpfApp2;

public partial class CreateAccountViewModel : ObservableObject
{
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RegisterCommand))] private string _username = "";
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RegisterCommand))] private string _password = "";
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RegisterCommand))] private string _rePassword = "";
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(RegisterCommand))] private string _error = "";
    private readonly IDbContextFactory<AppDbContext> factory;

    public CreateAccountViewModel(IDbContextFactory<AppDbContext> factory)
    {
        RegisterCommand = new AsyncRelayCommand(Register, CanRegister);
        this.factory = factory;
    }

    public AsyncRelayCommand RegisterCommand { get; }

    private async Task Register()
    {
        Error = "";
        using var db = factory.CreateDbContext();
        if (await db.Users.AnyAsync(u => u.Login == Username)) {
            Error = $"User {Username} already exists.";
            return;
        }
        var user = new User
        {
            Login = Username,
            Password = Password
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        Username = "";
        Password = "";
        RePassword = "";
    }

    private bool CanRegister() =>
        !string.IsNullOrEmpty(Username) &&
        !string.IsNullOrEmpty(Password) &&
        string.Equals(Password, RePassword, StringComparison.Ordinal);
}