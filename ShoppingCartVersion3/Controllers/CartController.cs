using ShoppingCartVersion3.Repositories;
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

        public void AddCartProduct(int productId, int promotionId)
        {
            bool isPromotion = promotionId > 0 ? true : false;

            if (IsProductInCart(productId, isPromotion))
                CartProducts.FirstOrDefault(p => p.Id == productId && p.IsPromotion == isPromotion).Quantity++;
            else if(isPromotion)
                CartProducts.Add(new CartProduct(productId, promotionId));
            else
                CartProducts.Add(new CartProduct(productId));
        }

        public void RemoveCartProduct(int cartProductId, int promotionId)
        {
            bool isPromotion = promotionId > 0 ? true : false;

            var cartProduct = CartProducts.FirstOrDefault(p => p.Id == cartProductId && p.IsPromotion == isPromotion);
            if (cartProduct == null)
                throw new InvalidOperationException();
            if (cartProduct.Quantity == 1)
                CartProducts.Remove(cartProduct);
            else
                cartProduct.Quantity--;
        }

        public List<CartProduct> GetCartProducts(int itemId)
        {
            return CartProducts.Where(item => item.Id == itemId).ToList();
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

        private bool IsProductInCart(int cartProductId, bool isPromotion)
        {
            return CartProducts.Any(p => p.Id == cartProductId && p.IsPromotion == isPromotion);
        }

    }
}
