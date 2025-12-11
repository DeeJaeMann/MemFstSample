import { useState } from 'react'
import { useLoaderData } from "react-router-dom";
import UserLogin from "./components/UserLogin";
import './App.css'

function App() {
  const { books, users } = useLoaderData();
  const [currentUser, setCurrentUser] = useState(null);

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
          {books.map(book => (
              <article key={book.id}>
                <h3><cite>{book.title}</cite></h3>
                Author(s): {book.authors}<br />
                User(s):
                <ul>
                  {book.users.map(user => (
                      <li key={user}>{user}</li>
                  ))}
                </ul>
              </article>
          ))}
        </div>
        </section>
      </div>
      </div>
    </>
  )
}

export default App
