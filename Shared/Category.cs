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
        public bool Equals(Category? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id == other.Id;
        }
        
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return obj is Category category && Equals(category);
        }

        public override int GetHashCode() => Id;

        public static bool operator ==(Category? left, Category? right) => Equals(left, right);

        public static bool operator !=(Category? left, Category? right) => !Equals(left, right);

    }
}
