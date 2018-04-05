'use strict';
/**
	Plan benefit Service.
*/
angular.module('mhmApp.planbenefit')
.factory('PlanbenefitService',
    ['$http', '$cookieStore', '$rootScope', '$timeout','messages','$q',
    function ($http, $cookieStore, $rootScope, $timeout, messages, $q) {
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
			});
        };
		service.getAll = function (data, callback) {
			console.log("all data", data);
           var url = messages.serverLiveHost + messages.getAllPlanBenefits;		 
		   var searchby = "";		   
		   var lstParameter=[];	
		   lstParameter[0]='';
		   lstParameter[1]='';
		   lstParameter[2]='';
		   var businessYear = data.BusinessYear ? data.BusinessYear : '';

			if(typeof data.searchByPlanId!='undefined' && data.searchByPlanId!=''){				
				lstParameter[0]=data.searchByPlanId;				
			}
			if(typeof data.searchByMHMBenefitId!='undefined' && data.searchByMHMBenefitId!=''){
				lstParameter[1]=data.searchByMHMBenefitId;
			}
			if(typeof data.searchByIssuerId!='undefined' && data.searchByIssuerId!=''){
				lstParameter[2]=data.searchByIssuerId;
			}
			if(typeof data.searchby!='undefined' && data.searchby!='') {			
				searchby = data.searchby;
			}
		   
			var params={lstParameter:lstParameter,sortby:data.sortby,desc:data.desc,page:data.page,pageSize:data.pageSize,BusinessYear:businessYear,searchby:searchby};
			console.log("call search api", params);
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
			Purpose : Function to export plan benefit list.
			Get case report.
			Start
		*/
		service.getPlanbenefitReport = function (data,callback) {		
			var url = messages.serverLiveHost + messages.getPlanbenefitReport;
			var lstParameter=[];	
			lstParameter[0]='';
			lstParameter[1]='';
			lstParameter[2]='';
			lstParameter[3]='';	
			var searchby=''; 
			var sortby='';
			var desc='';		
			var BusinessYear='';

		    if(typeof data.searchByPlanId!='undefined' && data.searchByPlanId!=''){
				lstParameter[0]=data.searchByPlanId;				
		    }
		    if(typeof data.searchByMHMBenefitId!='undefined' && data.searchByMHMBenefitId!=''){
				lstParameter[1]=data.searchByMHMBenefitId;
		    }
		    if(typeof data.searchByIssuerId!='undefined' && data.searchByIssuerId!=''){				
				lstParameter[2]=data.searchByIssuerId; 
		    }
		    if(typeof data.searchByPhone!='undefined' && data.searchByPhone!=''){				
				lstParameter[3]=data.searchByPhone;		   
		    }
		   
		    if(typeof data.searchby!='undefined' && data.searchby!=''){
				searchby=data.searchby;
			}
			if(typeof data.sortby!='undefined' && data.sortby!=''){
				sortby=data.sortby;
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
		
		/** End : Function to export plan benefit list. **/
		
		service.get = function (id,callback) {
			var url = messages.serverLiveHost + messages.GetPlanBenefit+'?Id='+id;
			console.log("get editable plan url", url);	 	  
			$http.get(url).success(function(response) {
				console.log("plan benefit service", response);			
				callback(response); 				
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response);
			});
        };

        service.getData = function(data, callback) {
        	var url = messages.serverLiveHost + messages.GetPlanBenefitData+'?Id='+data.id+'&EventType='+data.eventType+'&sortby='+data.sortby+'&desc='+data.desc;
        	console.log("get edittable data new api", url);

        	$http.get(url).success(function(response) {
        		callback(response);
        	}).error(function(response) {
        		if (isEmpty(response)) {
        			var response={Status:false,Message:messages.TryLater,redirect:true};
        		}
        		callback(response);
        	});
        };

        service.GetSearchPlanID = function(data, callback) {
        	var searchByPlanId = data.searchByPlanId ? data.searchByPlanId : '';
        	var url = messages.serverLiveHost + messages.GetSearchPlanIds+'?keyword='+searchByPlanId;
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

        service.GetSearchMHMId = function(data, callback) {
        	var searchByMHMBenefitId = data.searchByMHMBenefitId ? data.searchByMHMBenefitId : '';
        	var url = messages.serverLiveHost + messages.GetSearchMHMBenefitIds+'?keyword='+searchByMHMBenefitId;
        	console.log("mhm benefit url", url);

        	$http.get(url).success(function(response) {
        		callback(response);
        	}).error(function(response) {
        		if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response);
        	});
        };

        service.GetSearchIssuerID = function(data, callback) {
        	var searchByIssuerId = data.searchByIssuerId ? data.searchByIssuerId : '';
        	var url = messages.serverLiveHost + messages.GetSearchIssuerIds+'?keyword='+searchByIssuerId;
        	console.log("issure url", url);
        	
        	$http.get(url).success(function(response) {
        		callback(response);
        	}).error(function(response) {
        		if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response);
        	});
        };
		
		service.remove = function (id,callback) {
           var url = messages.serverLiveHost + messages.deleteBenefit+'/'+window.parseInt(id);
			var data={id: window.parseInt(id)};				
			$http({method:'POST',url:url,data:data}).success(function(response) {				
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        };

        service.GetSearchMHMId1 = function() {
        	var url = messages.serverLiveHost + messages.GetSearchMHMBenefitIds+'?keyword='+'';

        	return $http.get(url);
        };
		
		service.GetAllStates = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetAllStates;					
			return $http.get(url);						  
        };
		
		service.GetMarketCoverage = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetMarketCoverage+'/PlanBenefitMsts';			
			return $http.get(url);						  
        };

        /*service.GetNextRecord = function() {
        	var url = messages.serverLiveHost + messages.GetNextRecord+'?Id=56229&EventType=&sortby=PlanId&desc=true';
        	console.log("get next record", url);
        	return $http.get(url);
        };*/
		
		service.waitForLayout=function(callback){				
			$q.all([			 
				 service.GetAllStates(),
				 service.GetMarketCoverage(),
				]).then(function(response){
					callback(response)				 		
			});
		};
		
		service.waitForLayoutView=function(callback){				
			$q.all([				
				 service.GetMarketCoverage(),
				 service.GetAllStates(),
				 service.GetSearchMHMId1(),
				]).then(function(response){
					callback(response)				 		
			  });					
		};
		
		service.waitForLayoutEdit=function(callback){				
			$q.all([				
				 service.GetMarketCoverage(),
				 service.GetAllStates(),
				 service.GetSearchMHMId1(),
				 service.GetCostSharingTypes(),
				 service.GetLimitUnits(),
				]).then(function(response){
					callback(response)				 		
			});
		};
		
		service.UpdatePlanBenefit = function (data, callback) {
			console.log("======", data);
			var url = messages.serverLiveHost + messages.UpdatePlanBenefit;	
			$http.post(url, JSON.stringify(data), {headers : {'Content-Type': 'application/json'
			}}).success(function(response) {			
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	  
        };

        service.GetDefaultBenefitId = function(data, callback) {
        	var url = messages.serverLiveHost + messages.GetDefaultBenefitId+'/'+data.IssuerId+'/'+data.BenefitNum;
        	$http.get(url).success(function(response) {
        		callback(response);
        	}).error(function(response) {
        		if(isEmpty(response)){
					var response = {Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response);
        	});
        };

        service.GetCostSharingTypes = function() {
        	var url = messages.serverLiveHost + messages.GetCostSharingTypes;
        	return $http.get(url);
        };

        service.GetLimitUnits = function() {
        	var url = messages.serverLiveHost + messages.GetLimitUnits;
        	return $http.get(url);
        };

        service.AddPlanBenefit = function(data, callback) {
        	var url = messages.serverLiveHost + messages.AddPlanBenefit;
        	$http.post(url, JSON.stringify(data), {headers : {'Content-Type': 'application/json'
			}}).success(function(response) {
        		callback(response);
        	}).error(function(response) {
        		if (isEmpty(response)) {
        			var response = {Status:false,Message:messages.TryLater,redirect:true};
        		}
        		callback(response);
        	});
        };
		
		service.ImportPlanBenefit = function(data, callback) {
        	var url = messages.serverLiveHost + '/api/PlanMaster/UploadPlanBenefitMst';	
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
		
        return service;
    }]);