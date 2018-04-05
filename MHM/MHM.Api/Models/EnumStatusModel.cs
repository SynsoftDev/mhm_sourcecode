using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MHM.Api.Models
{
    public class EnumStatusModel
    {
        public enum CalculationType
        {
            Subsidy = 1,
            Premium = 2
        }

        public enum JobStatus
        {
            New,
            Open,
            Closed,
            Billed,
            Cancelled
        }

        public enum CaseJobRunStatus
        {
            NULL,
            GeneratedOnly,
            ReportSent,
            SendError,
            GenError
        }

        public enum JobRunStatus
        {
            NULL,
            AllGenerated,
            AllSent,
            SendErrors,
            GenErrors
        }

        public enum CostSharingType
        {
            DeductibleOnly,
            DeductibleThenCopay,
            DeductibleThenCoIns,
            CopayBeforeDedThenNoCharge,
            CopayBeforeDedThenCoInsAfterDed_DedApplied,
            CopayBeforeDedThenCoInsAfterDed_DedNotApplied,
            CopayOnly,
            CopayOnlyExcludedFromMOOP,
            CoInsOnly,
            NoCharge
        }

        public enum LimitUnit
        {
            QtyYear,
            Qty6mnth,
            QtyMnth,
            _Year,
            _6mnth,
            _Mnth
        }

    }
}