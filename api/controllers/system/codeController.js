'use strict';

var sql = require('mssql');
var sqlConfig = {
    user: "sa",
    password: "sapassword",
    server: "DQSRV-TFS01\\TFS01",
    database: "DQTasks"
}

var mongoose = require('mongoose'),
    Code = mongoose.model('Code');

    exports.get = function (req, res) {
        console.log(1);
        sql.connect(sqlConfig, function() {
            console.log(2);
            var request = new sql.Request();
            console.log(3);
            request.query('select * from Codes', function(err, recordset) {
                if(err) console.log(err);
                res.end(JSON.stringify(recordset)); // Result in JSON format
            });
        })};

exports.create = function (req, res) {
    var oCode = new Code(req.body);
    oCode.save(function (err, code) {
        if (err)
            res.send(err);
        res.json(code);
    });
};

exports.update = function (req, res) {
    Code.findOneAndUpdate({ _id: req.params.id }, req.body, { new: true }, function (err, code) {
        if (err)
            res.send(err);
        res.json(code);
    });
};

exports.delete = function (req, res) {
    Code.remove({
        _id: req.params.id
    }, function (err, code) {
        if (err)
            res.send(err);
        res.json({ message: 'Code successfully deleted' });
    });
};

exports.loadSingle = function (req, res) {
    Code.findById(req.params.id, function (err, code) {
        if (err)
            res.send(err);
        res.json(code);
    });
};

exports.loadAll = function (req, res) {
    Code.find({}, function (err, code) {
        if (err)
            res.send(err);
        res.json(code);
    });
};