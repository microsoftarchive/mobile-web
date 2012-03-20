/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
ARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

// ***
// this mock helper is specific to the MileageStats application
// ***
(function (module) {

	// we'll generate a fresh instance of our mocks
	// for each unit test
	function generate(existing, behaviors) {

		// some of our mocks will record what they do
		// so that we can verify behavior
		var tracked = [];
		tracked.contains = function (candidate) {
			var i = tracked.length - 1;

			for (; i >= 0; i--) {
				if (tracked[i] === candidate) return true;
			}
			return false;
		};

		// we can pass into our generate function a hash of 
		// behaviors, this will allow us to define behavior
		// specific to individual unit tests
		function getBehavior(target, defaultFn) {
			if (behaviors && behaviors[target]) return behaviors[target];
			return defaultFn || function () { return this; };
		};

		// mock jQuery
		function buildMember(name, selector, defaultFn) {

			// we want to track calls to the mocked jQuery
			// we'll record the selector, the name of the member invoke,
			// and the value passed to the member
			// each entry will look like:
			//     selector.member(arg)
			// for example, if we invoke:
			//     $('a').attr('href')
			// we'll see the following in the tracked array:
			//    a.attr(href)
			// or if we invoke:
			//     $('form').submit(function() {});
			// we'll see :
			//    form.submit()

			return function (arg) {
				var arg_text = (arg && typeof (arg) !== 'function') ? arg : '';
				var context = selector || this.__parent__;
				tracked.push(context + '.' + name + '(' + arg_text + ')');
				var fn = getBehavior('$.' + name, defaultFn);
				var el = $(arg);
				el.__parent__ = context;
				return fn.apply(el, arguments);
			};
		}

		var jqueryMembers = [
            'addClass',
            'append',
            'children',
            'empty',
            'find',
            'first',
            'html',
            'last',
            'next',
            'on',
            'removeClass',
            'toggle',
            'toggleClass'
        ];

		var $ = function (selector) {
			var jquery = {},
                member,
                i = jqueryMembers.length - 1;

			for (; i >= 0; i--) {
				member = jqueryMembers[i];
				jquery[member] = buildMember(member, selector);
			}

			jquery.attr = buildMember('attr', selector, function (name, value) { return ''; });
			jquery.submit = buildMember('submit', selector, function () { return ''; });
			jquery.data = buildMember('data', selector, function () { return ''; });
			jquery.serialize = buildMember('serialize', selector, function () { return ''; });
			jquery.each = function (fn) {
				fn(0, selector + ' item');
			};

			return jquery;
		};

		$.ajax = function (args) {
			tracked.push('ajax: ' + args.url);
			if (args.success) args.success({});
		};

		var notifications = {
			send: function (model) {
				tracked.push('flash: ' + model.FlashAlert);
				tracked.push('confirm: ' + model.FlashConfirm);
			},
			subscribe: function (model) {
				tracked.push('subscribe');
			}
		};

		// return a hash of the objects we are mocking
		return merge({
			$: $,
			Mustache: {
				to_html: function () {
					return 'template';
				}
			},
			rootUrl: '/',
			tracked: tracked,
			window: {},
			log: function () { return console.log; },
			notifications: notifications
		}, existing);

	}

	function merge(target, source) {
		if (!source) return target;
		var prop;
		for (prop in source) {
			if (!source.hasOwnProperty(prop)) { continue; }
			target[prop] = source[prop];
		}

		return target;
	}

	// the only exposed member of our mocks module
	// is used to create a new set of mocks. 
	// we can optionally provide some behaviors to 
	// override.
	module.create = function (existing, behaviors) {
		var m = generate(existing, behaviors);
		var prop;

		// this emulates the service location in
		// the main app.js file
		var fn = function (service) {
			if (m[service]) return m[service];
			throw new Error('Could not find a module registered as ' + service);
		};

		// for convenience we'll alias the individual
		// mocks as members of the function
		for (prop in m) {
			// this could be quite dangerous, for example
			// we could override a default member of function
			fn[prop] = m[prop];
		}

		return fn;
	};

} (this.mocks = this.mocks || {}));