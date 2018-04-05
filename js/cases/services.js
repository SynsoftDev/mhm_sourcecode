'use strict';
/**
	Case Services.
*/
function isEmpty(value) {
	return angular.isUndefined(value) || value === '' || value === null || value !== value;
}
angular.module('mhmApp.cases')
	.factory('CaseService',
		['$document', '$window', '$http', '$cookieStore', '$rootScope', '$timeout', 'messages', '$q',
			function ($document, $window, $http, $cookieStore, $rootScope, $timeout, messages, $q) {
				var service = {};
				var d = $q.defer();
				var config = {
					headers: {
						'Content-Type': 'application/x-www-form-urlencoded'
					}
				}
				function onScriptLoad() {
					d.resolve($window.d3);
				}
				var scriptTag = $document[0].createElement('script');
				scriptTag.type = 'text/javascript';
				scriptTag.async = true;
				scriptTag.src = location.protocol + '//d3js.org/d3.v3.min.js';
				scriptTag.onreadystatechange = function () {
					if (this.readyState == 'complete') onScriptLoad();
				}
				scriptTag.onload = onScriptLoad;

				var s = $document[0].getElementsByTagName('head')[0];
				s.appendChild(scriptTag);
				/**
					Save Case API.
				*/
				service.update = function (data, action, callback) {
					if (action == 0) {
						data.CaseID = 0;
						var url = messages.serverLiveHost + messages.saveCase;
					}
					else
						var url = messages.serverLiveHost + messages.updateCase;
					$http.post(url, JSON.stringify(data), {
						headers: {
							'Content-Type': 'application/json'
						}
					}).success(function (response) {
						callback(response);
					}).error(function (response) {
						if (isEmpty(response)) {
							var response = { Status: false, Message: messages.TryLater, redirect: true };
						}
						callback(response);
					});
				};
				/**
					Update Case Opt.
				*/
				service.updateCaseOpt = function (data, callback) {
					
					var url = messages.serverLiveHost + messages.UpdateCaseOpt + '/' + data.CaseId + '/' + data.Status;
					console.log(url)
					$http.post(url, {}, {
						headers: {
							'Content-Type': 'application/json'
						}
					}).success(function (response) {
						callback(response);
					}).error(function (response) {
						if (isEmpty(response)) {
							var response = { Status: false, Message: messages.TryLater, redirect: true };
						}
						callback(response);
					});
				};
				/**
					Update Case Alternate.
				*/
				service.UpdateCaseAlternate = function (data, callback) {
					
					var url = messages.serverLiveHost + messages.UpdateCaseAlternate + '/' + data.CaseId + '/' + data.Status;
					console.log(url)
					$http.post(url, {}, {
						headers: {
							'Content-Type': 'application/json'
						}
					}).success(function (response) {
						callback(response);
					}).error(function (response) {
						if (isEmpty(response)) {
							var response = { Status: false, Message: messages.TryLater, redirect: true };
						}
						callback(response);
					});
				};
				/**
					Send email report  API.
				*/
				service.upload = function (data, callback) {
					var url = messages.serverLiveHost + messages.sendMail;
					$http({ method: 'POST', url: url, data: data, headers: { 'Content-Type': 'application/x-www-form-urlencoded;' } }).success(function (response) {
						callback(response);
					}).error(function (response) {
						if (isEmpty(response)) {
							var response = { Status: false, Message: messages.TryLater, redirect: true };
						}
						callback(response);
					});
				};
				/**
					Get all cases.
				*/
				service.getAll = function (data, callback) {
					var url = messages.serverLiveHost + messages.getCases;
					var lstParameter = [];
					lstParameter[0] = '';
					lstParameter[1] = '';
					lstParameter[2] = '';
					lstParameter[3] = '';
					lstParameter[4] = '';
					lstParameter[5] = '';
					lstParameter[6] = '';
					lstParameter[7] = '';
					lstParameter[8] = '';
					//  Applicant Name, Case title, Agent, Applicant Phone and Email 
					if (typeof data.searchByEmployer != 'undefined' && data.searchByEmployer != '') {
						lstParameter[0] = data.searchByEmployer;
					} if (typeof data.searchByCaseTitle != 'undefined' && data.searchByCaseTitle != '') {
						lstParameter[1] = data.searchByCaseTitle;
					} if (typeof data.searchByAgent != 'undefined' && data.searchByAgent != '') {
						lstParameter[2] = data.searchByAgent;
					} if (typeof data.searchByPhone != 'undefined' && data.searchByPhone != '') {
						lstParameter[3] = data.searchByPhone;
					} if (typeof data.searchByJobNo != 'undefined' && data.searchByJobNo != '') {
						lstParameter[4] = data.searchByJobNo;
					} if (typeof data.searchByStatusCode != 'undefined' && data.searchByStatusCode != '') {
						lstParameter[5] = data.searchByStatusCode;
					} if (typeof data.searchByBusinessYear != 'undefined' && data.searchByBusinessYear != '') {
						lstParameter[6] = data.searchByBusinessYear;
					}
					if (typeof data.searchByAnalytics != 'undefined' && data.searchByAnalytics != '') {
						lstParameter[7] = data.searchByAnalytics;
					}
					if (typeof data.searchByAlternate != 'undefined' && data.searchByAlternate != '') {
						lstParameter[8] = data.searchByAlternate;
					}

					var postData = { searchby: angular.lowercase(data.searchby), sortby: data.sortby, desc: data.desc, page: data.page, pageSize: data.pageSize, startDate: data.start_date, endDate: data.end_date, lstParameter: lstParameter, EmailId: data.customer.Email };

					$http({ method: 'GET', url: url, params: postData })
						.success(function (response) {
							callback(response);
						}).error(function (response) {
							if (isEmpty(response)) {
								var response = { Status: false, Message: messages.TryLater, redirect: true };
							}
							callback(response);
						});;
				};
				/**
					Get case report.
				*/
				service.getCaseReport = function (data, callback) {
					var url = messages.serverLiveHost + messages.getCaseReport;
					var lstParameter = [];
					lstParameter[0] = '';
					lstParameter[1] = '';
					lstParameter[2] = '';
					lstParameter[3] = '';
					lstParameter[4] = '';
					lstParameter[5] = '';
					lstParameter[6] = '';
					lstParameter[7] = '';
					lstParameter[8] = '';
					var searchby = '';
					var sortby = '';
					var desc = '';
					var start_date = '';
					var end_date = '';
					var Email = '';
					//  Applicant Name, Case title, Agent, Applicant Phone and Email 
					if (typeof data.searchByName != 'undefined' && data.searchByName != '') {
						lstParameter[0] = data.searchByName;
					} if (typeof data.searchByCaseTitle != 'undefined' && data.searchByCaseTitle != '') {
						lstParameter[1] = data.searchByCaseTitle;
					} if (typeof data.searchByAgent != 'undefined' && data.searchByAgent != '') {
						lstParameter[2] = data.searchByAgent;
					} if (typeof data.searchByPhone != 'undefined' && data.searchByPhone != '') {
						lstParameter[3] = data.searchByPhone;
					} if (typeof data.searchByJobNo != 'undefined' && data.searchByJobNo != '') {
						lstParameter[4] = data.searchByJobNo;
					} if (typeof data.searchByStatusCode != 'undefined' && data.searchByStatusCode != '') {

						lstParameter[5] = JSON.stringify(data.searchByStatusCode);
					} if (typeof data.searchByBusinessYear != 'undefined' && data.searchByBusinessYear != '') {
						lstParameter[6] = data.searchByBusinessYear;
					}
					if (typeof data.searchByAnalytics != 'undefined' && data.searchByAnalytics != '') {
						lstParameter[7] = data.searchByAnalytics;
					}
					if (typeof data.searchByAlternate != 'undefined' && data.searchByAlternate != '') {
						lstParameter[8] = data.searchByAlternate;
					}


					if (typeof data.searchby != 'undefined' && data.searchby != '') {
						searchby = angular.lowercase(data.searchby);
					}
					if (typeof data.sortby != 'undefined' && data.sortby != '') {
						sortby = data.sortby;
					}

					if (typeof data.start_date != 'undefined' && data.start_date != '') {
						start_date = data.start_date;
					}
					if (typeof data.end_date != 'undefined' && data.end_date != '') {
						end_date = data.end_date;
					}
					if (typeof data.customer.Email != 'undefined' && data.customer.Email != '') {
						Email = data.customer.Email;
					}

					url += '?searchby=' + searchby;
					url += '&sortby=' + sortby;
					url += '&desc=' + data.desc;
					url += '&startDate=' + start_date;
					url += '&endDate=' + end_date;
					url += '&lstParameter=' + lstParameter[0];
					url += '&lstParameter=' + lstParameter[1];
					url += '&lstParameter=' + lstParameter[2];
					url += '&lstParameter=' + lstParameter[3];
					url += '&lstParameter=' + lstParameter[4];
					url += '&lstParameter=' + lstParameter[5];
					url += '&lstParameter=' + lstParameter[6];
					url += '&EmailId=' + Email;

					var xhr = new XMLHttpRequest();
					xhr.open('GET', url, true);
					xhr.responseType = 'blob';
					xhr.onload = function () {
						callback(this.status, this.response, xhr);
					};
					xhr.setRequestHeader('Authorization', 'bearer ' + $rootScope.globals.currentUser.authdata);
					xhr.send();
				};
				/**
					Get case detail.
				*/
				service.getCase = function (id, callback) {
					var url = messages.serverLiveHost + messages.getCase + id;
					$http.get(url).success(function (response) {
						callback(response);
					}).error(function (response) {
						if (isEmpty(response)) {
							var response = { Status: false, Message: messages.TryLater, redirect: true };
						}
						callback(response);
					});;
				};
				/**
					Calculate plan API.
				*/
				service.calculatePlans = function (data, callback) {
					/* var url = messages.serverLiveHost + messages.calculatePlans+'?ZipCode='+data.ZipCode+'&CountyName='+data.CountyName+'&Income='+data.Income+'&IsAmericanIndian='+data.IsAmericanIndian+'&SubsidyStatus='+data.SubsidyStatus+'&HSAPercentage='+data.HSAPercentage+'&TaxRate='+data.TaxRate+'&UsageCode='+data.UsageCode+'&IssuerId='+data.IssuerId+'&PlanTypeID='+data.PlanID+'&Welness='+data.Welness+'&EmployerId='+data.EmployerId+'&InsuranceTypeId='+data.InsuranceTypeId+'&JobNumber='+data.JobNumber+'&BusinessYear='+data.BusinessYear+'&DedBalAvailToRollOver='+data.DedBalAvailToRollOver+'&DedBalAvailDate='+data.DedBalAvailDate+'&TierIntention='+data.TierIntention;	*/

					var url = messages.serverLiveHost + messages.calculatePlans + '?ZipCode=' + data.ZipCode + '&CountyName=' + data.CountyName + '&Income=' + data.Income + '&IsAmericanIndian=' + data.IsAmericanIndian + '&SubsidyStatus=' + data.SubsidyStatus + '&HSAPercentage=' + data.HSAPercentage + '&TaxRate=' + data.TaxRate + '&UsageCode=' + data.UsageCode + '&IssuerId=' + data.IssuerId + '&PlanTypeID=' + data.PlanID + '&Welness=' + data.Welness + '&EmployerId=' + data.EmployerId + '&JobNumber=' + data.JobNumber + '&BusinessYear=' + data.BusinessYear + '&DedBalAvailToRollOver=' + data.DedBalAvailToRollOver + '&DedBalAvailDate=' + data.DedBalAvailDate + '&TierIntention=' + data.TierIntention;
					$http.post(url, $.param({ data: data.post.data, UsesDetail: data.post.UsesDetail }), config)
						.success(function (response) {
							callback(response);
						}).error(function (response) {
							if (isEmpty(response)) {
								var response = { Status: false, Message: messages.TryLater, redirect: true };
							}
							callback(response);
						});;
				};
				/**
					get County.
				*/
				service.getCounty = function (zipcode, callback) {
					if (typeof zipcode != 'undefined' && zipcode != '') {
						var url = messages.serverLiveHost + messages.county + '/' + zipcode;
						$http.get(url).success(function (response) {
							callback(response);
						}).error(function (response) {
							callback(response);
						});
					} else {
						callback({ Status: false, CountyList: [] });
					}

				};
				/**
					get Insurance Type.
				*/
				service.getInsuranceType = function (EmployerId, Year, callback) {
					if (typeof EmployerId != 'undefined' && EmployerId != '') {
						var url = messages.serverLiveHost + messages.InsuranceType + '/' + EmployerId + '/' + Year;
						$http.get(url).success(function (response) {
							callback(response);
						}).error(function (response) {
							callback(response);
						});
					} else {
						callback({ Status: false, InsuranceTypeList: [] });
					}

				};
				/**
					get Carrier.
				*/
				service.getCarrier = function (data, callback) {
					var url = messages.serverLiveHost + messages.carrier + '/' + data.EmployerId + '/' + data.ZipCode + '/' + data.CountyName + '/' + data.InsuranceTypeId + '/' + data.BusinessYear;
					$http.get(url).success(function (response) {
						callback(response);
					}).error(function (response) {
						callback(response);
					});
				};
				/**
					get Employer.
				*/
				// service.getEmployer = function (Year,callback) {				  			
				// var url = messages.serverLiveHost + messages.employer+'/'+Year;					
				// $http.get(url).success(function(response) {	
				// callback(response); 	
				// }).error(function(response) {	
				// callback(response); 	
				// });


				// };

				/**
					pdf Generation.
				*/
				service.pdfGeneration = function (data, callback) {
					var url = messages.serverLiveHost + messages.pdfGeneration;
					$http({ method: 'POST', url: url, data: $.param(data), headers: { 'Content-Type': 'application/x-www-form-urlencoded;' } }).success(function (response) {
						callback(response);
					}).error(function (response) {
						if (isEmpty(response)) {
							var response = { Status: false, Message: messages.TryLater, redirect: true };
						}
						callback(response);
					});
				};
				service.getPlan = function () {
					var result = {};
					var url = messages.serverLiveHost + messages.plan;
					return $http.get(url);
				};
				service.getCode = function () {
					var result = {};
					var url = messages.serverLiveHost + messages.code;
					return $http.get(url);
				};
				service.GetAllStates = function () {
					var result = {};
					var url = messages.serverLiveHost + messages.GetAllStates;
					return $http.get(url);
				};
				/****
					Created By : Aastha Jain
					Created Date : 08-06-2016
					Purpose : Function to call service of Case Status Code.		
				****/
				service.GetAllCaseStatusCodes = function () {
					var result = {};
					var url = messages.serverLiveHost + messages.GetAllCaseStatusCodes;
					return $http.get(url);
				};
				/**** End : Function to call service of Case Status Code. ****/
				/****
					Created By : Aastha Jain
					Created Date : 25-08-2016
					Purpose : Function to call service of Case Employer Companies.		
				****/
				service.GetEmployerCompanies = function () {
					var result = {};
					var url = messages.serverLiveHost + messages.GetEmployerCompanies;
					return $http.get(url);
				};
				/**** End : Function to call service of Case Employer Companies. ****/
				/****
					Created By : Aastha Jain
					Created Date : 26-08-2016
					Purpose : Function to call service of Job Numbers and Agents.		
				****/
				service.GetJobNumbers = function () {
					var result = {};
					var url = messages.serverLiveHost + messages.GetJobNumbers;
					return $http.get(url);
				};

				service.GetAgentNames = function () {
					var result = {};
					var url = messages.serverLiveHost + messages.GetAgentNames;
					return $http.get(url);
				};
				/**** End : Function to call service of Job Numbers and Agents. ****/
				service.GetCriticalillness = function () {
					var result = {};
					var url = messages.serverLiveHost + messages.GetCriticalillness;
					return $http.get(url);
				};


				/* Requested Parameter : api/case/medicalusage/{ZipCode}/{CountyName}/{EmployerId}/{StateCode} */
				service.getMedicalUsage = function (data, callback) {
					var url = messages.serverLiveHost + messages.medicalusageCase + data.zipcode + '/' + data.CountyName + '/' + data.EmployerId + '/' + data.StateCode;
					$http.get(url).success(function (response) {
						callback(response);
					}).error(function (response) {
						if (isEmpty(response)) {
							var response = { Status: false, Message: messages.TryLater, redirect: true };
						}
						callback(response);
					});
				};
				/**
					Initial Services For Create/View Case.
				*/
				service.waitForLayout = function (callback) {
					$q.all([
						service.getCode(),
						service.getPlan(),
						service.GetAllStates(),
						service.GetCriticalillness(),
						service.GetAllCaseStatusCodes(),
						service.getJobNumberList(),
						service.getEmployer(),
						service.GetAgentNames(),
					]).then(function (response) {
						callback(response)
					});
				};
				/**
					Initial Services For List Cases.
				*/
				service.waitForLayoutCases = function (callback) {
					$q.all([
						service.GetAllCaseStatusCodes(),
						service.GetEmployerCompanies(),
						service.GetJobNumbers(),
						service.GetAgentNames(),
					]).then(function (response) {
						callback(response)
					});
				};

				service.d3service = {
					d3: function () { return d.promise; }
				};

				/**
					Created By : Aastha Jain
					Created Date : 18-07-2016
					Start : .
				**/
				service.getJobNumberList = function () {
					var result = {};
					var url = messages.serverLiveHost + messages.GetJobMstrLists;
					return $http.get(url);
				};

				service.getEmployer = function () {
					var result = {};
					var url = messages.serverLiveHost + messages.employer;
					return $http.get(url);
				};

				// service.getJobNumberList = function (callback) {				  			
				// var url = 				
				// $http.get(url).success(function(response) {	
				// callback(response); 	
				// }).error(function(response) {	
				// callback(response); 	
				// });
				// };

				service.GetHSALimit = function (MaxMemberAge, UsageCode, BusinessYear, callback) {
					//  var params={MaxMemberAge:MaxMemberAge,UsageCode:UsageCode};
					var url = messages.serverLiveHost + messages.GetHSALimit + MaxMemberAge + '/' + UsageCode + '/' + BusinessYear;
					$http({ method: 'GET', url: url }).success(function (response) {
						callback(response);
					}).error(function (response) {
						if (isEmpty(response)) {
							var response = { Status: false, Message: messages.TryLater, redirect: true };
						}
						callback(response);
					});
				}

				service.GetJobPlans = function(data, callback) {

					var url = messages.serverLiveHost + messages.GetJobPlans+'?JobNumber='+data.jobNo;
					console.log("url", url);
		
					$http.get(url).success(function(response) {
						callback(response);
					}).error(function(response) {
						callback(response);
					});
				};

				return service;
			}]);