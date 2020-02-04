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
            //temp primitivna metoda

            var cartProduct = CartProducts.FirstOrDefault(p => p.Id == productId && p.IsPromotion == false);
            if (cartProduct == null)
                throw new InvalidOperationException();
            if (cartProduct.Quantity == 1)
                CartProducts.Remove(cartProduct);
            else
                cartProduct.Quantity--;
        }

        public void RemoveCartProductRefactor(int productId)
        {

            var cartProducts = GetCartProducts(productId);

            if(cartProducts.Exists(p => p.IsPromotion))
            {

            }
            else if (ProductPromotionRepository.GetProductPromotionByPromoProductId(productId).PromotionalProductId == productId)
            {
                //ako je productId zapravo promo produkt nekog drugog proizvoda
            }
            else
            {
                var cartProduct = CartProducts.FirstOrDefault(p => p.Id == productId && p.IsPromotion == false);
                if (cartProduct == null)
                    throw new InvalidOperationException();
                if (cartProduct.Quantity == 1)
                    CartProducts.Remove(cartProduct);
                else
                    cartProduct.Quantity--;
            }
            //var cartProduct = CartProducts.FirstOrDefault(p => p.Id == productId && p.IsPromotion == false);
            //if (cartProduct == null)
            //    throw new InvalidOperationException();
            //if (cartProduct.Quantity == 1)
            //    CartProducts.Remove(cartProduct);
            //else
            //    cartProduct.Quantity--;

            //var promotionTypesForProduct = IsPromotionAvailable(productId);

            //if(promotionTypesForProduct.Count != 0)
            //{
            //    if (promotionTypesForProduct.Contains(ProductType.Product) && 
            //        !ArePromotionRequirementsMet(ProductPromotionRepository.GetProductPromotionByProductId(productId).ProductId,
            //                                    ProductPromotionRepository.GetProductPromotionByProductId(productId).PromotionId))
            //    {
            //        if(IsProductInCart(ProductPromotionRepository.GetProductPromotionByProductId(productId).ProductId, true))
            //        {

            //        }
            //    }
            //    else if(promotionTypesForProduct.Contains(ProductType.PromoProduct) &&
            //        !ArePromotionRequirementsMet(ProductPromotionRepository.GetProductPromotionByPromoProductId(productId).PromotionalProductId,
            //                                    ProductPromotionRepository.GetProductPromotionByPromoProductId(productId).PromotionId))
            //    {

            //    }
            //}
        }

        private void RemoveCartProduct(int productId, int promotionId)
        {
            bool isPromotion = promotionId > 0 ? true : false;

            var cartProduct = CartProducts.FirstOrDefault(p => p.Id == productId && p.IsPromotion == isPromotion);
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

            LogCartDetails();
            return CalculateTotalPrice();
        }

        private void LogCartDetails()
        {
            Console.WriteLine("Current state of cart");
            var temp = CartProducts.GroupBy(item => item.Id);
            
            foreach (var cartProduct in temp)
            {
                var totalQuantity = cartProduct.Sum(p => p.Quantity);
                Console.WriteLine($"{cartProduct.First().ToString()}\n" +
                    $"item Quantity: {cartProduct.Sum(p => p.Quantity)} \n" +
                    $"Total price: {cartProduct.Sum(p => p.Price * p.Quantity)}\n");
                if(cartProduct.Any( p => p.IsPromotion) && ProductPromotionRepository.GetProductPromotionByProductId(cartProduct.First().Id) != null)
                    Console.Write($"Applied promotion id {ProductPromotionRepository.GetProductPromotionByProductId(cartProduct.First().Id).PromotionId}");
                else if(cartProduct.Any(p => p.IsPromotion) && ProductPromotionRepository.GetProductPromotionByPromoProductId(cartProduct.First().Id) != null)
                    Console.Write($"Applied promotion id {ProductPromotionRepository.GetProductPromotionByPromoProductId(cartProduct.First().Id).PromotionId}");
            }
        }

        private double CalculateTotalPrice()
        {
            return Math.Round(CartProducts.Sum(p => p.Quantity * p.Price), 2);
        }

        private List<ProductType> IsPromotionAvailable(int lastAlteredItemId)
        {
            List<ProductType> types = new List<ProductType>();
            var productPromotionByProductId = ProductPromotionRepository.GetProductPromotionByProductId(lastAlteredItemId);
            if(productPromotionByProductId != null &&
                ArePromotionRequirementsMet(productPromotionByProductId.ProductId, productPromotionByProductId.PromotionId))
            {
                    types.Add(ProductType.Product);

            }
            var productPromotionByPromoProductId = ProductPromotionRepository.GetProductPromotionByPromoProductId(lastAlteredItemId);
            if(productPromotionByPromoProductId != null && ArePromotionRequirementsMet(productPromotionByPromoProductId.ProductId, productPromotionByPromoProductId.PromotionId))
            {
                    types.Add(ProductType.PromoProduct);
            }

            return types;
        }

        private bool ArePromotionRequirementsMet(int itemId, int promotionId)
        {
            var promotion = PromotionRepository.FindById(promotionId);

            int numberOfProducts = GetCartProducts(itemId).Sum(p => p.Quantity);

            return numberOfProducts >= promotion.NumberOfRequiredItems && (numberOfProducts / (double)promotion.NumberOfRequiredItems) % 1 == 0;

        }

        private void ApplyPromotion(int lastAlteredItemId, ProductType productType)
        {
            var cartProducts = GetCartProducts(lastAlteredItemId);
            ProductPromotion productPromotion;
            if (productType == ProductType.Product)
                productPromotion = ProductPromotionRepository.GetProductPromotionByProductId(lastAlteredItemId);
            else if (productType == ProductType.PromoProduct)
                productPromotion = ProductPromotionRepository.GetProductPromotionByPromoProductId(lastAlteredItemId);
            else
                throw new Exception("greska");

            var promotion = PromotionRepository.FindById(productPromotion.PromotionId);

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

    }
}
