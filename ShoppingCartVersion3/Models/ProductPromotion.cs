using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartVersion3.Models
{
    public class ProductPromotion
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int PromotionalProductId { get; set; }
        public int PromotionId { get; set; }
    }
}
