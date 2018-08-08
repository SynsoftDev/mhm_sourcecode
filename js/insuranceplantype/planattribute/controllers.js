		'use strict';
		/**
			Plan attribute Controller List all Plan attribute.
		*/
		angular.module('mhmApp.planattribute', ['ui.select'])
			.controller('PlanattributeController',
				['$scope', '$rootScope', '$location', 'PlanattributeService', 'checkCreds', 'businessServices', 'messages', '$timeout', '$window', '$routeParams',
					function ($scope, $rootScope, $location, PlanattributeService, checkCreds, businessServices, messages, $timeout, $window, $routeParams) {
						// reset login status	
						var d = new Date();
						var curr_year = d.getFullYear();
						$scope.searchby = '';
						$scope.title = 'Plan Attributes';
						$scope.filterByCarrier = "";
						$scope.filterByPlan = "";
						$scope.filterByBenefitName = '';
						$scope.searchByIsActive = '';
						$scope.searchByStateCode = '';
						$scope.filterByMarketCover = '';
						$scope.filterByMetalLevel = '';
						$scope.start_date = '';
						$scope.end_date = '';
						$scope.BusinessYear = '';
						$scope.sortby = 'CreatedDateTime';
						$scope.desc = true;
						$scope.planattributes = [];
						$scope.categoryList = {};
						$scope.TempSearch = {};
						$scope.TempSearch.BusinessYear = '';
						$scope.page = 1;
						$scope.years = [];
						$scope.carriers = [];
						$scope.plans = [];
						$scope.states = [];
						$scope.statesA = [];
						$scope.marketCoverages = [];
						$scope.metalLevels = [];
						$scope.employers = [];
						$scope.approvalStatus = [];
						$scope.issuerIds = [];
						$scope.groupNames = [];
						$scope.TempSearch.IssuerId = "";
						$scope.IssuerId = "";
						$scope.TempSearch.GroupName = "";
						$scope.GroupName = "";

						$scope.TempSearch.ApprovalStatus = "";
						$scope.ApprovalStatus = "";
						$scope.TempSearch.filterByCarrier = "";
						$scope.TempSearch.filterByPlan = "";
						$scope.TempSearch.searchByIsActive = "";
						$scope.TempSearch.searchByStateCode = "";
						$scope.TempSearch.filterByMarketCover = "";
						$scope.TempSearch.filterByMetalLevel = "";
						$scope.TempSearch.searchByEmployer = "";
						$scope.pageSize = messages.pageSize;
						$scope.messages = messages;
						$scope.TotalCount = 0;
						$scope.lastCount = 0;
						$scope.pageSizes = [10, 20, 50, 100];
						$scope.IsDefaultStatus = { 'true': "True", 'false': 'False' };
						$scope.breadcrumb = true;
						$scope.planStatus = { 'true': "Yes", "false": "No" };
						$scope.sessionPage = "PlanattrSession";
						$scope.searchSession = '';
						$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li class="active">Plan Attribute</li>';
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
						$scope.popup1 = { opened: false };

						$scope.onPageSize = function () {
							if ($scope.pageSize != undefined && $scope.pageSize != '') {
								$scope.page = 1;
								$scope.getPlanattributeList();
							}
						}

						PlanattributeService.waitForLayout(function (response) {
							$rootScope.pageLoading = false;
							$scope.states = ((typeof response[0].data != 'undefined') && (response[0].data.Status)) ? response[0].data.States : [];
							$scope.marketCoverages = ((typeof response[1].data != 'undefined') && (response[1].data.Status)) ? response[1].data.lstMarketCoverage : [];

							$scope.approvalStatus = ((typeof response[2].data != 'undefined') && (response[2].data.Status)) ? response[2].data.ApprovalStatus : [];
							$scope.issuerIds = ((typeof response[3].data != 'undefined') && (response[3].data.Status)) ? response[3].data.CarrierIds : [];
							$scope.groupNames = ((typeof response[4].data != 'undefined') && (response[4].data.Status)) ? response[4].data.GroupName : [];

							angular.forEach($scope.states, function (state, key) {
								$scope.statesA[state.StateCode] = state.StateName;
							});
							$scope.dataLoading = false;
							if ($scope.pageSize != undefined && $scope.pageSize != '') {
								$scope.page = 1;

								if ($scope.searchSession != '')
									$scope.setPlanattributeList();

								$scope.getPlanattributeList();
							}

						});

						$scope.changeStatus = function (status, e) {
							console.log('e.target', e.target);
							bootbox.confirm($(e.target).attr('data-alert'), function (response) {
								if (response) {
									var ID = e.target.id;
									if (e.target.value === 'In Production') {
										PlanattributeService.UpdatePlanAttributes(status, ID, function (response) {
											$scope.flash = {};
											if (response.Status) {
												$scope.flash.message = 'Successfully Updated.';
												$scope.flash.status = true;
												$scope.flash.type = 'alert-success ';
												$scope.getPlanattributeList();
												//$scope.$apply();
											} else {
												if (response.redirect) {
													bootbox.alert(messages.TryLater, function () {
														$location.path('/');
														$scope.$apply();
													})
												}
												$scope.flash.message = response.Message;
												$scope.flash.status = true;
												$scope.flash.type = 'alert-danger';
												$scope.dataLoading = false;
												$("html").animate({ scrollTop: 0 }, 500);
												$scope.$apply();
											}
										});
									}
									else {
										bootbox.alert('This Plan is in production and cannot be edited', function () {
											$scope.$apply();
										});
										return;
									}
								}
							});
						}




						$scope.maxDate = new Date();

						$scope.setPlanattributeList = function () {
							$scope.searchby = $scope.TempSearch.searchby;
							$scope.filterByCarrier = $scope.TempSearch.filterByCarrier;
							$scope.filterByPlan = $scope.TempSearch.filterByPlan;
							$scope.filterByMetalLevel = $scope.TempSearch.filterByMetalLevel;
							$scope.EmployerID = $scope.TempSearch.searchByEmployer;
							$scope.page = 1;

							$scope.ApprovalStatus = $scope.TempSearch.ApprovalStatus;
							$scope.IssuerId = $scope.TempSearch.IssuerId;
							$scope.BusinessYear = $scope.TempSearch.BusinessYear;
							$scope.filterByMarketCover = $scope.TempSearch.filterByMarketCover;
							$scope.GroupName = $scope.TempSearch.GroupName;
							$scope.searchByIsActive = $scope.TempSearch.searchByIsActive;
							$scope.searchByStateCode = $scope.TempSearch.searchByStateCode;

							$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
							$scope.getPlanattributeList();
						}

						$scope.resetPlanattributeList = function () {
							$scope.TempSearch.searchby = $scope.searchby = '';
							$scope.TempSearch.filterByCarrier = $scope.filterByCarrier = "";
							$scope.TempSearch.filterByPlan = $scope.filterByPlan = "";
							$scope.TempSearch.filterByMetalLevel = $scope.filterByMetalLevel = "";
							$scope.TempSearch.searchByEmployer = $scope.EmployerID = "";
							$scope.page = 1;

							$scope.TempSearch.ApprovalStatus = $scope.ApprovalStatus = "";
							$scope.TempSearch.IssuerId = $scope.IssuerId = "";
							$scope.TempSearch.BusinessYear = $scope.BusinessYear = "";
							$scope.TempSearch.filterByMarketCover = $scope.filterByMarketCover = "";
							$scope.TempSearch.GroupName = $scope.GroupName = "";
							$scope.TempSearch.searchByIsActive = $scope.searchByIsActive = "";
							$scope.TempSearch.searchByStateCode = $scope.searchByStateCode = "";

							businessServices.resetSearchSession($scope.sessionPage);
							$scope.getPlanattributeList();
						}


						$scope.getPlanattributeList = function () {

							if ($routeParams.planid)
								$scope.searchby = $routeParams.planid;
							if ($routeParams.businessyear)
								$scope.BusinessYear = $routeParams.businessyear;

							$timeout(function () {
								$rootScope.pageLoading = false;
								$scope.dataLoading = true;
								PlanattributeService.getAll($scope, function (response) {
									if (response.Status) {
										console.log("plan attribute", response.PlanAttributes);
										$scope.planattributes = response.PlanAttributes;
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
										$scope.planattributes = [];
									}
								});
							})

						}

						/**
							Created By : Aastha Jain
							Created Date : 07-06-2016
							Purpose : Function to export plan attribute list.
							Export Plan Attribute List parameter and call API.
							Start
						*/
						$scope.exportReport = function () {
							$scope.dataLoading = true;
							PlanattributeService.getPlanattributeReport($scope, function (responseStatus, response, xhr) {
								if (responseStatus === 200) {
									var filename = 'PlanAttributeReport.xlsx';
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
									console.log('ex', ex)
								}

								reader.readAsBinaryString(file);
							}
						}

						$scope.savePlanAttribute = function (data) {
							$scope.dataLoading = true;
							PlanattributeService.ImportPlanAttribute(data, function (response) {

								// $scope.flash={};
								// if (response.Status) {
								// 	$scope.flash.message = 'Successfully Imported';	
								// 	bootbox.alert('Successfully Imported',function(){
								// 		$scope.flash.status = true;
								// 		$scope.flash.type = 'alert-success ';
								// 		$scope.$apply();
								// 	}) 			
								// 	$scope.dataLoading = false;	
								// } else {
								// 	if(response.redirect){
								// 		bootbox.alert(messages.TryLater,function(){
								// 				$location.path('/');
								// 				$scope.$apply();
								// 			}) 
								// 	}
								// 	$scope.flash.message = response.Message;
								// 	$scope.flash.status = true;
								// 	$scope.flash.type = 'alert-danger';
								// 	$scope.dataLoading = false;
								// 	$("html").animate({scrollTop:0},500);
								// }

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

						//End Import PlanAttributes	


						/**
							End : Function to export plan attribute list.
						**/

						// change sorting order
						$scope.sort_by = function (newSortingOrder) {
							if ($scope.sortby == newSortingOrder)
								$scope.desc = !$scope.desc;
							else
								$scope.desc = $scope.desc;
							$scope.sortby = newSortingOrder;
							$scope.page = 1;
							$scope.getPlanattributeList();
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
								$scope.getPlanattributeList();
							}
						};

						$scope.nextPage = function () {
							if ($scope.page < $scope.TotalCount - 1) {
								$scope.page++;
								$scope.getPlanattributeList();
							}
						};
						$scope.prevPage = function () {
							if ($scope.page > 1) {
								$scope.page--;
								$scope.getPlanattributeList();
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
			.controller('GetPlanattributeController',
				['$scope', '$rootScope', '$location', 'PlanattributeService', 'checkCreds', 'businessServices', '$routeParams', '$filter', 'messages',
					function ($scope, $rootScope, $location, PlanattributeService, checkCreds, businessServices, $routeParams, $filter, messages) {
						// reset login status $routeParams.Blogid
						$scope.closeFlash = function (e) {
							$scope.flash.status = false;
						}
						$rootScope.pageLoading = false;
						$scope.dataLoading = true;
						$scope.id = '';
						$scope.planattribute = {};
						$scope.sortby = $routeParams.sortby;
						$scope.desc = $routeParams.desc;
						$scope.eventType = "";
						$scope.years = [];
						$scope.conditionalExp = [];
						$scope.plans = [];
						$scope.marketCoverages = [];
						$scope.metalLevels = [];
						$scope.carriers = [];
						$scope.employers = [];
						$scope.states = [];
						$scope.insuranceTypes = [];
						$scope.approvalStatus = [];
						$scope.Createdby = '';
						$scope.ModifiedBy = '';

						$scope.breadcrumb = true;
						if (checkCreds() === true) {
							$scope.loggedIn = true;
							$scope.customer = businessServices.getUsername();
						} else {
							$scope.loggedIn = false;
							$location.path('/');
						}

						if ($routeParams.id && $routeParams.sortby && $routeParams.desc) {
							// $rootScope.pageLoading=true;	
							$scope.dataLoading = true;

							$scope.id = $routeParams.id;
							$scope.title = 'Plan Attribute #' + $routeParams.id;
							$scope.data = { 'id': $routeParams.id, 'eventType': $scope.eventType, 'sortby': $routeParams.sortby, 'desc': $routeParams.desc };

							PlanattributeService.getData($scope.data, function (response) {
								if (response.Status) {
									$scope.planattribute = response.PlanAttributes;
									console.log("plan attribute", response);
									$scope.NextRecord = response.NextRecord;
									$scope.PreviousRecord = response.PreviousRecord;
									if (!$scope.PreviousRecord) {
										$scope.prevButton = true;
									}
									if (!$scope.NextRecord) {
										$scope.nextButton = true;
									}
									$scope.planattribute.DentalOnlyPlan = $scope.planattribute.DentalOnlyPlan != null ? $scope.planattribute.DentalOnlyPlan.toString() : "";
									$scope.planattribute.EmployerId = $scope.planattribute.EmployerId != null ? $scope.planattribute.EmployerId.toString() : "";
									$scope.planattribute.IsHSAEligible = $scope.planattribute.IsHSAEligible != null ? $scope.planattribute.IsHSAEligible.toString() : "";
									$scope.planattribute.NonEmbeddedOOPLimits = $scope.planattribute.NonEmbeddedOOPLimits != null ? $scope.planattribute.NonEmbeddedOOPLimits.toString() : "";
									$scope.planattribute.MedicalDrugDeductiblesIntegrated = $scope.planattribute.MedicalDrugDeductiblesIntegrated != null ? $scope.planattribute.MedicalDrugDeductiblesIntegrated.toString() : "";
									$scope.planattribute.MedicalDrugMaximumOutofPocketIntegrated = $scope.planattribute.MedicalDrugMaximumOutofPocketIntegrated != null ? $scope.planattribute.MedicalDrugMaximumOutofPocketIntegrated.toString() : "";
									$scope.planattribute.ReferralForSpecialist = $scope.planattribute.ReferralForSpecialist != null ? $scope.planattribute.ReferralForSpecialist.toString() : "";
									$scope.planattribute.CarrierId = $scope.planattribute.CarrierId.toString();
									$scope.planattribute.OpenForEnrollment = $scope.planattribute.OpenForEnrollment != null ? $scope.planattribute.OpenForEnrollment.toString() : "";

									if ($scope.planattribute.CreatedDateTime) {
										$scope.planattribute.CreatedDateTime = $filter('date')(new Date($scope.planattribute.CreatedDateTime), 'MM/dd/yyyy HH:mm:ss');
									}
									if ($scope.planattribute.ModifiedDateTime) {
										$scope.planattribute.ModifiedDateTime = $filter('date')(new Date($scope.planattribute.ModifiedDateTime), 'MM/dd/yyyy HH:mm:ss');
									}

									$scope.Createdby = response.CreatedBy;
									$scope.ModifiedBy = response.ModifiedBy;

									if ($scope.planattribute.PlanType != null)
										$scope.planattribute.PlanType = $scope.planattribute.PlanType.toString();

									if ($scope.planattribute.InsuranceType != null)
										$scope.planattribute.InsuranceType = $scope.planattribute.InsuranceType.toString();

									if ($scope.planattribute.ApprovalStatus != null)
										$scope.planattribute.ApprovalStatus = $scope.planattribute.ApprovalStatus.toString();

									if ($scope.planattribute.IsHRAeligible != null)
										$scope.planattribute.IsHRAeligible = $scope.planattribute.IsHRAeligible.toString();

									$scope.dataLoading = false;

								} else {
									$location.path('insuranceplantype/planattribute');
								}
							});
						} else {
							$location.path('insuranceplantype/planattribute');
						}

						$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li><a href="insuranceplantype/planattribute">Plan Attribute</a></li><li class="active">' + $scope.title + '</li>';

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

						PlanattributeService.waitForLayoutView(function (response) {
							$rootScope.pageLoading = false;
							$scope.dataLoading = true;
							$scope.plans = ((typeof response[0].data != 'undefined') && (response[0].data.Status)) ? response[0].data.Plans : [];
							// $scope.carriers=((typeof response[1].data !='undefined') && (response[1].data.Status))?response[1].data.Carriers:[];	
							$scope.employers = ((typeof response[1].data != 'undefined') && (response[1].data.Status)) ? response[1].data.lstEmployerMaster : [];
							$scope.states = ((typeof response[2].data != 'undefined') && (response[2].data.Status)) ? response[2].data.States : [];

							$scope.marketCoverages = ((typeof response[3].data != 'undefined') && (response[3].data.Status)) ? response[3].data.lstMarketCoverage : [];
							$scope.metalLevels = ((typeof response[4].data != 'undefined') && (response[4].data.Status)) ? response[4].data.lstMarketCoverage : [];
							$scope.insuranceTypes = ((typeof response[5].data != 'undefined') && (response[5].data.Status)) ? response[5].data.lstInsuranceTypesMaster : [];
							$scope.approvalStatus = ((typeof response[6].data != 'undefined') && (response[6].data.Status)) ? response[6].data.ApprovalStatus : [];

							$scope.dataLoading = false;
						});

						for (var i = 2015; i < 2045; i++) {
							$scope.years.push({ "val": i });
						}

						$scope.conditionalExp.push({ "key": true, "val": "True" });
						$scope.conditionalExp.push({ "key": false, "val": "False" });

					}])

			/****
					Created By : Aastha Jain
					Created Date : 18-06-2016
					Start : Edit Plan Attribute Controller.
			****/

			.controller('EditPlanattributeController',
				['$scope', '$rootScope', '$location', 'PlanattributeService', 'checkCreds', 'businessServices', '$routeParams',
					'$filter', 'messages', '$timeout',
					function ($scope, $rootScope, $location, PlanattributeService, checkCreds, businessServices, $routeParams,
						$filter, messages, $timeout) {

						$scope.closeFlash = function (e) {
							$scope.flash.status = false;
						}
						$rootScope.pageLoading = false;
						$scope.dataLoading = true;
						$scope.id = '';
						$scope.planattribute = {};
						$scope.planattribute.PlanNumber = '000';
						$scope.EmbeddedDeductible = false;
						$scope.EmbeddedOOPL = false;
						$scope.years = [];
						$scope.conditionalExp = [];
						$scope.plans = [];
						$scope.marketCoverages = [];
						$scope.metalLevels = [];
						$scope.carriers = [];
						$scope.employers = [];
						$scope.states = [];
						$scope.insuranceTypes = [];
						$scope.approvalStatus = [];
						$scope.sortby = $routeParams.sortby;
						$scope.desc = $routeParams.desc;
						$scope.eventType = "";
						$scope.issuerIds = [];
						$scope.Createdby = '';
						$scope.ModifiedBy = '';
						$scope.groupNames = [];
						$scope.title = 'Plan Attribute';
						// $scope.PlanIds = [];

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
							console.log('$routeParams.id', $routeParams.id)
						} else {
							$scope.readOnly = true;
							$scope.actionBtnText = "Update";
							$scope.showBtn = false;
							$scope.title = 'Plan Attribute #' + $routeParams.id;
						}

						if ($routeParams.id) {
							// $rootScope.pageLoading=true;
							$scope.dataLoading = true;
							$scope.id = $routeParams.id;
							$scope.sortby = $routeParams.sortby ? $routeParams.sortby : null;
							$scope.desc = $routeParams.desc ? $routeParams.desc : null;
							$scope.eventType = $scope.eventType ? $scope.eventType : null;
							$scope.data = { 'id': $routeParams.id, 'eventType': $scope.eventType, 'sortby': $scope.sortby, 'desc': $scope.desc };

							PlanattributeService.getData($scope.data, function (response) {
								if (response.Status) {
									$scope.planattribute = response.PlanAttributes;
									console.log("plan attribute", response);
									$scope.NextRecord = response.NextRecord;
									$scope.PreviousRecord = response.PreviousRecord;
									if (!$scope.PreviousRecord) {
										$scope.prevButton = true;
									}
									if (!$scope.NextRecord) {
										$scope.nextButton = true;
									}
									$scope.planattribute.DentalOnlyPlan = $scope.planattribute.DentalOnlyPlan != null ? $scope.planattribute.DentalOnlyPlan.toString() : "";
									$scope.planattribute.EmployerId = $scope.planattribute.EmployerId != null ? $scope.planattribute.EmployerId.toString() : "";
									$scope.planattribute.IsHSAEligible = $scope.planattribute.IsHSAEligible.toString();
									$scope.planattribute.NonEmbeddedOOPLimits = $scope.planattribute.NonEmbeddedOOPLimits != null ? $scope.planattribute.NonEmbeddedOOPLimits.toString() : "";
									$scope.planattribute.MedicalDrugDeductiblesIntegrated = $scope.planattribute.MedicalDrugDeductiblesIntegrated != null ? $scope.planattribute.MedicalDrugDeductiblesIntegrated.toString() : "";
									$scope.planattribute.MedicalDrugMaximumOutofPocketIntegrated = $scope.planattribute.MedicalDrugMaximumOutofPocketIntegrated != null ? $scope.planattribute.MedicalDrugMaximumOutofPocketIntegrated.toString() : "";
									$scope.planattribute.ReferralForSpecialist = $scope.planattribute.ReferralForSpecialist != null ? $scope.planattribute.ReferralForSpecialist.toString() : "";
									$scope.planattribute.CarrierId = $scope.planattribute.CarrierId.toString();
									if ($routeParams.action == 'copy') {
										$scope.planattribute.OpenForEnrollment = "false";
										$scope.planattribute.OldPlanId = $scope.planattribute.Id;
										$scope.planattribute.Id = '';
									} else {
										$scope.planattribute.OpenForEnrollment = $scope.planattribute.OpenForEnrollment != null ? $scope.planattribute.OpenForEnrollment.toString() : "";
									}
									if ($scope.planattribute.CreatedDateTime) {
										$scope.planattribute.CreatedDateTime = $filter('date')(new Date($scope.planattribute.CreatedDateTime), 'MM/dd/yyyy HH:mm:ss');
									}
									if ($scope.planattribute.ModifiedDateTime) {
										$scope.planattribute.ModifiedDateTime = $filter('date')(new Date($scope.planattribute.ModifiedDateTime), 'MM/dd/yyyy HH:mm:ss');
									}
									// if ($scope.planattribute.ModifiedDateTime) { }
									// if ($scope.planattribute.ModifiedDateTime) { }
									// $scope.planattribute.OpenForEnrollment_ChangedDate = new Date($scope.planattribute.OpenForEnrollment_ChangedDate);
									$scope.planattribute.GroupName = $scope.planattribute.GroupName;

									$scope.planattribute.OldApprovalStatus = $scope.planattribute.ApprovalStatus;

									$scope.Createdby = response.CreatedBy;
									$scope.ModifiedBy = response.ModifiedBy;

									$scope.JobStatus = response.JobStatus;

									if ($scope.planattribute.PlanType != null)
										$scope.planattribute.PlanType = $scope.planattribute.PlanType.toString();

									if ($scope.planattribute.InsuranceType != null)
										$scope.planattribute.InsuranceType = $scope.planattribute.InsuranceType.toString();

									if ($scope.planattribute.IsHRAeligible != null)
										$scope.planattribute.IsHRAeligible = $scope.planattribute.IsHRAeligible.toString();

									if ($scope.planattribute.ApprovalStatus != null) {
										if ($routeParams.action == 'copy') {
											$scope.planattribute.ApprovalStatus = "1";
											$scope.planattribute.CreatedDateTime = new Date();
										} else {
											$scope.planattribute.ApprovalStatus = $scope.planattribute.ApprovalStatus.toString();
										}
									}

									// $scope.planattribute.PlanId = {PlanId: $scope.planattribute.PlanId, PlanMarketingName: $scope.planattribute.PlanMarketingName};

									$scope.dataLoading = false;

								} else {
									$location.path('insuranceplantype/planattribute');
								}
							});
						} else {
							$location.path('insuranceplantype/planattribute');
						}

						$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li><a href="insuranceplantype/planattribute">Plan Attribute</a></li><li class="active">' + $scope.title + '</li>';

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
$.validator.setDefaults({ ignore: '' });
							$("#createPlanAttributeForm").validate({
								errorElement: 'span',
								errorClass: 'error',
								onkeyup: false,
								rules: {
									planId: {
										required: true,
									},
									planName: {
										required: true,
									},
									approvalStatus: {
										required: true,
									},
									carrierId: {
										required: true,
									},
									businessYear: {
										required: true,
									},
									StandardCompId: {
										required: true,
									},
									TEHBDedInnTier1Coinsurance: {
										maxlength: 16,
									},
									TEHBDedInnTier1Individual: {
										maxlength: 16,
									},
									TEHBDedInnTier1FamilyPerPerson: {
										maxlength: 16,
									},
									TEHBDedInnTier1FamilyPerGroup: {
										maxlength: 16,
									},
									TEHBDedInnTier2Coinsurance: {
										maxlength: 16,
									},
									TEHBDedInnTier2Individual: {
										maxlength: 16,
									},
									TEHBDedInnTier2FamilyPerPerson: {
										maxlength: 16,
									},
									TEHBDedInnTier2FamilyPerGroup: {
										maxlength: 16,
									},
									MEHBDedInnTier1Coinsurance: {
										maxlength: 16,
									},
									MEHBDedInnTier1Individual: {
										maxlength: 16,
									},
									MEHBDedInnTier1FamilyPerPerson: {
										maxlength: 16,
									},
									MEHBDedInnTier1FamilyPerGroup: {
										maxlength: 16,
									},
									MEHBDedInnTier2Coinsurance: {
										maxlength: 16,
									},
									MEHBDedInnTier2Individual: {
										maxlength: 16,
									},
									MEHBDedInnTier2FamilyPerPerson: {
										maxlength: 16,
									},
									MEHBDedInnTier2FamilyPerGroup: {
										maxlength: 16,
									},
									DEHBDedInnTier1Coinsurance: {
										maxlength: 16,
									},
									DEHBDedInnTier1Individual: {
										maxlength: 16,
									},
									DEHBDedInnTier1FamilyPerPerson: {
										maxlength: 16,
									},
									DEHBDedInnTier1FamilyPerGroup: {
										maxlength: 16,
									},
									DEHBDedInnTier2Coinsurance: {
										maxlength: 16,
									},
									DEHBDedInnTier2Individual: {
										maxlength: 16,
									},
									DEHBDedInnTier2FamilyPerPerson: {
										maxlength: 16,
									},
									DEHBDedInnTier2FamilyPerGroup: {
										maxlength: 16,
									},
									TEHBInnTier1IndividualMOOP: {
										maxlength: 16,
									},
									TEHBInnTier1FamilyPerPersonMOOP: {
										maxlength: 16,
									},
									TEHBInnTier1FamilyPerGroupMOOP: {
										maxlength: 16,
									},
									TEHBInnTier2IndividualMOOP: {
										maxlength: 16,
									},
									TEHBInnTier2FamilyPerPersonMOOP: {
										maxlength: 16,
									},
									TEHBInnTier2FamilyPerGroupMOOP: {
										maxlength: 16,
									},
									MEHBInnTier1IndividualMOOP: {
										maxlength: 16,
									},
									MEHBInnTier1FamilyPerPersonMOOP: {
										maxlength: 16,
									},
									MEHBInnTier1FamilyPerGroupMOOP: {
										maxlength: 16,
									},
									MEHBInnTier2IndividualMOOP: {
										maxlength: 16,
									},
									MEHBInnTier2FamilyPerPersonMOOP: {
										maxlength: 16,
									},
									MEHBInnTier2FamilyPerGroupMOOP: {
										maxlength: 16,
									},
									DEHBInnTier1IndividualMOOP: {
										maxlength: 16,
									},
									DEHBInnTier1FamilyPerPersonMOOP: {
										maxlength: 16,
									},
									DEHBInnTier1FamilyPerGroupMOOP: {
										maxlength: 16,
									},
									DEHBInnTier2IndividualMOOP: {
										maxlength: 16,
									},
									DEHBInnTier2FamilyPerPersonMOOP: {
										maxlength: 16,
									},
									DEHBInnTier2FamilyPerGroupMOOP: {
										maxlength: 16,
									},
									HIOSProductId: {
										maxlength: 50,
									},
									ServiceAreaId: {
										maxlength: 50,
									},
									PlanFormalName: {
										maxlength: 120,
									},
									PlanNotes: {
										maxlength: 1000,
									},

								}
							});
							
						});

						$scope.enrollDate = {};
						$scope.effectDate = {};
						$scope.expDate = {};
						$scope.enrollDate.isOpned = false;
						$scope.effectDate.isOpned = false;
						$scope.expDate.isOpned = false;

						$scope.open_enroll_date = function () {
							$scope.enrollDate.isOpned = true;
						};
						$scope.open_effect_start = function () {
							$scope.effectDate.isOpned = true;
						};
						$scope.open_exp_start = function () {
							$scope.expDate.isOpned = true;
						};

						/****
							Created By : Vaibhav Chaurasiya
							Created Date : 29-09-2016
							Purpose : Function to get change GroupName based on EmployerID, BusinessYear, InsuranceType.
						****/

						$scope.onFieldsChange = function () {
							if ($scope.planattribute.MrktCover == 'GRP') {
								var Employer = _.find($scope.employers, function (item) {
									return item.EmployerId == $scope.planattribute.EmployerId;
								});

								var Insurance = _.find($scope.insuranceTypes, function (item) {
									return item.InsuranceTypeId == $scope.planattribute.InsuranceType;
								});

								if (!_.isUndefined(Employer) && !_.isUndefined(Insurance)) {
									$scope.planattribute.GroupName = Employer.EmployerName + ' ' + Insurance.InsuranceType1 + ' ' + $scope.planattribute.BusinessYear;
								}
							}
						};

						$scope.carrierIdChange = function () {
							let tempIssuer = $scope.issuerIds.filter(function (item) { return item.Id == $scope.planattribute.CarrierId; })[0];
							$scope.planattribute.IssuerName = tempIssuer.IssuerName;
						}

						var IsChangesOccur = false;
						$scope.ChangesOccur = function () {
							IsChangesOccur = true;
							console.log('changes occur', IsChangesOccur);
						}

						var IsApprovalStatusChange = false;
						$scope.ApprovalStatusChange = function () {
							IsApprovalStatusChange = true;
							console.log('IsApprovalStatusChange', IsApprovalStatusChange);
						}

						var IsOpenForEnrollmentChange = false;
						$scope.IsOpenForEnrollmentChange = function () {
							IsOpenForEnrollmentChange = true;
							$scope.planattribute.OpenForEnrollment_ChangedDate = new Date();
						}

						$scope.navigate = function (type) {
							console.log('JSON.parse(JSON.stringify($scope.planattribute))')
							console.log(JSON.parse(JSON.stringify($scope.planattribute)))
							if (type === 'close') {
								if (IsChangesOccur != false) {
									bootbox.dialog({
										message: 'Do you want to save plan?',
										title: "",
										buttons: {
											success: {
												label: "Yes",
												className: "btn-success",
												callback: function () {
													$scope.formLoading = true;
													$scope.editplanattributeredirect(type, function () {
														// $location.path('insuranceplantype/planattribute');
														// $scope.formLoading = false;
														// $scope.$apply();
													});
												}
											},
											danger: {
												label: "No",
												className: "btn-danger",
												callback: function () {
													$location.path('insuranceplantype/planattribute');
													$scope.formLoading = false;
													$scope.$apply();
												}
											}
										}
									});

								} else {
									$scope.formLoading = false;
									$location.path('insuranceplantype/planattribute');
									$timeout(function () {
										$scope.$apply();
									}, 300)
								}
							}
							else if (type === 'planbenefit') {
								if (IsChangesOccur != false) {
									bootbox.dialog({
										message: 'Do you want to save plan?',
										title: "",
										buttons: {
											success: {
												label: "Yes",
												className: "btn-success",
												callback: function () {
													$scope.formLoading = true;
													$scope.editplanattributeredirect(type, function () {
														// $location.path('insuranceplantype/planbenefit/'+JSON.parse(JSON.stringify($scope.planattribute)).PlanId+'/'+JSON.parse(JSON.stringify($scope.planattribute)).BusinessYear);
														// $scope.formLoading = false;
														// $scope.$apply();
													});
												}
											},
											danger: {
												label: "No",
												className: "btn-danger",
												callback: function () {
													$scope.formLoading = false;
													$location.path('insuranceplantype/planbenefit/' + JSON.parse(JSON.stringify($scope.planattribute)).PlanId + '/' + JSON.parse(JSON.stringify($scope.planattribute)).BusinessYear);
													$scope.$apply();
												}
											}
										}
									});

								} else {
									$scope.formLoading = false;
									$location.path('insuranceplantype/planbenefit/' + JSON.parse(JSON.stringify($scope.planattribute)).PlanId + '/' + JSON.parse(JSON.stringify($scope.planattribute)).BusinessYear);
								}
							}
							else if (type === 'csrrate') {
								console.log('csrrate')
								if (IsChangesOccur != false) {
									bootbox.dialog({
										message: 'Do you want to save plan?',
										title: "",
										buttons: {
											success: {
												label: "Yes",
												className: "btn-success",
												callback: function () {
													$scope.formLoading = true;
													$scope.editplanattributeredirect(type, function () {
														// $location.path('insuranceplantype/csrrate/'+JSON.parse(JSON.stringify($scope.planattribute)).PlanId);
														// $scope.formLoading = false;
														// $scope.$apply();
													});
												}
											},
											danger: {
												label: "No",
												className: "btn-danger",
												callback: function () {
													console.log('no');
													$scope.formLoading = false;
													$location.path('insuranceplantype/csrrate/' + JSON.parse(JSON.stringify($scope.planattribute)).StandardComponentId);
													$scope.$apply();
												}
											}
										}
									});
								} else {
									$scope.formLoading = false;
									$location.path('insuranceplantype/csrrate/' + JSON.parse(JSON.stringify($scope.planattribute)).StandardComponentId);
									//$scope.$apply();
								}
							}
						}

						for (var i = 2015; i < 2045; i++) {
							$scope.years.push({ "val": i });
						}

						$scope.conditionalExp.push({ "key": true, "val": "True" });
						$scope.conditionalExp.push({ "key": false, "val": "False" });

						PlanattributeService.waitForLayoutEdit(function (response) {
							$rootScope.pageLoading = false;
							$scope.dataLoading = true;
							$scope.plans = ((typeof response[0].data != 'undefined') && (response[0].data.Status)) ? response[0].data.Plans : [];
							$scope.marketCoverages = ((typeof response[1].data != 'undefined') && (response[1].data.Status)) ? response[1].data.lstMarketCoverage : [];
							$scope.metalLevels = ((typeof response[2].data != 'undefined') && (response[2].data.Status)) ? response[2].data.lstMarketCoverage : [];
							// $scope.carriers=((typeof response[3].data !='undefined') && (response[3].data.Status))?response[3].data.Carriers:[];	
							$scope.employers = ((typeof response[3].data != 'undefined') && (response[3].data.Status)) ? response[3].data.lstEmployerMaster : [];
							$scope.states = ((typeof response[4].data != 'undefined') && (response[4].data.Status)) ? response[4].data.States : [];
							/****
							   Created By : Vaibhav Chaurasiya
							   Created Date : 29-09-2016
							   Purpose : Function to get list of Insurance types.
						   ****/
							$scope.insuranceTypes = ((typeof response[5].data != 'undefined') && (response[5].data.Status)) ? response[5].data.lstInsuranceTypesMaster : [];
							$scope.approvalStatus = ((typeof response[6].data != 'undefined') && (response[6].data.Status)) ? response[6].data.ApprovalStatus : [];
							$scope.issuerIds = ((typeof response[7].data != 'undefined') && (response[7].data.Status)) ? response[7].data.CarrierIds : [];

							$scope.groupNames = ((typeof response[8].data != 'undefined') && (response[8].data.Status)) ? response[8].data.GroupName : [];

							// $scope.getSearchPlanID();
							$scope.dataLoading = false;
						});

						/**
							Palne Attribute Update api.
						*/
						$scope.editplanattribute = function () {

							if ($scope.planattribute.ApprovalStatus == 5 && $scope.planattribute.UnassignedBen > 0) {
								bootbox.alert('Plan cannot be set to Enrollment enabled or Production Status because there are Benefits that have yet to be assigned to MHM Common Benefits', function () {
									$scope.$apply();
								});
								return;
							}

							// if ($scope.planattribute.OpenForEnrollment == 'true' && $scope.planattribute.ApprovalStatus != 5) {
							// 	bootbox.alert('Plan cannot be set to Enrollment enabled as it is not in Production Status', function () {
							// 		$scope.$apply();
							// 	});
							// 	return;
							// }

							// if ($scope.planattribute.OldApprovalStatus == 5 && IsApprovalStatusChange) {
							// 	bootbox.alert('This Plan is in production and cannot be edited', function () {
							// 		$scope.$apply();
							// 	});
							// 	return;
							// }

							// if ($scope.planattribute.ApprovalStatus != 5 && $scope.JobStatus != '') {
							// 	bootbox.alert('Plan is currently included in Open Job ' + $scope.JobStatus, function () {
							// 		$scope.$apply();
							// 	});
							// 	return;
							// }

							// if (IsOpenForEnrollmentChange && $scope.JobStatus != '') {
							// if ($scope.JobStatus != '') {
							// 	bootbox.dialog({
							// 		message: "Plan is currently included in Open Jobs # " + $scope.JobStatus + " please CONFIRM or CANCEL your change to 'Is Open for Enrollment'",
							// 		title: "",
							// 		buttons: {
							// 			success: {
							// 				label: "Confirm",
							// 				className: "btn-success",
							// 				callback: function () {
							// 					$scope.ProcessSave();
							// 				}
							// 			},
							// 			danger: {
							// 				label: "Cancel",
							// 				className: "btn-danger",
							// 				callback: function () {
							// 					$scope.$apply();
							// 				}
							// 			}
							// 		}
							// 	});

							// 	return;
							// }
							// else {
							// 	$scope.ProcessSave();
							// }
							$scope.ProcessSave();
						};

						$scope.ProcessSave = function () {
							if ($("#createPlanAttributeForm").valid()) {
								$scope.formLoading = true;

								var data = {};
								var action = 0;

								data = $scope.planattribute;

								if ($routeParams.action == 'copy') {
									data.Createdby = $scope.customer.id;
									data.ModifiedBy = '';

									PlanattributeService.AddPlanAttribute(data, function (response) {
										if (response.Status) {
											if (response.Status == 'false') {
												bootbox.alert(response.Message, function () {
													$scope.$apply();
												});
											} else {
												bootbox.alert(messages.saved, function () {
													$location.path('insuranceplantype/planattribute');
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
									data.ModifiedBy = $scope.customer.id;
									PlanattributeService.UpdatePlanAttribute(data, function (response) {
										if (response.Status) {
											if (response.Status == 'false') {
												bootbox.alert(response.Message, function () {
													$scope.$apply();
												});
											} else {
												bootbox.alert(messages.saved, function () {
													// $location.path('insuranceplantype/planattribute');
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
							}
						}

						/**
							Plan Attribute Update api at the time of redirect.
						*/
						$scope.editplanattributeredirect = function (type) {

							if ($scope.planattribute.ApprovalStatus == 5 && $scope.planattribute.UnassignedBen > 0) {
								bootbox.alert('Plan cannot be set to Enrollment enabled or Production Status because there are Benefits that have yet to be assigned to MHM Common Benefits', function () {
									$scope.$apply();
								});
								return;
							}
							// if ($scope.planattribute.OpenForEnrollment == 'true' && $scope.planattribute.UnassignedBen > 0) {
							// 	bootbox.alert('Plan cannot be set to Enrollment enabled or Production Status because there are Benefits that have yet to be assigned to MHM Common Benefits', function () {
							// 		$scope.$apply();
							// 	});
							// 	return;
							// }

							// if ($scope.planattribute.OldApprovalStatus == 5 && IsApprovalStatusChange) {
							// 	bootbox.alert('This Plan is in production and cannot be edited', function () {
							// 		$scope.$apply();
							// 	});
							// 	return;
							// }

							// if ($scope.planattribute.ApprovalStatus != 5 && $scope.JobStatus != '') {
							// 	bootbox.alert('Plan is currently included in Open Job ' + $scope.JobStatus, function () {
							// 		$scope.$apply();
							// 	});
							// 	return;
							// }
							// if (IsOpenForEnrollmentChange && $scope.JobStatus != '') {
							// if ($scope.JobStatus != '') {
							// 	bootbox.dialog({
							// 		message: "Plan is currently included in Open Jobs # " + $scope.JobStatus + " please CONFIRM or CANCEL your change to 'Is Open for Enrollment'",
							// 		title: "",
							// 		buttons: {
							// 			success: {
							// 				label: "Confirm",
							// 				className: "btn-success",
							// 				callback: function () {
							// 					$scope.ProcessRedirectSave(type);
							// 				}
							// 			},
							// 			danger: {
							// 				label: "Cancel",
							// 				className: "btn-danger",
							// 				callback: function () {
							// 					$scope.$apply();
							// 				}
							// 			}
							// 		}
							// 	});

							// 	return;
							// }
							// else {
							// 	$scope.ProcessRedirectSave(type);
							// }
							$scope.ProcessRedirectSave(type);
						};

						$scope.ProcessRedirectSave = function (type) {
							console.log(JSON.parse(JSON.stringify($scope.planattribute)));
							if ($("#createPlanAttributeForm").valid()) {
								$scope.formLoading = true;

								var data = {};
								var action = 0;

								data = $scope.planattribute;

								if ($routeParams.action == 'copy') {
									data.Createdby = $scope.customer.id;
									data.ModifiedBy = '';

									PlanattributeService.AddPlanAttribute(data, function (response) {
										if (response.Status) {
											if (response.Status == 'false') {
												bootbox.alert(response.Message, function () {
													$scope.$apply();
												});
											} else {
												bootbox.alert(messages.saved, function () {
													if (type === 'close') {
														console.log('close');
														$location.path('insuranceplantype/planattribute');
													}
													else if (type === 'planbenefit') {
														console.log('insuranceplantype/planbenefit/' + JSON.parse(JSON.stringify($scope.planattribute)).PlanId + '/' + JSON.parse(JSON.stringify($scope.planattribute)).BusinessYear);
														$location.path('insuranceplantype/planbenefit/' + JSON.parse(JSON.stringify($scope.planattribute)).PlanId + '/' + JSON.parse(JSON.stringify($scope.planattribute)).BusinessYear);
													}
													else if (type === 'csrrate') {
														console.log('path csrrate', 'insuranceplantype/csrrate/' + JSON.parse(JSON.stringify($scope.planattribute)).StandardComponentId);
														$location.path('insuranceplantype/csrrate/' + JSON.parse(JSON.stringify($scope.planattribute)).StandardComponentId);
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
								} else {
									data.ModifiedBy = $scope.customer.id;
									PlanattributeService.UpdatePlanAttribute(data, function (response) {
										if (response.Status) {
											if (response.Status == 'false') {
												bootbox.alert(response.Message, function () {
													$scope.$apply();
												});
											} else {
												bootbox.alert(messages.saved, function () {
													if (type === 'close') {
														console.log('close');
														$location.path('insuranceplantype/planattribute');
													}
													else if (type === 'planbenefit') {
														console.log('insuranceplantype/planbenefit/' + JSON.parse(JSON.stringify($scope.planattribute)).PlanId + '/' + JSON.parse(JSON.stringify($scope.planattribute)).BusinessYear);
														$location.path('insuranceplantype/planbenefit/' + JSON.parse(JSON.stringify($scope.planattribute)).PlanId + '/' + JSON.parse(JSON.stringify($scope.planattribute)).BusinessYear);
													}
													else if (type === 'csrrate') {
														console.log('path csrrate', 'insuranceplantype/csrrate/' + JSON.parse(JSON.stringify($scope.planattribute)).StandardComponentId);
														$location.path('insuranceplantype/csrrate/' + JSON.parse(JSON.stringify($scope.planattribute)).StandardComponentId);
													}
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
							}
						}

						/*$scope.getMoreResultPlanId = function(value) {
							var searchedID = {searchByPlanId: value};
							console.log("searched data", searchedID);
				
							PlanattributeService.GetSearchPlanID(searchedID, function (response) {
								if (response.Status) {
									console.log("new plan id", response);
									$scope.PlanIds = response.PlanIds;
								}
							});
						};
				
						$scope.getSearchPlanID = function() {
							var searchedID = {searchByPlanId: ""};
							PlanattributeService.GetSearchPlanID(searchedID, function (response) {
								if (response.Status) {
									console.log("plan id", response);
									$scope.PlanIds = response.PlanIds;
								}
							});
						};*/

					}])

			/****
				Created By : Rahul Singh
				Created Date : 24-04-2016
				Start : Create New Plan Attribute Controller.
			****/

			.controller('NewPlanattributeController', ['$scope', '$rootScope', '$location', 'PlanattributeService',
				'checkCreds', 'businessServices', '$routeParams', '$filter', 'messages', '$timeout',
				function ($scope, $rootScope, $location, PlanattributeService, checkCreds, businessServices,
					$routeParams, $filter, messages, $timeout) {

					$scope.closeFlash = function (e) {
						$scope.flash.status = false;
					}
					$rootScope.pageLoading = false;
					$scope.dataLoading = true;
					$scope.id = '';
					$scope.planattribute = {};
					$scope.planattribute.PlanNumber = '000';
					$scope.years = [];
					$scope.conditionalExp = [];
					$scope.plans = [];
					$scope.marketCoverages = [];
					$scope.metalLevels = [];
					$scope.carriers = [];
					$scope.employers = [];
					$scope.states = [];
					$scope.insuranceTypes = [];
					$scope.approvalStatus = [];
					$scope.sortby = $routeParams.sortby;
					$scope.desc = $routeParams.desc;
					$scope.eventType = "";
					$scope.issuerIds = [];
					$scope.Createdby = '';
					$scope.ModifiedBy = '';
					$scope.groupNames = [];
					// $scope.PlanIds = [];

					$scope.breadcrumb = true;
					if (checkCreds() === true) {
						$scope.loggedIn = true;
						$scope.customer = businessServices.getUsername();
					} else {
						$scope.loggedIn = false;
						$location.path('/');
					}


					$scope.readOnly = false;
					$scope.actionBtnText = "Create";
					$scope.showBtn = true;
					$scope.title = 'New Plan Attribute';
					$scope.hideFields = true;

					$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li><a href="insuranceplantype/planattribute">Plan Attribute</a></li><li class="active">' + $scope.title + '</li>';

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
 						$.validator.setDefaults({ ignore: '' });
						$("#createPlanAttributeForm").validate({
							errorElement: 'span',
							errorClass: 'error',
							onkeyup: false,
							rules: {
								planId: {
									required: true,
								},
								planName: {
									required: true,
								},
								approvalStatus: {
									required: true,
								},
								carrierId: {
									required: true,
								},
								businessYear: {
									required: true,
								},
								StandardCompId: {
									required: true,
								},
								TEHBDedInnTier1Coinsurance: {
									maxlength: 16,
								},
								TEHBDedInnTier1Individual: {
									maxlength: 16,
								},
								TEHBDedInnTier1FamilyPerPerson: {
									maxlength: 16,
								},
								TEHBDedInnTier1FamilyPerGroup: {
									maxlength: 16,
								},
								TEHBDedInnTier2Coinsurance: {
									maxlength: 16,
								},
								TEHBDedInnTier2Individual: {
									maxlength: 16,
								},
								TEHBDedInnTier2FamilyPerPerson: {
									maxlength: 16,
								},
								TEHBDedInnTier2FamilyPerGroup: {
									maxlength: 16,
								},
								MEHBDedInnTier1Coinsurance: {
									maxlength: 16,
								},
								MEHBDedInnTier1Individual: {
									maxlength: 16,
								},
								MEHBDedInnTier1FamilyPerPerson: {
									maxlength: 16,
								},
								MEHBDedInnTier1FamilyPerGroup: {
									maxlength: 16,
								},
								MEHBDedInnTier2Coinsurance: {
									maxlength: 16,
								},
								MEHBDedInnTier2Individual: {
									maxlength: 16,
								},
								MEHBDedInnTier2FamilyPerPerson: {
									maxlength: 16,
								},
								MEHBDedInnTier2FamilyPerGroup: {
									maxlength: 16,
								},
								DEHBDedInnTier1Coinsurance: {
									maxlength: 16,
								},
								DEHBDedInnTier1Individual: {
									maxlength: 16,
								},
								DEHBDedInnTier1FamilyPerPerson: {
									maxlength: 16,
								},
								DEHBDedInnTier1FamilyPerGroup: {
									maxlength: 16,
								},
								DEHBDedInnTier2Coinsurance: {
									maxlength: 16,
								},
								DEHBDedInnTier2Individual: {
									maxlength: 16,
								},
								DEHBDedInnTier2FamilyPerPerson: {
									maxlength: 16,
								},
								DEHBDedInnTier2FamilyPerGroup: {
									maxlength: 16,
								},
								TEHBInnTier1IndividualMOOP: {
									maxlength: 16,
								},
								TEHBInnTier1FamilyPerPersonMOOP: {
									maxlength: 16,
								},
								TEHBInnTier1FamilyPerGroupMOOP: {
									maxlength: 16,
								},
								TEHBInnTier2IndividualMOOP: {
									maxlength: 16,
								},
								TEHBInnTier2FamilyPerPersonMOOP: {
									maxlength: 16,
								},
								TEHBInnTier2FamilyPerGroupMOOP: {
									maxlength: 16,
								},
								MEHBInnTier1IndividualMOOP: {
									maxlength: 16,
								},
								MEHBInnTier1FamilyPerPersonMOOP: {
									maxlength: 16,
								},
								MEHBInnTier1FamilyPerGroupMOOP: {
									maxlength: 16,
								},
								MEHBInnTier2IndividualMOOP: {
									maxlength: 16,
								},
								MEHBInnTier2FamilyPerPersonMOOP: {
									maxlength: 16,
								},
								MEHBInnTier2FamilyPerGroupMOOP: {
									maxlength: 16,
								},
								DEHBInnTier1IndividualMOOP: {
									maxlength: 16,
								},
								DEHBInnTier1FamilyPerPersonMOOP: {
									maxlength: 16,
								},
								DEHBInnTier1FamilyPerGroupMOOP: {
									maxlength: 16,
								},
								DEHBInnTier2IndividualMOOP: {
									maxlength: 16,
								},
								DEHBInnTier2FamilyPerPersonMOOP: {
									maxlength: 16,
								},
								DEHBInnTier2FamilyPerGroupMOOP: {
									maxlength: 16,
								},
								HIOSProductId: {
									maxlength: 50,
								},
								ServiceAreaId: {
									maxlength: 50,
								},
								PlanFormalName: {
									maxlength: 120,
								},
								PlanNotes: {
									maxlength: 1000,
								},

							}
						});

					});

					$scope.enrollDate = {};
					$scope.effectDate = {};
					$scope.expDate = {};
					$scope.enrollDate.isOpned = false;
					$scope.effectDate.isOpned = false;
					$scope.expDate.isOpned = false;

					$scope.open_enroll_date = function () {
						$scope.enrollDate.isOpned = true;
					};
					$scope.open_effect_start = function () {
						$scope.effectDate.isOpned = true;
					};
					$scope.open_exp_start = function () {
						$scope.expDate.isOpned = true;
					};

					$scope.onFieldsChange = function () {
						if ($scope.planattribute.MrktCover == 'GRP') {
							var Employer = _.find($scope.employers, function (item) {
								return item.EmployerId == $scope.planattribute.EmployerId;
							});

							var Insurance = _.find($scope.insuranceTypes, function (item) {
								return item.InsuranceTypeId == $scope.planattribute.InsuranceType;
							});

							if (!_.isUndefined(Employer) && !_.isUndefined(Insurance)) {
								$scope.planattribute.GroupName = Employer.EmployerName + ' ' + Insurance.InsuranceType1 + ' ' + $scope.planattribute.BusinessYear;
							}
						}
					};

					$scope.carrierIdChange = function () {
						let tempIssuer = $scope.issuerIds.filter(function (item) { return item.Id == $scope.planattribute.CarrierId; })[0];
						$scope.planattribute.IssuerName = tempIssuer.IssuerName;
					}

					for (var i = 2015; i < 2045; i++) {
						$scope.years.push({ "val": i });
					}

					$scope.conditionalExp.push({ "key": true, "val": "True" });
					$scope.conditionalExp.push({ "key": false, "val": "False" });

					var IsChangesOccur = false;
					$scope.ChangesOccur = function () {
						IsChangesOccur = true;
						console.log('changes occur', IsChangesOccur);
					}

					$scope.navigate = function (type) {
						console.log(JSON.parse(JSON.stringify($scope.planattribute)))
						if (type === 'close') {
							if (IsChangesOccur != false) {
								bootbox.dialog({
									message: 'Do you want to save plan?',
									title: "",
									buttons: {
										success: {
											label: "Yes",
											className: "btn-success",
											callback: function () {
												$scope.formLoading = true;
												$scope.editplanattribute(function () {
													$location.path('insuranceplantype/planattribute');
													$scope.formLoading = false;
													$scope.$apply();
												});
											}
										},
										danger: {
											label: "No",
											className: "btn-danger",
											callback: function () {
												$location.path('insuranceplantype/planattribute');
												$scope.formLoading = false;
												$scope.$apply();
											}
										}
									}
								});

							} else {
								$scope.formLoading = false;
								$location.path('insuranceplantype/planattribute');
								$timeout(function () {
									$scope.$apply();
								}, 300)
							}
						}
						else if (type === 'planbenefit') {
							if (IsChangesOccur != false) {
								bootbox.dialog({
									message: 'Do you want to save plan?',
									title: "",
									buttons: {
										success: {
											label: "Yes",
											className: "btn-success",
											callback: function () {
												$scope.formLoading = true;
												$scope.editplanattribute(function () {
													$location.path('insuranceplantype/planbenefit?planid=' + $scope.planattribute.PlanId + '&businessyear=' + $scope.planattribute.BusinessYear);
													$scope.formLoading = false;
													$scope.$apply();
												});
											}
										},
										danger: {
											label: "No",
											className: "btn-danger",
											callback: function () {
												$location.path('insuranceplantype/planbenefit?planid=' + $scope.planattribute.PlanId + '&businessyear=' + $scope.planattribute.BusinessYear);
												$scope.formLoading = false;
												$scope.$apply();
											}
										}
									}
								});

							} else {
								$scope.formLoading = false;
								$location.path('insuranceplantype/planbenefit?planid=' + $scope.planattribute.PlanId + '&businessyear=' + $scope.planattribute.BusinessYear);
							}
						}
						else if (type === 'csrrate') {
							if (IsChangesOccur != false) {
								bootbox.dialog({
									message: 'Do you want to save plan?',
									title: "",
									buttons: {
										success: {
											label: "Yes",
											className: "btn-success",
											callback: function () {
												$scope.formLoading = true;
												$scope.editplanattribute(function () {
													$location.path('insuranceplantype/csrrate?planid=' + $scope.planattribute.PlanId);
													$scope.formLoading = false;
													$scope.$apply();
												});
											}
										},
										danger: {
											label: "No",
											className: "btn-danger",
											callback: function () {
												$location.path('insuranceplantype/csrrate?planid=' + $scope.planattribute.PlanId);
												$scope.formLoading = false;
												$scope.$apply();
											}
										}
									}
								});
							} else {
								$scope.formLoading = false;
								$location.path('insuranceplantype/csrrate?planid=' + $scope.planattribute.PlanId);
								$scope.$apply();
							}
						}
					}

					PlanattributeService.waitForLayoutEdit(function (response) {
						console.log("===========", response);
						$rootScope.pageLoading = false;
						$scope.dataLoading = true;
						$scope.plans = ((typeof response[0].data != 'undefined') && (response[0].data.Status)) ? response[0].data.Plans : [];
						$scope.marketCoverages = ((typeof response[1].data != 'undefined') && (response[1].data.Status)) ? response[1].data.lstMarketCoverage : [];
						$scope.metalLevels = ((typeof response[2].data != 'undefined') && (response[2].data.Status)) ? response[2].data.lstMarketCoverage : [];
						// $scope.carriers=((typeof response[3].data !='undefined') && (response[3].data.Status))?response[3].data.Carriers:[];	
						$scope.employers = ((typeof response[3].data != 'undefined') && (response[3].data.Status)) ? response[3].data.lstEmployerMaster : [];
						$scope.states = ((typeof response[4].data != 'undefined') && (response[4].data.Status)) ? response[4].data.States : [];
						$scope.insuranceTypes = ((typeof response[5].data != 'undefined') && (response[5].data.Status)) ? response[5].data.lstInsuranceTypesMaster : [];
						$scope.approvalStatus = ((typeof response[6].data != 'undefined') && (response[6].data.Status)) ? response[6].data.ApprovalStatus : [];
						$scope.issuerIds = ((typeof response[7].data != 'undefined') && (response[7].data.Status)) ? response[7].data.CarrierIds : [];

						$scope.groupNames = ((typeof response[8].data != 'undefined') && (response[8].data.Status)) ? response[8].data.GroupName : [];

						// $scope.getSearchPlanID();
						$scope.planattribute.ApprovalStatus = "1";
						$scope.planattribute.OpenForEnrollment = "false"
						$scope.dataLoading = false;
					});

					/**
						Palne Attribute Update api.
					*/
					$scope.editplanattribute = function (isValid) {
						console.log(isValid);
						console.log($("#createPlanAttributeForm").valid({
			        ignore: [],
			    }));
						// console.log($("#createPlanAttributeForm").checkValidity())
						// console.log($("#createPlanAttributeForm.$pristine"));
						if ($("#createPlanAttributeForm").valid()) {
							$scope.formLoading = true;
							// $scope.planattribute.PlanId = $scope.planattribute.PlanId.PlanId;
							var data = {};
							var action = 0;

							data = $scope.planattribute;
							data.CreatedBy = $scope.customer.id;
							//data.ModifiedBy = $scope.customer.id;
							console.log("-----------", data);
							PlanattributeService.AddPlanAttribute(data, function (response) {
								if (response.Status) {
									console.log('response', response)
									if (response.Status == 'false') {
										bootbox.alert(response.Message, function () {
											$scope.$apply();
										});
									} else {
										bootbox.alert(messages.saved, function () {
											$location.path('insuranceplantype/planattribute');
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
					};

				}]);

			/** End : Edit Plan Attribute Controller. **/