using Npgsql;
using BooksAPI.Models;

namespace BooksAPI.Endpoints;

public static class BooksEndpoints
{
    public static void MapBooksEndpoints(this WebApplication app, string connString)
    {
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
    }
}