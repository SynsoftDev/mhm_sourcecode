'use strict';

angular.module('mhmApp.employermaster')
.controller('EmployermasterController',
    ['$scope', '$rootScope', '$location', 'EmployermasterService','checkCreds','businessServices','messages','$timeout','$window',
    function ($scope, $rootScope, $location, EmployermasterService,checkCreds,businessServices,messages,$timeout,$window){
	console.log('employercontroller created');
		
		$scope.searchby='';
		$scope.title='Employer Master';
		$scope.filterByEmployerId='';
		$scope.sortby='CreatedDateTime';
		$scope.desc=true;		
		$scope.employermaster=[];
		$scope.TempSearch={};
		$scope.TempSearch.filterByEmployerId="";			
		$scope.pageSize=messages.pageSize;
		$scope.messages=messages;
		$scope.TotalCount=0;
		$scope.lastCount=0;
		$scope.pageSizes = [10,20,50,100];
		$scope.IsDefaultStatus = {'true':"True",'false':'False'};
		$scope.breadcrumb=true;
		$scope.sessionPage = "PlanattrSession";
		$scope.searchSession = '';
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">Employer Master</li>'; 
		$scope.dataLoading = true;	
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
		
		$scope.onPageSize=function(){
			 if($scope.pageSize!=undefined && $scope.pageSize!=''){
				$scope.page=1;
				$scope.getEmployermasterlist();		
			  }
		}	
		
		EmployermasterService.waitForLayout(function(response){	
			$rootScope.pageLoading=false;						
			
			$scope.dataLoading = false;	
			if($scope.pageSize!=undefined && $scope.pageSize!=''){					
				$scope.page=1;
				
				if($scope.searchSession != '')
					$scope.setEmployermasterList();
				
				$scope.getEmployermasterlist();		
			}
					
		});	
		
		$scope.setEmployermasterList=function(){
			$scope.searchby=$scope.TempSearch.searchby;	
			$scope.filterByEmployerId=$scope.TempSearch.filterByEmployerId;
			$scope.page=1;	
			$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
			$scope.getEmployermasterlist();
		};
		$scope.resetEmployermasterList=function(){
			$scope.TempSearch.searchby=$scope.searchby='';	
			$scope.TempSearch.filterByEmployerId=$scope.filterByEmployerId="";	
			$scope.page=1;	
			businessServices.resetSearchSession($scope.sessionPage);
			$scope.getEmployermasterlist();
		};
		
				
		$scope.getEmployermasterlist=function(){
			$timeout(function(){
				$rootScope.pageLoading=false;
				$scope.dataLoading = true;		
				console.log('api called');
				EmployermasterService.getAll($scope,function (response) {
					if(response.Status){
						$scope.employermaster=response.EmployerMsts;					
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
						$scope.employermaster=[];
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
			  $scope.getEmployermasterlist();
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
		};
		  $scope.setPage = function () {
			if (isValidPageNumber(this.n)) {
				$scope.page = this.n;
				$scope.getEmployermasterlist();
			}
		  };
		  
		  $scope.nextPage = function () {
			if ($scope.page < $scope.TotalCount - 1) {			 	
			  $scope.page++;			
			  $scope.getEmployermasterlist();
			}
		  };
		$scope.prevPage = function () {
			if ($scope.page > 1) {
				$scope.page--;
				 $scope.getEmployermasterlist();
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

.controller('GetEmployerController',
['$scope','$rootScope','$location','EmployermasterService','checkCreds','businessServices','messages','$timeout',
function($scope, $location, EmployermasterService, checkCreds, businessServices, messages, $timeout){
	
	$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$rootScope.pageLoading=false;	
		$scope.dataLoading=true;	
		$scope.id='';		
		$scope.employermaster={};			
					
		$scope.breadcrumb=true;
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
        } else {
            $scope.loggedIn = false;
			$location.path('/');
        }
		if($routeParams.id){
			$scope.id = $routeParams.id;		
			$scope.title='Employer #'+ $routeParams.id ;			
			EmployermasterService.get($routeParams.id,function (response) {					
				if (response.Status) {					
                    $scope.employermaster=response.Employer; 
					
					$scope.dataLoading=false;	
                } else {
                    $location.path('masters/employermaster');
                }				
			});				
		}else{
			$location.path('masters/employermaster');
		}
		
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li><a href="masters/employermaster">Plan Attribute</a></li><li class="active">'+$scope.title+'</li>'; 	
		
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
		
		// EmployermasterService.waitForLayoutView(function(response){	
			// $rootScope.pageLoading=false;						
			// $scope.dataLoading = false;	
		// });
	
}])

.controller('AddEmployerController',
['$scope','$rootScope','$location','EmployermasterService','checkCreds','businessServices','messages','$timeout','$routeParams',
function($scope, $rootScope, $location, EmployermasterService, checkCreds, businessServices, messages, $timeout,$routeParams){
	console.log('create add');
	$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$rootScope.pageLoading=false;	
		$scope.dataLoading=false;	
		$scope.employermaster={};
		$scope.employermaster.EmployerId='0';
		$scope.employermaster.EmployerName='';	
		$scope.employermaster.CreatedDateTime = new Date();
		$scope.title='Add Employer';	
		$scope.actionBtnText = "Create";
		$scope.action = "add";
		$scope.employerId = $routeParams.employerId;
					
		$scope.breadcrumb=true;
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
        } else {
            $scope.loggedIn = false;
			$location.path('/');
        }
				
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li><a href="masters/employermaster">Employers</a></li><li class="active">'+$scope.title+'</li>'; 	
		
		$scope.addEmployer=function(){	
		console.log('employer add function called');
			if($("#createEmployerForm").valid())
			{
				console.log('employer valid');		
				// if($scope.employermaster.EmployerId == '' || $scope.employermaster.EmployerName == '') {
				if($scope.employermaster.EmployerName == '') {
					$scope.formInvalid = true;
					console.log('employer for not valid');	
					return;
				}else {
					$scope.formInvalid = false;
				}
				
				console.log('employer valid 1');			
				$scope.formLoading = true;
				var data={};
				
				//Assign created by user id
				//$scope.employermaster.EmployerId=0;
				$scope.employermaster.Createdby = $scope.customer.id;

			
				data=$scope.employermaster;
			
				EmployermasterService.Action(data, $scope.action, function (response) {	
					if(response.Status) {
						bootbox.alert(messages.saved,function() {
							$location.path('masters/employermaster');
							$scope.$apply();
						});
					} else {
						if(response.redirect){
							bootbox.alert(messages.TryLater,function(){
								$location.path('/');
								$scope.$apply();
							}) 
						}
						$scope.flash={};
						$scope.flash.message = response.Message;
						$scope.flash.status = true;
						$scope.flash.type = 'alert-danger';
						$scope.dataLoading = false;	
						$scope.formLoading = false;
						$("html").animate({scrollTop:0},500);
					}
			});
			}else {
				console.log('employer not valid');		
				if($scope.employermaster.EmployerId == '' || $scope.employermaster.EmployerName == '') {
					$scope.formInvalid = true;
					return;
				}else {
					$scope.formInvalid = false;
				}
			}
		
		
			
		};
		
		
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
			
			$("#createEmployerForm").validate({
				
				errorElement:'span',
				errorClass:'error',
				onkeyup:false,
				
				rules: {
					
					// employerId:{
						// required:true,	
						// maxlength:6,
					// },
					employerName:{
						required:true,
						maxlength:250,
					},
				}
			});
			
		});
		
		if($scope.employerId != "" && $scope.employerId != undefined)
		{
			$scope.title = "Edit Employer";
			$scope.action = "edit";
			$scope.actionBtnText = "Update";
			console.log('test');
			
			$scope.breadcrumbHTML='<li><a href="">Home</a></li><li><a href="masters/employermaster">Employers</a></li><li>'+$scope.title+'</li><li class="active">'+$scope.employerId+'</li>'; 
			
			EmployermasterService.getEmployer($scope.employerId, function (response) {
				if (response.Status) {					
                    $scope.employermaster=response.Employer; 
					
					// if($scope.employermaster.EmployerId)
						// $scope.employermaster.EmployerId = $scope.employermaster.EmployerId.toString();
					
					$scope.dataLoading=false;	
                } else {
                    $location.path('masters/employermaster');
                }				
			});	
			
		}
		
			
}])