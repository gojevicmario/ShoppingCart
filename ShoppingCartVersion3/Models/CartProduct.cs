using ShoppingCartVersion3.Repositories;
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

        public CartProduct(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Price = product.Price;
            Quantity = 1;
            IsPromotion = false;
        }

        public CartProduct(int baseProductId)
        {
            var baseProduct = ProductRepository.GetProduct(baseProductId);
            Id = baseProduct.Id;
            Name = baseProduct.Name;
            Price = baseProduct.Price;
            Quantity = 1;
            IsPromotion = false;
        }

        public CartProduct(int baseProductId,int promotionId)
        {
            var baseProduct = ProductRepository.GetProduct(baseProductId);
            var promotion = PromotionRepository.GetPromotion(promotionId);

            Id = baseProduct.Id;
            Name = baseProduct.Name;
            Price = GetPriceWithPromotion(baseProduct.Price, promotion.Amount);
            Quantity = 1;
            IsPromotion = true;
        }

        private double GetPriceWithPromotion(double oldPrice, int amount)
        {
            return oldPrice * ((100 - amount) / (double)100);
        }

        public override string ToString()
        {
            return $"\nProductId: {Id}\nName:{Name}\nbase price: {Price}";
        }
    }
}
