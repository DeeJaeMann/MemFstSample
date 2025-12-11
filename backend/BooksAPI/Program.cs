using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Npgsql;
using BooksAPI.Models;

var config = new ConfigurationBuilder()
    // Non-sensitive settings
    .AddJsonFile("appsettings.json")
    // Connection string stored in secrets
    .AddUserSecrets<Program>()
    .Build();

string? connString = config.GetConnectionString("DefaultConnection");

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Gets all users from DB
app.MapGet("/users", async () =>
{
    List<User> users = new();
    await using var conn = new NpgsqlConnection(connString);
    await conn.OpenAsync();

    const string query = "SELECT * FROM users";
    await using var command = new NpgsqlCommand(query, conn);
    await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        users.Add(new User(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetInt32(2)
        ));
    }

    return Results.Ok(users);
}).WithName("GetAllUsers");

// Gets user by Username
app.MapGet("/users/username/{username}", async (string username) => 
{
    await using var conn = new NpgsqlConnection(connString);
    await conn.OpenAsync();

    const string query = "SELECT * FROM users WHERE username = @username";
    await using var command = new NpgsqlCommand(query, conn);
    command.Parameters.AddWithValue("username", username);

    await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

    if (await reader.ReadAsync())
    {
        var user = new User(
        reader.GetInt32(0),
        reader.GetString(1),
        reader.IsDBNull(2) ? null : reader.GetInt32(2)
        );
        return Results.Ok(user);
    }

    return Results.NotFound();
}
).WithName("GetUserByName");

// Gets user by id
app.MapGet("/users/id/{id}", async (int id) => 
{
    await using var conn = new NpgsqlConnection(connString);
    await conn.OpenAsync();

    const string query = "SELECT * FROM users WHERE id = @id";
        await using var command = new NpgsqlCommand(query, conn);
        command.Parameters.AddWithValue("id", id);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var user = new User(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.IsDBNull(2) ? null : reader.GetInt32(2)
            );
            return Results.Ok(user);
        }
        
        return Results.NotFound();
        }
    ).WithName("GetUserById");

// Get all books
app.MapGet("/books", async () =>
{
    List<Book> books = new();
    await using var conn = new NpgsqlConnection(connString);
    await conn.OpenAsync();

    const string query = @"
        SELECT
            b.id,
            b.title,
            b.authors,
            COALESCE(array_agg(u.username ORDER BY u.username), '{}') AS usernames
        FROM books b 
        LEFT JOIN user_books ub ON ub.book_id = b.id
        LEFT JOIN users u ON u.id = ub.user_id
        GROUP BY b.id, b.title, b.authors;
    ";
    
    await using var command = new NpgsqlCommand(query, conn);
    await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        books.Add(new Book(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetFieldValue<string[]>(3).ToList()
            ));
    }
    
    return Results.Ok(books);
}).WithName("GetAllBooks");

// Set favorite book by id
app.MapPatch("/users/fav/{id}", async (int id, FavoriteBookUpdate update) =>
{
    await using var conn = new NpgsqlConnection(connString);
    await conn.OpenAsync();

    const string query = @"
        UPDATE users
        SET favorite_book_id =  @favorite_book_id
        WHERE id = @id;";
    await using var command = new NpgsqlCommand(query, conn);
    command.Parameters.AddWithValue("id", id);
    command.Parameters.AddWithValue("favorite_book_id", update.favorite_book_id);
    
    int rowsAffected = await command.ExecuteNonQueryAsync();

    if (rowsAffected is 0) return Results.NotFound();
    return Results.Ok();
}).WithName("UpdateUserFavoriteBook");

// Add a user to a book list
app.MapPost("/user_books", async (UserBooksUpdate update) =>
{
    await using var conn = new NpgsqlConnection(connString);
    await conn.OpenAsync();

    const string query = @"
            INSERT INTO user_books (user_id, book_id)
            VALUES (@user_id, @book_id);";
    await using var command = new NpgsqlCommand(query, conn);
    command.Parameters.AddWithValue("user_id", update.user_id);
    command.Parameters.AddWithValue("book_id", update.book_id);

    int rowsAffected = await command.ExecuteNonQueryAsync();

    if (rowsAffected is 0) return Results.BadRequest("Could not add user to book list.");
    return Results.Created($"/user_books/{update.user_id}-{update.book_id}", update);
}).WithName("AddUserToBookList");

app.Run();





