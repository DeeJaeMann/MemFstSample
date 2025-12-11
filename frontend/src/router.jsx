// router.jsx
import { createBrowserRouter } from "react-router-dom";
import App from "./App";
import { rootLoader } from "./loaders/rootLoader.js";

const router = createBrowserRouter([
    {
        path: "/",
        element: <App />,
        loader: rootLoader,
    },
]);

export default router;