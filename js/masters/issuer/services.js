'use strict';

angular.module('mhmApp.issuermaster')
.factory('IssuermasterService',
    ['$http','$cookieStore','$rootScope','$timeout','messages','$q',
    function ($http,$cookieStore,$rootScope,$timeout,messages,$q){
	 var service = {}; 
		var config = {
			headers : {
				'Content-Type': 'application/x-www-form-urlencoded',
			}
		}	
	
	service.getAllIssuer = function(data, callback){	
			var url = messages.serverLiveHost + messages.GetAllCarrier;		 
	   		   
			var params={sortby:data.sortby,desc:data.desc,page:data.page,pageSize:data.pageSize,searchby:data.searchby,issuerCode:data.filterByIssuerCode};
			console.log(params);
			
			$http({method:'GET',url:url,params:params}).success(function(response) {					
				callback(response); 	
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	
	};
	
	service.waitForLayout=function(callback){				
			$q.all([				
				  service.GetAllStates()
				]).then(function(response){
					callback(response)				 		
		    });					
		};
		
	service.GetAllStates = function () {
			var url = messages.serverLiveHost + messages.GetAllStates;
			return $http.get(url);						  
        };
	
	service.Action = function (data, action, callback) {
		console.log('issuer add service called', data);	
		if(action == "add")
				var url = messages.serverLiveHost + messages.AddCarrier;	
			else
				var url = messages.serverLiveHost + messages.UpdateCarrier;
			
			$http.post(url,JSON.stringify(data),{headers : {'Content-Type': 'application/json'
			}}).success(function(response) {			
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	  
        };
	
	service.getIssuer = function (id,callback) {
			var url = messages.serverLiveHost + messages.GetCarrier+'/'+id;		 	  
			$http.get(url).success(function(response) {					
				callback(response); 				
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	;		  
        };
	
	return service;
}]);