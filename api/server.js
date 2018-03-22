var express = require('express'),
app = express(),
port = process.env.PORT || 3000,
mongoose = require('mongoose'),
Entity = require('../api/api/models/entity/entityModel'), //created model loading here
Address = require('../api/api/models/entity/addressModel'),
Relation = require('../api/api/models/entity/relationModel'),

Company = require('../api/api/models/structure/companyModel'),
Branch = require('../api/api/models/structure/branchModel'),

Code = require('../api/api/models/system/codeModel'),
Database = require('../api/api/models/system/databaseModel'),
Description = require('../api/api/models/system/descriptionModel'),
Help = require('../api/api/models/system/helpModel'),
Errorlog = require('../api/api/models/system/log/errorLogModel'),
Eventlog = require('../api/api/models/system/log/eventLogModel'),
Menu = require('../api/api/models/system/security/menuModel'),
Module = require('../api/api/models/system/security/moduleModel'),
Permission = require('../api/api/models/system/security/permissionModel'),
Role = require('../api/api/models/system/security/roleModel'),
User = require('../api/api/models/system/security/userModel'),

Article = require('../api/api/models/portal/articleModel'),
Bible = require('../api/api/models/portal/bibleModel'),
Directory = require('../api/api/models/portal/directoryModel'),
Event = require('../api/api/models/portal/eventModel'),
Figure = require('../api/api/models/portal/figureModel'),
Flash = require('../api/api/models/portal/flashModel'),
Funeral = require('../api/api/models/portal/funeralModel'),
Immigrant = require('../api/api/models/portal/immigrantModel'),
Municipality = require('../api/api/models/portal/municipalityModel'),
NewBorn = require('../api/api/models/portal/newBornModel'),
NewsLetter = require('../api/api/models/portal/newsLetterModel'),
News = require('../api/api/models/portal/newsModel'),
Poll = require('../api/api/models/portal/pollModel'),
Project = require('../api/api/models/portal/projectModel'),
Tourism = require('../api/api/models/portal/articleModel'),
Wedding = require('../api/api/models/portal/articleModel'),

bodyParser = require('body-parser');

// mongoose instance connection url connection
mongoose.Promise = global.Promise;
mongoose.connect('mongodb://localhost/deirelahmar', { useMongoClient: true });  //database name goes here

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
// app.use(function(req, res) {
//     res.status(404).send({url: req.originalUrl + ' not found'})
//   });

var routes = require('../api/api/routes/System/codeRoutes'); //importing route
routes(app); //register the route

app.listen(port);

console.log('API started successfully: ' + port);