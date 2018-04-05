'use strict';
/**
	Benefits Controller List all Benefits.
*/
angular.module('mhmApp.benefits')
.controller('BenefitsController',
    ['$scope', '$rootScope', '$location', 'BenefitService','checkCreds','businessServices','messages',
    function ($scope, $rootScope, $location, BenefitService,checkCreds,businessServices,messages) {       	
		$scope.searchby='';
		$scope.filterByCategory="0";
		$scope.filterByBenefitName='';
		$scope.start_date='';
		$scope.end_date='';
		$scope.sortby='CreatedDateTime';
		$scope.desc=true;		
		$scope.page=1;
		$scope.benefits=[];
		$scope.categoryList={};
		$scope.TempSearch={};
		$scope.TempSearch.filterByCategory="0";	
		$scope.pageSize=messages.pageSize;
		$scope.TotalCount=0;
		$scope.lastCount=0;
		$scope.pageSizes = [10,20,50,100];
		$scope.IsDefaultStatus = {'true':"True",'false':'False'};
		$scope.breadcrumb=true;
		$scope.sessionPage = "BenefitsSession";
		$scope.searchSession = '';
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">MHM Benefits</li>'; 
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
			
			$scope.searchSession = businessServices.getSearchSession($scope.sessionPage);
			if($scope.searchSession != '') {
				$scope.TempSearch = $scope.searchSession;
			}
			
			businessServices.deleteOtherSearchSession($scope.sessionPage);
			
        } else {
            $scope.loggedIn = false;
			$location.path('/login');
        } 
		
		$scope.delete=function(e){
			bootbox.confirm(messages.confirmationMsgDelete,function(response){
				if(response){
					$scope.dataLoading = true;	
					var ID=e.target.id;				
					BenefitService.remove(ID,function (response) {					
						$scope.flash={};
						if (response.Status) {
							$scope.flash.message = 'Successfully deleted.';
							$scope.flash.status = true;
							$scope.flash.type = 'alert-success ';
							$scope.page=1;
							$scope.getBenefitList();							
						} else {
							if(response.redirect){
								bootbox.alert(response.Message,function(){
									$location.path('/');
									$scope.$apply();
								}) 	
							}
							$scope.flash.message = response.Message;
							$scope.flash.status = true;
							$scope.flash.type = 'alert-danger';
							$scope.dataLoading = false;
							$scope.$apply();
						}				
					});
				}
			
			});			
		}
		$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$scope.startDate={};
		$scope.endDate={};
		$scope.startDate.isOpned=false;
		$scope.endDate.isOpned=false;
		
		$scope.open_start = function() {					
			$scope.startDate.isOpned=true;
		};
		$scope.open_end = function() {					
			$scope.endDate.isOpned=true;
		};
		$scope.$watch('end_date', function() {						
			if($scope.end_date!=null){			
				$scope.maxStartDate = angular.copy($scope.end_date);			
			}else{
				$scope.maxStartDate = '';
			}		
          
		}, true);
		$scope.$watch('start_date', function() {			
			$scope.minEndDate = angular.copy($scope.start_date);
			
		}, true);
		$scope.dateOptions = {
			'show-weeks': false
		};	  
		$scope.format = 'MM/dd/yyyy';
		$scope.altInputFormats = ['M!/d!/yyyy'];
		$scope.popup1 = {
			opened: false
		};	
		
		/*$scope.onPageSize=function(){
			 if($scope.pageSize!=undefined && $scope.pageSize!=''){
				$scope.page=1;				
				$scope.getBenefitList();		
			  }
		}*/
		
		$scope.$watch('pageSize', function() {			
			  if($scope.pageSize!=undefined && $scope.pageSize!=''){	
					$scope.page=1;
					
					if($scope.searchSession != '')
						$scope.setBenefitList();
					
					$scope.getBenefitList();		
			  }
		}, true);
		
		$scope.maxDate = new Date();
		
		$scope.setBenefitList=function(){
			$scope.searchby=$scope.TempSearch.searchby;	
			$scope.filterByCategory=$scope.TempSearch.filterByCategory;				
			$scope.page=1;	
			$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
			$scope.getBenefitList();
		}
		$scope.resetBenefitList=function(){
			$scope.TempSearch.searchby=$scope.searchby='';	
			$scope.TempSearch.filterByCategory=$scope.filterByCategory="0";	
			$scope.page=1; 
			businessServices.resetSearchSession($scope.sessionPage);
			$scope.getBenefitList();
		}
		/**
			Get Benefit List Service.
		*/
		$scope.getBenefitList=function(){
			$rootScope.pageLoading=false;
			$scope.dataLoading = true;				
			BenefitService.getAll($scope,function (response) {
				if(response.Status){
					$scope.benefits=response.Benefits;					
					$scope.TotalCount=response.TotalRecords;
					$scope.lastCount=Math.ceil($scope.TotalCount/$scope.pageSize);
					$rootScope.pageLoading=false;	
					$scope.dataLoading = false;		
				}else{
					if(response.redirect){
						bootbox.alert(response.Message,function(){
								$location.path('/');
								$scope.$apply();
							}) 	
					}
					$scope.benefits=[];
				}	
			});
		}
		
		/**
			get Category Service.
		*/
		BenefitService.getCategory(function (response) {					
			if (response.Status) {				
				$.grep(response.CategoryMst,function(e){					
					$scope.categoryList[e.CategoryId]=e.CategoryName; 					
				});
				$scope.getBenefitList();	
			} else{
				$scope.getBenefitList();
			}		
		});
		
		/**
			Sort by get Benefit List Service.
		*/
		$scope.sort_by = function(newSortingOrder) {
			if ($scope.sortby == newSortingOrder)
			$scope.desc = !$scope.desc;			
			$scope.sortby = newSortingOrder;
			$scope.page=1;	
			$scope.getBenefitList();
		};
		  
	   $scope.range = function () {
			if (!$scope.maxSize) { $scope.maxSize = 9; }
			var paginationRange = Math.max($scope.maxSize, 5);
			var pages = [];
			var totalPages = Math.ceil($scope.TotalCount / $scope.pageSize);
			var halfWay = Math.ceil(paginationRange / 2);
			var position;

			if ($scope.page <= halfWay) {
				position = 'start';
			} else if (totalPages - halfWay < $scope.page) {
				position = 'end';
			} else {
			position = 'middle';
			}

			var ellipsesNeeded = paginationRange < totalPages;
			var i = 1;
			while (i <= totalPages && i <= paginationRange) {
				var pageNumber = $scope.calculatePageNumber(i, $scope.page, paginationRange, totalPages);
				var openingEllipsesNeeded = (i === 2 && (position === 'middle' || position === 'end'));
				var closingEllipsesNeeded = (i === paginationRange - 1 && (position === 'middle' || position === 'start'));
				if (ellipsesNeeded && (openingEllipsesNeeded || closingEllipsesNeeded)) {
					pages.push('...');
				} else {
					pages.push(pageNumber);
				}
				i ++;
			}			
			return pages;	
		};
		  
		$scope.calculatePageNumber = function (i, currentPage, paginationRange, totalPages) {		
			var halfWay = Math.ceil(paginationRange/2);
			if (i === paginationRange) {
				return totalPages;
			} else if (i === 1) {
				return i;
			} else if (paginationRange < totalPages) {
				if (totalPages - halfWay < currentPage) {
					return totalPages - paginationRange + i;
				} else if (halfWay < currentPage) {
					return currentPage - halfWay + i;
				} else {
					return i;
				}
			} else {
				return i;
			}
		}
		$scope.setPage = function () {
			if (isValidPageNumber(this.n)) {
				$scope.page = this.n;
				$scope.getBenefitList();
			}
		};
		  
		$scope.nextPage = function () {
			if ($scope.page < $scope.TotalCount - 1) {
				$scope.page++;
				$scope.getBenefitList();
			}
		};
		
		$scope.prevPage = function () {
			if ($scope.page > 1) {
				$scope.page--;
				$scope.getBenefitList();
			}
		};
		
		$rootScope.pageLoading=false;	
		
		$scope.$on('$includeContentLoaded', function(event) {		
			var idleState = false;
			var idleTimer = null;
			$('*').bind('mousemove click mouseup mousedown keydown keypress keyup submit change mouseenter scroll resize dblclick', function () {
				clearTimeout(idleTimer);				
				idleState = false;
				idleTimer = setTimeout(function () {					   
					$location.path('/logout');
					$scope.$apply();
					idleState = true; }, messages.sessionTimeout);
			});
			$("body").trigger("mousemove");
		});
			
    }])
	/**
		Edit Benefit Controller.
	*/
	.controller('EditBenefitController',
    ['$scope', '$rootScope', '$location', 'BenefitService','checkCreds','businessServices','$routeParams','$filter','messages',
    function ($scope, $rootScope, $location, BenefitService,checkCreds,businessServices,$routeParams,$filter,messages) {       
		$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$rootScope.pageLoading=false;	
		$scope.dataLoading=true;	
		$scope.id='';
		$scope.categories=[];	
		$scope.benefitResult=[];
		$scope.benefitNumbers=[];
		$scope.benefit={};	
		$scope.benefit.CategoryId="";
		$scope.benefit.MHMBenefitMappingMsts=[];			
					
		$scope.breadcrumb=true;
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
        } else {
            $scope.loggedIn = false;
			$location.path('/');
        }		
		/**
			Update Benefit Relation Service.
		*/
		$scope.UpdateBenefitRelation=function(){
			BenefitService.GetBenefitcarrier(function (response) {					
				if (response.Status) {	
					$scope.benefit.MHMBenefitMappingMsts=[];
					$.grep(response.Carriers,function(e){					
						var IssuerBenefitID=0;	
						var e1={};	
						var object_by_id = $filter('filter')($scope.benefitResult, {IssuerID:e.Id})[0];
						if(typeof object_by_id!='undefined'){							
							IssuerBenefitID=object_by_id.IssuerBenefitID;
							e1=object_by_id	;
						}
						
						e1.IssuerBenefitID=IssuerBenefitID.toString();
						e1.name=e.IssuerName;
						e1.IssuerID=e.Id;	
						
						//e1.BenefitNumbers = $filter('orderBy')(e.BenefitIds,'IssuerBenefitId',false);
						e1.BenefitNumbers = e.BenefitIds;
						
						$scope.benefit.MHMBenefitMappingMsts.push(e1);		
					});
					$scope.benefit.MHMBenefitMappingMsts=$filter('orderBy')($scope.benefit.MHMBenefitMappingMsts,'IssuerID',false);	
					$scope.dataLoading=false;	
				}		
			});
		}
		
		$scope.benefit.IsDefault = false;					
		$scope.title='Add Benefit';
		$scope.action='Submit';
		if($routeParams.id){
			$scope.id = $routeParams.id;		
			$scope.title='MHM Benefits Mapping to Issuer Benefits';
			$scope.action='Update';
			
			/**
				Get Benefit Relation Service.
			*/
			BenefitService.get($routeParams.id,function (response) {					
				if (response.Status) {					
                    $scope.benefit=response.MHMCommonBenefit;
                    $scope.benefitResult=response.MHMCommonBenefit.MHMBenefitMappingMsts;
					$scope.benefit.CategoryId=$scope.benefit.CategoryId.toString();					
					$scope.UpdateBenefitRelation();					
                } else {
                    $location.path('insuranceplantype/benefits');
                }				
			});				
		}else{
			$scope.UpdateBenefitRelation();			
		}
		
		/**
			Get Category Service.
		*/	
		BenefitService.getCategory(function (response) {					
			if (response.Status) {
				$scope.categories=response.CategoryMst;                    
			} else {
				$location.path('insuranceplantype/benefits');
			}			
		});	
		
		/**
			Created By : Aastha Jain
			Created Date : 26-08-2016
			Start : Call CheckBenefitMapping on change the dropdown of BenefitIds.. 		
		*/		
		$scope.checkBenefitMapping=function(carrierId, oldIssuerBenefitID, newIssuerBenefitID, indexValue){
			
			if($routeParams.id){
				var commonBenefitId = $routeParams.id;
			} 
			else {
				var commonBenefitId = 0;
			}	
					
			$scope.dataLoading=true;
		
			BenefitService.CheckBenefitMapping(carrierId, newIssuerBenefitID, commonBenefitId, function(response){
				if(response.Status == 'true'){
					if(response.BenefitMappingStatus == false) {
						bootbox.alert(response.Message,function(){
							$scope.benefit.MHMBenefitMappingMsts[indexValue].IssuerBenefitID = oldIssuerBenefitID;
							$scope.$apply();
						})
						$scope.dataLoading=false;
					}else {
						$scope.dataLoading=false;
					}	
				}else {
					$scope.dataLoading=false;
				}
			})
				
		}
		
		/**
			Update Benefit Service.
		*/	
		$scope.update = function () {
            $scope.dataLoading = true;
			if($scope.benefit.MHMBenefitID){				
				BenefitService.update($scope.benefit, function (response) {	
					$scope.flash={};
					if (response.Status) {
						$scope.flash.message = 'Successfully added!';						
						bootbox.alert(messages.saved,function(){
							$scope.flash.status = true;
							$scope.flash.type = 'alert-success ';						
							$scope.dataLoading = false;
							$location.path('insuranceplantype/benefits');
							$scope.$apply();
						}) 	
						
					} else {
						if(response.redirect){
							bootbox.alert(messages.TryLater,function(){
									$location.path('/');
									$scope.$apply();
								}) 
						}
						$scope.flash.message = response.Message;
						$scope.flash.status = true;
						$scope.flash.type = 'alert-danger';
						$scope.dataLoading = false;
						$("html").animate({scrollTop:0},500);
					}
				});
			}else{
				$scope.benefit.Createdby=$scope.customer.id;			
				BenefitService.add($scope.benefit, function (response) {	
					$scope.flash={};
					if (response.Status) {
						$scope.flash.message = 'Successfully added!';						
						bootbox.alert(messages.saved,function(){
							$scope.flash.status = true;
							$scope.flash.type = 'alert-success ';
							$location.path('insuranceplantype/benefits');
							$scope.$apply();
						}) 
						
					} else {
						if(response.redirect){
							bootbox.alert(messages.TryLater,function(){
									$location.path('/');
									$scope.$apply();
								}) 
						}
						$scope.flash.message = response.Message;
						$scope.flash.status = true;
						$scope.flash.type = 'alert-danger';
						$scope.dataLoading = false;
						$("html").animate({scrollTop:0},500);
					}
				});
			}				
        };
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li><a href="insuranceplantype/benefits">Benefits</a></li><li class="active">'+$scope.title+'</li>'; 	
		$scope.$on('$includeContentLoaded', function(event) {		
			var idleState = false;
			var idleTimer = null;
			$('*').bind('mousemove click mouseup mousedown keydown keypress keyup submit change mouseenter scroll resize dblclick', function () {
				clearTimeout(idleTimer);				
				idleState = false;
				idleTimer = setTimeout(function () {					   
					$location.path('/logout');
					$scope.$apply();
					idleState = true; }, messages.sessionTimeout);
			});
			$("body").trigger("mousemove");
		});
		
       
    }])
	/**
		Add Benefit Cost Controller.
	*/
	.controller('AddBenefitCostController',
    ['$scope', '$rootScope', '$location', 'BenefitService','checkCreds','businessServices','$routeParams','messages',
    function ($scope, $rootScope, $location, BenefitService,checkCreds,businessServices,$routeParams,messages) {      
		$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$rootScope.pageLoading=true;
		$scope.id='';
		$scope.ratingAreas=[];			
		$scope.stateList=[];			
		$scope.ratingAreaCost=[];	
		$scope.benefit={};	
		$scope.ratingAreaId='';		
		$scope.StateCode='';		
		$scope.breadcrumb=true;
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
        } else {
            $scope.loggedIn = false;
			$location.path('/');
        }
		
		$scope.benefit.IsDefault = false;					
		$scope.title='Add/Update Benefit Cost';
		$scope.action='Submit';	
		
		/**
			Get Rating Area Services.
		*/
		/*BenefitService.getRatingArea(function (response) {	
			$rootScope.pageLoading=false;	
			$scope.dataLoading=true;	
			if (response.Status) {
				$scope.ratingAreas=response.List;  
				$scope.dataLoading=false;	
			} else {
				$location.path('insuranceplantype/benefits');
			}			
		});*/
		
		/**
			Get All States Services.
		*/	
		BenefitService.GetAllStates(function (response) {	
			$scope.dataLoading=true;	
			if (response.Status) {
				$scope.stateList=response.States;  
				$scope.dataLoading=false;		
			} else {
				$location.path('insuranceplantype/benefits');
			}			
		});	
		
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li><a href="insuranceplantype/benefits">Benefits</a></li><li class="active">'+$scope.title+'</li>';
		
		
		$scope.updateRatingCost=function(){
			$scope.dataLoading=true;			
			if(!isEmpty($scope.ratingAreaId) && !isEmpty($scope.StateCode)){
				BenefitService.getRatingAreaCost($scope.ratingAreaId,$scope.StateCode,function (response) {				
					if (response.Status) {
						$scope.ratingAreaCost=response.Benefits; 
						$scope.dataLoading=false;
						$('#scroll-area_addbenefit').niceScroll({
							autohidemode: 'false',     // Do not hide scrollbar when mouse out
							cursorborderradius: '12px', // Scroll cursor radius
							background: '#ffffff',     // The scrollbar rail color
							cursorwidth: '10px',       // Scroll cursor width
							cursorcolor: '#3b84f2'     // Scroll cursor color
						});	
					} else {
						$location.path('insuranceplantype/benefits');
						$scope.dataLoading=false;	
					}			
				});	
			}else{
				$scope.ratingAreaCost=[];
				$scope.dataLoading=false;	
			}
		}
		
		/**
			get get Rating Area based on state cost.	
		*/
		$scope.GetRatingAreasState=function(){									
			$scope.dataLoading=true;
			$scope.ratingAreaId='';		
			$scope.ratingAreas=[];	
			if(!isEmpty($scope.StateCode)){
				$scope.requestInProgress=true;
				BenefitService.GetRatingAreasState($scope.StateCode,function(response){				
					if(response.Status == "true") {
						if(response.RatingAreas.length>0){					
							$scope.ratingAreas=response.RatingAreas;					
							$scope.dataLoading=false;
							$scope.requestInProgress=false;	
						}else {
							$scope.dataLoading=false;	
							$scope.requestInProgress=false;	
						}
					}else{					
						$scope.dataLoading=false;	
						$scope.requestInProgress=false;	
					}			
				})
			}else{
				$scope.dataLoading=false;	
			}					
		}
		
		/**
			Update Rating Area Cost Services.
		*/
		$scope.update = function () {
            $scope.dataLoading = true;			
			$scope.Createdby=$scope.customer.id;			
			BenefitService.updateRatingAreaCost($scope.ratingAreaId,$scope.StateCode,$scope.ratingAreaCost,$scope.Createdby, function (response) {	
				$scope.flash={};
				if (response.Status) {
					$scope.flash.message = 'Successfully Updated!';
					$scope.flash.status = true;
					$scope.flash.type = 'alert-success ';						
					$scope.dataLoading = false;
					$location.path('insuranceplantype/benefits');
				} else {
					if(response.redirect){
						bootbox.alert(messages.TryLater,function(){
									$location.path('/');
									$scope.$apply();
								}) 
						$location.path('/');
					}
					$scope.flash.message = response.Message;
					$scope.flash.status = true;
					$scope.flash.type = 'alert-danger';
					$scope.dataLoading = false;
					$("html").animate({scrollTop:0},500);
				}
			});
				
        }; 
		$rootScope.pageLoading=false;	
		$scope.$on('$includeContentLoaded', function(event) {		
			var idleState = false;
			var idleTimer = null;
			$('*').bind('mousemove click mouseup mousedown keydown keypress keyup submit change mouseenter scroll resize dblclick', function () {
				clearTimeout(idleTimer);				
				idleState = false;
				idleTimer = setTimeout(function () {					   
					$location.path('/logout');
					$scope.$apply();
					idleState = true; }, messages.sessionTimeout);
			});
			$("body").trigger("mousemove");
		});
    }]);