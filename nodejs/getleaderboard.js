module.exports = function (app, client) {
    app.get("/leaderboard", (req, res, next) => {
        client.json_get("leaderboard", function( err, results) {
            res.send(Object.values(JSON.parse(results)))
        })
    })
}