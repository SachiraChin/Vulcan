(function () {
    'use strict';

    angular
        .module('sharedPortalApp')
        .factory('organizationService', organizationService);

    organizationService.$inject = ['$http', 'sharedAppEndpoints'];

    function organizationService($http, sharedAppEndpoints) {
        
        var service = {};
        service.setOrganization = setOrganization;
        service.updateOrganization = updateOrganization;
        service.getOrganization = getOrganization;
        service.getTimeZones = getTimeZones;
        return service;

        function getTimeZones() {
            return $http.get(sharedAppEndpoints.webApi + "/v1/timezones").then(handleSuccess, handleError('Error in getting timezone'));
        }

        function getOrganization() {
            return $http.get(sharedAppEndpoints.webApi + "/v1/organizations").then(handleSuccess, handleError('Error in getting organization'));
        }

        function setOrganization(details) {
            return $http({
                url: sharedAppEndpoints.webApi + "/v1/organizations",
                method: "POST",
                data: details,
                headers: { 'Content-Type': 'application/json' }
            }).then(handleSuccess, handleError('Error in setting the app'));
        }

        function updateOrganization(organization) {
            return $http({
                url: sharedAppEndpoints.webApi + "/v1/organizations",
                method: "PUT",
                data: organization,
                headers: { 'Content-Type': 'application/json' }
            }).then(handleSuccess, handleError('Error in updating organization details'));
        }

        function handleSuccess(res) {
            return res.data;
        }

        function handleError(error) {
            return function () {
                return { success: false, message: error };
            };
        }

    }
})();