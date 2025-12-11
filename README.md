# MemFstSample


## Situation
A client has asked for a new feature that allows a user to select their favorite book from a pre-selected list of books. The application should allow the user to see the list of people who chose each book, and a total count, as well as the ability to add their name to a specific book.

## Minimum Requirements
 - Application is built in .Net
 - Application uses a database
 - Application has a min. Of 10 books in the predefined list. 
 - Users do not require authentication
 - Users can add their name to a list, but cannot delete. 
 - Code should be in GitHub for viewing

## Design Choices
 - Predefined list generated from .json
  - Helper project to populate PostgreSQL
 - books Table
  - id
  - title
  - authors
 - users Table
  - id
  - username
  - favorite_book_id - foreign key -> books.id
 - user_books table - users -> many to one -> books
  - user_id - foreign key -> users.id
  - book_id - foreign key -> books.id
 - front-end - React
 - back-end - ASP.NET Minimal API
 - DB - PostgreSQL

 ## Database/Backend Notes
  - When setting up PostgreSQL for ASP.NET, ensure host type connection methods are set to md5 in pg_hba.conf - in the event they are set to peer by default
  - Ensure self-signed development certificate is registered and trusted
  - Default connection string has been stored in secrets
   - Create database in PostgreSQL
   - Initialize secrets with: dotnet user-secrets init
   - Add secret: dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=dbname;Username=username;Password=myPassword"
    - Be sure to populate values with correct settings!
   - Seed Database (dotnet run from inside DataSeeder project directory)
   - Run backend api (dotnet run from inside BooksAPI project directory)
   - Frontend uses http requests to backend