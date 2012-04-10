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

    module('Tile tests', {
        setup: function () {
            $('#qunit-fixture').append('<div id="vehicle-list-contents"><div class="wrapper" /></div>');
        }
    });

    test('when moveTo is called, then gets moved to specified position and callback is called', function () {
        expect(2);
        var v = $('#vehicle-list-contents > div').tile();
        v.tile('moveTo', {
            position: { top: 50, left: 50 },
            duration: 500
        }, function () {
            equal($('.wrapper').css('top'), '50px', 'top has been set');
            equal($('.wrapper').css('left'), '50px', 'left has been set');
            start();
        });
        stop();
    });

    test('when moveTo is called with an array of animation infos, then gets moved to specified position', function () {
        expect(2);
        var v = $('#vehicle-list-contents > div').tile();
        v.tile('moveTo', [{
            position: { top: 50, left: 50 },
            duration: 10
        }, {
            position: { top: 150, left: 150 },
            duration: 10
        }], function () {
            equal($('.wrapper').css('top'), '150px', 'top has been set');
            equal($('.wrapper').css('left'), '150px', 'left has been set');
            start();
        });
        stop();
    });

}(jQuery));