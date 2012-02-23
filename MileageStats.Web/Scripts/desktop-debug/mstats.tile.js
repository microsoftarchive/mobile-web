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

    $.widget('mstats.tile', {

        // takes an array of animationInfo objects that looks like:
        // {position : {top: 10, left: 20}, duration : 400 };
        // or a single animationInfo object
        moveTo: function (animationInfoArray, callback) {
            var that = this,
                arrayLength = 0;

            this._unlock();

            // if this is an array, iterate over it
            if (animationInfoArray.length) {
                arrayLength = animationInfoArray.length;
                $.each(animationInfoArray, function (index, info) {
                    if (index === arrayLength - 1) {
                        that._animate(info, callback);
                    } else {
                        that._animate(info);
                    }
                });
            }

            // otherwise just animate one step with the data
            else {
                this._animate(animationInfoArray, callback);
            }
        },

        // takes an animationInfo object that looks like:
        // {position : {top: 10, left: 20}, duration : 400 };
        _animate: function (animationInfo, callback) {
            this.element.animate(
                animationInfo.position, {
                    duration: animationInfo.duration,
                    complete: function () {
                        if (callback) {
                            callback();
                        }
                }
            });
        },

        _unlock: function () {
            this.element.css({
                position: 'absolute',
                'float': 'none'
            });
        },

        beginAnimation: function () {
            var $element = this.element;
            $element.css({
                top: $element.position().top + 'px',
                left: $element.position().left + 'px'
            });
        },

        endAnimation: function () {
            this.element
                .attr('style', '')
                .css({
                    top: 0,
                    left: 0,
                    position: 'relative',
                    'float': 'left'
                });
        }
    });

}(jQuery));