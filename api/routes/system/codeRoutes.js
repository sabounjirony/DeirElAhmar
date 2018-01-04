'use strict';
module.exports = function (app) {
    var codeController = require('../../controllers/system/codeController');

    app.route('/codes')
        .get(codeController.get)

    app.route('/code')
        .get(codeController.loadAll)
        .post(codeController.create);

    app.route('/code/:id')
        .get(codeController.loadSingle)
        .put(codeController.update)
        .delete(codeController.delete);
};