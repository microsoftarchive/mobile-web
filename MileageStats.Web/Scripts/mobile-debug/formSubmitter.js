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

(function (mstats) {

    mstats.formSubmitter = function (require) {

        var $ = require('$');

        function validate(form) {
            // here we look for the validation object that has been
            // attached to the form. this assumes the present of 
            // jQuery validation and MVC's unobtrusive validation scripts.
            var validationInfo = $(form).data('unobtrusiveValidation');
            return !validationInfo || !validationInfo.validate || validationInfo.validate();
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

        function onSuccess(done) {
            return function (res, status, xhr) {
                if (res.Errors) {
                    displayErrors(res.Errors);
                } else {
                    done(res);
                }
            };
        }

        function attachFormSubmission(el, done) {

            var form = el.find('form').first(),
                action = form.attr('action');

            $.validator.unobtrusive.parse(form);

            form.submit(function (evt) {

                evt.preventDefault();

                if (!validate(this)) {
                    return;
                }

                var input = form.serialize();

                $.ajax({
                    dataType: 'json',
                    data: input,
                    type: 'POST',
                    url: action,
                    success: onSuccess(done)
                });
                return false;
            });
        };

        return {
            attach: attachFormSubmission
        };

    };
} (this.mstats = this.mstats || {}));
