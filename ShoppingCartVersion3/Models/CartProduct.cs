using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartVersion3.Models
{
    public class CartProduct : Product
    {
        public int Quantity { get; set; }
        public bool IsPromotion { get; set; }
    }
}
