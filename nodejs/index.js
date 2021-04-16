const express = require('express');
const router = express.Router();
const db = require('../../data/helpers/repDb'); 
//production redis url
let redis_url = process.env.REDIS_URL;
if (process.env.ENVIRONMENT === 'development') {  
  require('dotenv').config();  
  redis_url = "redis://127.0.0.1"; 
}  
//redis setup
let client = require('redis').createClient(redis_url);
let Redis = require('ioredis');
let redis = new Redis(redis_url);
//sample endpoint to GET representative deatils
router.get('/alldetails', (req,res) => { 
  const uid = req.body.uid;       //uid is unique identifier
  //check if rep details are present in cache     
  client.get(uid, (error, rep)=> {                
    if(error){                                                 
      res.status(500).json({error: error});                             
      return;                
  }                 
  if(rep){                        
  //JSON objects need to be parsed after reading from redis, since it is stringified before being stored into cache                      
 
  res.status(200).json(JSON.parse(rep));                 
 }                  
 else{
  //if data not present in cache, make a request to db
  const request = db.getDetails(uid);
  request.then(response_data => {                             
    if(response_data.length == 0) {
      res.status(400).json({ error: "The representative with the specified uid does not exist" });                        
}                        
 else {                                                                          
  res.status(200).json(response_data);               
  //cache data received from db          
  client.set(uid, JSON.stringify(response_data),(error, result)=> { 
  if(error){                                                
    res.status(500).json({ error: error});                        
  }})
}         //end of inner else                
})       //end of .then               
.catch(err => {                        
  res.status(500).json({ err: err.message });                
})         
}   //end of outer else
})  //end of clinet.get 
});