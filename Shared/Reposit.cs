using Microsoft.Data.SqlClient;
using Shared;

namespace WpfApp1
{
    public class Reposit
    {
        private readonly IConnectionStringProvider _cstp;

        public Reposit(IConnectionStringProvider cstp)
        {
            _cstp = cstp;
        }

        public async Task AddUser(string login, string password)
        {
            await Execute(async command =>
           {
               command.CommandText = "insert into Users(Login,Password) values(@Login, @Password)";
               command.Parameters.AddWithValue("Login", login);
               command.Parameters.AddWithValue("Password", password);
               await command.ExecuteNonQueryAsync();
               return 1;

           });

        }
        public async Task<bool> SignUp(string login, string password)
        {
            return await Execute(async command =>
            {
                command.CommandText = "SELECT 1 FROM Users WHERE login = @Login AND password = @Password COLLATE Latin1_General_CS_AS";
                command.Parameters.AddWithValue("Login", login);
                command.Parameters.AddWithValue("Password", password);
                return (int?)await command.ExecuteScalarAsync() == 1;
            });


        }
        public async Task AddProduct(Product p)
        {
            await Execute(async command =>
            {
                command.CommandText = "INSERT INTO Products (Name,Price,Description,CategoryId) output INSERTED.ID values (@Name,@Price,@Description,@CategoryId)";
                command.Parameters.AddWithValue("Name", p.Name);
                command.Parameters.AddWithValue("Price", p.Price);
                command.Parameters.AddWithValue("Description", p.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("CategoryId", p.Category.Id);
                p.Id = (int)(await command.ExecuteScalarAsync())!;
                return 0;
            });
        }
        public async Task SaveProduct(Product p)
        {
            await Execute(async command =>
            {
                command.CommandText = "Update Products set Name=@Name,Price=@Price,Description = @Description,CategoryId = @CategoryId Where Id=@Id";
                command.Parameters.AddWithValue("Name", p.Name);
                command.Parameters.AddWithValue("Price", p.Price);
                command.Parameters.AddWithValue("Description", p.Description ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("Id", p.Id);
                command.Parameters.AddWithValue("CategoryId", p.Category.Id);
                await command.ExecuteNonQueryAsync();
                return 0;
            });
        }
        public async Task<IReadOnlyCollection<Product>> GetProducts()
        {
            return await Execute(async command =>
            {
                var categories = await GetCategories();
                var categoriesById = categories.ToDictionary(c=>c.Id);
                var products = new List<Product>();
                command.CommandText = "SELECT Id,Name,Price,Description,CategoryId From Products ";
                using var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                        Category = categoriesById[reader.GetInt32(reader.GetOrdinal("CategoryId"))]
                    }) ;
                }
                return products;
            });
        }
        public async Task DeleteProduct(int id)
        {
            await Execute(async command =>
            {
                command.CommandText = "delete from Products where Id=@id";
                command.Parameters.AddWithValue("id", id);
                await command.ExecuteNonQueryAsync();
                return 1;
            });
        }
        public async Task<IReadOnlyCollection<Category>> GetCategories()
        {
            return await Execute(async command =>
            {
                var categories = new List<Category>();
                command.CommandText = "SELECT Id,Name From Categories ";
                using var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name"))

                    });
                }
                return categories;
            });
        }
        private async Task<T> Execute<T>(Func<SqlCommand, Task<T>> action)
        {
            using SqlConnection connection = new(_cstp.ConnectionString);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            return await action(command);
        }

        public async Task AddCategory(Category c)
        {
            await Execute(async command =>
            {
                command.CommandText = "INSERT INTO Categories (Name) output INSERTED.ID values (@Name)";
                command.Parameters.AddWithValue("Name", c.Name);
                c.Id = (int)(await command.ExecuteScalarAsync())!;
                return 0;
            });
        }

        public async Task DeleteCategory(int id)
        {
            await Execute(async command =>
            {
                command.CommandText = "delete from Categories where Id=@id";
                command.Parameters.AddWithValue("id", id);
                await command.ExecuteNonQueryAsync();
                return 1;
            });
        }

        public async Task SaveCategory(Category c)
        {
            await Execute(async command =>
            {
                command.CommandText = "Update Categories set Name=@Name Where Id=@Id";
                command.Parameters.AddWithValue("Name", c.Name);
                command.Parameters.AddWithValue("Id", c.Id);
                await command.ExecuteNonQueryAsync();
                return 0;
            });
        }
    }
}
