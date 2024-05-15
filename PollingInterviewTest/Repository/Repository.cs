using Microsoft.Data.Sqlite;
using PollingInterviewTest.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PollingInterviewTest.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        public Repository()
        {
            SQLitePCL.Batteries.Init();
            using (var connection = new SqliteConnection("Data Source=PollingInterviewTest.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                var fields = typeof(T).GetProperties().Select(x => x.Name);



                var createTableCommandText = $"CREATE TABLE IF NOT EXISTS {typeof(T).Name} ({string.Join(",", fields)})";
                command.CommandText = createTableCommandText;
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public async Task Insert(T data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using (var connection = new SqliteConnection("Data Source=PollingInterviewTest.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();

                var fields = typeof(T).GetProperties().Select(x => x.Name);
                

                var values = typeof(T).GetProperties().Select(x => x.GetValue(data));
                var insertCommand = connection.CreateCommand();
                var insertCommandText = $"INSERT INTO {typeof(T).Name} ({string.Join(",", fields)}) VALUES ('{string.Join("','", values)}')";
                insertCommand.CommandText = insertCommandText;
                await insertCommand.ExecuteNonQueryAsync();

                connection.Close();
            }
        }
        public async Task<List<T>> GetAll()
        {
            using (var connection = new SqliteConnection("Data Source=PollingInterviewTest.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {typeof(T).Name}";
                var reader = await command.ExecuteReaderAsync();
                var dataList = new List<T>();
                while (await reader.ReadAsync())
                {
                    var data = Activator.CreateInstance<T>();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if (property.PropertyType == typeof(DateTime))
                        {
                            property.SetValue(data, DateTime.Parse(reader[property.Name].ToString()));
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            property.SetValue(data, reader[property.Name].ToString() == "1");
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(data, int.Parse(reader[property.Name].ToString()));
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            property.SetValue(data, double.Parse(reader[property.Name].ToString()));
                        }
                        else
                        {
                            property.SetValue(data, reader[property.Name]);
                        }
                        //property.SetValue(data, reader[property.Name]);
                    }
                    dataList.Add(data);
                }
                connection.Close();
                return dataList;
            }
        }

        public async Task<List<T>> GetAll(Func<T, bool> expression)
        {
            using (var connection = new SqliteConnection("Data Source=PollingInterviewTest.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {typeof(T).Name}";
                var reader = await command.ExecuteReaderAsync();
                var dataList = new List<T>();
                while (reader.Read())
                {
                    var data = Activator.CreateInstance<T>();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        if(property.PropertyType == typeof(DateTime))
                        {
                            property.SetValue(data, DateTime.Parse(reader[property.Name].ToString()));
                        }
                        else if(property.PropertyType == typeof(bool))
                        {
                            property.SetValue(data, reader[property.Name].ToString() == "1");
                        }
                        else if(property.PropertyType == typeof(int))
                        {
                            property.SetValue(data, int.Parse(reader[property.Name].ToString()));
                        }
                        else if(property.PropertyType == typeof(double))
                        {
                            property.SetValue(data, double.Parse(reader[property.Name].ToString()));
                        }
                        else
                        {
                            property.SetValue(data, reader[property.Name]);
                        }
                        //property.SetValue(data, reader[property.Name]);
                    }
                    if (expression(data))
                    {
                        dataList.Add(data);
                    }
                }
                connection.Close();
                return dataList;
            }



        }

        public async Task<T?> GetById(int id)
        {
            using (var connection = new SqliteConnection("Data Source=PollingInterviewTest.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {typeof(T).Name} WHERE Id = {id}";
                var reader = await command.ExecuteReaderAsync();
                if (reader.Read())
                {
                    var data = Activator.CreateInstance<T>();
                    foreach (var property in typeof(T).GetProperties())
                    {
                        property.SetValue(data, reader[property.Name]);
                    }
                    connection.Close();
                    return data;
                }
                connection.Close();
                return null;
            }
        }



    }
}
