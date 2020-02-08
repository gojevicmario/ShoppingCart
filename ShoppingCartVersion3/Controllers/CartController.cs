using ShoppingCartVersion3.Enums;
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

        public void AddCartProduct(int productId)
        {
            AddCartProduct(productId, 0);
            var promotions = IsPromotionAvailable(productId);

            if (promotions.Any(p => p.Equals(ProductType.Product)))
                ApplyPromotion(productId,ProductType.Product);
            else if(promotions.Any(p => p.Equals(ProductType.PromoProduct)))
                ApplyPromotion(productId, ProductType.PromoProduct);

        }

        private void AddCartProduct(int productId, int promotionId)
        {
            bool isPromotion = promotionId > 0 ? true : false;

            if (IsProductInCart(productId, isPromotion))
                CartProducts.FirstOrDefault(p => p.Id == productId && p.IsPromotion == isPromotion).Quantity++;
            else if(isPromotion)
                CartProducts.Add(new CartProduct(productId, promotionId));
            else
                CartProducts.Add(new CartProduct(productId));
            
        }

        public void RemoveCartProduct(int productId)
        {
            if (!ProductPromotionRepository.GetProductPromotions().Any(p => p.ProductId == productId))
                RemoveCartProduct(productId, false);
            else
            {
                var productPromotion = ProductPromotionRepository.GetProductPromotionByProductId(productId);
                var promotion = PromotionRepository.GetPromotion(productPromotion.PromotionId);
                if(IsProductInCart(productPromotion.PromotionalProductId) &&
                    (!ArePromotionRequirementsMet(productPromotion.ProductId, productPromotion.PromotionId) 
                    && IsProductInCart(productPromotion.PromotionalProductId, true)))
                {
                    RemoveCartProduct(productId, false);
                    RemoveCartProduct(productPromotion.PromotionalProductId, true);
                    AddCartProduct(productPromotion.PromotionalProductId);
                }
                else {
                    RemoveCartProduct(productId, false);
                }
            }
        }

        private void RemoveCartProduct(int productId, bool isPromotion)
        {
            var cartProduct = CartProducts.FirstOrDefault(p => p.Id == productId && p.IsPromotion == isPromotion);
            if (cartProduct == null)
                throw new InvalidOperationException();
            if (cartProduct.Quantity == 1)
                CartProducts.Remove(cartProduct);
            else
                cartProduct.Quantity--;
        }

        public List<CartProduct> GetCartProducts(int productId)
        {
            return CartProducts.Where(product => product.Id == productId).ToList();
        }

        public double GetTotalPrice()
        {

            LogCartDetails();
            return CalculateTotalPrice();
        }

        private void LogCartDetails()
        {
            Console.WriteLine("Current state of cart");
            var temp = CartProducts.GroupBy(product => product.Id);
            
            foreach (var cartProduct in temp)
            {
                var totalQuantity = cartProduct.Sum(p => p.Quantity);
                Console.Write($"{cartProduct.First().ToString()}\n" +
                    $"product Quantity: {cartProduct.Sum(p => p.Quantity)}\n" +
                    $"cost of products: {cartProduct.Sum(p => p.Price * p.Quantity)}\n");
                if(cartProduct.Any( p => p.IsPromotion) && ProductPromotionRepository.GetProductPromotionByProductId(cartProduct.First().Id) != null)
                    Console.Write($"Applied promotion id {ProductPromotionRepository.GetProductPromotionByProductId(cartProduct.First().Id).PromotionId}\n");
                else if(cartProduct.Any(p => p.IsPromotion) && ProductPromotionRepository.GetProductPromotionByPromoProductId(cartProduct.First().Id) != null)
                    Console.Write($"Applied promotion id {ProductPromotionRepository.GetProductPromotionByPromoProductId(cartProduct.First().Id).PromotionId}\n");
            }
        }

        private double CalculateTotalPrice()
        {
            return Math.Round(CartProducts.Sum(p => p.Quantity * p.Price), 2);
        }
        private int GetNumberOfProductsInCart(int productId)
        {
            return GetCartProducts(productId).Sum(p => p.Quantity);
        }
        private List<ProductType> IsPromotionAvailable(int lastAlteredproductId)
        {
            List<ProductType> types = new List<ProductType>();
            var productPromotionByProductId = ProductPromotionRepository.GetProductPromotionByProductId(lastAlteredproductId);
            if(productPromotionByProductId != null &&
                ArePromotionRequirementsMet(productPromotionByProductId.ProductId, productPromotionByProductId.PromotionId))
            {
                    types.Add(ProductType.Product);

            }
            var productPromotionByPromoProductId = ProductPromotionRepository.GetProductPromotionByPromoProductId(lastAlteredproductId);
            if(productPromotionByPromoProductId != null && ArePromotionRequirementsMet(productPromotionByPromoProductId.ProductId, productPromotionByPromoProductId.PromotionId))
            {
                    types.Add(ProductType.PromoProduct);
            }

            return types;
        }

        private bool ArePromotionRequirementsMet(int productId, int promotionId)
        {
            var promotion = PromotionRepository.GetPromotion(promotionId);
            var productPromotion = ProductPromotionRepository.GetProductPromotionByPromotionId(promotionId);

            int numberOfProducts = GetNumberOfProductsInCart(productId);
            int numberOfRequiredproducts = promotion.NumberOfRequiredproducts;
            int numberOfPromotionalProductsInCart = GetCartProducts(productPromotion.PromotionalProductId).FirstOrDefault(p => p.IsPromotion) == null ? 0 : GetCartProducts(productPromotion.PromotionalProductId).FirstOrDefault(p => p.IsPromotion).Quantity;

            if (GetCartProducts(productPromotion.PromotionalProductId).FirstOrDefault(p => p.IsPromotion) != null && GetCartProducts(productPromotion.PromotionalProductId).FirstOrDefault(p => p.IsPromotion).Quantity >= promotion.MaximumOccurances)
                return false;
            return numberOfProducts >= numberOfRequiredproducts && (numberOfPromotionalProductsInCart == 0 || (numberOfProducts / numberOfRequiredproducts > numberOfPromotionalProductsInCart));
        }

        private void ApplyPromotion(int lastAlteredproductId, ProductType productType)
        {
            var cartProducts = GetCartProducts(lastAlteredproductId);
            ProductPromotion productPromotion;
            if (productType == ProductType.Product)
                productPromotion = ProductPromotionRepository.GetProductPromotionByProductId(lastAlteredproductId);
            else if (productType == ProductType.PromoProduct)
                productPromotion = ProductPromotionRepository.GetProductPromotionByPromoProductId(lastAlteredproductId);
            else
                throw new Exception("greska");

            var promotion = PromotionRepository.GetPromotion(productPromotion.PromotionId);

            if(IsProductInCart(productPromotion.PromotionalProductId, false))
            {
                RemoveCartProduct(productPromotion.PromotionalProductId);
                AddCartProduct(productPromotion.PromotionalProductId, productPromotion.PromotionId);
            }

        }
        private bool IsProductInCart(int productId, bool isPromotion)
        {
            return CartProducts.Any(p => p.Id == productId && p.IsPromotion == isPromotion);
        }

        private bool IsProductInCart(int productId)
        {
            return CartProducts.Any(p => p.Id == productId);
        }
    }
}
