using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechFirm.Models
{
    public class Delivery
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public virtual Storage Storage { get; set; }

        public virtual Provider Provider { get; set; }

    }
}
