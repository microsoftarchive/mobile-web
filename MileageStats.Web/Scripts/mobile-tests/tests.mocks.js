(function (module) {

    function generate() {

        var tracked = [];
        tracked.contains = function(candidate) {
            var i = tracked.length -1;

            for(;i>=0;i--){
                if(tracked[i] === candidate) return true;
            }
            return false;
        };

        // mock jQuery
        var $ = function (selector) {
            return {
                empty: function () {
                    tracked.push('empty: ' + selector);
                },
                html: function () {
                    tracked.push('html: ' + selector);
                    return '<div></div>';
                },
                append: function () {
                    tracked.push('append: ' + selector);
                },
                attr: function () {
                    tracked.push('attr: ' + selector);
                },
            };
        };

        $.ajax = function (args) {
            tracked.push('ajax: ' + args.url);
            if (args.success) args.success();
        };

        return {
            $: $,
            Mustache: {
                to_html: function () {
                    return 'template';
                }
            },
            tracked: tracked,
            window: {},
            log: function () { return console.log; }
        };

    }

    module.create = function () {
        var m = generate();
        var prop;

        var fn = function (service) {
            if (m[service]) return m[service];
            throw new Error('Could not find a module registered as ' + service);
        };

        for (prop in m) {
            // this could be quite dangerous
            fn[prop] = m[prop];
        }

        return fn;
    };

} (this.mocks = this.mocks || {}));