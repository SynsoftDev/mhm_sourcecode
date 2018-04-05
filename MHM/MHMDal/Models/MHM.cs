namespace MHMDal.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MHM : DbContext
    {
        public MHM()
            : base("name=MHM")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Applicant> Applicants { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<BenefitUserDetail> BenefitUserDetails { get; set; }
        public virtual DbSet<Case> Cases { get; set; }
        public virtual DbSet<CasePlanResult> CasePlanResults { get; set; }
        public virtual DbSet<CaseStatusMst> CaseStatusMsts { get; set; }
        public virtual DbSet<CategoryMst> CategoryMsts { get; set; }
        public virtual DbSet<CensusImport> CensusImports { get; set; }
        public virtual DbSet<ChipEligibility> ChipEligibilities { get; set; }
        public virtual DbSet<ClientCompany> ClientCompanies { get; set; }
        public virtual DbSet<CostSharingReductionScheduleMst> CostSharingReductionScheduleMsts { get; set; }
        public virtual DbSet<Criticalillness> Criticalillnesses { get; set; }
        public virtual DbSet<CriticalillnessMst> CriticalillnessMsts { get; set; }
        public virtual DbSet<CSR_Rate_Mst> CSR_Rate_Mst { get; set; }
        public virtual DbSet<CSTMst> CSTMsts { get; set; }
        public virtual DbSet<EmployerMst> EmployerMsts { get; set; }
        public virtual DbSet<Family> Families { get; set; }
        public virtual DbSet<FedPovertyLevelMst> FedPovertyLevelMsts { get; set; }
        public virtual DbSet<FPLBracketLookupMst> FPLBracketLookupMsts { get; set; }
        public virtual DbSet<FPLCapMst> FPLCapMsts { get; set; }
        public virtual DbSet<HSAFunding> HSAFundings { get; set; }
        public virtual DbSet<InsuranceType> InsuranceTypes { get; set; }
        public virtual DbSet<IssuerMst> IssuerMsts { get; set; }
        public virtual DbSet<JobMaster> JobMasters { get; set; }
        public virtual DbSet<JobPlansMst> JobPlansMsts { get; set; }
        public virtual DbSet<MedicaidEligibility> MedicaidEligibilities { get; set; }
        public virtual DbSet<MHMBenefitCostByAreaMst> MHMBenefitCostByAreaMsts { get; set; }
        public virtual DbSet<MHMBenefitMappingMst> MHMBenefitMappingMsts { get; set; }
        public virtual DbSet<MHMCommonBenefitsMst> MHMCommonBenefitsMsts { get; set; }
        public virtual DbSet<PlanAttributeMst> PlanAttributeMsts { get; set; }
        public virtual DbSet<PlanBenefitMst> PlanBenefitMsts { get; set; }
        public virtual DbSet<PlanMaster> PlanMasters { get; set; }
        public virtual DbSet<Rule> Rules { get; set; }
        public virtual DbSet<tblRatingAreaMst> tblRatingAreaMsts { get; set; }
        public virtual DbSet<tblStateAbrev> tblStateAbrevs { get; set; }
        public virtual DbSet<UsageCodeMaster> UsageCodeMasters { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<tblRatingArea> tblRatingAreas { get; set; }
        public virtual DbSet<tblZipCode> tblZipCodes { get; set; }
        public virtual DbSet<qryZipCodeToRatingArea> qryZipCodeToRatingAreas { get; set; }
        public virtual DbSet<qryZipStateCounty> qryZipStateCounties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Applicant>()
                .Property(e => e.EREmpId)
                .IsUnicode(false);

            modelBuilder.Entity<Applicant>()
                .Property(e => e.JobTitle)
                .IsUnicode(false);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<BenefitUserDetail>()
                .Property(e => e.UsageCost)
                .HasPrecision(12, 2);

            modelBuilder.Entity<BenefitUserDetail>()
                .Property(e => e.UsageQty)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Case>()
                .Property(e => e.CaseTitle)
                .IsUnicode(false);

            modelBuilder.Entity<Case>()
                .Property(e => e.Year)
                .IsUnicode(false);

            modelBuilder.Entity<Case>()
                .Property(e => e.TaxRate)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Case>()
                .Property(e => e.HSAFunding)
                .HasPrecision(16, 2);

            modelBuilder.Entity<Case>()
                .Property(e => e.FPL)
                .HasPrecision(16, 2);

            modelBuilder.Entity<Case>()
                .Property(e => e.MonthlySubsidy)
                .HasPrecision(16, 2);

            modelBuilder.Entity<Case>()
                .Property(e => e.HSALimit)
                .HasPrecision(16, 2);

            modelBuilder.Entity<Case>()
                .Property(e => e.HSAAmount)
                .HasPrecision(16, 2);

            modelBuilder.Entity<Case>()
                .Property(e => e.MAGIncome)
                .HasPrecision(16, 2);

            modelBuilder.Entity<Case>()
                .Property(e => e.TotalMedicalUsage)
                .HasPrecision(16, 2);

            modelBuilder.Entity<Case>()
                .Property(e => e.ZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<Case>()
                .Property(e => e.CountyName)
                .IsUnicode(false);

            modelBuilder.Entity<Case>()
                .Property(e => e.JobNumber)
                .IsUnicode(false);

            modelBuilder.Entity<Case>()
                .Property(e => e.CaseSource)
                .IsUnicode(false);

            modelBuilder.Entity<Case>()
                .Property(e => e.CaseJobRunStatus)
                .IsUnicode(false);

            modelBuilder.Entity<Case>()
                .Property(e => e.CaseJobRunMsg)
                .IsUnicode(false);

            modelBuilder.Entity<Case>()
                .HasMany(e => e.Families)
                .WithRequired(e => e.Case)
                .HasForeignKey(e => e.CaseNumId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.GovtPlanNumber)
                .IsUnicode(false);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.Year)
                .IsUnicode(false);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.GrossAnnualPremium)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.FederalSubsidy)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.NetAnnualPremium)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.MonthlyPremium)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.Copays)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.PaymentsToDeductibleLimit)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.CoinsuranceToOutOfPocketLimit)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.ContributedToYourHSAAccount)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.TaxSavingFromHSAAccount)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.Medical)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.TotalPaid)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.PaymentsByInsuranceCo)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.DeductibleSingle)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.DeductibleFamilyPerPerson)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.DeductibleFamilyPerGroup)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.OPLSingle)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.OPLFamilyPerPerson)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.OPLFamilyPerGroup)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.Coinsurance)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.WorstCase)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.MedicalNetwork)
                .IsUnicode(false);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.PlanName)
                .IsUnicode(false);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.HRAReimbursedAmt)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.TotalEmployerContribution_Pre)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.TotalEmployerContribution_Post)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CasePlanResult>()
                .Property(e => e.ExcludedAmount)
                .HasPrecision(16, 2);

            modelBuilder.Entity<CaseStatusMst>()
                .Property(e => e.StatusCode)
                .IsUnicode(false);

            modelBuilder.Entity<CaseStatusMst>()
                .Property(e => e.Descr)
                .IsUnicode(false);

            modelBuilder.Entity<CategoryMst>()
                .Property(e => e.CategoryName)
                .IsUnicode(false);

            modelBuilder.Entity<ChipEligibility>()
                .Property(e => e.Age)
                .IsUnicode(false);

            modelBuilder.Entity<ChipEligibility>()
                .Property(e => e.FundingType)
                .IsUnicode(false);

            modelBuilder.Entity<ChipEligibility>()
                .Property(e => e.FundPercent)
                .HasPrecision(6, 2);

            modelBuilder.Entity<ChipEligibility>()
                .Property(e => e.StateCode)
                .IsUnicode(false);

            modelBuilder.Entity<ChipEligibility>()
                .Property(e => e.Businessyear)
                .IsUnicode(false);

            modelBuilder.Entity<ClientCompany>()
                .Property(e => e.CompanyName)
                .IsUnicode(false);

            modelBuilder.Entity<CostSharingReductionScheduleMst>()
                .Property(e => e.IncomePercentageFPL)
                .IsUnicode(false);

            modelBuilder.Entity<CostSharingReductionScheduleMst>()
                .Property(e => e.BusinessYear)
                .IsUnicode(false);

            modelBuilder.Entity<CriticalillnessMst>()
                .Property(e => e.IllnessName)
                .IsUnicode(false);

            modelBuilder.Entity<CriticalillnessMst>()
                .HasMany(e => e.Criticalillnesses)
                .WithRequired(e => e.CriticalillnessMst)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CSR_Rate_Mst>()
                .Property(e => e.PlanID)
                .IsUnicode(false);

            modelBuilder.Entity<CSR_Rate_Mst>()
                .Property(e => e.Age)
                .IsUnicode(false);

            modelBuilder.Entity<CSR_Rate_Mst>()
                .Property(e => e.MetalLevel_Old)
                .IsUnicode(false);

            modelBuilder.Entity<CSR_Rate_Mst>()
                .Property(e => e.PlanMarketingName_Old)
                .IsUnicode(false);

            modelBuilder.Entity<CSR_Rate_Mst>()
                .Property(e => e.MrktCover_Old)
                .IsUnicode(false);

            modelBuilder.Entity<CSR_Rate_Mst>()
                .Property(e => e.BusinessYear)
                .IsUnicode(false);

            modelBuilder.Entity<CSR_Rate_Mst>()
                .Property(e => e.EHBPercentTotalPremium)
                .HasPrecision(18, 8);

            modelBuilder.Entity<CSR_Rate_Mst>()
                .Property(e => e.StateCode_Old)
                .IsUnicode(false);

            modelBuilder.Entity<CSTMst>()
                .Property(e => e.Key)
                .IsUnicode(false);

            modelBuilder.Entity<CSTMst>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<CSTMst>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<EmployerMst>()
                .Property(e => e.EmployerName)
                .IsUnicode(false);

            modelBuilder.Entity<Family>()
                .Property(e => e.Individual)
                .IsUnicode(false);

            modelBuilder.Entity<Family>()
                .HasMany(e => e.BenefitUserDetails)
                .WithRequired(e => e.Family)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Family>()
                .HasMany(e => e.Criticalillnesses)
                .WithRequired(e => e.Family)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FedPovertyLevelMst>()
                .Property(e => e.FPL)
                .HasPrecision(19, 4);

            modelBuilder.Entity<FedPovertyLevelMst>()
                .Property(e => e.BusinessYear)
                .IsUnicode(false);

            modelBuilder.Entity<FPLBracketLookupMst>()
                .Property(e => e.FPLBracketLookup)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FPLBracketLookupMst>()
                .Property(e => e.IncomePercentageFPL)
                .IsUnicode(false);

            modelBuilder.Entity<FPLBracketLookupMst>()
                .Property(e => e.BusinessYear)
                .IsUnicode(false);

            modelBuilder.Entity<FPLCapMst>()
                .Property(e => e.BusinessYear)
                .IsUnicode(false);

            modelBuilder.Entity<InsuranceType>()
                .Property(e => e.InsuranceType1)
                .IsUnicode(false);

            modelBuilder.Entity<InsuranceType>()
                .HasMany(e => e.PlanAttributeMsts)
                .WithOptional(e => e.InsuranceType1)
                .HasForeignKey(e => e.InsuranceType);

            modelBuilder.Entity<IssuerMst>()
                .Property(e => e.IssuerCode)
                .IsUnicode(false);

            modelBuilder.Entity<IssuerMst>()
                .Property(e => e.IssuerName)
                .IsUnicode(false);

            modelBuilder.Entity<IssuerMst>()
                .Property(e => e.MappingNumber)
                .IsUnicode(false);

            modelBuilder.Entity<IssuerMst>()
                .Property(e => e.Abbreviations)
                .IsUnicode(false);

            modelBuilder.Entity<IssuerMst>()
                .Property(e => e.StateCode)
                .IsUnicode(false);

            modelBuilder.Entity<IssuerMst>()
                .HasMany(e => e.Cases)
                .WithOptional(e => e.IssuerMst)
                .HasForeignKey(e => e.IssuerID);

            modelBuilder.Entity<IssuerMst>()
                .HasMany(e => e.MHMBenefitMappingMsts)
                .WithOptional(e => e.IssuerMst)
                .HasForeignKey(e => e.IssuerID);

            modelBuilder.Entity<IssuerMst>()
                .HasMany(e => e.PlanAttributeMsts)
                .WithOptional(e => e.IssuerMst)
                .HasForeignKey(e => e.CarrierId);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.JobNumber)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.JobDesc)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.JobStatus)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.JobYear)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.EmailBodyText)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.EmailSubjText)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.EmailSignText)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.JobRunStatus)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.JobType)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.LastJobRunStatus)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.CaseZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.ComparisonJobNum)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.JobCopiedFrom)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HRAMaxReimbursePrimary)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HRAMaxReimburseDependent)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HRADedLimitPrimary)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HRADedLimitDependent)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HRAReimburseRatePrimary)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HRAReimburseRateDependent)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobPlansMst>()
                .Property(e => e.JobNumber)
                .IsUnicode(false);

            modelBuilder.Entity<JobMaster>()
               .Property(e => e.HSAMatchLimit1)
               .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HSAMatchRate1)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HSAMatchLimit2)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HSAMatchRate2)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HSAMatchLimit3)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HSAMatchRate3)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HSAMatchLimit4)
                .HasPrecision(12, 2);

            modelBuilder.Entity<JobMaster>()
                .Property(e => e.HSAMatchRate4)
                .HasPrecision(12, 2);


            modelBuilder.Entity<JobPlansMst>()
                .Property(e => e.PlanId)
                .IsUnicode(false);

            modelBuilder.Entity<JobPlansMst>()
                .Property(e => e.BusinessYear)
                .IsUnicode(false);

            modelBuilder.Entity<MedicaidEligibility>()
                .Property(e => e.WithChildren)
                .HasPrecision(6, 2);

            modelBuilder.Entity<MedicaidEligibility>()
                .Property(e => e.WithoutChildren)
                .HasPrecision(6, 2);

            modelBuilder.Entity<MedicaidEligibility>()
                .Property(e => e.StateCode)
                .IsUnicode(false);

            modelBuilder.Entity<MHMBenefitCostByAreaMst>()
                .Property(e => e.MHMBenefitCost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<MHMBenefitCostByAreaMst>()
                .Property(e => e.StateCode)
                .IsUnicode(false);

            modelBuilder.Entity<MHMBenefitMappingMst>()
                .Property(e => e.IssuerBenefitVersion)
                .IsUnicode(false);

            modelBuilder.Entity<MHMCommonBenefitsMst>()
                .Property(e => e.MHMBenefitName)
                .IsUnicode(false);

            modelBuilder.Entity<MHMCommonBenefitsMst>()
                .HasMany(e => e.BenefitUserDetails)
                .WithRequired(e => e.MHMCommonBenefitsMst)
                .HasForeignKey(e => e.MHMMappingBenefitId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MHMCommonBenefitsMst>()
                .HasMany(e => e.MHMBenefitCostByAreaMsts)
                .WithRequired(e => e.MHMCommonBenefitsMst)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MHMCommonBenefitsMst>()
                .HasMany(e => e.MHMBenefitMappingMsts)
                .WithRequired(e => e.MHMCommonBenefitsMst)
                .HasForeignKey(e => e.MHMCommonBenefitID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.PlanId)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.StandardComponentId)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.BusinessYear)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.MrktCover)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.PlanMarketingName)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.HIOSProductId)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.ServiceAreaId)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.MetalLevel)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.QHPNonQHPTypeId)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.HSAOrHRAEmployerContribution_Old)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.CSRVariationType)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.URLForSummaryofBenefitsCoverage)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.FormularyURL)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.NetworkURL)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.PlanBrochure)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.PlanNumber)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.MappingNumber_Old)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.GroupName)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .Property(e => e.StateCode)
                .IsUnicode(false);

            modelBuilder.Entity<PlanAttributeMst>()
                .HasMany(e => e.JobPlansMsts)
                .WithOptional(e => e.PlanAttributeMst)
                .HasForeignKey(e => new { e.PlanId, e.BusinessYear });

            modelBuilder.Entity<PlanAttributeMst>()
                .HasMany(e => e.PlanBenefitMsts)
                .WithOptional(e => e.PlanAttributeMst)
                .HasForeignKey(e => new { e.PlanId, e.BusinessYear });

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.BenefitKey_Old)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.BusinessYear)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.BenefitName)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.CopayInnTier1Desc)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.CopayInnTier1Type_Old)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.CoinsInnTier1Desc)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.MarketConverage_Old)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.SourceName)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.StandardComponentId_Old)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.StateCode_Old)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.PlanId)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.LimitUnit)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.Exclusions)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.BenefitDeductible)
                .HasPrecision(18, 0);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.CostSharingType1)
                .IsUnicode(false);

            modelBuilder.Entity<PlanBenefitMst>()
                .Property(e => e.CostSharingType2)
                .IsUnicode(false);

            modelBuilder.Entity<PlanMaster>()
                .Property(e => e.PlanType)
                .IsUnicode(false);

            modelBuilder.Entity<PlanMaster>()
                .HasMany(e => e.PlanAttributeMsts)
                .WithOptional(e => e.PlanMaster)
                .HasForeignKey(e => e.PlanType);

            modelBuilder.Entity<Rule>()
                .Property(e => e.RuleName)
                .IsUnicode(false);

            modelBuilder.Entity<Rule>()
                .Property(e => e.ClassName)
                .IsUnicode(false);

            modelBuilder.Entity<tblRatingAreaMst>()
                .Property(e => e.RatingAreaName)
                .IsUnicode(false);

            modelBuilder.Entity<tblRatingAreaMst>()
                .HasMany(e => e.tblRatingAreas)
                .WithRequired(e => e.tblRatingAreaMst)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblStateAbrev>()
                .Property(e => e.StateName)
                .IsUnicode(false);

            modelBuilder.Entity<tblStateAbrev>()
                .Property(e => e.StateCode)
                .IsUnicode(false);

            modelBuilder.Entity<tblStateAbrev>()
                .Property(e => e.FipsState)
                .IsUnicode(false);

            modelBuilder.Entity<tblStateAbrev>()
                .Property(e => e.EntityType)
                .IsUnicode(false);

            modelBuilder.Entity<tblStateAbrev>()
                .Property(e => e.IsoCode)
                .IsUnicode(false);

            modelBuilder.Entity<tblStateAbrev>()
                .Property(e => e.Businessyear)
                .IsUnicode(false);

            modelBuilder.Entity<tblStateAbrev>()
                .HasMany(e => e.CSR_Rate_Mst)
                .WithOptional(e => e.tblStateAbrev)
                .HasForeignKey(e => e.StateCode_Old);

            modelBuilder.Entity<User>()
                .HasMany(e => e.AuditLogs)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblRatingArea>()
                .Property(e => e.MarketCoverage)
                .IsUnicode(false);

            modelBuilder.Entity<tblRatingArea>()
                .Property(e => e.CountyName)
                .IsUnicode(false);

            modelBuilder.Entity<tblRatingArea>()
                .Property(e => e.ThreeDigitZipCode)
                .IsUnicode(false);

            modelBuilder.Entity<tblRatingArea>()
                .Property(e => e.StateCode)
                .IsUnicode(false);

            modelBuilder.Entity<tblRatingArea>()
                .Property(e => e.Businessyear)
                .IsUnicode(false);

            modelBuilder.Entity<tblZipCode>()
                .Property(e => e.Zip)
                .IsUnicode(false);

            modelBuilder.Entity<tblZipCode>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<tblZipCode>()
                .Property(e => e.County)
                .IsUnicode(false);

            modelBuilder.Entity<tblZipCode>()
                .Property(e => e.City)
                .IsUnicode(false);
            modelBuilder.Entity<qryZipCodeToRatingArea>()
                .Property(e => e.County)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipCodeToRatingArea>()
                .Property(e => e.Zip)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipCodeToRatingArea>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipCodeToRatingArea>()
                .Property(e => e.CountyName)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipCodeToRatingArea>()
                .Property(e => e.StateCode)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipCodeToRatingArea>()
                .Property(e => e.StateName)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipCodeToRatingArea>()
                .Property(e => e.RatingAreaName)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipCodeToRatingArea>()
                .Property(e => e.MarketCoverage)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipStateCounty>()
                .Property(e => e.County)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipStateCounty>()
                .Property(e => e.Zip)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipStateCounty>()
                .Property(e => e.City)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipStateCounty>()
                .Property(e => e.State)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipStateCounty>()
                .Property(e => e.CountyName)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipStateCounty>()
                .Property(e => e.StateName)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipStateCounty>()
                .Property(e => e.ZIP3)
                .IsUnicode(false);

            modelBuilder.Entity<qryZipStateCounty>()
                .Property(e => e.StateCode)
                .IsUnicode(false);

        }
    }
}