using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartVersion3.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public int MaximumOccurances { get; set; }
        public int NumberOfRequiredItems { get; set; }

        public Promotion()
        {

        }
    }
}
