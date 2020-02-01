using ShoppingCartVersion3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartVersion3.Repositories
{
    public static class ProductPromotionRepository
    {
        private static List<ProductPromotion> _productPromotions = new List<ProductPromotion>() {
            new ProductPromotion()
            {
                Id = 1,
                ProductId = 1,
                PromotionalProductId = 3,
                PromotionId = 1
            },
            new ProductPromotion()
            {
                Id = 2,
                ProductId = 2,
                PromotionalProductId =2,
                PromotionId = 2
            }
        };

        public static IEnumerable<ProductPromotion> GetProductPromotions()
        {
            return _productPromotions;
        }

        public static ProductPromotion GetProductPromotionById(int id)
        {
            return _productPromotions.FirstOrDefault(p => p.Id == id);
        }

        public static ProductPromotion GetProductPromotionByProductId(int id)
        {
            return _productPromotions.FirstOrDefault(p => p.ProductId == id);
        }

        public static ProductPromotion GetProductPromotionByPromoProductId(int id)
        {
            return _productPromotions.FirstOrDefault(p => p.PromotionalProductId == id);
        }
        public static ProductPromotion GetProductPromotionByPromotionId(int id)
        {
            return _productPromotions.FirstOrDefault(p => p.PromotionId == id);
        }


    }
}
