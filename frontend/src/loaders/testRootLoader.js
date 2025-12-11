// testRootLoader.js
import rootLoader from "./rootLoader.js";

(async () => {
    try {
        const data = await rootLoader();
        console.log("Books:", data.books);
        console.log("Users:", data.users);
    } catch (error) {
        console.error("Loader failed:", error.message);
    }
})();