'use strict';

angular.module('mhmApp.issuermaster')
.controller('IssuermasterController',
    ['$scope', '$rootScope', '$location', 'IssuermasterService','checkCreds','businessServices','messages','$timeout','$window',
    function ($scope, $rootScope, $location, IssuermasterService,checkCreds,businessServices,messages,$timeout,$window){
	console.log('employercontroller created');
		
		$scope.searchby='';
		$scope.title='Issuer Master';
		$scope.filterByIssuerCode='';
		$scope.sortby='CreatedDateTime';
		$scope.desc=true;		
		$scope.issuermaster=[];
		$scope.TempSearch={};
		$scope.TempSearch.filterByIssuerCode="";			
		$scope.pageSize=messages.pageSize;
		$scope.messages=messages;
		$scope.TotalCount=0;
		$scope.lastCount=0;
		$scope.pageSizes = [10,20,50,100];
		$scope.IsDefaultStatus = {'true':"True",'false':'False'};
		$scope.breadcrumb=true;
		$scope.sessionPage = "PlanattrSession";
		$scope.searchSession = '';
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">Issuer Master</li>'; 
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
				$scope.getIssermasterlist();		
			  }
		}	
		
		IssuermasterService.waitForLayout(function(response){	
			$rootScope.pageLoading=false;						
			
			$scope.dataLoading = false;	
			if($scope.pageSize!=undefined && $scope.pageSize!=''){					
				$scope.page=1;
				
				if($scope.searchSession != '')
					$scope.getIssermasterlist();
				
				$scope.getIssermasterlist();		
			}
					
		});	
		
		$scope.setIssuermasterList=function(){
			$scope.searchby=$scope.TempSearch.searchby;	
			$scope.filterByIssuerCode=$scope.TempSearch.filterByIssuerCode;
			$scope.page=1;	
			$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
			$scope.getIssermasterlist();
		};
		$scope.resetIssuermasterList=function(){
			$scope.TempSearch.searchby=$scope.searchby='';	
			$scope.TempSearch.filterByIssuerCode=$scope.filterByIssuerCode="";	
			$scope.page=1;	
			businessServices.resetSearchSession($scope.sessionPage);
			$scope.getIssermasterlist();
		};
		
				
		$scope.getIssermasterlist=function(){
			$timeout(function(){
				$rootScope.pageLoading=false;
				$scope.dataLoading = true;		
				console.log('api called');
				IssuermasterService.getAllIssuer($scope,function (response) {
					if(response.Status){
						$scope.issuermaster=response.lstIssuerMasters;					
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
			  $scope.getIssermasterlist();
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
				$scope.getIssermasterlist();
			}
		  };
		  
		  $scope.nextPage = function () {
			if ($scope.page < $scope.TotalCount - 1) {			 	
			  $scope.page++;			
			  $scope.getIssermasterlist();
			}
		  };
		$scope.prevPage = function () {
			if ($scope.page > 1) {
				$scope.page--;
				 $scope.getIssermasterlist();
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
['$scope','$rootScope','$location','IssuermasterService','checkCreds','businessServices','messages','$timeout',
function($scope, $location, IssuermasterService, checkCreds, businessServices, messages, $timeout){
	
	$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$rootScope.pageLoading=false;	
		$scope.dataLoading=true;	
		$scope.id='';		
		$scope.issuermaster={};			
					
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
			$scope.title='Issuer #'+ $routeParams.id ;			
			IssuermasterService.get($routeParams.id,function (response) {					
				if (response.Status) {					
                    $scope.issuermaster=response.IssuerMaster; 
					
					$scope.dataLoading=false;	
                } else {
                    $location.path('masters/issuermaster');
                }				
			});				
		}else{
			$location.path('masters/issuermaster');
		}
		
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">'+$scope.title+'</li>'; 	
		
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

.controller('AddIssuerController',
['$scope','$rootScope','$location','IssuermasterService','checkCreds','businessServices','messages','$timeout','$routeParams',
function($scope, $rootScope, $location, IssuermasterService, checkCreds, businessServices, messages, $timeout,$routeParams){
	console.log('create add');
	$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$rootScope.pageLoading=false;	
		$scope.dataLoading=false;	
		$scope.issuermaster={};
		$scope.issuermaster.IssuerCode='';
		$scope.issuermaster.IssuerName='';	
		$scope.issuermaster.Abbreviations='';
		$scope.issuermaster.StateCode='';
		$scope.issuermaster.Status='';
		$scope.issuermaster.Createdby='';
		$scope.issuermaster.CreatedDateTime = new Date();
		$scope.issuermaster.ModifiedBy='';
		$scope.issuermaster.ModifiedDateTime = '';
		
		$scope.title='Add Issuer';	
		$scope.actionBtnText = "Create";
		$scope.action = "add";
		$scope.issuerId = $routeParams.issuerId;
		
		$scope.readOnly = false;
			
		$scope.breadcrumb=true;
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
        } else {
            $scope.loggedIn = false;
			$location.path('/');
        }
				
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">'+$scope.title+'</li>'; 	
		
		IssuermasterService.waitForLayout(function(response){	
			$rootScope.pageLoading=false;						
			$scope.dataLoading = false;	
			$scope.states=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.States:[];
		});
		
		$scope.addIssuer=function(){	
		console.log('issuer add function called');
			if($("#createIssuerForm").valid())
			{
				$scope.formLoading = true;
				var data={};
				
				//Assign created by user id
				if($scope.action == 'add')
					$scope.issuermaster.Createdby = $scope.customer.id;
				else
					$scope.issuermaster.ModifiedBy = $scope.customer.id;

				data=$scope.issuermaster;
			
				IssuermasterService.Action(data, $scope.action, function (response) {	
					if(response.Status) {
						bootbox.alert(messages.saved,function() {
							$location.path('masters/issuermaster');
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
			
			$("#createIssuerForm").validate({
				
				errorElement:'span',
				errorClass:'error',
				onkeyup:false,
				
				rules: {
					
					issuerCode:{
						required:true,	
						maxlength:10,
					},
					issuerName:{
						required:true,
						maxlength:250,
					},
					abbreviations:{
						required:true,
						maxlength:100,
					},
					stateCode:{
						required:true
					},
					status:{
						required:true
					}
				}
			});
			
		});
		
		if($scope.issuerId != "" && $scope.issuerId != undefined)
		{
			$scope.title = "Edit Employer";
			$scope.action = "edit";
			$scope.actionBtnText = "Update";
			console.log('test');
			
			$scope.readOnly = true;
			
			$scope.breadcrumbHTML='<li><a href="">Home</a></li><li><a href="masters/issuermaster">Employers</a></li><li>'+$scope.title+'</li><li class="active">'+$scope.issuerId+'</li>'; 
			
			IssuermasterService.getIssuer($scope.issuerId, function (response) {
				if (response.Status) {					
                    $scope.issuermaster=response.IssuerMaster; 
					
				if($scope.issuermaster.Status)
				$scope.issuermaster.Status = $scope.issuermaster.Status.toString();
				
				if($scope.issuermaster.StateCode)
				$scope.issuermaster.StateCode = $scope.issuermaster.StateCode.toString();
					
					$scope.dataLoading=false;	
                } else {
                    $location.path('masters/issuermaster');
                }				
			});	
			
		}
		
			
}])