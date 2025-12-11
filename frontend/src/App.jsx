import { useState } from 'react'
import { useLoaderData } from "react-router-dom";
import './App.css'

function App() {
  const { users, books } = useLoaderData();

  return (
    <>
      <div>
        <h1>Welcome</h1>

        <h2>Users</h2>
        <ul>
          {users.map(user => (
              <li key={user.id}>{user.username}</li>
          ))}
        </ul>

        <h2>Books</h2>
        <ul>
          {books.map(book => (
              <li key={book.id}>{book.title} by {book.authors}</li>
          ))}
        </ul>
      </div>
    </>
  )
}

export default App
