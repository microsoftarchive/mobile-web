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

$(function () {
    var charts,
        header,
        infoPane,
        summaryPane,
        vehicleList;
    
    // setup default error handler for redirects due to session timeout.
    $(document).ajaxError(function (ev, jqXHR, settings, errorThrown) {
        if (jqXHR.status === 200) {
          if (jqXHR.responseText.indexOf('Mileage Stats Sign In') !== -1) {
            window.location.replace(mstats.getRelativeEndpointUrl('/Auth/SignIn'));
          } else if (jqXHR.responseText.indexOf('Mileage Stats | Accident!') !== -1) {
            window.location.replace(mstats.getRelativeEndpointUrl('/GenericError.htm'));
          }
        }
    });

    $('#notification').status({
        subscribe: mstats.pubsub.subscribe
    });
    
    header = $('#header').header();
    
    mstats.pinnedSite.intializeData(mstats.dataManager.sendRequest);

    if (!window.mileageStatsOnDashboard) {
        return; // only enable widgets on the dashboard
    }

    vehicleList = $('#vehicles').vehicleList({
        // This allows the vehicleList to participate
        // in global messaging with other widgets
        publish: mstats.pubsub.publish,

        // This overrides the vehicleLists default ($.ajax) 
        // way of getting data so we can inject a caching layer
        sendRequest: mstats.dataManager.sendRequest,

        // this allows the vehicleList to invalidate data
        // stored in the data cache.  
        invalidateData: mstats.dataManager.resetData,

        // the ID of the element containing the vehicle list template
        templateId: '#mstats-vehicle-list-template'
    });

    infoPane = $('#info').infoPane({
        sendRequest: mstats.dataManager.sendRequest,
        invalidateData: mstats.dataManager.resetData,
        publish: mstats.pubsub.publish, 
        header: header
    });
    
    summaryPane = $('#summary').summaryPane({
        sendRequest: mstats.dataManager.sendRequest,
        invalidateData: mstats.dataManager.resetData,
        publish: mstats.pubsub.publish,
        header: header
    });
    
    charts = $('#main-chart').charts({
        sendRequest: mstats.dataManager.sendRequest,
        invalidateData: mstats.dataManager.resetData
    });
    
    $('body').layoutManager({
        subscribe: mstats.pubsub.subscribe,
        pinnedSite: mstats.pinnedSite,
        charts: charts,
        header: header,
        infoPane: infoPane,
        summaryPane: summaryPane,
        vehicleList: vehicleList
       });
});