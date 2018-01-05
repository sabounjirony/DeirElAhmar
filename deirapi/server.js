var express = require('express'),
app = express(),
port = process.env.PORT || 3000,
mongoose = require('mongoose'),
Entity = require('../deirapi/api/models/entity/entityModel'), //created model loading here
Address = require('../deirapi/api/models/entity/addressModel'),
Relation = require('../deirapi/api/models/entity/relationModel'),

Company = require('../deirapi/api/models/structure/companyModel'),
Branch = require('../deirapi/api/models/structure/branchModel'),

Code = require('../deirapi/api/models/system/codeModel'),
Database = require('../deirapi/api/models/system/databaseModel'),
Description = require('../deirapi/api/models/system/descriptionModel'),
Help = require('../deirapi/api/models/system/helpModel'),
Errorlog = require('../deirapi/api/models/system/log/errorLogModel'),
Eventlog = require('../deirapi/api/models/system/log/eventLogModel'),
Menu = require('../deirapi/api/models/system/security/menuModel'),
Module = require('../deirapi/api/models/system/security/moduleModel'),
Permission = require('../deirapi/api/models/system/security/permissionModel'),
Role = require('../deirapi/api/models/system/security/roleModel'),
User = require('../deirapi/api/models/system/security/userModel'),

Article = require('../deirapi/api/models/portal/articleModel'),
Bible = require('../deirapi/api/models/portal/bibleModel'),
Directory = require('../deirapi/api/models/portal/directoryModel'),
Event = require('../deirapi/api/models/portal/eventModel'),
Figure = require('../deirapi/api/models/portal/figureModel'),
Flash = require('../deirapi/api/models/portal/flashModel'),
Funeral = require('../deirapi/api/models/portal/funeralModel'),
Immigrant = require('../deirapi/api/models/portal/immigrantModel'),
Municipality = require('../deirapi/api/models/portal/municipalityModel'),
NewBorn = require('../deirapi/api/models/portal/newBornModel'),
NewsLetter = require('../deirapi/api/models/portal/newsLetterModel'),
News = require('../deirapi/api/models/portal/newsModel'),
Poll = require('../deirapi/api/models/portal/pollModel'),
Project = require('../deirapi/api/models/portal/projectModel'),
Tourism = require('../deirapi/api/models/portal/articleModel'),
Wedding = require('../deirapi/api/models/portal/articleModel'),

bodyParser = require('body-parser');

// mongoose instance connection url connection
mongoose.Promise = global.Promise;
mongoose.connect('mongodb://localhost/deirelahmar', { useMongoClient: true });  //database name goes here

app.use(bodyParser.urlencoded({ extended: true }));
app.use(bodyParser.json());
// app.use(function(req, res) {
//     res.status(404).send({url: req.originalUrl + ' not found'})
//   });

var routes = require('../deirapi/api/routes/System/codeRoutes'); //importing route
routes(app); //register the route

app.listen(port);

console.log('API started successfully: ' + port);