'use strict';
/**
	Benefits Services.
*/
angular.module('mhmApp.benefits')
.factory('BenefitService',
    ['$http', '$cookieStore', '$rootScope', '$timeout','messages',
    function ($http, $cookieStore, $rootScope, $timeout,messages) {
        var service = {}; 
		
		var config = {
			headers : {
				'Content-Type': 'application/x-www-form-urlencoded',
			}
		}
		
		/**
			Update Benefit Services.
		*/
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
		
		/**
			Add Benefit Services.
		*/
		service.add = function (data,callback) {
           var url = messages.serverLiveHost + messages.getBenefits;	
			var data={MHMBenefitName: data.MHMBenefitName,CategoryId:data.CategoryId,IsDefault:data.IsDefault,Createdby:data.Createdby,MHMBenefitMappingMsts:data.MHMBenefitMappingMsts};		
		  	
			$http.post(url,$.param(data),config).success(function(response) {				
				callback(response); 	
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	;		  
        }; 
		
		/**
			Get All Benefits Services.
		*/
		service.getAll = function (data,callback) {
           var url = messages.serverLiveHost + messages.getBenefits;		 
		   var searchby="";	 
		   
		   if(typeof data.searchby!='undefined' && data.searchby!=''){			
				searchby=data.searchby;				
		   }		   
			var data={searchby:angular.lowercase(searchby),sortby:data.sortby,desc:data.desc,page:data.page,pageSize:data.pageSize,filterByCategory:data.filterByCategory};		
			
			$http({method:'GET',url:url,params:data}).success(function(response) {					
				callback(response); 	
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        };
		
		/**
			Get Benefit Services.
		*/
		service.get = function (id,callback) {
           var url = messages.serverLiveHost + messages.getBenefits+'/'+id;		  
			$http.get(url).success(function(response) {					
				callback(response); 				
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	;		  
        };
		
		/**
			Remove Benefit Services.
		*/
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
		
		/**
			Get Category Benefit Services.
		*/
		service.getCategory = function (callback) {          
           var url = messages.serverLiveHost + messages.category;			   
			$http.get(url).success(function(response) {					
				callback(response); 			
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});			  
        };
		
		/**
			Get Category Benefit Number.
		*/
		service.getBenefitNumber = function (id,callback) {          
           var url = messages.serverLiveHost + messages.GetBenefitNumber+'/'+window.parseInt(id);		   
			$http.get(url).success(function(response) {					
				callback(response); 			
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});			  
        };
		
		
		/**
			Get Benefit carrier Services.
		*/
		service.GetBenefitcarrier = function (callback) {          
           var url = messages.serverLiveHost + messages.GetBenefitcarrier;				   
			$http.get(url).success(function(response) {					
				callback(response); 			
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});			  
        };
		
		/**
			Get Rating Area Services.
		*/
		service.getRatingArea = function (callback) {          
           var url = messages.serverLiveHost + messages.ratingArea;		 		    
			$http.get(url).success(function(response) {					
				callback(response); 			
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});			  
        };
		
		/**
			Get All States Services.
		*/
		service.GetAllStates = function (callback) {          
           var url = messages.serverLiveHost + messages.GetAllStates;			    
			$http.get(url).success(function(response) {					
				callback(response); 			
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});			  
        };	
		
		/**
			Get Rating Area Cost Services.
		*/
		service.getRatingAreaCost = function (id,StateCode,callback) {          
           var url = messages.serverLiveHost + messages.getBenefits+'?RatingAreaId='+id+'&StateCode='+StateCode;		   
			$http.get(url).success(function(response) {					
				callback(response); 			
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});			  
        };
		
		/**
			Update Rating Area Cost Services.
		*/
		service.updateRatingAreaCost = function (ratingAreaId,StateCode,ratingAreaCost,Createdby,callback) { 
			var data={RatingAreaID:ratingAreaId,StateCode:StateCode,Createdby:Createdby,Benefits:ratingAreaCost};		
            var url = messages.serverLiveHost + messages.saveBenefitCost; 			
			$http.post(url,$.param(data),config).success(function(response) {				
				callback(response); 			
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});			  
        };
		
		
		/**
			get Rating Area State.
		*/
		service.GetRatingAreasState = function (State,callback) {				  			
				var url = messages.serverLiveHost + messages.GetRatingAreas+'/'+State;					
				$http.get(url).success(function(response) {	
					callback(response); 	
				}).error(function(response) {	
					callback(response); 	
				});
			
        };
		
		/**
			Created By : Aastha Jain
			Created Date : 26-08-2016
			Start : Call CheckBenefitMapping on change the dropdown of BenefitIds.
		**/
		service.CheckBenefitMapping = function (carrierId, newIssuerBenefitID, commonBenefitId, callback) {				  			
			var url = messages.serverLiveHost + messages.CheckBenefitMapping+'/'+carrierId+'/'+newIssuerBenefitID+'/'+commonBenefitId;					
			$http.get(url).success(function(response) {	
				callback(response); 	
			}).error(function(response) {	
				callback(response); 	
			});
			
        };
		/** End : Call CheckBenefitMapping on change the dropdown of BenefitIds. **/
		
        return service;
    }]);