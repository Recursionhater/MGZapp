using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace WpfApp1
{
    public class AccViewModel : ObservableObject
    {
        
        public AsyncRelayCommand Si { get;}
        public AsyncRelayCommand Rg { get; }
        private string login="";
        private string password="";
        private Reposit r;
        private string _password => new System.Net.NetworkCredential(string.Empty, Password).Password;
        //Registracion rg = new Registracion();
        public AccViewModel(Reposit rep)
        {
            Si = new AsyncRelayCommand(SiEx, CanSiEx);
            Rg = new AsyncRelayCommand(RgEx,CanSiEx);
            r = rep;
        }

        private async Task SiEx()
        {
            if (await r.SignUp(login, _password))
            {
                EditWin editWin = new EditWin();
                editWin.ShowDialog();
            }
            else MessageBox.Show("Неверный пароль");
        }

        public string Login
        {
            get => login;
            set
            {
                if (SetProperty(ref login, value))
                {
                    Si.NotifyCanExecuteChanged();
                    Rg.NotifyCanExecuteChanged();
                }
                }
        }

        public string Password
        {
            get => password;
            set
            {
                if (SetProperty(ref password, value))
                {
                    Si.NotifyCanExecuteChanged();
                    Rg.NotifyCanExecuteChanged();
                }
            }
        }
        public async Task RgEx()
        {
            await r.AddUser(login, _password);
        }
        public bool CanSiEx() {
            
            return !String.IsNullOrEmpty(Login)&& !String.IsNullOrEmpty(_password);
        }
    }
   
}
