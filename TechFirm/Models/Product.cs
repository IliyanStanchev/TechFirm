using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechFirm.Models;

namespace TechFirm
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public int MinimalCount { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
