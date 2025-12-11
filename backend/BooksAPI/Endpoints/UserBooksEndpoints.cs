using Npgsql;
using BooksAPI.Models;

namespace BooksAPI.Endpoints;

public static class UserBooksEndpoints
{
    public static void MapUserBooksEndpoints(this WebApplication app, string connString)
    {
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
    }
}