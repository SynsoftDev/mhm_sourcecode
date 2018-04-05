'use strict';
/**
	Created By : Aastha Jain
	Created Date : 13-06-2016
	Start : Rating Area Controller List all Rating Area.
*/
angular.module('mhmApp.ratingarea')
.controller('RatingAreaController',
    ['$scope', '$rootScope', '$location', 'RatingAreaService', 'checkCreds', 'businessServices', 'messages', '$timeout', '$window',
    function ($scope, $rootScope, $location, RatingareaService, checkCreds, businessServices, messages, $timeout, $window) {
        // reset login status	
		var d = new Date();		
		var curr_year = d.getFullYear();						
		$scope.title='Rating Area';
		$scope.messages=messages;
		
		$scope.searchby='';	//country name and 3 digit zip code
		$scope.searchByStateCode='';
		$scope.searchByRatingArea='';
		$scope.searchByMarketCover='';
		$scope.searchByBusinessYear='';
		
		$scope.TempSearch={};
		$scope.TempSearch.searchByStateCode="";			
		$scope.TempSearch.searchByRatingArea="";			
		$scope.TempSearch.searchByMarketCover="";	
		$scope.TempSearch.searchByBusinessYear='';
		
		$scope.sortby='Id';
		$scope.desc=true;
		$scope.page=1;
		$scope.pageSize=messages.pageSize;
		
		$scope.ratingareaslist=[];
		$scope.states=[];
		$scope.ratingareas=[];
		$scope.marketCoverages=[];
		$scope.years=[];
		
		$scope.TotalCount=0;
		$scope.lastCount=0;
		$scope.breadcrumb=true;
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">Rating Area</li>'; 
		$scope.dataLoading = true;	
		
		$scope.sessionPage = "RatingAreaSession";
		$scope.searchSession = '';
		
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
		$scope.popup1 = {opened: false};		
		
		$scope.onPageSize=function(){
			 if($scope.pageSize!=undefined && $scope.pageSize!=''){
				$scope.page=1;
				$scope.getRatingServiceList();		
			  }
		}
		
		RatingareaService.waitForLayout(function(response){
			$rootScope.pageLoading=false;
			$scope.dataLoading = false;	
			
			$scope.states=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.States:[];
			
			$scope.ratingareas=((typeof response[1].data !='undefined') && (response[1].data.Status))?response[1].data.List:[];
			
			$scope.marketCoverages=((typeof response[2].data !='undefined') && (response[2].data.Status))?response[2].data.lstMarketCoverage:[];
			
			if($scope.pageSize!=undefined && $scope.pageSize!=''){					
				$scope.page=1;
				
				if($scope.searchSession != '')
					$scope.setRatingServiceList();
				
				$scope.getRatingServiceList();		
			}							
		});	
		
		for(var i=2015;i<2045;i++){
			$scope.years.push({"val":i});
		}
		
		$scope.setRatingareaList=function(){
			$scope.searchby=$scope.TempSearch.searchby;
			$scope.searchByStateCode=$scope.TempSearch.searchByStateCode;			
			$scope.searchByRatingArea=$scope.TempSearch.searchByRatingArea;			
			$scope.searchByMarketCover=$scope.TempSearch.searchByMarketCover;	
			$scope.searchByBusinessYear=$scope.TempSearch.searchByBusinessYear;
			$scope.page=1;	
			$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
			$scope.getRatingServiceList();
		}
		
		$scope.resetRatingareaList=function(){
			$scope.TempSearch.searchby=$scope.searchby='';	
			$scope.TempSearch.searchByStateCode=$scope.searchByStateCode='';			
			$scope.TempSearch.searchByRatingArea=$scope.searchByRatingArea='';
			$scope.TempSearch.searchByMarketCover=$scope.searchByMarketCover='';
			$scope.TempSearch.searchByBusinessYear=$scope.searchByBusinessYear='';	
			$scope.page=1;	
			businessServices.resetSearchSession($scope.sessionPage);
			$scope.getRatingServiceList();
		}
		
				
		$scope.getRatingServiceList=function(){
			$timeout(function(){
				$rootScope.pageLoading=false;
				$scope.dataLoading = true;				
				RatingareaService.getAll($scope,function (response) {
					if(response.Status){
						$scope.ratingareaslist=response.RatingAreas;					
						$scope.TotalCount=response.TotalCount;
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
						if(typeof response.Message!='undefined'){								
							bootbox.alert(response.Message);
							$scope.dataLoading = false;	
						}
						$scope.ratingareaslist=[];
					}	
				});
			})
			
		}
		
		// change sorting order
		$scope.sort_by = function(newSortingOrder) {
		if ($scope.sortby == newSortingOrder)
		  $scope.desc = !$scope.desc;			
		  $scope.sortby = newSortingOrder;
		  $scope.page=1;	
		  $scope.getRatingServiceList();
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
				$scope.getRatingServiceList();
			}
		};
		  
		$scope.nextPage = function () {
			if ($scope.page < $scope.TotalCount - 1) {			 	
			  $scope.page++;			
			  $scope.getRatingServiceList();
			}
		};
		
		$scope.prevPage = function () {
			if ($scope.page > 1) {
				$scope.page--;
				 $scope.getRatingServiceList();
			}
		};
		
		$scope.openPopUP = function () {			
			if($(".adv_srch_btn_box").css('display')=='none'){
				$(".adv_srch_btn_box").show();		
			}else{
				$(".adv_srch_btn_box").hide();		
			}
		}
		
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
	
	/** End : Rating Area Controller List all Rating Area. **/