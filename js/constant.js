'use strict';
(function () {
    angular.module('mhmApp.messages', [])
        .factory('messages', ['$rootScope', function ($rootScope) {
			var service = {};
			service.AccessToken_accessToken = null;
			service.serverLiveHost = hostAPI;
			service.base_url = hostBaseURL;
			service.login = "/api/Users/Login";
			service.maxDefaultFamily = 8;
			service.pageSize = 50;
			service.sessionTimeout = 1200000;
			service.IndividualEmployerId = "100000";
			service.ShopEmployerId = "99999";
			service.caseSource = "Manual Entry";
			service.caseStatus = "New";
			service.StatusId = "3";
			service.caseAction = "";
			service.caseSourceCopied = "Case copied from case";
			service.confirmationMsg = "Are you sure you want to make these changes? This cannot be undone.";
			service.confirmationMsgDelete = "Are you sure you want to delete this? This cannot be undone.";
			service.saved = "Data Saved successfully.";
			service.NoFamily = "You cannot add any more members.";
			service.ErrorForm = "Please check the details of this form before proceeding.";
			service.ErrorFormMedical = "Please check (MAGI, Date of Birth, Gender and Benefit Cost) before proceeding.";
			service.ErrorMedical = "Please check (MAGI, Date of Birth, Gender and Benefit Cost) before proceeding.";
			service.confirmUnlock = "Do you want to unlock user?";
			service.confirmActivate = "Do you want to activate user?";
			service.confirmActivatePlanAttribute = "Do you want to activate this plan?";
			service.confirmDeactivate = "Do you want to de-activate user?";
			service.confirmDeactivatePlanAttribute = "Do you want to de-activate this plan?";
			service.ForgotPasswordSuccess = "An email has been sent to your inbox.";
			service.NoRecordFound = "No Record Found.";
			service.UpdatePassword = "Password updated successfully.";
			service.IncorrectOldPassword = "Incorrect old password.";
			service.ValidPasswordCheck = "One lower and uppercase letter, one number, and one non-alphanumeric symbol.";
			service.MailSend = "Do you want to send report to applicant email?";
			service.TryLater = "Please try later.";
			service.NoBenefit = "No Benefits found.";
			service.NoCriticalIllness = "No Critical illness found.";
			service.InvalidDOB = "Invalid Date of Birth.";
			service.confirmationCaseSaveMsg = "Do you want to save case?";
			service.sameCaseTitleWarning = "Case cannot be saved under the same name and title as a current existing case, change the name or title to save.";
			service.changedEmployerCarrierWarning = "You have changed Employer data, the Carrier selection has been removed";
			service.confirmationJobSaveMsg = "Do you want to save Job?";
			service.confirmationJobReportSend = "Do you want to send reports?";

			service.limitUnit = "Please Specify Limit Unit";

			service.county = "/api/case/getcounty";
			service.carrier = "/api/case/carrier";
			service.plan = "/api/case/plan";
			service.employer = "/api/case/employer";
			service.code = "/api/case/usagecode";
			service.medicalusage = "/api/Benefit?RatingAreaId=";
			service.medicalusageCase = "/api/case/medicalusage/";
			service.calculatePlans = "/api/operation/CalculatePlans/";
			service.generatePlans = "/api/GenerateResultsController/GenerateResult/";
			service.sendMail = "/api/case/upload";
			service.sendMailwithUpdate="/api/GenerateResultsController/SendMail";
			service.pdfGeneration = "/api/case/pdfgeneration";
			service.CheckCaseTitle = "/api/case/CheckCaseTitle/";
			service.InsuranceType = "/api/case/InsuranceType";
			/****
				Created By : Vaibhav Chaurasiya
				Created Date : 29-09-2016
				Purpose : Function to get list of Insurance types.
			****/
			service.InsuranceTypeMaster = "/api/PlanMasters/GetInsuranceTypeMaster";
			/**** End : Function to get list of Insurance types. ****/
			
			service.GetCriticalillness = "/api/case/GetCriticalillness";

			service.addUser = "/api/Users/Register";
			service.updateUser = "/api/Users/Update";
			service.getUsers = "/api/Users";
			service.deleteUser = "/api/Users/Delete";
			service.getUser = "/api/Users/Detail";
			service.getForgotPassword = "/api/Users/ResetPassword";
			service.getChangePassword = "/api/Users/ChangePassword";
			service.SetPassword = "/api/Users/SetPassword";
			service.CheckEmailStatus = "/api/Users/CheckEmailStatus/";
			service.UnlockUser = "/api/Users/UnlockUser?emailId=";
			service.GetClientCompanies = "/api/Users/GetClientCompanies";
			service.ChangeUserStatus = "/api/Users/ChangeUserStatus";

			service.getBenefits = "/api/Benefit";
			service.updateBenefit = "/api/Updatebenefit";
			service.deleteBenefit = "/api/Benefit/DeleteBenefit";
			service.category = "/api/case/getusagecategory";
			service.ratingArea = "/api/Benefit/GetRatingArea";
			service.saveBenefitCost = "/api/benefit/SaveBenefitCost";
			service.GetAllApprovalStatus = "/api/PlanMaster/GetApprovalStatus";
			service.GetAllStates = "/api/benefit/GetAllStates";
			service.GetBenefitcarrier = "/api/Benefit/Getcarrier";
			service.GetGroupName = '/api/PlanMaster/GetGroupName';

			/****
				Created By : Aastha Jain
				Created Date : 07-06-2016
				Purpose : Function to get list of Case Status Code.
			****/

			service.GetAllCaseStatusCodes = "/api/case/GetCaseStatusMst";

			/**** End : Function to get list of Case Status Code. ****/

			/****
				Created By : Aastha Jain
				Created Date : 25-08-2016
				Purpose : Function to get list of Case Employer Companies.
			****/

			service.GetEmployerCompanies = "/api/case/GetEmployerCompanies";

			/**** End : Function to get list of Case Employer Companies. ****/

			/****
				Created By : Aastha Jain
				Created Date : 26-08-2016
				Purpose : Function to get list of Job Numbers and Agents.
			****/

			service.GetJobNumbers = "/api/case/GetJobNumbers";
			service.GetAgentNames = "/api/case/GetAgentNames";

			/**** End : Function to get list of Job Numbers and Agents. ****/

			service.getCases = "/api/case/getall";
			service.saveCase = "/api/case/save";
			service.updateCase = "/api/case/UpdateCase"
			service.getCase = "/api/case/details/";
			service.getCaseReport = "/api/case/GetCaseReport";
			service.ImportCaseCensus = "/api/PlanMaster/ImportCensus";

			service.getPlanattributes = "/api/PlanMasters/GetAllPlanAttributes";
			service.UpdatePlanAttributes = "/api/PlanMasters/UpdatePlanAttributes";
			service.GetPlanAttribute = "/api/PlanMasters/GetPlanAttributes";
			service.GetMarketCoverage = "/api/PlanMasters/GetMarketCoverage";
			service.GetMetalLevel = "/api/PlanMasters/GetMetalLevel";
			service.GetEmployerMaster = "/api/PlanMasters/GetEmployerMaster";
			service.UpdatePlanAttribute = "/api/PlanMasters/UpdatePlanAttribute";

			/****
				Created By : Aastha Jain
				Created Date : 07-06-2016
				Purpose : Function to export plan attribute list.
			****/

			service.getPlanattributeReport = "/api/PlanMasters/ExportPlanAttributes";

			/**** End : Function to export plan attribute list. ****/

			service.getAllPlanBenefits = "/api/PlanMasters/GetAllPlanBenefits";
			service.GetPlanBenefit = "/api/PlanMasters/GetPlanBenefit";
			service.UpdatePlanBenefit = "/api/PlanMasters/UpdatePlanBenefit";

			/****
				Created By : Aastha Jain
				Created Date : 07-06-2016
				Purpose : Function to export plan benefits list.
			****/

			service.getPlanbenefitReport = "/api/PlanMasters/ExportPlanBenefits";

			/**** End : Function to export plan benefits list. ****/

			service.getAllCSRRates = "/api/PlanMasters/GetAllCSRRates";
			service.getCSRRate = "/api/PlanMasters/GetCSRRate";
			service.UpdateCSRRate = "/api/PlanMasters/UpdateCSRRate";

			/****
				Created By : Aastha Jain
				Created Date : 07-06-2016
				Purpose : Function to export CSR Rate list.
			****/

			service.getCSRrateReport = "/api/PlanMasters/ExportCSRRates";

			/**** End : Function to export CSR Rate list. ****/

			/****
				Created By : Aastha Jain
				Created Date : 13-06-2016
				Purpose : To Create New Masters (Rating Area, ZIp Code, Country)
			****/

			service.GetRatingAreas = "/api/AreaMaster/GetRatingAreas";
			service.GetZipCodes = "/api/AreaMaster/GetZipCodes";
			service.GetCounties = "/api/AreaMaster/GetFIPS_County";

			/**** End : To Create New Masters (Rating Area, ZIp Code, Country) ****/

			/****
				Created By : Aastha Jain
				Created Date : 12-07-2016
				Purpose : To Create Job Master.
			****/

			service.GetJobMstrLists = '/api/PlanMaster/GetJobMasters';
			service.addJobMstr = '/api/PlanMaster/AddJobMaster';
			service.getJobDetail = '/api/PlanMaster/GetJobMastersDetail';
			service.editJobMstr = '/api/PlanMaster/UpdateJobMaster';
			service.GetJobStatus = '/api/PlanMaster/GetJobStatus';
			
			/****
				Created By : Vaibhav Chaurasiya
				Created Date : 24-10-2016
				Purpose : To Get CaseJobRunStatus.
			****/

			service.GetCaseJobRunStatus = '/api/PlanMaster/GetCaseJobRunStatus'

			/**** End : To Get CaseJobRunStatus. ****/

			service.GetNewJobNumber = '/api/PlanMaster/GetNewJobNumber';
			service.GetAllFinalNotSendCases = '/api/GenerateResultsController/GetAllFinalNotSendCases';
			service.UpdateAllFinalNotSendCases = '/api/GenerateResultsController/UpdateAllFinalNotSendCases';

			/**** End : To Create Job Master. ****/

			/****
				Created By : Aastha Jain
				Created Date : 12-07-2016
				Purpose : To CheckBenefitMapping.
			****/

			service.CheckBenefitMapping = '/api/benefit/CheckBenefitMapping'

			/**** End : To CheckBenefitMapping. ****/

			/****
				Created By : Vaibhav Chaurasiya
				Created Date : 30-09-2016
				Purpose : Function crud operation of Employer Master.
			****/
			service.GetEmployers = "/api/EmployerMaster/GetEmployers";
			service.GetEmployer = "/api/EmployerMaster/GetEmployer";
			service.AddEmployer = "/api/EmployerMster/AddEmployer";
			service.UpdateEmployer = "/api/EmployerMaster/UpdateEmployer";
			/**** End : Function crud operation of Employer Master. ****/


			/****
				Created By : Rahul Singh
				Created Date : 29-03-2017
				Purpose : Getting Data for PlanBenefit & Plan Attribute & CSR Rate.
			****/
			service.GetSearchIssuerIds = "/api/PlanMasters/GetSearchIssuerIds";
			service.GetCSRSearchPlanIds = "/api/PlanMasters/GetCSRSearchPlanIds";
			service.GetSearchMHMBenefitIds = "/api/PlanMasters/GetSearchMHMBenefitIds";
			service.GetSearchPlanIds = "/api/PlanMasters/GetSearchPlanIds";
			service.GetPlanBenefitData = "/api/PlanMasters/GetPlanBenefitByOrderEvent";
			service.GetDefaultBenefitId = "/api/PlanMasters/GetPlanBenefitByOrderEvent";
			service.GetCostSharingTypes = "/api/PlanMaster/GetCostSharingTypes";
			service.GetLimitUnits = "/api/PlanMaster/GetLimitUnits";
			service.AddPlanBenefit = "/api/PlanMasters/AddPlanBenefit";

			service.AddCSRRate = "/api/PlanMasters/AddCSRRate";
			service.AddPlanAttribute = "/api/PlanMasters/AddPlanAttribute";

			/****
				Created By : Rahul Singh
				Created Date : 11-04-2017
				Purpose : Job Related
			****/
			service.AddJobPlans = "/api/PlanMasters/AddJobPlans";
			service.GetJobPlans = "/api/PlanMasters/GetJobPlans";
			
			service.GetHSALimit = "/api/case/GetHSALimit/";
			
			service.GetAllCarrier = "/api/Issuer/GetAllCarrier";
			service.GetCarrier = "/api/Issuer/GetCarrier";
			service.AddCarrier = "/api/Issuer/SaveCarrier";
			service.UpdateCarrier = "/api/Issuer/UpdateCarrier";

			/****
				Created By : Vaibhav Chaurasiya
				Created Date : 20-03-2018
				Purpose : Function tp update "Use for Optimization" and "Alternate".
			****/
			service.UpdateCaseOpt = "/api/case/UpdateCaseOPT";
			service.UpdateCaseAlternate = "/api/case/UpdateCaseAlternate";
			/**** End : Function crud operation of Employer Master. ****/

			return service;
		}]);
} ()); 