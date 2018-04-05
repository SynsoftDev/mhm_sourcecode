'use strict';
/**
	User Controller List all users.
*/
angular.module('mhmApp.users')
/**
	Login Controller.
*/
.controller('LoginController',
    ['$scope', '$rootScope', '$location', 'UserService','checkCreds','businessServices','messages',
    function ($scope, $rootScope, $location, UserService,checkCreds,businessServices,messages) {
        // reset login status
		$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$scope.submitted=true;
		
		$scope.breadcrumb=true;
		$rootScope.pageLoading=true;
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">Login</li>';  
       
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();
			businessServices.deleteOtherSearchSession();
			$location.path('cases');	
        } else {
            $scope.loggedIn = false;
        }
		$scope.email='';
		$scope.password='';
		/**
			Login Services.
		*/
        $scope.login = function () {
			$scope.submitted=true;			
			if(!$scope.form.$invalid){
				$scope.dataLoading = true;
				UserService.Login($scope.email, $scope.password, function (response) {				
					if (response.Status) {
						UserService.SetCredentials($scope.email, $scope.password,response.Customer,response.Token);
						$location.path('cases');
					} else {
						if(response.redirect){
							bootbox.alert(messages.TryLater,function(){
									$location.path('/');
									$scope.$apply();
								}) 
						}
						$scope.flash = {};
						$scope.password ='';
						$scope.flash.message = response.Message;
						$scope.flash.status = true;
						$scope.flash.type = 'alert-danger';
						$scope.dataLoading = false;
						$("html").animate({scrollTop:0},500);
						$scope.submitted=true;
					}										
				});
			}
        };
		$rootScope.pageLoading=false;
    }])
	/**
		Forgot Password Controller.
	*/
	.controller('ForgotPasswordController',
    ['$scope', '$rootScope', '$location', 'UserService','checkCreds','businessServices','messages',
    function ($scope, $rootScope, $location, UserService,checkCreds,businessServices,messages) {
        // reset login status
		$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$scope.submitted=true;
		$scope.breadcrumb=true;
		$rootScope.pageLoading=true;
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">Forgot Password</li>';
		$scope.emailSent=false;
		$scope.error="";	
		$scope.messages=messages;	
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
			businessServices.deleteOtherSearchSession();
			$location.path('cases');
        } else {
            $scope.loggedIn = false;
        }
		/**
			Forgot Password Services.
		*/
        $scope.forgotPassword = function () {
			$scope.submitted=true;			
			if(!$scope.form.$invalid){				
				$scope.dataLoading = true;
				UserService.ForgotPassword($scope.email, function (response) {
					$scope.flash={};
					if (response.Status) {
						$scope.emailSent=true;
						$scope.flash.message = messages.ForgotPasswordSuccess;
						$scope.flash.status = true;
						$scope.flash.type = 'alert-success ';
						$scope.dataLoading = false;	
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
						$scope.submitted=true;
					}
					
				});
			}
        };
		$rootScope.pageLoading=false;
    }]) 
	/**
		Set Password Controller.
	*/
	.controller('SetPasswordController',
    ['$scope', '$rootScope', '$location', 'UserService','checkCreds','businessServices','$routeParams','messages',
    function ($scope, $rootScope, $location, UserService,checkCreds,businessServices,$routeParams,messages) {
		$rootScope.pageLoading=true;
		$scope.submitted=true;
		
		if(!$routeParams.id){
			$location.path('/');		
		}
		$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
        // reset login status
		$scope.breadcrumb=true;
		$scope.messages=messages;	
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">Set Password</li>';
		$scope.emailSent=false;  
		$scope.error="";		
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();
			businessServices.deleteOtherSearchSession();
			$location.path('/');	
        } else {
            $scope.loggedIn = false;
        }
		/**
			Check Email Status Services.
		*/
		UserService.CheckEmailStatus($routeParams.id,function(response){
			if(!response.Status) {
				bootbox.alert(response.Message,function(){
					$location.path('/');
					$scope.$apply();
				}) 				               
			}else{
				$rootScope.pageLoading=false;
			}
		})
		/**
			set Password Services.
		*/		
        $scope.setPassword = function () {
			$scope.submitted=true;			
			if(!$scope.form.$invalid){	
				$scope.dataLoading = true;
				UserService.SetPassword($routeParams.id,$scope.Password,$scope.ConfirmPassword, function (response) {		
					if (response.Status) {
						$scope.emailSent=true; 
						$location.path('/'); 
						$scope.dataLoading=false;					
						
					} else {
						if(response.redirect){
							bootbox.alert(response.Message,function(){
								$location.path('/');
								$scope.$apply();
							}) 	
						}
						$scope.flash={};
						$scope.Password='';					
						$scope.ConfirmPassword='';	
						$scope.flash.message = response.Message;
						$scope.flash.status = true;
						$scope.flash.type = 'alert-danger';
						$scope.dataLoading = false;
						$("html").animate({scrollTop:0},500);   
						$scope.submitted=true;	
					}
				});
			}
        };
		
    }])
	/**
		Change Password Controller.
	*/
	.controller('ChangePasswordController',
    ['$scope', '$rootScope', '$location', 'UserService','checkCreds','businessServices','messages',
    function ($scope, $rootScope, $location, UserService,checkCreds,businessServices,messages) {
        // reset login status
		$scope.breadcrumb=true;
		$scope.submitted=true;
		$rootScope.pageLoading=true;
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">Change Password</li>'; 
		$scope.emailSent=false;
		$scope.messages=messages;  
		$scope.error="";
		$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();
			businessServices.deleteOtherSearchSession();			
        } else {
            $scope.loggedIn = false;
        }	
		/**
			Change Password Services.
		*/
        $scope.changePassword = function () {
			$scope.submitted=true;			
			if(!$scope.form.$invalid){
				$scope.dataLoading = true;
				UserService.ChangePassword($scope.customer.Email,$scope.OldPassword,$scope.NewPassword,$scope.ConfirmPassword, function (response) {		
					if (response.Status) {						
						bootbox.alert(messages.UpdatePassword,function(){
							$location.path('cases');   
							$scope.$apply();
						})				                 	
					} else {
						if(response.redirect){
							bootbox.alert(response.Message,function(){
								$location.path('/');
								$scope.$apply();
							}) 						
						}
						$scope.flash={};					
						if(response.Message=='Incorrect password.'){
							response.Message=messages.IncorrectOldPassword;	
						}
						$scope.OldPassword='';					
						$scope.NewPassword='';					
						$scope.ConfirmPassword='';
						$scope.flash.message = response.Message;
						$scope.flash.status = true;
						$scope.flash.type = 'alert-danger';
						$scope.dataLoading = false;
						$("html").animate({scrollTop:0},500);
						$scope.submitted=true;
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
		});
		$rootScope.pageLoading=false;
    }])
	/**
		Users Controller (get all users).
	*/
	.controller('UsersController',
    ['$scope', '$rootScope', '$location', 'UserService','checkCreds','businessServices','messages',
    function ($scope, $rootScope, $location, UserService,checkCreds,businessServices,messages) {
        // reset login status
		$rootScope.pageLoading=true;
		$scope.breadcrumb=true;
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">Users</li>'; 
		$scope.searchby='';
		$scope.searchByRole="";
		$scope.searchByName="";
		$scope.searchByEmail="";
		$scope.searchByIsActive="";
		$scope.searchByLockedStatus="";
		$scope.start_date='';
		$scope.end_date='';
		$scope.sortby='CreateDate';
		$scope.desc=true;		
		$scope.page=1;
		$scope.users=[];
		$scope.pageSize=messages.pageSize;
		$scope.messages=messages;
		$scope.TotalCount=0;
		$scope.lastCount=0;
		$scope.TempSearch={};
		$scope.pageSizes = [10,20,50,100];	
		
		$scope.sessionPage = "UserSession";
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
			$location.path('/');
        } 
		
		$scope.userStatus={'true':"Yes","false":"No"};
		$scope.userLockedStatusList={'true':"Yes","false":"No"};
		/**
			Change Status services.
		*/
		$scope.changeStatus=function(status,e){				
			bootbox.confirm($(e.target).attr('data-alert'),function(response){
				if(response){
					var ID=e.target.id;					
					UserService.ChangeUserStatus(status,ID,function (response) {					
						$scope.flash={};
						if (response.Status) {
							$scope.flash.message = 'Successfully Updated.';
							$scope.flash.status = true;
							$scope.flash.type = 'alert-success ';
							$scope.getUsersList();
							//$scope.$apply();
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
							$scope.$apply();
						}				
					});
				}
			});
		}
		/**
			Unlock User services.
		*/
		$scope.UnlockUser=function(e){
			bootbox.confirm(messages.confirmUnlock,function(response){
				if(response){
					var ID=e.target.id;				
					UserService.UnlockUser(ID,function (response) {					
						$scope.flash={};
						if (response.Status) {
							$scope.flash.message = 'Successfully Updated.';
							$scope.flash.status = true;
							$scope.flash.type = 'alert-success ';
							$scope.getUsersList();
							//$scope.$apply();
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
							$scope.$apply();
						}				
					});
				}
			});
		}
		$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
			
		/** Set condition fro reason For Cancellation  */			
		$scope.$watch('pageSize', function() {			
			  if($scope.pageSize!=undefined && $scope.pageSize!=''){	
					$scope.page=1;
					
					if($scope.searchSession != '')
						$scope.setUsersList();
					
					$scope.getUsersList();		
			  }
		}, true);
		
		
		$scope.setUsersList=function(){
			$rootScope.pageLoading=false;
			$scope.dataLoading = true;
			$scope.searchby=$scope.TempSearch.searchby;	
			$scope.searchByName=$scope.TempSearch.searchByName;	
			$scope.searchByEmail=$scope.TempSearch.searchByEmail;	
			$scope.searchByRole=$scope.TempSearch.searchByRole;	
			$scope.searchByIsActive=$scope.TempSearch.searchByIsActive;	
			$scope.searchByLockedStatus=$scope.TempSearch.searchByLockedStatus;	
			$scope.page = 1;
			$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
			$scope.getUsersList();
		}
		$scope.resetUsersList=function(){
			$rootScope.pageLoading=false;
			$scope.dataLoading = true;
			$scope.searchby=$scope.TempSearch.searchby='';	
			$scope.searchByName=$scope.TempSearch.searchByName='';	
			$scope.searchByEmail=$scope.TempSearch.searchByEmail='';	
			$scope.searchByRole=$scope.TempSearch.searchByRole='';	
			$scope.searchByIsActive=$scope.TempSearch.searchByIsActive='';	
			$scope.searchByLockedStatus=$scope.TempSearch.searchByLockedStatus='';	
			$scope.page = 1;
			businessServices.resetSearchSession($scope.sessionPage);
			$scope.getUsersList();
		}
		
		$scope.getUsersList=function(){
			$rootScope.pageLoading=false;
			$scope.dataLoading = true;		
			UserService.getAll($scope,function (response) {
				if(response.Status){
					$scope.users=response.Users;
					$scope.TotalCount=response.TotalCount;
					$scope.lastCount=Math.ceil($scope.TotalCount/$scope.pageSize);					
					$rootScope.pageLoading=false;	
					$scope.dataLoading = false;	
				}else{
					if(response.redirect){
						bootbox.alert(messages.TryLater,function(){
							$location.path('/');
							$scope.$apply();
						}) 
					}
					$scope.users=[];
				}	
			});
		}
		 // change sorting order
		  $scope.sort_by = function(newSortingOrder) {
			if ($scope.sortby == newSortingOrder)
			  $scope.desc = !$scope.desc;			
			  $scope.sortby = newSortingOrder;
			  $scope.page = 1;
			  $scope.getUsersList();
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
				$scope.getUsersList();
			}
		  };
		  
		  $scope.nextPage = function () {
			if ($scope.page < $scope.TotalCount - 1) {
			  $scope.page++;
			  $scope.getUsersList();
			}
		  };
		  $scope.prevPage = function () {
			if ($scope.page > 1) {
				$scope.page--;
				$scope.getUsersList();
			}
		  };
		  
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
		
		$scope.maxDate = new Date();
		//$scope.getUsersList();
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
		Edit User Controller.
	*/
	.controller('EditUserController',
    ['$scope', '$rootScope', '$location', 'UserService','checkCreds','businessServices','$routeParams','messages',
    function ($scope, $rootScope, $location, UserService,checkCreds,businessServices,$routeParams,messages) {
        // reset login status $routeParams.Blogid
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
        } else {
            $scope.loggedIn = false;
			$location.path('/');
        }		
		$scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$scope.submitted=true;
		$rootScope.pageLoading=true;
		$scope.id='';	
		$scope.breadcrumb=true;
		$scope.title='Add New User';
		$scope.action='Submit';		
		$scope.readonly=false;		
		$scope.clientCompanies=[];
		$scope.user={};	
		/**
			Get Client Companies Services.
		*/
		UserService.GetClientCompanies(function (response) {					
				if (response.Status) {                    
					$scope.clientCompanies=response.Companies;					
                } else {
					if(response.redirect){
						bootbox.alert(messages.TryLater,function(){
							$location.path('/');
							$scope.$apply();
						}) 
					}
                    $location.path('users');
                }
				
			});
		if($routeParams.id){
			$scope.id = $routeParams.id;	
			$scope.title='Edit User';
			$scope.action='Update';
			$scope.readonly=true;
			/**
				Get User Services.
			*/			
			UserService.getUser($routeParams.id,function (response) {					
				if (response.Status) {
                    //$location.path('users'); 
					$scope.user=response.Users;
					$scope.$watch('clientCompanies', function() {	
						if(!isEmpty($scope.user.ClientCompanyId)){
							$scope.user.ClientCompanyId=$scope.user.ClientCompanyId.toString();
						}												  
					}, true);
					$rootScope.pageLoading=false;
                } else {
                    $location.path('users');
                }
				
			});
		}else{
			$scope.user={};
			$scope.user.ClientCompanyId='';
		}
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li><a href="users">Users</a></li><li class="active">'+$scope.title+'</li>'; 
		
		$scope.$watch('user.Role', function() {			
			  if(typeof $scope.user!='undefiend' && $scope.user.Role=='Admin'){						
					$scope.user.ClientCompanyId='';		
			  }
		}, true);			
		/**
			Update User Services.
		*/
        $scope.update = function () {
			$scope.submitted=true;			
			if(!$scope.form.$invalid){
				$scope.dataLoading = true;
				if($scope.user.id){
					$scope.user.UserID=$scope.user.id;
					UserService.update($scope.user, function (response) {	
						$scope.flash={};
						if (response.Status) {
							$scope.flash.message = 'Successfully added!';							
							bootbox.alert(messages.saved,function(){
								$scope.flash.status = true;
								$scope.flash.type = 'alert-success ';
								$location.path('users');
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
							$scope.submitted=true;	
						}
					});
				}else{
					$scope.user.CreatedBy=$scope.customer.id;
					//$scope.user.Role='Agent';
					$scope.user.BaseUrl=messages.base_url;
					console.log($scope.user);	
					UserService.add($scope.user, function (response) {	
						$scope.flash={};
						if (response.Status) {
							$scope.flash.message = 'Successfully added!';							
							bootbox.alert(messages.saved,function(){
									$scope.flash.status = true;
									$scope.flash.type = 'alert-success ';
									$location.path('users');
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
							$scope.submitted=true;	
						}
					});
				}	
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
    }]);