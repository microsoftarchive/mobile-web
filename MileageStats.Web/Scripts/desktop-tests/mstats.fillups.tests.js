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

(function ($) {
    module('Vehicle Fillups Widget', {
        setup: function () {
            this.savedAjax = $.ajax;
            $('#qunit-fixture').append('<div class="tab opened section" id="fillups-pane">' +
            	'<a class="trigger" href="/Fillup/List/1"></a>' +
            	'<div class="content">' +
            		'<div class="header">' +
            			'<form action="/Fillup/Add/1" method="get">' +
            				'<button class="button generic small" type="submit" data-action="fillup-add-selected">' +
            					'<img title="Add Fill up" alt="Add" src="/Content/button-add.png" />' +
            				'</button>' +
            			'</form>' +
            		'</div>' +
            		'<div class="aside">' +
                        '<div class="list nav">' +
                            '<a class="list-item selected" href="/Fillup/Details/1">' +
            					'<h1 class="date">1 Apr 2011</h1>' +
            					'<p class="vendor">Margie&apos;s Travel</p>' +
            					'<p class="total-cost">62.18</p>' +
            				'</a>' +
            				'<a class="list-item " href="/Fillup/Details/2">' +
            					'<h1 class="date">19 Mar 2011</h1>' +
            					'<p class="vendor">Adventure Works</p>' +
            					'<p class="total-cost">62.46</p>' +
            				'</a>' +
            			'</div>' +
            		'</div>' +
            	'</div>' +
            '</div>');
        },
        teardown: function () {
            $.ajax = this.savedAjax;
            $('#fillups-pane').fillups('destroy');
        }
    });

    /****************************************************************
    * Data Loading Tests
    ****************************************************************/
    test('while loading data, then the widget ensures the contents are hidden', function () {
        expect(1);
        var fillups = $('#fillups-pane').fillups({
            sendRequest: function (options) {
                ok($('.content:hidden').length > 0, 'contents are hidden');
                options.success({});
            }
        });

        // force a data refresh
        fillups.fillups('option', 'selectedVehicleId', 1);
    });

    test('when loading data is complete, then the contents are shown', function () {
        expect(1);
        var fillups = $('#fillups-pane').fillups({
            sendRequest: function (options) { options.success({}); }
        });

        // force a data refresh
        fillups.fillups('option', 'selectedVehicleId', 1);

        setTimeout(function () { // this allows the animation to finish
            ok($('.content:hidden').length === 0, 'contents are shown');
            start();
        }, 2500);

        stop();
    });

    test('when loading data errors out, then the widget ensures the contents are hidden', function () {
        expect(1);
        var fillups = $('#fillups-pane').fillups({
            sendRequest: function (options) {
                ok($('.content:hidden').length > 0, 'contents are hidden');
                options.error({});
            },
            templateId: 'testTemplate'
        });

        // force a data refresh
        fillups.fillups('option', 'selectedVehicleId', 1);
    });

    test('when loading data errors out, then triggers error status', function () {
        expect(2);
        var eventType = 'loadError',
            fillups = $('#fillups-pane').fillups({
                templateId: '#testTemplate',
                sendRequest: function (options) { options.error({}); },
                publish: function (event, status) {
                    if (status.type === eventType) {
                        ok(status, 'status object passed to publisher');
                        equal(status.type, eventType, 'status is of type : ' + eventType);
                    }
                }
            });

        // force a data refresh
        fillups.fillups('option', 'selectedVehicleId', 1);
    });

    test('when refreshData is called, then sendRequest is called', function () {
        expect(1);

        $('#fillups-pane').fillups({
            sendRequest: function (options) { options.success({}); }
        });

        $('#fillups-pane').fillups('option', 'sendRequest', function () {
            ok(true, 'sendRequest was called properly');
        });

        $('#fillups-pane').fillups('refreshData');
    });

    test('when requeryData is called, then cache invalidated', function () {
        expect(1);

        $('#fillups-pane').fillups({
            sendRequest: function (options) { options.success({}); },
            dataUrl: 'someEndpoint',
            invalidateData: function (endpoint) {
                equal(this.dataUrl, endpoint);
            }
        });

        $('#fillups-pane').fillups('requeryData');
    });

    test('when requeryData is called, then sendRequest called properly', function () {
        expect(1);

        $('#fillups-pane').fillups({
            sendRequest: function (options) { 
                ok(true, 'requeryData invoked sendRequest');
            }
        });

        $('#fillups-pane').fillups('requeryData');
    });

    /****************************************************************
    * Data Templating Tests
    ****************************************************************/
    module('Fillups Widget Template Tests', {
        setup: function () {
            $('#qunit-fixture').append(
                '<div id="fillups-pane" class="tab opened section"><div class="content"/></div>' +
                    '<script id="testTemplate" type="text/x-jquery-tmpl"><p>contents go here</p></script>' +
                    '<script id="testTemplate2" type="text/x-jquery-tmpl"><p>other contents</p></script>'
            );
            this.savedAjax = $.ajax;
        },
        teardown: function () {
            $.ajax = this.savedAjax;
            $('#fillups-pane').fillups('destroy');
        }
    });

    test('when selected vehicle id is set and sendRequest not specified, then widget calls $.ajax', function () {
        expect(1);

        $.ajax = function () {
            ok(true, '$.ajax was called');
        };

        var fillups = $('#fillups-pane').fillups();

        // force a data refresh
        fillups.fillups('option', 'selectedVehicleId', 1);
    });

    test('when widget data is retrieved, then widget contents are replaced by the template', function () {
        expect(1);

        var fillups = $('#fillups-pane').fillups({
            sendRequest: function (options) { options.success({}); },
            templateId: '#testTemplate'
        });

        // force a data refresh
        fillups.fillups('option', 'selectedVehicleId', 1);

        equal($.trim($('#fillups-pane').html()), $.trim($('#testTemplate').html().toLowerCase()), 'Template applied');
    });

    test('when refreshData is called, then template is re-applied', function () {
        expect(2);

        var fillups = $('#fillups-pane').fillups({
            sendRequest: function (options) { options.success({}); },
            templateId: '#testTemplate'
        });

        // force a data refresh
        fillups.fillups('option', 'selectedVehicleId', 1);

        equal($.trim($('#fillups-pane').html()), $.trim($('#testTemplate').html().toLowerCase()), 'Template applied');

        $('#fillups-pane').fillups('option', 'templateId', '#testTemplate2');
        $('#fillups-pane').fillups('refreshData');

        equal($.trim($('#fillups-pane').html()), $.trim($('#testTemplate2').html().toLowerCase()), 'Template applied');
    });

} (jQuery));
