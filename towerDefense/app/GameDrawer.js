(function () {
    var canvas, ctx;
    var foes = [];
    var tanks = [];
    var goals = [];
    var booms = [];
    var bloodSplatters = [];
    var gravities = [];
    var lost = false;
    var wave = 0;
    window.towerDefense = window.towerDefense || {};

    var splatterOptions = {
        spread: 1,
        consistency: 0.04,
        partCount: 15,
        partLifespan: 5,
        updateFrames: 2,
        maxSplatterPartsToDraw: 1000
    }

    var foeType = {
        monster: 0,
        boss: 1
    }

    var monsterType = {
        none: 0,
        kamakaze: 1,
        fire: 2,
        healing: 3,
        splitter: 4
    }

    var drawColoredRotatedImage = function(image, x, y, angle, color) {
        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(angle);
        
        var tintCanvas = document.createElement('canvas');
        tintCanvas.width = image.width;
        tintCanvas.height = image.height;
        var tintCtx = tintCanvas.getContext('2d');

        if (color) {
            // this will draw the tint on top of the image
            tintCtx.fillStyle = color;
            tintCtx.fillRect(0, 0, image.width, image.height); // destination image
            tintCtx.globalCompositeOperation = "destination-atop"; // Displays the destination image on top of the source image. The part of the destination image that is outside the source image is not shown
            tintCtx.drawImage(image, 0, 0); // source image
        }

        ctx.drawImage(image, -(image.width / 2), -(image.height / 2));
        ctx.globalAlpha = 0.5;
        ctx.drawImage(tintCanvas, -(image.width / 2), -(image.height / 2));
        ctx.globalAlpha = 1;

        ctx.restore();
    }

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
    
    // Board
    var spawnImage = new Image();
    spawnImage.src = "../Sprites/spawnPoint.png";
    var goalImage = new Image();
    goalImage.src = "../Sprites/tower.png";

    // Foe
    var jellyImage = new Image();
    jellyImage.src = "../Sprites/jelly.png";
    var healerImage = new Image();
    healerImage.src = "../Sprites/healer.png";
    var flameImage = new Image();
    flameImage.src = "../Sprites/flame.png";
    var splitterImage = new Image();
    splitterImage.src = "../Sprites/splitter.png";

    // Tank
    var tankTurretImage = new Image();
    tankTurretImage.src = "../Sprites/tankTurret.png";
    var tankBaseImage = new Image();
    tankBaseImage.src = "../Sprites/tankBase.png";

    // Explosions
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
        drawColoredRotatedImage(tankBaseImage, tank.x + 16, tank.y + 16, tank.movementAngle, tank.tankColor);
        if (tank.shooting) {
            ctx.beginPath();
            ctx.moveTo(tank.x + 16, tank.y + 16);
            ctx.lineTo(tank.target.x, tank.target.y);

            var getLaserColor = function (bullet) {
                var damageRatio = (bullet.damage) / (bullet.freeze + bullet.damage);
                var freezeRatio = (bullet.freeze) / (bullet.freeze + bullet.damage);
                var fractionToHex = function (ratio) { return ('0' + Math.round(ratio * 255).toString(16)).substr(-2); };
                return '#' + fractionToHex(damageRatio) + '00' + fractionToHex(freezeRatio);
            };

            var getSplashColor = function (bullet) {
                var damageRatio = (bullet.damage) / (bullet.freeze + bullet.damage);
                var freezeRatio = (bullet.freeze) / (bullet.freeze + bullet.damage);
                var f = function (r) { return Math.round(r * 255); };
                return 'rgba(' + f(damageRatio) + ', 0, ' + f(freezeRatio) + ', 0.5)';
            };

            var getLaserWidth = function (bullet) {
                return Math.round(Math.log2(bullet.damage + bullet.freeze));
            };

            ctx.strokeStyle = getLaserColor(tank.bullet);
            ctx.lineWidth = getLaserWidth(tank.bullet);
            ctx.stroke();

            ctx.beginPath();
            ctx.arc(tank.target.x, tank.target.y, tank.bullet.splashRange, 0, 2 * Math.PI, false);
            ctx.fillStyle = getSplashColor(tank.bullet);
            ctx.fill();
            ctx.stroke();
        }
        drawColoredRotatedImage(tankTurretImage, tank.x + 16, tank.y + 16, tank.angle, tank.tankColor);

        var yMod = tank.y > canvas.height / 2 ? -10 : 34;
        var yNameMod = tank.y > canvas.height / 2 ? -14 : 54;

        ctx.fillStyle = "#000";
        ctx.fillRect(tank.x, tank.y + yMod, 32, 9);
        ctx.fillStyle = "#F00";
        ctx.fillRect(tank.x, tank.y + yMod, -500 / (tank.heat + 16) + 32, 9);
        
        ctx.textAlign = "center";
        ctx.font = '7pt Calibri';
        ctx.fillStyle = 'rgba(255,255,255,1)';
        ctx.fillText(Math.round(tank.heat), tank.center.x, tank.y + yMod + 7, 32);

        ctx.textAlign = "center";
        ctx.font = '10pt Calibri';
        ctx.fillStyle = 'rgba(255,255,255,1)';
        ctx.fillText(tank.name, tank.center.x, tank.y + yNameMod, 100);
    }

    var drawFoe = function (foe) {
        foe.sprite.render();

        var yMod = foe.y > canvas.height / 2 ? -6 : foe.size.height + 4;

        ctx.fillStyle = "#000";
        ctx.fillRect(foe.x, foe.y + yMod, foe.size.width, 5);

        var percent = (foe.health / foe.maxHealth);
        ctx.fillStyle = percent > .5 ? "#0f0" : percent > .25 ? "#ff0" : "#F00";
        ctx.fillRect(foe.x, foe.y + yMod, percent * foe.size.width, 5);

        if (!foe.abilityResult) {
            return;
        }
        if (foe.abilityResult.abilityType === monsterType.fire) {
            ctx.beginPath();
            ctx.arc(foe.x + foe.size.width / 2, foe.y + foe.size.height / 2, foe.abilityResult.range, 0, 2 * Math.PI, false);
            ctx.fillStyle = 'rgba(255, 0, 0, 0.1)';
            ctx.strokeStyle = 'rgba(255, 0, 0, 0.25)';
            ctx.lineWidth = 1;
            ctx.fill();
            ctx.stroke();
        } else if (foe.abilityResult.abilityType === monsterType.healing) {
            ctx.beginPath();
            ctx.arc(foe.x + foe.size.width / 2, foe.y + foe.size.height / 2, foe.abilityResult.range, 0, 2 * Math.PI, false);
            ctx.fillStyle = 'rgba(0, 255, 0, 0.1)';
            ctx.strokeStyle = 'rgba(0, 255, 0, 0.25)';
            ctx.lineWidth = 1;
            ctx.fill();
            ctx.stroke();
        } else if (foe.abilityResult.abilityType === monsterType.kamakaze) {
            var deadFoe = _.extend({}, foe);
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
        }
    };

    var drawWave = function (wave) {
        ctx.textAlign = "center";
        ctx.font = '20pt Calibri';
        ctx.fillStyle = 'rgba(0,0,0,1)';
        ctx.fillText("Wave " + wave, 400, 25);
    }

    var drawGravity = function(gravity) {
        ctx.beginPath();
        ctx.arc(gravity.x + 5, gravity.y + 5, 10, 0, 2 * Math.PI, false);
        ctx.fillStyle = 'rgba(255, 255, 0, 0.8)';
        ctx.strokeStyle = 'rgba(255, 255, 0, 0.25)';
        ctx.lineWidth = 1;
        ctx.fill();
        ctx.stroke();
    }

    var updateBloodSplatterPart = function (part, i, partArray) {
        if (part.update) {
            part.dy -= 0;
            part.x -= (part.dx / splatterOptions.updateFrames);
            part.y -= (part.dy / splatterOptions.updateFrames);
            part.size -= (0.05 / splatterOptions.updateFrames);
        }

        if (part.size < 0.3 || Math.random() < splatterOptions.consistency) {
            part.update = false;
            part.lifespan--;
            
            drawBloodSplatterPart(part);

            if (part.lifespan <= 0) {
                partArray.splice(i, 1);
            }
        }
    };

    var drawBloodSplatterPart = function (part) {
        if (part.size > 0) {
            var alpha = 1 - (1 / part.lifespan);

            ctx.beginPath();
            ctx.arc(part.x + 1.5, part.y + 1.5, part.size * 1.5, 0, 2 * Math.PI, false);
            ctx.fillStyle = 'rgba(0, 255, 0, ' + alpha + ')';
            ctx.strokeStyle = 'rgba(0, 255, 0, 0.25)';
            ctx.fill();
        }
    };

    var drawBloodSplatter = function (bloodSplatter) {
        for (var i = 0; i < bloodSplatter.parts.length; i++) {
            drawBloodSplatterPart(bloodSplatter.parts[i]);
            updateBloodSplatterPart(bloodSplatter.parts[i], i, bloodSplatter.parts);
        }
    }

    var createBloodSplatterParts = function (isBoss, startX, startY, partArray) {
        for (var i = 0; i < splatterOptions.partCount; i++) {
            var s = Math.random() * Math.PI;
            var dirx = (((Math.random() < .5) ? 3 : -3) * (Math.random() * 3)) * splatterOptions.spread;
            var diry = (((Math.random() < .5) ? 3 : -3) * (Math.random() * 3)) * splatterOptions.spread;

            partArray.push({
                x: startX,
                y: startY,
                dx: dirx,
                dy: diry,
                size: s,
                update: true,
                lifespan: splatterOptions.partLifespan
            });
        }
    }

    var rendering = false;
    var renderLoop = function () {
        if (rendering) {
            window.requestAnimationFrame(renderLoop);
        }
        drawBackground();
        drawSpawn();
        _.each(goals, function (goal) {
            drawgoal(goal);
        });

        console.log("drawing " + bloodSplatters.length + " splatters (" + bloodSplatters.length * splatterOptions.partCount + " parts)");
        for (var i = bloodSplatters.length - 1; i >= 0; i--) {
            drawBloodSplatter(bloodSplatters[i]);

            if (bloodSplatters[i].parts.length === 0) {
                bloodSplatters.splice(i, 1);
            }
        }

        _.each(gravities, function(gravity) {
            drawGravity(gravity);
        });
        _.each(tanks, function (tank) {
            drawtank(tank);
        });
        _.each(foes, function (foe) {
            drawFoe(foe);
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
                    ctx.font = '16pt Calibri';
                    var message = tank.owner + " (" + tank.name + ") : " + tank.killed + " kills, " + tank.damage + " damage, " + tank.freeze + " freeze, ";
                    ctx.strokeText(message, 400, y);
                    ctx.fillText(message, 400, y);
                    y += 30;
                    var message = tank.bossesKilled + " boss kills, " + tank.shots + " shots, " + tank.maxDamageDealt + " max damage.";
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

            gravities = gameState.gravityEntities;

            _.each(gameState.foes, function (foe) {
                var renderFoe = _.find(foes, function (f) {
                    return f.id === foe.id;
                });
                if (typeof renderFoe === "undefined") {
                    renderFoe = _.extend({}, foe);

                    var image;
                    var imageWidth = 48, imageHeight = 16;
                    var renderWidth = foe.size.width;
                    var renderHeight = foe.size.height;
                    var frameCount = 3;
                    switch(foe.abilityType) {
                        case monsterType.kamakaze:
                            image = jellyImage;
                            break;
                        case monsterType.fire:
                            image = flameImage;
                            imageWidth = 80;
                            imageHeight = 36;
                            frameCount = 4;
                            break;
                        case monsterType.healing:
                            image = healerImage;
                            break;
                        case monsterType.splitter:
                            image = splitterImage;
                            break;
                    }

                    renderFoe.sprite = window.towerDefense.makeSprite({
                        context: ctx,
                        width: imageWidth,
                        height: imageHeight,
                        image: image,
                        numberOfFrames: frameCount,
                        ticksPerFrame: 5,
                        renderWidth: renderWidth,
                        renderHeight: renderHeight
                    });
                    renderFoe.sprite.x = Math.floor(renderFoe.x);
                    renderFoe.sprite.y = Math.floor(renderFoe.y);
                    renderFoe.abilityResult = foe.abilityResult;

                    foes.push(renderFoe);
                } else {
                    _.extend(renderFoe, foe);
                    renderFoe.sprite.x = Math.floor(renderFoe.x);
                    renderFoe.sprite.y = Math.floor(renderFoe.y);
                    renderFoe.abilityResult = foe.abilityResult;
                }
            });

            var deadFoes = _.filter(foes, function (foe) {
                return !_.find(gameState.foes, function (f) { return f.id === foe.id; });
            });

            var splattersToDraw = splatterOptions.maxSplatterPartsToDraw / splatterOptions.partCount;
            _.each(deadFoes, function (deadFoe) {
                var maxSplatterParts = splatterOptions.partCount;
                var oldMaxSplatterParts = maxSplatterParts;
                if (bloodSplatters.length >= splattersToDraw) {
                    maxSplatterParts = 2;
                }

                var bloodSplatter = {};
                var splatterParts = [];
                var isBoss = deadFoe.foeType === foeType.boss;

                splatterOptions.partCount = maxSplatterParts;
                createBloodSplatterParts(isBoss, Math.floor(deadFoe.x), Math.floor(deadFoe.y), splatterParts);
                splatterOptions.partCount = oldMaxSplatterParts;

                bloodSplatter.parts = splatterParts;
                bloodSplatters.push(bloodSplatter);
            });

            foes = _.filter(foes, function (foe) {
                return _.find(gameState.foes, function (f) { return f.id === foe.id; });
            });

            var calculateAngle = function (tank, target) {
                if (target) {
                    var xComponent = target.x - tank.x - 16;
                    var yComponent = target.y - tank.y - 16;
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

                renderTank.tankColor = gameTank.tankColor;
                renderTank.angle = calculateAngle(renderTank, gameTank.shotTarget);
                renderTank.movementAngle = calculateAngle(renderTank, gameTank.movementTarget);
                renderTank.shooting = gameTank.shooting;
                renderTank.bullet = gameTank.bullet;

                renderTank.killed = gameTank.killed;
                renderTank.damage = gameTank.damage;
                renderTank.freeze = gameTank.freeze;
                renderTank.bossesKilled = gameTank.bossesKilled;
                renderTank.shots = gameTank.shots;
                renderTank.maxDamageDealt = gameTank.maxDamageDealt;


                renderTank.owner = gameTank.owner;
                renderTank.heat = gameTank.heat;
                if (renderTank.shooting) {
                    renderTank.target = gameTank.shotTarget;
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