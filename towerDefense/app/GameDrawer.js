(function () {
    var canvas, ctx;
    var foes = [];
    var tanks = [];
    var goals = [];
    var booms = [];
    var lost = false;
    var wave = 0;
    window.towerDefense = window.towerDefense || {};

    var drawRotatedImage = function (image, x, y, angle) {
        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(angle);
        ctx.drawImage(image, -(image.width / 2), -(image.height / 2));
        ctx.restore();
    }

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
    var goalImage = new Image();
    goalImage.src = "../Sprites/tower.png";
    var foeImage = new Image();
    foeImage.src = "../Sprites/jelly.png";
    var tankTurretImage = new Image();
    tankTurretImage.src = "../Sprites/tankTurret.png";
    var tankBaseImage = new Image();
    tankBaseImage.src = "../Sprites/tankBase.png";
    var boomImage = new Image();
    boomImage.src = "../Sprites/boom.2.png";
    var goalBoomImage = new Image();
    goalBoomImage.src = "../Sprites/boom.4.png";

    var drawSpawn = function () {
        ctx.drawImage(spawnImage, 400 - 20, 400 - 20);
    }

    var drawgoal = function (goal) {
        ctx.drawImage(goalImage, goal.x, goal.y, 38, 38);

        var yMod = goal.y > canvas.height / 2 ? -5 : 38;

        ctx.fillStyle = "#000";
        ctx.fillRect(goal.x, goal.y + yMod, 38, 5);
        var percent = (goal.health / goal.maxHealth);
        ctx.fillStyle = percent > .5 ? "#0f0" : percent > .25 ? "#ff0" : "#F00";
        ctx.fillRect(goal.x, goal.y + yMod, percent * 38, 5);
    }

    var drawtank = function (tank) {
        ctx.drawImage(tankBaseImage, tank.x, tank.y, 32, 32);
        if (tank.shooting) {
            ctx.beginPath();
            ctx.moveTo(tank.x + 16, tank.y + 16);
            ctx.lineTo(tank.target.x + 8, tank.target.y + 8);
            ctx.strokeStyle = '#ff0000';
            ctx.stroke();
        }
        drawRotatedImage(tankTurretImage, tank.x + 16, tank.y + 16, tank.angle);

        var yMod = tank.y > canvas.height / 2 ? -6 : 36;

        ctx.fillStyle = "#000";
        ctx.fillRect(tank.x, tank.y + yMod, 32, 5);
        ctx.fillStyle = "#F00";
        ctx.fillRect(tank.x, tank.y + yMod, -500/(tank.heat + 16) + 32, 5);
    }

    var drawFoe = function (foe) {
        foe.sprite.render();

        var yMod = foe.y > canvas.height / 2 ? -6 : 20;

        ctx.fillStyle = "#000";
        ctx.fillRect(foe.x, foe.y + yMod, 16, 5);

        var percent = (foe.health / foe.maxHealth);
        ctx.fillStyle = percent > .5 ? "#0f0" : percent > .25 ? "#ff0" : "#F00";
        ctx.fillRect(foe.x, foe.y + yMod, percent * 16, 5);
    };

    var drawWave = function (wave) {
        ctx.textAlign = "center";
        ctx.font = '20pt Calibri';
        ctx.fillStyle = 'rgba(0,0,0,1)';
        ctx.fillText("Wave " + wave, 400, 25);
    }

    var rendering = false;
    var renderLoop = function () {
        if (rendering) {
            window.requestAnimationFrame(renderLoop);
        }
        drawBackground();
        drawSpawn();
        _.each(tanks, function (tank) {
            drawtank(tank);
        });
        _.each(foes, function (foe) {
            drawFoe(foe);
        });
        _.each(goals, function (goal) {
            drawgoal(goal);
        });
        _.each(booms, function (boom) {
            boom.sprite.render();
        });

        drawWave(wave);

        if (lost) {
            ctx.beginPath();
            ctx.textAlign = "center";
            ctx.font = '55pt Calibri';
            ctx.lineWidth = 3;
            ctx.fillStyle = 'rgba(255,255,255,0.4)';
            ctx.strokeStyle = 'rgba(0,0,0,0.7)';

            ctx.fillRect(100, 100, 600, 600);
            ctx.rect(100, 100, 600, 600);
            ctx.stroke();

            ctx.fillStyle = 'rgba(255,0,0,0.4)';
            var loser = "A Loser(s) Are You!";
            ctx.fillText(loser, 400, 200);
            ctx.strokeText(loser, 400, 200);

            var y = 300;
            _.chain(tanks)
                .sortBy(function (tank) { return -tank.killed; })
                .each(function (tank) {
                    ctx.font = '25pt Calibri';
                    var message = tank.owner + " (" + tank.name + ") : " + tank.killed + " kills";
                    ctx.strokeText(message, 400, y);
                    ctx.fillText(message, 400, y);
                    y += 30;
                });

            var killedAt = "Final Wave: " + wave;
            ctx.fillText(killedAt, 400, y);
            ctx.strokeText(killedAt, 400, y);
        }
    }

    var gameDrawer = {
        init: function (c) {
            canvas = c;
            ctx = canvas.getContext("2d");
            backgroundImage.src = "../Sprites/background.png";
            canvas.width = towerDefense.game.size.width;
            canvas.height = towerDefense.game.size.height;
        },
        drawGame: function (gameState) {
            if (!ctx) { return; }
            lost = gameState.lost;
            wave = gameState.wave;
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
                        ticksPerFrame: 5
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

            var deadFoes = _.filter(foes, function (foe) {
                return !_.find(gameState.foes, function (f) { return f.id === foe.id; });
            });
            _.each(deadFoes, function (deadFoe) {
                deadFoe = _.extend({}, deadFoe);
                deadFoe.sprite = window.towerDefense.makeSprite({
                    context: ctx,
                    width: 2560,
                    height: 64,
                    image: boomImage,
                    numberOfFrames: 40,
                    ticksPerFrame: 1,
                    loop: false,
                    destroyCallback: function () {
                        booms = _.filter(booms, function (boom) {
                            return boom.id !== deadFoe.id;
                        });
                    }
                });
                deadFoe.sprite.x = Math.floor(deadFoe.x - 32);
                deadFoe.sprite.y = Math.floor(deadFoe.y - 32);
                booms.push(deadFoe);
            });

            foes = _.filter(foes, function (foe) {
                return _.find(gameState.foes, function (f) { return f.id === foe.id; });
            });

            var calculateAngle = function (tank, foe) {
                if (foe) {
                    var xComponent = foe.x + foe.size.width / 2 - tank.x - 16;
                    var yComponent = foe.y + foe.size.height / 2 - tank.y - 16;
                    return Math.atan2(xComponent, -yComponent);
                }
                return 0;
            };

            _.each(gameState.gameTanks, function (gameTank) {
                var renderTank = _.find(tanks, function (f) {
                    return f.id === gameTank.tank.id;
                });
                if (typeof renderTank === "undefined") {
                    renderTank = _.extend({}, gameTank.tank);
                    tanks.push(renderTank);
                } else {
                    _.extend(renderTank, gameTank.tank);
                }
                renderTank.angle = calculateAngle(renderTank, gameTank.target);
                renderTank.shooting = gameTank.shooting;
                renderTank.killed = gameTank.killed;
                renderTank.owner = gameTank.owner;
                renderTank.heat = gameTank.heat;
                if (renderTank.shooting) {
                    renderTank.target = gameTank.target;
                }
            });
            tanks = _.filter(tanks, function (tank) {
                return _.find(gameState.gameTanks, function (f) { return f.tank.id === tank.id; });
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

            var deadGoals = _.filter(goals, function (goal) {
                return !_.find(gameState.goals, function (f) { return f.id === goal.id; });
            });
            _.each(deadGoals, function (deadGoal) {
                deadGoal = _.extend({}, deadGoal);
                deadGoal.sprite = window.towerDefense.makeSprite({
                    context: ctx,
                    width: 6144,
                    height: 128,
                    image: goalBoomImage,
                    numberOfFrames: 48,
                    ticksPerFrame: 1,
                    loop: false,
                    destroyCallback: function () {
                        booms = _.filter(booms, function (boom) {
                            return boom.id !== deadGoal.id;
                        });
                    }
                });
                deadGoal.sprite.x = Math.floor(deadGoal.x - 45);
                deadGoal.sprite.y = Math.floor(deadGoal.y - 45);
                booms.push(deadGoal);
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