(function () {
    window.towerDefense = window.towerDefense || {};

    var proxy = $.connection.gameHub;

    var doGameStateStuff = function (gameState) {

        var convertFoes = function (foes) {
            return _.map(foes, function (foe) {
                return {
                    health: foe.Health,
                    id: foe.Id,
                    speed: foe.Speed,
                    x: foe.X,
                    y: foe.Y
                };
            });
        };
        var convertedGameState = { foes: convertFoes(gameState.Foes) };

        towerDefense.gameDrawer.drawGame(convertedGameState);
    };

    var init = function () {
        proxy.server.getGameState().done(doGameStateStuff);
    }

    proxy.client.updateGameState = doGameStateStuff;

    $.connection.hub.start().done(init);
})();