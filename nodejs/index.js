require('dotenv').config();
const express = require('express')
const app = express()
const port = 80

app.get('/', (req, res) => {
  res.send('hi')
})

let
  redis     = require('redis'),
  /* Values are hard-coded for this example, it's usually best to bring these in via file or environment variable for production */
  client    = redis.createClient({
    port      : 12282,               // replace with your port
    host      : process.env.HOST,        // replace with your hostanme or IP address
    password  : process.env.PASSWORD,    // replace with your password
  });


app.listen(port, () => {
  console.log(`Example app listening at http://localhost:${port}`)
})