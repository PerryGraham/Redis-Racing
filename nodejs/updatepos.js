module.exports = function(app,client){
    app.post("/updatepos", (req, res, next) => {
        var myjson = req.body;
        var playerdata;
        var leaderboarddata;
        const data = {
        "name": myjson.name,
        "xPos": myjson.xPos,
        "yPos": myjson.yPos,
        "zRot": myjson.zRot,
        "lastping": new Date()
        }
        const namekey = `_${myjson.name}`
        client.json_set("players",namekey, JSON.stringify(data))
        client.json_get("players", function (err, results) {
            res.send(Object.values(JSON.parse(results)))
        })
    })
}