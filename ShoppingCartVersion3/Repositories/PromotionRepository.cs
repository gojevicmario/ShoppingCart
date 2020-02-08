using ShoppingCartVersion3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartVersion3.Repositories
{
    public static class PromotionRepository
    {
        private static List<Promotion> _promotions = new List<Promotion>() {
            new Promotion()
            {
                Id = 1,
                NumberOfRequiredproducts = 2,
                Amount = 50,
                MaximumOccurances = 1
            },
            new Promotion()
            {
                Id = 2,
                NumberOfRequiredproducts = 4,
                Amount = 100,
                MaximumOccurances = int.MaxValue
            }
        };

        public static Promotion GetPromotion(int id)
        {
            return _promotions.FirstOrDefault(p => p.Id == id);
        }
    }
}
