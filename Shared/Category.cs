using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public partial class Category:ObservableObject
    {
        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private string _name="";
        //public int Id { get; set; }
        //public string Name { get; set; } = "";


    }
}
