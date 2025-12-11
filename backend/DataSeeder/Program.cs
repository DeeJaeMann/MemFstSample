using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Npgsql;

var config = new ConfigurationBuilder()
    // Non-sensitive settings
    .AddJsonFile("appsettings.json")
    // Connection string stored in secrets
    .AddUserSecrets<Program>()
    .Build();

string? connString = config.GetConnectionString("DefaultConnection");

// Populate books and users variables from json
string json = File.ReadAllText("Data/books.json");
var books = JsonSerializer.Deserialize<List<Book>>(json);

json = File.ReadAllText("Data/users.json");
var users = JsonSerializer.Deserialize<List<User>>(json);

try
{
    using NpgsqlConnection conn  = new(connString);
    conn.Open();
    Debug.WriteLine("Connection successful!");
    
    // Drop books table and create a new table for seeding
    using (var command = new NpgsqlCommand(@"
        DROP TABLE IF EXISTS books CASCADE;
        CREATE TABLE books (
            id INT PRIMARY KEY,
            title TEXT NOT NULL,
            authors TEXT NOT NULL);", conn))
    {
        command.ExecuteNonQuery();
    }
    // Populate the books table
    foreach (Book book in books)
    {
        using var command = new NpgsqlCommand(
            "INSERT INTO books (id, title, authors) VALUES (@id, @title, @authors)", conn);
        command.Parameters.AddWithValue("id", book.id);
        command.Parameters.AddWithValue("title", book.title);
        command.Parameters.AddWithValue("authors", book.authors);
        command.ExecuteNonQuery();
    }
    
    // Drop users table and create a new table for seeding
    using (var command = new NpgsqlCommand(@"
        DROP TABLE IF EXISTS users CASCADE;
        CREATE TABLE users (
            id SERIAL PRIMARY KEY,
            username VARCHAR(30) UNIQUE NOT NULL,
            favorite_book_id INT REFERENCES books(id));", conn))
    {
        command.ExecuteNonQuery();
    }
    // Populate the users table
    foreach (User user in users)
    {
        using var command = new NpgsqlCommand(
            "INSERT INTO users (id, username, favorite_book_id) VALUES (@id, @username, @favorite_book_id)", conn);
        command.Parameters.AddWithValue("id", user.id);
        command.Parameters.AddWithValue("username", user.username);
        command.Parameters.AddWithValue("favorite_book_id", user.favorite_book_id);
        command.ExecuteNonQuery();
    }
    
    // Update the serial number in postgres
    using (var command = new NpgsqlCommand("SELECT setval('users_id_seq', (SELECT MAX(id) FROM users));", conn))
        command.ExecuteNonQuery();
    
    // Drop users_books table and create a new one
    // This table is for the relationships between users and book ids selected
    using (var command = new NpgsqlCommand(@"
        DROP TABLE IF EXISTS user_books;
        CREATE TABLE user_books (
            user_id INT REFERENCES users(id),
            book_id INT REFERENCES books(id),
            PRIMARY KEY (user_id, book_id));", conn))
    {
        command.ExecuteNonQuery();
    }
    
    // Populate user_books table
    foreach (Book book in books)
    {
        foreach (int id in book.user_ids)
        {
            using var command = new NpgsqlCommand(
                "INSERT INTO user_books (user_id, book_id) VALUES (@user_id, @book_id)", conn);
            command.Parameters.AddWithValue("user_id", id);
            command.Parameters.AddWithValue("book_id", book.id);
            command.ExecuteNonQuery();
        }
    }
    
}
catch (Exception ex)
{
    Debug.WriteLine($"Connection failed: {ex.Message}");
    throw;
}

public class Book
{
    public int id { get; set; }
    public string title { get; set; }
    public string authors { get; set; }
    public int[] user_ids { get; set; }
}

public class User
{
    public int id { get; set; }
    public string username { get; set; }
    public int favorite_book_id { get; set; }
}