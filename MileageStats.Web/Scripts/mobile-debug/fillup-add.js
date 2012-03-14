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

(window.mstats = window.mstats || {}).vehicleFillupAdd = function (require) {

    var rootUrl = require('rootUrl'),
        $ = require('$');

    var next = '#/Vehicle/' + 1 + '/Fillup/List';

    var ev = new $.Event('removeClass'),
        orig = $.fn.removeClass;
    $.fn.removeClass = function() {
        $(this).trigger(ev);
        return orig.apply(this, arguments);
    };

    function makeRelativeToRoot(url) {
        return (rootUrl + url).replace('//', '/');
    }

    function displayErrors(errors) {
        var item, el;
        for (item in errors) {
            el = $('form #' + item).parent('li');
            el.addClass('validation-error');
            el.find('label').after('<span class="validation-error">' + errors[item] + '</span>');
        }
    }

    function clearErrors() {
        $('span.validation-error').remove();
        $('.validation-error').removeClass('validation-error');
    }

    function success(res, status, xhr) {
        if (res.Errors) {
            displayErrors(res.Errors);
        } else if (!res.Model) {
            // success!
            window.location.hash = next;
        } else {
            // render errors
        }
    }

    function error(xhr, status, errorThrown) {
        debugger;
    }

    var data_validation = 'unobtrusiveValidation';

    function validate(form) {
        var validationInfo = $(form).data(data_validation);
        return !validationInfo || !validationInfo.validate || validationInfo.validate();
    }

    function postrender(model, el) {
        var form = el.find('form'),
            action = form.attr('action');

        form.find('span[data-valmsg-for]').on('removeClass',function (a, b) {
            form.find('li').has('.field-validation-valid').removeClass('validation-error');
        });

        $.validator.unobtrusive.parse(form);

        form.submit(function (evt) {
            //                    clearErrors();
            evt.preventDefault();
            if (!validate(this)) {
                form.find('li').has('.field-validation-error').addClass('validation-error');
                return;
            }

            var input = form.serialize();

            $.ajax({
                dataType: 'json',
                data: input,
                type: 'POST',
                url: makeRelativeToRoot(action),
                success: success,
                error: error
            });
            return false;
        });
    }

    return {
        fetch: false,
        postrender: postrender
    };

};