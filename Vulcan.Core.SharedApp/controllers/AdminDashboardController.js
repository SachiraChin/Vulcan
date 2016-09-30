(function () {
    'use strict';
    angular
        .module('sharedPortalApp')
        .controller('adminDashboardController', adminDashboardController);

    adminDashboardController.$inject = ['$scope', '$location', '$state'];
    function adminDashboardController($scope, $location, $state) {
        
        $scope.editGroup = function () {
            $state.go('groups');
        }
        $scope.editOrganizationDetails = function () {
            $state.go('organizationconfig');
        }
    }

})();