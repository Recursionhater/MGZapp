using AdminApp;
using CommunityToolkit.Mvvm.DependencyInjection;
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
using WpfApp2;

namespace WpfApp1
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

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddDbContextFactory<AppDbContext>();
            services.AddSingleton<Reposit>();
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<EditViewModel>();
            services.AddTransient<CreateAccountViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddTransient<ConnectViewModel>();
            services.AddSingleton<IConnectionStringProvider,ConnectionStringProvider>();

            return services.BuildServiceProvider();
        }
        private void ShowError(string text, string title) { 
        MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }


    }
}
