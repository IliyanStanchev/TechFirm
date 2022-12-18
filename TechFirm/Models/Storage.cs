using System.Collections.Generic;
using TechFirm.Models;

namespace TechFirm
{
    public class Storage
    {
        public int Id { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public override string ToString()
        {
            return Address;
        }
    }
}