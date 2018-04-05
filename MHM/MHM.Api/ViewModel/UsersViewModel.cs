using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHM.Api.ViewModel
{
    public class UsersViewModel
    {
        public long id { get; set; }
        public string Phone { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool UserLockedStatus { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
