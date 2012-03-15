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
    var ctr = function (require) {

        var rootUrl = require('rootUrl'),
            $ = require('$');

        var urlPattern = '/Vehicle/:vehicleId/Details';

        //TODO: refactor copied code
        function nextUrl(params) {
            var p;
            for (p in params) {
                urlPattern = urlPattern.replace(':' + p, params[p]);
            }
            return urlPattern;
        }

        //TODO: refactor copied code
        function validate(form) {
            // here we look for the validation object that has been
            // attached to the form. this assumes the present of 
            // jQuery validation and MVC's unobtrusive validation scripts.
            var validationInfo = $(form).data('unobtrusiveValidation');
            return !validationInfo || !validationInfo.validate || validationInfo.validate();
        }

        //TODO: refactor copied code
        function onSuccess() {
            return function (res, status, xhr) {
                if (res.Errors) {
                    displayErrors(res.Errors);
                } else {
                    window.location.hash = nextUrl({ vehicleId: res.Model.Vehicle.VehicleId });
                }
            };
        }

        function postrender(model, el, context) {

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
                    success: onSuccess()
                });
                return false;
            });

            el.find('#DeleteVehicleButton').click(function (event) {
                var answer = confirm("Are you sure you want to delete this vehicle and all of it's related data?");
                if (!answer) {
                    event.preventDefault();
                }

            });

            var $vehicleEditForm = el.find('#vehicleEditForm'),
                $yearSelect = el.find('#Year', $vehicleEditForm),
                $makeSelect = el.find('#MakeName', $vehicleEditForm),
                $modelSelect = el.find('#ModelName', $vehicleEditForm),
                makesUrl = $vehicleEditForm.data('makes-url'),
                modelsUrl = $vehicleEditForm.data('models-url');

            $yearSelect.change(function () {
                $.post(makesUrl,
                { year: $yearSelect.val() },
                function (data) {
                    $makeSelect.children().not(':first').remove();
                    $modelSelect.children().not(':first').remove();
                    updateList(data, $makeSelect);
                });
            });

            $makeSelect.change(function () {
                $.post(modelsUrl,
                { year: $yearSelect.val(), make: $makeSelect.val() },
                function (data) {
                    $modelSelect.children().not(':first').remove();
                    updateList(data, $modelSelect);
                });
            });
        };

        return {
            postrender: postrender
        };

        function updateList(data, $selectList) {
            $.each(data,
              function (key, value) {
                  $selectList.append(
                    $('<option></option>')
                    .attr('value', value)
                    .text(value)
                    );
              });
        };
    };
    mstats.vehicleEdit = ctr;
    mstats.vehicleAdd = ctr;
} (this.mstats = this.mstats || {}));
