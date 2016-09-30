(function () {
    'use strict';
    app.factory('applicationLoadService', applicationLoadService);
    applicationLoadService.$inject = ['$rootScope', '$q', 'authenticateService', 'localStorageService', 'sharedAppEndpoints', '$state'];
    function applicationLoadService($rootScope, $q, authenticateService, localStorageService, sharedAppEndpoints, $state) {
        if ($rootScope.applicationLoadPromise === undefined) {
            $rootScope.applicationLoadPromise = authenticateService.tryUpdateToken();

            $rootScope.applicationLoadPromise.then(function (options) {
                if (options.loggedIn !== true) {
                    $state.go('login', {});
                }
            });
        }

        return $rootScope.applicationLoadPromise;
    }
})();