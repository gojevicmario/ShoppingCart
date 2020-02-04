﻿using ShoppingCartVersion3.Enums;
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
                ApplyPromotionRefactor(productId,ProductType.Product);
            else if(promotions.Any(p => p.Equals(ProductType.PromoProduct)))
                ApplyPromotionRefactor(productId, ProductType.PromoProduct);

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
            return Math.Round(CartProducts.Sum(p => p.Quantity * p.Price),2);
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
        private void ApplyPromotion(int lastAlteredItemId)
        {
            var productPromotion = ProductPromotionRepository.GetProductPromotionByProductId(lastAlteredItemId);
            var promotion = PromotionRepository.FindById(productPromotion.PromotionId);
            var cartProducts = GetCartProducts(lastAlteredItemId);

            if (promotion == null)
                throw new InvalidOperationException();
            if(cartProducts.Single(p => p.IsPromotion == false).Quantity >= promotion.NumberOfRequiredItems && (cartProducts.Single(p => p.IsPromotion == false).Quantity / (double)promotion.NumberOfRequiredItems) % 1 == 0)
            {
                if((cartProducts.SingleOrDefault(p => p.IsPromotion == true) == null ||
                    cartProducts.SingleOrDefault(p => p.IsPromotion == true).Quantity < promotion.MaximumOccurances) && 
                    IsProductInCart(productPromotion.PromotionalProductId,false))
                {
                    //Prođi opet kroz ovu logiku, sad obriše i doda promo, mozda bi trebalo samo dodat promo?
                    // vjerovatno bi trebalo prvo ga dodat pa onda runnat ovaj kurac da ga moze prebacit u pravu listu
                    RemoveCartProduct(productPromotion.PromotionalProductId, 0);
                    AddCartProduct(productPromotion.PromotionalProductId, promotion.Id);
                }
                else
                {
                    AddCartProduct(productPromotion.PromotionalProductId, productPromotion.PromotionId);
                }
            }
            else
            {
                throw new NotImplementedException("Remove discount");
            }

        }

        private void ApplyPromotionRefactor(int lastAlteredItemId, ProductType productType)
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
