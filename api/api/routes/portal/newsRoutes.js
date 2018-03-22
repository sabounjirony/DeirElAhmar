'use strict';

module.exports = function (app) {
    var objController = require('../../controllers/portal/newsController');

    app.route('/news')
        .get(objController.getAll)
        .post(objController.post);

    app.route('/news/:id')
        .get(objController.get)
        .put(objController.put)
        .delete(objController.delete);
};