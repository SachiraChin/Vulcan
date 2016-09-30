var app = angular.module('sharedPortalApp', [
  'ui.router', 'ngMaterial', 'ngMessages', 'LocalStorageModule', 'angular-jwt'
]);

app.constant('sharedAppEndpoints', {
    authorizationServer: 'https://localhost:44300',
    webApi: 'http://localhost:47866',
    applicationPath: 'https://localhost:44377',
    //authorizationServer: 'https://Vulcancore.azurewebsites.net/api',
    //webApi: 'https://Vulcancore.azurewebsites.net/api',
    //applicationPath: 'https://Vulcancore.azurewebsites.net'
});

app.config(function ($stateProvider, $urlRouterProvider) {
    $urlRouterProvider.otherwise("/login");
    $stateProvider
        .state('login',
        {
            url: "/login?code&tenant_id&jobKey",
            templateUrl: 'views/loginPage.html',
            controller: 'loginController',
        })
        .state('organizationconfig',
        {
            url: "/organizationconfig",
            templateUrl: 'views/configuration/organizationConfig.html',
            controller: 'organizationConfigController',
            resolve: {
                loginOptions: 'applicationLoadService'
            }
        })
        .state('dashboard',
        {
            url: "/dashboard",
            templateUrl: 'views/admin/adminDashboard.html',
            controller: 'adminDashboardController',
            resolve: {
                loginOptions: 'applicationLoadService'
            }
        })
        .state('addGroups',
        {
            url: "/addGroups",
            templateUrl: 'views/admin/addGroup.html',
            controller: 'groupController',
            resolve: {
                loginOptions: 'applicationLoadService'
            }
        })
        .state('editGroups',
        {
            url: "/editGroups/:id",
            templateUrl: 'views/admin/editGroup.html',
            controller: 'groupController',
            resolve: {
                loginOptions: 'applicationLoadService'
            }
        })
        .state('groups',
        {
            url: "/groups",
            templateUrl: 'views/admin/groups.html',
            controller: 'groupController',
            resolve: {
                loginOptions: 'applicationLoadService'
            }
        });
});

app.config(function ($mdThemingProvider) {
    $mdThemingProvider.theme('default')
      .primaryPalette('indigo', {
          'default': '400', 
          'hue-1': '100',
          'hue-2': '600', 
          'hue-3': 'A100' 
      })
      .accentPalette('purple', {
          'default': '200' 
      });
});
