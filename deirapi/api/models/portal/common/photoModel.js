'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var PhotoSchema = new Schema({
    body: String,
    date: Date
});

module.exports = mongoose.model('photo', PhotoSchema);