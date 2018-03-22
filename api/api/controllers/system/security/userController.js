'use strict';

var mongoose = require('mongoose'),
    User = mongoose.model('user');

exports.get = function (req, res) {
    User.findById(req.params.id, function (err, user) {
        if (err)
            res.send(err);
        res.json(user);
    });
};

exports.getAll = function (req, res) {
    User.find({}, function (err, user) {
        if (err)
            res.send(err);
        res.json(user);
    });
};

exports.post = function (req, res) {
    var oUser = new User(req.body);
    oUser.save(function (err, user) {
        if (err)
            res.send(err);
        res.json(user);
    });
};

exports.put = function (req, res) {
    User.findOneAndUpdate({ _id: req.params.id }, req.body, { new: true }, function (err, user) {
        if (err)
            res.send(err);
        res.json(user);
    });
};

exports.delete = function (req, res) {
    User.remove({
        _id: req.params.id
    }, function (err, user) {
        if (err)
            res.send(err);
        res.json({ message: 'Successfully deleted' });
    });
};

exports.authenticate = function (req, res) {
    User.find({}, function (err, user) {
        if (err)
            res.send(err);
        res.json(user);
    });
};