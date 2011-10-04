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

(function (mstats, $) {

    var dashboardUrl = '/';
    
    mstats.log = function (args) {
        if (typeof console !== 'undefined' && typeof console.log !== 'undefined') {
            console.log(args);
        }
    };

    mstats.strings = {
        shortMonths: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
    };

    /**
    * Recieves a JSON encoded date and returns a formatted string in the following format: "18 Dec 2010"
    *
    * @param {String} dt  A JSON encoded date string in the format: /Date(123456789)/
    */
    mstats.makeDate = function (dt) {
        var dateStr = dt.replace(/^\/Date\(/, '').replace(/\)\//, ''),
            dateObj = new Date(Number(dateStr)),
            day = dateObj.getDate().toString(),
            month = mstats.strings.shortMonths[dateObj.getMonth()],
            year = dateObj.getFullYear().toString();
        return day + ' ' + month + ' ' + year;
    };

    /**
    * Receives a floating point value and converts it into "money" format, i.e. 7.1287 turns into 7.13
    *
    * @param {Float} val  
    */
    mstats.makeMoney = function (val) {
        // Round to 2 decimal places
        val = (Math.round(val * 100) / 100).toString();

        // Pad with a 0 if needed
        if (val.indexOf('.') >= 0) {
            // If val has a decimal value to only one digit
            if (val.substring(val.indexOf('.') + 1).length === 1) {
                val += "0";
            }
        } else {
            // If val doesn't have a decimal value at all
            val += '.00';
        }

        return val;
    };

    mstats.makeMPGDisplay = function (val) {
        // Round to 0 decimal places
        return (Math.round(val)).toString();
    };

    mstats.makeCostToDriveDisplay = function (val) {
        var money = mstats.makeMoney(val * 100);
        return (Math.round(money)).toString();
    };

    mstats.makeCostPerMonthDisplay = function (val) {
        var money = mstats.makeMoney(val);
        return (Math.round(money)).toString();
    };
    
    mstats.getRelativeEndpointUrl = function (endpoint) {
        var i,
            splitString = function (string) {
                if ((string === null) || (string === undefined)) {
                    return '';
                }
                return string.split('/');
            },
            createUrl = function (newUrl, stringArray) {
                for (i = 0; i < stringArray.length; i += 1) {
                    if (stringArray[i].length > 0) {
                        newUrl += '/' + stringArray[i];
                    }
                }
                return newUrl;
            },
            splitRoot = splitString(mstats.rootUrl),
            splitUrl = splitString(endpoint),
            result = '';
        
        if (!endpoint) {
            return '';
        }

        if (endpoint.indexOf(mstats.rootUrl || '') === 0) {
            return endpoint;
        }

        result = createUrl(result, splitRoot);
        result = createUrl(result, splitUrl);

        return result;
    };
    
    mstats.getBaseUrl = function() {
        return mstats.getRelativeEndpointUrl(dashboardUrl);
    };
    
     function buildFunction(widget, options) {
        var context = options[widget],
            fn;
        
        if(!context) {
            mstats.log('Attempted to create a helper for ' + widget + ' but the widget was not found in the options.');
            return;
        }
        
        fn = context[widget];
        return function() {
                    var result = fn.apply(context, arguments);
                    return result;
                };
    }
    
    // The preferred means of invoking public methods on a widget stored in options 
    // results in the awkward syntax: this.options.widget.widget('methodToInvoke')
    // Here we setup a set of helper methods to make the meaning of the code more
    // clear in general. Additionally, all of the complexity of invoking the widget
    // methods is located here, so there is less chance of errors at the site
    // where we consume this generated api.
    // The new syntax is: this._widget('methodToInvoke')
    mstats.setupWidgetInvocationMethods = function(host, options, widgetNames) {
        var i,
            widgetName;

        for (i = widgetNames.length - 1; i >= 0; i -= 1) {
            widgetName = widgetNames[i];
            host["_" + widgetName] = buildFunction(widgetName, options);
        }
    };

} (this.mstats = this.mstats || {}, jQuery));
