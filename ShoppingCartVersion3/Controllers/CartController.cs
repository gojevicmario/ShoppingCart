using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartVersion3.Models.Controllers
{
    public class CartController
    {
        public List<CartProduct> CartProducts;

        public CartController()
        {
            CartProducts = new List<CartProduct>();
        }

        public void AddCartProduct(int cartProductId)
        {
            throw new NotImplementedException();
        }

        public void RemoveCartProduct(int cartProductId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CartProduct> GetCartProducts(int itemId)
        {
            return CartProducts.Where(item => item.Id == itemId);
        }

        public double GetTotalPrice()
        {
            throw new NotImplementedException();
        }

        private bool IsPromotionAvailable(int lastAddedItemId)
        {
            throw new NotImplementedException();
        }
        
        private void ApplyPromotion(int productDiscountPromotionId)
        {
            throw new NotImplementedException();
        }

    }
}
