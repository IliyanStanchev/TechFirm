using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechFirm.Models
{
    public class AccountInformation
    {
        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime LastPasswordChangeDate { get; set; }

        public virtual User User { get; set; }
    }
}
