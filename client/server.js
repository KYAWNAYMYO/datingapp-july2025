const express = require('express');
const path = require('path');
const app = express();

// Serve static files from the Angular dist folder
app.use(express.static(path.join(__dirname, 'dist/client/browser'))); // Replace 'your-app-name'

// Handle all other requests by serving index.html
app.get('/', (req, res) => {
    res.sendFile(path.join(__dirname, 'dist/client/browser/index.html')); // Replace 'your-app-name'
    //console.log('Response inside server.js: ', res);
});

// Set port and start server
const PORT = process.env.PORT || 8080;
app.listen(PORT, () => {
    console.log(`Server listening on port ${PORT}`);
});