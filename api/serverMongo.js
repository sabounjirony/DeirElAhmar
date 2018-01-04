var express = require('express'),
app = express(),
port = process.env.PORT || 3000;

mongoose = require('mongoose'),
Code = require('../api/models/system/codeModel'), //created model loading here
bodyParser = require('body-parser');

// mongoose instance connection url connection
mongoose.Promise = global.Promise;
mongoose.connect('mongodb://localhost/Seed', { useMongoClient: true }); 

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
// app.use(function(req, res) {
//     res.status(404).send({url: req.originalUrl + ' not found'})
//   });

var routes = require('../api/routes/System/codeRoutes'); //importing route
routes(app); //register the route

app.listen(port);

console.log('API started successfully: ' + port);