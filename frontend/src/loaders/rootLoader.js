// loaders/rootLoader.js
import axios from "axios";

async function rootLoader() {
    try {
        const [ booksResponse, usersResponse ] = await Promise.all([
            axios.get("http://localhost:5015/books"),
            axios.get("http://localhost:5015/users"),
        ]);

        return {
            books: booksResponse.data,
            users: usersResponse.data,
        };
    } catch (error) {
        console.error(error.message);
        return{
            books: [],
            users: [],
        };
    }

}

export default rootLoader;