var redis = require('redis');
var client = redis.createClient(12282, process.env.HOST); 

client.on('connect', function() {
    console.log('connected');
});

client.set('poopypants', 'lol');
