// loaders/rootLoader.js
import axios from "axios";

export async function rootLoader() {
    const [userResponse, booksResponse] = await Promise.all([
        axios.get("http://localhost:5015/users"),
        axios.get("http://localhost:5015/books"),
    ]);

    return {
        users: userResponse.data,
        books: booksResponse.data,
    };
}