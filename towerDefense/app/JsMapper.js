(function () {
    window.towerDefense = window.towerDefense || {};

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
            else if (_.isObject(child)) {
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

    window.towerDefense.mapToJsObject = mapToJsObject;
})();