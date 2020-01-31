using ShoppingCartVersion3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartVersion3.Repositories
{
    public static class ProductRepository
    {
        private static List<Product> _products = new List<Product>() {
            new Product()
            {
                Id = 1,
                Name = "Butter",
                Price = 0.8
            },
            new Product()
            {
                Id = 2 ,
                Name = "Milk",
                Price = 1.15,
            },
            new Product()
            {
                Id = 3,
                Name = "Bread",
                Price = 1
            }
        };
    }
}
