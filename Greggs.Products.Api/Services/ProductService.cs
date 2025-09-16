using System;
using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;

namespace Greggs.Products.Api.Services;

public class ProductService : IProductService
{
    private readonly IDataAccess<Product> _productAccess;
    private readonly ICurrencyConverter _currencyConverter;

    public ProductService(IDataAccess<Product> productAccess, ICurrencyConverter currencyConverter)
    {
        _productAccess = productAccess;
        _currencyConverter = currencyConverter;
    }

    public IEnumerable<Product> GetProducts(int pageStart, int pageSize, string currency)
    {
        var products = _productAccess.List(pageStart, pageSize);
        var targetCurrency = string.IsNullOrWhiteSpace(currency) ? "GBP" : currency;

        if (string.Equals(targetCurrency, "GBP", StringComparison.OrdinalIgnoreCase))
        {
            return products;
        }

        return products.Select(p => new Product
        {
            Name = p.Name,
            Price = _currencyConverter.Convert(p.Price, "GBP", targetCurrency)
        }).ToList();
    }
}
