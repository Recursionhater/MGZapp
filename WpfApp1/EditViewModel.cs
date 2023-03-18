using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{

    internal class EditViewModel : ObservableObject
    {
        public AsyncRelayCommand WinLoad { get; }
        private readonly Reposit r;
        public ObservableCollection<Product> Products { get; }
        public EditViewModel(Reposit rep) {
            Products = new ObservableCollection<Product>();
            Products.CollectionChanged += Products_CollectionChanged;
            WinLoad = new AsyncRelayCommand(OnWinLoad);
            
            r = rep;
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

       
        private void I_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _=r.SaveProduct((Product)sender!);
        }

        private async Task OnWinLoad() {
            foreach (var item in await r.GetProducts()) {
                Products.Add(item);
            }
        }
        



    }
}
