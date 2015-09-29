﻿(function () {
    var canvas, ctx;
    var foes = [];
    var goals = [];
    window.towerDefense = window.towerDefense || {};

    var backgroundImage = new Image();
    var backgroundPattern;
    backgroundImage.onload = function () {
        if (!ctx) { return; }
        backgroundPattern = ctx.createPattern(backgroundImage, 'repeat');
    }
    var drawBackground = function () {
        ctx.fillStyle = backgroundPattern;
        ctx.fillRect(0, 0, canvas.width, canvas.height);
    }

    var spawnImage = new Image();
    spawnImage.src = "../Sprites/spawnPoint.png";
    var drawSpawn = function () {
        ctx.drawImage(spawnImage, 0, 0);
    }

    var goalImage = new Image();
    goalImage.src = "../Sprites/tower.png";
    var drawgoal = function (x, y) {
        ctx.drawImage(goalImage, x, y, 32, 48);
    }

    var rendering = false;
    var renderLoop = function () {
        if (rendering) {
            window.requestAnimationFrame(renderLoop);
        }
        drawBackground();
        drawSpawn();
        _.each(foes, function (foe) {
            foe.sprite.render();
        });
        _.each(goals, function (goal) {
            drawgoal(goal.x, goal.y);
        });
    }

    var foeImage = new Image();
    //foeImage.addEventListener("load", renderLoop);
    foeImage.src = "../Sprites/jelly.png";

    var gameDrawer = {
        init: function (c) {
            canvas = c;
            ctx = canvas.getContext("2d");
            backgroundImage.src = "../Sprites/background.png";
            canvas.width = 800;
            canvas.height = 800;
        },
        drawGame: function (gameState) {
            if (!ctx) { return; }
            _.each(gameState.foes, function (foe) {
                var renderFoe = _.find(foes, function (f) {
                    return f.id === foe.id;
                });
                if (typeof renderFoe === "undefined") {
                    renderFoe = _.extend({}, foe);
                    renderFoe.sprite = window.towerDefense.makeSprite({
                        context: ctx,
                        width: 48,
                        height: 16,
                        image: foeImage,
                        numberOfFrames: 3,
                        ticksPerFrame: 10
                    });
                    renderFoe.sprite.x = Math.floor(renderFoe.x);
                    renderFoe.sprite.y = Math.floor(renderFoe.y);
                    foes.push(renderFoe);
                } else {
                    _.extend(renderFoe, foe);
                    renderFoe.sprite.x = Math.floor(renderFoe.x);
                    renderFoe.sprite.y = Math.floor(renderFoe.y);
                }
            });
            foes = _.filter(foes, function (foe) {
                return _.find(gameState.foes, function (f) { return f.id === foe.id; });
            });

            _.each(gameState.goals, function (goal) {
                var rendergoal = _.find(goals, function (f) {
                    return f.id === goal.id;
                });
                if (typeof rendergoal === "undefined") {
                    rendergoal = _.extend({}, goal);
                    goals.push(rendergoal);
                } else {
                    _.extend(rendergoal, goal);
                }
            });
            goals = _.filter(goals, function (goal) {
                return _.find(gameState.goals, function (f) { return f.id === goal.id; });
            });

            if (!rendering) {
                rendering = true;
                renderLoop();
            }
        }
    };
    window.towerDefense.gameDrawer = gameDrawer;
})();