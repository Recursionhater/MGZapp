using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class Reposit
    {
        private string _strconnect;
        public Reposit(string strconnect)
        {
            _strconnect = strconnect;
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
        public async Task AddProduct(Product p) {
            await Execute(async command =>
            {
                command.CommandText = "INSERT INTO Products (Name,Price,Description) output INSERTED.ID values (@Name,@Price,@Description)";
                command.Parameters.AddWithValue("Name", p.Name);
                command.Parameters.AddWithValue("Price", p.Price);
                command.Parameters.AddWithValue("Description", p.Description);
                p.Id = (int)(await command.ExecuteScalarAsync())!;
                return 0;
            });
        }
        public async Task SaveProduct(Product p) {
            await Execute(async command =>
            {
                command.CommandText = "Update Products set Name=@Name,Price=@Price,Description = @Description Where Id=@Id";
                command.Parameters.AddWithValue("Name", p.Name);
                command.Parameters.AddWithValue("Price", p.Price);
                command.Parameters.AddWithValue("Description", p.Description);
                command.Parameters.AddWithValue("Id", p.Id);
                await command.ExecuteNonQueryAsync();
                return 0;
            });
        }
        public async Task<IReadOnlyCollection<Product>> GetProducts() {
            return await Execute(async command =>
            {
                var products = new List<Product>();
                command.CommandText = "SELECT Id,Name,Price,Description From Products ";
                using var reader = await command.ExecuteReaderAsync();
                while (reader.Read()) {
                    products.Add(new Product {
                       Id=reader.GetInt32(reader.GetOrdinal("Id")),
                      Name = reader.GetString(reader.GetOrdinal("Name")),
                       Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                        Description = reader.GetString(reader.GetOrdinal("Description"))
                    });
                }
                return products;
            });
        }
        public async Task DeleteProduct(int id) {
            await Execute(async command =>
            {
                command.CommandText = "delete from Products where Id=@id";
                command.Parameters.AddWithValue("id", id);
                await command.ExecuteNonQueryAsync();
                return 1;
            });
        }

        private async Task<T> Execute<T>(Func<SqlCommand, Task<T>> action)
        {
           using SqlConnection connection = new SqlConnection(_strconnect);
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            return await action(command);
        }

    }
}
