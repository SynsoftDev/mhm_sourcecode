'use strict';
/**
	Created By : Aastha Jain
	Created Date : 13-06-2016
	Start : Rating Area Services.
*/
angular.module('mhmApp.county')
.factory('CountyService',
    ['$http', '$cookieStore', '$rootScope', '$timeout', 'messages', '$q',
    function ($http, $cookieStore, $rootScope, $timeout,messages,$q) {
        var service = {}; 
		var config = {
			headers : {
				'Content-Type': 'application/x-www-form-urlencoded',
			}
		}

		/**** To get Rating Area List ****/
				
		service.getAll = function (data,callback) {
			var url = messages.serverLiveHost + messages.GetCounties;		 
			var searchby="";		   
			var lstParameter=[];	
			lstParameter[0]='';
			lstParameter[1]='';
			lstParameter[2]='';
			lstParameter[3]='';
			
			//  StateCode, Rating Area and MarketCover 
			if(typeof data.searchByState!='undefined' && data.searchByState!=''){
				lstParameter[0]=data.searchByState;				
			} if(typeof data.searchByCounty!='undefined' && data.searchByCounty!=''){
				lstParameter[1]=data.searchByCounty;				
			} if(typeof data.searchByFipsState!='undefined' && data.searchByFipsState!=''){
				lstParameter[2]=data.searchByFipsState;				
			} if(typeof data.searchByFipsCountry!='undefined' && data.searchByFipsCountry!=''){
				lstParameter[3]=data.searchByFipsCountry;				
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
		 	
        return service;
		
    }]);
	
/** End : Rating Area Services. **/