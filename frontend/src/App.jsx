import { useState } from 'react'
import { useLoaderData } from "react-router-dom";
import './App.css'

function App() {
  const { users, books } = useLoaderData();

  return (
    <>
      <div>
        <h1>Welcome</h1>

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
    </>
  )
}

export default App
