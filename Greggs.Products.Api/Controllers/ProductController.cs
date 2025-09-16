using System;
using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IDataAccess<Product> _productAccess;
    private const decimal GbpToEurRate = 1.11m;

    public ProductController(ILogger<ProductController> logger, IDataAccess<Product> productAccess)
    {
        _logger = logger;
        _productAccess = productAccess;
    }

    [HttpGet]
    public IEnumerable<Product> Get(int pageStart = 0, int pageSize = 5, string currency = "GBP")
    {
        var products = _productAccess.List(pageStart, pageSize);

        if (currency.ToUpper() == "EUR")
        {
            // Return products with price converted to EUR
            return products.Select(p => new Product
            {
                Name = p.Name,
                Price = decimal.Round(p.Price * GbpToEurRate, 2)
            }).ToList();
        }

        // Default: return products in GBP
        return products;
    }


}