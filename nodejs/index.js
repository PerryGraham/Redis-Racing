require('dotenv').config();
const express = require('express')
const app = express();
const port = 80
// const posrouter = express.Router();

// app.use("/updatepos", posrouter)

let
  redis     = require('redis'),
  /* Values are hard-coded for this example, it's usually best to bring these in via file or environment variable for production */
  client    = redis.createClient({
    port      : 12282,               // replace with your port
    host      : process.env.HOST,        // replace with your hostanme or IP address
    password  : process.env.PASSWORD,    // replace with your password
  });

app.post("/updatepos", (req, res, next) => {
  var myjson = req.body;
  var myjson = JSON.parse(myJSON);
  // console.log("hi");
  res.send("200")
})

// app.get('/', (req, res) => {
//   res.send('hi')
// })




app.listen(port, () => {
  console.log(`Example app listening at http://localhost:${port}`)
})