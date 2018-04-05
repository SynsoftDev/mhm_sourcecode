'use strict';
/**
	Created By : Aastha Jain
	Created Date : 13-06-2016
	Start : Rating Area Services.
*/
angular.module('mhmApp.ratingarea')
.factory('RatingAreaService',
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
			var url = messages.serverLiveHost + messages.GetRatingAreas;		 
			var searchby="";		   
			var lstParameter=[];	
			lstParameter[0]='';
			lstParameter[1]='';
			lstParameter[2]='';
			
			//  StateCode, Rating Area and MarketCover 
			if(typeof data.searchByStateCode!='undefined' && data.searchByStateCode!=''){
				lstParameter[0]=data.searchByStateCode;				
			}if(typeof data.searchByRatingArea!='undefined' && data.searchByRatingArea!=''){
				lstParameter[1]=data.searchByRatingArea;
			}if(typeof data.searchByMarketCover!='undefined' && data.searchByMarketCover!=''){
				lstParameter[2]=data.searchByMarketCover;		  
			}
		   
			if(typeof data.searchby!='undefined' && data.searchby!=''){			
				searchby=data.searchby;				
			}	
		   
			var params={lstParameter:lstParameter,BusinessYear:data.searchByBusinessYear,searchby:data.searchby,sortby:data.sortby,desc:data.desc,page:data.page,pageSize:data.pageSize};
			
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
		
		/**** Functions to get list : States, Rating Area, Market Coverage ****/
		
		service.GetAllStates = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetAllStates;		   
			return $http.get(url);						  
        };
		
		service.ratingArea = function () {
			var result={};
			var url = messages.serverLiveHost + messages.ratingArea;		   
			return $http.get(url);						  
        };
		
		service.GetMarketCoverage = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetMarketCoverage+'/PlanAttributeMsts';
			return $http.get(url);						  
        };		
		
		service.waitForLayout=function(callback){				
			$q.all([				
				service.GetAllStates(),
				service.ratingArea(),
				service.GetMarketCoverage(),
			]).then(function(response){
				callback(response)				 		
			});					
		};
		 	
        return service;
		
    }]);
	
/** End : Rating Area Services. **/