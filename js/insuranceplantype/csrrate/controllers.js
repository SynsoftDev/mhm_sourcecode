'use strict';
/**
	Csrrate Controller List all Csrrate.
*/
angular.module('mhmApp.csrrate', ['ui.select'])
	.controller('CsrrateController',
	['$scope', '$rootScope', '$location', 'CsrrateService', 'checkCreds', 'businessServices', 'messages', '$timeout','$routeParams',
		function ($scope, $rootScope, $location, CsrrateService, checkCreds, businessServices, messages, $timeout, $routeParams) {
			// reset login status	
			var d = new Date();
			var curr_year = d.getFullYear();
			$scope.searchby = '';
			$scope.title = 'Rates';
			$scope.searchByStateCode = "";
			$scope.filterByBenefitName = '';
			$scope.filterByMarketCover = '';
			$scope.filterByMetalLevel = '';
			$scope.start_date = '';
			$scope.end_date = '';
			// $scope.BusinessYear = curr_year.toString();
			$scope.BusinessYear = "";
			$scope.sortby = 'CreatedDateTime';
			$rootScope.desc = true;
			$scope.CSRRates = [];
			$scope.categoryList = {};
			$scope.TempSearch = {};
			// $scope.TempSearch.BusinessYear=curr_year.toString();
			$scope.TempSearch.BusinessYear = "";
			$scope.page = 1;
			$scope.years = [];
			$scope.states = [];
			$scope.statesA = [];
			$scope.marketCoverages = [];
			$scope.metalLevels = [];
			$scope.ratingAreaId = [];
			$scope.TempSearch.searchByStateCode = "";
			$scope.TempSearch.filterByMarketCover = "";
			$scope.TempSearch.filterByMetalLevel = "";

			$scope.TempSearch.RatingAreaId = "";
			$scope.TempSearch.PlanID = "";
			$scope.RatingAreaId = "";
			$scope.PlanID = "";

			$scope.TempSearch.IssuerId = "";
			$scope.IssuerId = "";
			$scope.issuerIds = [];

			$scope.pageSize = messages.pageSize;
			$scope.TotalCount = 0;
			$scope.lastCount = 0;
			$scope.pageSizes = [10, 20, 50, 100];
			$scope.IsDefaultStatus = { 'true': "True", 'false': 'False' };
			$scope.breadcrumb = true;
			$scope.sessionPage = "RatesSession";
			$scope.searchSession = '';
			$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li class="active">Rates</li>';
			$scope.dataLoading = true;
			if (checkCreds() === true) {
				$scope.loggedIn = true;
				$scope.customer = businessServices.getUsername();

				$scope.searchSession = businessServices.getSearchSession($scope.sessionPage);
				if ($scope.searchSession != '') {
					$scope.TempSearch = $scope.searchSession;
				}

				businessServices.deleteOtherSearchSession($scope.sessionPage);

			} else {
				$scope.loggedIn = false;
				$location.path('/login');
			}

			$scope.closeFlash = function (e) {
				$scope.flash.status = false;
			}

			$scope.startDate = {};
			$scope.endDate = {};
			$scope.startDate.isOpned = false;
			$scope.endDate.isOpned = false;
			$scope.rateEffectiveDate = {};
			$scope.rateEffectiveDate.isOpned = false;

			$scope.open_date = function () {
				$scope.rateEffectiveDate.isOpned = true;
			};
			$scope.open_start = function () {
				$scope.startDate.isOpned = true;
			};
			$scope.open_end = function () {
				$scope.endDate.isOpned = true;
			};
			$scope.$watch('end_date', function () {
				if ($scope.end_date != null) {
					$scope.maxStartDate = angular.copy($scope.end_date);
				} else {
					$scope.maxStartDate = '';
				}

			}, true);
			$scope.$watch('start_date', function () {
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

			$scope.onPageSize = function () {
				if ($scope.pageSize != undefined && $scope.pageSize != '') {
					$scope.page = 1;
					$scope.getCsrrateList();
				}
			}

			$scope.maxDate = new Date();

			$scope.setCsrrateList = function () {
				$scope.searchby = $scope.TempSearch.searchby;
				$scope.BusinessYear = $scope.TempSearch.BusinessYear;

				/*$scope.searchByStateCode=$scope.TempSearch.searchByStateCode;				
				$scope.filterByMarketCover=$scope.TempSearch.filterByMarketCover;				
				$scope.filterByMetalLevel=$scope.TempSearch.filterByMetalLevel;*/

				$scope.RatingAreaId = $scope.TempSearch.RatingAreaId;
				$scope.PlanID = $scope.TempSearch.PlanID;
				$scope.filterByMarketCover = $scope.TempSearch.filterByMarketCover;
				$scope.IssuerId = $scope.TempSearch.IssuerId;

				$scope.page = 1;
				$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
				$scope.getCsrrateList();
			}

			$scope.resetCsrrateList = function () {
				$scope.TempSearch.searchby = $scope.searchby = '';

				/*$scope.TempSearch.searchByStateCode=$scope.searchByStateCode="";	
				$scope.TempSearch.filterByMarketCover=$scope.filterByMarketCover="";	
				$scope.TempSearch.filterByMetalLevel=$scope.filterByMetalLevel="";*/

				$scope.TempSearch.RatingAreaId = $scope.RatingAreaId = "";
				$scope.TempSearch.PlanID = $scope.PlanID = "";
				$scope.TempSearch.filterByMarketCover = $scope.filterByMarketCover = "";
				$scope.TempSearch.IssuerId = $scope.IssuerId = "";

				$scope.TempSearch.BusinessYear = $scope.BusinessYear = "";
				$scope.page = 1;
				businessServices.resetSearchSession($scope.sessionPage);
				$scope.getCsrrateList();
			}

			$scope.getCsrrateList = function () {
				
				if($routeParams.planid)
				$scope.searchby = $routeParams.planid;
				
				$timeout(function () {
					$rootScope.pageLoading = false;
					$scope.dataLoading = true;
					CsrrateService.getAll($scope, function (response) {
						if (response.Status) {
							$scope.CSRRates = response.CSRRates;
							$scope.TotalCount = response.TotalCount;
							$scope.lastCount = Math.ceil($scope.TotalCount / $scope.pageSize);
							$rootScope.pageLoading = false;
							$scope.dataLoading = false;
						} else {
							if (response.redirect) {
								bootbox.alert(response.Message, function () {
									$location.path('/');
									$scope.$apply();
								})
							}
							if (typeof response.Message != 'undefined') {
								bootbox.alert(response.Message);
								$scope.dataLoading = false;
							}
							$scope.CSRRates = [];
						}
					});
				})

			}

			/**
				Created By : Aastha Jain
				Created Date : 07-06-2016
				Purpose : Function to export CSR Rate list.
				Export Plan Attribute List parameter and call API.
				Start
			*/
			$scope.exportReport = function () {
				$scope.dataLoading = true;
				CsrrateService.getCSRrateReport($scope, function (responseStatus, response, xhr) {
					if (responseStatus === 200) {
						var filename = 'CSRrateReport.xlsx';
						var disposition = xhr.getResponseHeader('Content-Disposition');
						if (disposition && disposition.indexOf('attachment') !== -1) {
							var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
							var matches = filenameRegex.exec(disposition);
						}
						var type = xhr.getResponseHeader('Content-Type');

						var blob = new Blob([response], { type: type });
						if (typeof window.navigator.msSaveBlob !== 'undefined') {
							window.navigator.msSaveBlob(blob, filename);
							$scope.dataLoading = false;
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
							$scope.dataLoading = false;
							$scope.$apply();
						}
					} else {
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
    	CsrrateService.ImportCSRRate(data, function (response) {		
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
				End : Function to export CSR Rate list.
			**/

			CsrrateService.waitForLayout(function (response) {
				console.log('waitForLayout calles')
				$scope.dataLoading = false;
				$scope.states = ((typeof response[0].data != 'undefined') && (response[0].data.Status)) ? response[0].data.States : [];
				$scope.marketCoverages = ((typeof response[1].data != 'undefined') && (response[1].data.Status)) ? response[1].data.lstMarketCoverage : [];
				$scope.metalLevels = ((typeof response[2].data != 'undefined') && (response[2].data.Status)) ? response[2].data.lstMarketCoverage : [];
				$scope.ratingAreaId = ((typeof response[3].data != 'undefined') && (response[3].data.Status)) ? response[3].data.RatingAreas : [];
				$scope.issuerIds = ((typeof response[4].data != 'undefined') && (response[4].data.Status)) ? response[4].data.CarrierIds : [];
				console.log('$scope.issuerIds',response)
				angular.forEach($scope.states, function (state, key) {
					$scope.statesA[state.StateCode] = state.StateName;
				});
				$rootScope.pageLoading = false;
				if ($scope.pageSize != undefined && $scope.pageSize != '') {
					$scope.page = 1;

					if ($scope.searchSession != '')
						$scope.setCsrrateList();

					$scope.getCsrrateList();
				}
			});


			// change sorting order
			$scope.sort_by = function (newSortingOrder) {
				if ($scope.sortby == newSortingOrder)
					$scope.desc = !$scope.desc;
				$scope.sortby = newSortingOrder;
				$scope.page = 1;
				$scope.getCsrrateList();
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
					i++;
				}
				return pages;
			};

			$scope.calculatePageNumber = function (i, currentPage, paginationRange, totalPages) {
				var halfWay = Math.ceil(paginationRange / 2);
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
					$scope.getCsrrateList();
				}
			};

			$scope.nextPage = function () {
				if ($scope.page < $scope.TotalCount - 1) {
					$scope.page++;
					$scope.getCsrrateList();
				}
			};
			$scope.prevPage = function () {
				if ($scope.page > 1) {
					$scope.page--;
					$scope.getCsrrateList();
				}
			};
			$rootScope.pageLoading = false;

			for (var i = 2015; i < 2045; i++) {
				$scope.years.push({ "val": i });
			}

			$scope.$on('$includeContentLoaded', function (event) {
				var idleState = false;
				var idleTimer = null;
				$('*').bind('mousemove click mouseup mousedown keydown keypress keyup submit change mouseenter scroll resize dblclick', function () {
					clearTimeout(idleTimer);
					idleState = false;
					idleTimer = setTimeout(function () {
						$location.path('/logout');
						$scope.$apply();
						idleState = true;
					}, messages.sessionTimeout);
				});
				$("body").trigger("mousemove");
				$('#wizard').smartWizard({ enableAllSteps: true });
			});

		}])
		
	.controller('GetCsrrateController',
	['$scope', '$rootScope', '$location', 'CsrrateService', 'checkCreds', 'businessServices', '$routeParams', '$filter', 'messages',
		function ($scope, $rootScope, $location, CsrrateService, checkCreds, businessServices, $routeParams, $filter, messages) {
			// reset login status $routeParams.Blogid
			$scope.closeFlash = function (e) {
				$scope.flash.status = false;
			}
			$rootScope.pageLoading = false;
			$scope.dataLoading = true;
			$scope.id = '';
			$scope.csrate = {};
			$scope.states = [];
			$scope.ratingareas = [];
			$scope.sortby = $routeParams.sortby;
			$scope.desc = $routeParams.desc;
			$scope.eventType = "";
			$scope.years = [];
			$scope.metalLevels = [];
			$scope.marketCoverages = [];
			$scope.CreatedBy = '';
			$scope.ModifiedBy = '';

			$scope.breadcrumb = true;
			if (checkCreds() === true) {
				$scope.loggedIn = true;
				$scope.customer = businessServices.getUsername();
			} else {
				$scope.loggedIn = false;
				$location.path('/');
			}

			for (var i = 2015; i < 2045; i++) {
				$scope.years.push({ "val": i });
			}

			if ($routeParams.id && $routeParams.sortby && $routeParams.desc) {
				$scope.dataLoading = true;
				$scope.id = $routeParams.id;
				$scope.title = 'Rate #' + $routeParams.id;
				$scope.data = { 'id': $routeParams.id, 'eventType': $scope.eventType, 'sortby': $routeParams.sortby, 'desc': $routeParams.desc };

				CsrrateService.getData($scope.data, function (response) {
					
					if (response.Status) {
						$scope.csrate = response.CSRRates;
						$scope.NextRecord = response.NextRecord;
						$scope.PreviousRecord = response.PreviousRecord;
						$scope.PlanAttributeId = response.PlanAttributeId;
						
						if (!$scope.PreviousRecord) {
							$scope.prevButton = true;
						}
						if (!$scope.NextRecord) {
							$scope.nextButton = true;
						}
						$scope.csrate.RatingAreaId = $scope.csrate.RatingAreaId ? $scope.csrate.RatingAreaId.toString() : "";

						if($scope.csrate.RateEffectiveDate){
							$scope.csrate.RateEffectiveDate = $filter('date')(new Date($scope.csrate.RateEffectiveDate),'MM/dd/yyyy');	
						}
						if($scope.csrate.CreatedDateTime){
							$scope.csrate.CreatedDateTime = $filter('date')(new Date($scope.csrate.CreatedDateTime),'MM/dd/yyyy HH:mm:ss');	
						}
						if($scope.csrate.ModifiedDateTime){
							$scope.csrate.ModifiedDateTime = $filter('date')(new Date($scope.csrate.ModifiedDateTime),'MM/dd/yyyy HH:mm:ss');	
						}

						$scope.CreatedBy = response.CreatedBy;
						$scope.ModifiedBy = response.ModifiedBy;

						$scope.dataLoading = false;
					} else {
						$location.path('insuranceplantype/csrrate');
					}
				});
			} else {
				$location.path('insuranceplantype/csrrate');
			}

			$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li><a href="insuranceplantype/csrrate">Rates</a></li><li class="active">' + $scope.title + '</li>';

			$scope.$on('$includeContentLoaded', function (event) {
				var idleState = false;
				var idleTimer = null;
				$('*').bind('mousemove click mouseup mousedown keydown keypress keyup submit change mouseenter scroll resize dblclick', function () {
					clearTimeout(idleTimer);
					idleState = false;
					idleTimer = setTimeout(function () {
						$location.path('/logout');
						$scope.$apply();
						idleState = true;
					}, messages.sessionTimeout);
				});
				$("body").trigger("mousemove");
				$('#wizard').smartWizard({ enableAllSteps: true });
			});

			/*CsrrateService.waitForLayoutView(function(response){	
				$rootScope.pageLoading=false;
				$scope.states=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.States:[];
				$scope.ratingareas=((typeof response[1].data !='undefined') && (response[1].data.Status))?response[1].data.List:[];
				$scope.dataLoading = false;	
			});*/

		}])

	/****
		Created By : Aastha Jain
		Created Date : 23-06-2016
		Start : Edit CSR Rate Controller.
	****/

	.controller('EditCsrrateController',
	['$scope', '$rootScope', '$location', 'CsrrateService', 'checkCreds', 'businessServices', '$routeParams', '$filter', 'messages',
		function ($scope, $rootScope, $location, CsrrateService, checkCreds, businessServices, $routeParams, $filter, messages) {

			// reset login status $routeParams.Blogid
			$scope.closeFlash = function (e) {
				$scope.flash.status = false;
			}
			$rootScope.pageLoading = false;
			$scope.dataLoading = true;
			$scope.id = '';
			$scope.csrate = {};
			$scope.years = [];
			$scope.metalLevels = [];
			$scope.marketCoverages = [];
			$scope.states = [];
			$scope.ratingareas = [];
			$scope.sortby = $routeParams.sortby;
			$scope.desc = $routeParams.desc;
			$scope.eventType = "";
			$scope.CreatedBy = '';
			$scope.formInvalid = false;
			$scope.ModifiedBy = '';
			$scope.title = 'Rate';
			$scope.PlanIds = [];
			$scope.BusinessYears = [];
			$scope.ratingAreaId = [];

			$scope.breadcrumb = true;
			if (checkCreds() === true) {
				$scope.loggedIn = true;
				$scope.customer = businessServices.getUsername();
			} else {
				$scope.loggedIn = false;
				$location.path('/');
			}

			if ($routeParams.action == 'copy') {
				$scope.readOnly = false;
				$scope.actionBtnText = "Create";
				$scope.showBtn = true;
				$scope.hideFields = true;
			} else {
				$scope.readOnly = true;
				$scope.actionBtnText = "Update";
				$scope.showBtn = false;
				$scope.title = 'Rate #' + $routeParams.id;
			}

			
			if ($routeParams.id) {
				$scope.dataLoading = true;
				$scope.id = $routeParams.id;
				$scope.sortby = $routeParams.sortby ? $routeParams.sortby : null;
				$scope.desc = $routeParams.desc ? $routeParams.desc : null;
				$scope.eventType = $scope.eventType ? $scope.eventType : null;

				$scope.data = { 'id': $routeParams.id, 'eventType': $scope.eventType, 'sortby': $scope.sortby, 'desc': $scope.desc };

				CsrrateService.getData($scope.data, function (response) {
					if(response.Message=='CSR Rate does not exist or Issue is not active.'){
						bootbox.alert(response.Message, function () {
							$location.path('insuranceplantype/csrrate');
							$scope.$apply();
						});
					}
					if (response.Status) {
						$scope.csrate = response.CSRRates;
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
						$scope.csrate.RatingAreaId = $scope.csrate.RatingAreaId ? $scope.csrate.RatingAreaId.toString() : "";
						
						if($scope.csrate.RateEffectiveDate){
							$scope.csrate.RateEffectiveDate = $filter('date')(new Date($scope.csrate.RateEffectiveDate),'MM/dd/yyyy');	
						}
						if($scope.csrate.CreatedDateTime){
							$scope.csrate.CreatedDateTime = $filter('date')(new Date($scope.csrate.CreatedDateTime),'MM/dd/yyyy HH:mm:ss');	
						}
						if($scope.csrate.ModifiedDateTime){
							$scope.csrate.ModifiedDateTime = $filter('date')(new Date($scope.csrate.ModifiedDateTime),'MM/dd/yyyy HH:mm:ss');	
						}
						
						$scope.CreatedBy = response.CreatedBy;
						$scope.ModifiedBy = response.ModifiedBy;

						if ($routeParams.action == 'copy') {
							$scope.csrate.CreatedDateTime = new Date();
						}

						$scope.BusinessYears.push($scope.csrate.BusinessYear);

						$scope.csrate.PlanIDNew = { PlanId: $scope.csrate.PlanID, PlanMarketingName: response.PlanMarketingName };

						$scope.getSearchPlanID();

						$scope.dataLoading = false;
					} else {
						$location.path('insuranceplantype/csrrate');
					}
				});
			} else {
				$location.path('insuranceplantype/csrrate');
			}

			$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li><a href="insuranceplantype/csrrate">Rates</a></li><li class="active">' + $scope.title + '</li>';

			$scope.$on('$includeContentLoaded', function (event) {
				var idleState = false;
				var idleTimer = null;
				$('*').bind('mousemove click mouseup mousedown keydown keypress keyup submit change mouseenter scroll resize dblclick', function () {
					clearTimeout(idleTimer);
					idleState = false;
					idleTimer = setTimeout(function () {
						$location.path('/logout');
						$scope.$apply();
						idleState = true;
					}, messages.sessionTimeout);
				});
				
				$('#wizard').smartWizard({ enableAllSteps: true });
				$.validator.setDefaults({ ignore: '' });
				$("#createCSRRateForm").validate({
					errorElement: 'span',
					errorClass: 'error',
					onkeyup: false,
					rules: {
						planId: {
							required: true,
						},
						businessYear: {
							required: true,
						},
						ratingAreaId: {
							required: true,
						},
						age: {
							required: true,
						},
						GrpCostAmt: {
							maxlength: 16,
						},
						GrpHSAAmt: {
							maxlength: 16,
						},
						IndividualRate: {
							maxlength: 16,
						},
						IndividualTobaccoRate: {
							maxlength: 16,
						},
						GrpEmplrPremAmt: {
							maxlength: 16,
						},
						GrpCashAmt: {
							maxlength: 16,
						},
					}
				});
				$("body").trigger("mousemove");
			});

			$scope.rateEffDate = {};
			$scope.rateEffDate.isOpned = false;

			$scope.open_rate_date = function () {
				$scope.rateEffDate.isOpned = true;
			};

			for (var i = 2015; i < 2045; i++) {
				$scope.years.push({ "val": i });
			}

			
		var IsChangesOccur = false;
		
		$scope.ChangesOccur = function(){
			IsChangesOccur=true;
			console.log('changes occur',IsChangesOccur);
		}

		$scope.navigate = function (type) {
			console.log('test',JSON.parse(JSON.stringify($scope.csrate)));
			if(type === 'close'){
				if (IsChangesOccur!=false) {
					bootbox.dialog({
						message: 'Do you want to save csr rate?',
						title: "",
						buttons: {
							success: {
								label: "Yes",
								className: "btn-success",
								callback: function () {
									$scope.formLoading = true;
									$scope.editcsrrateredirect(type, function () {
										// $location.path('insuranceplantype/csrrate');
										// $scope.formLoading = false;
										// $scope.$apply();
									});
								}
							},
							danger: {
								label: "No",
								className: "btn-danger",
								callback: function () {
									$location.path('insuranceplantype/csrrate');
									$scope.formLoading = false;
									$scope.$apply();
								}
							}
						}
					});

				} else {
					$scope.formLoading = false;
					$location.path('insuranceplantype/csrrate');
				}
			}
			else if(type === 'planattribute'){
				if (IsChangesOccur!=false) {
					bootbox.dialog({
						message: 'Do you want to save csr rate?',
						title: "",
						buttons: {
							success: {
								label: "Yes",
								className: "btn-success",
								callback: function () {
									$scope.formLoading = true;
									$scope.editcsrrateredirect(type, function () {
										// $location.path('insuranceplantype/planattribute/'+JSON.parse(JSON.stringify($scope.csrate)).PlanID+'/'+JSON.parse(JSON.stringify($scope.csrate)).BusinessYear);
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
				Palne Attribute Update api.
			*/
			$scope.editcsrrate = function () {
				if ($("#createCSRRateForm").valid()) {
					if ($scope.csrate.PlanIDNew == '' || $scope.csrate.PlanIDNew == null || $scope.csrate.PlanIDNew == undefined) {
						$scope.formInvalid = true;
						return;
					} else {
						$scope.formInvalid = false;
					}
					$scope.formLoading = true;
					$scope.csrate.PlanID = $scope.csrate.PlanIDNew.PlanId;
					var data = {};
					var action = 0;
					
					data = $scope.csrate;
					
					if ($routeParams.action == 'copy') {
						data.Createdby = $scope.customer.id;
						data.ModifiedBy = '';
						CsrrateService.AddCSRRate(data, function (response) {
							if (response.Status) {
								if (response.Status == 'false') {
									bootbox.alert(response.Message, function () {
										$scope.$apply();
									});
								} else {
									bootbox.alert(messages.saved, function () {
										$location.path('insuranceplantype/csrrate');
										$scope.$apply();
									});
								}
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
					} else {
						
						// if ($scope.ApprovalStatus === 5) {
						// 	bootbox.alert('This csrrate plan status is in production and cannot be edited', function() {
						// 		$scope.$apply();
						// 	});
						// 	return;
						// }
					
						data.ModifiedBy = $scope.customer.id;
						CsrrateService.UpdateCSRRate(data, function (response) {
							if (response.Status) {
								if (response.Status == 'false') {
									bootbox.alert(response.Message, function () {
										$scope.$apply();
									});
								} else {
									bootbox.alert(messages.saved, function () {
										$location.path('insuranceplantype/csrrate');
										$scope.$apply();
									});
								}
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
				}
				else{
					if ($scope.csrate.PlanIDNew == '' || $scope.csrate.PlanIDNew == null || $scope.csrate.PlanIDNew == undefined) {
						$scope.formInvalid = true;
						return;
					}
				}
			};
			
			/**
				Palne Attribute Update api redirect.
			*/
			$scope.editcsrrateredirect = function (type) {
				if ($("#createCSRRateForm").valid()) {
					if ($scope.csrate.PlanIDNew == '' || $scope.csrate.PlanIDNew == null || $scope.csrate.PlanIDNew == undefined) {
						$scope.formInvalid = true;
						return;
					} else {
						$scope.formInvalid = false;
					}
					$scope.formLoading = true;
					$scope.csrate.PlanID = $scope.csrate.PlanIDNew.PlanId;
					var data = {};
					var action = 0;
					
					data = $scope.csrate;
					
					if ($routeParams.action == 'copy') {
						data.Createdby = $scope.customer.id;
						data.ModifiedBy = '';
						CsrrateService.AddCSRRate(data, function (response) {
							if (response.Status) {
								if (response.Status == 'false') {
									bootbox.alert(response.Message, function () {
										$scope.$apply();
									});
								} else {
									bootbox.alert(messages.saved, function () {
										if(type === 'close'){
											$location.path('insuranceplantype/csrrate');	
										}
										else if(type === 'planattribute'){
											$location.path('insuranceplantype/editplanattribute/'+$scope.PlanAttributeId+'/CreatedDateTime/true');
										}
										// $location.path('insuranceplantype/csrrate');
										$scope.$apply();
									});
								}
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
					} else {
						
						// if ($scope.ApprovalStatus === 5) {
						// 	bootbox.alert('This csrrate plan status is in production and cannot be edited', function() {
						// 		$scope.$apply();
						// 	});
						// 	return;
						// }
						
						data.ModifiedBy = $scope.customer.id;
						CsrrateService.UpdateCSRRate(data, function (response) {
							if (response.Status) {
								if (response.Status == 'false') {
									bootbox.alert(response.Message, function () {
										$scope.$apply();
									});
								} else {
									bootbox.alert(messages.saved, function () {
										if(type === 'close'){
											$location.path('insuranceplantype/csrrate');	
										}
										else if(type === 'planattribute'){
											$location.path('insuranceplantype/editplanattribute/'+$scope.PlanAttributeId+'/CreatedDateTime/true');
										}
										$scope.$apply();
									});
								}
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
				}
				else{
					if ($scope.csrate.PlanIDNew == '' || $scope.csrate.PlanIDNew == null || $scope.csrate.PlanIDNew == undefined) {
						$scope.formInvalid = true;
						return;
					}
				}
			};

			$scope.getMoreResultPlanId = function (value) {
				var searchedID = { searchByPlanId: value };
				CsrrateService.GetSearchPlanID(searchedID, function (response) {
					if (response.Status) {
						$scope.PlanIds = response.PlanIds;
					}
				});
			};

			$scope.getSearchPlanID = function () {
				var searchedID = { searchByPlanId: "" };
				CsrrateService.GetSearchPlanID(searchedID, function (response) {
					if (response.Status) {
						$scope.PlanIds = response.PlanIds;
					}
				});
			};

			$scope.setValue = function() {
				if ($scope.csrate.PlanIDNew != null) {
					$scope.BusinessYears=[];
					$scope.BusinessYears.push($scope.csrate.PlanIDNew.BusinessYear);
					//if ($scope.BusinessYears.length == 1) {
						$scope.csrate.BusinessYear = $scope.csrate.PlanIDNew.BusinessYear.toString();
						$scope.PlanAttributeId = $scope.csrate.PlanIdNew.Id;
					//} else {
					//	$scope.csrate.BusinessYear = "";
					//}
				}
			};
			
		CsrrateService.waitForLayoutEdit(function(response){	
			$rootScope.pageLoading=false;
			$scope.dataLoading = true;
			$scope.ratingAreaId=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.List:[];
			if(typeof $scope.csrate.Id != 'undefined'){$scope.dataLoading = false;}
		});

	}])

	/****
		Created By : Rahul Singh
		Created Date : 21-04-2016;
		Start : Add New CSR Rate Controller.
	****/

	.controller('NewCsrrateController', ['$scope', '$rootScope', '$location', 'CsrrateService', 'checkCreds', 'businessServices', '$routeParams', '$filter', 'messages', function ($scope, $rootScope, $location, CsrrateService, checkCreds, businessServices, $routeParams, $filter, messages) {

		// reset login status $routeParams.Blogid
		$scope.closeFlash = function (e) {
			$scope.flash.status = false;
		}
		$rootScope.pageLoading = false;
		$scope.dataLoading = false;
		$scope.id = '';
		$scope.csrate = {};
		$scope.years = [];
		$scope.metalLevels = [];
		$scope.marketCoverages = [];
		$scope.states = [];
		$scope.ratingareas = [];
		$scope.sortby = $routeParams.sortby;
		$scope.desc = $routeParams.desc;
		$scope.eventType = "";
		$scope.CreatedBy = '';
		$scope.ModifiedBy = '';
		$scope.PlanIds = [];
		$scope.formInvalid = false;
		$scope.BusinessYears = [];
		$scope.ratingAreaId=[];
		$scope.getSearchPlanID = function () {
			var searchedID = { searchByPlanId: "" };
			CsrrateService.GetSearchPlanID(searchedID, function (response) {
				if (response.Status) {
					$scope.PlanIds = response.PlanIds;
				}
			});
		};

		$scope.breadcrumb = true;
		if (checkCreds() === true) {
			$scope.loggedIn = true;
			$scope.customer = businessServices.getUsername();

			$scope.getSearchPlanID();
		} else {
			$scope.loggedIn = false;
			$location.path('/');
		}

		CsrrateService.waitForLayoutEdit(function(response){	
			$rootScope.pageLoading=false;
			$scope.dataLoading = true;
			$scope.ratingAreaId=((typeof response[0].data !='undefined') && (response[0].data.Status))?response[0].data.List:[];
			$scope.dataLoading = false;
		});

		$scope.readOnly = false;
		$scope.actionBtnText = "Create";
		$scope.showBtn = true;
		$scope.title = 'New Rate';
		$scope.hideFields = true;

		$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li><a href="insuranceplantype/csrrate">Rates</a></li><li class="active">' + $scope.title + '</li>';

		$scope.$on('$includeContentLoaded', function (event) {
			var idleState = false;
			var idleTimer = null;
			$('*').bind('mousemove click mouseup mousedown keydown keypress keyup submit change mouseenter scroll resize dblclick', function () {
				clearTimeout(idleTimer);
				idleState = false;
				idleTimer = setTimeout(function () {
					$location.path('/logout');
					$scope.$apply();
					idleState = true;
				}, messages.sessionTimeout);
			});
			$('#wizard').smartWizard({ enableAllSteps: true });
			$.validator.setDefaults({ ignore: '' });
			$("#createCSRRateForm").validate({
				errorElement: 'span',
				errorClass: 'error',
				onkeyup: false,
				rules: {
					planId: {
						required: true,
					},
					businessYear: {
						required: true,
					},
					ratingAreaId: {
						required: true,
					},
					age: {
						required: true,
					},
					GrpCostAmt: {
						maxlength: 16,
					},
					GrpHSAAmt: {
						maxlength: 16,
					},
					IndividualRate: {
						maxlength: 16,
					},
					IndividualTobaccoRate: {
						maxlength: 16,
					},
					GrpEmplrPremAmt: {
						maxlength: 16,
					},
					GrpCashAmt: {
						maxlength: 16,
					},
					
				}
			});
			$("body").trigger("mousemove");
		});

		$scope.rateEffDate = {};
		$scope.rateEffDate.isOpned = false;

		$scope.open_rate_date = function () {
			$scope.rateEffDate.isOpned = true;
		};

		for (var i = 2015; i < 2045; i++) {
			$scope.years.push({ "val": i });
		}

		var IsChangesOccur = false;
		
		$scope.ChangesOccur = function(){
			IsChangesOccur=true;
			console.log('changes occur',IsChangesOccur);
		}

		$scope.navigate = function (type) {
			console.log('test',JSON.parse(JSON.stringify($scope.csrate)));
			if(type === 'close'){
				if (IsChangesOccur!=false) {
					bootbox.dialog({
						message: 'Do you want to save csrrate?',
						title: "",
						buttons: {
							success: {
								label: "Yes",
								className: "btn-success",
								callback: function () {
									$scope.formLoading = true;
									$scope.editcsrrate(function () {
										$location.path('insuranceplantype/csrrate');
										$scope.formLoading = false;
										$scope.$apply();
									});
								}
							},
							danger: {
								label: "No",
								className: "btn-danger",
								callback: function () {
									$location.path('insuranceplantype/csrrate');
									$scope.formLoading = false;
									$scope.$apply();
								}
							}
						}
					});

				} else {
					$scope.formLoading = false;
					$location.path('insuranceplantype/csrrate');
					$timeout(function () {
					$scope.$apply();},300)
				}
			}
			else if(type === 'planattribute'){
				if (IsChangesOccur!=false) {
					bootbox.dialog({
						message: 'Do you want to save plan?',
						title: "",
						buttons: {
							success: {
								label: "Yes",
								className: "btn-success",
								callback: function () {
									$scope.formLoading = true;
									$scope.editcsrrate(function () {
										$location.path('insuranceplantype/editplanattribute/'+$scope.PlanAttributeId+'/CreatedDateTime/true');
										$scope.formLoading = false;
										$scope.$apply();
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
			Palne Attribute Update api.
		*/
		$scope.editcsrrate = function () {
			if ($("#createCSRRateForm").valid()) {
						if ($scope.csrate.PlanIDNew == '' || $scope.csrate.PlanIDNew == null || $scope.csrate.PlanIDNew == undefined) {
						$scope.formInvalid = true;
						return;
					} else {
						$scope.formInvalid = false;
					}
				$scope.formLoading = true;
				$scope.csrate.PlanID = $scope.csrate.PlanIDNew ? $scope.csrate.PlanIDNew.PlanId : '';
				var data = {};
				var action = 0;

				// if ($scope.csrate.RateEffectiveDate) {
				// 	$scope.csrate.RateEffectiveDate .setHours(0);
				// 	$scope.csrate.RateEffectiveDate .setMinutes(0)
				// 	$scope.csrate.RateEffectiveDate .setSeconds(0)
				// 	//= $filter('date')(new Date($scope.csrate.RateEffectiveDate), 'yyyy-MM-dd') + ' 00:00:00';
				// }

				data = $scope.csrate;
				data.Createdby = $scope.customer.id;
				//data.ModifiedBy = $scope.customer.id;
				
				CsrrateService.AddCSRRate(data, function (response) {
					if (response.Status) {
						if (response.Status == 'false') {
									bootbox.alert(response.Message, function () {
										$scope.$apply();
									});
								} else {
									bootbox.alert(messages.saved, function () {
										$location.path('insuranceplantype/csrrate');
										$scope.$apply();
									});
								}
					} else {
						if (response.redirect) {
							bootbox.alert(messages.TryLater, function () {
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
						$("html").animate({ scrollTop: 0 }, 500);
					}
				});
			}
			else
			{
				if ($scope.csrate.PlanIDNew == '' || $scope.csrate.PlanIDNew == null || typeof($scope.csrate.PlanIDNew) == undefined) {
						$scope.formInvalid = true;
						return;
					} 
			}
		};

		$scope.getMoreResultPlanId = function (value) {
			var searchedID = { searchByPlanId: value };
			CsrrateService.GetSearchPlanID(searchedID, function (response) {
				if (response.Status) {
					$scope.PlanIds = response.PlanIds;
				}
			});
		};

		$scope.setValue = function() {
			if ($scope.csrate.PlanIDNew != null) {
				$scope.BusinessYears=[];
				$scope.BusinessYears.push($scope.csrate.PlanIDNew.BusinessYear);
				//if ($scope.BusinessYears.length == 1) {
					$scope.csrate.BusinessYear = $scope.csrate.PlanIDNew.BusinessYear.toString();
					$scope.PlanAttributeId = $scope.csrate.PlanIdNew.Id;
				//} else {
				//	$scope.csrate.BusinessYear = "";
				//}
			}
		};

	}]);