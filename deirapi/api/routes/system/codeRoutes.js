'use strict';

module.exports = function (app) {
    var objController = require('../../controllers/system/codeController');

    app.route('/code')
        .get(objController.getAll)
        .post(objController.post);

    app.route('/code/:id')
        .get(objController.get)
        .put(objController.put)
        .delete(objController.delete);
};