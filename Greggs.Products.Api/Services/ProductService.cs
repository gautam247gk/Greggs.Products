using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;

namespace Greggs.Products.Api.Services;

public class ProductService : IProductService
{
    private readonly IDataAccess<Product> _productAccess;
    private const decimal GbpToEurRate = 1.11m;

    public ProductService(IDataAccess<Product> productAccess)
    {
        _productAccess = productAccess;
    }

    public IEnumerable<Product> GetProducts(int pageStart, int pageSize, string currency)
    {
        var products = _productAccess.List(pageStart, pageSize);

        if (currency.ToUpper() == "EUR")
        {
            return products.Select(p => new Product
            {
                Name = p.Name,
                Price = decimal.Round(p.Price * GbpToEurRate, 2)
            }).ToList();
        }

        return products;
    }
}
