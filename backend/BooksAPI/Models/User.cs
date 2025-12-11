namespace BooksAPI.Models;

public record User(
    int id, 
    string username, 
    int? favorite_book_id);
