import { photo } from '../common.photo';
import { video } from '../common.video';
import { link } from '../common.link';
import { comment } from '../common.comment';

'use strict';
var mongoose = require('mongoose');
var Schema = mongoose.Schema;

var NewsSchema = new Schema({
    id: { type: Integer },
    category: { type: String, required: 'Category is required!' },
    subCategory: { type: String, required: 'Sub category is required!' },
    title: { type: String, required: 'Title is required!' },
    body: { string, default: "" },
    source: { type: String, required: 'Source is required!' },
    date: { type: Date, default: Date.now },
    branchId: { type: Number },
    languageId: { type: Number },
    entryDate: { type: Date, default: Date.now },
    statusDate: { type: Date, default: Date.now },
    userId: { type: Number },
    status: { type: String, default: ["D"] },
    xtraData: { type: String, default: "" },
    photos: [photo],
    videos: [video],
    links: [link],
    comments: [comment],
    tags: [String]
});

module.exports = mongoose.model('news', NewsSchema);