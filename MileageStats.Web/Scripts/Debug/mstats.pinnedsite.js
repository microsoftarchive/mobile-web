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
    var sendRequest;
    
    function showCallout () {
        $('#pinnedSiteCallout').show();
    }
    
    function hideCallout() {
        $('#pinnedSiteCallout').hide();
    }
    
    mstats.pinnedSite = {
        initializePinnedSiteImage: function () {
            
            // Do not enabled site pinning for non-Internet Explorer 9+ browsers
            // Do not show the callout if the site is already pinned
            if (!(!document.documentMode || this.isPinned())) {
                $('#pinnedSiteImage')
                    .bind('mousedown mouseout', hideCallout)
                    .bind('mouseover', showCallout)
                    .addClass('active');
                $('#pinnedSiteCallout').show();
                setTimeout(hideCallout, 5000);
            }
        },
        
        intializeData: function (sendRequestFunc) {
            sendRequest = sendRequestFunc;
            this.requeryJumpList();
        },
        
        requeryJumpList: function () {
            var getRelativeUrl = mstats.getRelativeEndpointUrl;
            
            try {
                if (this.isPinned()) {

                    sendRequest({
                        url: '/reminder/overduelist/',
                        contentType: 'application/json',
                        cache: false,
                        success: function (data) {

                            try {
                                var g_ext = window.external,
                                    faviconUrl = getRelativeUrl('/favicon.ico'),
                                    iconOverlayUrl,
                                    iconOverlayMessage,
                                    numReminders = data.Reminders.length,
                                    reminderUrl,
                                    reminder,
                                    i;
                                
                                g_ext.msSiteModeClearJumpList();
                                g_ext.msSiteModeCreateJumpList("Reminders");
                                g_ext.msSiteModeClearIconOverlay();

                                if (data.Reminders) {
                                    for (i = 0; i < numReminders; i += 1) {
                                        reminder = data.Reminders[i];
                                        reminderUrl = getRelativeUrl('/reminder/details/' + reminder.Reminder.ReminderId.toString());
                                        g_ext.msSiteModeAddJumpListItem(reminder.FullTitle, reminderUrl, faviconUrl, "self");
                                    }

                                    if (numReminders > 0) {
                                        iconOverlayUrl = '/content/overlay-' + numReminders + '.ico';
                                        iconOverlayMessage = 'You have ' + numReminders.toString() + ' maintenance tasks that are ready to be accomplished.';
                                        if (numReminders > 3) {
                                            iconOverlayUrl = '/content/overlay-3plus.ico';
                                        }
                                        g_ext.msSiteModeSetIconOverlay(getRelativeUrl(iconOverlayUrl), iconOverlayMessage);
                                    }
                                }

                                g_ext.msSiteModeShowJumpList();
                            }
                            catch (e) {
                                // Fail silently. Pinned Site API not supported.
                            }
                        }
                    });
                }
            }
            catch (e) {
                // Fail silently. Pinned Site API not supported.
            }
        },
        
        isPinned: function () {
            try {
                // we have to use try/catch because checking for the presence
                // of msIsSiteMode explicitly does not work
                return window.external.msIsSiteMode();
            }
            catch (e) {
                return false;
            }
        }
    };

}(this.mstats = this.mstats || {}, jQuery));