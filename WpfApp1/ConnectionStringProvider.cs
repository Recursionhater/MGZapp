using AdminApp.Properties;
using Shared;

namespace AdminApp
{
    internal class ConnectionStringProvider : IConnectionStringProvider
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
}
