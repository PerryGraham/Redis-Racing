var redis = require('redis');
var client = redis.createClient(port, host); 

client.on('connect', function() {
    console.log('connected');
});

client.set('poopypants', 'lol');
