# MemFstSample
Code Sample For Process

# Situation
A client has asked for a new feature that allows a user to select their favorite book from a pre-selected list of books. The application should allow the user to see the list of people who chose each book, and a total count, as well as the ability to add their name to a specific book.

# Minimum Requirements
 - Application is built in .Net
 - Application uses a database
 - Application has a min. Of 10 books in the predefined list. 
 - Users do not require authentication
 - Users can add their name to a list, but cannot delete. 
 - Code should be in GitHub for viewing

# Additional Clarifications

# Design Choices
 - Predefined list generated from .json
  - Helper project to populate PostgreSQL
 - Book Table
  - ID
  - Title
  - Author(s)
  - UserList (Favorite) - Add but not delete - foreign key
 - User Table
  - ID
  - UserName
  - FavoriteBookId - foreign key
 - Favorite Indication

 # Database Notes
  - When setting up PostgreSQL for ASP.NET, ensure host type connection methods are set to md5 in pg_hba.conf - in the event they are set to peer by default
  - Default connection string has been stored in secrets