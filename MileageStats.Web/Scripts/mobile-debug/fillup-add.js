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

(window.mstats = window.mstats || {}).fillupAdd = function (require) {

    var rootUrl = require('rootUrl'),
        $ = require('$');

    var urlPattern = '/Vehicle/:vehicleId/Fillup/List';

//    var ev = new $.Event('removeClass'),
//        orig = $.fn.removeClass;
//    $.fn.removeClass = function () {
//        $(this).trigger(ev);
//        return orig.apply(this, arguments);
//    };

    function makeRelativeToRoot(url) {
        return (rootUrl + url).replace('//', '/');
    }

    function displayErrors(errors) {
        var item, el;
        var errorList;
        var msg;

        for (item in errors) {
            el = $('[data-valmsg-for="' + item + '"]');
            el.parent('li').addClass('validation-error');
            errorList = errors[item];
            msg = '';
            for (var i = 0; i < errorList.length; i++) {
                msg = msg + errorList[i];
            }
            el.html(msg);
        }
    }

    function onSuccess(next) {
        return function(res, status, xhr) {
            if (res.Errors) {
                displayErrors(res.Errors);
            } else {
                window.location.hash = next;
            }
        };
    }

    function validate(form) {
        // here we look for the validation object that has been
        // attached to the form. this assumes the present of 
        // jQuery validation and MVC's unobtrusive validation scripts.
        var validationInfo = $(form).data('unobtrusiveValidation');
        return !validationInfo || !validationInfo.validate || validationInfo.validate();
    }

    function nextUrl(params) {
        var p;
        for (p in params) {
            urlPattern = urlPattern.replace(':' + p, params[p]);
        }
        return urlPattern;
    }

    function postrender(model, el, context) {
        var form = el.find('form'),
            action = form.attr('action'),
            next = nextUrl(context.params);

        form.find('span[data-valmsg-for]').on('removeClass', function () {
            form.find('li').has('.field-validation-valid').removeClass('validation-error');
        });

        $.validator.unobtrusive.parse(form);

        form.submit(function (evt) {

            evt.preventDefault();

            if (!validate(this)) {
                // the following requires over
                // form.find('li').has('.field-validation-error').addClass('validation-error');
                return;
            }

            var input = form.serialize();

            $.ajax({
                dataType: 'json',
                data: input,
                type: 'POST',
                url: makeRelativeToRoot(action),
                success: onSuccess(next)
            });
            return false;
        });
    }

    return {
        fetch: false,
        postrender: postrender
    };

};