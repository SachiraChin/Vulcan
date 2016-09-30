(function () {
    'use strict';

    angular
        .module('sharedPortalApp')
        .controller('organizationConfigController', organizationConfigController);

    organizationConfigController.$inject = ['$location', '$scope', 'organizationService','$state'];

    function organizationConfigController($location, $scope, organizationService, $state) {

        organizationService.getTimeZones().then(function (state) {
            $scope.TimeZones = state;
        });

        organizationService.getOrganization().then(function (state) {
            $scope.organization = state;
        });

        $scope.setOrgDetail = function () {
            var details = $scope.organization;
            organizationService.setOrganization(details).then(function (state) {
                $state.go('dashboard');
            });
        }

        $scope.EditOrgDetail = function () {
            var details = $scope.organization;
            organizationService.updateOrganization(details).then(function (state) {
                $state.go('dashboard');
            });
        }

    }
})();
