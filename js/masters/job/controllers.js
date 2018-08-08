'use strict';
/**
	Created By : Aastha Jain
	Created Date : 11-07-2016
	Start : Job Mstr List Controller.
*/
angular.module('mhmApp.job', ['multi-select'])
	.controller('JobController',
		['$scope', '$rootScope', '$location', 'JobService', 'checkCreds', 'businessServices', 'messages', '$filter', '$timeout', '$window',
			function ($scope, $rootScope, $location, JobService, checkCreds, businessServices, messages, $filter, $timeout, $window) {

				// reset login status	
				var d = new Date();
				var curr_year = d.getFullYear();
				$scope.title = 'Job';
				$scope.messages = messages;

				$scope.searchby = '';		//Job Number
				$scope.searchByJobYear = '';
				$scope.searchByEmployerId = '';
				$scope.searchByJobStatusId = '';
				$scope.searchByJobDateStart = '';
				$scope.searchByJobDateEnd = '';

				$scope.TempSearch = {};
				$scope.TempSearch.searchby = '';		//Job Number
				$scope.TempSearch.searchByJobYear = '';
				$scope.TempSearch.searchByEmployerId = '';
				$scope.TempSearch.searchByJobStatusId = '';
				$scope.TempSearch.searchByJobDateStart = '';
				$scope.TempSearch.searchByJobDateEnd = '';

				$scope.sortby = 'Id';
				$scope.desc = true;
				$scope.page = 1;
				$scope.pageSize = messages.pageSize;

				$scope.jobmstrlist = [];
				$scope.jobnumbers = [];
				$scope.jobyears = [];
				$scope.employers = [];
				$scope.jobstatus = [];

				$scope.TotalCount = 0;
				$scope.lastCount = 0;
				$scope.breadcrumb = true;
				$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li class="active">Job</li>';
				$scope.dataLoading = true;

				$scope.sessionPage = "JobSession";
				$scope.searchSession = '';

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
				$scope.$watch('TempSearch.searchByJobDateEnd', function () {
					if ($scope.TempSearch.searchByJobDateEnd != null) {
						$scope.maxStartDate = angular.copy($scope.TempSearch.searchByJobDateEnd);
					} else {
						$scope.maxStartDate = '';
					}

				}, true);
				$scope.$watch('TempSearch.searchByJobDateStart', function () {
					$scope.minEndDate = angular.copy($scope.TempSearch.searchByJobDateStart);

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
						$scope.getJobMstrList();
					}
				}

				JobService.waitForLayout(function (response) {
					$rootScope.pageLoading = false;
					$scope.dataLoading = false;

					$scope.employers = ((typeof response[0].data != 'undefined') && (response[0].data.Status)) ? response[0].data.lstEmployerMaster : [];
					$scope.jobStatus = ((typeof response[1].data != 'undefined') && (response[1].data.Status)) ? response[1].data.JobStatus : [];

					if ($scope.pageSize != undefined && $scope.pageSize != '') {
						$scope.page = 1;

						if ($scope.searchSession != '')
							$scope.setJobMstrList();

						$scope.getJobMstrList();
					}
				});

				for (var i = 2015; i < 2045; i++) {
					$scope.jobyears.push({ "val": i });
				}

				$scope.setJobMstrList = function () {
					$scope.searchby = $scope.TempSearch.searchby;
					$scope.searchByJobYear = $scope.TempSearch.searchByJobYear;
					$scope.searchByEmployerId = $scope.TempSearch.searchByEmployerId;
					$scope.searchByJobStatusId = $scope.TempSearch.searchByJobStatusId;
					if ($scope.TempSearch.searchByJobDateStart) {
						$scope.TempSearch.searchByJobDateStart = $filter('date')(new Date($scope.TempSearch.searchByJobDateStart), 'MM/dd/yyyy');
					}
					if ($scope.TempSearch.searchByJobDateEnd) {
						$scope.TempSearch.searchByJobDateEnd = $filter('date')(new Date($scope.TempSearch.searchByJobDateEnd), 'MM/dd/yyyy');
					}
					$scope.page = 1;
					$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
					$scope.getJobMstrList();
				}

				$scope.resetJobMstrList = function () {
					$scope.TempSearch.searchby = $scope.searchby = '';
					$scope.TempSearch.searchByJobYear = $scope.searchByJobYear = '';
					$scope.TempSearch.searchByEmployerId = $scope.searchByEmployerId = '';
					$scope.TempSearch.searchByJobStatusId = $scope.searchByJobStatusId = '';
					$scope.TempSearch.searchByJobDateStart = $scope.searchByJobDateStart = '';
					$scope.TempSearch.searchByJobDateEnd = $scope.searchByJobDateEnd = '';
					$scope.page = 1;
					businessServices.resetSearchSession($scope.sessionPage);
					$scope.getJobMstrList();
				}

				$scope.getJobMstrList = function () {
					$timeout(function () {
						$rootScope.pageLoading = false;
						$scope.dataLoading = true;
						JobService.getAll($scope, function (response) {
							if (response.Status) {
								$scope.jobmstrlist = response.lstJobMasters;
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
								$scope.jobmstrlist = [];
							}
						});
					})

				}

				// change sorting order
				$scope.sort_by = function (newSortingOrder) {
					if ($scope.sortby == newSortingOrder)
						$scope.desc = !$scope.desc;
					$scope.sortby = newSortingOrder;
					$scope.page = 1;
					$scope.getJobMstrList();
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
						$scope.getJobMstrList();
					}
				};

				$scope.nextPage = function () {
					if ($scope.page < $scope.TotalCount - 1) {
						$scope.page++;
						$scope.getJobMstrList();
					}
				};

				$scope.prevPage = function () {
					if ($scope.page > 1) {
						$scope.page--;
						$scope.getJobMstrList();
					}
				};

				$scope.openPopUP = function () {
					if ($(".adv_srch_btn_box").css('display') == 'none') {
						$(".adv_srch_btn_box").show();
					} else {
						$(".adv_srch_btn_box").hide();
					}
				}

				$rootScope.pageLoading = false;

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
				});

			}
		])

	/** End : Job Mstr List Controller. **/

	/**
		Created By : Aastha Jain
		Created Date : 14-07-2016
		Start : Job Mstr Create Controller.
	*/
	.controller('CreateJobController',
		['$scope', '$rootScope', '$location', 'JobService', 'checkCreds', 'businessServices', '$routeParams', '$filter', 'messages', '$timeout', '$q', '$window',
			function ($scope, $rootScope, $location, JobService, checkCreds, businessServices, $routeParams, $filter, messages, $timeout, $q, $window) {
				$scope.data = {};
				$scope.data.post = [];
				$scope.closeFlash = function (e) {
					$scope.flash.status = false;
				}

				$scope.planFiltersData = {};
				$scope.issuerIds = [];
				$scope.States = [];
				$scope.Plans = [];
				$scope.years = [];
				$scope.GroupNames = [];
				$scope.MarketCoverages = [];
				$scope.allPlans = [];
				$scope.CensusData = [];

				/**
					Check login credentials.
				*/
				$scope.dataLoading = false;

				$scope.formInvalid = false;

				if (checkCreds() === true) {
					$scope.loggedIn = true;
					$scope.customer = businessServices.getUsername();
				} else {
					$scope.loggedIn = false;
					$location.path('/');
				}

				$scope.title = 'Add Job';
				$scope.job = {};
				$scope.job.JobPlansSelectionLocked = "";
				$scope.job.HRACanCoverPremium = "";
				$scope.CreatedByName = "";
				$scope.ModifiedByName = "";
				$scope.test = true;
				$scope.conditionalExp = [];
				$scope.conditionalExp.push({ "key": true, "val": "True" });
				$scope.conditionalExp.push({ "key": false, "val": "False" });
				$scope.jobNo = $routeParams.jobNo;
				$scope.action = "add";
				$scope.isEditForm = false;
				$scope.actionBtnText = "Create";

				$scope.messages = messages;
				$scope.job.InsuranceTypeId = '';

				$scope.jobyears = [];

				$scope.jobRunStatus = '';

				$scope.startDate = {};
				$scope.endDate = {};
				$scope.startDate.isOpned = false;
				$scope.endDate.isOpned = false;

				$scope.IsBackButton = false;

				$scope.job.IsHSAMatch = 0;
				$scope.job.HSAMatchLimit1 = 0;
				$scope.job.HSAMatchRate1 = 0;
				$scope.job.HSAMatchLimit2 = 0;
				$scope.job.HSAMatchRate2 = 0;
				$scope.job.HSAMatchLimit3 = 0;
				$scope.job.HSAMatchRate3 = 0;
				$scope.job.HSAMatchLimit4 = 0;
				$scope.job.HSAMatchRate4 = 0;
				$scope.isHSAMatchFlag = false;
				$scope.job.PayPeriodsPerYear = 0;
				$scope.job.WellnessOffered = false;

				for (var i = 2015; i < 2045; i++) {
					$scope.years.push({ "val": i });
				}

				$scope.open_start = function ($event) {
					$event.preventDefault();
					$event.stopPropagation();
					$scope.startDate.isOpned = true;
				};
				$scope.open_end = function () {
					$scope.endDate.isOpned = true;
				};
				$scope.$watch('job.JobDateEnd', function () {
					if ($scope.job.JobDateEnd != null) {
						$scope.maxStartDate = angular.copy($scope.job.JobDateEnd);
					} else {
						$scope.maxStartDate = '';
					}

				}, true);
				$scope.$watch('job.JobDateStart', function () {
					$scope.minEndDate = angular.copy($scope.job.JobDateStart);

				}, true);

				// start Check validation Plan Start date and Plan end Date 17/02/2018

				$scope.$watch('job.PlanYearEndDt', function () {
					if ($scope.job.PlanYearEndDt != null) {
						$scope.maxStartDate1 = angular.copy($scope.job.PlanYearEndDt);
					} else {
						$scope.maxStartDate1 = '';
					}

				}, true);

				$scope.$watch('job.PlanYearStartDt', function () {
					$scope.minEndDate1 = angular.copy($scope.job.PlanYearStartDt);

				}, true);

				// End Check validation Plan Start date and Plan end Date 17/02/2018

				$scope.set_width = function (max, currentValue) {
				if(max >10000)
					currentValue = currentValue*90/100;
				else if(max >12500)
					currentValue = currentValue*80/100;
				else if(max >15000)
					currentValue = currentValue*70/100;
				else if(max >20000)
					currentValue = currentValue*60/100;
			return {
					width: Math.round(currentValue * 100 / max) + '%'
				}
			}

			$scope.check_width = function (max, currentValue) {
				if(max >10000)
					currentValue = currentValue*90/100;
				else if(max >12500)
					currentValue = currentValue*80/100;
				else if(max >15000)
					currentValue = currentValue*70/100;
				else if(max >20000)
					currentValue = currentValue*60/100;
				return Math.round(currentValue * 100 / max);
			}


				$scope.pro_red = '30%';
				$scope.pro_green = '17.5%';


				JobService.waitForLayoutCreate(function (response) {
					$rootScope.pageLoading = false;
					$scope.dataLoading = true;
					$scope.employers = ((typeof response[0].data != 'undefined') && (response[0].data.Status)) ? response[0].data.lstEmployerMaster : [];
					$scope.jobStatus = ((typeof response[1].data != 'undefined') && (response[1].data.Status)) ? response[1].data.JobStatus : [];
					$scope.NewJobNum = ((typeof response[2].data != 'undefined') && (response[2].data.Status)) ? response[2].data.NewJobNum : [];
					$scope.caseJobRunStatus = ((typeof response[3].data != 'undefined') && (response[3].data.Status)) ? response[3].data.CaseJobRunStatus : [];
					$scope.usageCode = ((typeof response[4].data != 'undefined') && (response[4].data.Status)) ? response[4].data.Usagecodes : [];
					$scope.insuranceTypes = ((typeof response[5].data != 'undefined') && (response[5].data.Status)) ? response[5].data.lstInsuranceTypesMaster : [];
					$scope.plans = ((typeof response[6].data != 'undefined') && (response[6].data.Status)) ? response[6].data.Plans : [];
					$scope.issuerIds = ((typeof response[7].data != 'undefined') && (response[7].data.Status)) ? response[7].data.CarrierIds : [];

					$scope.States = ((typeof response[8].data != 'undefined') && (response[8].data.Status)) ? response[8].data.States : [];
					$scope.Plans = ((typeof response[9].data != 'undefined') && (response[9].data.Status)) ? response[9].data.Plans : [];
					$scope.GroupNames = ((typeof response[10].data != 'undefined') && (response[10].data.Status)) ? response[10].data.GroupName : [];
					$scope.MarketCoverages = ((typeof response[11].data != 'undefined') && (response[11].data.Status)) ? response[11].data.lstMarketCoverage : [];
					// alert($scope.action);
					if ($scope.action == "add") {
						$scope.job.OldJobNumber = $scope.job.JobNumber;
						$scope.job.JobNumber = $scope.NewJobNum;
					}


					if ($scope.pageSize != undefined && $scope.pageSize != '') {
						$scope.page = 1;
						$scope.getJobMstrList();
					}

					$scope.getPlanInfo(0);
					$scope.dataLoading = false;
				});

				for (var i = 2015; i < 2045; i++) {
					$scope.jobyears.push({ "val": i });
				}

				$scope.$on('$includeContentLoaded', function (event) {
					$('#wizard').smartWizard({ enableAllSteps: true });
				});

				var IsJobDetailsChange = false;
				$scope.chagesOccur = function () {
					if ($scope.action != "add") {
						if ($scope.job.CurrentJobStatus == "Open" || $scope.job.CurrentJobStatus == "Cancelled" || $scope.job.CurrentJobStatus == "Billed" || $scope.job.CurrentJobStatus == "Closed") {
							console.log('chagesOccur')
							IsJobDetailsChange = true;
						}
					}
				}

				$scope.jobStatusChange = function () {
					if ($scope.action != "add") {
						if ($scope.job.CurrentJobStatus == "New" && ($scope.job.JobStatus == "Open" || $scope.job.JobStatus == "Cancelled" || $scope.job.JobStatus == "Billed" || $scope.job.JobStatus == "Closed"))
						{
							console.log('$scope.job.CurrentJobStatus', $scope.job.CurrentJobStatus);
							$scope.job.JobPlansSelectionLocked = 'true';
							$scope.job.JobPlansSelectionLockedDt = new Date();
						}
					}
				}

				$scope.jobPlansSelectionChange = function () {
						if ($scope.job.JobPlansSelectionLocked == 'true' )
						{
							$scope.job.JobPlansSelectionLockedDt = new Date();
						}
				}

				/**
					Job Master Add API.
				*/
				$scope.addJob = function () {

					if ($("#createJobForm").valid()) {
						if ($scope.job.EmailSignText == '' || $scope.job.EmailBodyText == '') {
							$scope.formInvalid = true;
							return;
						} else {
							$scope.formInvalid = false;
						}

						if ($scope.action != "add") {

							// if (IsJobDetailsChange) {
							// 	bootbox.alert('This Job is ' + $scope.job.CurrentJobStatus + ' and cannot be edited', function () {
							// 		$scope.$apply();
							// 	});
							// 	return;
							// }
							$scope.job.CurrentJobStatus = $scope.job.JobStatus;
						}

						if(new Date($scope.job.PlanYearStartDt) > new Date($scope.job.PlanYearEndDt))
						{
							bootbox.alert('PlanYearStartDt is later than PlanYearEndDt', function () {
									$scope.$apply();
								});
								return;
						}

						$scope.dataLoading = true;

						var data = {};

						if ($scope.job.OldJobNumber != undefined) {
							$scope.job.ModifiedBy = $scope.customer.id;
						} else {
							$scope.job.Createdby = $scope.customer.id;
							$scope.job.ModifiedBy = $scope.customer.id;
						}

						var jobStartDate = $scope.job.JobDateStart;
						var jobEndDate = $scope.job.JobDateEnd;
						var jobExpectedCompletionDt = $scope.job.ExpectedCompletionDt;
						var jobPlanYearStartDate = $scope.job.PlanYearStartDt;
						var jobPlanYearEndDate = $scope.job.PlanYearEndDt;
						var jobPlansSelectionLockedDate = $scope.job.JobPlansSelectionLockedDt;
						var jobCensusImportDate = $scope.job.JobCensusImportDt;



						if ($scope.job.JobDateStart) {
							$scope.job.JobDateStart = $filter('date')(new Date($scope.job.JobDateStart), 'MM/dd/yyyy') + ' 00:00:00';
						}
						if ($scope.job.JobDateEnd) {
							$scope.job.JobDateEnd = $filter('date')(new Date($scope.job.JobDateEnd), 'MM/dd/yyyy') + ' 23:59:59';
						}
						if ($scope.job.jobExpectedCompletionDt) {
							$scope.job.jobExpectedCompletionDt = $filter('date')(new Date($scope.job.jobExpectedCompletionDt), 'MM/dd/yyy');
						}
						if ($scope.job.jobPlanYearStartDate) {
							$scope.job.jobPlanYearStartDate = $filter('date')(new Date($scope.job.jobPlanYearStartDate), 'MM/dd/yyyy');
						}

						if ($scope.job.jobPlanYearEndDate) {
							$scope.job.jobPlanYearEndDate = $filter('date')(new Date($scope.job.jobPlanYearEndDate), 'MM/dd/yyyy');
						}

						if ($scope.job.jobPlansSelectionLockedDate) {
							$scope.job.jobPlansSelectionLockedDate = $filter('date')(new Date($scope.job.jobPlansSelectionLockedDate), 'MM/dd/yyy');
						}
						if ($scope.job.jobCensusImportDate) {
							$scope.job.jobCensusImportDate = $filter('date')(new Date($scope.job.jobCensusImportDate), 'MM/dd/yyyy');
						}

						if ($scope.job.ExpectedCompletionDt) {
							$scope.job.ExpectedCompletionDt = $filter('date')(new Date($scope.job.ExpectedCompletionDt), 'MM/dd/yyyy');
						}
						if ($scope.job.PlanYearStartDt) {
							$scope.job.PlanYearStartDt = $filter('date')(new Date($scope.job.PlanYearStartDt), 'MM/dd/yyyy');
						}
						if ($scope.job.PlanYearEndDt) {
							$scope.job.PlanYearEndDt = $filter('date')(new Date($scope.job.PlanYearEndDt), 'MM/dd/yyyy');
						}
						if ($scope.job.JobPlansSelectionLockedDt) {
							$scope.job.JobPlansSelectionLockedDt = $filter('date')(new Date($scope.job.JobPlansSelectionLockedDt), 'MM/dd/yyyy');
						}
						if ($scope.job.JobCensusImportDt) {
							$scope.job.JobCensusImportDt = $filter('date')(new Date($scope.job.JobCensusImportDt), 'MM/dd/yyyy');
						}

						data = $scope.job;

						JobService.jobAction(data, $scope.action, function (response) {

							if (response.Status) {
								bootbox.alert(messages.saved, function () {
									$scope.dataLoading = false;
									$scope.formLoading = false;
									if (!$scope.IsBackButton) $location.path('masters/editJob/' + $scope.job.JobNumber + '/update');
									else $location.path('masters/job');
									$scope.$apply();
								});
							} else {
								if (response.redirect) {
									bootbox.alert(messages.TryLater, function () {
										$location.path('/');
										$scope.$apply();
									})
								}

								$scope.job.JobDateStart = jobStartDate;
								$scope.job.JobDateEnd = jobEndDate;

								$scope.job.ExpectedCompletionDt = jobExpectedCompletionDt;
								$scope.job.PlanYearStartDt = jobPlanYearStartDate;
								$scope.job.PlanYearEndDt = jobPlanYearEndDate;
								$scope.job.JobPlansSelectionLockedDt = jobPlansSelectionLockedDate;
								$scope.job.JobCensusImportDt = jobCensusImportDate;


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
						if ($scope.job.EmailSignText == '' || $scope.job.EmailBodyText == '') {
							$scope.formInvalid = true;
							return;
						} else {
							$scope.formInvalid = false;
						}
					}
				};

				if ($scope.jobNo != "" && $scope.jobNo != undefined) {

					if ($routeParams.action == 'update') {
						$scope.title = "Edit Job";
						$scope.action = "edit";
						$scope.isEditForm = true;
						$scope.actionBtnText = "Update";
					}

					JobService.getJobDetail($scope.jobNo, function (response) {
						if (response.Status) {

							$scope.job = response.JobMasters;
							$scope.CreatedByName = response.CreatedByName;
							$scope.ModifiedByName = response.ModifiedByName;
							$scope.job.JobPlansSelectionLocked = $scope.job.JobPlansSelectionLocked ? $scope.job.JobPlansSelectionLocked.toString() : "false";
							$scope.job.HRACanCoverPremium = $scope.job.HRACanCoverPremium ? $scope.job.HRACanCoverPremium.toString() : "false";
							$scope.job.WellnessOffered = $scope.job.WellnessOffered ? $scope.job.WellnessOffered.toString() : "false";
							
							if($scope.job.JobDateStart)
								$scope.job.JobDateStart = new Date($scope.job.JobDateStart);
							if($scope.job.JobDateEnd)
								$scope.job.JobDateEnd = new Date($scope.job.JobDateEnd);
							if($scope.job.ExpectedCompletionDt)
								$scope.job.ExpectedCompletionDt = new Date($scope.job.ExpectedCompletionDt);
							if($scope.job.PlanYearStartDt)
								$scope.job.PlanYearStartDt = new Date($scope.job.PlanYearStartDt);
							if ($scope.job.PlanYearEndDt != null) {
								$scope.job.PlanYearEndDt = new Date($scope.job.PlanYearEndDt);
							}
							if($scope.job.JobPlansSelectionLockedDt)
								$scope.job.JobPlansSelectionLockedDt = new Date($scope.job.JobPlansSelectionLockedDt);
							if($scope.job.JobCensusImportDt)
								$scope.job.JobCensusImportDt = new Date($scope.job.JobCensusImportDt);

							$scope.job.CurrentJobStatus = $scope.job.JobStatus;

							if ($scope.job.CreatedDateTime) {
								$scope.job.CreatedDateTime = $filter('date')(new Date($scope.job.CreatedDateTime), 'MM/dd/yyyy HH:mm:ss');
							}
							if ($scope.job.ModifiedDateTime) {
								$scope.job.ModifiedDateTime = $filter('date')(new Date($scope.job.ModifiedDateTime), 'MM/dd/yyyy HH:mm:ss');
							}

							if ($scope.job.InsuranceTypeId != null) { $scope.job.InsuranceTypeId = $scope.job.InsuranceTypeId.toString(); }
							if ($scope.job.EmployerId)
								$scope.job.EmployerId = $scope.job.EmployerId.toString();

							if ($scope.job.IsHSAMatch)
								$scope.isHSAMatchFlag = true;

							// if($scope.NewJobNum != "" && $scope.NewJobNum != undefined && $scope.NewJobNum != null)
							// {
							// 	$scope.job.JobNumber = $scope.NewJobNum;
							// }

							if ($scope.action == "add") {
								$scope.job.OldJobNumber = $scope.job.JobNumber;
								$scope.job.JobNumber = $scope.NewJobNum;
								// console.log('a',$scope.job.OldJobNumber);
								// console.log('a',$scope.job.JobNumber);
							}

							$scope.getInsuranceType($scope.job.InsuranceTypeId);
							$scope.dataLoading = false;
						} else {
							$location.path('masters/job');
						}
					});

				}

				/***
					Created By : Aastha Jain
					Created Date : 27-06-2016
					Start : Copy Default Email Info 
				***/

				$scope.copyDefault = function () {

					$scope.job.EmailSubjText = "Case : ##CaseTitle##"
					$scope.job.EmailBodyText = "Dear ##ApplicantName## <br/> <br/> Attached is your MyHealthMath report. We've done the math behind the amount you will pay in premiums (gold color), as well as an estimate of your share of your medical expenses (green color). <br/> <br/>The graph shows the totals, and the table below shows the breakdown behind the totals; color coding ties them together. <br/> <br/>We hope that you find these calculations helpful. Please let us know if you have any questions. <br/><br/>&#34;Note, your top ranked plan is ##PlanTotalCostRange## less expensive than your lowest ranked plan.&#34;";
					$scope.job.EmailSignText = "Best regards, <br/> <br/> ##AgentName## <br/>MyHealthMath Analyst <br/> ##AgentEmail## <br/> ##AgentPhone## ";

				}

				$scope.items = [];
				/*** End : Copy Default Email Info ***/

				$scope.SendError = 0;
				$scope.GenError = 0;
				$scope.GeneratedOnly = 0;
				$scope.ReportSent = 0;

				$scope.sendReportsToAll = function (action) {

					$scope.SendError = 0;
					$scope.GenError = 0;
					$scope.GeneratedOnly = 0;
					$scope.ReportSent = 0;

					var mailcontainer = {};

					var CaseStatusIds = [];

					var messageText = '';
					if (action == 'NoSend') {
						messageText = '<div> <button type="button" class="close" data-dismiss="modal" aria-label="Close"></button> <h2 class="text-left mb0">Generate Results?</h2></div><div class="modal-body pull-left"> <div class="col-md-12 text-left"> <input type="checkbox" value="3" id="chkNew"> <label>New</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="16" id="chkNewLY"> <label>New - LY Copy</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="10" id="chkNoshow"> <label>No Show </label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="12" id="chkInProcessMed"> <label>InProcess Med Use </label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="13" id="chkInProcessRx"> <label>InProcess Rx Tiers </label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="14" id="chkInProcessFollow"> <label>InProcess Follow Up</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="15" id="chkInProcessSystem"> <label>InProcess System Issue</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="1" id="chkOpen"> <label>InProcess Other </label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="5" id="chkFinalNotSent"> <label>Final - Not Sent</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="9" id="chkFinalhold"> <label>Final - Hold </label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="2" id="chkFinalSent"> <label>Final - Sent</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="6" id="chkTest"> <label>Test</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="8" id="chkTestSent"> <label>Test - Sent </label> </div></div>';
					}
					else {
						messageText = '<div> <h2 class="mb0 text-left">'+messages.confirmationJobReportSend+'</h2> </button></div><div class="modal-body pull-left"> <div class="col-md-12 text-left"> <input type="checkbox" value="3" id="chkNew"> <label>New</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="16" id="chkNewLY"> <label>New - LY Copy</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="10" id="chkNoshow"> <label>No Show</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="12" id="chkInProcessMed"> <label>InProcess Med Use</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="13" id="chkInProcessRx"> <label>InProcess Rx Tiers</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="14" id="chkInProcessFollow"> <label>InProcess Follow Up</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="15" id="chkInProcessSystem"> <label>InProcess System Issue</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="1" id="chkOpen"> <label>InProcess Other</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="5" id="chkFinalNotSent"> <label>Final - Not Sent</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="9" id="chkFinalhold"> <label>Final - Hold</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="2" id="chkFinalSent"> <label>Final - Sent</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="6" id="chkTest"> <label>Test</label> </div><div class="col-md-12 text-left"> <input type="checkbox" value="8" id="chkTestSent"> <label>Test - Sent</label> </div></div>';
						//CaseStatusIds.push(5);
					}

					bootbox.dialog({
						message: messageText,
						title: "",
						buttons: {
							success: {
								label: "Ok",
								className: "btn-success",
								callback: function () {
									if ($('#chkOpen').is(":checked")) {
										CaseStatusIds.push($('#chkOpen').val());
									}
									if ($('#chkFinalSent').is(":checked")) {
										CaseStatusIds.push($('#chkFinalSent').val());
									}
									if ($('#chkNew').is(":checked")) {
										CaseStatusIds.push($('#chkNew').val());
									}
									if ($('#chkFinalNotSent').is(":checked")) {
										CaseStatusIds.push($('#chkFinalNotSent').val());
									}
									if ($('#chkTest').is(":checked")) {
										CaseStatusIds.push($('#chkTest').val());
									}
									// if ($('#chkCopy').is(":checked")) {
									// 	CaseStatusIds.push($('#chkCopy').val());
									// }

									if ($('#chkTestSent').is(":checked")) {
										CaseStatusIds.push($('#chkTestSent').val());
									}
									if ($('#chkFinalhold').is(":checked")) {
										CaseStatusIds.push($('#chkFinalhold').val());

									} if ($('#chkNoshow').is(":checked")) {
										CaseStatusIds.push($('#chkNoshow').val());

									} if ($('#chkInProcessMed').is(":checked")) {
										CaseStatusIds.push($('#chkInProcessMed').val());

									} if ($('#chkInProcessRx').is(":checked")) {
										CaseStatusIds.push($('#chkInProcessRx').val());

									} if ($('#chkInProcessFollow').is(":checked")) {
										CaseStatusIds.push($('#chkInProcessFollow').val());

									} if ($('#chkInProcessSystem').is(":checked")) {
										CaseStatusIds.push($('#chkInProcessSystem').val());

									} if ($('#chkNewLY').is(":checked")) {
										CaseStatusIds.push($('#chkNewLY').val());
									}
									$scope.caseLoading = false;
									JobService.updateAllCases($scope.jobNo, $scope.jobRunStatus, CaseStatusIds, function (response) {
										if (response.Status) {
											callServiceForEachItem(action, CaseStatusIds);
										}
										else {
											bootbox.alert(response.TryLater);
										}
									});

								}
							},
							danger: {
								label: "Cancel",
								className: "btn-danger"
							}
						}
					});
				}


				$scope.CurrentPlanName = '';
				$scope.CurrentPlan = '';
				function callServiceForEachItem(action, CaseStatusIds) {
					$scope.dataLoading = true;
					JobService.getAllCases($scope.jobNo, $scope.jobRunStatus, CaseStatusIds, function (response) {
						$scope.Case = {};

						$("body").trigger("mousemove");

						if (response.Case != '' && response.Case != undefined) {
							$scope.Case = response.Case;
							$scope.caseLoading = true;
							$scope.caseLoadingText = 'Case No. ' + $scope.Case.CaseID + ' Processing';
							$scope.Applicant = $scope.Case.Applicant;

							if (action == 'NoSend') {

								console.log('Generate-Not Send')
								var data = {};
								$scope.Applicant = {};
								data.HSAPercentage = $scope.Case.HSAFunding;
								data.TaxRate = $scope.Case.TaxRate;
								data.IssuerId = $scope.Case.IssuerID;
								data.PlanID = $scope.Case.PlanID;
								$scope.Applicant = $scope.Case.Applicant;
								data.EmployerId = $scope.Applicant.EmployerId;
								// data.InsuranceTypeId = $scope.Applicant.InsuranceTypeId;
								data.BusinessYear = $scope.Case.Year;
								data.JobNumber = $scope.jobNo;


								if (!($scope.Case.IssuerID && $scope.Case.IssuerID != 'null' && parseInt($scope.Case.IssuerID) != 'NaN')) {
									data.IssuerId = "0";
								}
								if (!($scope.Case.PlanID && $scope.Case.PlanID != 'null' && parseInt($scope.Case.PlanID) != 'NaN')) {
									data.PlanID = "0";
								}
								if (!($scope.Case.TaxRate && $scope.Case.TaxRate != 'null' && parseInt($scope.Case.TaxRate) != 'NaN')) {
									data.TaxRate = "0";
								}
								if (!($scope.Case.HSAFunding && $scope.Case.HSAFunding != 'null' && parseInt($scope.Case.HSAFunding) != 'NaN')) {
									data.HSAPercentage = "0";
								}
								if (!($scope.Case.Year && $scope.Case.Year != 'null' && parseInt($scope.Case.Year) != 'NaN')) {
									data.BusinessYear = "0";
								}
								if (!($scope.Applicant.EmployerId && $scope.Applicant.EmployerId != 'null' && parseInt($scope.Applicant.EmployerId) != 'NaN')) {
									data.EmployerId = "0";
								}
								if (!($scope.Applicant.InsuranceTypeId && $scope.Applicant.InsuranceTypeId != 'null' && parseInt($scope.Applicant.InsuranceTypeId) != 'NaN')) {
									data.InsuranceTypeId = "0";
								}
								/*if(messages.EmployerId != $scope.Applicant.EmployerId){
									data.HSAPercentage="0";	
								}*/

								data.ZipCode = $scope.Case.ZipCode;
								data.Income = $scope.Case.MAGIncome;
								data.IsAmericanIndian = $scope.Applicant.Origin;
								data.UsageCode = $scope.Case.UsageID;
								data.Welness = $scope.Case.Welness;

								data.CountyName = $scope.Case.CountyName;
								data.SubsidyStatus = $scope.Case.IsSubsidy;

								data.TierIntention = +$scope.Case.TierIntention;
								data.DedBalAvailToRollOver = $scope.Case.DedBalAvailToRollOver;
								data.DedBalAvailDate = $scope.Case.DedBalAvailDate;

								JobService.calculatePlans(data, $scope.Case, $scope.customer.id, function (response) {
									if (response.Status) {
										$scope.GeneratedOnly = $scope.GeneratedOnly + 1;
										callServiceForEachItem('NoSend', CaseStatusIds);
									} else {
										$scope.caseLoadingText = '';
										$scope.dataLoading = false;
										$scope.caseLoading = false;
										$("#print_wizard").hide();
										$scope.GenError = $scope.GenError + 1;
										callServiceForEachItem('NoSend', CaseStatusIds);
									}
								});
							}
							else {
								console.log('Generate-Send')
								var data = {};
								$scope.Applicant = {};
								data.HSAPercentage = $scope.Case.HSAFunding;
								data.TaxRate = $scope.Case.TaxRate;
								data.IssuerId = $scope.Case.IssuerID;
								data.PlanID = $scope.Case.PlanID;
								$scope.Applicant = $scope.Case.Applicant;
								data.EmployerId = $scope.Applicant.EmployerId;
								// data.InsuranceTypeId = $scope.Applicant.InsuranceTypeId;
								data.BusinessYear = $scope.Case.Year;
								data.JobNumber = $scope.jobNo;


								if (!($scope.Case.IssuerID && $scope.Case.IssuerID != 'null' && parseInt($scope.Case.IssuerID) != 'NaN')) {
									data.IssuerId = "0";
								}
								if (!($scope.Case.PlanID && $scope.Case.PlanID != 'null' && parseInt($scope.Case.PlanID) != 'NaN')) {
									data.PlanID = "0";
								}
								if (!($scope.Case.TaxRate && $scope.Case.TaxRate != 'null' && parseInt($scope.Case.TaxRate) != 'NaN')) {
									data.TaxRate = "0";
								}
								if (!($scope.Case.HSAFunding && $scope.Case.HSAFunding != 'null' && parseInt($scope.Case.HSAFunding) != 'NaN')) {
									data.HSAPercentage = "0";
								}
								if (!($scope.Case.Year && $scope.Case.Year != 'null' && parseInt($scope.Case.Year) != 'NaN')) {
									data.BusinessYear = "0";
								}
								if (!($scope.Applicant.EmployerId && $scope.Applicant.EmployerId != 'null' && parseInt($scope.Applicant.EmployerId) != 'NaN')) {
									data.EmployerId = "0";
								}
								if (!($scope.Applicant.InsuranceTypeId && $scope.Applicant.InsuranceTypeId != 'null' && parseInt($scope.Applicant.InsuranceTypeId) != 'NaN')) {
									data.InsuranceTypeId = "0";
								}
								/*if(messages.EmployerId != $scope.Applicant.EmployerId){
									data.HSAPercentage="0";	
								}*/

								data.ZipCode = $scope.Case.ZipCode;
								data.Income = $scope.Case.MAGIncome;
								data.IsAmericanIndian = $scope.Applicant.Origin;
								data.UsageCode = $scope.Case.UsageID;
								data.Welness = $scope.Case.Welness;

								data.CountyName = $scope.Case.CountyName;
								data.SubsidyStatus = $scope.Case.IsSubsidy;

								data.TierIntention = +$scope.Case.TierIntention;
								data.DedBalAvailToRollOver = $scope.Case.DedBalAvailToRollOver;
								data.DedBalAvailDate = $scope.Case.DedBalAvailDate;

								var found = {};

								$scope.UsageType = '';
								found = $filter('filter')($scope.usageCode, function (d) { return d.UsagaId == data.UsageCode; })[0];
								if (!isEmpty(found)) {
									$scope.UsageType = found.UsageType;
								}

								$scope.InsuranceName = '';
								found = $filter('filter')($scope.insuranceTypes, function (d) { return d.InsuranceTypeId == data.InsuranceTypeId; })[0];
								if (!isEmpty(found)) {
									$scope.InsuranceName = found.InsuranceType1;
								}

								$scope.PlanType = '';
								found = $filter('filter')($scope.plans, function (d) { return d.PlanID == data.PlanID; })[0];
								if (!isEmpty(found)) {
									$scope.PlanType = found.PlanType;
								}

								JobService.calculatePlans(data, $scope.Case, $scope.customer.id, function (response) {
									if (response.Status) {
										$scope.GeneratedOnly = $scope.GeneratedOnly + 1;
										$scope.graphResults = response.Plans;
	
								var MaxWorstCase = Math.max.apply(Math, $scope.graphResults.map(function (item) {
									return item.WorstCase;
								})) + 3000;

								var MaxTotalCost = Math.max.apply(Math, $scope.graphResults.map(function (item) {
									return item.NetAnnualPremium + item.Medical
								})) + 3000;

								$scope.max = MaxTotalCost > MaxWorstCase ? MaxTotalCost : MaxWorstCase;

								if($scope.graphResults.length === 1)
								{		
									$scope.pro_red = '70%';
									$scope.pro_green = '30%';
								}
								else if($scope.graphResults.length === 2)
								{		
									$scope.pro_red = '50%';
									$scope.pro_green = '25%';
								}
								else if($scope.graphResults.length === 3)
								{		
									$scope.pro_red = '40%';
									$scope.pro_green = '20%';
								}
								else if($scope.graphResults.length === 4)
								{		
									$scope.pro_red = '30%';
									$scope.pro_green = '17.5%';
								}

								$scope.CurrentPlanName = $scope.Applicant.CurrentPlan;
								if ($scope.Case.Applicant.CurrentPlan != '' && $scope.Case.Applicant.CurrentPlan != 'Waived' && $scope.Case.Applicant.CurrentPlan != 'NewHire' && $scope.graphResults.length > 0) {
									$scope.CurrentPlan = $scope.Case.Applicant.CurrentPlan;
									var CurrentPlanResult = $filter('filter')($scope.graphResults, function (item) {
										return item.PlanId == $scope.CurrentPlan;
									});
									if (CurrentPlanResult.length > 0){
											$scope.CurrentPlanName = CurrentPlanResult[0].PlanName;
											$scope.DifferenceAmount = CurrentPlanResult[0].TotalPaid - $scope.Case.CasePlanResults[0].TotalPaid;
									}
								}
								else
								{
									$scope.CurrentPlan = '';
								}


								if($scope.CurrentPlanName == 'Waived')
								{
									$scope.CurrentPlanName = 'n/a (Waived)';
								}
								if($scope.CurrentPlanName == 'NewHire')
								{
									$scope.CurrentPlanName = 'n/a (New Hire)';
								}



									$scope.data.post = [];

									angular.forEach($scope.graphResults, function (e) {
										$scope.data.post.push({ "State": e.PlanName, "Rank": e.Rank, "Your Net Annual Premium  Contribution": parseFloat(e.NetAnnualPremium), "Your Net Out-of-Pocket Medical Costs": parseFloat(e.Medical) });
									});

									$scope.data.width = ($scope.graphResults.length * 184) + ($scope.graphResults.length * 32.5);
									$("#holderHtml").find('d3-bars').html('')
									$scope.Case.CreatedDateTime = new Date();

									$("#print_wizard").show();
									console.log('$scope.data.width', $scope.data.width)

									//$rootScope.updateGraph($("#holderHtml").find('d3-bars'), true, function () {

										var PlanTotalCostRange = 0;
										var EmployerHSAContribution = 0;
										var EmployerPremiumContribution = 0;
										var EmployerHRAReimbursement = 0;
										var TotalEmployerContribution = 0;
										var OptimalPlanName = '';

										if ($scope.graphResults.length > 0) {

											PlanTotalCostRange = $scope.graphResults[$scope.graphResults.length - 1].TotalPaid - $scope.graphResults[0].TotalPaid;
											EmployerHSAContribution = $scope.graphResults[0].ContributedToYourHSAAccount;
											EmployerPremiumContribution = $scope.graphResults[0].FederalSubsidy;
											EmployerHRAReimbursement = $scope.graphResults[0].HRAReimbursedAmt;
											TotalEmployerContribution = EmployerHSAContribution + EmployerPremiumContribution + EmployerHRAReimbursement;
											OptimalPlanName = $scope.graphResults[0].PlanName;
										}

										$timeout(function () {

											$("#print_wizard").hide();

											var Html = btoa(unescape(encodeURIComponent('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml"><body style="font-family: calibri !important;  font-size: 15px !important; color: rgb(51, 51, 51) !important;  margin: 0px auto !important;padding: 10px !important; ">' + $("#print_wizard").html() + '</body></html>')));

											var xhr = new XMLHttpRequest();
											xhr.open('post', messages.serverLiveHost + messages.sendMailwithUpdate, true);
											xhr.setRequestHeader('Content-type', 'application/json');
											xhr.send(JSON.stringify({ 
												ApplicantEmail: $scope.Applicant.Email, 
												AgentEmail: $scope.customer.Email, 
												CaseTitle: $scope.Case.CaseTitle, 
												ApplicantName: $scope.Applicant.FirstName, 
												AgentName: $scope.customer.FirstName, 
												AgentPhone: $scope.customer.Phone, 
												Html: Html, 
												JobNumber: $scope.Case.JobNumber, 
												CaseId: $scope.Case.CaseID, 
												ModifiedBy: $scope.customer.id, 
												PlanTotalCostRange: PlanTotalCostRange,
												EmployerHSAContribution : EmployerHSAContribution,
												EmployerPremiumContribution : EmployerPremiumContribution,
												EmployerHRAReimbursement : EmployerHRAReimbursement,
												TotalEmployerContribution : TotalEmployerContribution,
												OptimalPlanName : OptimalPlanName
											}));

											xhr.onreadystatechange = function () {

												if (xhr.readyState == 4) {
													if (xhr.status == 200) {

														$scope.data = {};
														$scope.data.post = [];
														console.log(xhr.statusText)
														$scope.ReportSent = $scope.ReportSent + 1;
														callServiceForEachItem('Send', CaseStatusIds);

													} else if (xhr.status === 400) {
														console.log(xhr.responseText)
														$scope.SendError = $scope.SendError + 1;
														callServiceForEachItem('Send', CaseStatusIds);
													}
													else {
														console.log("Error", xhr.statusText);
														$scope.SendError = $scope.SendError + 1;
														callServiceForEachItem('Send', CaseStatusIds);
													}
												}
											}
										}, 3000);

									//})



									} else {
										$scope.caseLoadingText = '';
										$scope.dataLoading = false;
										$scope.caseLoading = false;
										$("#print_wizard").hide();
										$scope.GenError = $scope.GenError + 1;
										callServiceForEachItem('Send', CaseStatusIds);
									}
								});
							}
						} else {
							$scope.caseLoadingText = '';
							$scope.dataLoading = false;
							$scope.caseLoading = false;
							var PopUpMessage = 'Job execution completed.<br>GeneratedOnly Cases : ' + $scope.GeneratedOnly;
							if (action != 'NoSend') { PopUpMessage = PopUpMessage + '<br>ReportSent Cases : ' + $scope.ReportSent }
							if ($scope.GenError != 0) {
								PopUpMessage = PopUpMessage + '<br>GenError Cases : ' + $scope.GenError;
							}
							if ($scope.SendError != 0) {
								PopUpMessage = PopUpMessage + '<br>SendError Cases : ' + $scope.SendError;
							}
							bootbox.alert(PopUpMessage);

							$("#print_wizard").hide();
						}
					});
				}

				$scope.getInsuranceType = function (InsuranceType) {
					if (InsuranceType != 0 && $scope.job.EmployerId != messages.IndividualEmployerId) { $scope.job.InsuranceTypeId = InsuranceType; } else { $scope.job.InsuranceTypeId = ''; }
					if (isEmpty($scope.job.EmployerId)) { console.log('temp'); $scope.job.EmployerId = ''; }
					$scope.InsuranceTypeList = [];
					if (!isEmpty($scope.job.EmployerId) && !isEmpty($scope.job.JobYear) && $scope.job.EmployerId != messages.IndividualEmployerId && $scope.job.EmployerId != messages.ShopEmployerId) {
						$scope.dataLoading = true;
						$scope.requestInProgress = true;
						JobService.getInsuranceType($scope.job.EmployerId, $scope.job.JobYear, function (response) {
							if (response.Status && response.InsuranceTypes.length > 0) {
								$scope.InsuranceTypeList = response.InsuranceTypes;

								$scope.dataLoading = false;
								$scope.requestInProgress = false;
							} else {
								$scope.dataLoading = false;
								$scope.requestInProgress = false;
							}
						})
					} else {
						$scope.dataLoading = false;
					}
				}

				$scope.backNotify = function () {

					bootbox.dialog({
						message: messages.confirmationJobSaveMsg,
						title: "",
						buttons: {
							success: {
								label: "Yes",
								className: "btn-success",
								callback: function () {
									if ($("#createJobForm").valid()) {
										$scope.IsBackButton = true;
										$scope.addJob(true, function () {
											$scope.formLoading = false;
											$location.path('masters/job');
											$scope.$apply();
										});
									} else {
										$scope.formLoading = false;
										$scope.$apply();
										if (typeof callback != 'undefined' && typeof (callback) === "function") {
											callback();
										}
									}
								}
							},
							danger: {
								label: "No",
								className: "btn-danger",
								callback: function () {
									$scope.formLoading = false;
									$location.path('masters/job');
									$scope.$apply();
								}
							}
						}
					});

				}


				/** Import census data */
				$scope.selectedFile = null;
				$scope.msg = "";

				$scope.loadFile = function (files) {
					console.log('file1')

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

							//						var data = e.target.result;  

							var workbook = XLSX.read(data, { type: 'binary' });

							var first_sheet_name = workbook.SheetNames[0];

							var dataObjects = XLSX.utils.sheet_to_json(workbook.Sheets[first_sheet_name]);

							if (dataObjects.length > 0) {

								$scope.$apply(function () {
									$scope.CensusData = dataObjects;
								});

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

				$scope.ImportCensusCases = function () {

					if ($scope.CensusData.length <= 0) {
						bootbox.alert('Please select census file', function () {
							$scope.$apply();

						});
						return;
					}

					if (typeof $scope.job.OldJobNumber != 'undefined') {
						$scope.dataLoading = true;
						var JobData = {};
						JobData.EmployerId = $scope.job.EmployerId;
						JobData.JobNo = $scope.job.JobNumber;
						JobData.FileName = $scope.selectedFile.name;
						JobService.ImportCaseCensus($scope.CensusData, JobData, function (response) {
							if (response.Status) {
								bootbox.alert(response.Message, function () {
									$scope.$apply();
								});

							} else {
								bootbox.alert(messages.TryLater, function () {
									$location.path('/');
									$scope.$apply();
								})
							}
							$scope.flash = {};
							$scope.flash.status = false;
							$scope.dataLoading = false;
							$scope.formLoading = false;
							$("html").animate({ scrollTop: 0 }, 500);
						});
					}
					else {
						bootbox.alert('Please save job to import census data', function () {
							$scope.$apply();
						});
					}
				}

				$scope.changeHsaMatchFlag = function () {
					if ($scope.job.IsHSAMatch)
						$scope.isHSAMatchFlag = true;
					else
						$scope.isHSAMatchFlag = false;
				}

				// Create custom validation
				jQuery.validator.addMethod("alphanumeric", function (value, element) {
					return this.optional(element) || /^[(a-z)(0-9)\s]+$/i.test(value);
				}, "Please enter alphanumeric value.");

				$scope.$on('$includeContentLoaded', function (event) {
					$("#print_wizard").hide();

					$("#createJobForm").validate({
						ignore: "",
						errorElement: 'span',
						errorClass: 'error',
						onkeyup: false,
						rules: {
							employerId: {
								required: true,
							},
							year1: {
								required: true,
							},
							insuranceType: {
								required: true,
							},
							zipCode: {
								required: true,
								maxlength: 5,
							},
							jobNo: {
								required: true,
								maxlength: 18,
							},
							jobDescription: {
								required: true,
								maxlength: 150,
							},
							jobYear: {
								required: true,
							},
							jobStatus: {
								required: true,
								alphanumeric: true,
								maxlength: 18,
							},
							jobStartDate: {
								required: true,
							},
							jobEndDate: {
								required: true,
							},
							jobHRAMaxReimburseDependent: {
								maxlength: 10,
							},
							jobHRADedLimitDependent: {
								maxlength: 10,
							},
							jobHRAReimburseRatePrimary: {
								maxlength: 10,
							},
							jobHRAMaxReimbursePrimary: {
								maxlength: 10,
							},
							jobHRADedLimitPrimary: {
								maxlength: 10,
							},
							jobHRAReimburseRateDependent: {
								maxlength: 10,
							},
							acctMgr: {
								maxlength: 10,
							},
							jobHSAMatchLimit1: {
								required: function () {
									return $('#jobHSAMatchFlag:checked');
								}
							},
							jobHSAMatchRate1: {
								required: function () {
									return $('#jobHSAMatchFlag:checked');
								}
							},
							jobHSAMatchLimit2: {
								required: function () {
									return $('#jobHSAMatchFlag:checked');
								}
							},
							jobHSAMatchRate2: {
								required: function () {
									return $('#jobHSAMatchFlag:checked');
								}
							},
							jobHSAMatchLimit3: {
								required: function () {
									return $('#jobHSAMatchFlag:checked');
								}
							},
							jobHSAMatchRate3: {
								required: function () {
									return $('#jobHSAMatchFlag:checked');
								}
							},
							jobHSAMatchLimit4: {
								required: function () {
									return $('#jobHSAMatchFlag:checked');
								}
							},
							jobHSAMatchRate4: {
								required: function () {
									return $('#jobHSAMatchFlag:checked');
								}
							},

							// comparisonJobNum: {
							// 	required: true,
							// },
							// jobCopiedFrom: {
							// 	required: true,
							// },
							// acctMgr: {
							// 	required: true,
							// },
							// jobExpectedCompletionDt: {
							// 	required: true,
							// },
							// jobPlanYearStartDate: {
							// 	required: true,
							// },
							// jobPlanYearEndDate: {
							// 	required: true,
							// },
							// jobPlansSelectionLockedDate: {
							// 	required: true,
							// },
							// jobPlansSelectionLocked: {
							// 	required: true,
							// },
							// jobCensusImportDate: {
							// 	required: true,
							// },
							// jobHRAMaxReimbursePrimary: {
							// 	required: true,
							// },
							// jobHRAMaxReimburseDependent: {
							// 	required: true,
							// },
							// jobHRADedLimitPrimary: {
							// 	required: true,
							// },
							// jobHRADedLimitDependent: {
							// 	required: true,
							// },
							// jobHRACanCoverPremium: {
							// 	required: true,
							// },
							// jobHRAReimburseRatePrimary: {
							// 	required: true,
							// },
							// jobHRAReimburseRateDependent: {
							// 	required: true,
							// },
							subText: {
								required: true,
								maxlength: 500,
							},
							bodyText: {
								required: true,
							},
							signText: {
								required: true,
							},
						}
					});
				});

				$scope.userSelectedPlans = [];

				$scope.getPlanInfo = function (type) {
					var data = { jobNo: $scope.jobNo, data: $scope.planFiltersData };
					$scope.dataLoading = true;
					JobService.GetJobPlans(data, function (response) {
						if (response != null) {
							if (response.Status) {
								console.log("=============", response);
								if (type == 1) {
									$scope.allPlans = response.NotSelectedPlans;
								}
								else {
									$scope.allPlans = response.SelectedPlans;
									$scope.allPlans.forEach(function (entry) {
										$scope.userSelectedPlans.push({ "Id": entry.Id, "SelectedPlanTitle": entry.SelectedPlanTitle, "selected": false });
									});
								}
							}
						}
						$scope.dataLoading = false;
					});
				}

				$scope.AddPlans = function () {

					if ($scope.action == 'add') {
						bootbox.alert('Save job first to update plans', function () {
							$scope.$apply();
						});
						return;
					}

					// if ($scope.job.CurrentJobStatus == "Open" || $scope.job.CurrentJobStatus == "Cancelled" || $scope.job.CurrentJobStatus == "Billed" || $scope.job.CurrentJobStatus == "Closed") {
					// 	bootbox.alert('This Job is ' + $scope.job.CurrentJobStatus + ' and cannot be edited', function () {
					// 		$scope.$apply();
					// 	});
					// 	return;
					// }

					var SelectedPlanIds = [];
					$scope.userSelectedPlans.forEach(function (entry) {
						SelectedPlanIds.push(entry.Id);
					});

					if ($scope.job.JobStatus === 'New' && $scope.job.JobPlansSelectionLocked == 'true') {
						bootbox.alert('Plan Selection for this Job is LOCKED', function () {
							$scope.$apply();
						});
						return;
					}

					console.log("add user plan", $scope.userSelectedPlans);
					var data = { JobNumber: $scope.jobNo, SelectedPlanIds: SelectedPlanIds };

					JobService.AddJobPlans(data, function (response) {
						if (response.Status) {
							bootbox.alert(messages.saved, function () {
								// $location.path('masters/job');
								$scope.$apply();
							});
						} else {
							if (response.redirect) {
								bootbox.alert(messages.TryLater, function () {
									$location.path('/');
									$scope.$apply();
								})
							}
						}
					});
				}

				/*$scope.roles = [
					{ roleId: 1, roleName: "Administrator", roleDescription: "Can do a bunch of stuff" },
					{ roleId: 2, roleName: "Super User", roleDescription: "Ultimate power!" }];
	
				$scope.user = { roles: [$scope.allPlans] };*/

				$scope.selectConfig = { requiredMin: 1 };

				$scope.resetFilters = function () {
					$scope.planFiltersData.marketCoverage = '';
					$scope.planFiltersData.groupName = '';
					$scope.planFiltersData.businessYear = '';
					$scope.planFiltersData.planType = '';
					$scope.planFiltersData.stateCode = '';
					$scope.planFiltersData.issuerId = '';
					$scope.allPlans = [];
				};
			}
		]);

/** End : Job Mstr Create Controller. **/