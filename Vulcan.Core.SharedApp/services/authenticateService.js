(function () {
    'use strict';
    app.factory('authenticateService', authenticateService);
    authenticateService.$inject = ['$http', '$rootScope', '$q', 'localStorageService', 'jwtHelper', '$location', 'sharedAppEndpoints', '$interval'];
    function authenticateService($http, $rootScope, $q, localStorageService, jwtHelper, $location, sharedAppEndpoints, $interval) {
        var service = {};
        service.signout = signout;
        service.getToken = getToken;
        service.tryUpdateToken = tryUpdateToken;
        service.waitTillJobCompletes = waitTillJobCompletes;
        return service;

        function tryUpdateToken() {
            var deffered = $q.defer();

            var exitingToken = localStorageService.get("authorizationData");
            if (exitingToken !== undefined && exitingToken != null) {

                var data = "refresh_token=" + exitingToken.refreshToken + "&tenant_id=" + exitingToken.tenantId + "&grant_type=refresh_token&audience=VulcanAuthApi";

                $http.post(sharedAppEndpoints.authorizationServer + '/auth', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {
                    localStorageService.set('authorizationData',
                    {
                        token: response.access_token,
                        refreshToken: response.refresh_token,
                        useRefreshTokens: false,
                        expiretime: response.expires_in,
                        tenantId: exitingToken.tenantId
                    });
                    $http.defaults.headers.common['Authorization'] = 'bearer ' + response.access_token;
                    deffered.resolve({ loggedIn: true, token: response.access_token });
                }).error(function (err, status) {
                    deffered.resolve({ loggedIn: false });
                });
            } else {
                deffered.resolve({ loggedIn: false });
            }

            return deffered.promise;
        }

        function getToken(codeId, tenantId) {
            var data = "code=" + codeId + "&tenant_id=" + tenantId + "&grant_type=azureauth&audience=VulcanAuthApi";
            var deffered = $q.defer();
            $http.post(sharedAppEndpoints.authorizationServer + '/auth', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {
                localStorageService.set('authorizationData',
                {
                    token: response.access_token,
                    refreshToken: response.refresh_token,
                    useRefreshTokens: false,
                    expiretime: response.expires_in,
                    tenantId: tenantId
                });
                $http.defaults.headers.common['Authorization'] = 'bearer ' + response.access_token;
                deffered.resolve(response);
            }).error(function (err, status) {
                signout();
                deffered.reject(err);
            });
            return deffered.promise;
        };

        function waitTillJobCompletes(tenantId, jobKey) {
            var deffered = $q.defer();

            var stop = $interval(function () {
                $http.post(sharedAppEndpoints.authorizationServer + "/azureauth/status?tenantId=" + tenantId + "&jobKey=" + jobKey).success(function (response) {
                    $interval.cancel(stop);

                    getToken(response.code, response.tenant_id).then(function (token) {
                        deffered.resolve(token);
                    }, function () {
                        deffered.reject();
                    });
                });
            }, 1000 * 30);

            return deffered.promise;
        }

        //signout
        function signout() {
            localStorageService.remove('authorizationData');
        }

        function handleSuccess(res) {
            return res;
        }

        function handleError(error) {
            return function () {
                return { success: false, message: error };
            };
        }
    }
})();