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
        sprite.renderWidth = options.renderWidth || sprite.width / numberOfFrames;
        sprite.renderHeight = options.renderHeight || sprite.height;
        sprite.image = options.image;
        sprite.x = options.x || 0;
        sprite.y = options.y || 0;
        sprite.loop = typeof options.loop === 'undefined' ? true : options.loop;
        sprite.destroy = options.destroyCallback || function () { };

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

            var x = sprite.x;
            var y = sprite.y;
            var xs = sprite.width / numberOfFrames;
            var ys = sprite.height;
            sprite.context.translate(x, y);
            if (sprite.angle) {
                sprite.context.translate(sprite.renderWidth / 2, sprite.renderHeight / 2);
                sprite.context.rotate(sprite.angle);
                sprite.context.translate(-sprite.renderWidth / 2, -sprite.renderHeight / 2);
            }

            sprite.context.drawImage(
                sprite.image,
                frameIndex * xs,
                0,
                xs,
                ys,
                0,
                0,
                sprite.renderWidth,
                sprite.renderHeight);

            if (sprite.angle) {
                sprite.context.translate(sprite.renderWidth / 2, sprite.renderHeight / 2);
                sprite.context.rotate(-sprite.angle);
                sprite.context.translate(-sprite.renderWidth / 2, -sprite.renderHeight / 2);
            }
            sprite.context.translate(-x, -y);
        };

        return sprite;
    };
})();