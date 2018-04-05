'use strict';
/**
	Plan attribute Service.
*/
angular.module('mhmApp.planattribute')
.factory('PlanattributeService',
    ['$http', '$cookieStore', '$rootScope', '$timeout','messages','$q',
    function ($http, $cookieStore, $rootScope, $timeout,messages,$q) {
        var service = {}; 
		var config = {
			headers : {
				'Content-Type': 'application/x-www-form-urlencoded',
			}
		}	
				
		service.getAll = function (data, callback) {
           var url = messages.serverLiveHost + messages.getPlanattributes;		 
		   var searchby="";		   
		   var lstParameter=[];	
		   lstParameter[0]='';
		   lstParameter[1]='';
		   lstParameter[2]='';
		   lstParameter[3]='';	
		   lstParameter[4]='';	
		   lstParameter[5]='';
		   lstParameter[6]='';
			//  Carrier, Plan, IsActive , StateCode, MetalLevel and MarketCover 
		   /*if(typeof data.filterByCarrier!='undefined' && data.filterByCarrier!=''){				
				lstParameter[0]=data.filterByCarrier;				
		   }if(typeof data.filterByPlan!='undefined' && data.filterByPlan!=''){				
				lstParameter[1]=data.filterByPlan;
		   }if(typeof data.searchByIsActive!='undefined' && data.searchByIsActive!=''){				
				lstParameter[2]=data.searchByIsActive;		  
		   }if(typeof data.searchByStateCode!='undefined' && data.searchByStateCode!=''){			
				lstParameter[3]=data.searchByStateCode;		   
		   }if(typeof data.filterByMetalLevel!='undefined' && data.filterByMetalLevel!=''){				
				lstParameter[4]=data.filterByMetalLevel;		   
		   }if(typeof data.filterByMarketCover!='undefined' && data.filterByMarketCover!=''){				
				lstParameter[5]=data.filterByMarketCover;		   
		   }if(typeof data.EmployerID!='undefined' && data.EmployerID!=''){				
				lstParameter[6]=data.EmployerID;					
		   }*/

		   if(typeof data.ApprovalStatus!='undefined' && data.ApprovalStatus!=''){				
				lstParameter[0] = data.ApprovalStatus;		
		   }if(typeof data.IssuerId!='undefined' && data.IssuerId!='') {				
				lstParameter[1] = data.IssuerId;
		   }if(typeof data.BusinessYear!='undefined' && data.BusinessYear!=''){				
				lstParameter[2]=data.BusinessYear;		  
		   }if(typeof data.filterByMarketCover!='undefined' && data.filterByMarketCover!=''){			
				lstParameter[3]=data.filterByMarketCover;		   
		   }if(typeof data.GroupName!='undefined' && data.GroupName!=''){				
				lstParameter[4]=data.GroupName;		   
		   }if(typeof data.searchByIsActive!='undefined' && data.searchByIsActive!=''){				
				lstParameter[5]=data.searchByIsActive;		   
		   }if(typeof data.searchByStateCode!='undefined' && data.searchByStateCode!=''){				
				lstParameter[6]=data.searchByStateCode;		
		   }
		   
		   if(typeof data.searchby!='undefined' && data.searchby!=''){			
				searchby=data.searchby;				
		   }	
		   
			var params={lstParameter:lstParameter,sortby:data.sortby,desc:data.desc,page:data.page,pageSize:data.pageSize,BusinessYear:data.BusinessYear,searchby:data.searchby};
			console.log("search plan attribute data", params);
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
			Purpose : Function to export plan attribute list.
			Get case report.
			Start
		*/
		service.getPlanattributeReport = function (data,callback) {		
			var url = messages.serverLiveHost + messages.getPlanattributeReport;
			var lstParameter=[];	
			lstParameter[0]='';
			lstParameter[1]='';
			lstParameter[2]='';
			lstParameter[3]='';	
			lstParameter[4]='';	
			lstParameter[5]='';
			lstParameter[6]='';

			var searchby=''; 
			var sortby='';
			var desc='';		
			var BusinessYear='';			
			//  Carrier, Plan, IsActive , StateCode, MetalLevel and MarketCover 
		   /*if(typeof data.filterByCarrier!='undefined' && data.filterByCarrier!=''){				
				lstParameter[0]=data.filterByCarrier;				
		   }if(typeof data.filterByPlan!='undefined' && data.filterByPlan!=''){				
				lstParameter[1]=data.filterByPlan;
		   }if(typeof data.searchByIsActive!='undefined' && data.searchByIsActive!=''){				
				lstParameter[2]=data.searchByIsActive;		  
		   }if(typeof data.searchByStateCode!='undefined' && data.searchByStateCode!=''){			
				lstParameter[3]=data.searchByStateCode;		   
		   }if(typeof data.filterByMetalLevel!='undefined' && data.filterByMetalLevel!=''){				
				lstParameter[4]=data.filterByMetalLevel;		   
		   }if(typeof data.filterByMarketCover!='undefined' && data.filterByMarketCover!=''){				
				lstParameter[5]=data.filterByMarketCover;		   
		   }*/

			if(typeof data.ApprovalStatus!='undefined' && data.ApprovalStatus!=''){                
                lstParameter[0] = data.ApprovalStatus;        
           	}if(typeof data.IssuerId!='undefined' && data.IssuerId!='') {                
                lstParameter[1] = data.IssuerId;
           	}if(typeof data.BusinessYear!='undefined' && data.BusinessYear!=''){                
                lstParameter[2]=data.BusinessYear;          
           	}if(typeof data.filterByMarketCover!='undefined' && data.filterByMarketCover!=''){            
                lstParameter[3]=data.filterByMarketCover;           
           	}
           	if(typeof data.GroupName!='undefined' && data.GroupName!=''){                
                lstParameter[4]=data.GroupName;           
           	}
           	if(typeof data.searchByIsActive!='undefined' && data.searchByIsActive!=''){                
                lstParameter[5]=data.searchByIsActive;           
           	}
           	if(typeof data.searchByStateCode!='undefined' && data.searchByStateCode!=''){                
                lstParameter[6]=data.searchByStateCode;        
           	}
		   
		   	if(typeof data.searchby!='undefined' && data.searchby!=''){
				searchby=data.searchby;
			}
			if(typeof data.sortby!='undefined' && data.sortby!=''){
				sortby=data.sortby;
			}
			if(typeof data.BusinessYear!='undefined' && data.BusinessYear!=''){
				BusinessYear=data.BusinessYear;
			}
			
			url +='?searchby='+searchby;
			url +='&sortby='+sortby;
			url +='&desc='+data.desc;
			url +='&lstParameter='+lstParameter[0];
			url +='&lstParameter='+lstParameter[1];
			url +='&lstParameter='+lstParameter[2];
			url +='&lstParameter='+lstParameter[3];
			url +='&lstParameter='+lstParameter[4];
			url +='&lstParameter='+lstParameter[5];
			url +='&lstParameter='+lstParameter[6];
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
		
		/** End : Function to export plan attribute list. **/
		
		service.get = function (id,callback) {
			var url = messages.serverLiveHost + messages.GetPlanAttribute+'?Id='+id;		 	  
			$http.get(url).success(function(response) {					
				callback(response); 				
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	;		  
        };

        service.getData = function (data, callback) {
			var url = messages.serverLiveHost + messages.GetPlanAttribute+'?Id='+data.id+'&EventType='+data.eventType+'&sortby='+data.sortby+'&desc='+data.desc;
			$http.get(url).success(function(response) {					
				callback(response); 				
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});
        };

		service.getCarrier = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetBenefitcarrier;		   
			//return $http.get(messages.base_url+'/sampleResponse/plan.json');
			return $http.get(url);						  
        };
		service.getPlan = function () {
			var result={};
			var url = messages.serverLiveHost + messages.plan;		   
			//return $http.get(messages.base_url+'/sampleResponse/plan.json');
			return $http.get(url);						  
        };
		service.GetAllStates = function () {
			var url = messages.serverLiveHost + messages.GetAllStates;
			return $http.get(url);						  
        };
		service.GetAllApprovalStatus = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetAllApprovalStatus;
			// console.log(url);		   
			//return $http.get(messages.base_url+'/sampleResponse/plan.json');
			return $http.get(url);						  
        };

        service.GetGroupName = function() {
        	var url = messages.serverLiveHost + messages.GetGroupName;
        	return $http.get(url);
        };

		service.GetMarketCoverage = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetMarketCoverage+'/PlanAttributeMsts';		   
			//return $http.get(messages.base_url+'/sampleResponse/plan.json');
			return $http.get(url);						  
        };
		service.GetMetalLevel = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetMetalLevel+'/PlanAttributeMsts';		   
			//return $http.get(messages.base_url+'/sampleResponse/plan.json');
			return $http.get(url);						  
        };
		service.GetEmployerMaster = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetEmployerMaster;
			return $http.get(url);						  
        };
		
		/****
				Created By : Vaibhav Chaurasiya
				Created Date : 29-09-2016
				Purpose : Function to get list of Insurance types.
			****/
		service.GetInsuranceTypeMaster = function () {
			var result={};
			var url = messages.serverLiveHost + messages.InsuranceTypeMaster;
			return $http.get(url);						  
        };
		
		service.waitForLayout=function(callback){				
			$q.all([				
				 service.GetAllStates(),	
				 service.GetMarketCoverage(),	
				 service.GetAllApprovalStatus(),
				 service.GetSearchIssuerID(),
				 service.GetGroupName(),
				]).then(function(response){
					callback(response)				 		
			  });					
		};
		
		service.waitForLayoutView=function(callback){				
			$q.all([				
				 service.getPlan(),	
				 service.GetEmployerMaster(),
				 service.GetAllStates(),
				 service.GetMarketCoverage(),
				 service.GetMetalLevel(),
				 service.GetInsuranceTypeMaster(),
				 service.GetAllApprovalStatus(),
				]).then(function(response){
					callback(response)				 		
			  });					
		};
		
		service.UpdatePlanAttributes = function (status,id,callback) {
            var url = messages.serverLiveHost + messages.UpdatePlanAttributes+'?Id='+id+'&Status='+status;	
			//var data={Id:id,Status:status};
		   $http.get(url).success(function(response) {					
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        }; 	
		
		service.waitForLayoutEdit=function(callback){				
			$q.all([				
				 service.getPlan(),	
				 service.GetMarketCoverage(),	
				 service.GetMetalLevel(),
				 
				 service.GetEmployerMaster(),
				 service.GetAllStates(),
				 /****
				Created By : Vaibhav Chaurasiya
				Created Date : 29-09-2016
				Purpose : Function to get list of Insurance types.
				****/
				 service.GetInsuranceTypeMaster(),
				 service.GetAllApprovalStatus(),
				 service.GetSearchIssuerID(),
				 service.GetGroupName(),
				]).then(function(response){
					callback(response)				 		
			  });					
		};
		
		service.UpdatePlanAttribute = function (data, callback) {
			var url = messages.serverLiveHost + messages.UpdatePlanAttribute;	
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

        service.GetSearchIssuerID = function(callback) {
        	var url = messages.serverLiveHost + messages.GetSearchIssuerIds+'?keyword='+'';
        	
        	return $http.get(url);
        };

        service.AddPlanAttribute = function(data, callback) {
        	var url = messages.serverLiveHost + messages.AddPlanAttribute;	
			if(data.Id==''){ data.Id=0; }
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
		
		service.ImportPlanAttribute = function(data, callback) {
        	var url = messages.serverLiveHost + '/api/PlanMaster/UploadPlanAttribute';	
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
		
        return service;
    }]);