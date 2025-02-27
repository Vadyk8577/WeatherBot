using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Telegram.Bot.Types;
using WeatherHistory.Models;

namespace WeatherHistory
{
    public class DatabaseHelper
    {
        private static readonly string connectionString = "Server=DESKTOP-ORL793I\\SQLEXPRESS;Database=WeatherBotDB;Trusted_Connection=True;TrustServerCertificate=True";
        

        // Get User History
        public async Task<List<Models.WeatherHistory>> GetWeatherHistoryByUserAsync(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM WeatherHistory WHERE UserId = @UserId ORDER BY RequestedAt DESC";
                    return connection.Query<Models.WeatherHistory>(query, new { UserId = userId }).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
                return null;
            }
        }
        public static List<Models.User> GetUsers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Users";
                    return connection.Query<Models.User>(query).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
                return null;
            }
        }
        public static Models.User GetUserById(long userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Users WHERE UserId = @UserId";
                    return connection.Query<Models.User>(query, new { UserId = userId }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
                return null;
            }
        }
        public static void SaveUser(long userId, string firstName, string lastName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"IF NOT EXISTS (SELECT * FROM USERS WHERE UserId = @UserId)
                            INSERT INTO Users (UserId, FirstName, LastName) 
                            VALUES (@UserId, @FirstName, @LastName)";
                    connection.Query<Models.User>(query, new
                    {
                        UserId = userId,
                        FirstName = firstName,
                        LastName = lastName
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
        }
        public static void SaveWeather(long userId, string city, decimal temp,
                                                    decimal humid, decimal wind, string condition)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO WeatherHistory (UserId, City, Temperature, Humidity,WindSpeed,Condition)" +
                                                             " VALUES (@UserId, @City, @Temperature,@Humidity,@WindSpeed,@Condition)";
                    connection.Query<Models.WeatherHistory>(query, new
                    {
                        UserId = userId,
                        City = char.ToUpper(city[0]) + city.Substring(1).ToLower(),
                        Temperature = temp,
                        Humidity = humid,
                        WindSpeed = wind,
                        Condition = condition
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
        }
        public static Models.User UpdateUser(long userId,string firstName,string lastName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Users SET FirstName = @FirstName,LastName = @LastName WHERE UserId = @UserId";
                    connection.Query<Models.User>(query, new { UserId = userId, FirstName = firstName, LastName = lastName }).FirstOrDefault();
                    return GetUserById(userId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
                return null;
            }
        }
        public static void DeleteUser(long userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Users WHERE UserId = @UserId ";
                    connection.Query<Models.User>(query, new { UserId = userId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
            
        }
        public static List<Models.WeatherHistory> GetWeathers()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM WeatherHistory";
                    return connection.Query<Models.WeatherHistory>(query).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
                return null;
            }
            
        }
        public static Models.WeatherHistory GetWeatherById(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM WeatherHistory WHERE Id = @Id";
                    return connection.Query<Models.WeatherHistory>(query, new { Id = id }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
                return null;
            }
        }
        
        public static Models.WeatherHistory UpdateWeatherHistory(int id, long userId, string city, decimal temp,
                                            decimal humid, decimal wind, string condition)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE WeatherHistory SET UserId = @UserId, City = @City, Temperature = @Temperature, Humidity = @Humidity," +
                                                             "WindSpeed = @WindSpeed, Condition = @Condition Where Id = @Id";
                    connection.Query<Models.WeatherHistory>(query, new
                    {
                        Id = id,
                        UserId = userId,
                        City = char.ToUpper(city[0]) + city.Substring(1).ToLower(),
                        Temperature = temp,
                        Humidity = humid,
                        WindSpeed = wind,
                        Condition = condition
                    });
                    return GetWeatherById(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
                return null;
            }
            
        }
        public static void DeleteWeatherHistory(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM WeatherHistory WHERE Id = @Id";
                    connection.Query<Models.WeatherHistory>(query, new { Id = id });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
            }
        }
        public static List<Models.WeatherHistory> GetAllFromHistory(long userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM WeatherHistory WHERE UserId = @UserId";
                    return connection.Query<Models.WeatherHistory>(query, new { UserId = userId }).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database Error: {ex.Message}");
                return null;
            }
        }
    }
}