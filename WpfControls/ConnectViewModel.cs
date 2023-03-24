using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Shared;

namespace WpfApp2;

public partial class ConnectViewModel : ObservableObject
{
    private readonly IConnectionStringProvider _csp;
    private readonly IDbContextFactory<AppDbContext> _dbfactory;
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ConnectCommand))] private string _selectedDataSource = "";
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ConnectCommand)), NotifyPropertyChangedFor(nameof(IsSql))]
    private AuthenticationMode _authenticationMode = AuthenticationMode.Windows;
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ConnectCommand))] private string _username = "";
    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(ConnectCommand))] private string _password = "";
    private bool _isLoading;
    private bool _isConnected;

    public ConnectViewModel(IConnectionStringProvider csp, IDbContextFactory<AppDbContext> dbfactory)
    {
        _csp = csp;
        _dbfactory = dbfactory;
        DataSources = new ObservableCollection<string>();
        ConnectCommand = new AsyncRelayCommand(Connect, CanConnect);
        LoadDataSourcesCommand = new AsyncRelayCommand(LoadDataSources);
    }

    public ObservableCollection<string> DataSources { get; }

    public IReadOnlyDictionary<AuthenticationMode, string> AuthenticationModes { get; } =
        new Dictionary<AuthenticationMode, string>
        {
            [AuthenticationMode.Windows] = "Windows Authentication",
            [AuthenticationMode.SqlServer] = "SQL Server Authentication"
        };

    public AsyncRelayCommand ConnectCommand { get; }

    public AsyncRelayCommand LoadDataSourcesCommand { get; }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        private set => SetProperty(ref _isConnected, value);
    }

    public bool IsSql => AuthenticationMode == AuthenticationMode.SqlServer;

    private async Task Connect()
    {
        var csb = new SqlConnectionStringBuilder
        {
            TrustServerCertificate = true,
            DataSource = SelectedDataSource,
            //InitialCatalog = "MGZ"
        };

        if (AuthenticationMode == AuthenticationMode.Windows)
        {
            csb.IntegratedSecurity = true;
        }
        else
        {
            csb.UserID = Username;
            csb.Password = Password;
        }

        await using var connection = new SqlConnection(csb.ConnectionString);
        await connection.OpenAsync();
        csb.InitialCatalog = "MGZ";
        _csp.ConnectionString = csb.ConnectionString;
        using var db = await _dbfactory.CreateDbContextAsync();
        db.Database.EnsureCreated();
        IsConnected = true;
    }

    private bool CanConnect() =>
        !string.IsNullOrEmpty(SelectedDataSource) &&
        (AuthenticationMode == AuthenticationMode.Windows ||
        !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));

    private async Task LoadDataSources()
    {
        if (DataSources.Count > 0)
            return;

        IsLoading = true;
        try
        {
            var dataSources = await Task.Run(GetDataSources);
            foreach (var dataSource in dataSources)
            {
                DataSources.Add(dataSource);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static IReadOnlyCollection<string> GetDataSources()
    {
        var dataSources = new List<string>();
        var table = SqlDataSourceEnumerator.Instance.GetDataSources();

        foreach (DataRow row in table.Rows)
        {
            var dataSource = row.Field<string>("ServerName") ?? "Unknown";
            var instanceName = row.Field<string>("InstanceName");
            if (!string.IsNullOrEmpty(instanceName))
            {
                dataSource += "\\" + instanceName;
            }

            dataSources.Add(dataSource);
        }

        return dataSources;
    }
}

public enum AuthenticationMode
{
    Windows,
    SqlServer
}
