(function () {
    'use strict';
    angular
        .module('sharedPortalApp')
        .controller('groupController', groupController);

    groupController.$inject = ['$q', '$interval', 'groupService', '$scope', '$location', '$state', '$stateParams'];
    function groupController($q, $timeout, groupService, $scope, $location, $state, $stateParams) {

        $scope.readonly = false;
        $scope.selectedItem = null;
        $scope.searchText = null;
        $scope.querySearch = querySearch;
        $scope.querySearchUsers = querySearchUsers;
        $scope.roles = loadRoles();
        $scope.group = { selectedRoles: [], selectedUsers: [] };

        if ($stateParams.id !== undefined || $stateParams.id != null) {
            groupService.getGroup($stateParams.id).then(function (state) {
                $scope.group = state.data;
            });
        }

        $scope.addGroups = function () {
            $state.go('addGroups');
        }

        $scope.getSelectedChipIndex = function (event, chip) {
        }

        function querySearch(query) {
            var results = query ? $scope.allroles.filter(createFilterFor(query)) : [];
            return results;
        }

        function querySearchUsers(queryUser) {
            var deferred = $q.defer();
            groupService.getUsers(queryUser).then(function (state) {
                deferred.resolve(state.data);
            });

            return deferred.promise;
        }

        function createFilterFor(query) {
            var lowercaseQuery = angular.lowercase(query);

            return function filterFn(role) {
                return (role._lowername.indexOf(lowercaseQuery) === 0);
            };
        }

        function loadRoles() {
            groupService.getRoles().then(function (state) {
                $scope.allroles = state.data;
                return $scope.allroles.map(function (rl) {
                    rl._lowername = rl.name.toLowerCase();
                    return rl;
                });
            });
        }

        groupService.getGroups().then(function (state) {
            $scope.allgroups = state.data;
        });

        $scope.deleteGroup = function (id) {
            groupService.deleteGroup(id).then(function (state) {
                $scope.result = state.data;
                $state.go('dashboard');
            });
        }

        $scope.addGroup = function () {
            $scope.group.audienceid = 1;
            groupService.addGroup($scope.group).then(function (state) {
                $scope.result = state.data;
                $state.go('dashboard');
            });
        }

        $scope.gotoEditGroup = function (gid) {
            $state.go('editGroups', { id: gid });
        }

        $scope.saveEdited = function () {
            groupService.editGroup($scope.group).then(function (state) {
                $scope.result = state;
                $state.go('groups');
            });
            
        }
    }
})();
