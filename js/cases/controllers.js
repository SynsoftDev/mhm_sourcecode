		angular.module('mhmApp.cases')

			/**
				Case Controller List all cases.
			*/
			.controller('CasesController', ['$scope', '$rootScope', '$location', 'CaseService', 'checkCreds', 'businessServices', 'messages', '$filter', '$window', '$http',
				function ($scope, $rootScope, $location, CaseService, checkCreds, businessServices, messages, $filter, $window, $http) {
					// reset login status		
					$scope.currentPage = 1;
					$scope.pageSize = 10;
					$scope.dataLoading = false;
					$scope.breadcrumb = true;
					$scope.searchby = '';
					$scope.TempSearch = {};
					$scope.TempSearch.start_date = '';
					$scope.start_date = '';
					$scope.searchByEmployer = '';
					$scope.searchByCaseTitle = '';
					$scope.searchByAgent = '';
					$scope.searchByPhone = '';
					$scope.searchByJobNo = '';
					$scope.searchByStatusCode = '';
					$scope.searchByBusinessYear = '';
					$scope.searchByAnalytics = '';
					$scope.searchByAlternate = '';
					$scope.end_date = '';
					$scope.TempSearch.end_date = new Date();
					$scope.sortby = 'CreatedDateTime';
					$scope.desc = true;
					$scope.page = 1;
					$scope.cases = [];
					$scope.TempSearch.searchByStatusCode = [];
					$scope.statusCodesettings = {
						displayProp: 'StatusCode',
						idProp: 'StatusCode'
					};
					$scope.businessYears = [];
					$scope.pageSize = messages.pageSize;
					$scope.TotalCount = 0;
					$scope.lastCount = 0;
					$scope.pageSizes = [10, 20, 50, 100];
					$scope.messages = messages;
					$scope.ActiveMenu = "activemenu";
					$scope.sessionPage = "CaseSession";
					$scope.searchSession = '';
					$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li class="active">Cases</li>';
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
					/**
						Initial services load all required list values. (statusCode)
					*/
					CaseService.waitForLayoutCases(function (response) {
						$scope.statusCode = ((typeof response[0].data != 'undefined') && (response[0].data.Status)) ? response[0].data.CaseStatusMst : [];
						//var indexss = $scope.statusCode.findIndex(x => x.StatusId === 11);
						$scope.statusCode.splice(3, 1);
						$scope.employerCompanies = ((typeof response[1].data != 'undefined') && (response[1].data.Status)) ? response[1].data.EmployerCompanies : [];
						$scope.jobNumbers = ((typeof response[2].data != 'undefined') && (response[2].data.Status)) ? response[2].data.JobNumbers : [];
						$scope.agentsList2 = ((typeof response[3].data != 'undefined') && (response[3].data.Status)) ? response[3].data.AgentsList : [];
						$scope.agentsList1 = [];
						$.each($scope.agentsList2, function (i, el) {
							if ($.inArray(el.Name, $scope.agentsList1) === -1) $scope.agentsList1.push(el.Name);
						});

						$rootScope.pageLoading = false;

						if ($scope.searchSession != '')
							$scope.setCaseList();

						$scope.getCaseList();
					});

					for (var i = 2015; i < 2045; i++) {
						$scope.businessYears.push({
							"val": i
						});
					}

					$scope.startDate = {};
					$scope.endDate = {};
					$scope.startDate.isOpned = false;
					$scope.endDate.isOpned = false;

					$scope.open_start = function () {
						$scope.startDate.isOpned = true;
					};
					$scope.open_end = function ($event) {
						$event.preventDefault();
						$event.stopPropagation();
						$scope.endDate.isOpned = true;
					};
					$scope.$watch('TempSearch.end_date', function () {
						if ($scope.TempSearch.end_date != null) {
							$scope.maxStartDate = angular.copy($scope.TempSearch.end_date);
						} else {
							$scope.maxStartDate = '';
						}
					}, true);
					$scope.$watch('TempSearch.start_date', function () {
						$scope.minEndDate = angular.copy($scope.TempSearch.start_date);
					}, true);


					$scope.dateOptions = {
						'show-weeks': false
					};
					$scope.format = 'MM/dd/yyyy';
					$scope.altInputFormats = ['M!/d!/yyyy'];
					$scope.popup1 = {
						opened: false
					};
					/**
						Set Case List parameter and call API.
					*/
					$scope.setCaseList = function () {
						//:- Applicant Name, Case title, Agent, Created date, Applicant Phone and Email
						var start_dateF = $scope.TempSearch.start_date;
						if ($scope.TempSearch.start_date) {
							start_dateF = $filter('date')(new Date(start_dateF), 'yyyy-MM-dd') + ' 00:00:00';
						}
						var end_dateF = $scope.TempSearch.end_date;
						if ($scope.TempSearch.end_date) {
							end_dateF = $filter('date')(new Date(end_dateF), 'yyyy-MM-dd') + ' 23:59:59';
						}
						$rootScope.pageLoading = false;
						$scope.dataLoading = true;
						$scope.searchby = $scope.TempSearch.searchby;
						$scope.searchByEmployer = $scope.TempSearch.searchByEmployer;
						$scope.searchByCaseTitle = $scope.TempSearch.searchByCaseTitle;
						$scope.searchByAgent = $scope.TempSearch.searchByAgent;
						$scope.searchByPhone = $scope.TempSearch.searchByPhone;
						$scope.searchByJobNo = $scope.TempSearch.searchByJobNo;
						$scope.searchByStatusCode = $scope.TempSearch.searchByStatusCode;
						$scope.searchByBusinessYear = $scope.TempSearch.searchByBusinessYear;
						$scope.searchByAnalytics = $scope.TempSearch.searchByAnalytics;
						$scope.searchByAlternate = $scope.TempSearch.searchByAlternate;
						$scope.end_date = end_dateF;
						$scope.start_date = start_dateF;
						$scope.page = 1;
						$scope.searchSession = businessServices.setSearchSession($scope.sessionPage, $scope.TempSearch);
						$scope.getCaseList();
					}
					/**
						ReSet Case List parameter and call API.
					*/
					$scope.resetCaseList = function () {
						//:- Applicant Name, Case title, Agent, Created date, Applicant Phone and Email
						$rootScope.pageLoading = false;
						$scope.dataLoading = true;
						$scope.searchby = $scope.TempSearch.searchby = '';
						$scope.searchByEmployer = $scope.TempSearch.searchByEmployer = '';
						$scope.searchByCaseTitle = $scope.TempSearch.searchByCaseTitle = '';
						$scope.searchByAgent = $scope.TempSearch.searchByAgent = '';
						$scope.searchByPhone = $scope.TempSearch.searchByPhone = '';
						$scope.searchByJobNo = $scope.TempSearch.searchByJobNo = '';
						$scope.searchByStatusCode = $scope.TempSearch.searchByStatusCode = [];
						$scope.searchByBusinessYear = $scope.TempSearch.searchByBusinessYear = '';
						$scope.searchByAnalytics = $scope.TempSearch.searchByAnalytics = '';
						$scope.searchByAlternate = $scope.TempSearch.searchByAlternate = '';
						$scope.end_date = $scope.TempSearch.end_date = new Date();
						$scope.start_date = $scope.TempSearch.start_date = '';
						$scope.page = 1;
						businessServices.resetSearchSession($scope.sessionPage);
						$scope.getCaseList();
					}


					$scope.maxDate = new Date();
					/**
						get Case List parameter and call API.
					*/
					$scope.getCaseList = function () {
						$rootScope.pageLoading = false;
						$scope.dataLoading = true;
						CaseService.getAll($scope, function (response) {
							if (response.Status) {
								$scope.cases = response.Cases;
								$scope.TotalCount = response.TotalCount;
								$scope.lastCount = Math.ceil($scope.TotalCount / $scope.pageSize);
								$rootScope.pageLoading = false;
								$scope.dataLoading = false;
							} else {
								if (response.redirect) {
									bootbox.alert(messages.TryLater + 'Fetching list', function () {
										// $location.path('/');
										$scope.$apply();
									})
								}
								if (typeof response.Message != 'undefined') {
									bootbox.alert(response.Message);
									$scope.dataLoading = false;
								}
								$scope.cases = [];
							}
						});
					}

					/**
						update case Opt and Alternates
					*/
					$scope.updateCaseOpt = function (caseid, item) {
						$rootScope.pageLoading = false;
						$scope.dataLoading = true;
						var data = {};
						data.CaseId = caseid;
						data.Status = item.PrimaryCase;
						CaseService.updateCaseOpt(data, function (response) {
							if (response.Status) {
								$rootScope.pageLoading = false;
								$scope.dataLoading = false;
							} else {
									bootbox.alert('Unable to update case.');
									item.PrimaryCase = item.PrimaryCase ? false : true;
									$scope.dataLoading = false;
							}
						});
					}
					$scope.updateCaseAlternates = function (caseid, item) {
						$rootScope.pageLoading = false;
						$scope.dataLoading = true;
						var data = {};
						data.CaseId = caseid;
						data.Status = item.AlternateCase;
						CaseService.UpdateCaseAlternate(data, function (response) {
							if (response.Status) {
								$rootScope.pageLoading = false;
								$scope.dataLoading = false;
							} else {
									bootbox.alert('Unable to update case.');
									item.AlternateCase = item.AlternateCase ? false : true;
									$scope.dataLoading = false;
							}
						});
					}
					/**
						Export Case List parameter and call API.
					*/
					/*$scope.exportReport=function(){				
						$scope.dataLoading=true;			
						CaseService.getCaseReport($scope,function (response,status,xhr) {			
							if (status === 200) {
									var filename = 'Report.xlsx';
									var a = document.createElement('a');
									a.href = window.URL.createObjectURL(xhr.response); // xhr.response is a blob
									a.download = filename; // Set the file name.
									a.style.display = 'none';
									document.body.appendChild(a);
									a.click();
									delete a;				
									$scope.dataLoading = false;	
									$scope.$apply();			
							}else{
								$scope.dataLoading = false;	
								$scope.$apply();
							}				
						});	
					}*/

					$scope.exportReport = function () {
						$scope.dataLoading = true;
						CaseService.getCaseReport($scope, function (responseStatus, response, xhr) {
							if (responseStatus === 200) {
								var filename = 'Report.xlsx';
								var disposition = xhr.getResponseHeader('Content-Disposition');
								if (disposition && disposition.indexOf('attachment') !== -1) {
									var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
									var matches = filenameRegex.exec(disposition);
								}
								var type = xhr.getResponseHeader('Content-Type');

								var blob = new Blob([response], {
									type: type
								});
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
									setTimeout(function () {
										URL.revokeObjectURL(downloadUrl);
									}, 100); // cleanup
									$scope.dataLoading = false;
									$scope.$apply();
								}
							} else {
								$scope.dataLoading = false;
								$scope.$apply();
							}
						});
					}


					/**
						Change sorting order.
					*/
					$scope.sort_by = function (newSortingOrder) {
						if ($scope.sortby == newSortingOrder)
							$scope.desc = !$scope.desc;
						else
							$scope.desc = !$scope.desc;
						$scope.sortby = newSortingOrder;
						$scope.page = 1;
						$scope.getCaseList();
					};
					/**
						Call pagination function.
					*/
					$scope.range = function () {
						if (!$scope.maxSize) {
							$scope.maxSize = 9;
						}
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
							$scope.getCaseList();
						}
					};

					$scope.nextPage = function () {
						if ($scope.page < $scope.TotalCount - 1) {
							$scope.page++;
							$scope.getCaseList();
						}
					};
					$scope.prevPage = function () {
						if ($scope.page > 1) {
							$scope.page--;
							$scope.getCaseList();
						}
					};
					$scope.openPopUP = function () {
						if ($(".adv_srch_btn_box").css('display') == 'none') {
							$(".adv_srch_btn_box").show();
						} else {
							$(".adv_srch_btn_box").hide();
						}
					}

					$scope.$on('$includeContentLoaded', function (event) {
						$(document).click(function (e) {
							if (!$(e.target).hasClass("adv_srch_btn_search") &&
								$(e.target).parents(".adv_srch_btn_box").length === 0 && $(e.target).parents(".uib-datepicker-popup").length === 0) {
								$(".adv_srch_btn_box").hide();
							}
						});

						var idleState = false;
						var idleTimer = null;
						$('*').bind('mousemove click mouseup mousedown keydown keypress keyup submit change mouseenter scroll resize dblclick', function () {
							clearTimeout(idleTimer);
							idleState = false;
							idleTimer = setTimeout(function () {
								$location.path('/logout').replace();
								$scope.$apply();
								idleState = true;
							}, messages.sessionTimeout);
						});
						$("body").trigger("mousemove");

					});
				}
			])

			.controller('PDFGenerator', ['$scope', '$rootScope', '$routeParams', '$location', 'CaseService', 'checkCreds', 'businessServices', 'messages', '$filter', '$window', '$http', '$timeout',
				function ($scope, $rootScope, $routeParams, $location, CaseService, checkCreds, businessServices, messages, $filter, $window, $http, $timeout) {
					$scope.Case = {};
					$scope.Case.Year = '';
					$scope.Applicant = {};
					$scope.graphResults = {};
					$scope.data = {};
					$scope.data.post = [];
					CaseService.getCase($routeParams.id, function (response) {
						if (response.Status) {
							$scope.dataLoading = false;
							$rootScope.pageLoading = false;
							$scope.Case = response.Case;
							$scope.caseID = $scope.Case.CaseId;
							$scope.Case.Year = response.Case.Year;
							$scope.Applicant = response.Case.Applicant;
							$scope.graphResults = response.Case.CasePlanResults;

							angular.forEach($scope.graphResults, function (e) {
								// $scope.data.post.push({"State":e.PlanName+" ("+e.Rank+")","Net Annual Premium":parseFloat(e.NetAnnualPremium),"Medical (net of HSA tax savings)":parseFloat(e.Medical)});	
								$scope.data.post.push({
									"State": e.PlanName + '-' + e.PlanIdIndiv1,
									"Rank": e.Rank,
									"Your Net Annual Premium Contribution": parseFloat(e.NetAnnualPremium),
									"Your Net Out-of-Pocket Medical Costs": parseFloat(e.Medical)
								});
							});
							$scope.data.width = ($scope.graphResults.length * 184) + ($scope.graphResults.length * 32.5);
							$scope.Case.CreatedDateTime = new Date();
							$rootScope.updateGraph($("#holderHtml").find('d3-bars'), true);

							$timeout(function () {
								var Html = btoa(unescape(encodeURIComponent('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml"><body style="font-family: calibri !important;  font-size: 15px !important; color: rgb(51, 51, 51) !important;  margin: 0px auto !important;padding: 10px !important; ">' + $("#print_wizard").html() + '</body></html>')));


								var xhr = new XMLHttpRequest();
								xhr.open('post', messages.serverLiveHost + messages.sendMailwithUpdate, true);
								xhr.setRequestHeader('Content-type', 'application/json');

								xhr.send(JSON.stringify({
									ApplicantEmail: $scope.Applicant.Email,
									AgentEmail: 'vchaurasiya4@gmail.com',
									CaseTitle: $scope.Case.CaseTitle,
									ApplicantName: $scope.Applicant.FirstName,
									AgentName: 'Sandeep',
									AgentPhone: '9098070207',
									Html: Html,
									JobNumber: $scope.Case.JobNumber,
									CaseId: $scope.Case.CaseId,
									PlanTotalCostRange: 'wsere'
								}));

								xhr.onreadystatechange = function () {
									/*if (xhr.readyState==4 && xhr.status==200){					 
											 	
									   $scope.formLoading=false;	
									   $scope.$apply();	
									}*/

									if (xhr.readyState == 4) {
										if (xhr.status == 200) {
											$window.close()
											console.log(xhr.statusText)
										} else if (xhr.status === 400) {
											console.log(xhr.responseText)
										} else {
											$window.close()
											console.log("Error", xhr.statusText);
										}

									}
								}
							}, 3000);



						}

					});
				}
			])
			/**
				Get Case detail page.
			*/
			.controller('CreateCaseController', ['$scope', '$rootScope', '$location', 'CaseService', 'checkCreds', 'businessServices', '$routeParams', '$q', '$uibModal', '$uibModalStack', 'messages', '$timeout', '$filter', 'ngDialog', '$http',
				function ($scope, $rootScope, $location, CaseService, checkCreds, businessServices, $routeParams, $q, $modal, $modalStack, messages, $timeout, $filter, ngDialog, $http) {


					// reset login status $routeParams.Blogid
					$scope.closeFlash = function (e) {
						$scope.flash.status = false;
					}
					/**
						Check login credentials.
					*/
					$scope.dataLoading = false;
					if (checkCreds() === true) {
						$scope.loggedIn = true;
						$scope.customer = businessServices.getUsername();
					} else {
						$scope.loggedIn = false;
						$location.path('/');
					}
					$scope.base_url = messages.base_url;
					$scope.title = 'Add Case';
					$scope.action = 'Submit';

					$scope.Formaction = $routeParams.action;

					$scope.readOnly = false;
					var IsChangesOccur = false;

					if ($routeParams.view == 'true') {
						$scope.readOnly = true;
						IsChangesOccur = true;
					}

					$scope.IsDedBalAvlDate = false;

					$scope.errorShow = false;
					$scope.edit = false;
					$scope.showBenefits = false;
					$scope.showNoBenefit = false;
					$scope.showBenefitTest = false;
					$scope.requestInProgress = false;
					$scope.requestInProgressCarrier = false;
					$scope.id = '';
					$scope.CaseReferenceId = '';
					$scope.Applicant = {};
					$scope.stateList = [];
					$scope.criticalillness = [];
					$scope.grossIncome = '';
					$scope.Applicant.EmployerId = '';
					$scope.Applicant.FirstName = '';
					$scope.Applicant.LastName = '';
					$scope.Applicant.Origin = false;
					$scope.Applicant.Street = '';
					$scope.Applicant.City = '';
					$scope.Applicant.State = '';
					$scope.Applicant.Zip = '';
					$scope.Applicant.CountyName = '';
					$scope.Applicant.Mobile = '';
					$scope.Applicant.CurrentPlan = '';
					$scope.Applicant.CurrentPremium = '';
					$scope.Applicant.PreferredLanguage = '';
					$scope.auto_calculate = 0;
					$scope.calculateWatch = ['Case.ZipCode', 'Case.CountyName', 'Case.UsageID', 'Case.PlanID', 'Case.IssuerID', 'Case.TaxRate', 'Case.HSAFunding', 'UseBenefits.users.age', 'UseBenefits.users.dobs', 'UseBenefits.users.smoking', 'UseBenefits.users.benefitsVal'];
					$scope.Applicant.Email = '';




					var d = new Date();
					var curr_year = d.getFullYear();

					$scope.Case = {};
					$scope.Case.CaseID = '';
					$scope.Case.ZipCode = '';
					$scope.Case.CaseTitle = '';
					$scope.Case.Year = '';
					$scope.Case.Notes = '';
					$scope.Case.PreviousYrHSA = '';
					$scope.Case.CreatedDateTime = new Date();
					$scope.Case.CaseReferenceId = '';
					$scope.Case.StateCode = '';

					$scope.Case.MAGIncome = 40000;
					$scope.Case.UsageID = "1";
					$scope.Case.TaxRate = 27;
					$scope.Case.InterviewDate = null;
					$scope.Case.HSAFunding = 0;
					$scope.Case.MonthlySubsidy = "";
					$scope.Case.FPL = "";
					$scope.Case.HSALimit = "";
					$scope.Case.HSAActual = '';
					$scope.Case.HSAAmount = "";
					$scope.Case.SecondLowestPlanId = "";
					$scope.Case.PlanID = "";
					$scope.Case.IssuerID = "";
					$scope.Case.Welness = false;
					$scope.Case.PreviousYrHSA = true;
					$scope.Case.IsSubsidy = true;
					$scope.Case.PrimaryCase = false;
					$scope.Case.AlternateCase = false;
					$scope.Case.CaseJobRunStatus = 'NULL';
					$scope.Case.CaseSource = messages.caseSource;
					$scope.Case.CaseStatus = messages.caseStatus;
					$scope.Case.CaseAction = messages.caseAction;
					$scope.Case.StatusId = messages.StatusId.toString();
					$scope.Case.DedBalAvailToRollOver = 0;
					$scope.Case.TierIntention = "1";
					$scope.CountyList = [];
					$scope.jobNos = [];

					//$scope.Case.HSA_PER='12';	
					$scope.county = '';
					$scope.Case.TotalMedicalUsage = "";
					$scope.PlanType = "";
					$scope.IssuerName = "";
					$scope.UsageType = "";
					$scope.EmployerName = "";
					$scope.CountyName = "";
					$scope.IssuerAbbreviations = "";
					$scope.InsuranceName = "";
					$scope.CurrentPlan = "";
					$scope.CurrentPlanName = "";
					$scope.DifferenceAmount = 0;

					$scope.carrier = [];
					$scope.plan = [];
					$scope.employer = [];
					$scope.years = [];
					$scope.codes = [];
					$scope.totalCount = 1;
					$scope.totalCountClass = "cst_tables_1";
					$scope.totalGridClass = "";
					$scope.altGridClass = "";
					$scope.Defaults = {};
					$scope.Defaults.display = [];
					$scope.count = 1;
					$scope.benefit_note = '';
					$scope.data = {};
					$scope.data.post = [];
					$scope.addNewMemberBtn = false;
					$scope.ShowFinish = true;
					$scope.ShowPrintEmail = true;
					$scope.messages = messages;
					var found = '';
					var found1 = '';
					var foundDisplay = '';
					var foundDefaultDisplay = '';
					var foundCounts = '';
					var foundDob = '';
					var foundGender = '';
					var foundSmoking = '';
					var foundAge = '';
					var foundUseMedicalUsage = '';
					var counts = {};
					var dobs = {};
					var gender = {};
					var smoking = {};
					var age = {};
					var UseMedicalUsage = {};
					var caseID = '';
					$scope.InterviewYears = [];
					$scope.InterviewMonths = [];
					$scope.InterviewDays = [];
					$scope.startDate = {};
					$scope.endDate = {};
					$scope.startDate.isOpned = false;
					$scope.endDate.isOpned = false;
					$scope.InterviewDate = '';

					$scope.Interviewvaliyear = true;
					$scope.Interviewvaliday = true;
					$scope.InterviewvaliMonth = true;
					$scope.SelectedMonth = null;
					$scope.SelectedYear = null;
					$scope.SelectedYear = null;


					// $scope.pro_red = '30%';
					// $scope.pro_green = '17.5%';

					// Interview Date Start
					var numberOfYears = (new Date()).getYear() - 10;
					$scope.InterviewYears = $.map($(Array(numberOfYears)), function (val, i) {
						return i + 2010;
					});
					//$scope.InterviewMonths = $.map($(Array(12)), function (val, i) { return i + 1; });
					$scope.InterviewMonths = [{
						label: "January",
						id: 1
					}, {
						label: "February",
						id: 2
					}, {
						label: "March",
						id: 3
					}, {
						label: "April",
						id: 4
					}, {
						label: "May",
						id: 5
					}, {
						label: "June",
						id: 6
					}, {
						label: "July",
						id: 7
					}, {
						label: "August",
						id: 8
					}, {
						label: "September",
						id: 9
					}, {
						label: "October",
						id: 10
					}, {
						label: "November",
						id: 11
					}, {
						label: "December",
						id: 12
					}];




					$scope.changeMonth = function () {
						if ($scope.SelectedMonth != null && $scope.SelectedMonth != "") {
							var days = getDaysInMonth($scope.SelectedMonth, 0);
							$scope.InterviewDays = $.map($(Array(days)), function (val, i) { return i + 1; });
							$scope.Interviewvaliday = false;
						}
						if ($scope.SelectedMonth == '' && $scope.SelectedDay == undefined && $scope.SelectedYear == undefined) {
							$scope.Interviewvaliyear = true;
							$scope.Interviewvaliday = true;
							$scope.InterviewvaliMonth = true;
							$scope.InterviewDays = [];
							return;
						}
						if (($scope.SelectedMonth == null || $scope.SelectedMonth == "")) {
							$scope.Interviewvaliday = false;
							$scope.InterviewDays = [];
						}

					}

					var getDaysInMonth = function (month, year) {
						return new Date(year, month, 0).getDate();
					};
					$scope.SetCurrentdate = function () {
						$scope.SelectedMonth = (new Date().getMonth() + 1).toString();
						$scope.SelectedYear = new Date().getFullYear();
						var days = getDaysInMonth($scope.SelectedMonth, $scope.SelectedYear);
						$scope.InterviewDays = $.map($(Array(days)), function (val, i) { return i + 1; });
						$scope.SelectedDay = new Date().getDate();
						$scope.InterviewDate = new Date($scope.SelectedYear, parseInt($scope.SelectedMonth-1), $scope.SelectedDay);
						console.log($scope.InterviewDate)
					}
					$scope.checkDay = function () {
						if (($scope.SelectedMonth == '' || $scope.SelectedMonth == null || $scope.SelectedMonth == undefined) && ($scope.SelectedDay == undefined || $scope.SelectedDay == "" || $scope.SelectedDay == null) && ($scope.SelectedYear == undefined || $scope.SelectedYear == null || $scope.SelectedYear == '')) {
							$scope.Interviewvaliyear = true;
							$scope.Interviewvaliday = true;
							$scope.InterviewvaliMonth = true;
							return;
						}
						if ($scope.SelectedDay == undefined || $scope.SelectedDay == "" || $scope.SelectedDay == null) {
							$scope.Interviewvaliday = false;
						}
						else {
							if ($scope.SelectedYear == undefined || $scope.SelectedYear == "" || $scope.SelectedYear == null) {
								$scope.Interviewvaliyear = false;
							}
							$scope.Interviewvaliday = true;
						}
					}

					$scope.checkYear = function () {
						if (($scope.SelectedMonth == '' || $scope.SelectedMonth == null || $scope.SelectedMonth == undefined) && ($scope.SelectedDay == undefined || $scope.SelectedDay == "" || $scope.SelectedDay == null) && ($scope.SelectedYear == undefined || $scope.SelectedYear == null || $scope.SelectedYear == '')) {
							$scope.Interviewvaliyear = true;
							$scope.Interviewvaliday = true;
							$scope.InterviewvaliMonth = true;
							return;
						}
						if ($scope.SelectedYear == undefined || $scope.SelectedYear == "" || $scope.SelectedYear == null) {
							$scope.Interviewvaliyear = false;
						}
						else {
							if ($scope.SelectedDay == undefined || $scope.SelectedDay == "" || $scope.SelectedDay == null) {
								$scope.Interviewvaliday = false;
							}
							$scope.Interviewvaliyear = true;
						}

					}

					/**
						Set by default user array for first.
					*/
					$scope.UseBenefits = {
						"users": {
							"counts": [{
								"val": "Primary",
								"user_id": "user_1",
								"display": "Primary",
								"isDelete": false
							}],
							"dobs": [{
								"val": "01/01/1900",
								"isOpned": false,
								"user_id": "user_1"
							}],
							"gender": [{
								"val": "M",
								"user_id": "user_1"
							}],
							"smoking": [{
								"val": "false",
								"user_id": "user_1"
							}],
							"age": [{
								"val": "",
								"user_id": "user_1"
							}],
							"Criticalillnesses": [{
								"val": [],
								"user_id": "user_1"
							}],
							"UseMedicalUsage": [{
								"val": 0,
								"user_id": "user_1"
							}],
							"benefitsVal": {}
						}
					};
					/**
						Maintain default user array as well for all users (max 8 users) restore in between.
					*/
					$scope.Defaults.UseBenefits = {
						"users": {
							"counts": [{
								"val": "Primary",
								"user_id": "user_1",
								"display": "Primary",
								"isDelete": false,
								"isDisplay": false
							}, {
								"val": "2",
								"user_id": "user_2",
								"display": "spouse",
								"isDelete": false,
								"isDisplay": false
							}, {
								"val": "3",
								"user_id": "user_3",
								"display": "child_1",
								"isDelete": false,
								"isDisplay": false
							}, {
								"val": "4",
								"user_id": "user_4",
								"display": "child_2",
								"isDelete": true,
								"isDisplay": true
							}, {
								"val": "5",
								"user_id": "user_5",
								"display": "child_3",
								"isDelete": true,
								"isDisplay": true
							}, {
								"val": "6",
								"user_id": "user_6",
								"display": "child_4",
								"isDelete": true,
								"isDisplay": true
							}, {
								"val": "7",
								"user_id": "user_7",
								"display": "child_5",
								"isDelete": true,
								"isDisplay": true
							}, {
								"val": "8",
								"user_id": "user_8",
								"display": "child_6",
								"isDelete": true,
								"isDisplay": true
							}, {
								"val": "9",
								"user_id": "user_9",
								"display": "child_7",
								"isDelete": true,
								"isDisplay": true
							}],

							"dobs": [{
								"val": "",
								"isOpned": false,
								"user_id": "user_1"
							}, {
								"val": "",
								"isOpned": false,
								"user_id": "user_2"
							}, {
								"val": "",
								"isOpned": false,
								"user_id": "user_3"
							}, {
								"val": "",
								"isOpned": false,
								"user_id": "user_4"
							}, {
								"val": "",
								"isOpned": true,
								"user_id": "user_5"
							}, {
								"val": "",
								"isOpned": false,
								"user_id": "user_6"
							}, {
								"val": "",
								"isOpned": false,
								"user_id": "user_7"
							}, {
								"val": "",
								"isOpned": false,
								"user_id": "user_8"
							}, {
								"val": "",
								"isOpned": false,
								"user_id": "user_9"
							}],

							"gender": [{
								"val": "",
								"user_id": "user_1"
							}, {
								"val": "",
								"user_id": "user_2"
							}, {
								"val": "",
								"user_id": "user_3"
							}, {
								"val": "",
								"user_id": "user_4"
							}, {
								"val": "",
								"user_id": "user_5"
							}, {
								"val": "",
								"user_id": "user_6"
							}, {
								"val": "",
								"user_id": "user_7"
							}, {
								"val": "",
								"user_id": "user_8"
							}, {
								"val": "",
								"user_id": "user_9"
							}],

							"smoking": [{
								"val": "false",
								"user_id": "user_1"
							}, {
								"val": "false",
								"user_id": "user_2"
							}, {
								"val": "false",
								"user_id": "user_3"
							}, {
								"val": "false",
								"user_id": "user_4"
							}, {
								"val": "false",
								"user_id": "user_5"
							}, {
								"val": "false",
								"user_id": "user_6"
							}, {
								"val": "false",
								"user_id": "user_7"
							}, {
								"val": "false",
								"user_id": "user_8"
							}, {
								"val": "false",
								"user_id": "user_9"
							}],

							"age": [{
								"val": "",
								"user_id": "user_1"
							}, {
								"val": "",
								"user_id": "user_2"
							}, {
								"val": "",
								"user_id": "user_3"
							}, {
								"val": "",
								"user_id": "user_4"
							}, {
								"val": "",
								"user_id": "user_5"
							}, {
								"val": "",
								"user_id": "user_6"
							}, {
								"val": "",
								"user_id": "user_1"
							}, {
								"val": "",
								"user_id": "user_7"
							}, {
								"val": "",
								"user_id": "user_8"
							}, {
								"val": "",
								"user_id": "user_9"
							}],

							"Criticalillnesses": [{
								"val": [],
								"user_id": "user_1"
							}, {
								"val": "",
								"user_id": "user_2"
							}, {
								"val": "",
								"user_id": "user_3"
							}, {
								"val": "",
								"user_id": "user_4"
							}, {
								"val": "",
								"user_id": "user_5"
							}, {
								"val": "",
								"user_id": "user_6"
							}, {
								"val": "",
								"user_id": "user_1"
							}, {
								"val": "",
								"user_id": "user_7"
							}, {
								"val": "",
								"user_id": "user_8"
							}, {
								"val": "",
								"user_id": "user_9"
							}],

							"UseMedicalUsage": [{
								"val": 0,
								"user_id": "user_1"
							}, {
								"val": 0,
								"user_id": "user_2"
							}, {
								"val": 0,
								"user_id": "user_3"
							}, {
								"val": 0,
								"user_id": "user_4"
							}, {
								"val": 0,
								"user_id": "user_5"
							}, {
								"val": 0,
								"user_id": "user_6"
							}, {
								"val": 0,
								"user_id": "user_7"
							}, {
								"val": 0,
								"user_id": "user_8"
							}, {
								"val": 0,
								"user_id": "user_9"
							}],

							"benefitsVal": {}
						}
					};

					$scope.$watch('count', function (newValue) {
						$scope.totalCountClass = 'cst_tables_' + $scope.count;
						$scope.totalGridClass = 'grid_tables_' + $scope.count;
						$scope.altGridClass = 'alt_grid_tables_' + $scope.count;
					});

					/**
						Update Zip code.
					*/
					$scope.updateZipcode = function () {
						if (!$scope.edit && isEmpty($scope.Case.ZipCode)) {
							$scope.Case.ZipCode = $scope.Applicant.Zip;
							if (!isEmpty($scope.Case.ZipCode)) {
								$("#caseZipCode-error").hide();
								$("input[name=caseZipCode]").removeClass("error");
							}
						}
						$scope.getZipCodeState();
					}
					/**
						Call API when usage ID change in case page.
					*/
					$scope.$watch('Case.UsageID', function (newValue) {
						if (!isEmpty($scope.Case.UsageID)) {
							$scope.addNewMemberBtn = true;
							if ($scope.Case.UsageID == "1") {
								$scope.addNewMemberBtn = false;
							} else if ($scope.Case.UsageID == "2") {
								$scope.addNewMemberBtn = false;
							} else if ($scope.Case.UsageID == "5") {
								$scope.addNewMemberBtn = false;
							}
						}
					})

					$scope.getUsageCode = function () {
						$scope.oldUsageCode = $scope.Case.UsageID;
					};
					/**
						Watch user array in anything change in medical usage we can reset grid.
					*/
					$scope.$watch('UseBenefits.users.counts', function (newValue) {
						if (!isEmpty($scope.Case.UsageID)) {
							if ($scope.Case.UsageID == "2") {
								if ($scope.UseBenefits.users.counts.length > 0) {
									var i = 1;
									$.grep($scope.UseBenefits.users.counts, function (e) {
										if (e.user_id == 'user_' + i && (i == 2)) {
											e.isDelete = false;
										}
										i++;
									});
								}
							} else if ($scope.Case.UsageID == "3") {
								if ($scope.UseBenefits.users.counts.length > 0) {
									var i = 1;
									$.grep($scope.UseBenefits.users.counts, function (e) {
										if (e.user_id == 'user_' + i && (i == 2 || i == 3)) {
											e.isDelete = false;
										}
										i++;
									});
								}
							} else if ($scope.Case.UsageID == "4") {
								if ($scope.UseBenefits.users.counts.length > 0) {
									var i = 1;
									$.grep($scope.UseBenefits.users.counts, function (e) {
										if (e.user_id == 'user_' + i && (i == 2 || i == 3)) {
											e.isDelete = false;
										}
										i++;
									});
								}
							} else if ($scope.Case.UsageID == "5") {
								if ($scope.UseBenefits.users.counts.length > 0) {
									var i = 1;
									$.grep($scope.UseBenefits.users.counts, function (e) {
										if (e.user_id == 'user_' + i && (i == 2 || i == 3)) {
											e.isDelete = false;
										}
										i++;
									});
								}
							}
						}
					});
					/**
						Watch UsageId as well to manage as per change in type we need to show number of users in medical usage grid.
						
					*/
					$scope.$watch('Case.UsageID', function (newValue) {
						if (!isEmpty($scope.Case.UsageID)) {
							if ($scope.Case.UsageID == "2") {
								if ($scope.UseBenefits.users.counts.length > 0) {
									var i = 1;
									$.grep($scope.UseBenefits.users.counts, function (e) {
										if (e.user_id == 'user_' + i && (i == 2)) {
											e.isDelete = false;
										}
										i++;
									});
								}
							} else if ($scope.Case.UsageID == "3") {
								if ($scope.UseBenefits.users.counts.length > 0) {
									var i = 1;
									$.grep($scope.UseBenefits.users.counts, function (e) {
										if (e.user_id == 'user_' + i && (i == 2 || i == 3)) {
											e.isDelete = false;
										}
										i++;
									});
								}
							} else if ($scope.Case.UsageID == "4") {
								if ($scope.UseBenefits.users.counts.length > 0) {
									var i = 1;
									$.grep($scope.UseBenefits.users.counts, function (e) {
										if (e.user_id == 'user_' + i && (i == 2 || i == 3)) {
											e.isDelete = false;
										}
										i++;
									});
								}
							} else if ($scope.Case.UsageID == "5") {
								if ($scope.UseBenefits.users.counts.length > 0) {
									var i = 1;
									$.grep($scope.UseBenefits.users.counts, function (e) {
										if (e.user_id == 'user_' + i && (i == 2 || i == 3)) {
											e.isDelete = false;
										}
										i++;
									});
								}
							}
						}
					});
					/**
						Supporting function to manage UsageId.
						
					*/
					$scope.changeUsageCodeFn = function (flag) {
						if ($scope.Case.UsageID == "1") {
							if ($scope.UseBenefits.users.counts.length > 1) {
								var i = 1;
								$.grep($scope.UseBenefits.users.counts, function (e) {
									if (!(i == 1)) {
										$scope.removeMemberColumn(false, e.user_id, false)
									}
									i++;
								});
							}
						} else if ($scope.Case.UsageID == "2") {

							if ($scope.UseBenefits.users.counts.length > 1) {
								var i = 1;
								$.grep($scope.UseBenefits.users.counts, function (e) {
									if (!(i == 1)) {
										$scope.removeMemberColumn(false, e.user_id, false)
									}
									i++;
								});
							}

							if ($scope.UseBenefits.users.counts.length == 1) {
								$scope.addNewMember('spouse');
							}

						} else if ($scope.Case.UsageID == "3") {

							if ($scope.UseBenefits.users.counts.length > 1) {
								var i = 1;
								$.grep($scope.UseBenefits.users.counts, function (e) {
									if (!(i == 1)) {
										$scope.removeMemberColumn(false, e.user_id, false)
									}
									i++;
								});
							}
							if ($scope.UseBenefits.users.counts.length == 1) {
								$scope.addNewMember('child_1');
							}

							if ($scope.Defaults.UseBenefits.users.counts.length > 1) {
								var i = 1;
								$.grep($scope.Defaults.UseBenefits.users.counts, function (e) {
									if (!(i == 1 || i == 2 || i == 3) && !e.isDisplay) {
										if (!isEmpty(e.isDisplay)) {
											$scope.addNewMember(e.display);
										}
									}
									i++;
								});
							}

						} else if ($scope.Case.UsageID == "4") {
							if ($scope.UseBenefits.users.counts.length > 1) {
								var i = 1;
								$.grep($scope.UseBenefits.users.counts, function (e) {
									if (!(i == 1)) {
										$scope.removeMemberColumn(false, e.user_id, false)
									}
									i++;
								});
							}
							if ($scope.UseBenefits.users.counts.length == 1) {
								$scope.addNewMember('spouse');
								$scope.addNewMember('child_1');
							}

							if ($scope.Defaults.UseBenefits.users.counts.length > 1) {
								var i = 1;
								$.grep($scope.Defaults.UseBenefits.users.counts, function (e) {
									if (!(i == 1 || i == 2 || i == 3) && !e.isDisplay) {
										if (!isEmpty(e.isDisplay)) {
											$scope.addNewMember(e.display);
										}
									}
									i++;
								});
							}



						} else if ($scope.Case.UsageID == "5") {
							if ($scope.UseBenefits.users.counts.length > 1) {
								var i = 1;
								$.grep($scope.UseBenefits.users.counts, function (e) {
									if (!(i == 1)) {
										$scope.removeMemberColumn(false, e.user_id, false)
									}
									i++;
								});
							}
							if ($scope.UseBenefits.users.counts.length == 1) {
								$scope.addNewMember('child_1');
							}
						}
						$scope.dataLoading = false;
					};
					/**
						Call usage code function when changed in view page.
						1. Usage Code = 1, only one member (hide "add new member" button?
						2. Usage Code=2, add second member  ( hide add member button)
						3. Usage Code= 3, two members (don't hide add member button)
						4. Usage Code=4, Add second and third member, (don't hide add member button)
						
					*/
					$scope.changeUsageCode = function (flag) {
						$scope.dataLoading = true;
						$scope.updateDefault();
						$scope.changeUsageCodeFn();
						$scope.dataLoading = false;
					}

					/**
						Reset user grid.			
					*/
					$scope.updateDefault = function () {
						$.grep($scope.UseBenefits.users.counts, function (e) {
							found = $filter('filter')($scope.Defaults.UseBenefits.users.counts, function (d) {
								return d.user_id == e.user_id;
							})[0];
							if (!isEmpty(found)) {
								found.isDisplay = e.isDisplay;
							}
						})

						$.grep($scope.UseBenefits.users.dobs, function (e) {
							found = $filter('filter')($scope.Defaults.UseBenefits.users.dobs, function (d) {
								return d.user_id == e.user_id;
							})[0];
							if (!isEmpty(found)) {
								found.val = e.val;
							}
						})

						$.grep($scope.UseBenefits.users.gender, function (e) {
							found = $filter('filter')($scope.Defaults.UseBenefits.users.gender, function (d) {
								return d.user_id == e.user_id;
							})[0];
							if (!isEmpty(found)) {
								found.val = e.val;
							}
						})

						$.grep($scope.UseBenefits.users.smoking, function (e) {
							found = $filter('filter')($scope.Defaults.UseBenefits.users.smoking, function (d) {
								return d.user_id == e.user_id;
							})[0];
							if (!isEmpty(found)) {
								found.val = e.val;
							}
						})

						$.grep($scope.UseBenefits.users.age, function (e) {
							var found = $filter('filter')($scope.Defaults.UseBenefits.users.age, function (d) {
								return d.user_id == e.user_id;
							})[0];
							if (!isEmpty(found)) {
								found.val = e.val;
							}
						})

						$.grep($scope.UseBenefits.users.Criticalillnesses, function (e) {
							var found = $filter('filter')($scope.Defaults.UseBenefits.users.Criticalillnesses, function (d) {
								return d.user_id == e.user_id;
							})[0];
							if (!isEmpty(found)) {
								found.val = e.val;
							}
						})



						$.grep($scope.UseBenefits.users.UseMedicalUsage, function (e) {
							found = $filter('filter')($scope.Defaults.UseBenefits.users.UseMedicalUsage, function (d) {
								return d.user_id == e.user_id;
							})[0];
							if (!isEmpty(found)) {
								found.val = e.val;
							}
						})
						if (typeof $scope.display != 'undefined' && $scope.display.length > 0) {
							for (var i = 0; i < $scope.display.length; i++) {
								if ($scope.display[i].benefit) {
									$.grep($scope.UseBenefits.users.benefitsVal[$scope.display[i].id], function (e) {
										found = $filter('filter')($scope.Defaults.UseBenefits.users.benefitsVal[$scope.display[i].id], function (d) {
											return d.user_id == e.user_id;
										})[0];
										if (!isEmpty(found)) {
											found.cost = e.cost;
											found.defaultCost = e.defaultCost;
											found.note = e.note;
											found.qty = e.qty;
										}
									});
								}
							}
						}
					}


					/**
						Watch Case ZipCode	to update carrier and get County dropdown values as well.		
					*/
					var filterTextTimeout;
					$scope.$watch('Case.ZipCode', function (newValue) {
						if (filterTextTimeout) $timeout.cancel(filterTextTimeout);
						filterTextTimeout = $timeout(function () {

							$scope.getZipCodeCounty();
							$scope.ZipCodeChange = true;
							if (!$scope.requestInProgressCarrier) {
								$scope.getCarrier();
							}
						}, 1000); // delay 250 ms						
					});

					/**
						Update Zip code.		
					*/
					$scope.updateZip = function () {
						$timeout(function () {
							$scope.updateCarrier();
							$scope.getMedicalUsages();
							$scope.getTopInsurancePlan(0);
						})
					}
					/**
						Update County (Reset medical usage grid and carrier list and call top 6 insurance API).		
					*/
					$scope.updateCounty = function () {
						$timeout(function () {
							$scope.getMedicalUsages();
							$scope.updateCarrier();
							$scope.getTopInsurancePlan(0);
						})
					}

					/**
						Update Year (get employer lsit, get carrier list and call top 6 insurance API).		
					*/
					//$scope.updateYear=function(){					
					//	$scope.getEmployer();
					//	$scope.updateCarrier();
					//	$scope.getInsuranceType();
					//	$scope.getTopInsurancePlan(0);			
					//}

					/**
						Update Employer (Reset medical usage grid and carrier list and call top 6 insurance API).		
					*/
					// $scope.updateEmloyer=function(){					
					// $scope.Applicant.InsuranceTypeId="";
					// $scope.Case.JobNumber="";
					// if(!$scope.requestInProgressCarrier){
					// $scope.getCarrier();					
					// }
					// if(!isEmpty($scope.Case.IssuerID)){	
					// $scope.Case.IssuerID="";	
					// bootbox.alert(messages.changedEmployerCarrierWarning);	
					// }
					// //$scope.getJobNumberList();
					// $scope.getMedicalUsages();
					// $scope.getTopInsurancePlan(0);
					// }
					/**
						Update Carrier (carrier list).		
					*/
					$scope.updateCarrier = function () {
						if (!$scope.requestInProgressCarrier) {
							$scope.getCarrier();
						}
						if (!isEmpty($scope.Case.IssuerID)) {
							$scope.Case.IssuerID = "";
							bootbox.alert(messages.changedEmployerCarrierWarning);
						}
					}

					var filterInsuranceTypeTimeout;
					/**
						Watch employer ID and call insurance type list and carrier list.		
					*/
					$scope.$watch('Applicant.EmployerId', function (newValue) {
						$scope.dataLoading = true;
						//$scope.EmployerName='';
						if (!isEmpty($scope.Applicant.EmployerId) && $scope.Applicant.EmployerId > 0 && $scope.Applicant.EmployerId != "") {
							if (typeof $scope.employers != 'undefined' && $scope.employers.length > 0) {
								found = $filter('filter')($scope.employers, function (d) {
									return d.EmployerId == $scope.Applicant.EmployerId;
								})[0];
								if (found) {
									$scope.EmployerName = found.EmployerName;
								}
							}
							if ($scope.Applicant.EmployerId == messages.IndividualEmployerId || $scope.Applicant.EmployerId == messages.ShopEmployerId) {
								$scope.Applicant.InsuranceTypeId = '';
								$scope.dataLoading = false;
							} else {
								$scope.getInsuranceType($scope.Applicant.InsuranceTypeId);
								if (filterInsuranceTypeTimeout) $timeout.cancel(filterInsuranceTypeTimeout);
								filterInsuranceTypeTimeout = $timeout(function () {
									//$scope.getInsuranceType();
									if (!$scope.requestInProgressCarrier) {
										$scope.getCarrier();
									}
									$scope.dataLoading = false;
								}, 1000); // delay 250 ms	
							}
						} else {
							$scope.dataLoading = false;
						}
					});
					/**
						Watch State Code and Reset medical usage Grid.		
					*/

					$scope.$watch('Case.StateCode', function (newValue) {
						if ($scope.Case.StateCode != '') {
							$scope.getMedicalUsages();
						}
					});

					/**
						Watch Case CountyName and get list carrier.		
					*/
					$scope.$watch('Case.CountyName', function (newValue) {
						if ($scope.Case.CountyName != '') {
							$scope.getMedicalUsages();
							$.grep($scope.CountyList, function (e) {
								if ($scope.Case.CountyName == e.CountyName) {
									$scope.Case.StateCode = e.StateCode;

								}
							})
							if (!$scope.requestInProgressCarrier) {
								$scope.getCarrier();
							}
						}
					});
					/**
						Watch County List and get CountyName and StateCode.		
					*/
					$scope.$watch('CountyList', function (newValue) {
						if ($scope.CountyList.length > 0) {
							$.grep($scope.CountyList, function (e) {
								if ($scope.Case.CountyName == e.CountyName) {
									$scope.Case.StateCode = e.StateCode;
								}
							})
						}

						if (!isEmpty($scope.Case.CountyName)) {
							if (typeof $scope.CountyList != 'undefined' && $scope.CountyList.length > 0) {
								found = $filter('filter')($scope.CountyList, function (d) {
									return d.CountyName == $scope.Case.CountyName;
								})[0];
								if (!isEmpty(found)) {
									$scope.CountyName = found.CountyName;
								}
							}
						}
						if ($scope.CountyList.length == 1) {
							//if(!$routeParams.id){						
							$scope.Case.CountyName = $scope.CountyList[0].CountyName;
							if (!isEmpty($scope.Case.CountyName)) {
								$("#CountyName-error").hide();
								$("input[name=CountyName]").removeClass("error");
							}
							//}
						}

					});



					/**
						Watch InsuranceTypeList and set name.		
					*/
					// $scope.$watch('InsuranceTypeList', function(newValue) {	
					// console.log('test1')
					// if(!isEmpty($scope.Applicant.InsuranceTypeId)){
					// if(typeof $scope.InsuranceTypeList!='undefined' && $scope.InsuranceTypeList.length>0){
					// found=$filter('filter')($scope.InsuranceTypeList, function (d) {return d.InsuranceTypeId == $scope.Applicant.InsuranceTypeId;})[0];

					// if(!isEmpty(found)){
					// $scope.InsuranceName=found.InsuranceType1;
					// }	
					// }
					// }				
					// });

					$scope.medicalUsagesCount = false;
					/**
						Call getMedicalUsages API and get list of plan benefit in medical area.		
					*/
					$scope.getMedicalUsages = function () {
						$scope.dataLoading = true;
						$scope.medicalUsages = [];
						var medicalUsagesParam = {};
						var medicalUsagesFlag = true;

						if ((isEmpty($scope.Case.ZipCode)) || (isEmpty($scope.Case.CountyName)) || (isEmpty($scope.Applicant.EmployerId)) || (isEmpty($scope.Case.StateCode))) {
							medicalUsagesFlag = false;
						}
						if (medicalUsagesFlag) {
							medicalUsagesParam.zipcode = $scope.Case.ZipCode;
							medicalUsagesParam.CountyName = $scope.Case.CountyName;
							medicalUsagesParam.EmployerId = $scope.Applicant.EmployerId;
							medicalUsagesParam.StateCode = $scope.Case.StateCode;
							CaseService.getMedicalUsage(medicalUsagesParam, function (response) {
								if (response.Status) {
									$scope.medicalUsages = response.MedicalsUsage;
									$scope.Case.RatingAreaID = response.RatingAreaId;
									$scope.medicalUsagesUpdate($scope.medicalUsagesCount);
									$scope.dataLoading = false;
									$scope.medicalUsagesCount = true;
								} else {
									$scope.medicalUsages = [];
									$scope.Case.RatingAreaID = '';
									$scope.medicalUsagesUpdate($scope.medicalUsagesCount);
									$scope.dataLoading = false;
								}
							})
						} else {
							$scope.dataLoading = false;
						}
					};
					/**
						get getZipCodeCounty.		
					*/
					$scope.getZipCodeCounty = function () {
						$scope.dataLoading = true;
						if (!$scope.edit) {
							$scope.Case.CountyName = '';
							$scope.Case.RatingAreaID = '';
						}
						$scope.medicalUsages = [];
						$scope.CountyList = [];

						CaseService.getCounty($scope.Case.ZipCode, function (response) {
							if (response.Status && response.CountyList.length > 0) {
								$scope.CountyList = response.CountyList;
								$scope.dataLoading = false;
							} else {
								$scope.Case.CountyName = '';
								$scope.dataLoading = false;
							}
						})
					}
					/**
								get getZipCodeState.		
							*/
					$scope.getZipCodeState = function () {
						$scope.dataLoading = true;
						CaseService.getCounty($scope.Applicant.Zip, function (response) {
							if (response.Status && response.CountyList.length > 0) {
								$scope.Applicant.State = response.CountyList[0].StateId.toString();
								$scope.Applicant.City = response.CountyList[0].City.toString();
								$scope.Applicant.CountyName = response.CountyList[0].CountyName.toString();
								$scope.dataLoading = false;
							} else {
								$scope.Case.CountyName = '';
								$scope.Applicant.State = '';
								$scope.Applicant.City = '';
								$scope.Applicant.CountyName = '';
								$scope.dataLoading = false;
							}
						})
					}
					/**
						get getInsuranceType.		
					*/
					$scope.getInsuranceType = function (InsuranceTypeId) {
						$scope.dataLoading = true;
						if (!$scope.edit) {
							$scope.Applicant.InsuranceTypeId = '';
						}
						$scope.InsuranceTypeList = [];
						CaseService.getInsuranceType($scope.Applicant.EmployerId, $scope.Case.Year, function (response) {
							if (response.Status && response.InsuranceTypes.length > 0) {
								$scope.InsuranceTypeList = response.InsuranceTypes;


								if (typeof $scope.InsuranceTypeList != 'undefined' && $scope.InsuranceTypeList.length > 0) {
									found = $filter('filter')($scope.InsuranceTypeList, function (d) {
										return d.InsuranceTypeId == InsuranceTypeId;
									})[0];
									if (found) {
										$scope.Applicant.InsuranceTypeId = found.InsuranceTypeId;
										$scope.InsuranceName = found.InsuranceType1;
									}

								}
								$scope.dataLoading = false;
							} else {
								$scope.Applicant.InsuranceTypeId = '';
								$scope.dataLoading = false;
							}
						})
					}

					/**
						get getCarrier.		
					*/
					$scope.getCarrier = function () {
						var carrierParam = {};
						var CarrierFlag = true;
						$scope.carriers = [];
						carrierParam.EmployerId = $scope.Applicant.EmployerId;
						carrierParam.ZipCode = $scope.Case.ZipCode;
						carrierParam.CountyName = $scope.Case.CountyName;
						carrierParam.InsuranceTypeId = $scope.Applicant.InsuranceTypeId;
						carrierParam.BusinessYear = $scope.Case.Year;

						if (!($scope.Applicant.EmployerId && $scope.Applicant.EmployerId != 'null' && parseInt($scope.Applicant.EmployerId) != 'NaN')) {
							carrierParam.EmployerId = "0";
							CarrierFlag = false;
						}
						if (!($scope.Case.ZipCode && $scope.Case.ZipCode != 'null' && parseInt($scope.Case.ZipCode) != 'NaN')) {
							carrierParam.ZipCode = "0";
							CarrierFlag = false;
						}
						if (!($scope.Case.CountyName && $scope.Case.CountyName != 'null')) {
							carrierParam.CountyName = "";
							CarrierFlag = false;
						}
						if (!($scope.Case.Year && $scope.Case.Year != 'null' && parseInt($scope.Case.Year) != 'NaN')) {
							carrierParam.BusinessYear = "0";
							CarrierFlag = false;
						}
						//$scope.Applicant.EmployerId != messages.IndividualEmployerId || $scope.Applicant.EmployerId != messages.ShopEmployerId
						if ($scope.Applicant.EmployerId != messages.IndividualEmployerId && $scope.Applicant.EmployerId != messages.ShopEmployerId) {
							if (!($scope.Applicant.InsuranceTypeId && $scope.Applicant.InsuranceTypeId != 'null' && parseInt($scope.Applicant.InsuranceTypeId) != 'NaN')) {
								carrierParam.InsuranceTypeId = "0";
								CarrierFlag = false;
							}
						} else {
							carrierParam.InsuranceTypeId = "0";
						}
						if (CarrierFlag) {
							if (!$scope.requestInProgressCarrier) {
								$scope.dataLoading = true;
								$scope.requestInProgressCarrier = true;
								CaseService.getCarrier(carrierParam, function (response) {
									if (response.Status && response.Carriers.length > 0) {
										$scope.carriers = response.Carriers;
										$scope.dataLoading = false;
										$scope.requestInProgressCarrier = false;
									} else {
										$scope.dataLoading = false;
										$scope.requestInProgressCarrier = false;
									}
								})
							}

						}
					}

					/**
						get getEmployerList.	
					*/
					// $scope.getEmployerList=function(){
					// $scope.dataLoading=true;			
					// $scope.employers=[];	
					// if(!isEmpty($scope.Case.Year)){
					// $scope.requestInProgress=true;
					// CaseService.getEmployer($scope.Case.Year,function(response){				
					// if(response.Status && response.Employers.length>0){					
					// $scope.employers=response.Employers;					
					// $scope.dataLoading=false;
					// $scope.requestInProgress=false;	
					// }else{					
					// $scope.dataLoading=false;	
					// $scope.requestInProgress=false;	
					// }				
					// })
					// }else{
					// $scope.dataLoading=false;	
					// }					
					// }
					/**
						check isValidFirstStep.	
					*/
					$scope.isValidFirstStep = function () {
						console.log('$scope.form.CaseTitle.$valid', $scope.form.CaseTitle.$valid)
						if ($scope.form.fname.$valid && $scope.form.lname.$valid && $scope.form.address.$valid && $scope.form.city.$valid && $scope.form.State.$valid && $scope.form.zipcode.$valid && $scope.form.phone.$valid && $scope.form.caseZipCode.$valid && $scope.form.CountyName.$valid && $scope.form.CurrentPlan.$valid && $scope.form.Notes.$valid && $scope.form.CaseTitle.$valid && $scope.form.email.$valid) {
							return true;
						} else {
							return false;
						}
					}
					/**
						check isValidSecondStep.	
					*/
					$scope.isValidSecondStep = function () {
						if ($scope.form.code.$valid && $scope.form.MAGIncome.$valid && $scope.form.TaxRate.$valid) {

							if ($scope.Applicant.EmployerId == messages.IndividualEmployerId || $scope.Applicant.EmployerId == messages.ShopEmployerId) {
								if (typeof ($scope.form.HSAFunding) != 'undefined') {
									if ($scope.form.HSAFunding.$valid) {
										return true;
									} else {
										return false;
									}
								} else {
									return true;
								}

							} else {
								// if(isEmpty($scope.Applicant.EmployerId) || isEmpty($scope.Applicant.InsuranceTypeId)){
								if (isEmpty($scope.Applicant.EmployerId)) {
									return false;
								} else {
									return true;
								}

							}


						} else {
							return false;
						}
					}
					/**
						check isValidMedicalStep.	
					*/
					$scope.isValidMedicalStep = function () {
						var flag = true;
						$.grep($scope.UseBenefits.users.age, function (e) {
							var age = parseInt(e.val);
							if (age <= 0 || age == 'NaN' || typeof age == 'string' || isEmpty(e.val)) {
								flag = false;
							}
						});

						$.grep($scope.UseBenefits.users.gender, function (e) {
							var user_id = e.user_id;
							if (isEmpty(e.val)) {
								flag = false;
							}
						});
						$.grep($scope.UseBenefits.users.smoking, function (e) {
							var user_id = e.user_id;
							if (e.val == '') {
								flag = false;
							}
						});
						return flag;
					}

					$scope.$watch('Case.DedBalAvailToRollOver', function (newValue) {
						if ($scope.Case.DedBalAvailDate == null || $scope.Case.DedBalAvailDate == "") {
							$scope.IsDedBalAvlDate = newValue > 0 ? true : false;
							//do something
						}
					});
					$scope.$watch('Case.DedBalAvailDate', function (newValue) {
						// console.log('newValue',newValue)
						// console.log('$scope.Case.DedBalAvailToRollOver>0',$scope.Case.DedBalAvailToRollOver>0)
						// console.log('($scope.Case.DedBalAvailDate == null || $scope.Case.DedBalAvailDate == "")',($scope.Case.DedBalAvailDate == null || $scope.Case.DedBalAvailDate == ""))
						if ($scope.Case.DedBalAvailToRollOver > 0 && ($scope.Case.DedBalAvailDate == null || $scope.Case.DedBalAvailDate == "")) {
							$scope.IsDedBalAvlDate = true;
						} else {
							$scope.IsDedBalAvlDate = false;
						}
					});



					/**
						Input: 
						Return: All top 6 insurance plans.
					*/
					$scope.getTopInsurancePlan = function (flag) {

						var data = {};
						var calculatePlan = true;
						// $('#collapseOne').collapse('toggle');

						if ($scope.IsDedBalAvlDate) {
							return;
						}

						if (!$scope.isValidSecondStep() || $scope.Case.UsageID == '0' || $scope.Case.MAGIncome < 1 || !$scope.isValidMedicalStep() || isEmpty($scope.Case.CountyName)) {
							calculatePlan = false;
						}
						if (!$scope.auto_calculate && !flag) {
							calculatePlan = false;
							return;
						}

						$scope.ChangesOccur();

						data.HSAPercentage = $scope.Case.HSAFunding;
						data.TaxRate = $scope.Case.TaxRate;
						data.IssuerId = $scope.Case.IssuerID;
						data.PlanID = $scope.Case.PlanID;
						data.EmployerId = $scope.Applicant.EmployerId;
						data.InsuranceTypeId = $scope.Applicant.InsuranceTypeId;
						data.BusinessYear = $scope.Case.Year;
						data.JobNumber = $scope.Case.JobNumber;
						data.DedBalAvailToRollOver = $scope.Case.DedBalAvailToRollOver;
						data.DedBalAvailDate = $scope.Case.DedBalAvailDate;
						data.TierIntention = $scope.Case.TierIntention;

						$scope.CurrentPlanName = $scope.Applicant.CurrentPlan;
						if ($scope.Applicant.CurrentPlan != '' && $scope.Applicant.CurrentPlan != 'Waived' && $scope.Applicant.CurrentPlan != 'NewHire') {
							data.CurrentPlan = $scope.Applicant.CurrentPlan;
							$scope.CurrentPlan = $scope.Applicant.CurrentPlan;
							var CurrentPlanResult = $filter('filter')($scope.Case.CasePlanResults, function (item) {
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
						// else {
						// 	data.CurrentPlan = '';
						// 	$scope.CurrentPlan = '';
						// 	$scope.CurrentPlanName = '';
						// }
						if($scope.CurrentPlanName == 'Waived')
						{
							$scope.CurrentPlanName = 'n/a (Waived)';
						}
						if($scope.CurrentPlanName == 'NewHire')
						{
							$scope.CurrentPlanName = 'n/a (New Hire)';
						}

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
						if (!($scope.Case.JobNumber && $scope.Case.JobNumber != 'null' && parseInt($scope.Case.JobNumber) != 'NaN')) {
							data.JobNumber = "0";
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
						data.post = {};
						data.post.data = {};
						data.post.UsesDetail = {};
						// console.log($scope.Applicant);
						// data.ZipCode = $scope.Applicant.Zip;
						// data.CountyName = $scope.Applicant.CountyName;

						//By Vaibhav to recalulate ages of all users when calculation will run
						$.grep($scope.UseBenefits.users.dobs, function (e) {
							var user_id = e.user_id;
							if (typeof e.val != 'undefined' && e.val != '') {
								var currentDOB = new Date(e.val);
							} else {
								var currentDOB = '';
							}
							$.grep($scope.UseBenefits.users.age, function (e) {
								if (user_id == e.user_id) {
									e.val = $scope.calculateAge(currentDOB);
								}
								return;
							});
						});
						//End

						var i = 0;
						$.grep($scope.UseBenefits.users.age, function (e) {
							if (typeof data.post.data[i] == 'undefined') {
								data.post.data[i] = {};
							}
							var age = parseInt(e.val);
							if (age <= 0 || age == 'NaN' || typeof age == 'string' || isEmpty(e.val)) {
								calculatePlan = false;
							}
							data.post.data[i].Age = parseInt(e.val);
							i++;
						});

						$.grep($scope.UseBenefits.users.gender, function (e) {
							if (isEmpty(e.val)) {
								calculatePlan = false;
							}
						});
						var i = 0;
						$.grep($scope.UseBenefits.users.smoking, function (e) {
							if (typeof data.post.data[i] == 'undefined') {
								data.post.data[i] = {};
							}
							if (e.val == '') {
								calculatePlan = false;
							}
							data.post.data[i].SmokingStatus = e.val;
							i++;
						});
						if (typeof $scope.display != 'undefined' && $scope.display.length > 0) {
							for (var i = 0; i < $scope.display.length; i++) {
								if ($scope.display[i].benefit) {
									var j = 0;
									$.grep($scope.UseBenefits.users.benefitsVal[$scope.display[i].id], function (e) {
										var qty = ($.isNumeric(e.qty)) ? parseInt(e.qty) : 0;
										var cost = ($.isNumeric(e.cost)) ? parseFloat(e.cost) : 0;
										var user_id = e.user_id;
										var cat_id = e.id;
										if (qty > 0 && cost < -1) {
											calculatePlan = false;
											return;
										}
										if (cost.length > 7) {
											calculatePlan = false;
											return;
										}
										found = $filter('filter')($scope.UseBenefits.users.UseMedicalUsage, function (d) {
											return d.user_id === user_id;
										})[0];
										if (found) {
											if (typeof data.post.UsesDetail[j] == 'undefined') {
												data.post.UsesDetail[j] = {};
											}

											data.post.UsesDetail[j].FamilyMemberNumber = parseInt(j) + 1;
											if (typeof data.post.UsesDetail[j].BenefitUses == 'undefined') {
												data.post.UsesDetail[j].BenefitUses = [];
											}
											data.post.UsesDetail[j].BenefitUses.push({
												"BenefitId": cat_id,
												"UsageCost": qty * cost,
												"UsageQty": qty
											});
											j++;

										}
										return;
									});
								}
							}
						} else {
							calculatePlan = false;
						}

						if (!$("#caseAdd").valid()) {
							$("#caseAdd").submit();
						} else {
							if (calculatePlan) {
								$('#collapseOne').collapse('toggle');
								if (!$scope.requestInProgress) {
									$scope.formLoading = true;
									$scope.requestInProgress = true;
									// Instantiate these variables outside the watch	CountyName				
									CaseService.calculatePlans(data, function (response) {
										if (response.Status) {
											$scope.data.post = [];
											$scope.Case.SubsidyAmount = response.SubsidyAmount;
											$scope.Case.HSALimit = response.HSALimit;
											$scope.Case.HSAAmount = response.HSAAmount;
											$scope.Case.FPL = response.FPL;
											$scope.Case.SecondLowestPlanId = response.SecondLowestPlanId;
											$scope.data.totalRecord = response.Plans.length;
											$scope.graphResults = response.Plans;
											$scope.Case.CasePlanResults = response.Plans;


											// if($scope.Case.CasePlanResults.length === 1)
											// {		
											// 	$scope.pro_red = '62%';
											// 	$scope.pro_green = '38%';
											// }
											// else if($scope.Case.CasePlanResults.length === 2)
											// {		
											// 	$scope.pro_red = '45%';
											// 	$scope.pro_green = '27.5%';
											// }
											// else if($scope.Case.CasePlanResults.length === 3)
											// {		
											// 	$scope.pro_red = '35%';
											// 	$scope.pro_green = '21.66%';
											// }
											// else if($scope.Case.CasePlanResults.length === 4)
											// {		
											// 	$scope.pro_red = '30%';
											// 	$scope.pro_green = '17.5%';
											// }

											if($scope.Case.CasePlanResults.length === 1)
											{		
												$scope.pro_red = '70%';
												$scope.pro_green = '30%';
											}
											else if($scope.Case.CasePlanResults.length === 2)
											{		
												$scope.pro_red = '50%';
												$scope.pro_green = '25%';
											}
											else if($scope.Case.CasePlanResults.length === 3)
											{		
												$scope.pro_red = '40%';
												$scope.pro_green = '20%';
											}
											else if($scope.Case.CasePlanResults.length === 4)
											{		
												$scope.pro_red = '30%';
												$scope.pro_green = '17.5%';
											}

											else if($scope.Case.CasePlanResults.length === 5)
											{		
												$scope.pro_red = '30%';
												$scope.pro_green = '14%';
											}

											if ($scope.CurrentPlan != '' && $scope.CurrentPlan != $scope.Case.CasePlanResults[0].PlanId) {
												var CurrentPlanResult = $filter('filter')($scope.Case.CasePlanResults, function (item) {
													return item.PlanId == $scope.CurrentPlan;
												});
												if (CurrentPlanResult.length > 0)
													$scope.DifferenceAmount = CurrentPlanResult[0].TotalPaid - $scope.Case.CasePlanResults[0].TotalPaid;
											}

											if ($scope.Applicant.CurrentPlan != '' && $scope.Applicant.CurrentPlan != 'Waived' && $scope.Applicant.CurrentPlan != 'NewHire') {
												$scope.CurrentPlan = $scope.Applicant.CurrentPlan;
												var CurrentPlanResult = $filter('filter')($scope.Case.CasePlanResults, function (item) {
													return item.PlanId == $scope.CurrentPlan;
												});
												if(CurrentPlanResult != null)
													if (CurrentPlanResult.length > 0)
													 $scope.CurrentPlanName = CurrentPlanResult[0].PlanName;
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

											var MaxWorstCase = Math.max.apply(Math, $scope.Case.CasePlanResults.map(function (item) {
												return item.WorstCase;
											})) + 3000;
											
											var MaxTotalCost = Math.max.apply(Math, $scope.Case.CasePlanResults.map(function (item) {
												return item.NetAnnualPremium + item.Medical
											})) + 3000;
											
											$scope.max = MaxTotalCost > MaxWorstCase ? MaxTotalCost : MaxWorstCase;
											
											$.grep(response.Plans, function (e) {


												// $scope.data.post.push({"State":e.PlanName+" ("+e.Rank+")","Net Annual Premium":parseFloat(e.NetAnnualPremium),"Medical (net of HSA tax savings)":parseFloat(e.Medical)});
												$scope.data.post.push({
													"State": e.PlanName + '-' + e.PlanIdIndiv1,
													"Rank": e.Rank,
													"Your Net Annual Premium Contribution": parseFloat(e.NetAnnualPremium),
													"Your Net Out-of-Pocket Medical Costs": parseFloat(e.Medical)
												});

											});
											$scope.data.width = ($scope.graphResults.length * 162) + ($scope.graphResults.length * 32.5);
											$scope.Case.CreatedDateTime = new Date();
											// $rootScope.updateGraph($("#step-2").find('d3-bars'), false);
											$scope.requestInProgress = false;
											$('#collapseOne').collapse('hide');
											$timeout(function () {
												$('#holder').niceScroll({
													autohidemode: 'false', // Do not hide scrollbar when mouse out
													cursorborderradius: '12px', // Scroll cursor radius
													background: '#ffffff', // The scrollbar rail color
													cursorwidth: '5px', // Scroll cursor width
													cursorcolor: '#3b84f2' // Scroll cursor color
												});
											}, 1000);
										} else {
											if (response.redirect) {
												bootbox.alert(messages.TryLater + 'Calculation', function () {
													// $location.path('/');
													$scope.$apply();
												})
											}
											$scope.data.post = [];
											$scope.data.totalRecord = 0;
											$scope.Case.SubsidyAmount = 0;
											$scope.Case.HSALimit = 0;
											$scope.Case.HSAAmount = 0;
											$scope.Case.FPL = 0;
											$scope.Case.SecondLowestPlanId = "";
											$scope.graphResults = [];
											$scope.Case.CasePlanResults = [];

											if (typeof response.Message != 'undefined') {
												bootbox.alert(response.Message)
											}
											$scope.requestInProgress = false;
										}
										$scope.formLoading = false;
									});
								}
								return calculatePlan;
							} else {
								bootbox.alert(messages.ErrorFormMedical, function () {
									$scope.data.post = [];
									$scope.data.totalRecord = 0;
									$scope.Case.SubsidyAmount = 0;
									$scope.Case.HSALimit = 0;
									$scope.Case.HSAAmount = 0;
									$scope.Case.FPL = 0;
									$scope.Case.SecondLowestPlanId = "";
									$scope.graphResults = [];
									$scope.Case.CasePlanResults = [];
									$scope.$apply();
								})

							}
						}

					}

					$scope.ChangesOccur = function () {
						IsChangesOccur = true;
						console.log('changes occur', IsChangesOccur);
					}


					$scope.set_width = function (max, currentValue) {
						if(max >20000)
						currentValue = currentValue*65/100;
						else if(max >15000)
							currentValue = currentValue*75/100;
							else if(max >12500)
							currentValue = currentValue*85/100;
							else	if(max >10000)
							currentValue = currentValue*95/100;
					return {
							width: Math.round(currentValue * 100 / max) + '%'
						}
					}

					$scope.check_width = function (max, currentValue) {
						if(max >20000)
						currentValue = currentValue*65/100;
						else if(max >15000)
							currentValue = currentValue*75/100;
							else if(max >12500)
							currentValue = currentValue*85/100;
					else	if(max >10000)
							currentValue = currentValue*95/100;
						return Math.round(currentValue * 100 / max);
					}

					/**
						Initial services load all required list values. (codes, plans, stateList, criticalillness)
					*/
					CaseService.waitForLayout(function (response) {
						$scope.codes = ((typeof response[0].data != 'undefined') && (response[0].data.Status)) ? response[0].data.Usagecodes : [];
						$scope.plans = ((typeof response[1].data != 'undefined') && (response[1].data.Status)) ? response[1].data.Plans : [];
						$scope.stateList = ((typeof response[2].data != 'undefined') && (response[2].data.Status)) ? response[2].data.States : [];
						$scope.criticalillness = ((typeof response[3].data != 'undefined') && (response[3].data.Status)) ? response[3].data.CriticalillnessList : [];
						$scope.statusCode = ((typeof response[4].data != 'undefined') && (response[4].data.Status)) ? response[4].data.CaseStatusMst : [];
						$scope.jobNos = [];
						$scope.jobNos = ((typeof response[5].data != 'undefined') && (response[5].data.Status)) ? response[5].data.JobMasters : [];
						$scope.employers = ((typeof response[6].data != 'undefined') && (response[6].data.Status)) ? response[6].data.Employers : [];
						$scope.agentsList2 = ((typeof response[7].data != 'undefined') && (response[7].data.Status)) ? response[7].data.AgentsList : [];
						$scope.agentsList1 = [];
						$.each($scope.agentsList2, function (i, el) {
							if ($.inArray(el.Name, $scope.agentsList1) === -1) $scope.agentsList1.push(el.Name);
						});
						$rootScope.pageLoading = false;
						if ($scope.codes.length > 0) {
							if (!$routeParams.id) {
								$scope.Case.UsageID = $scope.codes[0].UsagaId.toString();
							}
						}
						var dropdowndata = '';
						for (i = 0; i < $scope.statusCode.length; i++) {
							if ($scope.statusCode[i].Parent == 11) {
								$scope.statusCode[i].StatusCode = '\u00A0\u00A0\u00A0' + $scope.statusCode[i].StatusCode;
							}
						}

						if ($routeParams.action == 'copy' && $scope.statusCode.length > 0) {
							var objCaseStatus = $filter('filter')($scope.statusCode, function (item) {
								return item.StatusId == $scope.Case.StatusId;
							});
							if (objCaseStatus[0].StatusCode == 'Final - Sent') {
								var FinalNotSentStatusId = $filter('filter')($scope.statusCode, function (item) {
									return item.StatusCode == 'Final - Not Sent';
								});
							$scope.Case.StatusId = FinalNotSentStatusId[0].StatusId.toString();
							$scope.Case.CaseStatus = FinalNotSentStatusId[0].StatusCode;
							}
						}

						if (!isEmpty($scope.Case.UsageID)) {
							if (typeof $scope.codes != 'undefined' && $scope.codes.length > 0) {
								found = $filter('filter')($scope.codes, function (d) {
									return d.UsagaId == $scope.Case.UsageID;
								})[0];
								if (found) {
									$scope.UsageType = found.UsageType;
								}
							}
						}

						//	$('#StatusId1').html(dropdowndata);
					});


					/**
						Watch case planID and set name.
					*/
					$scope.$watch('Case.PlanID', function (newValue) {
						$scope.PlanType = '';
						if (!isEmpty($scope.Case.PlanID)) {
							if (typeof $scope.plans != 'undefined' && $scope.plans.length > 0) {
								found = $filter('filter')($scope.plans, function (d) {
									return d.PlanID == $scope.Case.PlanID;
								})[0];
								if (!isEmpty(found)) {
									$scope.PlanType = found.PlanType;
								}
							}
						}
					});
					/**
						Watch case IssuerID and set name.
					*/
					$scope.$watch('Case.IssuerID', function (newValue) {
						$scope.IssuerName = '';
						if (!isEmpty($scope.Case.IssuerID)) {
							if (typeof $scope.carriers != 'undefined' && $scope.carriers.length > 0) {
								found = $filter('filter')($scope.carriers, function (d) {
									return d.Id == $scope.Case.IssuerID;
								})[0];
								if (!isEmpty(found)) {
									$scope.IssuerName = found.IssuerName;
									$scope.IssuerAbbreviations = found.Abbreviations;
								}
							}
						}
					});
					/**
						Watch case UsageID and set name.
					*/
					$scope.$watch('Case.UsageID', function (newValue) {
						if (!isEmpty($scope.Case.UsageID)) {
							if (typeof $scope.codes != 'undefined' && $scope.codes.length > 0) {
								found = $filter('filter')($scope.codes, function (d) {
									return d.UsagaId == $scope.Case.UsageID;
								})[0];
								if (found) {
									$scope.UsageType = found.UsageType;
									$scope.getHSALimit();
									//write code for HSA Limit

								}
							}
						}
					});
					/**
						Watch case CountyName and set name.
					*/
					$scope.$watch('Case.CountyName', function (newValue) {
						$scope.CountyName = '';
						if (!isEmpty($scope.Case.CountyName)) {
							if (typeof $scope.CountyList != 'undefined' && $scope.CountyList.length > 0) {
								found = $filter('filter')($scope.CountyList, function (d) {
									return d.CountyName == $scope.Case.CountyName;
								})[0];
								if (!isEmpty(found)) {
									$scope.CountyName = found.CountyName;
								}
							}
						}
					});
					/**
						Watch case IssuerID and set name.
					*/
					$scope.$watch('Case.IssuerID', function (newValue) {
						$scope.IssuerName = '';
						if (!isEmpty($scope.Case.IssuerID) && $scope.Case.IssuerID > 0 && $scope.Case.IssuerID != "") {
							if (typeof $scope.carriers != 'undefined' && $scope.carriers.length > 0) {
								found = $filter('filter')($scope.carriers, function (d) {
									return d.Id == $scope.Case.IssuerID;
								})[0];
								if (found) {
									$scope.IssuerName = found.IssuerName;
								}

							}
						}
					});
					/**
						Watch case PlanID and set name.
					*/
					$scope.$watch('Case.PlanID', function (newValue) {
						$scope.PlanType = '';
						if (!isEmpty($scope.Case.PlanID) && $scope.Case.PlanID > 0 && $scope.Case.PlanID != "") {
							if (typeof $scope.plans != 'undefined' && $scope.plans.length > 0) {
								found = $filter('filter')($scope.plans, function (d) {
									return d.PlanID == $scope.Case.PlanID;
								})[0];
								if (found) {
									$scope.PlanType = found.PlanType;
								}

							}
						}
					});
					/**
						Watch case InsuranceTypeId and set name.
					*/
					// $scope.$watch('Applicant.InsuranceTypeId', function(newValue) {
					// $scope.InsuranceName='';				
					// console.log('testing')
					// if(!isEmpty($scope.Applicant.InsuranceTypeId) && $scope.Applicant.InsuranceTypeId > 0 && $scope.Applicant.InsuranceTypeId != ""){
					// if(typeof $scope.InsuranceTypeList!='undefined' && $scope.InsuranceTypeList.length>0){					
					// found=$filter('filter')($scope.InsuranceTypeList, function (d) {return d.InsuranceTypeId == $scope.Applicant.InsuranceTypeId;})[0];					
					// if(found){						
					// $scope.InsuranceName=found.InsuranceType1;	 	
					// } 

					// }
					// }									
					// });	

					/**
						Update Medical usage grid and per get Values.
					*/
					$scope.medicalUsagesUpdate = function (flag) {
						var oldCat = '';
						$scope.display = [];
						$scope.benefitV = {};
						$scope.benefitsPopup = [];
						$scope.category = [];
						//$scope.UseBenefits.users.benefitsVal={};
						//$scope.Defaults.UseBenefits.users.benefitsVal={};		
						if ($scope.medicalUsages.length > 0) {
							for (var j = 0; j < $scope.medicalUsages.length; j++) {
								var cat = $scope.medicalUsages[j];
								if (oldCat != cat.CategoryId) {
									oldCat = cat.CategoryId;
									$scope.category[cat.CategoryId] = cat.CategoryName;

									$scope.display.push({
										"id": cat.CategoryId,
										"name": cat.CategoryName,
										"benefit": false,
										"categoryDisplayOrder": cat.CategoryDisplayOrder
									});
									$scope.Defaults.display.push({
										"id": cat.CategoryId,
										"name": cat.CategoryName,
										"benefit": false,
										"categoryDisplayOrder": cat.CategoryDisplayOrder
									});

									$scope.display.push({
										"id": cat.MHMBenefitID,
										"name": cat.MHMBenefitName,
										"benefit": true,
										"cost": cat.MHMBenefitCost,
										"default": cat.IsDefault,
										"cat_id": cat.CategoryId,
										"display": cat.IsDefault,
										"defaultCost": cat.MHMBenefitCost,
										"benefitDisplayOrder": cat.BenefitDisplayOrder
									});

									$scope.Defaults.display.push({
										"id": cat.MHMBenefitID,
										"name": cat.MHMBenefitName,
										"benefit": true,
										"cost": cat.MHMBenefitCost,
										"default": cat.IsDefault,
										"cat_id": cat.CategoryId,
										"display": cat.IsDefault,
										"defaultCost": cat.MHMBenefitCost,
										"benefitDisplayOrder": cat.BenefitDisplayOrder
									});

									if (typeof $scope.UseBenefits.users.benefitsVal[cat.MHMBenefitID] == 'undefined') {
										$scope.UseBenefits.users.benefitsVal[cat.MHMBenefitID] = [];
										$scope.Defaults.UseBenefits.users.benefitsVal[cat.MHMBenefitID] = [];
									}

									var test = {};
									$.grep($scope.UseBenefits.users.counts, function (e) {
										test = $filter('filter')($scope.UseBenefits.users.benefitsVal[cat.MHMBenefitID], function (d) {
											return d.user_id == e.user_id;
										});


										if (typeof test[0] == 'undefined') {
											$scope.Defaults.UseBenefits.users.benefitsVal[cat.MHMBenefitID].push({
												"id": cat.MHMBenefitID,
												"qty": "",
												"cost": cat.MHMBenefitCost,
												"note": "",
												"user_id": e.user_id,
												"defaultCost": cat.MHMBenefitCost
											});
											$scope.UseBenefits.users.benefitsVal[cat.MHMBenefitID].push({
												"id": cat.MHMBenefitID,
												"qty": "",
												"cost": cat.MHMBenefitCost,
												"note": "",
												"user_id": e.user_id,
												"defaultCost": cat.MHMBenefitCost
											});
										}
									})

								} else {

									if (typeof $scope.UseBenefits.users.benefitsVal[cat.MHMBenefitID] == 'undefined') {
										$scope.UseBenefits.users.benefitsVal[cat.MHMBenefitID] = [];
										$scope.Defaults.UseBenefits.users.benefitsVal[cat.MHMBenefitID] = [];
									}

									$scope.display.push({
										"id": cat.MHMBenefitID,
										"name": cat.MHMBenefitName,
										"benefit": true,
										"cost": cat.MHMBenefitCost,
										"default": cat.IsDefault,
										"cat_id": cat.CategoryId,
										"display": cat.IsDefault,
										"defaultCost": cat.MHMBenefitCost,
										"benefitDisplayOrder": cat.BenefitDisplayOrder
									});

									$scope.Defaults.display.push({
										"id": cat.MHMBenefitID,
										"name": cat.MHMBenefitName,
										"benefit": true,
										"cost": cat.MHMBenefitCost,
										"default": cat.IsDefault,
										"cat_id": cat.CategoryId,
										"display": cat.IsDefault,
										"defaultCost": cat.MHMBenefitCost,
										"benefitDisplayOrder": cat.BenefitDisplayOrder
									});

									var test = {};

									var test = {};
									$.grep($scope.UseBenefits.users.counts, function (e) {
										test = $filter('filter')($scope.UseBenefits.users.benefitsVal[cat.MHMBenefitID], function (d) {
											return d.user_id == e.user_id;
										});


										if (typeof test[0] == 'undefined') {
											$scope.Defaults.UseBenefits.users.benefitsVal[cat.MHMBenefitID].push({
												"id": cat.MHMBenefitID,
												"qty": "",
												"cost": cat.MHMBenefitCost,
												"note": "",
												"user_id": e.user_id,
												"defaultCost": cat.MHMBenefitCost
											});
											$scope.UseBenefits.users.benefitsVal[cat.MHMBenefitID].push({
												"id": cat.MHMBenefitID,
												"qty": "",
												"cost": cat.MHMBenefitCost,
												"note": "",
												"user_id": e.user_id,
												"defaultCost": cat.MHMBenefitCost
											});
										}
									})
								}

								var i = 1;
								if (typeof $scope.Case.Families != 'undefined' && $scope.Case.Families.length > 0)
									$.grep($scope.Case.Families, function (e) {
										var userD = $scope.getUserDetail(i);
										$.grep(e.BenefitUserDetails, function (e1) {
											if (e1.MHMMappingBenefitId == cat.MHMBenefitID) {
												found = $filter('filter')($scope.UseBenefits.users.benefitsVal[cat.MHMBenefitID], function (d) {
													return d.user_id == userD.user_id
												})[0];
												if (found) {
													if (flag) {
														found.cost = found.cost;
														found.qty = found.qty;
														found.note = found.note;
													} else {
														found.cost = e1.UsageCost;
														found.qty = e1.UsageQty;
														found.note = e1.UsageNotes;
													}

													foundDisplay = $filter('filter')($scope.display, function (d) {
														return d.id === cat.MHMBenefitID
													})[0];
													foundDisplay.display = true;
													foundDefaultDisplay = $filter('filter')($scope.Defaults.display, function (d) {
														return d.id === cat.MHMBenefitID
													})[0];
													foundDisplay.display = true;

												}

												found1 = $filter('filter')($scope.Defaults.UseBenefits.users.benefitsVal[cat.MHMBenefitID], function (d) {
													return d.user_id == userD.user_id
												})[0];
												if (found1) {
													found1.cost = e1.UsageCost;
													found1.qty = e1.UsageQty;
													found1.note = e1.UsageNotes;
												}
											}
										})
										i++;
									})


							}
						}
						/** Restore Medical Usage after change county id */
						if ($scope.medicalUsagesCount) {
							window.setTimeout(function () {
								$scope.restoreDefault(false);
							})
						}
					};
					/**
						get User Detail.
					*/
					$scope.getUserDetail = function (i) {
						var display = i;
						var displayVal = i;
						var user_id = '';
						if (i == 1) {
							display = "Primary";
							user_id = "user_" + i;
							displayVal = "Primary";
						} else if ($scope.Case.UsageID == '2' && i == 2) {
							display = "spouse";
							user_id = "user_" + i;
						} else if ($scope.Case.UsageID == '3' && i == 2) {
							display = "child_" + (i - 1);
							user_id = "user_" + (i + 1);
						} else if ($scope.Case.UsageID == '4' && i == 2) {
							display = "spouse";
							user_id = "user_" + i;
						} else if ($scope.Case.UsageID == '4' && i == 3) {
							display = "child_" + (i - 2);
							user_id = "user_" + (i);
						} else if ($scope.Case.UsageID == '5' && i == 2) {
							display = "child_" + (i - 1);
							user_id = "user_" + (i + 1);
						} else {
							if ($scope.Case.UsageID == '4') {
								display = "child_" + (i - 2);
								user_id = "user_" + (i);
							} else {
								display = "child_" + (i - 1);
								user_id = "user_" + (i + 1);
							}
						}
						return ({
							display: display,
							displayVal: displayVal,
							user_id: user_id
						});
					}

					/**
						Case print and Save function.
					*/
					$scope.printFN = function (type, callback) {

						if (type == "save") {
							var Html = btoa(unescape(encodeURIComponent('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml"><body style="font-family: calibri !important;  font-size: 15px !important; color: rgb(51, 51, 51) !important;  margin: 0px auto !important;padding: 10px !important; ">' + $("#print_wizard").html() + '</body></html>')));
							console.log('html', $("#print_wizard").html());

							var xhr = new XMLHttpRequest();
							xhr.open('POST', messages.serverLiveHost + messages.pdfGeneration, true);
							xhr.responseType = 'arraybuffer';
							xhr.onload = function () {
								if (this.status === 200) {
									var filename = $scope.Case.CaseTitle + '.pdf';
									var disposition = xhr.getResponseHeader('Content-Disposition');
									if (disposition && disposition.indexOf('attachment') !== -1) {
										var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
										var matches = filenameRegex.exec(disposition);
									}
									var type = xhr.getResponseHeader('Content-Type');

									var blob = new Blob([this.response], {
										type: type
									});
									if (typeof window.navigator.msSaveBlob !== 'undefined') {
										window.navigator.msSaveBlob(blob, filename);
										$scope.formLoading = false;
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
										setTimeout(function () {
											URL.revokeObjectURL(downloadUrl);
										}, 100); // cleanup
										$scope.formLoading = false;
										$scope.$apply();
									}
								}
							};
							xhr.setRequestHeader('Content-type', 'application/json');
							xhr.send(JSON.stringify({
								Html: Html,
								CaseTitle: $scope.Case.CaseTitle
							}));

						} else if (type == "mail") {
							$scope.formLoading = true;
							$scope.dataLoading = true;

							var Html = btoa(unescape(encodeURIComponent('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html xmlns="http://www.w3.org/1999/xhtml"><body style="font-family: calibri !important;  font-size: 15px !important; color: rgb(51, 51, 51) !important;  margin: 0px auto !important;padding: 10px !important; ">' + $("#print_wizard").html() + '</body></html>')));

							var xhr = new XMLHttpRequest();
							xhr.open('post', messages.serverLiveHost + messages.sendMail, true);
							xhr.setRequestHeader('Content-type', 'application/json');

							var PlanTotalCostRange = 0;
							var EmployerHSAContribution = 0;
							var EmployerPremiumContribution = 0;
							var EmployerHRAReimbursement = 0;
							var TotalEmployerContribution = 0;
							var OptimalPlanName = '';

							if ($scope.Case.CasePlanResults.length > 0) {

								PlanTotalCostRange = $scope.Case.CasePlanResults[$scope.Case.CasePlanResults.length - 1].TotalPaid - $scope.Case.CasePlanResults[0].TotalPaid;
								EmployerHSAContribution = $scope.Case.CasePlanResults[0].ContributedToYourHSAAccount;
								EmployerPremiumContribution = $scope.Case.CasePlanResults[0].FederalSubsidy;
								EmployerHRAReimbursement = $scope.Case.CasePlanResults[0].HRAReimbursedAmt;
								TotalEmployerContribution = EmployerHSAContribution + EmployerPremiumContribution + EmployerHRAReimbursement;
								OptimalPlanName = $scope.Case.CasePlanResults[0].PlanName;
							}

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
								PlanTotalCostRange: PlanTotalCostRange,
								EmployerHSAContribution : EmployerHSAContribution,
								EmployerPremiumContribution : EmployerPremiumContribution,
								EmployerHRAReimbursement : EmployerHRAReimbursement,
								TotalEmployerContribution : TotalEmployerContribution,
								OptimalPlanName : OptimalPlanName
							}));

							xhr.onreadystatechange = function () {
								/*if (xhr.readyState==4 && xhr.status==200){					 
										 	
								   $scope.formLoading=false;	
								   $scope.$apply();	
								}*/

								if (xhr.readyState == 4) {
									if (xhr.status == 200) {
										if ($scope.Case.StatusId == "6" || $scope.Case.StatusId == "8")
										{
											$scope.Case.StatusId = "8";
										}
										else
										{
											$scope.Case.StatusId = "2";
										}
										$scope.formLoading = false;
										$scope.dataLoading = false;
										$scope.$apply();
									} else if (xhr.status === 400) {
										console.log(xhr.responseText)
									} else {
										console.log("Error", xhr.statusText);
									}

								}
							}
						}
						if (typeof callback != 'undefined' && typeof (callback) === "function") {
							callback();
						}
					}

					/**
						Case print and Save function.
					*/
					// $scope.InterviewDate = {};
					$scope.print = function () {

						var MaxWorstCase = Math.max.apply(Math, $scope.Case.CasePlanResults.map(function (item) {
							return item.WorstCase;
						})) + 3000;
						
						var MaxTotalCost = Math.max.apply(Math, $scope.Case.CasePlanResults.map(function (item) {
							return item.NetAnnualPremium + item.Medical
						})) + 3000;
						
						$scope.max = MaxTotalCost > MaxWorstCase ? MaxTotalCost : MaxWorstCase;
						// $scope.InterviewDate = $scope.Case.InterviewDate;

						if ($scope.ShowFinish && !$scope.readOnly) {
							bootbox.dialog({
								message: messages.confirmationCaseSaveMsg,
								title: "",
								buttons: {
									success: {
										label: "Yes",
										className: "btn-success",
										callback: function () {
											$scope.formLoading = true;
											$scope.createCase(true, function () {
												$scope.caseID = $scope.Case.CaseID;
												$scope.Case.CreatedDateTime = new Date();
												$("#print_wizard").show();
												$rootScope.updateGraph($("#holderHtml").find('d3-bars'), true, function () {
													$scope.printFN('save');
													$("#print_wizard").hide();
												})
												$scope.$apply();
											});
										}
									},
									danger: {
										label: "No",
										className: "btn-danger",
										callback: function () {
											$scope.formLoading = false;
											$scope.$apply();
										}
									}
								}
							});

						} else {
							if ($scope.CurrentPlan != '' && $scope.CurrentPlan != $scope.Case.CasePlanResults[0].PlanId) {
								var CurrentPlanResult = $filter('filter')($scope.Case.CasePlanResults, function (item) {
									return item.PlanId == $scope.CurrentPlan;
								});
								if (CurrentPlanResult.length > 0)
									$scope.DifferenceAmount = CurrentPlanResult[0].TotalPaid - $scope.Case.CasePlanResults[0].TotalPaid;
							}

							$scope.formLoading = true;
							$scope.caseID = $scope.Case.CaseID;
							$scope.Case.CreatedDateTime = new Date();
							$("#print_wizard").show();
							$rootScope.updateGraph($("#holderHtml").find('d3-bars'), true, function () {
								$scope.printFN('save');
								$("#print_wizard").hide();
							})
						}

					}

					/**
						Case close function and redirect to list cases page.
					*/
					$scope.close = function () {
						if ($scope.ShowFinish && IsChangesOccur != false) {
							bootbox.dialog({
								message: messages.confirmationCaseSaveMsg,
								title: "",
								buttons: {
									success: {
										label: "Yes",
										className: "btn-success",
										callback: function () {
											$scope.formLoading = true;
											$scope.createCase(true, function () {
												$location.path('cases');
												$scope.formLoading = false;
												$scope.$apply();
											});
										}
									},
									danger: {
										label: "No",
										className: "btn-danger",
										callback: function () {
											$location.path('cases');
											$scope.formLoading = false;
											$scope.$apply();
										}
									}
								}
							});

						} else {
							$scope.formLoading = false;
							$location.path('cases');
							$timeout(function () {
								$scope.$apply();
							}, 300)
						}

					}

					/**
						Case Mail function and redirect to list cases page.
					*/
					$scope.mail = function (callback) {
						if ($scope.ShowFinish && !$scope.readOnly) {
							bootbox.dialog({
								message: messages.confirmationCaseSaveMsg,
								title: "",
								buttons: {
									success: {
										label: "Yes",
										className: "btn-success",
										callback: function () {
											$scope.formLoading = true;
											$scope.createCase(true, function () {
												$scope.caseID = $scope.Case.CaseID;
												$("#print_wizard").show();
												$scope.Case.CreatedDateTime = new Date();
												$rootScope.updateGraph($("#holderHtml").find('d3-bars'), true, function () {
													$scope.printFN('mail', callback);
													$("#print_wizard").hide();
												})
												$scope.$apply();
											});
										}
									},
									danger: {
										label: "No",
										className: "btn-danger",
										callback: function () {
											$scope.formLoading = false;
											$scope.$apply();
										}
									}
								}
							});

						} else {
							$scope.formLoading = true;
							$scope.caseID = $scope.Case.CaseID;
							$("#print_wizard").show();
							$scope.Case.CreatedDateTime = new Date();
							$rootScope.updateGraph($("#holderHtml").find('d3-bars'), true, function () {
								$scope.printFN('mail', callback);
								$("#print_wizard").hide();
							})
						}
					}

					/**
						Update medical usage note while adding benefit.
					*/
					$scope.updateNote = function (benefit_note) {
						var BenefitText = $filter('filter')($scope.UseBenefits.users.benefitsVal[$scope.Bbenefit_id], function (d) {
							return d.user_id == $scope.Buser_id;
						});
						BenefitText[0].note = benefit_note;
					}

					/**
						Show pop up medical benefit.
					*/
					$scope.showPopUpNote = function () {
						$scope.closeAllModals();
						$scope.benefitV = {};
						$scope.modalInstance = ngDialog.open({
							template: 'note.html',
							scope: $scope,
							resolve: {
								updateNote: function () {
									return $scope.updateNote;
								}

							}
						});
					}

					/**
						Show pop up for more medical benefit plan.
					*/
					$scope.showPopUpMedical = function () {
						$scope.closeAllModals();
						$scope.benefitV = {};
						$scope.modalInstance = ngDialog.open({
							template: 'showBenefits.html',
							scope: $scope,
							resolve: {
								getBenefit: function () {
									return $scope.getBenefit;
								}
							}
						});
					}

					/**
						Show pop up for more Critical illness plan.
					*/
					$scope.showPopUpCriticalillness = function () {
						$scope.closeAllModals();
						$scope.modalInstance = ngDialog.open({
							template: 'showCriticalillness.html',
							scope: $scope,
							resolve: {
								getCriticalIllness: function () {
									return $scope.getCriticalIllness;
								}
							}
						});
					}

					/**
						Save benefit after adding from pop up.
					*/
					$scope.getBenefit = function () {
						var benefitVA = $.map($scope.benefitV, function (value, index) {
							return [{
								"value": value,
								"index": index
							}];
						});
						if (benefitVA.length > 0) {
							for (var j = 0; j < benefitVA.length; j++) {
								if (benefitVA[j].value) {
									var result = $.grep($scope.display, function (e) {
										if (e.id == benefitVA[j].index) {
											e.display = true;
										}
										return e.id == benefitVA[j].index;
									});
								}
							}
						}
					}

					/**
						Save get Critical Illness after adding from pop up.
					*/
					$scope.getCriticalIllness = function () {
						var CriticalIllnessResult = [];
						var result = $.grep($scope.criticals, function (e) {
							if (e.checked) {
								CriticalIllnessResult.push({
									'IllnessId': e.IllnessId
								});
							}
						});
						var criticalillnessVal = $filter('filter')($scope.UseBenefits.users.Criticalillnesses, function (d) {
							return d.user_id == $scope.Cuser_id;
						})[0];

						criticalillnessVal.val = CriticalIllnessResult;
					}

					/**
						show Medical Usage pop up.
					*/
					$scope.showMedicalUsage = function (e) {
						var cat_id = e.target.id;
						$scope.benefitName = $scope.category[cat_id];
						var result = $.grep($scope.display, function (e) {
							if (e.display != '1') {
								return e.cat_id == cat_id;
							}
						});
						$scope.benefits = result;
						$scope.showPopUpMedical();
					}

					/**
						show Critical illness pop up.
					*/
					$scope.showCriticalillness = function (e) {
						var user_id = e.target.id;
						$scope.Cuser_id = user_id;
						var CurrentIllness = [];
						var criticalillnessVal = $filter('filter')($scope.UseBenefits.users.Criticalillnesses, function (d) {
							return d.user_id == $scope.Cuser_id;
						})[0];

						$.grep(criticalillnessVal.val, function (e) {
							CurrentIllness.push(e.IllnessId);
						});

						var result = $.grep($scope.criticalillness, function (e) {
							e.checked = false;
							if (CurrentIllness.indexOf(e.IllnessId) !== -1) {
								e.checked = true;
							}
							return e;
						});
						$scope.criticals = result;
						$scope.showPopUpCriticalillness();
					}

					/**
						Add note function.
					*/
					$scope.addNotes = function (e) {
						$scope.benefitsA = (e.target.id).split("-");
						$scope.Bbenefit_id = $scope.benefitsA[0];
						$scope.Buser_id = $scope.benefitsA[1];
						var BenefitText = $filter('filter')($scope.UseBenefits.users.benefitsVal[$scope.Bbenefit_id], function (d) {
							return d.user_id == $scope.Buser_id;
						});
						$scope.benefit_note = BenefitText[0].note;
						$scope.showPopUpNote();
					}

					/**
						Remove medical benefit plan.
					*/
					$scope.removeRow = function (e) {
						var cat_id = e.target.id;
						var result = $.grep($scope.display, function (e) {
							if (e.id == cat_id) {
								e.display = false;
							}
							return e.id == cat_id;
						});
						$.grep($scope.UseBenefits.users.benefitsVal[cat_id], function (e) {
							e.qty = "";
							e.note = '';
							e.cost = e.defaultCost;
						});
						$scope.updateRow();
					}


					/**
						Remove Member user in plan.
					*/
					$scope.removeMemberColumnFn = function (user_id, isDisplay) {

						foundCounts = $filter('filter')($scope.Defaults.UseBenefits.users.counts, function (d) {
							return d.user_id == user_id
						})[0];
						if (isDisplay) {
							foundCounts.isDisplay = true;
							$scope.Defaults.UseBenefits.users.counts = $.grep($scope.Defaults.UseBenefits.users.counts, function (e) {
								return e.user_id != user_id;
							});
							$scope.Defaults.UseBenefits.users.dobs = $.grep($scope.Defaults.UseBenefits.users.dobs, function (e) {
								return e.user_id != user_id;
							});
							$scope.Defaults.UseBenefits.users.gender = $.grep($scope.Defaults.UseBenefits.users.gender, function (e) {
								return e.user_id != user_id;
							});
							$scope.Defaults.UseBenefits.users.smoking = $.grep($scope.Defaults.UseBenefits.users.smoking, function (e) {
								return e.user_id != user_id;
							});
							$scope.Defaults.UseBenefits.users.age = $.grep($scope.Defaults.UseBenefits.users.age, function (e) {
								return e.user_id != user_id;
							});
							$scope.Defaults.UseBenefits.users.Criticalillnesses = $.grep($scope.Defaults.UseBenefits.users.Criticalillnesses, function (e) {
								return e.user_id != user_id;
							});



							$scope.Defaults.UseBenefits.users.UseMedicalUsage = $.grep($scope.Defaults.UseBenefits.users.UseMedicalUsage, function (e) {
								return e.user_id != user_id;
							});
						}

						$scope.UseBenefits.users.counts = $.grep($scope.UseBenefits.users.counts, function (e) {
							return e.user_id != user_id;
						});

						$scope.UseBenefits.users.dobs = $.grep($scope.UseBenefits.users.dobs, function (e) {
							return e.user_id != user_id;
						});

						$scope.UseBenefits.users.gender = $.grep($scope.UseBenefits.users.gender, function (e) {
							return e.user_id != user_id;
						});


						$scope.UseBenefits.users.smoking = $.grep($scope.UseBenefits.users.smoking, function (e) {
							return e.user_id != user_id;
						});

						$scope.UseBenefits.users.age = $.grep($scope.UseBenefits.users.age, function (e) {
							return e.user_id != user_id;
						});
						$scope.UseBenefits.users.Criticalillnesses = $.grep($scope.UseBenefits.users.Criticalillnesses, function (e) {
							return e.user_id != user_id;
						});

						$scope.UseBenefits.users.UseMedicalUsage = $.grep($scope.UseBenefits.users.UseMedicalUsage, function (e) {
							return e.user_id != user_id;
						});
						for (var i = 0; i < $scope.display.length; i++) {
							if ($scope.display[i].benefit) {
								$scope.UseBenefits.users.benefitsVal[$scope.display[i].id] = $.grep($scope.UseBenefits.users.benefitsVal[$scope.display[i].id], function (e) {
									return e.user_id != user_id;
								});
								if (isDisplay) {
									$scope.Defaults.UseBenefits.users.benefitsVal[$scope.display[i].id] = $.grep($scope.Defaults.UseBenefits.users.benefitsVal[$scope.display[i].id], function (e) {
										return e.user_id != user_id;
									});
								}
							}
						}
						var i = 1;
						$scope.count = 1;
						$.grep($scope.UseBenefits.users.counts, function (e) {
							if (e.val != 'Primary') {
								$scope.count = $scope.count + 1;
								e.val = $scope.count;
							}
							return;
						});

					}

					/**
						Remove Member user in plan.
					*/
					$scope.removeMemberColumn = function (e, id, display) {

						if (display) {
							foundCounts = $filter('filter')($scope.UseBenefits.users.counts, function (d) {
								return d.display == display
							})[0];
							if (isEmpty(foundCounts)) {
								return true;
							}
							var user_id = foundCounts.user_id;
							$scope.removeMemberColumnFn(user_id, false);
							$scope.updateRow();
						} else {
							if (id) {
								var user_id = id;
							} else {
								var user_id = e.target.id;
							}
							if (id) {
								$scope.removeMemberColumnFn(user_id, false);
								$scope.updateRow();
							} else {
								bootbox.confirm(messages.confirmationMsg, function (response) {
									if (response) {
										$scope.removeMemberColumnFn(user_id, true);
										$scope.updateRow();
										$scope.$apply();
									}
								})
							}
						}

					}

					/**
						Restore to default values medical usage grid.
					*/
					$scope.restoreDefault = function (flag) {
						if (flag) {
							bootbox.confirm(messages.confirmationMsg, function (response) {
								if (response) {
									$scope.changeUsageCode(false);
									for (var i = 0; i < $scope.display.length; i++) {
										if ($scope.display[i].benefit) {
											$.grep($scope.UseBenefits.users.benefitsVal[$scope.display[i].id], function (e) {
												e.cost = e.defaultCost;
												e.qty = "";
												e.note = "";
											});
											$.grep($scope.Defaults.UseBenefits.users.benefitsVal[$scope.display[i].id], function (e) {
												e.cost = e.defaultCost;
												e.qty = "";
												e.note = "";
											});

										}
									}
									$scope.updateRow();
									$scope.$apply();
								}
							})
						} else {
							$scope.changeUsageCode(false);
							//no need to change cost with defaultcost as we implmented login through which cost will not be change on the change of ZipCode, JoNumber
							// for(var i=0;i<$scope.display.length;i++){
							// if($scope.display[i].benefit){						
							// $.grep($scope.UseBenefits.users.benefitsVal[$scope.display[i].id], function(e){
							// e.cost=e.defaultCost;
							// //e.qty="";
							// e.note="";	
							// });	
							// $.grep($scope.Defaults.UseBenefits.users.benefitsVal[$scope.display[i].id], function(e){
							// e.cost=e.defaultCost;
							// //e.qty="";
							// e.note="";	
							// });		

							// }			
							// }
							$scope.updateRow();
							$scope.$apply();
						}
					}

					$scope.quickViewPopUp = function () {


		var QuickViewData = {};
		QuickViewData.Families = [];
		$.grep($scope.UseBenefits.users.counts, function (e) {
			var BenefitsData = {};
			var user_id = e.user_id;
			$.grep($scope.UseBenefits.users.UseMedicalUsage, function (e) {
				if (e.user_id == user_id) {
					BenefitsData.TotalMedicalUsage = e.val;
					return;
				}
			});

			if (typeof BenefitsData.BenefitUserDetails == 'undefined') {
				BenefitsData.BenefitUserDetails = [];
			}


			for (var i = 0; i < $scope.display.length; i++) {
				if ($scope.display[i].benefit) {
					$.grep($scope.UseBenefits.users.benefitsVal[$scope.display[i].id], function (e) {
						if (e.user_id == user_id) {
							if (e.qty != '' && e.qty > 0 || (e.note != null && e.note != "")) {
								BenefitsData.BenefitUserDetails.push({
									"MHMMappingBenefitId": e.id,
									"UsageCost": e.cost,
									"UsageQty": e.qty,
									"UsageNotes": e.note,
									"UsageName": $scope.display[i].name,
								})
							}
						}
						return;
					});

				}
			}
			QuickViewData.Families.push(BenefitsData);
		});
		console.log(QuickViewData);


						$scope.closeAllModals();
						$scope.modalInstance = ngDialog.open({
							template: 'BenefitUsages.html',
							scope: $scope,
							data: QuickViewData,
							className: 'ngdialog-theme-default model-benefit',
							showClose: true,
						});
					}

					$scope.showquickView = function (e) {
						var user_id = e.target.id;
						$scope.Cuser_id = user_id;
						var CurrentIllness = [];
						var criticalillnessVal = $filter('filter')($scope.UseBenefits.users.Criticalillnesses, function (d) {
							return d.user_id == $scope.Cuser_id;
						})[0];

						$.grep(criticalillnessVal.val, function (e) {
							CurrentIllness.push(e.IllnessId);
						});

						var result = $.grep($scope.criticalillness, function (e) {
							e.checked = false;
							if (CurrentIllness.indexOf(e.IllnessId) !== -1) {
								e.checked = true;
							}
							return e;
						});
						$scope.SelectedBenefitUsages = result;
						$scope.quickViewPopUp();
					}

					/**
						find Last Child when add new member.
					*/
					$scope.findLastChild = function () {
						var flag = 0;
						var lastUser = '';
						$.grep($scope.Defaults.UseBenefits.users.counts, function (e) {
							if (!(e.display == 'Primary' || e.display == 'spouse' || e.display == 'child_1')) {
								var childID = (e.display).replace("child_", '');
								var currentData = $filter('filter')($scope.UseBenefits.users.counts, function (d) {
									return d.display == "child_" + childID
								})[0];
								if (isEmpty(currentData) && flag == 0) {
									flag = 1;
									lastUser = $filter('filter')($scope.Defaults.UseBenefits.users.counts, function (d) {
										return d.display == "child_" + childID
									})[0];
									return lastUser;
								}
							}
						});
						return lastUser;
					}

					/**
						add new member.
					*/
					$scope.addNewMember = function (display) {
						if ($scope.UseBenefits.users.counts.length + 1 <= messages.maxDefaultFamily) {
							$scope.totalCount = $scope.totalCount + 1;
							$scope.count = $scope.UseBenefits.users.counts.length + 1;
							console.log('display', display)
							if (display) {

								foundCounts = $filter('filter')($scope.Defaults.UseBenefits.users.counts, function (d) {
									return d.display == display
								})[0];

								foundDob = $filter('filter')($scope.Defaults.UseBenefits.users.dobs, function (d) {
									return d.user_id == foundCounts.user_id;
								})[0];
								if (foundDob.val == '') foundDob.val = '01/01/1900';

								foundGender = $filter('filter')($scope.Defaults.UseBenefits.users.gender, function (d) {
									return d.user_id == foundCounts.user_id;
								})[0];
								if (foundGender.val == '') foundGender.val = 'M';

								foundSmoking = $filter('filter')($scope.Defaults.UseBenefits.users.smoking, function (d) {
									return d.user_id == foundCounts.user_id;
								})[0];

								foundAge = $filter('filter')($scope.Defaults.UseBenefits.users.age, function (d) {
									return d.user_id == foundCounts.user_id;
								})[0];

								foundCriticalillnesses = $filter('filter')($scope.Defaults.UseBenefits.users.Criticalillnesses, function (d) {
									return d.user_id == foundCounts.user_id;
								})[0];

								foundUseMedicalUsage = $filter('filter')($scope.Defaults.UseBenefits.users.UseMedicalUsage, function (d) {
									return d.user_id == foundCounts.user_id;
								})[0];

								counts = {
									"val": $scope.count,
									"display": foundCounts.display,
									"isDelete": true,
									"user_id": foundCounts.user_id,
									"isDelete": foundCounts.isDelete,
									isDisplay: false
								};

								dobs = {
									"val": foundDob.val,
									"user_id": foundCounts.user_id
								};

								gender = {
									"val": foundGender.val,
									"user_id": foundCounts.user_id
								};

								smoking = {
									"val": foundSmoking.val,
									"user_id": foundCounts.user_id
								};

								age = {
									"val": foundAge.val,
									"user_id": foundCounts.user_id
								};

								Criticalillnesses = {
									"val": foundCriticalillnesses.val,
									"user_id": foundCriticalillnesses.user_id
								};

								UseMedicalUsage = {
									"val": foundUseMedicalUsage.val,
									"user_id": foundCounts.user_id
								};

								for (var i = 0; i < $scope.display.length; i++) {
									if ($scope.display[i].benefit) {
										found = $filter('filter')($scope.Defaults.UseBenefits.users.benefitsVal[$scope.display[i].id], function (d) {
											return d.user_id == foundCounts.user_id;
										})[0];
										if (!isEmpty(found)) {

											$scope.UseBenefits.users.benefitsVal[$scope.display[i].id].push({
												"id": $scope.display[i].id,
												"qty": found.qty,
												"cost": found.cost,
												"note": found.note,
												"user_id": found.user_id,
												"defaultCost": found.defaultCost
											});

										} else {

											$scope.UseBenefits.users.benefitsVal[$scope.display[i].id].push({
												"id": $scope.display[i].id,
												"qty": "",
												"cost": $scope.display[i].cost,
												"note": "",
												"user_id": foundCounts.user_id,
												"defaultCost": $scope.display[i].cost
											});

											$scope.Defaults.UseBenefits.users.benefitsVal[$scope.display[i].id].push({
												"id": $scope.display[i].id,
												"qty": "",
												"cost": $scope.display[i].cost,
												"note": "",
												"user_id": foundCounts.user_id,
												"defaultCost": $scope.display[i].cost
											});
										}
									}
								}
							} else {
								var lastUser = $scope.UseBenefits.users.counts[$scope.UseBenefits.users.counts.length - 1];
								var childID = (lastUser.display).replace("child_", '');
								var userID = (lastUser.user_id).replace("user_", '');

								foundCounts = $filter('filter')($scope.Defaults.UseBenefits.users.counts, function (d) {
									return d.display == "child_" + (parseInt(childID) + 1)
								})[0];

								if (!isEmpty(foundCounts)) {

									foundDob = $filter('filter')($scope.Defaults.UseBenefits.users.dobs, function (d) {
										return d.user_id == foundCounts.user_id;
									})[0];
									if (foundDob.val == '') foundDob.val = '01/01/1900';

									foundGender = $filter('filter')($scope.Defaults.UseBenefits.users.gender, function (d) {
										return d.user_id == foundCounts.user_id;
									})[0];
									if (foundGender) foundGender.val = 'M';

									foundSmoking = $filter('filter')($scope.Defaults.UseBenefits.users.smoking, function (d) {
										return d.user_id == foundCounts.user_id;
									})[0];

									foundAge = $filter('filter')($scope.Defaults.UseBenefits.users.age, function (d) {
										return d.user_id == foundCounts.user_id;
									})[0];

									foundAge = $filter('filter')($scope.Defaults.UseBenefits.users.Criticalillnesses, function (d) {
										return d.user_id == foundCounts.user_id;
									})[0];

									foundUseMedicalUsage = $filter('filter')($scope.Defaults.UseBenefits.users.UseMedicalUsage, function (d) {
										return d.user_id == foundCounts.user_id;
									})[0];

									counts = {
										"val": $scope.count,
										"display": foundCounts.display,
										"isDelete": true,
										"user_id": foundCounts.user_id,
										"isDelete": foundCounts.isDelete,
										isDisplay: false
									};

									dobs = {
										"val": foundDob.val,
										"user_id": foundCounts.user_id
									};

									gender = {
										"val": foundGender.val,
										"user_id": foundCounts.user_id
									};

									smoking = {
										"val": foundSmoking.val,
										"user_id": foundCounts.user_id
									};

									age = {
										"val": foundAge.val,
										"user_id": foundCounts.user_id
									};

									Criticalillnesses = {
										"val": foundAge.val,
										"user_id": foundCounts.user_id
									};

									UseMedicalUsage = {
										"val": foundUseMedicalUsage.val,
										"user_id": foundCounts.user_id
									};

									for (var i = 0; i < $scope.display.length; i++) {
										if ($scope.display[i].benefit) {

											found = $filter('filter')($scope.Defaults.UseBenefits.users.benefitsVal[$scope.display[i].id], function (d) {
												return d.user_id == foundCounts.user_id;
											})[0];

											if (!isEmpty(found)) {
												$scope.UseBenefits.users.benefitsVal[$scope.display[i].id].push({
													"id": $scope.display[i].id,
													"qty": found.qty,
													"cost": found.cost,
													"note": found.note,
													"user_id": found.user_id,
													"defaultCost": found.defaultCost
												});
											} else {
												$scope.UseBenefits.users.benefitsVal[$scope.display[i].id].push({
													"id": $scope.display[i].id,
													"qty": "",
													"cost": $scope.display[i].cost,
													"note": "",
													"user_id": foundCounts.user_id,
													"defaultCost": $scope.display[i].cost
												});
												$scope.Defaults.UseBenefits.users.benefitsVal[$scope.display[i].id].push({
													"id": $scope.display[i].id,
													"qty": "",
													"cost": $scope.display[i].cost,
													"note": "",
													"user_id": foundCounts.user_id,
													"defaultCost": $scope.display[i].cost
												});
											}


										}
									}
								} else {
									counts = {
										"val": $scope.count,
										"user_id": "user_" + (parseInt(userID) + 1),
										"display": "child_" + (parseInt(childID) + 1),
										"isDelete": true,
										isDisplay: false
									};

									dobs = {
										"val": "01/01/1900",
										"user_id": "user_" + (parseInt(userID) + 1)
									};

									gender = {
										"val": "M",
										"user_id": "user_" + (parseInt(userID) + 1)
									};

									smoking = {
										"val": "false",
										"user_id": "user_" + (parseInt(userID) + 1)
									};

									age = {
										"val": "",
										"user_id": "user_" + (parseInt(userID) + 1)
									};

									Criticalillnesses = {
										"val": [],
										"user_id": "user_" + (parseInt(userID) + 1)
									};

									UseMedicalUsage = {
										"val": 0,
										"user_id": "user_" + (parseInt(userID) + 1)
									};

									for (var i = 0; i < $scope.display.length; i++) {
										if ($scope.display[i].benefit) {
											$scope.UseBenefits.users.benefitsVal[$scope.display[i].id].push({
												"id": $scope.display[i].id,
												"qty": "",
												"cost": $scope.display[i].cost,
												"note": "",
												"user_id": "user_" + (parseInt(userID) + 1),
												"defaultCost": $scope.display[i].cost
											});
											$scope.Defaults.UseBenefits.users.benefitsVal[$scope.display[i].id].push({
												"id": $scope.display[i].id,
												"qty": "",
												"cost": $scope.display[i].cost,
												"note": "",
												"user_id": "user_" + (parseInt(userID) + 1),
												"defaultCost": $scope.display[i].cost
											});
										}
									}
									$scope.Defaults.UseBenefits.users.counts.push(counts);
									$scope.Defaults.UseBenefits.users.dobs.push(dobs);
									$scope.Defaults.UseBenefits.users.gender.push(gender);
									$scope.Defaults.UseBenefits.users.smoking.push(smoking);
									$scope.Defaults.UseBenefits.users.age.push(age);
									$scope.Defaults.UseBenefits.users.Criticalillnesses.push(Criticalillnesses);
									$scope.Defaults.UseBenefits.users.UseMedicalUsage.push(UseMedicalUsage);
								}
							}
							$scope.UseBenefits.users.counts.push(counts);
							$scope.UseBenefits.users.dobs.push(dobs);
							$scope.UseBenefits.users.gender.push(gender);
							$scope.UseBenefits.users.smoking.push(smoking);
							$scope.UseBenefits.users.age.push(age);
							$scope.UseBenefits.users.Criticalillnesses.push(Criticalillnesses);
							$scope.UseBenefits.users.UseMedicalUsage.push(UseMedicalUsage);
						} else {
							bootbox.alert(messages.NoFamily)
						}
					}

					/**
						Update benefit cost in medical usage grid.
					*/
					$scope.updateRow = function () {
						var total = [];
						for (var i = 0; i < $scope.display.length; i++) {
							if ($scope.display[i].benefit) {
								$.grep($scope.UseBenefits.users.benefitsVal[$scope.display[i].id], function (e) {
									if (!(e.qty != '' && e.qty > 0)) {
										e.cost = ($.isNumeric(e.defaultCost)) ? e.defaultCost : 0;
									} else {
										var qty = parseInt(e.qty);
										var cost = ($.isNumeric(e.cost)) ? parseFloat(e.cost) : 0;
										var user_id = e.user_id;
										$.grep($scope.UseBenefits.users.UseMedicalUsage, function (e) {
											if (e.user_id == user_id) {
												if (typeof total[e.user_id] == 'undefined') {
													total[e.user_id] = 0;
												}
												total[e.user_id] = parseFloat(total[e.user_id]) + (qty * cost);
												e.val = parseFloat(total[e.user_id]);
											}
											return;
										});
									}
									return;
								});
							}
						}
						$scope.Case.TotalMedicalUsage = 0;
						$.grep($scope.UseBenefits.users.UseMedicalUsage, function (e) {
							if (typeof total[e.user_id] == 'undefined') {
								e.val = 0;
							} else {
								$scope.Case.TotalMedicalUsage = $scope.Case.TotalMedicalUsage + total[e.user_id];
							}
						});
						//$scope.formLoading=false;
						if (!$scope.requestInProgress) {
							$scope.getTopInsurancePlan(0);
						}

					}

					/**
						validate DOB.
					*/
					$scope.validateDOB = function validateDOB(date1) {
						var dateF = new Date(date1);
						var isValid = !isNaN(dateF);
						if (!isValid) {
							bootbox.alert(messages.InvalidDOB);
							return;
						}
					}

					/**
						calculate Age.
					*/
					$scope.calculateAge = function calculateAge(date1) {
						var dateF = new Date(date1);
						var isValid = !isNaN(dateF);
						if (isValid && !isEmpty(date1)) {
							var d = new Date();
							var curr_date = d.getDate();
							var curr_month = d.getMonth() + 1;
							var curr_year = d.getFullYear();

							var bdays = dateF.getDate();
							var bmonths = dateF.getMonth() + 1;
							var byear = dateF.getFullYear();
							var sdays = curr_date;
							var smonths = curr_month;
							var syear = curr_year;

							var todayDate = new Date(),
								todayYear = todayDate.getFullYear(),
								todayMonth = todayDate.getMonth(),
								todayDay = todayDate.getDate(),
								age = todayYear - byear;

							if (todayMonth < bmonths - 1) {
								age--;
							}

							if (bmonths - 1 === todayMonth && todayDay < bdays) {
								age--;
							}
							if (age < 1) {
								age = 1;
							}
							return age;
						} else {
							return 0;
						}
					}

					/**
						Watch UseBenefits.users.dobs and calculate Age.
					*/
					$scope.$watch('UseBenefits.users.dobs', function (newValue) {
						$.grep($scope.UseBenefits.users.dobs, function (e) {
							var user_id = e.user_id;
							if (typeof e.val != 'undefined' && e.val != '') {
								var currentDOB = new Date(e.val);
							} else {
								var currentDOB = '';
							}
							$.grep($scope.UseBenefits.users.age, function (e) {
								if (user_id == e.user_id) {
									e.val = $scope.calculateAge(currentDOB);
								}
								return;
							});
						});
					});

					/**
						Watch UseBenefits.users.dobs and calculate Age.
					*/
					$scope.calAge = function (user_id) {
						var currentDOB = $filter('filter')($scope.UseBenefits.users.dobs, function (d) {
							return d.user_id == user_id
						})[0];
						if (typeof currentDOB != 'undefined' && currentDOB.val != '') {
							var currentAge = $filter('filter')($scope.UseBenefits.users.age, function (d) {
								return d.user_id == user_id
							})[0];
							currentAge.val = $scope.calculateAge(currentDOB.val);
							$scope.getTopInsurancePlan(0);
							$scope.getHSALimit();
							//write code for HSA Limit
						}
					};

					$scope.getHSALimit = function () {
						var i = 0;
						var MemberAges = [];
						$.grep($scope.UseBenefits.users.age, function (e) {
							MemberAges[i] = parseInt(e.val);
							i++;
						});
						if (!isEmpty($scope.Case.UsageID) && MemberAges.length > 0) {
							// var MaxMemberAge = Math.max.apply(null, MemberAges);
							var MaxMemberAge = MemberAges[0];
							if (typeof MaxMemberAge != "NaN" && MaxMemberAge > 0) {
								$scope.dataLoading = true;
								CaseService.GetHSALimit(MaxMemberAge, $scope.Case.UsageID, $scope.Case.Year, function (response) {
									if (response.Status) {
										$scope.Case.HSALimit = response.HSALimit;
										$scope.dataLoading = false;
									} else {
										bootbox.alert(response.Message);
										$scope.dataLoading = false;
									}
								});
							}
						}
					}

					/**
						Open calender pop up.
					*/
					$scope.open1 = function (e) {
						var user_id = e.target.id
						$.grep($scope.UseBenefits.users.dobs, function (e) {
							if (e.user_id == user_id) {
								e.isOpned = true;
							}
						});
					};

					$scope.format = 'MM/dd/yyyy';
					$scope.altInputFormats = ['M!/d!/yyyy'];

					$scope.popup1 = {
						opened: false
					};


					$scope.maxDate = new Date();

					$scope.open_start = function () {
						$scope.startDate.isOpned = true;
					};
					$scope.open_end = function () {
						$scope.endDate.isOpned = true;
					};


					/** Initialize parameter  */
					$scope.closeAllModals = function () {
						$modalStack.dismissAll();
					}

					/**
						Check if case id resent in url copy or create new case.
					*/
					if ($routeParams.id) {
						$scope.formLoading = true;
						$scope.CaseReferenceId = $routeParams.id;
						// Copied from Case # _____

						$scope.title = 'Edit Case';
						$scope.action = 'Update';
						CaseService.getCase($routeParams.id, function (response) {

							if (response.Status) {
								$scope.Case = response.Case;
								console.log($scope.Case);
								$scope.caseID = $scope.Case.CaseId;
								$scope.edit = true;
								$scope.Defaults.Case = response.Case;
								$scope.title = $scope.Case.CaseTitle;
								$scope.Case.CaseReferenceId = $scope.CaseReferenceId;
								$scope.Applicant = response.Case.Applicant;
								$scope.Case.StatusId = $scope.Case.StatusId.toString();
								$scope.Case.CaseStatus = $scope.Case.CaseStatusMst.StatusCode;
								$scope.Case.CaseAction = $routeParams.action;
								$scope.EmployerName = response.Case.Applicant.EmployerName;
								$scope.Case.TierIntention = $scope.Case.TierIntention ? $scope.Case.TierIntention.toString() : "";

								$scope.breadcrumb = true;
								$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li><a href="cases">Cases</a></li><li class="active">' + 'Case# ' + $scope.Case.CaseID + '</li>';
								if ($routeParams.action == 'copy') {
									$scope.Case.CaseSource = messages.caseSourceCopied + ' ' + $routeParams.id;
									$scope.Case.PrimaryCase = false;
									$scope.Case.AlternateCase = true;
									$scope.Case.CaseReferenceId = $routeParams.id;
									$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li><a href="cases">Cases</a></li><li class="active">' + 'Copy Case ' + '</li>';
								}
								$scope.Case.DedBalAvailDate = new Date($scope.Case.DedBalAvailDate);
								// Set interview Date Start
								if ($scope.Case.InterviewDate != null) {

									var intervidate = $scope.Case.InterviewDate.split("-");
									$scope.SelectedYear = parseInt(intervidate[0]);
									var interviewmonth = parseInt(intervidate[1]);
									$scope.SelectedMonth = interviewmonth.toString();
									// $scope.SelectedYear = $scope.Case.InterviewDate.getFullYear();
									var days = getDaysInMonth($scope.SelectedMonth, $scope.SelectedYear);
									$scope.InterviewDays = $.map($(Array(days)), function (val, i) {
										return i + 1;
									});
									$scope.SelectedDay = parseInt(intervidate[2]);

									$scope.InterviewDate = new Date($scope.Case.InterviewDate);
								}
								// Set interview Date End
								//if($scope.Applicant.EmployerId && $scope.Applicant.EmployerId!='null' && parseInt($scope.Applicant.EmployerId)!='NaN'){						
								$scope.Applicant.EmployerId = $scope.Applicant.EmployerId.toString();
								//}else{
								//	$scope.Applicant.EmployerId=messages.IndividualEmployerId;
								//}

								if ($scope.Applicant.InsuranceTypeId && $scope.Applicant.InsuranceTypeId != 'null' && parseInt($scope.Applicant.InsuranceTypeId) != 'NaN') {
									$scope.Applicant.InsuranceTypeId = $scope.Applicant.InsuranceTypeId.toString();
								} else {
									$scope.Applicant.InsuranceTypeId = '';
								}

								$scope.data.totalRecord = response.Case.CasePlanResults.length;
								$scope.graphResults = response.Case.CasePlanResults;

								$.grep(response.Case.CasePlanResults, function (e) {
									// $scope.data.post.push({"State":e.PlanName+" ("+e.Rank+")","Net Annual Premium":parseFloat(e.NetAnnualPremium),"Medical (net of HSA tax savings)":parseFloat(e.Medical)});
									//console.log(e.PlanName);
									$scope.data.post.push({
										"State": e.PlanName + '-' + e.PlanIdIndiv1,
										"Rank": e.Rank,
										"Your Net Annual Premium Contribution": parseFloat(e.NetAnnualPremium),
										"Your Net Out-of-Pocket Medical Costs": parseFloat(e.Medical)
									});
								});
								$scope.data.width = ($scope.graphResults.length * 162) + ($scope.graphResults.length * 32.5);
								if ($scope.Case.IssuerID && $scope.Case.IssuerID != 'null' && parseInt($scope.Case.IssuerID) != 'NaN') {
									var IssuerID = parseInt($scope.Case.IssuerID);
									$scope.Case.IssuerID = IssuerID.toString();
								} else {
									$scope.Case.IssuerID = "";
								}

								if ($scope.Case.PlanID && $scope.Case.PlanID != 'null' && parseInt($scope.Case.PlanID) != 'NaN') {
									var PlanID = parseInt($scope.Case.PlanID);
									$scope.Case.PlanID = PlanID.toString();
								} else {
									$scope.Case.PlanID = "";
								}

								if (!($scope.Case.SubsidyAmount && $scope.Case.SubsidyAmount != 'null' && parseInt($scope.Case.SubsidyAmount) != 'NaN')) {
									$scope.Case.SubsidyAmount = 0;
								}

								if ($scope.Case.Agent && $scope.Case.Agent != 'null' && parseInt($scope.Case.Agent) != 'NaN') {
									var AgentInvestigate = parseInt($scope.Case.Agent);
									$scope.Case.Agent = AgentInvestigate.toString();
								} else {
									$scope.Case.Agent = "";
								}

								if (!isEmpty($scope.Case.UsageID)) {
									if (typeof $scope.codes != 'undefined' && $scope.codes.length > 0) {
										found = $filter('filter')($scope.codes, function (d) {
											return d.UsagaId == $scope.Case.UsageID;
										})[0];
										if (found) {
											$scope.UsageType = found.UsageType;
										}
									}
								}

								$scope.CurrentPlanName = $scope.Applicant.CurrentPlan;
								if ($scope.Applicant.CurrentPlan != '' && $scope.Applicant.CurrentPlan != 'Waived' && $scope.Applicant.CurrentPlan != 'NewHire' && $scope.Case.CasePlanResults.length > 0) {
									$scope.CurrentPlan = $scope.Applicant.CurrentPlan;
									var CurrentPlanResult = $filter('filter')($scope.Case.CasePlanResults, function (item) {
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

								if($scope.Case.CasePlanResults.length === 1)
								{		
									$scope.pro_red = '70%';
									$scope.pro_green = '30%';
								}
								else if($scope.Case.CasePlanResults.length === 2)
								{		
									$scope.pro_red = '50%';
									$scope.pro_green = '25%';
								}
								else if($scope.Case.CasePlanResults.length === 3)
								{		
									$scope.pro_red = '40%';
									$scope.pro_green = '20%';
								}
								else if($scope.Case.CasePlanResults.length === 4)
								{		
									$scope.pro_red = '30%';
									$scope.pro_green = '17.5%';
								}

								else if($scope.Case.CasePlanResults.length === 5)
								{		
									$scope.pro_red = '30%';
									$scope.pro_green = '14%';
								}


								var MaxWorstCase = Math.max.apply(Math, $scope.Case.CasePlanResults.map(function (item) {
									return item.WorstCase;
								})) + 3000;
								
								var MaxTotalCost = Math.max.apply(Math, $scope.Case.CasePlanResults.map(function (item) {
									return item.NetAnnualPremium + item.Medical
								})) + 3000;
								
								$scope.max = MaxTotalCost > MaxWorstCase ? MaxTotalCost : MaxWorstCase;

								if ($routeParams.action == 'copy' && $scope.statusCode != undefined) {
									var objCaseStatus = $filter('filter')($scope.statusCode, function (item) {
										return item.StatusId == $scope.Case.StatusId;
									});
									if (objCaseStatus[0].StatusCode == 'Final - Sent') {
										var FinalNotSentStatusId = $filter('filter')($scope.statusCode, function (item) {
											return item.StatusCode == 'Final - Not Sent';
										});
									$scope.Case.StatusId = FinalNotSentStatusId[0].StatusId.toString();
									$scope.Case.CaseStatus = FinalNotSentStatusId[0].StatusCode;
									}
								}

								$scope.getCarrier();
								//$scope.getEmployerList();						
								$scope.Case.UsageID = $scope.Case.UsageID.toString();
								$scope.getMedicalUsages();
								//$scope.getJobNumberList();
								$scope.getPlanInfo($scope.Case.JobNumber, 0);

								$scope.getZipCodeState();

								var i = 1;
								$scope.count = $scope.Case.Families.length;
								$.grep($scope.Case.Families, function (e) {
									var display = i;
									var displayVal = i;
									var user_id = '';

									if (i == 1) {
										display = "Primary";
										user_id = "user_" + i;
										displayVal = "Primary";
									} else if ($scope.Case.UsageID == '2' && i == 2) {
										display = "spouse";
										user_id = "user_" + i;
									} else if ($scope.Case.UsageID == '3' && i == 2) {
										display = "child_" + (i - 1);
										user_id = "user_" + (i + 1);
									} else if ($scope.Case.UsageID == '4' && i == 2) {
										display = "spouse";
										user_id = "user_" + i;
									} else if ($scope.Case.UsageID == '4' && i == 3) {
										display = "child_" + (i - 2);
										user_id = "user_" + (i);
									} else if ($scope.Case.UsageID == '5' && i == 2) {
										display = "child_" + (i - 1);
										user_id = "user_" + (i + 1);
									} else {
										if ($scope.Case.UsageID == '4') {
											display = "child_" + (i - 2);
											user_id = "user_" + (i);
										} else {
											display = "child_" + (i - 1);
											user_id = "user_" + (i + 1);
										}
									}


									found = $filter('filter')($scope.Defaults.UseBenefits.users.counts, function (d) {
										return d.user_id == user_id
									})[0];
									if (found) {
										found1 = $filter('filter')($scope.UseBenefits.users.counts, function (d) {
											return d.user_id == user_id
										})[0];
										if (!found1) {
											$scope.UseBenefits.users.counts.push({
												"val": displayVal,
												"user_id": user_id,
												"display": display,
												"isDelete": true,
												isDisplay: false
											});
										}
									} else {
										$scope.UseBenefits.users.counts.push({
											"val": displayVal,
											"user_id": user_id,
											"display": display,
											"isDelete": true,
											isDisplay: false
										});
										$scope.Defaults.UseBenefits.users.counts.push({
											"val": displayVal,
											"user_id": user_id,
											"display": display,
											"isDelete": true,
											isDisplay: false
										});
									}

									found = $filter('filter')($scope.Defaults.UseBenefits.users.age, function (d) {
										return d.user_id == user_id
									})[0];
									if (found) {
										found.val = e.Age;
										var found1 = $filter('filter')($scope.UseBenefits.users.age, function (d) {
											return d.user_id == user_id
										})[0];
										if (found1) {
											found1.val = e.Age;
										} else {
											$scope.UseBenefits.users.age.push({
												"val": e.Age,
												"user_id": user_id
											});
										}
									} else {
										$scope.UseBenefits.users.age.push({
											"val": e.Age,
											"user_id": user_id
										});
										$scope.Defaults.UseBenefits.users.age.push({
											"val": e.Age,
											"user_id": user_id
										});
									}

									found = $filter('filter')($scope.Defaults.UseBenefits.users.Criticalillnesses, function (d) {
										return d.user_id == user_id
									})[0];
									if (found) {
										if (e.Criticalillnesses == "") {
											e.Criticalillnesses = [];
										}
										found.val = e.Criticalillnesses;
										var found1 = $filter('filter')($scope.UseBenefits.users.Criticalillnesses, function (d) {
											return d.user_id == user_id
										})[0];
										if (found1) {
											if (e.Criticalillnesses == "") {
												e.Criticalillnesses = [];
											}
											found1.val = e.Criticalillnesses;
										} else {
											$scope.UseBenefits.users.Criticalillnesses.push({
												"val": e.Criticalillnesses,
												"user_id": user_id
											});
										}
									} else {
										$scope.UseBenefits.users.Criticalillnesses.push({
											"val": e.Criticalillnesses,
											"user_id": user_id
										});
										$scope.Defaults.UseBenefits.users.Criticalillnesses.push({
											"val": e.Criticalillnesses,
											"user_id": user_id
										});
									}

									found = $filter('filter')($scope.Defaults.UseBenefits.users.dobs, function (d) {
										return d.user_id == user_id
									})[0];
									if (found) {
										found.val = e.DOB;
										var found1 = $filter('filter')($scope.UseBenefits.users.dobs, function (d) {
											return d.user_id == user_id
										})[0];
										if (found1) {
											found1.val = e.DOB;
										} else {
											$scope.UseBenefits.users.dobs.push({
												"val": e.DOB,
												"user_id": user_id
											});
										}
									} else {
										$scope.UseBenefits.users.dobs.push({
											"val": e.DOB,
											"user_id": user_id
										});
										$scope.Defaults.UseBenefits.users.dobs.push({
											"val": e.DOB,
											"user_id": user_id
										});
									}

									found = $filter('filter')($scope.Defaults.UseBenefits.users.gender, function (d) {
										return d.user_id == user_id
									})[0];
									if (found) {
										found.val = e.Gender;
										var found1 = $filter('filter')($scope.UseBenefits.users.gender, function (d) {
											return d.user_id == user_id
										})[0];
										if (found1) {
											found1.val = e.Gender;
										} else {
											$scope.UseBenefits.users.gender.push({
												"val": e.Gender,
												"user_id": user_id
											});
										}
									} else {
										$scope.UseBenefits.users.gender.push({
											"val": e.Gender,
											"user_id": user_id
										});
										$scope.Defaults.UseBenefits.users.gender.push({
											"val": e.Gender,
											"user_id": user_id
										});
									}
									found = $filter('filter')($scope.Defaults.UseBenefits.users.smoking, function (d) {
										return d.user_id == user_id;
									})[0];
									if (found) {
										found.val = e.Smoking.toString();
										var found1 = $filter('filter')($scope.UseBenefits.users.smoking, function (d) {
											return d.user_id === user_id
										})[0];
										if (found1) {
											found1.val = e.Smoking.toString();
										} else {
											$scope.UseBenefits.users.smoking.push({
												"val": e.Smoking.toString(),
												"user_id": user_id
											});
										}
									} else {
										$scope.UseBenefits.users.smoking.push({
											"val": e.Smoking.toString(),
											"user_id": user_id
										});
										$scope.Defaults.UseBenefits.users.smoking.push({
											"val": e.Smoking.toString(),
											"user_id": user_id
										});
									}

									found = $filter('filter')($scope.Defaults.UseBenefits.users.UseMedicalUsage, function (d) {
										return d.user_id == user_id
									})[0];
									if (found) {
										found.val = e.TotalMedicalUsage;
										var found1 = $filter('filter')($scope.UseBenefits.users.UseMedicalUsage, function (d) {
											return d.user_id == user_id
										})[0];
										if (found1) {
											found1.val = e.TotalMedicalUsage;
										} else {
											$scope.UseBenefits.users.UseMedicalUsage.push({
												"val": e.TotalMedicalUsage,
												"user_id": user_id
											});
										}
									} else {
										$scope.UseBenefits.users.UseMedicalUsage.push({
											"val": e.TotalMedicalUsage,
											"user_id": user_id
										});
										$scope.Defaults.UseBenefits.users.UseMedicalUsage.push({
											"val": e.TotalMedicalUsage,
											"user_id": user_id
										});
									}
									i++;
								});
								$scope.formLoading = false;
							} else {
								bootbox.alert(response.Message, function () {
									$location.path('cases');
									$scope.formLoading = false;
									$scope.$apply();
								})

							}

						});

					}

					/**
						Case Save API.
					*/
					$scope.createCase = function (flag, callback) {
						$scope.formLoading = true;
						var data = {};
						var action = 0;
						$scope.Applicant.Createdby = $scope.customer.id;
						$scope.Case.Createdby = $scope.customer.id;
						data = $scope.Case;
						if ($scope.SelectedYear != null && $scope.SelectedMonth!=null) {
							$scope.Case.InterviewDate = $scope.SelectedYear + "-" + parseInt($scope.SelectedMonth) + "-" + $scope.SelectedDay;
							$scope.InterviewDate = new Date($scope.SelectedYear, parseInt($scope.SelectedMonth-1), $scope.SelectedDay);
						} else {
							$scope.Case.InterviewDate = null;
						}
						if ($scope.Formaction == 'copy') {
							//data.CaseID='';					
						}

						if ($scope.Formaction === 'update') {
							action = 1;
						}

						data.Families = [];
						data.Applicant = $scope.Applicant;

						$.grep($scope.UseBenefits.users.counts, function (e) {
							var BenefitsData = {};
							var user_id = e.user_id;
							$.grep($scope.UseBenefits.users.gender, function (e) {
								if (e.user_id == user_id) {
									BenefitsData.Gender = e.val
									return;
								}
							});
							$.grep($scope.UseBenefits.users.dobs, function (e) {
								if (e.user_id == user_id) {
									BenefitsData.DOB = $filter('date')(new Date(e.val), 'MM/dd/yyyy');
									return;
								}
							});
							$.grep($scope.UseBenefits.users.smoking, function (e) {
								if (e.user_id == user_id) {
									BenefitsData.Smoking = e.val;
									return;
								}
							});
							$.grep($scope.UseBenefits.users.age, function (e) {
								if (e.user_id == user_id) {
									BenefitsData.Age = e.val;
									return;
								}
							});
							$.grep($scope.UseBenefits.users.Criticalillnesses, function (e) {
								if (e.user_id == user_id) {
									if (e.val == "") {
										e.val = [];
									}
									BenefitsData.Criticalillnesses = e.val;
									return;
								}
							});

							$.grep($scope.UseBenefits.users.UseMedicalUsage, function (e) {
								if (e.user_id == user_id) {
									BenefitsData.TotalMedicalUsage = e.val;
									return;
								}
							});

							if (e.val == 'Primary') {
								BenefitsData.IsPrimary = true;
							} else {
								BenefitsData.IsPrimary = false;
							}
							if (typeof BenefitsData.BenefitUserDetails == 'undefined') {
								BenefitsData.BenefitUserDetails = [];
							}


							for (var i = 0; i < $scope.display.length; i++) {
								if ($scope.display[i].benefit) {
									$.grep($scope.UseBenefits.users.benefitsVal[$scope.display[i].id], function (e) {
										if (e.user_id == user_id) {
											if (e.qty != '' && e.qty > 0 || (e.note != null && e.note != "")) {
												BenefitsData.BenefitUserDetails.push({
													"MHMMappingBenefitId": e.id,
													"UsageCost": e.cost,
													"UsageQty": e.qty,
													"UsageNotes": e.note
												})
											}
										}
										return;
									});

								}
							}
							data.Families.push(BenefitsData);
						});


						CaseService.update(data, action, function (response) {
							if (response.Status) {
								$scope.ShowFinish = false;
								console.log( response);
								$scope.caseID = response.CaseId;
								$scope.Case.CaseID = $scope.caseID;
								bootbox.alert(messages.saved, function () {
									if ($scope.graphResults.length > 0) {
										if (!flag) {
											bootbox.dialog({
												message: messages.MailSend,
												title: "",
												buttons: {
													success: {
														label: "Yes",
														className: "btn-success",
														callback: function () {
															$scope.mail(function () {
																//$location.path('cases');
																$scope.formLoading = false;
																$scope.$apply();
																if (typeof callback != 'undefined' && typeof (callback) === "function") {
																	callback();
																}
															});
														}
													},
													danger: {
														label: "No",
														className: "btn-danger",
														callback: function () {
															//$location.path('cases');	
															$scope.formLoading = false;
															$scope.$apply();
															if (typeof callback != 'undefined' && typeof (callback) === "function") {
																callback();
															}
														}
													}
												}
											});
										} else {
											if (typeof callback != 'undefined' && typeof (callback) === "function") {
												callback();
											}
										}
									} else {
										$scope.formLoading = false;
										//$location.path('cases');
										$scope.$apply();
									}
								})
							} else {
								if (response.redirect) {
									bootbox.alert(messages.TryLater + ' Save Failed', function () {
										// $location.path('/');
										$scope.$apply();
									})
								}
								$scope.flash = {};
								$scope.flash.message = response.Message;
								$scope.flash.status = true;
								$scope.flash.type = 'alert-danger';
								$scope.dataLoading = false;
								$scope.formLoading = false;
								$("html").animate({
									scrollTop: 0
								}, 500);
							}
						});
					}

					/**
						Created By : Aastha Jain
						Created Date : 14-06-2016
						Start : Case Save on MEDICAL USAGE PAGE.
					*/

					$scope.saveChangesOnClose = function () {
						if ($scope.Formaction != 'view' && IsChangesOccur != false) {
							bootbox.dialog({
								message: messages.confirmationCaseSaveMsg,
								title: "",
								buttons: {
									success: {
										label: "Yes",
										className: "btn-success",
										callback: function () {
											if ($("#caseAdd").valid()) {
												$scope.getTopInsurancePlan(1);
												$scope.createCase(true, function () {
													$scope.formLoading = false;
													$location.path('cases');
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
											$location.path('cases');
											$scope.$apply();
										}
									}
								}
							});

						} else {
							$scope.formLoading = false;
							$location.path('cases');
							$timeout(function () {
								$scope.$apply();
							}, 300)
						}
					}

					$scope.tempSaveChanges = function () {
						var isStepValid = false;
						if (!$("#caseAdd").valid()) {
							//$("#caseAdd").submit();
							isStepValid = false;
						} else {
							if (!$scope.isValidFirstStep() && !$scope.readOnly) {
								bootbox.alert(messages.ErrorForm);
								isStepValid = false;
							} else {
								isStepValid = true;
							}
						}
						console.log('isStepValid', isStepValid)
						if (!isStepValid) {
							return;
						}
						if ($scope.SelectedYear != null && $scope.SelectedMonth!=null) {
							$scope.Case.InterviewDate = $scope.SelectedYear + "-" + parseInt($scope.SelectedMonth) + "-" + $scope.SelectedDay;
							$scope.InterviewDate = new Date($scope.SelectedYear, parseInt($scope.SelectedMonth-1), $scope.SelectedDay);
						} else {
							$scope.Case.InterviewDate = null;
						}
						$scope.formLoading = true;
						var data = {};
						var action = 0;
						$scope.Applicant.Createdby = $scope.customer.id;
						$scope.Case.Createdby = $scope.customer.id;
						data = $scope.Case;

						if ($scope.Formaction == 'copy') {
							//data.CaseID='';					
						}

						if ($scope.Formaction === 'update') {
							action = 1;
						}

						data.Families = [];
						data.Applicant = $scope.Applicant;

						$.grep($scope.UseBenefits.users.counts, function (e) {
							var BenefitsData = {};
							var user_id = e.user_id;
							$.grep($scope.UseBenefits.users.gender, function (e) {
								if (e.user_id == user_id) {
									BenefitsData.Gender = e.val
									return;
								}
							});
							$.grep($scope.UseBenefits.users.dobs, function (e) {
								if (e.user_id == user_id) {
									BenefitsData.DOB = $filter('date')(new Date(e.val), 'MM/dd/yyyy');
									return;
								}
							});
							$.grep($scope.UseBenefits.users.smoking, function (e) {
								if (e.user_id == user_id) {
									BenefitsData.Smoking = e.val;
									return;
								}
							});
							$.grep($scope.UseBenefits.users.age, function (e) {
								if (e.user_id == user_id) {
									BenefitsData.Age = e.val;
									return;
								}
							});
							$.grep($scope.UseBenefits.users.Criticalillnesses, function (e) {
								if (e.user_id == user_id) {
									if (e.val == "") {
										e.val = [];
									}
									BenefitsData.Criticalillnesses = e.val;
									return;
								}
							});

							$.grep($scope.UseBenefits.users.UseMedicalUsage, function (e) {
								if (e.user_id == user_id) {
									BenefitsData.TotalMedicalUsage = e.val;
									return;
								}
							});

							if (e.val == 'Primary') {
								BenefitsData.IsPrimary = true;
							} else {
								BenefitsData.IsPrimary = false;
							}
							if (typeof BenefitsData.BenefitUserDetails == 'undefined') {
								BenefitsData.BenefitUserDetails = [];
							}

							console.log('$scope.display', $scope.display);
							if ($scope.display != undefined) {
								for (var i = 0; i < $scope.display.length; i++) {
									if ($scope.display[i].benefit) {
										$.grep($scope.UseBenefits.users.benefitsVal[$scope.display[i].id], function (e) {
											if (e.user_id == user_id) {
												if (e.qty != '' && e.qty > 0 || (e.note != null && e.note != "")) {
													BenefitsData.BenefitUserDetails.push({
														"MHMMappingBenefitId": e.id,
														"UsageCost": e.cost,
														"UsageQty": e.qty,
														"UsageNotes": e.note
													})
												}
											}
											return;
										});

									}
								}
							}
							data.Families.push(BenefitsData);
						});

						CaseService.update(data, action, function (response) {
							if (response.Status) {
								$scope.caseID = response.CaseId;
								$scope.Case.CaseID = $scope.caseID;

								$scope.formLoading = false;

								if (response.CaseId === undefined) {
									$scope.flash = {};
									$scope.flash.message = response.Message;
									$scope.flash.status = true;
									$scope.flash.type = 'alert-danger';
									$("html").animate({
										scrollTop: 0
									}, 500);
								} else {
									bootbox.alert(messages.saved, function () {

										if (action === 0) {
											$location.path('editCase/false/' + $scope.caseID + '/update');
										}
										$scope.$apply();
									})
								}
							} else {
								$scope.flash = {};
								$scope.flash.message = response.Message;
								$scope.flash.status = true;
								$scope.flash.type = 'alert-danger';
								$scope.dataLoading = false;
								$scope.formLoading = false;
								$("html").animate({
									scrollTop: 0
								}, 500);
							}
						});

						// if ($("#caseAdd").valid()) {
						// $scope.getTopInsurancePlan(1);
						// $scope.createCase(true, function () {
						// $scope.formLoading = false;
						// $scope.$apply();
						// });
						// } else {
						// $scope.formLoading = false;
						// $scope.$apply();
						// }

					}

					/*****
						Created By : Aastha Jain
						Created Date : 18-07-2016
						Start : Function to get list of job number based on employer id
					*****/

					// $scope.getJobNumberList = function() {
					// $scope.dataLoading=true;			
					// $scope.jobNos=[];	
					// //if(!isEmpty($scope.Case.Year) && !isEmpty($scope.Applicant.EmployerId)){
					// $scope.requestInProgress=true;
					// CaseService.getJobNumberList(function(response){				
					// if(response.Status == true){
					// if(response.JobMasters.length > 0) {
					// $scope.jobNos=response.JobMasters;					
					// $scope.dataLoading=false;
					// $scope.requestInProgress=false;	
					// } else {
					// $scope.dataLoading=false;	
					// $scope.requestInProgress=false;
					// $scope.jobNos=[];
					// }
					// }else{					
					// $scope.dataLoading=false;	
					// $scope.requestInProgress=false;
					// $scope.jobNos=[];
					// }		
					// })
					// //}else{
					// //	$scope.dataLoading=false;	
					// //}

					// }

					$scope.updateJobNo = function () {
						$scope.dataLoading = true;
						$scope.Applicant.InsuranceTypeId = '';
						if (!isEmpty($scope.Case.JobNumber)) {
							$scope.requestInProgress = true;

							//$scope.getemployer();
							var job = $filter('filter')($scope.jobNos, {
								JobNumber: $scope.Case.JobNumber.toString()
							})[0];
							$scope.Case.Year = job.JobYear;
							$scope.Case.JobMaster = job;
							console.log('$scope.Case.JobMaster');
							console.log($scope.Case.JobMaster);
							$scope.Applicant.EmployerId = job.EmployerId;
							$scope.EmployerName = job.EmployerName;
							if ($scope.Applicant.EmployerId != messages.IndividualEmployerId && $scope.Applicant.EmployerId != messages.ShopEmployerId) {
								$scope.getInsuranceType(job.InsuranceTypeId);
								$scope.Case.ZipCode = job.CaseZipCode;
							} else {

								$scope.InsuranceName = '';
							}
							$scope.getMedicalUsages();
							$scope.getCarrier();
							$scope.getPlanInfo($scope.Case.JobNumber, 1);

							$scope.requestInProgress = false;
							$scope.dataLoading = false;

						} else {
							$scope.dataLoading = false;
						}
					}

					$scope.allPlans = [];

					$scope.getPlanInfo = function (JobNum, callFrom) {
						var data = {
							jobNo: JobNum
						};
						if(callFrom == 1) $scope.Applicant.CurrentPlan = '';
						$scope.dataLoading = true;
						CaseService.GetJobPlans(data, function (response) {
							if (response != null) {
								if (response.Status) {
									$scope.allPlans = response.SelectedPlans;
								}
							}

							$scope.dataLoading = false;
						});
					}

					/**** End : Function to get list of job number based on employer id ****/

					$scope.createCaseTitle = function () {
						$scope.Case.CaseTitle = $scope.Applicant.LastName + ' ' + $scope.Applicant.FirstName.charAt(0);
					}

					/** End : Case Save on MEDICAL USAGE PAGE. **/

					$scope.breadcrumb = true;
					$scope.breadcrumbHTML = '<li><a href="">Home</a></li><li><a href="cases">Cases</a></li><li class="active">' + $scope.title + '</li>';

					$scope.$on('$includeContentLoaded', function (event) {

						$("#print_wizard").hide();
						$('#scroll-area').niceScroll({
							autohidemode: 'false', // Do not hide scrollbar when mouse out
							cursorborderradius: '12px', // Scroll cursor radius
							background: '#ffffff', // The scrollbar rail color
							cursorwidth: '5px', // Scroll cursor width
							cursorcolor: '#3b84f2' // Scroll cursor color
						});


						$('body').on('click', function (e) {
							var target = $(event.target),
								$parent = target.parents('.bootstrap-select');

							if ($parent.length) {
								e.stopPropagation();
								$parent.toggleClass('open');
							} else {
								$('.bootstrap-select').removeClass('open');
							}
						});

						$scope.FinalGraphWidth = $("#tp-wizard-form").innerWidth();
						if ($routeParams.id) {
							$('#wizard').smartWizard({
								onLeaveStep: leaveAStepCallback,
								onFinish: onFinishCallback,
								keyNavigation: false,
								enableAllSteps: true,
								onShowStep: onShowStepCallback
							});
						} else {
							$('#wizard').smartWizard({
								onLeaveStep: leaveAStepCallback,
								onFinish: onFinishCallback,
								keyNavigation: false,
								enableAllSteps: false,
								onShowStep: onShowStepCallback
							});
						}

						function onFinishCallback(objs, context) {
							$scope.createCase(false);
							return false;

							$('#wizard').smartWizard('showMessage', 'Finish Clicked');
							if (validateAllSteps()) {
								$("#caseAdd").submit();
							}
						}

						function onShowStepCallback(obj, context) {
							if (context.toStep == 3) {
								$scope.Case.CreatedDateTime = new Date();
								if($scope.Case.CasePlanResults.length > 0 && $rootScope.updateGraph != undefined)
									$rootScope.updateGraph($("#step-3").find('d3-bars'), false);
							}
							if (context.toStep == 2) {
								$scope.Case.CreatedDateTime = new Date();
								// 	console.log($rootScope.updateGraph);
								// 	if($rootScope.updateGraph != undefined){
								// 	$rootScope.updateGraph($("#step-2").find('d3-bars'), false, function(){
								// 		console.log('draw');
								// 	});
								// }
								$('#holder').niceScroll({
									autohidemode: 'false', // Do not hide scrollbar when mouse out
									cursorborderradius: '12px', // Scroll cursor radius
									background: '#ffffff', // The scrollbar rail color
									cursorwidth: '5px', // Scroll cursor width
									cursorcolor: '#3b84f2' // Scroll cursor color
								});
								$scope.getHSALimit();
							}
							if (context.toStep == 3) {
								$('.buttonFinish').show();
								if ($scope.readOnly) {
									$('.buttonFinish').hide();
								}
							} else {
								$('.buttonFinish').hide();
							}
						}

						function leaveAStepCallback(obj, context) {
							if (!$scope.ShowFinish) {
								return false;
							}
							if (context.fromStep < context.toStep) {
								var res = validateSteps(context.fromStep);
								if (res) {
									if (context.toStep == 3 && !$scope.readOnly) {
										return $scope.getTopInsurancePlan(1);
									}
									return true;
								} else {
									return res; // return false to stay on step and true to continue navigation 
								}

							} else {
								return true;
							}

						}

						// Your Step validation logic
						function validateSteps(stepnumber) {
							var isStepValid = true;
							// validate step 1
							if (stepnumber == 1) {
								if (!$("#caseAdd").valid()) {
									$("#caseAdd").submit();
									isStepValid = false;
								} else {
									if (!$scope.isValidFirstStep() && !$scope.readOnly) {
										bootbox.alert(messages.ErrorForm);
										isStepValid = false;
									} else {
										isStepValid = true;
									}
								}
							}
							if (stepnumber == 2) {
								if (!$("#caseAdd").valid()) {
									$("#caseAdd").submit();
									isStepValid = false;
								} else {
									if (!($scope.isValidFirstStep()) && !$scope.readOnly) {
										bootbox.alert(messages.ErrorFormMedical);
										isStepValid = false;
									} else if (!($scope.isValidSecondStep()) && !$scope.readOnly) {
										bootbox.alert(messages.ErrorFormMedical);
										isStepValid = false;
									} else if (!($scope.isValidMedicalStep()) && !$scope.readOnly) {
										bootbox.alert(messages.ErrorFormMedical);
										isStepValid = false;
									} else {
										isStepValid = true;
									}

								}
							}
							if (stepnumber == 3) {
								if (!$("#caseAdd").valid()) {
									$("#caseAdd").submit();
									isStepValid = false;
								} else {
									isStepValid = true;
								}
							}
							return isStepValid;

						}

						function validateAllSteps() {
							var isStepValid = true;
							// all step validation logic     
							return isStepValid;
						}

						jQuery.validator.addMethod("phone", function (value, element) {
							return this.optional(element) || /^\(?(\d{3})\)?[ .-]?(\d{3})[ .-]?(\d{4})$/.test(value);
						}, "This is not a valid phone number.");

						jQuery.validator.addMethod("EmailFormat", function (value, element) {
							return this.optional(element) || /^[a-z]+[a-z0-9._-]+@[a-z]+\.[a-z.]{2,5}$/.test(value);
						}, "Invalid email.");

						jQuery.validator.addMethod("TaxRateFormat", function (value, element) {
							return this.optional(element) || /^[0-9]\d*(\.\d+)?$/.test(value);
						}, "Invalid Tax Rate.");

						jQuery.validator.addMethod("HSAFormat", function (value, element) {
							return this.optional(element) || /^[0-9]\d*(\.\d+)?$/.test(value);
						}, "Invalid HSA Funding.");

						jQuery.validator.addMethod("QTYFormat", function (value, element) {
							return this.optional(element) || /^[0-9]{1,9}$/.test(value);
						}, "Invalid Qty Value.");

						jQuery.validator.addMethod("CostFormat", function (value, element) {
							return this.optional(element) || /^[0-9]\d*(\.\d+)?$/.test(value);
						}, "Invalid Cost Value.");

						$.validator.addMethod("customemail", function (value, element) {
							return /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/.test(value.toLowerCase());
						},
							"Sorry, I've enabled very strict email validation"
						);

						jQuery.validator.addMethod("DOBFormat", function (value, element) {
							return this.optional(element) || /(?=\d)^(?:(?!(?:10\D(?:0?[5-9]|1[0-4])\D(?:1582))|(?:0?9\D(?:0?[3-9]|1[0-3])\D(?:1752)))((?:0?[13578]|1[02])|(?:0?[469]|11)(?!\/31)(?!-31)(?!\.31)|(?:0?2(?=.?(?:(?:29.(?!000[04]|(?:(?:1[^0-6]|[2468][^048]|[3579][^26])00))(?:(?:(?:\d\d)(?:[02468][048]|[13579][26])(?!\x20BC))|(?:00(?:42|3[0369]|2[147]|1[258]|09)\x20BC))))))|(?:0?2(?=.(?:(?:\d\D)|(?:[01]\d)|(?:2[0-8])))))([-.\/])(0?[1-9]|[12]\d|3[01])\2(?!0000)((?=(?:00(?:4[0-5]|[0-3]?\d)\x20BC)|(?:\d{4}(?!\x20BC)))\d{4}(?:\x20BC)?)(?:$|(?=\x20\d)\x20))?((?:(?:0?[1-9]|1[012])(?::[0-5]\d){0,2}(?:\x20[aApP][mM]))|(?:[01]\d|2[0-3])(?::[0-5]\d){1,2})?$/.test(value);
						}, "Invalid DOB.");

						/**
							Case page Validation.
						*/

						$("#caseAdd").validate({
							rules: {
								fname: {
									required: true,
									maxlength: 50
								},
								lname: {
									required: true,
									maxlength: 50
								},
								address: {
									maxlength: 200
								},
								city: {
									required: true,
									maxlength: 50
								},
								zipcode: {
									required: true,
									maxlength: 10
								},
								phone: {
									required: true,
									maxlength: 14,
									phone: true
								},
								email: {
									required: true,
									maxlength: 50,
									customemail: true
								},
								CurrentPlan: {
									maxlength: 50
								},
								CurrentPremium: {
									maxlength: 50
								},
								Notes: {
									maxlength: 4000
								},
								CaseTitle: {
									maxlength: 50
								},
								MAGIncome: {
									required: true,
									maxlength: 7
								},
								TaxRate: {
									TaxRateFormat: true,
									max: 100,
									maxlength: 6
								},
								HSAFunding: {
									HSAFormat: true,
									max: 100,
									maxlength: 6
								},
								DedBalAvailToRO: {
									required: true
								},
								// DedBalAvlDate: {
								// 	required: true
								// },
								TierIn: {
									required: true
								}
							}
						});

						$('.QTYFormat').rules('add', {
							QTYFormat: true
						});
						$('.CostFormat').rules('add', {
							CostFormat: true
						});
						$('.DOBFormat').rules('add', {
							DOBFormat: true
						});


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

					})
				}
			]);