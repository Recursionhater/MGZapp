using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WpfApp1;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;
            Application.Current.Dispatcher.UnhandledExceptionFilter += OnFilterDispatcherException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            Ioc.Default.ConfigureServices(ConfigureServices());
            
        }
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddDbContextFactory<AppDbContext>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<CreateAccountViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<ConnectViewModel>();
            services.AddSingleton<IConnectionStringProvider,ConnectionStringProvider>();
            services.AddTransient<ShoppingCartViewModel>();

            return services.BuildServiceProvider();
        }

        private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            ShowError(e.Exception.ToString(), "OnUnobservedTaskException");
        }

        private void OnFilterDispatcherException(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
        {

            ShowError(e.Exception.ToString(), "OnFilterDispatcherException");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ShowError(e.Exception.ToString(), "OnDispatcherUnhandledException");
        }

        private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowError(((Exception)e.ExceptionObject).ToString(), "OnAppDomainUnhandledException");
        }


        private void ShowError(string text, string title)
        {
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
