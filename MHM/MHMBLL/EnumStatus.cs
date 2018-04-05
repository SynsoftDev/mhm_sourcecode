using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHMBLL
{
    public class EnumStatusModel : Attribute
    {
        internal enum CalculationType
        {
            Subsidy = 1,
            Premium = 2
        }

        public enum CaseApprovalStatus
        {
            [EnumMember(Value = "Not Reviewed")]
            NotReviewed = 1,
            [EnumMember(Value = "Reviewed, not Tested")]
            ReviewednotTested = 2,
            [EnumMember(Value = "Tested, Errors to Address")]
            TestedErrorstoAddress = 3,
            [EnumMember(Value = "Tested, Approved")]
            TestedApproved = 4,
            [EnumMember(Value = "In Production")]
            InProduction = 5
        }

    }

    // Summary:
    //     Specifies that the field is an enumeration member and should be serialized.
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumMemberAttribute : Attribute
    {

        // Summary:
        //     Gets or sets the value associated with the enumeration member the attribute
        //     is applied to.
        //
        // Returns:
        //     The value associated with the enumeration member.
        public string Value { get; set; }
    }


}