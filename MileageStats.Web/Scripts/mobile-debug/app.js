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
    // this is entry point for the main javascript

    function require(service) {
        // this function is responsible for resolving depedencies
        // it assists us in keeping our modules isolated from the 
        // global scope which is useful for composition as well 
        // as unit testing 
        
        // in this context, 'service' can be a globally scoped
        // object such as `window` or jQuery's `$`
        // in addition, it can refer to one of the "modules" that
        // that has been attached to `app`, where `app` is the 
        // local variable for `mstats`.
        
	    if (service in global) return global[service];
	    
		// below is a simplified approach to resolving items.
		// a more thorough approach would check for
	    // cyclical registration
        
		if (service in app) {
			if (typeof app[service] === 'function') {
				app[service] = app[service](require);
			}
			return app[service];
		}

		throw new Error('unable to locate ' + service);
	}

	$(function () {
	    var registration,
	        module;

	    // we consider each property of `app` to be a "registration"
	    // in other words, each module is responsible for telling 
	    // the global `mstats` object about itself.
	    // we iterate through these "registrations" and invoke any that
		// happen to be functions. we store the result of the invocation
	    // back as the same name.
	    
        // we also pass `require` to each module, in order to allow 
	    // individual modules to resolve their dependencies
	    
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

		// from an MVC perspective, you can think of these as 
		// associating a route with a controller
		// however, these routes correspond to the portion of 
	    // the url after the #
	    
		register('/Dashboard/Index', app.dashboard);

		register('/Vehicle/:vehicleId/Details');
		register('/Vehicle/:vehicleId/Fillup/List', { postrender: function (res, view) { app.expander.attach(view); } });
		register('/Vehicle/:vehicleId/Fillup/:id/Details');
        register('/Vehicle/:vehicleId/Fillup/Add', app.fillupAdd);
        register('/Vehicle/:vehicleId/Reminder/ListByGroup');

		register('/Vehicle/:vehicleId/Reminder/ListByGroup', { postrender: function (res, view) { app.expander.attach(view); } });
		register('/Vehicle/:vehicleId/Reminder/:id/Details', mstats.reminderFulfill);
		register('/Vehicle/:vehicleId/Reminder/Add', mstats.reminderAdd);

		register('/Chart', mstats.charts);

		register('/Vehicle/:id/Edit', mstats.vehicleEdit);
		register('/Vehicle/Add', mstats.vehicleAdd);

		// the root url
		register('/', app.dashboard);

		app.router.initialize();

        // add a visual indicator when in SPA
        $('header h1').after('<span class="spa">SPA</span>');
	});

})(window.mstats = window.mstats || {}, window, $);