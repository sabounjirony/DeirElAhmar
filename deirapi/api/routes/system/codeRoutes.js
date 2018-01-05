'use strict';
module.exports = function (app) {
    var codeController = require('../../controllers/system/codeController');

    app.route('/Code')
        .get(codeController.loadAll)
        .post(codeController.create);

    app.route('/Code/:id')
        .get(codeController.loadSingle)
        .put(codeController.update)
        .delete(codeController.delete);
};