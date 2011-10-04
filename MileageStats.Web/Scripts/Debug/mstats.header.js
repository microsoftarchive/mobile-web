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

    $.widget('mstats.header', {

        options: {
            title: '',
            displayName: ''
        },
            
        _create: function () {
            this._adjustNavigation();
        },

        _adjustNavigation: function () {
            var state = $.bbq.getState() || {  },
                url = this.element.data('url'),
                newUrlBase = mstats.getRelativeEndpointUrl(url);

            state.layout = 'dashboard';
            this.element.find('#dashboard-link').attr('href', $.param.fragment(newUrlBase, state));

            state.layout = 'charts';
            this.element.find('#charts-link').attr('href', $.param.fragment(newUrlBase, state));
        },

        _setOption: function (key, value) {
            switch (key) {
                case 'title':
                    this.element.find('[data-title]').text(value);
                    break;
                case 'displayName':
                    this.element.find('[data-display-name]').text(value);
                    break;
            }
            $.Widget.prototype._setOption.apply(this, arguments);
        }
    });

}(this.mstats, jQuery));