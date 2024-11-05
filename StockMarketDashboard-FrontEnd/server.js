const express = require('express');
const path = require('path');

const app = express();

// Serve the static files from the Angular build directory
app.use(express.static(path.join(__dirname, 'dist/stock-market-dashboard-front-end')));

// Redirect all requests to index.html to enable Angular routing
app.get('*', (req, res) => {
  res.sendFile(path.join(__dirname, 'dist/stock-market-dashboard-front-end/index.html'));
});

// Start the server
const port = process.env.PORT || 8080;
app.listen(port, () => {
  console.log(`Server running on port ${port}`);
});
