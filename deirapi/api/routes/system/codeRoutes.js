'use strict';
module.exports = function (app) {
    var codeController = require('../../controllers/system/codeController');

    app.route('/code')
        .get(codeController.getAll)
        .post(codeController.post);

    app.route('/code/:id')
        .get(codeController.get)
        .put(codeController.put)
        .delete(codeController.delete);
};