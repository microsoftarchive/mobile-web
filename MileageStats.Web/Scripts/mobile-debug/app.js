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

(function (app, global, $) {

	// ** bootstrapper **
	// iterate through the modules calling the
	// constructor functions for each module
	// and storing the resulting export
	// back as the same name.
	// we also pass in depedencies to each module
	var module;

	// this function is responsible for fulfilling
	// depedencies in modules
	function require(service) {
		// this is a simplified approach to resolving
		// a more thorough approach would check for
		// cyclical registration
		if (service in global) return global[service];

		if (service in app) {
			if (typeof app[service] === 'function') {
				app[service] = app[service](require);
			}
			return app[service];
		}

		throw new Error('unable to locate ' + service);
	}

	$(function () {
		var registration;
		
		for (registration in app) {
			module = app[registration];
			// check to see if the module is
			// a function or an object and only apply it
			// when it is a function
			if (typeof module === 'function') {
				app[registration] = module(require);
			}
		}

		// after the modules are all bootstrapped
		// perform any necessary configuration

		app.router.setDefaultRegion('#main');
		var register = app.router.register;

		register('/Dashboard/Index', app.dashboard);

		register('/Vehicle/:vehicleId/Details');
		register('/Vehicle/:vehicleId/Fillup/List', { postrender: function (res, view) { app.expander.attach(view); } });
		register('/Vehicle/:vehicleId/Fillup/:id/Details');
        register('/Vehicle/:vehicleId/Fillup/Add', app.vehicleFillupAdd);
        register('/Vehicle/:vehicleId/Reminder/ListByGroup');

		register('/Vehicle/:vehicleId/Reminder/ListByGroup', { postrender: function (res, view) { app.expander.attach(view); } });
		register('/Vehicle/:vehicleId/Reminder/:id/Details');

		register('/Chart', mstats.charts);

		// these forms are special cases, we need to address them
		//register('/Vehicle/Edit/:id');
		//register('/Vehicle/Edit');
		//register('/Vehicle/Add/:id');
		//register('/Vehicle/Add', { fetch: false }); // the vehicle form is complicated because of it's wizard like workflow

		// the root url
		register('/', app.dashboard);

		app.router.initialize();

        // add a visual indicator when in SPA
        $('header h1').after('<span class="spa">SPA</span>');
	});



})(window.mstats = window.mstats || {}, window, $);