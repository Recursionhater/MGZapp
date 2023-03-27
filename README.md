Приложение для онлайн магазина
====================
Содержание 
--------------------
* Общее для обоих решений
1. Shared
    1. AppDbContext
    2. Category
    3. IConnectionStringProvider
    4. Product
    5. Reposit
    6. User
2. WpfControls
    1. ConnectControl
    2. ConnectViewModel
    3. CreateAccountControl
    4. CreateAccountViewModel
    5. LoginControl
    6. LoginViewModel
    
А также в каждом из приложений есть свои папки Properties в которых после входа ложиться Connection String
     

* Админская часть 
1. IoC и зависимости
2. ConectionStringProvider
3. EditControl
4. EditViewModel
5. MainWindow
6. MainWindowViewModel
* Клиентская часть
1. IoC и зависимости
2. ConectionStringProvider
3. MainWindow
4. MainWindowViewModel
5. ShoppingCartControl
6. ShoppingCartviewModel
7. ShoppingCartItem
---
# Общее
## 1.Shared
### 1.1 AppDbContext
Класс созданный для Entity в котором:

Присваевается IConnectionStringProvider 

         private readonly IConnectionStringProvider csp;

         public AppDbContext(IConnectionStringProvider csp)
        {
            this.csp = csp;
        }
Устанавливаются списки в дбсет 

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
Устанавливается строка подключения из csp 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
          {
                optionsBuilder.UseSqlServer(csp.ConnectionString);
          }
И ограничиваются столбцы sql 

       protected override void OnModelCreating(ModelBuilder modelBuilder)
               {
                   modelBuilder.Entity<Product>().Property(x => x.Name).HasMaxLength(200);
                   modelBuilder.Entity<User>().Property(x => x.Login).HasMaxLength(30);
                   modelBuilder.Entity<User>().Property(x => x.Password).HasMaxLength(30);
                   modelBuilder.Entity<User>().HasIndex(x => x.Login).IsUnique();
                   modelBuilder.Entity<Category>().Property(x => x.Name).HasMaxLength(200);
               }

### 1.2 Category
Наследуется от ObservableObject чтобы было проще отслеживать изменение свойств в mvvm паттерне

Класс сущности из таблицы в котором лежит id и name категории 
        
        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private string _name="";
Также изза ado, нужно сравнивать класс не по ссылкам а по id 

        // Метод сравнения ссылок категорий по ID
        public bool Equals(Category? other)
        {
            if (ReferenceEquals(null, other)) return false; //Сравнение ссылок по null ()
            if (ReferenceEquals(this, other)) return true; //Два обьекта ссылаются на одну область памяти (Один и тот же обьект)

            return Id == other.Id; // Проверка по ID
        }
---
          public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false; //Тоже самое что сверху только по obj
            if (ReferenceEquals(this, obj)) return true;

            return obj is Category category && Equals(category);
        }
                // HashCode обычно рекомендуеться реализовывать если переопределил Equals
        public override int GetHashCode() => Id;
---
         // Перегрузка обычно рекомендуеться реализовывать если переопределил Equals

        public static bool operator ==(Category? left, Category? right) => Equals(left, right);

        public static bool operator !=(Category? left, Category? right) => !Equals(left, right);

### 1.3 IConnectionStringProvider
### 1.4 Product
### 1.5 Reposit
### 1.6 User
## 1. IoC и зависимости
hfgdjfghj

---
# Админская часть 
## 1. IoC и зависимости
