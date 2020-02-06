using System;
using System.Linq;
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

            cart.AddCartProduct(1);

            Assert.That(cart.GetCartProducts(1)[0].Quantity, Is.EqualTo(1));
        }

        [TestCase]
        public void AddTwoSameProductsToCart()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(1);
            cart.AddCartProduct(1);

            Assert.That(cart.GetCartProducts(1)[0].Quantity, Is.EqualTo(2));
        }

        [TestCase]
        public void AddTwoProductsAndDeleteOne()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(1);
            cart.AddCartProduct(1);

            cart.RemoveCartProduct(1);
            Assert.That(cart.GetCartProducts(1)[0].Quantity, Is.EqualTo(1));
        }

        [TestCase]
        public void CalculateTotalPrice()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(2);
            cart.AddCartProduct(2);
            cart.AddCartProduct(2);

            Assert.That(cart.GetTotalPrice(), Is.EqualTo(Math.Round(ProductRepository.GetProduct(2).Price * 3,2)));
        }

        [TestCase]
        public void AddFourMilksFourthShouldBeFree()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(2);
            cart.AddCartProduct(2);
            cart.AddCartProduct(2);
            cart.AddCartProduct(2);

            Assert.Multiple(() =>
            {
                Assert.That(cart.GetTotalPrice(), Is.EqualTo(Math.Round(ProductRepository.GetProduct(2).Price * 3, 2)));
                Assert.That(cart.GetCartProducts(2).Single(p => p.IsPromotion).Price, Is.EqualTo(0));
            });
        }


        [TestCase]
        public void CaseA()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(3);
            cart.AddCartProduct(2);
            cart.AddCartProduct(1);

            Assert.That(cart.GetTotalPrice(), Is.EqualTo((double)2.95));
        }

        [TestCase]
        public void CaseB()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(3);
            cart.AddCartProduct(3);
            cart.AddCartProduct(1);
            cart.AddCartProduct(1);

            Assert.That(cart.GetTotalPrice(), Is.EqualTo((double)3.1));
        }

        [TestCase]
        public void CaseC()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(2);
            cart.AddCartProduct(2);
            cart.AddCartProduct(2);
            cart.AddCartProduct(2);

            Assert.That(cart.GetTotalPrice(), Is.EqualTo((double)3.45));
        }

        [TestCase]
        public void CaseD()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(1);
            cart.AddCartProduct(1);

            cart.AddCartProduct(3);

            for (int i = 0; i < 8; i++)
            {
                cart.AddCartProduct(2);
            }

            Assert.That(cart.GetTotalPrice(), Is.EqualTo((double)9));
        }

        [TestCase]
        public void CaseE()
        {
            CartController cart = new CartController();

            cart.AddCartProduct(1);
            cart.AddCartProduct(1);


            cart.AddCartProduct(3);

            for (int i = 0; i < 8; i++)
            {
                cart.AddCartProduct(2);
            }

            cart.RemoveCartProduct(1);
            cart.RemoveCartProduct(1);
            cart.RemoveCartProduct(2);

            Assert.That(cart.GetTotalPrice(), Is.EqualTo((double)7.9));
        }
    }
}
