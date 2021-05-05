module.exports = function (app, client) {
    app.post("/finish", (req, res, next) => {
        var myjson = req.body;
        var oldtime;
        const data = {
            "name": myjson.name,
            "laptime": myjson.time,
            "created": new Date()
        }
        const namekey = `_${myjson.name}`
        client.json_get("leaderboard", `.${namekey}`, function (err, results) {
            oldtime = results
            if (oldtime) {
                oldtime = JSON.parse(oldtime)
                if (myjson.time < oldtime.laptime) {
                    client.json_set("leaderboard", namekey, JSON.stringify(data))
                }
            }
            else {
                client.json_set("leaderboard", namekey, JSON.stringify(data))
            }
        client.json_get("leaderboard", function( err, results) {
            res.send(Object.values(JSON.parse(results)))
            // console.log(Object.values(JSON.parse(results)))
        })
        })
    })
}