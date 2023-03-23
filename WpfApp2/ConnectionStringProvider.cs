using ClientApp.Properties;
using Shared;

namespace WpfApp2;

internal sealed class ConnectionStringProvider:IConnectionStringProvider
{
    public string ConnectionString
    {
        get => Settings.Default.ConnectionString;
        set
        {
            Settings.Default.ConnectionString = value;
            Settings.Default.Save();
        }
    }
}