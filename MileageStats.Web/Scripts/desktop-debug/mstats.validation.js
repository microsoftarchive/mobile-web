/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED ON AN *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

(function ($) {
    // The textlineinput validator function
    $.validator.addMethod('textlineinput', function (value, element, pattern) {
        if (!value) {
            return true; // not testing 'is required' here!
        }
        try {
            var match = value.match(pattern);
            return (match && (match.index === 0) && (match[0].length === value.length));
        } catch (e) {
            return false;
        }
    });

    // The adapter to support ASP.NET MVC unobtrusive validation
    $.validator.unobtrusive.adapters.add('textlineinput', ['pattern'], function (options) {
        options.rules.textlineinput = options.params.pattern;
        if (options.message) {
            options.messages.textlineinput = options.message;
        }
    });
    $.validator.unobtrusive.adapters.addSingleVal("textlineinput", "pattern");


    // the postalcode validator.  
    $.validator.addMethod('postalcode', function (value, element, params) {
        if (!value) {
            return true; // not testing 'is required' here!
        }
        try {
            var country = $('#Country').val(),
                postalCode = $('#PostalCode').val(),
                usMatch = postalCode.match(params.unitedStatesPattern),
                internationalMatch = postalCode.match(params.internationalPattern),
                message = '',
                match;

            if (country.toLowerCase() === 'united states') {
                message = params.unitedStatesErrorMessage;
                match = usMatch;
            } else {
                message = params.internationalErrorMessage;
                match = internationalMatch;
            }

            $.extend($.validator.messages, {
                postalcode: message
            });

            return (match && (match.index === 0) && (match[0].length === postalCode.length));
        } catch (e) {
            return false;
        }
    });

    // The adapter to support ASP.NET MVC unobtrusive validation
    $.validator.unobtrusive.adapters.add(
        'postalcode',
        ['internationalErrorMessage', 'unitedStatesErrorMessage', 'internationalPattern', 'unitedStatesPattern'],
        function (options) {
            options.rules.postalcode = options.params;
        }
    );

}(jQuery));