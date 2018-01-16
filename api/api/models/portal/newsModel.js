'use strict';

require('./common/photoModel.js');
require('./common/videoModel.js');
require('./common/linkModel.js');
require('./common/commentModel.js');

var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var NewsSchema = new Schema({
    category: { type: String, required: 'Category is required!' },
    subCategory: { type: String, required: 'Sub category is required!' },
    title: { type: String, required: 'Title is required!' },
    body: { String, default: "" },
    source: { type: String, required: 'Source is required!' },
    date: { type: Date, default: Date.now },
    branchId: { type: Number },
    languageId: { type: Number },
    entryDate: { type: Date, default: Date.now },
    statusDate: { type: Date, default: Date.now },
    userId: { type: Number },
    status: { type: String, default: ["D"] },
    xtraData: { type: String, default: "" },
    photos: [mongoose.model('photo').schema],
    videos: [mongoose.model('video').schema],
    links: [mongoose.model('link').schema],
    comments: [mongoose.model('comment').schema],
    tags: [String]
}, { collection: 'news' });

module.exports = mongoose.model('news', NewsSchema);