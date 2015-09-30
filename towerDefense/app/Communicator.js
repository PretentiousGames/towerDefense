(function () {
    window.towerDefense = window.towerDefense || {};

    var proxy = $.connection.gameHub;

    var updateGameState = function (gameState) {
        towerDefense.gameDrawer.drawGame(towerDefense.mapToJsObject(gameState));
    };
    
    var init = function () {
        proxy.server.getGameState().done(updateGameState);
    }

    proxy.client.updateGameState = updateGameState;

    $.connection.hub.start().done(init);
})();