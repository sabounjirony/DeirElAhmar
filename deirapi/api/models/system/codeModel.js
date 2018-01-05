'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var CodeSchema = new Schema({
    Category: {type: String, required: 'Kindly enter the category of the code'},
    Name: {type: String, required: 'Kindly enter the name of the code'},
    Value: {type: String, required: 'Kindly enter the value of the code'},
    Description: {type: String},
    Status: {type: [{type: String,enum: ['Active', 'Stopped']}], default: ['Active']},
    IsProtected: {type: Boolean, default: true}
}, { collection: 'code' });

module.exports = mongoose.model('code', CodeSchema);