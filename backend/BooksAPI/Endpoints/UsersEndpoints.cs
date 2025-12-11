using Npgsql;
using BooksAPI.Models;

namespace BooksAPI.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this WebApplication app, string connString)
    {
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
        
        // Add new user by name
        app.MapPost("/users/new", async (NewUser user) =>
        {
            await using var conn = new NpgsqlConnection(connString);
            await conn.OpenAsync();
            
            const string query = @"INSERT INTO users (username) VALUES (@username) RETURNING id;";
            await using var command = new NpgsqlCommand(query, conn);
            command.Parameters.AddWithValue("username", user.username);
            var newId = (int)await command.ExecuteScalarAsync();

            var createdUser = new User(Id: newId, Username: user.username, FavoriteBookId: null);
            
            return Results.Created($"/users/{newId}", createdUser);
        }).WithName("CreateUser");
    }
}