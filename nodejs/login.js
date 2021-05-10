module.exports = function (app, client) {
    app.post("/login", (req, res, next) => {
        var response 
        var myjson = req.body
        var playernames = []
        client.json_get("players", function (err, results) {
            const currentplayers = Object.values(JSON.parse(results))
            currentplayers.forEach(function (item, index){
                playernames.push(item.name)
            })
            if (playernames.includes(myjson.name)) {
                response = {
                    "name" : myjson.name,
                    "success" : false, 
                    "message" : "Username already taken"
                }
            } else {
                response = {
                    "name" : myjson.name,
                    "success" : true, 
                    "message" : "Connected"
                }
            }
            if (/\s/.test(myjson.name)) {
                response = {
                    "name" : myjson.name,
                    "success" : false, 
                    "message" : "Username can not contain spaces"
                }
            }
            res.send(response)
        })
    })

}