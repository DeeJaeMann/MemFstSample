namespace BooksAPI.Models;

public record UserBooksUpdate(
    int user_id, 
    int book_id);