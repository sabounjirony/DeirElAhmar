'use strict';

var mongoose = require('mongoose'),
    Code = mongoose.model('code');

exports.get = function (req, res) {
    Code.findById(req.params.id, function (err, code) {
        if (err)
            res.send(err);
        res.json(code);
    });
};

exports.getAll = function (req, res) {
    Code.find({}, function (err, code) {
        if (err)
            res.send(err);
        res.json(code);
    });
};

exports.put = function (req, res) {
    var oCode = new Code(req.body);
    oCode.save(function (err, code) {
        if (err)
            res.send(err);
        res.json(code);
    });
};

exports.post = function (req, res) {
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
        res.json({ message: 'Successfully deleted' });
    });
};