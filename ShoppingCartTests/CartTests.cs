using System;
using NUnit.Framework;
using ShoppingCartVersion3.Models.Controllers;
using ShoppingCartVersion3.Repositories;

namespace ShoppingCartTests
{
    [TestFixture]
    public class CartTests
    {
        [TestCase]
        public void AddProductToCart()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(1,0);

            Assert.That(cart.GetCartProducts(1)[0].Quantity, Is.EqualTo(1));
        }

        [TestCase]
        public void AddTwoSameProductsToCart()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(1,0);
            cart.AddCartProduct(1,0);

            Assert.That(cart.GetCartProducts(1)[0].Quantity, Is.EqualTo(2));
        }

        [TestCase]
        public void AddOnePromoProductAndOneRegularProductToCart()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(2, 2);
            cart.AddCartProduct(2, 0);

            Assert.That(cart.CartProducts.Count, Is.EqualTo(2));
        }

        [TestCase]
        public void AddTwoProductsAndDeleteOne()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(1, 0);
            cart.AddCartProduct(1, 0);

            cart.RemoveCartProduct(1, 0);
            Assert.That(cart.GetCartProducts(1)[0].Quantity, Is.EqualTo(1));
        }

        [TestCase]
        public void CalculateTotalPrice()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(2, 2);
            cart.AddCartProduct(2, 0);
            cart.AddCartProduct(2, 0);

            Assert.That(cart.GetTotalPrice(), Is.EqualTo(cart.CartProducts[0].Price * cart.CartProducts[0].Quantity + cart.CartProducts[1].Price * cart.CartProducts[1].Quantity));
        }

    }
}
