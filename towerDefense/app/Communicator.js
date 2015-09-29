(function () {
    window.towerDefense = window.towerDefense || {};

    var proxy = $.connection.gameHub;

    var doGameStateStuff = function (gameState) {
        //var convertFoes = function (foes) {
        //    return _.map(foes, function (foe) {
        //        return {
        //            health: foe.Health,
        //            id: foe.Id,
        //            speed: foe.Speed,
        //            x: foe.X,
        //            y: foe.Y
        //        };
        //    });
        //};
        //var convertedGameState = { foes: convertFoes(gameState.Foes) };

        //towerDefense.gameDrawer.drawGame(convertedGameState);

        var mappedGameState = mapToJsObject(gameState);
        towerDefense.gameDrawer.drawGame(mappedGameState);
    };


    var firstToLower = function (str) {
        return str.charAt(0).toLowerCase() + str.slice(1);
    };

    var mapToJsObject = function (o) {
        var r = {};
        _.map(o, function (item, index) {

            var child = o[index];

            if (_.isArray(child)) {
                var mappedChild = [];
                _.map(child, function (i) {
                    var m = mapToJsObject(i);
                    mappedChild.push(m);
                });
                child = mappedChild;
            }

            if (_.isObject(child)) {
                child = mapToJsObject(child);
            }

            if (_.isString(index)) {
                r[firstToLower(index)] = child;
            } else {
                r[index] = child;
            }
        });
        return r;
    };
    
    var init = function () {
        proxy.server.getGameState().done(doGameStateStuff);
    }

    proxy.client.updateGameState = doGameStateStuff;

    $.connection.hub.start().done(init);
})();