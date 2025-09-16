using System;
using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Greggs.Products.Api.Services;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductService _productService;

    public ProductController(ILogger<ProductController> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [HttpGet]
    public IEnumerable<Product> Get(int pageStart = 0, int pageSize = 5, string currency = "GBP")
    {
        return _productService.GetProducts(pageStart, pageSize, currency);
    }


}