using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public partial class Category:ObservableObject,IEquatable<Category>
    {
        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private string _name="";

        // Iequatble - микро оптимизация , Equals - Сравнивать сущность по id , а не по адрессу в памяти
        // Метод сравнения ссылок категорий по ID
        public bool Equals(Category? other)
        {
            if (ReferenceEquals(null, other)) return false; //Сравнение ссылок по null ()
            if (ReferenceEquals(this, other)) return true; //Два обьекта ссылаются на одну область памяти (Один и тот же обьект)

            return Id == other.Id; // Проверка по ID
        }
        
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false; //Тоже самое что сверху только по obj
            if (ReferenceEquals(this, obj)) return true;

            return obj is Category category && Equals(category);
        }
        // HashCode обычно рекомендуеться реализовывать если переопределил Equals
        public override int GetHashCode() => Id;

        // Перегрузка обычно рекомендуеться реализовывать если переопределил Equals

        public static bool operator ==(Category? left, Category? right) => Equals(left, right);

        public static bool operator !=(Category? left, Category? right) => !Equals(left, right);

    }
}
