'use strict';
/**
	Declare app level module which depends on filters, and services 
*/
angular.module('mhmApp.cases', []);
angular.module('mhmApp.users', []);
angular.module('mhmApp.benefits', []);
angular.module('mhmApp.planattribute', []);
angular.module('mhmApp.planbenefit', []);
angular.module('mhmApp.csrrate', []);
angular.module('mhmApp.ratingarea', []);
angular.module('mhmApp.zipcode', []);
angular.module('mhmApp.county', []);
angular.module('mhmApp.job', []);
angular.module('mhmApp.employermaster', []);
angular.module('mhmApp.issuermaster', []);
angular.module('mhmApp', [	
	'mhmApp.cases',
	'mhmApp.users',
	'mhmApp.benefits',
	'mhmApp.planattribute',
	'mhmApp.planbenefit',
	'mhmApp.csrrate',
	'ngRoute',
	'angularjs-dropdown-multiselect',
	'ngCookies',
	'mhmApp.filters',
	'mhmApp.messages',
	'mhmApp.businessServices',
	'mhmApp.directives',
	'mhmApp.controllers',
	'ui.bootstrap',	
	'ngDialog',
	/****
		Created By : Aastha Jain
		Created Date : 21-07-2016
		Start : Libs for HTML Editor.
	****/
	'textAngular',
	/**** End : Libs for HTML Editor. ****/
	/****
		Created By : Aastha Jain
		Created Date : 13-06-2016
		Start : Add 3 new masters (Rating Area, Zip Code, Country).
	****/
	'mhmApp.ratingarea',
	'mhmApp.zipcode',
	'mhmApp.county',
	/**** End : Add 3 new masters (Rating Area, Zip Code, Country). ****/
	
	/****
		Created By : Aastha Jain
		Created Date : 02-07-2016
		Start : Add new masters (Job Master).
	****/
	'mhmApp.job',
	/**** End : Add new masters (Job Master). ****/
	'mhmApp.employermaster',
	'mhmApp.issuermaster',
]).
/** Set Controller routes and templates. */	
config(['$routeProvider','$locationProvider', function($routeProvider,$locationProvider) {
	$routeProvider
        .when('/login', {
            controller: 'LoginController',
            templateUrl: 'partials/users/login.html'
        })	
		.when('/generateCasePDF/:id', {
            controller: 'PDFGenerator',
            templateUrl: 'partials/pdfGenerator/pdf.html'
        })
		.when('/set-password/:id', {
            controller: 'SetPasswordController',
            templateUrl: 'partials/users/set-password.html'
        }) 
		.when('/change-password', {
            controller: 'ChangePasswordController',
            templateUrl: 'partials/users/change-password.html'
        }) 
		.when('/forgot-password', {
            controller: 'ForgotPasswordController',
            templateUrl: 'partials/users/forgot-password.html'
        })
		.when('/logout', {
            controller: 'LogOutController',
            templateUrl: 'partials/users/logOut.html',
        })
		.when('/users', {
            controller: 'UsersController',
            templateUrl: 'partials/users/users.html',
        })
		.when('/addUser', {
            controller: 'EditUserController',
            templateUrl: 'partials/users/edit.html',
        })
		.when('/editUser/:id', {
            controller: 'EditUserController',
            templateUrl: 'partials/users/edit.html',
        })		
		.when('/insuranceplantype/benefits', {
            controller: 'BenefitsController',
            templateUrl: 'partials/insuranceplantype/benefits/benefits.html',
        })
		.when('/insuranceplantype/addBenefit', {
            controller: 'EditBenefitController',
            templateUrl: 'partials/insuranceplantype/benefits/edit.html',
        })		
		.when('/insuranceplantype/editBenefit/:id', {
            controller: 'EditBenefitController',
            templateUrl: 'partials/insuranceplantype/benefits/edit.html',
        })
		.when('/insuranceplantype/addBenefitCost', {
            controller: 'AddBenefitCostController',
            templateUrl: 'partials/insuranceplantype/benefits/benefit-cost.html',
        })
		.when('/insuranceplantype/planattribute', {
            controller: 'PlanattributeController',
            templateUrl: 'partials/insuranceplantype/planattribute/planattribute.html',
        })
        .when('/insuranceplantype/planattribute/:planid/:businessyear', {
            controller: 'PlanattributeController',
            templateUrl: 'partials/insuranceplantype/planattribute/planattribute.html',
        })
		.when('/insuranceplantype/viewplanattribute/:id/:sortby/:desc', {
            controller: 'GetPlanattributeController',
            templateUrl: 'partials/insuranceplantype/planattribute/view.html',
        })		
		.when('/insuranceplantype/editplanattribute/:id/:sortby/:desc', {
            controller: 'EditPlanattributeController',
            templateUrl: 'partials/insuranceplantype/planattribute/edit.html',
        })
        .when('/insuranceplantype/editplanattribute/:id/:action', {
            controller: 'EditPlanattributeController',
            templateUrl: 'partials/insuranceplantype/planattribute/edit.html',
        })
        .when('/insuranceplantype/newplanattribute', {
            controller: 'NewPlanattributeController',
            templateUrl: 'partials/insuranceplantype/planattribute/edit.html',
        })
		.when('/insuranceplantype/planbenefit', {
            controller: 'PlanbenefitController',
            templateUrl: 'partials/insuranceplantype/planbenefit/planbenefit.html',
        })
        .when('/insuranceplantype/planbenefit/:planid/:businessyear', {
            controller: 'PlanbenefitController',
            templateUrl: 'partials/insuranceplantype/planbenefit/planbenefit.html',
        })
		.when('/insuranceplantype/viewplanbenefit/:id/:sortby/:desc', {
            controller: 'GetPlanbenefitController',
            templateUrl: 'partials/insuranceplantype/planbenefit/view.html',
        })	
		.when('/insuranceplantype/editplanbenefit/:id/:sortby/:desc', {
            controller: 'EditPlanbenefitController',
            templateUrl: 'partials/insuranceplantype/planbenefit/edit.html',
        })
        .when('/insuranceplantype/editplanbenefit/:id/:action', {
            controller: 'EditPlanbenefitController',
            templateUrl: 'partials/insuranceplantype/planbenefit/edit.html',
        })
        .when('/insuranceplantype/newplanbenefit', {
            controller: 'NewPlanbenefitController',
            templateUrl: 'partials/insuranceplantype/planbenefit/edit.html',
        })
		.when('/insuranceplantype/csrrate', {
            controller: 'CsrrateController',
            templateUrl: 'partials/insuranceplantype/csrrate/csrrate.html',
        })
        .when('/insuranceplantype/csrrate/:planid', {
            controller: 'CsrrateController',
            templateUrl: 'partials/insuranceplantype/csrrate/csrrate.html',
        })
		.when('/insuranceplantype/viewcsrrate/:id/:sortby/:desc', {
            controller: 'GetCsrrateController',
            templateUrl: 'partials/insuranceplantype/csrrate/view.html',
        })
		.when('/insuranceplantype/editcsrrate/:id/:sortby/:desc', {
            controller: 'EditCsrrateController',
            templateUrl: 'partials/insuranceplantype/csrrate/edit.html',
        })
        .when('/insuranceplantype/editcsrrate/:id/:action', {
            controller: 'EditCsrrateController',
            templateUrl: 'partials/insuranceplantype/csrrate/edit.html',
        })
        .when('/insuranceplantype/newcsrrate', {
            controller: 'NewCsrrateController',
            templateUrl: 'partials/insuranceplantype/csrrate/edit.html',
        })
		.when('/cases', {
            controller: 'CasesController',
            templateUrl: 'partials/cases/cases.html',
        })
		.when('/newCase', {
            controller: 'CreateCaseController',
            templateUrl: 'partials/cases/new.html',
        })
		.when('/editCase/:view/:id/:action', {
            controller: 'CreateCaseController',
            templateUrl: 'partials/cases/new.html',
        })
		/****
			Created By : Aastha Jain
			Created Date : 13-06-2016
			Start : Add 3 new masters (Rating Area, Zip Code, Country).
		****/
		.when('/masters/ratingarea', {
            controller: 'RatingAreaController',
            templateUrl: 'partials/masters/ratingarea/ratingarea.html',
        })
		.when('/masters/zipcode', {
            controller: 'ZipCodeController',
            templateUrl: 'partials/masters/zipcode/zipcode.html',
        })
		.when('/masters/county', {
            controller: 'CountyController',
            templateUrl: 'partials/masters/county/county.html',
        })
		
		/**** End : Add 3 new masters (Rating Area, Zip Code, Country). ****/
		
		/**
			Created By : Aastha Jain
			Created Date : 11-07-2016
			Start : Job Mstr Controller.
		**/
		
		.when('/masters/job', {
            controller: 'JobController',
            templateUrl: 'partials/masters/job/job.html',
        })
		.when('/masters/newJob', {
            controller: 'CreateJobController',
            templateUrl: 'partials/masters/job/new.html',
        })
		.when('/masters/viewJob/:jobNo', {
            controller: 'CreateJobController',
            templateUrl: 'partials/masters/job/view.html',
        })
		.when('/masters/editJob/:jobNo/:action', {
            controller: 'CreateJobController',
            templateUrl: 'partials/masters/job/new.html',
        })
		.when('/masters/employermaster', {
            controller: 'EmployermasterController',
            templateUrl: 'partials/masters/employer/employer.html',
        })
		.when('/masters/newemployer', {
            controller: 'AddEmployerController',
            templateUrl: 'partials/masters/employer/new.html',
        })
		.when('/masters/editEmployer/:employerId', {
            controller: 'AddEmployerController',
            templateUrl: 'partials/masters/employer/new.html',
        })

        .when('/masters/issuermaster', {
            controller: 'IssuermasterController',
            templateUrl: 'partials/masters/issuer/issuer.html',
        })
		.when('/masters/newissuer', {
            controller: 'AddIssuerController',
            templateUrl: 'partials/masters/issuer/new.html',
        })
		.when('/masters/editIssuer/:issuerId', {
            controller: 'AddIssuerController',
            templateUrl: 'partials/masters/issuer/new.html',
        })
		
		/** End : Job Mstr Controller. **/
		
        .otherwise({ redirectTo: '/login' });	
		$locationProvider.hashPrefix = '!';	
		$locationProvider.html5Mode({enabled: true,requireBase: false});	
		
}])
.run(['$rootScope', '$location', '$cookieStore', '$http',
    function ($rootScope, $location, $cookieStore, $http) {
		$rootScope.pageLoading=true;			
        // keep user logged in after page refresh
        $rootScope.globals = $cookieStore.get('globals') || {};
        if ($rootScope.globals.currentUser) {
            $http.defaults.headers.common['Authorization'] = 'bearer ' + $rootScope.globals.currentUser.authdata; // jshint ignore:line
        }
		$rootScope.$on('$includeContentLoaded', function(event) {	
		   positionFooter();
		   $(window).scroll(positionFooter).resize(positionFooter);
		   $('body').niceScroll({
				autohidemode: 'false',     // Do not hide scrollbar when mouse out
				cursorborderradius: '12px', // Scroll cursor radius
				background: '#ffffff',     // The scrollbar rail color
				cursorwidth: '10px',       // Scroll cursor width
				cursorcolor: '#3b84f2'     // Scroll cursor color
			});		
			/** Custom coding for menu support */	
			if($location.path()=="/cases" || $location.path()=="/newCase" || ($location.path().indexOf("editCase")>-1)){				
				$rootScope.activeMenuCa="activeMenu";
				$rootScope.activeMenuA="";
				$rootScope.activeMenuB="";
				$rootScope.activeMenuC="";
			}else if($location.path()=="/users" || $location.path()=="/addUser" || ($location.path().indexOf("/editUser")>-1)){					
				$rootScope.activeMenuCa="";
				$rootScope.activeMenuA="activeMenu";
				$rootScope.activeMenuB="";
				$rootScope.activeMenuC="";
				
			}else if($location.path()=="/insuranceplantype/benefits" || $location.path()=="/insuranceplantype/addBenefit" ||($location.path().indexOf("/insuranceplantype/editBenefit")>-1) || $location.path()=="/insuranceplantype/addBenefitCost"){					
				$rootScope.activeMenuCa="";
				$rootScope.activeMenuA="";
				$rootScope.activeMenuB="activeMenu";
				$rootScope.activeMenuC="";
			}else if($location.path()=="/change-password"){	
				console.log("password");
				$rootScope.activeMenuCa="";
				$rootScope.activeMenuA="";
				$rootScope.activeMenuB="";
				$rootScope.activeMenuC="activeMenu";
			}
	   });	
    }]);