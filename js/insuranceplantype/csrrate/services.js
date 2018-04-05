'use strict';
/**
	Csrrate Service.
*/
angular.module('mhmApp.csrrate')
.factory('CsrrateService',
    ['$http', '$cookieStore', '$rootScope', '$timeout','messages','$q',
    function ($http, $cookieStore, $rootScope, $timeout,messages, $q) {
        var service = {}; 
		var config = {
			headers : {
				'Content-Type': 'application/x-www-form-urlencoded',
			}
		}
		
		service.update = function (data,callback) {			
           var url = messages.serverLiveHost + messages.updateBenefit;		   
			var data={MHMBenefitID: data.MHMBenefitID, MHMBenefitName: data.MHMBenefitName,CategoryId:data.CategoryId,IsDefault:data.IsDefault,Createdby:data.Createdby,MHMBenefitMappingMsts:data.MHMBenefitMappingMsts};				
			$http({method:'POST',url:url,data:data}).success(function(response) {				
				callback(response); 	
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	;		  
        }; 
		
		service.getAll = function (data,callback) {
           var url = messages.serverLiveHost + messages.getAllCSRRates;		 
		   var searchby="";		   
		   var lstParameter=[];
		   lstParameter[0]='';
		   lstParameter[1]='';
		   lstParameter[2]='';
		   //Vaibhav
		   //lstParameter[3]='';
		   //lstParameter[4]='';
		   //lstParameter[5]='';
			
			//  MarketCoverage State MetalLevel
		   /*if(typeof data.filterByMarketCover!='undefined' && data.filterByMarketCover!=''){				
				lstParameter[0]=data.filterByMarketCover;				
		   }if(typeof data.searchByStateCode!='undefined' && data.searchByStateCode!=''){				
				lstParameter[1]=data.searchByStateCode;
		   }if(typeof data.filterByMetalLevel!='undefined' && data.filterByMetalLevel!=''){				
				lstParameter[2]=data.filterByMetalLevel;		  
		   }if(typeof data.searchByPhone!='undefined' && data.searchByPhone!=''){
				lstParameter[3]=data.searchByPhone;		   
		   }*/

		   //if(typeof data.BusinessYear!='undefined' && data.BusinessYear!=''){				
			//	lstParameter[0]=data.BusinessYear;
		   //}
		   if(typeof data.RatingAreaId!='undefined' && data.RatingAreaId!=''){				
				lstParameter[0]=data.RatingAreaId;
		   }
		   if(typeof data.filterByMarketCover!='undefined' && data.filterByMarketCover!=''){				
				lstParameter[1]=data.filterByMarketCover;
		   }
		   if(typeof data.IssuerId!='undefined' && data.IssuerId!=''){				
				lstParameter[2]=data.IssuerId;
		   }
		   
		   if(typeof data.searchby!='undefined' && data.searchby!=''){			
				searchby=data.searchby;				
		   }
		   
			var params={lstParameter:lstParameter,sortby:data.sortby,desc:data.desc,page:data.page,pageSize:data.pageSize,BusinessYear:data.BusinessYear,searchby:searchby};
			console.log("csr params", params);
			$http({method:'GET',url:url,params:params}).success(function(response) {					
				callback(response); 	
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});
        };
		
		/**
			Created By : Aastha Jain
			Created Date : 07-06-2016
			Purpose : Function to export CSR Rate list.
			Get case report.
			Start
		*/
		
		service.getCSRrateReport = function (data,callback) {		
			var url = messages.serverLiveHost + messages.getCSRrateReport;
			var lstParameter=[];	
			lstParameter[0]='';
			lstParameter[1]='';
			lstParameter[2]='';	
			var searchby=''; 
			var sortby='';
			var desc='';		
			var BusinessYear='';			
			//  MarketCoverage State MetalLevel
			/*if(typeof data.filterByMarketCover!='undefined' && data.filterByMarketCover!=''){				
				lstParameter[0]=data.filterByMarketCover;				
			}if(typeof data.searchByStateCode!='undefined' && data.searchByStateCode!=''){				
				lstParameter[1]=data.searchByStateCode;
			}if(typeof data.filterByMetalLevel!='undefined' && data.filterByMetalLevel!=''){				
				lstParameter[2]=data.filterByMetalLevel;		  
			}if(typeof data.searchByPhone!='undefined' && data.searchByPhone!=''){
				lstParameter[3]=data.searchByPhone;		   
			}
		   
		   	if(typeof data.searchby!='undefined' && data.searchby!=''){
				searchby=data.searchby;
			}
			if(typeof data.sortby!='undefined' && data.sortby!=''){
				sortby=data.sortby;
			}
			if(typeof data.BusinessYear!='undefined' && data.BusinessYear!=''){
				BusinessYear=data.BusinessYear;
			}*/
			
			if(typeof data.RatingAreaId!='undefined' && data.RatingAreaId!=''){				
				lstParameter[0]=data.RatingAreaId;
		    }
		    if(typeof data.filterByMarketCover!='undefined' && data.filterByMarketCover!=''){				
				lstParameter[1]=data.filterByMarketCover;
		    }
		    if(typeof data.IssuerId!='undefined' && data.IssuerId!=''){				
				lstParameter[2]=data.IssuerId;
		    }
		   
		    if(typeof data.BusinessYear!='undefined' && data.BusinessYear!=''){			
				BusinessYear=data.BusinessYear;				
		    }
			
			if(typeof data.searchby!='undefined' && data.searchby!=''){			
				searchby=data.searchby;				
		    }

			url +='?searchby='+searchby;
			url +='&sortby='+sortby;
			url +='&desc='+data.desc;
			url +='&lstParameter='+lstParameter[0];
			url +='&lstParameter='+lstParameter[1];
			url +='&lstParameter='+lstParameter[2];
			url +='&BusinessYear='+BusinessYear;
		  		   
			var xhr = new XMLHttpRequest();
			xhr.open('GET', url, true);
			xhr.responseType = 'blob';
			xhr.onload = function () {				
				callback(this.status,this.response,xhr);				
			};	
			xhr.setRequestHeader('Authorization', 'bearer '+ $rootScope.globals.currentUser.authdata);			
			xhr.send();				
        };
		
		/** End : Function to export CSR Rate list. **/
		
		service.get = function (id,callback) {
			var url = messages.serverLiveHost + messages.getCSRRate+'?Id='+id;		 	  
			$http.get(url).success(function(response) {					
				callback(response); 				
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});
        };

        service.getData = function (data, callback) {
        	var url = messages.serverLiveHost + messages.getCSRRate+'?Id='+data.id+'&EventType='+data.eventType+'&sortby='+data.sortby+'&desc='+data.desc;
			$http.get(url).success(function(response) {					
				callback(response);
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});
        };

        
		
		service.GetAllStates = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetAllStates;
			return $http.get(url);						  
        };
		
		service.GetMarketCoverage = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetMarketCoverage+'/CSR_Rate_Mst';		
			return $http.get(url);						  
        };
		
		service.GetMetalLevel = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetMetalLevel+'/CSR_Rate_Mst';			
			return $http.get(url);						  
        };
		
		service.ratingArea = function () {
			var result={};
			var url = messages.serverLiveHost + messages.ratingArea;		   
			return $http.get(url);						  
        };
		
		service.waitForLayout=function(callback){				
			$q.all([			
					service.GetAllStates(),	
					service.GetMarketCoverage(),	
					service.GetMetalLevel(),
					service.GetRatingAreas(),
					service.GetSearchIssuerID(),
				]).then(function(response) {
					callback(response)				 		
			});					
		};	
		
		service.waitForLayoutView=function(callback){				
			$q.all([				
					service.GetAllStates(),
					service.ratingArea(),
				]).then(function(response){
					callback(response)				 		
			  });					
		};

		service.waitForLayoutEdit=function(callback){				
			$q.all([				
					// service.GetMetalLevel(),	
					// service.GetMarketCoverage(),	
					// service.GetAllStates(),
					service.ratingArea(),
				]).then(function(response){
					callback(response)				 		
			  });					
		};
		
		service.UpdateCSRRate = function (data, callback) {
			var url = messages.serverLiveHost + messages.UpdateCSRRate;	
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

        service.GetRatingAreas = function(response) {
        	var url = messages.serverLiveHost + messages.GetRatingAreas;
        	return $http.get(url);
        }

        service.GetSearchIssuerID = function(callback) {
        	var url = messages.serverLiveHost + messages.GetSearchIssuerIds+'?keyword='+'';
        	
        	return $http.get(url);
        };

        service.AddCSRRate = function(data, callback) {
        	var url = messages.serverLiveHost + messages.AddCSRRate;	
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
		
		service.ImportCSRRate = function(data, callback) {
        	var url = messages.serverLiveHost + '/api/PlanMaster/UploadCsrRateMst';	
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

        service.GetSearchPlanID = function(data, callback) {
        	var searchByPlanId = data.searchByPlanId ? data.searchByPlanId : '';
        	var url = messages.serverLiveHost + messages.GetCSRSearchPlanIds+'?keyword='+searchByPlanId;
        	console.log("planId url", url);

        	$http.get(url).success(function(response) {
        		callback(response);
        	}).error(function(response) {
        		if(isEmpty(response)) {
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response);
        	});
        };
		
        return service;
    }]);