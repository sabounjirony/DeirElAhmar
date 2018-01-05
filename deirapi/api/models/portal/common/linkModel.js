'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var LinkSchema = new Schema({
    body: String,
    date: Date
});

module.exports = mongoose.model('link', LinkSchema);