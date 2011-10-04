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

    module('Header Widget Tests', {
        setup: function () {
            $('#qunit-fixture').append('<div class="header" id="header" data-url="/">' +
                '<div><div><h1 data-title>Dashboard</h1>' +
                    '<div id="notification"></div>' +
                        '<div class="nav">' +
                            '<span id="welcome">Welcome <span data-display-name>Sample User</span></span>' +
                            '[ <a id="dashboard-link" href="/">Dashboard</a>' +        
                            '| <a id="charts-link" href="/Chart/List">Charts</a>' +
                            '| <a id="profile-link" href="/Profile/Edit">Profile</a>' +
                            '| <a id="login-link" href="/Auth/SignOut">Sign Out</a> ]' +
                        '</div></div>' +
                    '</div>' +
                '</div>'
            );
        }
    });

    test('when widget is attached to the #header element, then dashboard and chart links is changed to update the URL hash', function () {
        expect(2);

        $('#header').header();

        var link = $('#dashboard-link').attr('href'),
            rootUrl = link.substring(0, link.indexOf('#')),
            querystring = $.param.fragment(link),
            state = $.deparam.querystring(querystring),
            layoutHash = state.layout,
            vehicledIdHash = state.vid;

        equal(rootUrl, '/', 'base url updated so no redirect occurs');
        equal(layoutHash, 'dashboard', 'screen set properly');
    });

    test('when title option is changed, then it displays new title', function() {
        expect(1);
        var header = $('#header').header();
        header.header('option', 'title', 'test title');
        
        equal($('[data-title]').text(), 'test title', 'header text set properly');
    });

}(jQuery));