'use strict';
/**
	Created By : Aastha Jain
	Created Date : 13-06-2016
	Start : Rating Area Services.
*/
angular.module('mhmApp.zipcode')
.factory('ZipCodeService',
    ['$http', '$cookieStore', '$rootScope', '$timeout', 'messages', '$q',
    function ($http, $cookieStore, $rootScope, $timeout, messages, $q) {
        var service = {}; 
		var config = {
			headers : {
				'Content-Type': 'application/x-www-form-urlencoded',
			}
		}

		/**** To get Rating Area List ****/
				
		service.getAll = function (data,callback) {
			var url = messages.serverLiveHost + messages.GetZipCodes;		 
			var searchby="";		   
			var lstParameter=[];	
			lstParameter[0]='';
			lstParameter[1]='';
			
			// State Code
			if(typeof data.searchByStateCode!='undefined' && data.searchByStateCode!=''){
				lstParameter[0]=data.searchByStateCode;				
			}
			if(typeof data.searchByCountyName!='undefined' && data.searchByCountyName!=''){
				lstParameter[1]=data.searchByCountyName;				
			}
		   
			if(typeof data.searchby!='undefined' && data.searchby!=''){			
				searchby=data.searchby;				
			}	
		   
			var params={lstParameter:lstParameter,BusinessYear:data.searchByBusinessYear,searchby:data.searchby,sortby:data.sortby,desc:data.desc,page:data.page,pageSize:data.pageSize};
			
			$http({method:'GET',url:url,params:params}).success(function(response) {
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        };
		
		/**** Functions to get list : States ****/
		
		service.GetAllStates = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetAllStates;		   
			return $http.get(url);						  
        };
		
		service.waitForLayout=function(callback){				
			$q.all([				
				service.GetAllStates()
			]).then(function(response){
				callback(response)				 		
			});					
		};
		 	
        return service;
		
    }]);
	
/** End : Rating Area Services. **/