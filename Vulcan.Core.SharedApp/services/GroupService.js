(function () {
    'use strict';
    app.factory('groupService', groupService);
    groupService.$inject = ['$http', 'sharedAppEndpoints'];
    function groupService($http, sharedAppEndpoints) {
        var service = {};
        service.getRoles = getRoles;
        service.getUsers = getUsers;
        service.addGroup = addGroup;
        service.deleteGroup = deleteGroup;
        service.getGroups = getGroups;
        service.getGroup = getGroup;
        service.editGroup = editGroup;
        
        return service;
        function getGroups() {
            return $http.get(sharedAppEndpoints.webApi + '/v1/groups').then(handleSuccess, handleError('Error getting all groups'));
        }

        function getRoles() {
            return $http.get(sharedAppEndpoints.webApi + '/v1/roles').then(handleSuccess, handleError('Error getting all roles'));
        }

        function getUsers(searchtext) {
            
            return $http.get(sharedAppEndpoints.webApi + '/v1/users?searchtext=' + searchtext).then(handleSuccess, handleError('Error searching User'));
        }

        function getGroup(id) {
            return $http.get(sharedAppEndpoints.webApi + '/v1/groups/' + id).then(handleSuccess, handleError('Error getting Single Group'));
        }

        function addGroup(group) {
            return $http({
                url: sharedAppEndpoints.webApi + '/v1/groups',
                method: "POST",
                data: group,
                headers: { 'Content-Type': 'application/json' }
            }).then(handleSuccess, handleError('Error adding group'));
        }

        function deleteGroup(id) {
            return $http.delete(sharedAppEndpoints.webApi + '/v1/groups/' + id).then(handleSuccess, handleError('Error deleting group'));
        }

        function editGroup(group) {
            return $http({
                url: sharedAppEndpoints.webApi + '/v1/groups/' + group.id,
                method: "PUT",
                data: group,
                headers: { 'Content-Type': 'application/json' }
            }).then(handleSuccess, handleError('Error in editing group'));

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