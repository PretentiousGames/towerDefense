(function () {
    window.towerDefense = window.towerDefense || {};
    window.towerDefense.makeSprite = function (options) {
        var sprite = {},
          frameIndex = 0,
          tickCount = 0,
          ticksPerFrame = options.ticksPerFrame || 0,
          numberOfFrames = options.numberOfFrames || 1;

        sprite.context = options.context;
        sprite.width = options.width;
        sprite.height = options.height;
        sprite.image = options.image;
        sprite.x = options.x || 0;
        sprite.y = options.y || 0;
        sprite.loop = typeof options.loop === 'undefined' ? true : options.loop;
        sprite.destroy = options.destroyCallback || function() {};

        sprite.update = function () {
            tickCount += 1;
            if (tickCount > ticksPerFrame) {
                tickCount = 0;
                if (frameIndex < numberOfFrames - 1) {
                    frameIndex += 1;
                } else {
                    if (sprite.loop) {
                        frameIndex = 0;
                    } else {
                        sprite.destroy();
                    }
                }
            }
        };

        sprite.render = function () {
            sprite.update();
            sprite.context.drawImage(
                sprite.image,
                frameIndex * sprite.width / numberOfFrames,
                0,
                sprite.width / numberOfFrames,
                sprite.height,
                sprite.x,
                sprite.y,
                sprite.width / numberOfFrames,
                sprite.height);
        };

        return sprite;
    };
})();