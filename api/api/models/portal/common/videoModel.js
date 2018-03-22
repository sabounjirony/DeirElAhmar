'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var VideoSchema = new Schema({
    body: String,
    date: Date
});

module.exports = mongoose.model('video', VideoSchema);