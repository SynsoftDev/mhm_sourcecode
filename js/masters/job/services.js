'use strict';
/**
	Created By : Aastha Jain
	Created Date : 11-07-2016
	Start : Job Services.
*/
angular.module('mhmApp.job')
.factory('JobService',
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
			var url = messages.serverLiveHost + messages.GetJobMstrLists;		 
			var searchby="";		   
			var lstParameter=[];	
			lstParameter[0]='';
			lstParameter[1]='';
			lstParameter[2]='';
			lstParameter[3]='';
			
			//  Employer Id, Job Status Id, Job Start Date and Job End Date
			if(typeof data.searchByEmployerId!='undefined' && data.searchByEmployerId!=''){
				lstParameter[0]=data.searchByEmployerId;				
			}if(typeof data.searchByJobStatusId!='undefined' && data.searchByJobStatusId!=''){
				lstParameter[1]=data.searchByJobStatusId;
			}if(typeof data.searchByJobDateStart!='undefined' && data.searchByJobDateStart!=''){
				lstParameter[2]=data.searchByJobDateStart;		  
			}if(typeof data.searchByJobDateEnd!='undefined' && data.searchByJobDateEnd!=''){
				lstParameter[3]=data.searchByJobDateEnd;		  
			}
		   
			if(typeof data.searchby!='undefined' && data.searchby!=''){			
				searchby=data.searchby;				
			}	
		   
			var params={lstParameter:lstParameter,JobYear:data.searchByJobYear,searchby:data.searchby,sortby:data.sortby,desc:data.desc,page:data.page,pageSize:data.pageSize};
			
			$http({method:'GET',url:url,params:params}).success(function(response) {
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        };
		
		service.GetEmployerMaster = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetEmployerMaster;
			return $http.get(url);						  
        };

		service.GetJobStatus = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetJobStatus;
			return $http.get(url);						  
        };
		
		service.GetCaseJobRunStatus = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetCaseJobRunStatus;
			return $http.get(url);						  
        };

		service.GetNewJobNumber = function () {
			var result={};
			var url = messages.serverLiveHost + messages.GetNewJobNumber;
			return $http.get(url);						  
        };
		
		service.getCode = function () {	
			var result={};
			var url = messages.serverLiveHost + messages.code;				
		    return $http.get(url);				
        };	
		
		service.GetInsuranceTypeMaster = function () {
			var result={};
			var url = messages.serverLiveHost + messages.InsuranceTypeMaster;
			return $http.get(url);						  
        };
		
		service.getPlan = function () {
			var result={};
			var url = messages.serverLiveHost + messages.plan;				
			return $http.get(url);						  
        };
		
		service.waitForLayout=function(callback){				
			$q.all([				
				service.GetEmployerMaster(),
				service.GetJobStatus(),
			]).then(function(response){
				callback(response)				 		
			});					
		};
		
		service.waitForLayoutCreate=function(callback){				
			$q.all([				
				service.GetEmployerMaster(),
				service.GetJobStatus(),
				service.GetNewJobNumber(),
				service.GetCaseJobRunStatus(),
				service.getCode(),
				service.GetInsuranceTypeMaster(),
				service.getPlan(),
				service.GetSearchIssuerID(),
				service.GetAllStates(),
				service.getPlan(),
				service.GetGroupName(), 
				service.GetMarketCoverage(),
			]).then(function(response){
				callback(response)				 		
			});					
		};
		
		service.jobAction = function (data, action, callback) {
			
			if(action == "add")
				var url = messages.serverLiveHost + messages.addJobMstr;
			else
				var url = messages.serverLiveHost + messages.editJobMstr;
			
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
		
		service.getJobDetail = function (jobNo,callback) {
			var url = messages.serverLiveHost + messages.getJobDetail+'/'+jobNo;		 	  
			$http.get(url).success(function(response) {					
				callback(response); 				
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});
		};
		
		service.getAllCases = function (jobNo,jobRunStatus,caseStatusIds,callback) {
			var postData={JobNumber:jobNo,JobRunStatus:jobRunStatus,CaseStatusIds:caseStatusIds};
			var url = messages.serverLiveHost + messages.GetAllFinalNotSendCases;		 	  
			$http({method:'GET',url:url,params:postData})
			.success(function(response) {				
				callback(response); 	
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	
		};
		
		service.updateAllCases = function (jobNo,jobRunStatus,caseStatusIds,callback) {
			var postData={JobNumber:jobNo,JobRunStatus:jobRunStatus,CaseStatusIds:caseStatusIds};	
			console.log('postData',postData)
			var url = messages.serverLiveHost + messages.UpdateAllFinalNotSendCases;		 	  
			$http({method:'GET',url:url,params:postData})
			.success(function(response) {				
				callback(response); 	
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	
		};
		
		/**
			Calculate plan API.
		*/
		service.calculatePlans = function (data,Case,ModifiedBy,callback) {				
           var url = messages.serverLiveHost + messages.generatePlans+'?ZipCode='+data.ZipCode+'&CountyName='+data.CountyName+'&Income='+data.Income+'&IsAmericanIndian='+data.IsAmericanIndian+'&SubsidyStatus='+data.SubsidyStatus+'&HSAPercentage='+data.HSAPercentage+'&TaxRate='+data.TaxRate+'&UsageCode='+data.UsageCode+'&IssuerId='+data.IssuerId+'&PlanTypeID='+data.PlanID+'&Welness='+data.Welness+'&EmployerId='+data.EmployerId+'&JobNumber='+data.JobNumber+'&TierIntention='+data.TierIntention+'&BusinessYear='+data.BusinessYear+'&DedBalAvailDate='+data.DedBalAvailDate+'&DedBalAvailToRollOver='+data.DedBalAvailToRollOver+'&ResultStatus=true&ModifiedBy='+ModifiedBy;	   		 
		  console.log(url)
		  $http.post(url,$.param({oCase:Case}),config)
		  .success(function(response) {	
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	;		  
        };
		
		/**
			get Insurance Type.
		*/
		service.getInsuranceType = function (EmployerId,Year,callback) {
			if(typeof EmployerId!='undefined' && EmployerId!=''){
				var url = messages.serverLiveHost + messages.InsuranceType+'/'+EmployerId+'/'+Year;					
				$http.get(url).success(function(response) {	
					callback(response); 	
				}).error(function(response) {	
					callback(response); 	
				});	
			}else{
				callback({Status:false,InsuranceTypeList:[]}); 	
			}
        };

        service.GetJobPlans = function(data, callback) {

        	var url = messages.serverLiveHost + messages.GetJobPlans+'?JobNumber='+data.jobNo;
        	var lstParameter = [];
		    lstParameter[0] = '';
		    lstParameter[1] = '';
		    lstParameter[2] = '';
		   	lstParameter[3] = '';
		   	lstParameter[4] = '';
		   	lstParameter[5] = '';

		   	if(typeof data.data.issuerId!='undefined' && data.data.issuerId!='') {				
				lstParameter[0]=data.data.issuerId;
		    }
		    if(typeof data.data.stateCode!='undefined' && data.data.stateCode!='') {				
				lstParameter[1]=data.data.stateCode;
		    }
		    if(typeof data.data.planType!='undefined' && data.data.planType!='') {				
				lstParameter[2]=data.data.planType;
		    }
		    if(typeof data.data.businessYear!='undefined' && data.data.businessYear!='') {				
				lstParameter[3]=data.data.businessYear;
		    }
		    if(typeof data.data.groupName!='undefined' && data.data.groupName!='') {				
				lstParameter[4]=data.data.groupName;
		    }
		    if(typeof data.data.marketCoverage!='undefined' && data.data.marketCoverage!='') {				
				lstParameter[5]=data.data.marketCoverage;
		    }

			url +='&filters='+lstParameter[0];
			url +='&filters='+lstParameter[1];
			url +='&filters='+lstParameter[2];
			url +='&filters='+lstParameter[3];
			url +='&filters='+lstParameter[4];
			url +='&filters='+lstParameter[5];

			console.log("url", url);

        	$http.get(url).success(function(response) {
        		callback(response);
        	}).error(function(response) {
        		callback(response);
        	});
        };

        service.GetSearchIssuerID = function(callback) {
        	var url = messages.serverLiveHost + messages.GetSearchIssuerIds+'?keyword='+'';
        	return $http.get(url);
        };

        service.GetAllStates = function () {
			var url = messages.serverLiveHost + messages.GetAllStates;
			return $http.get(url);						  
        };

        service.getPlan = function () {
			var url = messages.serverLiveHost + messages.plan;
			return $http.get(url);						  
        };

        service.GetGroupName = function() {
        	var url = messages.serverLiveHost + messages.GetGroupName;
        	return $http.get(url);
        };

        service.GetMarketCoverage = function () {
			var url = messages.serverLiveHost + messages.GetMarketCoverage+'/PlanAttributeMsts';
			return $http.get(url);						  
        };
		
        service.AddJobPlans = function (data, callback) {
			
			var url = messages.serverLiveHost + messages.AddJobPlans;
			console.log(url)
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
		
		service.ImportCaseCensus = function(data, jobdata, callback) {
        	var url = messages.serverLiveHost + messages.ImportCaseCensus+'?JobNumber='+jobdata.JobNo+'&EmployerId='+jobdata.EmployerId+'&FileName='+jobdata.FileName+'';	
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

        /*service.ResetFilters = function(callback) {
        	$q.all([
        		service.GetSearchIssuerID(),
				service.GetAllStates(),
				service.getPlan(),
				service.GetGroupName(), 
				service.GetMarketCoverage()
        	]).then(function(response) {
        		callback(response)
        	});
        };*/

        return service;
		
    }]);
	
/** End : Job Services. **/