'use strict';

angular.module('mhmApp.employermaster')
.factory('EmployermasterService',
    ['$http','$cookieStore','$rootScope','$timeout','messages','$q',
    function ($http,$cookieStore,$rootScope,$timeout,messages,$q){
	 var service = {}; 
		var config = {
			headers : {
				'Content-Type': 'application/x-www-form-urlencoded',
			}
		}	
	
	service.getAll= function(data, callback){	
console.log('api called');	
			var url = messages.serverLiveHost + messages.GetEmployers;		 
		   		   
			var params={sortby:data.sortby,desc:data.desc,page:data.page,pageSize:data.pageSize,searchby:data.searchby,employerid:data.filterByEmployerId};
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
				 
				]).then(function(response){
					callback(response)				 		
			  });					
		};
	
	service.Action = function (data, action, callback) {
		console.log('employer add service called');	
		if(action == "add")
				var url = messages.serverLiveHost + messages.AddEmployer;	
			else
				var url = messages.serverLiveHost + messages.UpdateEmployer;
			
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
	
	service.getEmployer = function (id,callback) {
			var url = messages.serverLiveHost + messages.GetEmployer+'/'+id;		 	  
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