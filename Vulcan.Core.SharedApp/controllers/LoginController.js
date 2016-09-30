(function () {
    'use strict';

    angular
        .module('sharedPortalApp')
        .controller('loginController', loginController);

    loginController.$inject = ['$location', '$state', '$scope', 'authenticateService', 'organizationService', '$stateParams', 'sharedAppEndpoints'];

    function loginController($location, $state, $scope, authenticateService, organizationService, $stateParams, sharedAppEndpoints) {
        $scope.authApiPath = sharedAppEndpoints.authorizationServer;
        $scope.redirectPath = encodeURIComponent(sharedAppEndpoints.applicationPath + "/#login");
        $scope.showLoginButtons = false;
        $scope.isWaitingForJob = false;
        var code = $stateParams.code;
        var tenantId = $stateParams.tenant_id;
        var jobKey = $stateParams.jobKey;
        if (code == null && tenantId == null && jobKey == null) {
            authenticateService.tryUpdateToken().then(function (loginOptions) {

                if (loginOptions.loggedIn === true) {
                    gotoInitialPage();
                } else {
                    $scope.showLoginButtons = true;
                }

            }, function () {
                $scope.showLoginButtons = true;
            });

        } else if (code != null && tenantId != null) {
            authenticateService.getToken(code, tenantId).then(function (state) {
                gotoInitialPage();
            }, function () {
                $scope.showLoginButtons = true;
            });
        }
        else if (jobKey != null && tenantId != null) {
            $scope.isWaitingForJob = true;
            authenticateService.waitTillJobCompletes(tenantId, jobKey).then(function (state) {
                gotoInitialPage();
            }, function () {
                $scope.showLoginButtons = true;
            });
        }

        function gotoInitialPage() {
            organizationService.getOrganization()
                .then(function (state) {
                    if (state == undefined) {
                        $state.go('organizationconfig');
                    } else {
                        $state.go('dashboard');
                    }
                });
        }
    }
})();
