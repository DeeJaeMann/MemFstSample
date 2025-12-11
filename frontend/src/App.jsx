import { useState } from 'react'
import { useLoaderData } from "react-router-dom";
import axios from "axios";
import UserLogin from "./components/UserLogin";
import './App.css'

function App() {
  const { books: initialBooks, users } = useLoaderData();
  const [books, setBooks] = useState(initialBooks);
  const [currentUser, setCurrentUser] = useState(null);

  const handleSetFavorite = async (bookId) => {
    if (!currentUser) return;

    try {
      await axios.patch(`http://localhost:5015/users/fav/${currentUser.id}`, {
        favorite_book_id: bookId,
      });

      setCurrentUser({
        ...currentUser,
        favorite_book_id: bookId,
      });
    } catch (error) {
      console.error("Error setting favorite book: ", error.message);
      alert("Could not set favorite book.");
    }
  }

  const handleAddUserToBook = async (bookId) => {
    if (!currentUser) return;

    try {
      await axios.post("http://localhost:5015/user_books", {
        book_id: bookId,
        user_id: currentUser.id,
      });

      // refresh books from backend
      const response = await axios.get("http://localhost:5015/books");
      setBooks(response.data);
    } catch (error) {
      console.error("Error adding user to book list: ", error.message);
      alert("Could not add user to book list");
    }
  }

  return (
    <>
      <div className="container mt-5">
        {!currentUser && (
            <UserLogin
                onLogin={(user) => setCurrentUser(user)}
                users={users}
            />
        )}
        {currentUser && (
          <div>
            <h1>Welcome, {currentUser.username.charAt(0).toUpperCase() + currentUser.username.slice(1)}</h1>
            <button className="btn btn-secondary mt-3" onClick={() => setCurrentUser(null)}>Logout</button>
          </div>
        )}
      <div>
        <section>
        <h2>Books</h2>
        <div>
          {books.map((book) => {
            // check if currentUser is in the list
            const isUserInBookList = currentUser && book.users.includes(currentUser.username);
            return (
                <article key={book.id}>
                  <h3><cite>{book.title}</cite></h3>
                  Author(s): {book.authors}<br/>
                  <button
                      className={!currentUser || currentUser?.favorite_book_id === book.id
                          ? "btn btn-sm btn-primary mt-2"
                          : "btn btn-sm btn-outline-primary mt-2"}
                      onClick={() => handleSetFavorite(book.id)}
                      disabled={!currentUser || currentUser?.favorite_book_id === book.id}
                  >
                    {currentUser?.favorite_book_id === book.id
                        ? "Favorite"
                        : "Set as Favorite"}
                  </button>
                  {currentUser && (
                      <button
                          className={isUserInBookList ? "btn btn-sm btn-success mt-2" : "btn btn-sm btn-outline-success mt-2"}
                          onClick={() => handleAddUserToBook(book.id)}
                          disabled={isUserInBookList}
                      >
                        {isUserInBookList ? "Already in List" : "Add Me to List"}
                      </button>
                  )}
                  <br/>
                  User(s):
                  <ul>
                    {book.users.map(user => (
                        <li key={user}>{user}</li>
                    ))}
                  </ul>

                </article>
            );
          })}
        </div>
        </section>
      </div>
      </div>
    </>
  )
}

export default App
