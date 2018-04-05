'use strict';
/**
	Created By : Aastha Jain
	Created Date : 16-06-2016
	Start : County Controller List all Counties.
*/
angular.module('mhmApp.county')
.controller('CountyController',
    ['$scope', '$rootScope', '$location', 'CountyService', 'checkCreds', 'businessServices', 'messages', '$timeout', '$window',
    function ($scope, $rootScope, $location, CountyService, checkCreds, businessServices, messages, $timeout, $window) {
        // reset login status	
		var d = new Date();		
		var curr_year = d.getFullYear();						
		$scope.title='County';
		$scope.messages=messages;
		
		$scope.searchby='';
		$scope.searchByState='';
		$scope.searchByCounty='';
		$scope.searchByFipsState='';
		$scope.searchByFipsCountry='';
		$scope.searchByBusinessYear=curr_year.toString();
		
		$scope.TempSearch={};
		$scope.TempSearch.searchByState="";
		$scope.TempSearch.searchByCounty="";	
		$scope.TempSearch.searchByFipsState="";	
		$scope.TempSearch.searchByFipsCountry="";	
		$scope.TempSearch.searchByBusinessYear=curr_year.toString();
		
		$scope.sortby='Id';
		$scope.desc=true;
		$scope.page=1;
		$scope.pageSize=messages.pageSize;
		
		$scope.countieslist=[];
		$scope.years=[];
		
		$scope.TotalCount=0;
		$scope.lastCount=0;
		$scope.breadcrumb=true;
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">County</li>'; 
		$scope.dataLoading = true;	
		
		$scope.sessionPage = "CountySession";
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
				$scope.getCountyList();		
			  }
		}
		
		$scope.$watch('pageSize', function() {			
			  if($scope.pageSize!=undefined && $scope.pageSize!=''){	
					$scope.page=1;
					
					if($scope.searchSession != '')
						$scope.setCountyList();
					
					$scope.getCountyList();		
			  }
		}, true);
		
		for(var i=2015;i<2045;i++){
			$scope.years.push({"val":i});
		}
		
		$scope.setCountyList=function(){
			$scope.searchby=$scope.TempSearch.searchby;
			$scope.searchByState=$scope.TempSearch.searchByState;
			$scope.searchByCounty=$scope.TempSearch.searchByCounty;
			$scope.searchByFipsState=$scope.TempSearch.searchByFipsState;
			$scope.searchByFipsCountry=$scope.TempSearch.searchByFipsCountry;
			$scope.searchByBusinessYear=$scope.TempSearch.searchByBusinessYear;
			$scope.page=1;
			$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
			$scope.getCountyList();
		}
		
		$scope.resetCountyList=function(){
			$scope.TempSearch.searchby=$scope.searchby='';	
			$scope.TempSearch.searchByState=$scope.searchByState='';
			$scope.TempSearch.searchByCounty=$scope.searchByCounty='';
			$scope.TempSearch.searchByFipsState=$scope.searchByFipsState='';
			$scope.TempSearch.searchByFipsCountry=$scope.searchByFipsCountry='';
			$scope.TempSearch.searchByBusinessYear=$scope.searchByBusinessYear=curr_year.toString();	
			$scope.page=1;
			businessServices.resetSearchSession($scope.sessionPage);
			$scope.getCountyList();
		}
		
				
		$scope.getCountyList=function(){
			$timeout(function(){
				$rootScope.pageLoading=false;
				$scope.dataLoading = true;				
				CountyService.getAll($scope,function (response) {
					if(response.Status){
						$scope.countieslist=response.FIPS_County;					
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
						$scope.countrieslist=[];
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
		  $scope.getCountyList();
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
				$scope.getCountyList();
			}
		};
		  
		$scope.nextPage = function () {
			if ($scope.page < $scope.TotalCount - 1) {			 	
			  $scope.page++;			
			  $scope.getCountyList();
			}
		};
		
		$scope.prevPage = function () {
			if ($scope.page > 1) {
				$scope.page--;
				 $scope.getCountyList();
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
	
	/** End : County Controller List all Counties. **/