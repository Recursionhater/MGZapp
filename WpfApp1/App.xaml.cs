using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

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
            this.InitializeComponent();
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
            services.AddSingleton( new Reposit("Trust Server Certificate=true; Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=MGZ;Data Source=DESKTOP-32P70LE\\SQLEXPRESS"));
            services.AddTransient<AccViewModel>();
            services.AddTransient<EditViewModel>();

            return services.BuildServiceProvider();
        }
        private void ShowError(string text, string title) { 
        MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }


    }
}
