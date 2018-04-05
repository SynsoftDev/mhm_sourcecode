'use strict';
/**
	Business logic services only for manage login and logout state.
*/
var mhmAppBusinessServices = angular.module('mhmApp.businessServices', ['ngResource', 'ngCookies']);
mhmAppBusinessServices.factory('checkCreds', ['$cookieStore', function($cookieStore) {
	return function() {
		console.log('@checkCreds');		
		var returnVal = false;	           
		var Creds =  $cookieStore.get('globals');				
		if (Creds!= undefined && Creds!= "" && Creds.currentUser!= undefined && Creds.currentUser.authdata != "") {				
			returnVal = true;
		}
		return returnVal;
		
	};
}]);

mhmAppBusinessServices.factory('getUsername', ['$cookieStore', function($cookieStore) {
	return function() {	  
		var returnVal='';		
		 var Creds =  $cookieStore.get('globals');				
		if (Creds!= undefined && Creds.currentUser.customer != "") {				
			returnVal=Creds.currentUser.customer;
		}				
		return returnVal;
		
	};

}]);

mhmAppBusinessServices.factory('deleteCreds', ['$cookieStore','$rootScope','$http', function($cookieStore,$rootScope,$http) {
	return function() {
		$rootScope.globals = {};
		$cookieStore.remove('globals');
		$cookieStore.remove('CaseSession');
		$cookieStore.remove('UserSession');
		$cookieStore.remove('BenefitsSession');
		$cookieStore.remove('RatesSession');		
		$cookieStore.remove('PlanattrSession');
		$cookieStore.remove('PlanbenefitsSession');
		$cookieStore.remove('CountySession');
		$cookieStore.remove('JobSession');
		$cookieStore.remove('RatingAreaSession');
		$cookieStore.remove('ZipcodeSession');
		$http.defaults.headers.common.Authorization = 'bearer ';               
	};
}]);

mhmAppBusinessServices.factory('DateUtil', function() {
    return {
        stringifyDate: function(dateLong) {
            var theDate = new Date(dateLong);
            var month = theDate.getMonth();
            month++;            
            var day = theDate.getDate();
            var year = theDate.getFullYear();
            var dateStr = month + "/" + day + "/" + year;
            return dateStr;
        }
    };
});

mhmAppBusinessServices.factory('setSearchSession', ['$cookieStore','$rootScope','$http', function($cookieStore,$rootScope,$http) {
	return function(master, sessionObject) {
		var pageArray = $cookieStore.get('page');
		if (pageArray != undefined) {
			pageArray.filter(function(e) {
				if(e.pagename == master) {
					e.data = sessionObject;
				}else {
					var object = {};
					object['pagename'] = master;
					object['data'] = sessionObject;
					pageArray.push(object);
				}
			});
		}else {
			pageArray = [];
			var object = {};
			object['pagename'] = master;
			object['data'] = sessionObject;
			pageArray.push(object);
		}
		$cookieStore.put('page', pageArray);
	};
}]);

mhmAppBusinessServices.factory('getSearchSession', ['$cookieStore','$rootScope','$http', function($cookieStore,$rootScope,$http) {
	return function(master, sessionObject) {
		
		var returnVal='';
		var pageArray = $cookieStore.get('page');
		
		if (pageArray != undefined) {
			pageArray.filter(function(e) {
				if(e.pagename == master) {
					returnVal=e.data;
				}
			});
		}
		
		return returnVal;
		
	};
}]);

mhmAppBusinessServices.factory('resetSearchSession', ['$cookieStore','$rootScope','$http', function($cookieStore,$rootScope,$http) {
	return function(master) {
		$cookieStore.remove('page');
	};
}]);

mhmAppBusinessServices.factory('deleteOtherSearchSession', ['$cookieStore','$rootScope','$http', function($cookieStore,$rootScope,$http) {
	return function(master) {
		
		var currentPage='';
		var pageArray = $cookieStore.get('page');
		
		if (pageArray != undefined) {
			pageArray.filter(function(e) {
				if(e.pagename == master) {
					currentPage=e.data;
				}
			});
			
			$cookieStore.remove('page');
			
			var newpageArray = [];
			var object = {};
			object['pagename'] = master;
			object['data'] = currentPage;
			newpageArray.push(object);
			$cookieStore.put('page', newpageArray);
		}
		
	};
}]);

mhmAppBusinessServices.factory("businessServices", ["DateUtil","getUsername","checkCreds",'deleteCreds','setSearchSession','getSearchSession','resetSearchSession','deleteOtherSearchSession',
	function(DateUtil,getUsername,checkCreds,deleteCreds,setSearchSession,getSearchSession,resetSearchSession,deleteOtherSearchSession) {
		return {
			'DateUtil': DateUtil, 			
			'getUsername': getUsername,			
			'checkCreds': checkCreds,
			'deleteCreds': deleteCreds,
			'setSearchSession': setSearchSession,
			'getSearchSession': getSearchSession,
			'resetSearchSession': resetSearchSession,
			'deleteOtherSearchSession': deleteOtherSearchSession
		};
	}]);
        
