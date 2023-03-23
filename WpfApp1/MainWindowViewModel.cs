using CommunityToolkit.Mvvm.ComponentModel;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1;
using WpfApp2;

namespace AdminApp
{
    internal class MainWindowViewModel:ObservableObject
    {
        private int _transitionIndex;
        public LoginViewModel LoginViewModel { get; }
        public CreateAccountViewModel CreateAccountViewModel { get; }
        public ConnectViewModel ConnectViewModel { get; }
        public int TransitionIndex
        {
            get => _transitionIndex;
            private set => SetProperty(ref _transitionIndex, value);
        }
        public MainWindowViewModel(LoginViewModel loginViewModel, CreateAccountViewModel createAccountViewModel, ConnectViewModel connectViewModel,IConnectionStringProvider csp)
        {
            LoginViewModel = loginViewModel;
            CreateAccountViewModel = createAccountViewModel;
            ConnectViewModel = connectViewModel;
            _transitionIndex = string.IsNullOrEmpty(csp.ConnectionString) ? 0 : 1;
            ConnectViewModel.PropertyChanged += ConnectViewModel_PropertyChanged;
            LoginViewModel.PropertyChanged += LoginViewModelOnPropertyChanged;
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
                TransitionIndex = 3;
            }

        }

    }
}
