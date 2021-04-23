require('dotenv').config();
var bodyParser = require('body-parser');
const 
  rejson = require('redis-rejson');
  redis = require('redis');
  // json = require('json')
  express = require('express')
  app = express();
  port = 80

app.use(bodyParser.json());
app.use(bodyParser.urlencoded());

rejson(redis);

let
  client    = redis.createClient({
    port      : 12282,               // replace with your port
    host      : process.env.HOST,        // replace with your hostanme or IP address
    password  : process.env.PASSWORD,    // replace with your password
  });

app.post("/updatepos", (req, res, next) => {
  var myjson = req.body;
  const data = {
    "name": myjson.name,
    "xPos": myjson.xPos,
    "yPos": myjson.yPos,
    "zRot": myjson.zRot,
    "lastping": new Date()
  }
  client.json_set("players",myjson.name, JSON.stringify(data))
  const returndata = client.json_get("players", function (err, results) {
    res.send(Object.values(JSON.parse(results)))
  })
})

app.get('/', (req, res) => {
  res.send("hi")
})

app.listen(port, () => {
  console.log(`Example app listening at http://localhost:${port}`)
})