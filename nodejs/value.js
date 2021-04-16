require('dotenv').config();
var redis = require('redis');
var client = redis.createClient({port:12282, host:process.env.HOST, password:process.env.PASSWORD}); 

client.on('connect', function() {
    console.log('connected');
});

client.set('car', 'fast', function(err, reply) {
    console.log(reply);
  });

console.log(client.get('car', function(err, reply) {
    console.log(reply);
}));