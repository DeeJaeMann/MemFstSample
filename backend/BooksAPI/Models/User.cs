namespace BooksAPI.Models;

public record User(
    int Id, 
    string Username, 
    int? FavoriteBookId);
