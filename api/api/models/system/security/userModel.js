'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var UserSchema = new Schema({
    Id: { type: Number },
    Pin: { type: Number },
    UserName: { type: String },
    Password: { type: String },
    LangId: { type: String },
    AutoLogin: { type: Boolean, default: false },
    PwdChanged: { type: String },
    PwdList: { type: String },
    PwdGrpLogin: { type: Number },
    UserSCode: { type: String },
    Agencies: { type: String },
    PageSize: { type: Number },
    IsRestricted: { type: Boolean },
    InDate: { type: Date },
    EntryUserId: { type: Number },
    Version: { type: String },
    IsLoggedIn: { type: Boolean, default: false }
}, { collection: 'user' });

module.exports = mongoose.model('user', UserSchema);