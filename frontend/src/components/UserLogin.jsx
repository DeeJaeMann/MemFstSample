import { useState } from "react";
import axios from "axios";

function UserLogin({ onLogin, users }) {
    const [username, setUsername] = useState("");

    const handleSubmit = async (event) => {
        event.preventDefault();

        if (!username.trim()) return;

        // Check if the user exists
        const existingUser = users.find((user) => user.username === username);
        if (existingUser)
        {
            // user exists, assign fields
            onLogin({
                id: existingUser.id,
                username: existingUser.username,
                favorite_book_id: existingUser.favorite_book_id ?? null,
            });
            return;
        }

        // user does not exist, create a new one
        try {
            const response = await axios.post("http://localhost:5015/users/new", {
                username,
            });

            // Transform API response
            const apiUser = response.data;
            const createdUser = {
                id: apiUser.id,
                username: apiUser.username,
                favorite_book_id: apiUser.favorite_book_id ?? null,
            };

            onLogin(createdUser);
        } catch (error) {
            console.error(error.message);
            alert("Error creating user");
        }

    };

    return (
        <div className="popup-overlay">
            <div className="popup-content">
                <h2>Login</h2>
                <form onSubmit={handleSubmit}>
                    <div className="mb-3">
                        <label>Username</label>
                        <input
                            type="text"
                            className="form-control"
                            value={username}
                            onChange={(event) => setUsername(event.target.value.toLowerCase())}
                            placeholder="Enter username"
                            />
                    </div>
                    <button type="submit" className="btn btn-primary">Login</button>
                </form>
            </div>
        </div>
    );
}

export default UserLogin;