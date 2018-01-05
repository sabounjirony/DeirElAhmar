'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var CommentSchema = new Schema({
    body: String,
    date: Date
});

module.exports = mongoose.model('comment', CommentSchema);