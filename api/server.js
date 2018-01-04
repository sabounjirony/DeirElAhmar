var express = require('express'); //Web Framework
var app = express();

var sql = require('mssql'); //MS Sql Server client

var sqlConfig = {
    user: "sa",
    password: "sapassword",
    server: "DQSRV-TFS01\\TFS01",
    database: "DQTasks"
}

Code = require('../api/models/system/codeModel'), //created model loading here
bodyParser = require('body-parser');
var routes = require('../api/routes/System/codeRoutes'); //importing route
routes(app); //register the route

//Start server and listen on http://localhost:8081/
var server = app.listen(8081, function () {
    var host = server.address().address
    var port = server.address().port
    console.log("app listening at http://%s:%s", host, port)
});
