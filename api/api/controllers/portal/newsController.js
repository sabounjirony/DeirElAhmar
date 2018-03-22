'use strict';

var mongoose = require('mongoose'),
    model = mongoose.model('news');

exports.get = function (req, res) {
    model.findById(req.params.id, function (err, news) {
        if (err)
            res.send(err);
        res.json(news);
    });
};

exports.getAll = function (req, res) {
    model.find({}, function (err, news) {
        if (err)
            res.send(err);
        res.json(news);
    });
};

exports.put = function (req, res) {
    var obj = new News(req.body);
    obj.save(function (err, news) {
        if (err)
            res.send(err);
        res.json(news);
    });
};

exports.post = function (req, res) {
    model.findOneAndUpdate({ _id: req.params.id }, req.body, { new: true }, function (err, news) {
        if (err)
            res.send(err);
        res.json(news);
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