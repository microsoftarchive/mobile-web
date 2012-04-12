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
// This mock helper is specific to the MileageStats application
// ***
(function (module) {

	// We'll generate a fresh instance of our mocks
	// for each unit test.
	function generate(existing) {

		// Some of our mocks will record what they do
		// so that we can verify behavior.
		var tracked = [];
		tracked.contains = function (candidate) {
			var i = tracked.length - 1;

			for (; i >= 0; i--) {
				if (tracked[i] === candidate) return true;
			}
			return false;
		};

		var notifications = {
			log: function (model) {
				tracked.push('flash: ' + model.FlashAlert);
				tracked.push('confirm: ' + model.FlashConfirm);
			},
			renderTo: function (view) {
				tracked.push('subscribe');
			}
		};

        // Return a hash of the objects we are mocking.
        // Note: we're not actually mocking out jQuery, 
        // However, it is included here, because some 
	    // modules require it.
        return merge({
            $:$,
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

	// The only exposed member of our mocks module
	// is used to create a new set of mocks. 
	// We can optionally provide some behaviors to 
	// override.
	module.create = function (existing, behaviors) {
		var m = generate(existing, behaviors);
		var prop;

		// This emulates the service location in
		// the main app.js file.
		var fn = function (service) {
			if (m[service]) return m[service];
			throw new Error('Could not find a module registered as ' + service);
		};

		// For convenience we'll alias the individual
		// mocks as members of the function.
		for (prop in m) {
			// This could be quite dangerous; for example,
			// we could override a default member of function.
			fn[prop] = m[prop];
		}

		return fn;
	};

} (this.mocks = this.mocks || {}));