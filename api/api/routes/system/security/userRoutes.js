'use strict';

module.exports = function (app) {
    var objController = require('../../controllers/system/security/userController');

    app.route('/user')
        .get(objController.getAll)
        .post(objController.post)
        .authenticate(objController.post);

    app.route('/user/:id')
        .get(objController.get)
        .put(objController.put)
        .delete(objController.delete);
};