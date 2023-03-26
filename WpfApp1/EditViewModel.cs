using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfApp2;

namespace WpfApp1
{

    internal class EditViewModel : ObservableObject
    {
        public AsyncRelayCommand WinLoad { get; }
        private readonly Reposit r;
        public ObservableCollection<Product> Products { get; }
        public ObservableCollection<Category> Categories { get; }
        public RelayCommand<AddingNewItemEventArgs> AddNewRowCommand { get; }
        public RelayCommand<object> SelectImageCommand { get; }
        public LoginViewModel logvm { get; }
        public EditViewModel(Reposit rep, LoginViewModel loginViewModel) {
            logvm=loginViewModel;
            logvm.PropertyChanged += LoginViewModelOnPropertyChanged;
            Products = new ObservableCollection<Product>();
            Products.CollectionChanged += Products_CollectionChanged;
            WinLoad = new AsyncRelayCommand(OnWinLoad);
            Categories = new ObservableCollection<Category>();
            Categories.CollectionChanged += Categories_CollectionChanged;
            r = rep;
            AddNewRowCommand = new RelayCommand<AddingNewItemEventArgs>(AddNewRow);
            SelectImageCommand = new RelayCommand<object>(SelectImage);
        }

        private void SelectImage(object? value)
        {
            if (value is not Product product)
                return;

            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                product.Image = File.ReadAllBytes(openFileDialog.FileName);
            }
        }

        private void LoginViewModelOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(logvm.IsLoggedIn))
                return;

            if (logvm.IsLoggedIn)
            {
               _= OnWinLoad();
            }

        }

        private void Categories_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Category i in e.NewItems)
                {
                    if (i.Id == 0) {
                        _ = r.AddCategory(i);
                    }

                    i.PropertyChanged += С_PropertyChanged;
                    

                }
            }
            if (e.OldItems != null)
            {
                foreach (Category i in e.OldItems)
                {
                    i.PropertyChanged -= С_PropertyChanged;
                    for (var j=Products.Count-1; j>=0;j--) 
                    {
                        var item = Products[j];
                        if (item.Category.Id == i.Id) {
                        Products.Remove(item);
                        }
                    }
                    _ = r.DeleteCategory(i.Id);
                }
            }
        }

        private void С_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _ = r.SaveCategory((Category)sender!); 
        }

        private void Products_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null) {
                foreach (Product i in e.NewItems) {
                    if(i.Id==0) _=r.AddProduct(i);
                    
                    i.PropertyChanged += I_PropertyChanged;


                }
            }
            if (e.OldItems != null)
            {
                foreach (Product i in e.OldItems)
                {
                    i.PropertyChanged -= I_PropertyChanged;
                    _=r.DeleteProduct(i.Id);
                    
                }
            }
        }
        private void AddNewRow(AddingNewItemEventArgs? e) {
            if (e == null) return;
            e.NewItem = new Product()
            {
                Name = string.Empty,
                Category = Categories[1],
            };

        }


        private void I_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _=r.SaveProduct((Product)sender!);
        }

        private async Task OnWinLoad() {
            foreach (var item in await r.GetProducts()) 
            {
                Products.Add(item);
            }
            foreach (var item in await r.GetCategories())
            {
                Categories.Add(item);
            }
        }

    }
}
