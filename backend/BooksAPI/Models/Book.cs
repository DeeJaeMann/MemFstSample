namespace BooksAPI.Models;

public record Book(
    int Id, 
    string Title, 
    string Authors, 
    List<string> Users);