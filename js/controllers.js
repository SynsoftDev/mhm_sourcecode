'use strict';
/**
	Controllers common for all pages (Logout).
*/
angular.module('mhmApp.controllers', [])	
.controller('LogOutController', ['$scope', 'deleteCreds', '$location', '$http',
function LogOutCtrl($scope, deleteCreds, $location, $http) { 
	console.log('@LogOutCtrl');       
	deleteCreds();			
	$http.defaults.headers.common['Authorization'] = 'bearer ';	
	$scope.loggedIn = false;
	window.location.href = "login";
}]);
	