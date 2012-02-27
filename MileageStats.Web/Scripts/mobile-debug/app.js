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

(function (app) {

    // ** bootstrapper **
    // iterate through the modules calling the 
    // constructor functions for each module
    // and storing the resulting export 
    // back as the same name.
    // we also pass in depedencies to each module
    var module, registration;
    var global = this;

    // default logging
    app.log = app.log || function () { return console.log; };

    // this function is responsible for fulfilling
    // depedencies in modules
    function require(service) {
        //todo: check for cyclical registration
        if (service in global) return global[service];

        if (service in app) {
            if (typeof app[service] === 'function') {
                app[service] = app[service](require);
            };
            return app[service];
        }

        throw new Error('unable to locate ' + service);
    }

    for (registration in app) {
        module = app[registration];
        // check to see if the module is
        // a function or an object and only apply it
        // when it is a function
        if (typeof module === 'function') app[registration] = module(require);
    }

    // after the modules are all bootstrapped
    // perform any necessary configuration

    app.router.setDefaultRegion('#view');
    var register = app.router.register;

    register('/Profile/Edit');
    register('/Dashboard/Index');
    //register('/Vehicle/Details/:id'); // requires matching args in a route
    //register('/Vehicle/Add', { fetch: false }); // the vehicle form is complicated because of it's wizard like workflow
    register('/', {
        route: 'Dashboard/Index'
    });

    $(app.router.initialize);

})(window.mstats = window.mstats || {});