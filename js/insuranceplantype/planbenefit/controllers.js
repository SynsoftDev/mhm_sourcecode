'use strict';
/**
	Plan benefit Controller.
*/
angular.module('mhmApp.planbenefit', ['ui.select'])
.controller('PlanbenefitController',
    ['$scope', '$rootScope', '$location', 'PlanbenefitService','checkCreds','businessServices','messages','$timeout','$routeParams',
    function ($scope, $rootScope, $location, PlanbenefitService,checkCreds,businessServices,messages,$timeout,$routeParams) {
        // reset login status	
		var d = new Date();		
		var curr_year = d.getFullYear();						
		$scope.searchby='';
		$scope.title='Plan Benefit';
		$scope.filterByMarketCover="";
		$scope.searchByStateCode="";
		$scope.filterByBenefitName='';

		$scope.searchByPlanId = "";
		$scope.searchByMHMBenefitId = "";
		$scope.searchByIssuerId = "";
		$scope.PlanIds = [];
		$scope.IssuerIds = [];
		$scope.MHMBenefitIds = [];

		$scope.start_date='';
		$scope.end_date='';
		$scope.BusinessYear='';
		$scope.sortby='CreatedDateTime';
		$rootScope.desc=true;		
		$scope.planBenefits=[];
		$scope.categoryList={};
		$scope.TempSearch={};
		$scope.TempSearch.PlanId ="";
		$scope.TempSearch.MHMBenefitId ="";
		$scope.TempSearch.IssuerId ="";
		$scope.TempSearch.BusinessYear='';
		$scope.page=1;
		$scope.years=[];
		$scope.states=[];
		$scope.statesA=[];
		$scope.marketCoverages=[];
		$scope.TempSearch.filterByMarketCover="";			
		$scope.TempSearch.searchByStateCode="";			
		$scope.pageSize=messages.pageSize;
		$scope.TotalCount=0;
		$scope.lastCount=0;
		$scope.pageSizes = [10,20,50,100];
		$scope.IsDefaultStatus = {'true':"True",'false':'False'};
		$scope.breadcrumb=true;
		$scope.sessionPage = "PlanbenefitsSession";
		$scope.searchSession = '';
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li class="active">Plan Benefit</li>'; 
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
		
		$scope.onPageSize=function(){
			 if($scope.pageSize!=undefined && $scope.pageSize!=''){
				$scope.page=1;
				$scope.getPlanbenefitList();		
			  }
		}	
		
		$scope.maxDate = new Date();		
		
		$scope.setPlanbenefitList = function() {
			$scope.searchby = $scope.TempSearch.searchby;	
			
			/*$scope.filterByMarketCover = $scope.TempSearch.filterByMarketCover;
			$scope.searchByStateCode = $scope.TempSearch.searchByStateCode;
			$scope.BusinessYear = $scope.TempSearch.BusinessYear;*/

			$scope.searchByPlanId = $scope.TempSearch.PlanId.PlanId;
			$scope.searchByMHMBenefitId = $scope.TempSearch.MHMBenefitId.MHMBenefitID;
			$scope.searchByIssuerId = $scope.TempSearch.IssuerId.Id;
			$scope.BusinessYear = $scope.TempSearch.BusinessYear.val;

			$scope.page = 1;
			$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
			$scope.getPlanbenefitList();
		}
		$scope.resetPlanbenefitList=function(){
			$scope.TempSearch.searchby = $scope.searchby = '';
			
			/*$scope.TempSearch.filterByMarketCover=$scope.filterByMarketCover="";	
			$scope.TempSearch.searchByStateCode=$scope.searchByStateCode="";	
			$scope.TempSearch.BusinessYear=$scope.BusinessYear='';*/

			$scope.TempSearch.PlanId = $scope.searchByPlanId = "";
			$scope.TempSearch.MHMBenefitId = $scope.searchByMHMBenefitId = "";
			$scope.TempSearch.IssuerId = $scope.searchByIssuerId = "";
			$scope.TempSearch.BusinessYear = $scope.BusinessYear ='';

			$scope.page = 1;
			businessServices.resetSearchSession($scope.sessionPage);
			
			PlanbenefitService.waitForLayout(function(response) {	
				$rootScope.pageLoading=false;
				$scope.dataLoading = false;	
				$scope.states=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.States:[];
				$scope.marketCoverages=((typeof response[1].data !='undefined') && (response[1].data.Status))?response[1].data.lstMarketCoverage:[];
				angular.forEach($scope.states, function(state, key){
					 $scope.statesA[state.StateCode]=state.StateName;
				});			
				if($scope.pageSize!=undefined && $scope.pageSize!='') {					
					$scope.page=1;
					
					if($scope.searchSession != '')
						$scope.setPlanbenefitList();
					
					$scope.getPlanbenefitList();
					$scope.getSearchPlanID();
					$scope.getSearchMHMBenefitId();
					$scope.getSearchIssuerID();
				}
			});
		}
		
				
		$scope.getPlanbenefitList = function() {
			//$scope.searchByPlanId = 'G230';businessyear
			
			if($routeParams.planid)
			$scope.searchByPlanId = $routeParams.planid;
			if($routeParams.businessyear)
			$scope.BusinessYear = $routeParams.businessyear;
				
			$timeout(function() {
				$rootScope.pageLoading=false;
				$scope.dataLoading = true;
				console.log("searched data", $scope);				
				PlanbenefitService.getAll($scope, function (response) {
					if(response.Status) {
						$scope.planBenefits = response.PlanBenefits;
						console.log("all plan benefit", $scope.planBenefits);
						$scope.TotalCount = response.TotalCount;
						$scope.lastCount = Math.ceil($scope.TotalCount/$scope.pageSize);
						$rootScope.pageLoading = false;	
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
						$scope.planBenefits=[];
					}
				});
			})
		}

		/**
			Created By : Aastha Jain
			Created Date : 07-06-2016
			Purpose : Function to export plan benefit list.
			Export Plan Attribute List parameter and call API.
			Start
		*/
		$scope.exportReport = function() {
			$scope.dataLoading = true;
			PlanbenefitService.getPlanbenefitReport($scope,function (responseStatus,response,xhr) {			
				if (responseStatus === 200) {
					var filename = 'PlanBenefitReport.xlsx';
					var disposition = xhr.getResponseHeader('Content-Disposition');
					if (disposition && disposition.indexOf('attachment') !== -1) {
						var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
						var matches = filenameRegex.exec(disposition);							
					}
					var type = xhr.getResponseHeader('Content-Type');

					var blob = new Blob([response], { type: type });
					if (typeof window.navigator.msSaveBlob !== 'undefined') {							
						window.navigator.msSaveBlob(blob, filename);
						$scope.dataLoading=false;
						$scope.$apply();	
					} else {
						var URL = window.URL || window.webkitURL;
						var downloadUrl = URL.createObjectURL(blob);
						if (filename) {								
							var a = document.createElement("a");								
							if (typeof a.download === 'undefined') {
								window.location = downloadUrl;
							} else {
								a.href = downloadUrl;
								a.download = filename;
								document.body.appendChild(a);
								a.click();
							}
						} else {
							window.location = downloadUrl;
						}
						setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100); // cleanup
						$scope.dataLoading=false;
						$scope.$apply();	
					}		
				}else{
					$scope.dataLoading = false;	
					$scope.$apply();
				}				
			});	
		}
		
		
		//Import PlanAttributes 
		$scope.selectedFile = null;  
		$scope.msg = "";  

		$scope.loadFile = function (files) {  
			$scope.$apply(function () {  
				$scope.selectedFile = files[0]; 
				$scope.handleFile(); 
			})  
		}  
			
		$scope.handleFile = function () {  

			var file = $scope.selectedFile;  

			if (file) {  

				var reader = new FileReader();  

				reader.onload = function (e) {  

					if (!e) {
						var data = reader.content;
					}
					else {
						var data = e.target.result;
					}

					var workbook = XLSX.read(data, { type: 'binary' });  

					var first_sheet_name = workbook.SheetNames[0];  

					var dataObjects = XLSX.utils.sheet_to_json(workbook.Sheets[first_sheet_name]);  

					//console.log(excelData);  

					if (dataObjects.length > 0) {  

						$scope.savePlanAttribute(dataObjects);  


					} else {  
						$scope.msg = "Error : Something Wrong !";  
					}  

				}  

				reader.onerror = function (ex) {  
					console.log('ex',ex)
				}  

				reader.readAsBinaryString(file);  
			}  
		}  
		
		 $scope.savePlanAttribute = function (data) {  
			 $scope.dataLoading = true;
    	PlanbenefitService.ImportPlanBenefit(data, function (response) {		
					if (response.Status) {
									bootbox.alert(response.Message, function () {
										$scope.$apply();
									});
								$scope.dataLoading = false;
							} else {
								if (response.redirect) {
									bootbox.alert(messages.TryLater, function () {
										$location.path('/');
										$scope.$apply();
									})
								}
								$scope.flash = {};
								$scope.flash.message = response.Message;
								$scope.flash.status = true;
								$scope.flash.type = 'alert-danger';
								$scope.dataLoading = false;
								$scope.formLoading = false;
								$("html").animate({ scrollTop: 0 }, 500);
							}
				});
				}
		
		/**
			End : Function to export plan benefit list.
		**/
		
		PlanbenefitService.waitForLayout(function(response){	
			$rootScope.pageLoading=false;
			$scope.dataLoading = false;	
			$scope.states=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.States:[];
			$scope.marketCoverages=((typeof response[1].data !='undefined') && (response[1].data.Status))?response[1].data.lstMarketCoverage:[];
			angular.forEach($scope.states, function(state, key){
				 $scope.statesA[state.StateCode]=state.StateName;
			});			
			if($scope.pageSize!=undefined && $scope.pageSize!=''){					
				$scope.page=1;
				
				if($scope.searchSession != '')
					$scope.setPlanbenefitList();
				
				$scope.getPlanbenefitList();
				$scope.getSearchPlanID();
				$scope.getSearchMHMBenefitId();
				$scope.getSearchIssuerID();
			}
		});


		$scope.getSearchPlanID = function() {
			var searchedID = {searchByPlanId: ""};
			PlanbenefitService.GetSearchPlanID(searchedID, function (response) {
				if (response.Status) {
					console.log("plan id", response);
					$scope.PlanIds = response.PlanIds;
				}
			});
		};

		$scope.getSearchMHMBenefitId = function() {
			var searchedID = {searchByMHMBenefitId: ""};
			PlanbenefitService.GetSearchMHMId($scope, function (response) {
				if (response.Status) {
					console.log("mhm id", response);
					$scope.MHMBenefitIds = response.MHMBenefitIds;
				}
			});
		};

		$scope.getSearchIssuerID = function() {
			var searchedID = {searchByIssuerId: ""};
			PlanbenefitService.GetSearchIssuerID(searchedID, function (response) {
				if (response.Status) {
					console.log("issuer id ", response);
					$scope.IssuerIds = response.CarrierIds;
				}
			});
		};

		 // change sorting order
		  $scope.sort_by = function(newSortingOrder) {
			if ($scope.sortby == newSortingOrder)
				$scope.desc = !$scope.desc;
			else
				$scope.desc = !$scope.desc;
			$scope.sortby = newSortingOrder;
			$scope.page = 1;
			$scope.getPlanbenefitList();
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
				$scope.getPlanbenefitList();
			}
		};
		  
		$scope.nextPage = function () {
			if ($scope.page < $scope.TotalCount - 1) {
			  $scope.page++;
			  $scope.getPlanbenefitList();
			}
		};

		$scope.prevPage = function () {
			if ($scope.page > 1) {
				$scope.page--;
				 $scope.getPlanbenefitList();
			}
		};
		$rootScope.pageLoading=false;	
		
		for(var i=2015;i<2045;i++){
			$scope.years.push({"val":i});
		}
		  
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
			$('#wizard').smartWizard({ enableAllSteps: true });
		});

		$scope.getMoreResultPlanId = function(value) {
			var searchedID = {searchByPlanId: value};
			console.log("searched data", searchedID);

			PlanbenefitService.GetSearchPlanID(searchedID, function (response) {
				if (response.Status) {
					console.log("new plan id", response);
					$scope.PlanIds = response.PlanIds;
					// $scope.PlanIds[0]['PlanId']="38396ME0710032-04";
				}
			});
		};

		$scope.getMoreResultIssuerId = function(value) {
			var searchedID = {searchByIssuerId: value};
			console.log("searched data", searchedID);

			PlanbenefitService.GetSearchIssuerID(searchedID, function (response) {
				if (response.Status) {
					console.log("new issuer id", response);
					$scope.IssuerIds = response.CarrierIds;
					// $scope.IssuerIds[0]['Id']="38396ME0710032-04";
				}
			});
		};

    }])
	.filter('propsFilter', function() {
	  return function(items, props) {
	    var out = [];
	    /*if (angular.isArray(items) && props) {
	    	var itemMatches = false;
	    	var searchedItem = props;
	    	var patt = new RegExp(searchedItem);

	    	for(var i=0; i<items.length; i++) {
	    		var res = patt.test(items[i]);
	    		if (res) {
	    			out.push(items[i]);
	    			console.log("item match");
	    		} else {
	    			console.log("item not match");
	    		}
	    	}

	    	if (out.length > 0) {
	    		console.log(out.length);
	    		return out;
	    	} else {
	    		return [];
	    	}
	    } else {
	    	return items;
	    }*/

	    if (angular.isArray(items)) {
	        var keys = Object.keys(props);
	        items.forEach(function(item) {
	            var itemMatches = false;

	            for (var i = 0; i < keys.length; i++) {
	            	var prop = keys[i];
	              	var text = props[prop].toLowerCase();
	              	if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
	                	itemMatches = true;
	                	break;
	              	}
	            }

	            if (itemMatches) {
	            	out.push(item);
	            }
	        });
	    } else {
			out = items;
	    }

	    return out;

	  };
	})
	
	.controller('GetPlanbenefitController',
    ['$scope', '$rootScope', '$location', 'PlanbenefitService','checkCreds','businessServices','$routeParams','$filter','messages',
    function ($scope, $rootScope, $location, PlanbenefitService,checkCreds,businessServices,$routeParams,$filter,messages) {
        $scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		$rootScope.pageLoading=false;	
		$scope.dataLoading=true;	
		$scope.id = '';		
		$scope.planBenefit={};
		$scope.years = [];
		$scope.conditionalExp = [];
		$scope.eventType = '';
		$scope.planBenefit.MHMBenefitId = "";
		$scope.sortby = $routeParams.sortby;
		$scope.desc = $routeParams.desc;
		$scope.CreatedBy='';		
		$scope.ModifiedBy='';
		$scope.breadcrumb=true;

		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
        } else {
            $scope.loggedIn = false;
			$location.path('/');
        }

		/*if($routeParams.id){
			$scope.id = $routeParams.id;		
			$scope.title='Plan Benefit #'+ $routeParams.id ;			
			PlanbenefitService.get($routeParams.id,function (response) {					
				if (response.Status) {					
                    $scope.planBenefit=response.PlanBenefits;
					$scope.dataLoading=false;	
                } else {
                    $location.path('insuranceplantype/planbenefit');
                }				
			});				
		}else{
			$location.path('insuranceplantype/planbenefit');
		}*/

		if($routeParams.id && $routeParams.sortby && $routeParams.desc) {
			$scope.dataLoading = true;
			$scope.id = $routeParams.id;
			$scope.title ='Plan Benefit #'+ $routeParams.id;
			$scope.data = {'id': $routeParams.id, 'eventType': $scope.eventType, 'sortby': $routeParams.sortby, 'desc': $routeParams.desc};
			PlanbenefitService.getData($scope.data, function (response) {					
				if (response.Status) {
					console.log("response", response);
                    $scope.planBenefit = response.PlanBenefits;
                    $scope.NextRecord = response.NextRecord;
                    $scope.PreviousRecord = response.PreviousRecord;
					$scope.PlanAttributeId = response.PlanAttributeId;
					
                    if (!$scope.PreviousRecord) {
                    	$scope.prevButton = true;
                    }
                    if (!$scope.NextRecord) {
                    	$scope.nextButton = true;
                    }
					$scope.planBenefit.IsCovered = $scope.planBenefit.IsCovered ? $scope.planBenefit.IsCovered.toString() : "false";
					$scope.planBenefit.IsExclFromOonMOOP = $scope.planBenefit.IsExclFromOonMOOP ? $scope.planBenefit.IsExclFromOonMOOP.toString() : "false";
					$scope.planBenefit.IsExclFromInnMOOP = $scope.planBenefit.IsExclFromInnMOOP ? $scope.planBenefit.IsExclFromInnMOOP.toString() :"false";
					$scope.planBenefit.QuantLimitOnSvc = $scope.planBenefit.QuantLimitOnSvc ? $scope.planBenefit.QuantLimitOnSvc.toString() : "false";
					$scope.planBenefit.MHMBenefitId = $scope.planBenefit.MHMBenefitId ? $scope.planBenefit.MHMBenefitId.toString() : "";
					
					$scope.planBenefit.MaxQtyBeforeCoPay=$scope.planBenefit.MaxQtyBeforeCoPay ? $scope.planBenefit.MaxQtyBeforeCoPay : '0';
					$scope.planBenefit.LimitQty=$scope.planBenefit.LimitQty ? $scope.planBenefit.LimitQty : '0';
					$scope.CreatedBy=response.CreatedBy;
					$scope.ModifiedBy=response.ModifiedBy;
					
					if($scope.planBenefit.CreatedDateTime){
						$scope.planBenefit.CreatedDateTime = $filter('date')(new Date($scope.planBenefit.CreatedDateTime),'MM/dd/yyyy HH:mm:ss');	
					}
					if($scope.planBenefit.ModifiedDateTime){
						$scope.planBenefit.ModifiedDateTime = $filter('date')(new Date($scope.planBenefit.ModifiedDateTime),'MM/dd/yyyy HH:mm:ss');	
					}

					$scope.dataLoading = false;
				} else {
                    $location.path('insuranceplantype/planbenefit');
                }
			});
		}else{
			$location.path('insuranceplantype/planbenefit');
		}
		
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li><a href="insuranceplantype/planbenefit">Plan Benefit</a></li><li class="active">'+$scope.title+'</li>'; 	
		
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
			$('#wizard').smartWizard({ enableAllSteps: true });
		});
		
		PlanbenefitService.waitForLayoutView(function(response){	
			/*$rootScope.pageLoading=false;						
			$scope.marketCoverages=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.lstMarketCoverage:[];
			$scope.states=((typeof response[1].data !='undefined') && (response[1].data.Status))?response[1].data.States:[];
			$scope.dataLoading = false;	*/
			
			$rootScope.pageLoading=false;
			$scope.dataLoading=true;			
			$scope.marketCoverages=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.lstMarketCoverage:[];
			$scope.states=((typeof response[1].data !='undefined') && (response[1].data.Status))?response[1].data.States:[];
			$scope.mhmBenefitIds = ((typeof response[2].data !='undefined') && (response[2].data.Status)) ? response[2].data.MHMBenefitIds:[];

			$scope.mhmBenefitIds.unshift({MHMBenefitID: 0, MHMBenefitName: "None"});

			if($scope.planBenefit.MHMBenefitId!="") {
				$scope.dataLoading = false;
			}
		});

		for(var i=2015;i<2045;i++){
			$scope.years.push({"val":i});
		}

		$scope.conditionalExp.push({"key":true, "val":"True"});
		$scope.conditionalExp.push({"key":false, "val":"False"});
       
    }])
	
	/****
			Created By : Aastha Jain
			Created Date : 22-06-2016
			Start : Edit Plan Benefit Controller.
	****/
	
	.controller('EditPlanbenefitController',
	['$scope', '$rootScope', '$location', 'PlanbenefitService','checkCreds','businessServices','$routeParams','$filter','messages',
    function ($scope, $rootScope, $location, PlanbenefitService,checkCreds,businessServices,$routeParams,$filter,messages) {
        $scope.closeFlash=function(e){
			$scope.flash.status = false;
		}
		
		$rootScope.pageLoading=false;	
		$scope.dataLoading=true;	
		$scope.formInvalid = false;
		$scope.id='';		
		$scope.planBenefit={};	
		$scope.years = [];
		$scope.conditionalExp = [];
		$scope.marketCoverages = [];
		$scope.mhmBenefitIds = [];
		$scope.states = [];
		$scope.breadcrumb=true;
		$scope.eventType = '';
		$scope.planBenefit.MHMBenefitId = "";
		$scope.sortby = $routeParams.sortby;
		$scope.desc = $routeParams.desc;
		$scope.CreatedBy = '';		
		$scope.ModifiedBy = '';
		$scope.CostSharingTypes = [];
		$scope.LimitUnits = [];
		$scope.title ='Plan Benefit';
		$scope.PlanIds = [];
		$scope.IssuerIds = [];
		$scope.BusinessYears = [];
		
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
        } else {
            $scope.loggedIn = false;
			$location.path('/');
        }
console.log('2')
        if ($routeParams.action == 'copy') {
        	$scope.readOnly = false;
        	$scope.actionBtnText = "Create";
        	$scope.showBtn = true;
        	$scope.hideFields = true;
        } else {
        	$scope.readOnly = true;
        	$scope.actionBtnText = "Update";
        	$scope.showBtn = false;
        	$scope.title ='Plan Benefit #'+ $routeParams.id;
        }
		
			if($routeParams.id) {
			$scope.dataLoading=true;
			$scope.id = $routeParams.id;
			$scope.sortby = $routeParams.sortby ? $routeParams.sortby : null;
			$scope.desc = $routeParams.desc ? $routeParams.desc : null;
			$scope.eventType = $scope.eventType ? $scope.eventType : null;
			$scope.data = {'id': $routeParams.id, 'eventType': $scope.eventType, 'sortby': $scope.sortby, 'desc': $scope.desc};
			console.log("====================", $scope.data);
			PlanbenefitService.getData($scope.data, function (response) {					
				if (response.Status) {
					console.log("plan benefit", response);
                    $scope.planBenefit = response.PlanBenefits;
                    $scope.NextRecord = response.NextRecord;
                    $scope.PreviousRecord = response.PreviousRecord;
					$scope.PlanAttributeId = response.PlanAttributeId;
					$scope.ApprovalStatus = response.ApprovalStatus;
					
                    if (!$scope.PreviousRecord) {
                    	$scope.prevButton = true;
                    }
                    if (!$scope.NextRecord) {
                    	$scope.nextButton = true;
                    }
                    $scope.planBenefit.IsCovered = $scope.planBenefit.IsCovered ? $scope.planBenefit.IsCovered.toString() : "false";
					$scope.planBenefit.IsExclFromOonMOOP = $scope.planBenefit.IsExclFromOonMOOP ? $scope.planBenefit.IsExclFromOonMOOP.toString() : "false";
					$scope.planBenefit.IsExclFromInnMOOP = $scope.planBenefit.IsExclFromInnMOOP ? $scope.planBenefit.IsExclFromInnMOOP.toString() : 'false';
					$scope.planBenefit.QuantLimitOnSvc = $scope.planBenefit.QuantLimitOnSvc ? $scope.planBenefit.QuantLimitOnSvc.toString() : "false";
					// $scope.planBenefit.IsEHB = $scope.planBenefit.IsEHB.toString();
					// $scope.planBenefit.IsSubjToDedTier1 = $scope.planBenefit.IsSubjToDedTier1 ? $scope.planBenefit.IsSubjToDedTier1.toString() : "";
					$scope.planBenefit.MHMBenefitId = $scope.planBenefit.MHMBenefitId ? $scope.planBenefit.MHMBenefitId.toString() : '';
					$scope.planBenefit.MaxQtyBeforeCoPay = $scope.planBenefit.MaxQtyBeforeCoPay ? $scope.planBenefit.MaxQtyBeforeCoPay : 0;
					$scope.planBenefit.LimitQty = $scope.planBenefit.LimitQty ? $scope.planBenefit.LimitQty : 0;

					if($scope.planBenefit.CreatedDateTime){
						$scope.planBenefit.CreatedDateTime = $filter('date')(new Date($scope.planBenefit.CreatedDateTime),'MM/dd/yyyy HH:mm:ss');	
					}
					if($scope.planBenefit.ModifiedDateTime){
						$scope.planBenefit.ModifiedDateTime = $filter('date')(new Date($scope.planBenefit.ModifiedDateTime),'MM/dd/yyyy HH:mm:ss');	
					}

					$scope.CreatedBy = response.CreatedBy;
					$scope.ModifiedBy = response.ModifiedBy;

					if ($routeParams.action == 'copy') {
						$scope.planBenefit.CreatedDateTime = new Date();
					}

					$scope.BusinessYears.push($scope.planBenefit.BusinessYear);

					$scope.planBenefit.PlanIdNew = {PlanId: $scope.planBenefit.PlanId, PlanMarketingName: response.PlanMarketingName};

					$scope.dataLoading = false;
				} else {
                    $location.path('insuranceplantype/planbenefit');
                }
			});
		}else{
			$location.path('insuranceplantype/planbenefit');
		}
		
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li><a href="insuranceplantype/planbenefit">Plan Benefit</a></li><li class="active">'+$scope.title+'</li>'; 	
		
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
			$('#wizard').smartWizard({ enableAllSteps: true });
$.validator.setDefaults({ ignore: '' });
			$("#createPlanBenefitForm").validate({
				errorElement: 'span',
				errorClass: 'error',
				onkeyup: false,
				rules: {
					planId: {
						required: true
					},
					year: {
						required: true
					},
					benefitName: {
						required: true
					},
					benefitNum: {
						maxlength: 10,
					},
					maxQtyBeforeCopay: {
						maxlength: 10,
					},
					copayInnTier1: {
						maxlength: 16,
					},
					coinsInnTier1: {
						maxlength: 16,
					},
					coinsMaxAmt: {
						maxlength: 16,
					},
					copayInnTier2: {
						maxlength: 16,
					},
					coinsInnTier2: {
						maxlength: 16,
					},
					maxCoinsInnTier1Amt: {
						maxlength: 16,
					},
					maxCoinsInnTier2Amt: {
						maxlength: 16,
					},
					copayOutofNet: {
						maxlength: 16,
					},
					coinsOutofNet: {
						maxlength: 16,
					},
					limitQty: {
						maxlength: 10,
					},
					PlanBenNotes: {
						maxlength: 1000,	
					},
					SBCText: {
						maxlength: 4000,	
					},
				}
			});
		});
		
		for(var i=2015;i<2045;i++){
			$scope.years.push({"val":i});
		}
		
		$scope.conditionalExp.push({"key":true, "val":"True"});
		$scope.conditionalExp.push({"key":false, "val":"False"});
		
		PlanbenefitService.waitForLayoutEdit(function(response) {
			$rootScope.pageLoading=false;	
			$scope.dataLoading=true;
			$scope.marketCoverages=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.lstMarketCoverage:[];
			$scope.states=((typeof response[1].data !='undefined') && (response[1].data.Status))?response[1].data.States:[];
			$scope.mhmBenefitIds = ((typeof response[2].data !='undefined') && (response[2].data.Status)) ? response[2].data.MHMBenefitIds:[];
			$scope.CostSharingTypes = ((typeof response[3].data !='undefined') && (response[3].data.Status)) ? response[3].data.CostSharingTypes:[];
			$scope.LimitUnits = ((typeof response[4].data !='undefined') && (response[4].data.Status)) ? response[4].data.LimitUnits:[];

			$scope.mhmBenefitIds.unshift({MHMBenefitID: 0, MHMBenefitName: "None"});

			if($scope.planBenefit.MHMBenefitId!="") {
				$scope.dataLoading = false;
			}

			$scope.getSearchPlanID();
			$scope.getSearchIssuerID();
		});

		$scope.getSearchPlanID = function() {
			var searchedID = {searchByPlanId: ""};
			PlanbenefitService.GetSearchPlanID(searchedID, function (response) {
				if (response.Status) {
					console.log("plan id", response);
					$scope.PlanIds = response.PlanIds;
				}
			});
		};

		$scope.getSearchIssuerID = function() {
			var searchedID = {searchByIssuerId: ""};
			PlanbenefitService.GetSearchIssuerID(searchedID, function (response) {
				if (response.Status) {
					console.log("issuer id ", response);
					$scope.IssuerIds = response.CarrierIds;
				}
			});
		};

		$scope.setValue = function() {
			if ($scope.planBenefit.PlanIdNew != null) {
				$scope.BusinessYears = $scope.planBenefit.PlanIdNew.BusinessYear;
				console.log("bussiness year", $scope.planBenefit.PlanIdNew);
				if ($scope.BusinessYears.length == 1) {
					$scope.planBenefit.BusinessYear = $scope.planBenefit.PlanIdNew.BusinessYear.toString();
				} else {
					$scope.planBenefit.BusinessYear = "";
				}

				$scope.PlanAttributeId = $scope.planBenefit.PlanIdNew.Id;
				$scope.planBenefit.IssuerName = $scope.planBenefit.PlanIdNew.IssuerName;
				$scope.planBenefit.IssuerId = $scope.planBenefit.PlanIdNew.IssuerId;
			}
		};
		
				var IsChangesOccur = false;
		
		$scope.ChangesOccur = function(){
			IsChangesOccur=true;
			console.log('changes occur',IsChangesOccur);
		}
		
		$scope.navigate = function (type) {
			if(type === 'close'){
				if (IsChangesOccur!=false) {
					bootbox.dialog({
						message: 'Do you want to save benefit?',
						title: "",
						buttons: {
							success: {
								label: "Yes",
								className: "btn-success",
								callback: function () {
									$scope.formLoading = true;
									$scope.editplanbenefitnavigate(type, function () {
										// $location.path('insuranceplantype/planbenefit');
										// $scope.formLoading = false;
										// $scope.$apply();
									});
								}
							},
							danger: {
								label: "No",
								className: "btn-danger",
								callback: function () {
									$location.path('insuranceplantype/planbenefit');
									$scope.formLoading = false;
									$scope.$apply();
								}
							}
						}
					});

				} else {
					$scope.formLoading = false;
					$location.path('insuranceplantype/planbenefit');
				}
			}
			else if(type === 'planattribute'){
				console.log('$scope.planBenefit',$scope.planBenefit.PlanId)
				if (IsChangesOccur!=false) {
					bootbox.dialog({
						message: 'Do you want to save benefit?',
						title: "",
						buttons: {
							success: {
								label: "Yes",
								className: "btn-success",
								callback: function () {
									$scope.formLoading = true;
									$scope.editplanbenefitnavigate(type, function () {
										// $location.path('insuranceplantype/planattribute/'+$scope.planBenefit.PlanId+'/'+$scope.planBenefit.BusinessYear);
										// $scope.formLoading = false;
										// $scope.$apply();
									});
								}
							},
							danger: {
								label: "No",
								className: "btn-danger",
								callback: function () {
									$location.path('insuranceplantype/editplanattribute/'+$scope.PlanAttributeId+'/CreatedDateTime/true');
									$scope.formLoading = false;
									$scope.$apply();
								}
							}
						}
					});

				} else {
					$scope.formLoading = false;
					$location.path('insuranceplantype/editplanattribute/'+$scope.PlanAttributeId+'/CreatedDateTime/true');
				}
			}
		}
		
		/**
			Palne Benefit Update api.
		*/
		$scope.editplanbenefit = function() {
			$scope.formLoading = true;
			if ($("#createPlanBenefitForm").valid()) {
				if ($scope.planBenefit.PlanIdNew == '' || $scope.planBenefit.PlanIdNew == null || $scope.planBenefit.PlanIdNew == undefined) {
						$scope.formInvalid = true;
						return;
					} else {
						$scope.formInvalid = false;
					}
				$scope.planBenefit.PlanId = $scope.planBenefit.PlanIdNew ? $scope.planBenefit.PlanIdNew.PlanId : '';
				if ($routeParams.action == 'copy') {
					if ($scope.planBenefit.LimitQty > 0 && !$scope.planBenefit.LimitUnit) {
						bootbox.alert(messages.limitUnit, function() {
							$scope.$apply();
						});
						return;
					} 
					
						var data = {};
						var action = 0;
						console.log('$scope.planBenefit',$scope.planBenefit)
						data = $scope.planBenefit;
						data.Createdby =  $scope.customer.id;
						data.ModifiedBy = '';
						console.log('********Copy data', data);
						$scope.dataLoading = true;	
						PlanbenefitService.AddPlanBenefit(data, function (response) {
							if(response.Status) {
								if (response.Status.toString() == "false") {
									bootbox.alert(response.Message, function() {
										$scope.$apply();
									});
									$scope.dataLoading = false;	
								} else {
									bootbox.alert(messages.saved,function() {
										$location.path('insuranceplantype/planbenefit');
										$scope.$apply();
									});
								}
							} else {
								if(response.redirect){
									bootbox.alert(messages.TryLater,function() {
										$location.path('/');
										$scope.$apply();
									});
								}
								$scope.flash = {};
								$scope.flash.message = response.Message;
								$scope.flash.status = true;
								$scope.flash.type = 'alert-danger';
								$scope.dataLoading = false;	
								$scope.formLoading = false;
			                    $("html").animate({scrollTop:0},500);
			                }
						});
				
				} else {
					// if ($scope.ApprovalStatus === 5 && IsChangesOccur) {
					// 	bootbox.alert('This Benefit plan status is in production and cannot be edited', function() {
					// 		$scope.$apply();
					// 	});
					// 	return;
					// }
					
					if ($scope.planBenefit.LimitQty > 0 && !$scope.planBenefit.LimitUnit && IsChangesOccur) {
						bootbox.alert(messages.limitUnit, function() {
							$scope.$apply();
						});
						return;
					} 
					
						var data = {};
						var action = 0;
						data = $scope.planBenefit;
						data.ModifiedBy = $scope.customer.id;
						console.log("update data", data);
						$scope.dataLoading = true;	
						PlanbenefitService.UpdatePlanBenefit(data, function (response) {
							if(response.Status) {
								console.log("nadkjnkjasdn", response);
								if(response.Status == "true") {
									bootbox.alert(messages.saved,function() {
										$location.path('insuranceplantype/planbenefit');
										$scope.$apply();
									});
								} else {
									bootbox.alert(response.Message,function() {
										$scope.$apply();
									});
									$scope.dataLoading=false;
								}
							} else {
								if(response.redirect){
									bootbox.alert(messages.TryLater,function() {
										$location.path('/');
										$scope.$apply();
									});
								}
								$scope.flash = {};
								$scope.flash.message = response.Message;
								$scope.flash.status = true;
								$scope.flash.type = 'alert-danger';
								$scope.dataLoading = false;	
								$scope.formLoading = false;
			                    $("html").animate({scrollTop:0},500);
			                }
						});
				
				}
			}
			else
			{
				if ($scope.planBenefit.PlanIdNew == '' || $scope.planBenefit.PlanIdNew == null || $scope.planBenefit.PlanIdNew == undefined) {
						$scope.formInvalid = true;
						return;
					} else {
						$scope.formInvalid = false;
					}
			}
		};

		/**
			Palne Benefit Update api navigate.
		*/
		$scope.editplanbenefitnavigate = function(type) {
			$scope.formLoading = true;
			if ($("#createPlanBenefitForm").valid()) {
				if ($scope.planBenefit.PlanIdNew == '' || $scope.planBenefit.PlanIdNew == null || $scope.planBenefit.PlanIdNew == undefined) {
						$scope.formInvalid = true;
						return;
					} else {
						$scope.formInvalid = false;
					}
				$scope.planBenefit.PlanId = $scope.planBenefit.PlanIdNew ? $scope.planBenefit.PlanIdNew.PlanId : '';
				if ($routeParams.action == 'copy') {
					
					if ($scope.planBenefit.LimitQty > 0 && !$scope.planBenefit.LimitUnit) {
						bootbox.alert(messages.limitUnit, function() {
							$scope.$apply();
						});
						return;
					} 
					
						var data = {};
						var action = 0;
						console.log('$scope.planBenefit',$scope.planBenefit)
						data = $scope.planBenefit;
						data.Createdby =  $scope.customer.id;
						data.ModifiedBy = '';
						console.log('********Copy data', data);
						$scope.dataLoading = true;	
						PlanbenefitService.AddPlanBenefit(data, function (response) {
							if(response.Status) {
								if (response.Status.toString() == "false") {
									bootbox.alert(response.Message, function() {
										$scope.$apply();
									});
									$scope.dataLoading = false;	
								} else {
									bootbox.alert(messages.saved,function() {
										if(type === 'close'){
											$location.path('insuranceplantype/planbenefit');	
										}
										else if(type === 'planattribute'){
											$location.path('insuranceplantype/editplanattribute/'+$scope.PlanAttributeId+'/CreatedDateTime/true');
										}
										$scope.$apply();
									});
								}
							} else {
								if(response.redirect){
									bootbox.alert(messages.TryLater,function() {
										$location.path('/');
										$scope.$apply();
									});
								}
								$scope.flash = {};
								$scope.flash.message = response.Message;
								$scope.flash.status = true;
								$scope.flash.type = 'alert-danger';
								$scope.dataLoading = false;	
								$scope.formLoading = false;
			                    $("html").animate({scrollTop:0},500);
			                }
						});
				
				} else {
					// if ($scope.ApprovalStatus === 5) {
					// 	bootbox.alert('This Benefit plan status is in production and cannot be edited', function() {
					// 		$scope.$apply();
					// 	});
					// 	return;
					// }
					
					if ($scope.planBenefit.LimitQty > 0 && !$scope.planBenefit.LimitUnit) {
						bootbox.alert(messages.limitUnit, function() {
							$scope.$apply();
						});
						return;
					} 
					
						var data = {};
						var action = 0;
						data = $scope.planBenefit;
						data.ModifiedBy = $scope.customer.id;
						console.log("update data", data);
						$scope.dataLoading = true;	
						PlanbenefitService.UpdatePlanBenefit(data, function (response) {
							if(response.Status) {
								console.log("nadkjnkjasdn", response);
								if(response.Status == "true") {
									bootbox.alert(messages.saved,function() {
										if(type === 'close'){
											$location.path('insuranceplantype/planbenefit');	
										}
										else if(type === 'planattribute'){
											$location.path('insuranceplantype/editplanattribute/'+$scope.PlanAttributeId+'/CreatedDateTime/true');
										}
										$scope.$apply();
									});
								} else {
									bootbox.alert(response.Message,function() {
										$scope.$apply();
									});
									$scope.dataLoading=false;
								}
							} else {
								if(response.redirect){
									bootbox.alert(messages.TryLater,function() {
										$location.path('/');
										$scope.$apply();
									});
								}
								$scope.flash = {};
								$scope.flash.message = response.Message;
								$scope.flash.status = true;
								$scope.flash.type = 'alert-danger';
								$scope.dataLoading = false;	
								$scope.formLoading = false;
			                    $("html").animate({scrollTop:0},500);
			                }
						});
				
				}
			}
			else
			{
				if ($scope.planBenefit.PlanIdNew == '' || $scope.planBenefit.PlanIdNew == null || $scope.planBenefit.PlanIdNew == undefined) {
						$scope.formInvalid = true;
						return;
					} else {
						$scope.formInvalid = false;
					}
			}
		};
		
		$scope.resetMHMBenefitId = function() {
			var data = {};
			data.BenefitNum = $scope.planBenefit.BenefitNum;
			data.IssuerId = $scope.planBenefit.IssuerId;
			PlanbenefitService.GetDefaultBenefitId(data, function(response) {
				if (response.Status) {
					$scope.planBenefit.MHMBenefitId = response.MHMBenefitId.toString();
				} else {
					$location.path('insuranceplantype/planbenefit');
				}
			});
		};

		$scope.getMoreResultPlanId = function(value) {
			var searchedID = {searchByPlanId: value};
			console.log("searched data", searchedID);

			PlanbenefitService.GetSearchPlanID(searchedID, function (response) {
				if (response.Status) {
					console.log("new plan id", response);
					$scope.PlanIds = response.PlanIds;
				}
			});
		};
       
    }])


	/****
		Created By : Rahul Singh
		Created Date: 21-04-2016
		Start: Create New Planbenefit Controller.
	****/

	.controller('NewPlanbenefitController', ['$scope', '$rootScope', '$location', 'PlanbenefitService','checkCreds','businessServices','$routeParams','$filter','messages', function($scope, $rootScope, $location, PlanbenefitService,checkCreds,businessServices,$routeParams,$filter,messages) {

		$scope.closeFlash=function(e) {
			$scope.flash.status = false;
		}
		
		$rootScope.pageLoading=false;	
		$scope.dataLoading=true;	
		$scope.id='';		
		$scope.planBenefit={};	
		$scope.years = [];
		$scope.conditionalExp = [];
		$scope.formInvalid = false;
		$scope.marketCoverages = [];
		$scope.mhmBenefitIds = [];
		$scope.states = [];
		$scope.breadcrumb=true;
		$scope.eventType = '';
		$scope.planBenefit.MHMBenefitId = "";
		$scope.sortby = $routeParams.sortby;
		$scope.desc = $routeParams.desc;
		$scope.CreatedBy = '';		
		$scope.ModifiedBy = '';
		$scope.CostSharingTypes = [];
		$scope.LimitUnits = [];
		$scope.PlanIds = [];
		$scope.IssuerIds = [];
		$scope.BusinessYears = [];
		
		if (checkCreds() === true) {
            $scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();	
        } else {
            $scope.loggedIn = false;
			$location.path('/');
        }
console.log('1')
        $scope.readOnly = false;
    	$scope.actionBtnText = "Create";
    	$scope.showBtn = true;
    	$scope.title ='New Plan Benefit';
    	$scope.hideFields = true;
		
		$scope.breadcrumbHTML='<li><a href="">Home</a></li><li><a href="insuranceplantype/planbenefit">Plan Benefit</a></li><li class="active">'+$scope.title+'</li>'; 	
		
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
			$('#wizard').smartWizard({ enableAllSteps: true });
$.validator.setDefaults({ ignore: '' });
			$("#createPlanBenefitForm").validate({
				errorElement: 'span',
				errorClass: 'error',
				onkeyup: false,
				rules: {
					planId: {
						required: true
					},
					year: {
						required: true
					},
					benefitName: {
						required: true
					},
					benefitNum: {
						maxlength: 10,
					},
					maxQtyBeforeCopay: {
						maxlength: 10,
					},
					copayInnTier1: {
						maxlength: 16,
					},
					coinsInnTier1: {
						maxlength: 16,
					},
					coinsMaxAmt: {
						maxlength: 16,
					},
					copayInnTier2: {
						maxlength: 16,
					},
					coinsInnTier2: {
						maxlength: 16,
					},
					maxCoinsInnTier1Amt: {
						maxlength: 16,
					},
					maxCoinsInnTier2Amt: {
						maxlength: 16,
					},
					copayOutofNet: {
						maxlength: 16,
					},
					coinsOutofNet: {
						maxlength: 16,
					},
					limitQty: {
						maxlength: 10,
					},
					PlanBenNotes: {
						maxlength: 1000,	
					},
					SBCText: {
						maxlength: 4000,	
					},
				}
			});
		});
		
		for(var i=2015;i<2045;i++) {
			$scope.years.push({"val":i});
		}
		
		$scope.conditionalExp.push({"key":true, "val":"True"});
		$scope.conditionalExp.push({"key":false, "val":"False"});
		
		PlanbenefitService.waitForLayoutEdit(function(response) {
			$rootScope.pageLoading=false;	
			$scope.dataLoading=false;
			$scope.marketCoverages=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.lstMarketCoverage:[];
			$scope.states=((typeof response[1].data !='undefined') && (response[1].data.Status))?response[1].data.States:[];
			$scope.mhmBenefitIds = ((typeof response[2].data !='undefined') && (response[2].data.Status)) ? response[2].data.MHMBenefitIds:[];
			$scope.CostSharingTypes = ((typeof response[3].data !='undefined') && (response[3].data.Status)) ? response[3].data.CostSharingTypes:[];
			$scope.LimitUnits = ((typeof response[4].data !='undefined') && (response[4].data.Status)) ? response[4].data.LimitUnits:[];

			$scope.mhmBenefitIds.unshift({MHMBenefitID: 0, MHMBenefitName: "None"});

			if($scope.planBenefit.MHMBenefitId!="") {
				$scope.dataLoading = false;
			}

			$scope.getSearchPlanID();
			$scope.getSearchIssuerID();
		});
		
		/**
			Palne Benefit Update api.
		*/
		$scope.editplanbenefit = function() {
			$scope.formLoading = true;
			if ($("#createPlanBenefitForm").valid()) {
					if ($scope.planBenefit.PlanIdNew == '' || $scope.planBenefit.PlanIdNew == null || $scope.planBenefit.PlanIdNew == undefined) {
						$scope.formInvalid = true;
						return;
					} else {
						$scope.formInvalid = false;
					}
				// $scope.planBenefit.PlanId = $scope.planBenefit.PlanId ? $scope.planBenefit.PlanId.PlanId : '';
				$scope.planBenefit.PlanId = $scope.planBenefit.PlanIdNew ? $scope.planBenefit.PlanIdNew.PlanId : '';
				if ($scope.planBenefit.LimitQty > 0 && !$scope.planBenefit.LimitUnit) {
					bootbox.alert(messages.limitUnit, function() {
						$scope.$apply();
					});
				} else {
					var data = {};
					// $scope.planBenefit.IssuerName = $scope.planBenefit.IssuerName.split(':');
					// $scope.planBenefit.IssuerName = $scope.planBenefit.IssuerName[0];
					data = $scope.planBenefit;
					data.CreatedBy =  $scope.customer.id;
					// data.ModifiedBy = $scope.customer.id;
					console.log("update=============", data);
					$scope.dataLoading = true;	
					PlanbenefitService.AddPlanBenefit(data, function(response) {
						if (response.Status) {
							if (response.Status.toString() == "false") {
									bootbox.alert(response.Message, function() {
										$scope.$apply();
									});
									$scope.dataLoading = false;	
								} else {
									bootbox.alert(messages.saved,function() {
										$location.path('insuranceplantype/planbenefit');
										$scope.$apply();
									});
								}
						} else {
							if(response.redirect) {
								bootbox.alert(messages.TryLater,function() {
									$location.path('insuranceplantype/planbenefit');
									$scope.$apply();
								});
							}
							$scope.flash = {};
							$scope.flash.message = response.Message;
							$scope.flash.status = true;
							$scope.flash.type = 'alert-danger';
							$scope.dataLoading = false;	
							$scope.formLoading = false;
		                    $("html").animate({scrollTop:0},500);
		                }
					});
				}
			}
			else
			{
				if ($scope.planBenefit.PlanIdNew == '' || $scope.planBenefit.PlanIdNew == null || $scope.planBenefit.PlanIdNew == undefined) {
						$scope.formInvalid = true;
						return;
					} else {
						$scope.formInvalid = false;
					}
			}
		};

		$scope.getSearchPlanID = function() {
			var searchedID = {searchByPlanId: ""};
			PlanbenefitService.GetSearchPlanID(searchedID, function (response) {
				if (response.Status) {
					console.log("plan id", response);
					$scope.PlanIds = response.PlanIds;
				}
			});
		};

		$scope.setValue = function() {
			if ($scope.planBenefit.PlanIdNew != null) {
				$scope.BusinessYears = $scope.planBenefit.PlanIdNew.BusinessYear;
				console.log("bussiness year", $scope.planBenefit.PlanIdNew);
				if ($scope.BusinessYears.length == 1) {
					$scope.planBenefit.BusinessYear = $scope.planBenefit.PlanIdNew.BusinessYear.toString();
				} else {
					$scope.planBenefit.BusinessYear = "";
				}
				
				$scope.PlanAttributeId = $scope.planBenefit.PlanIdNew.Id;
				$scope.planBenefit.IssuerName = $scope.planBenefit.PlanIdNew.IssuerName;
				$scope.planBenefit.IssuerId = $scope.planBenefit.PlanIdNew.IssuerId;
			}
		};

		$scope.getMoreResultPlanId = function(value) {
			var searchedID = {searchByPlanId: value};
			console.log("searched data", searchedID);

			PlanbenefitService.GetSearchPlanID(searchedID, function (response) {
				if (response.Status) {
					console.log("new plan id", response);
					$scope.PlanIds = response.PlanIds;
				}
			});
		};

		$scope.getSearchIssuerID = function() {
			var searchedID = {searchByIssuerId: ""};
			PlanbenefitService.GetSearchIssuerID(searchedID, function (response) {
				if (response.Status) {
					console.log("issuer id ", response);
					$scope.IssuerIds = response.CarrierIds;
				}
			});
		};

	}]);
	
	/** End : Edit Plan benefit Controller. **/