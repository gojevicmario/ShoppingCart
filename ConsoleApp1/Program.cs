using ShoppingCartVersion3.Models.Controllers;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var cart = new CartController();

            cart.AddCartProduct(1);
            cart.AddCartProduct(1);


            cart.AddCartProduct(3);

            for (int i = 0; i < 8; i++)
            {
                cart.AddCartProduct(2);
            }

            cart.RemoveCartProduct(1);
            cart.RemoveCartProduct(1);
            Console.WriteLine(cart.GetTotalPrice());
            cart.RemoveCartProduct(2);
        }
    }
}
