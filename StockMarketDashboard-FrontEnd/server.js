const express = require('express');
const path = require('path');

const app = express();
const port = process.env.PORT || 8080;

// Serve the static files from the Angular build directory
app.use(express.static(path.join(__dirname, 'browser')));

// Redirect all requests to index.html to enable Angular routing
app.get('/*', (req, res) => {
  res.sendFile(path.join(__dirname, 'browser', 'index.html'));
});

// Start the server
app.listen(port, () => {
  console.log(`Server is running on port ${port}`);
});
