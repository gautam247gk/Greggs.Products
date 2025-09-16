using System;
using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.Controllers;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Greggs.Products.UnitTests;

public class ProductControllerTests
{


    private static List<Product> GetSampleProducts() =>
        new()
        {
            new Product { Name = "Sausage Roll", Price = 1m },
            new Product { Name = "Vegan Sausage Roll", Price = 1.1m },
            new Product { Name = "Steak Bake", Price = 1.2m }
        };

    private ProductController CreateController(List<Product> products)
    {
        var loggerMock = new Mock<ILogger<ProductController>>();
        var serviceMock = new Mock<IProductService>();
        serviceMock
            .Setup(s => s.GetProducts(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
            .Returns((int start, int size, string currency) =>
            {
                var page = products.Skip(start).Take(size).ToList();
                if (currency.ToUpper() == "EUR")
                {
                    return page.Select(p => new Product
                    {
                        Name = p.Name,
                        Price = decimal.Round(p.Price * 1.11m, 2)
                    }).ToList();
                }

                return page;
            });

        return new ProductController(loggerMock.Object, serviceMock.Object);
    }

    [Fact]
    public void Get_ReturnsProductsInGBP_ByDefault()
    {
        var products = GetSampleProducts();
        var controller = CreateController(products);

        var result = controller.Get(0, 3, "GBP").ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(products[0].Price, result[0].Price);
    }

    [Fact]
    public void Get_ReturnsProductsInEUR_WhenCurrencyIsEUR()
    {
        var products = GetSampleProducts();
        var controller = CreateController(products);

        var result = controller.Get(0, 3, "EUR").ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(decimal.Round(products[0].Price * 1.11m, 2), result[0].Price);
    }

    [Fact]
    public void Get_RespectsPagination()
    {
        var products = GetSampleProducts();
        var controller = CreateController(products);

        var result = controller.Get(1, 1, "GBP").ToList();

        Assert.Single(result);
        Assert.Equal(products[1].Name, result[0].Name);
    }

    [Fact]
    public void Get_OmitsCurrency_UsesDefaultGBP()
    {
        var products = GetSampleProducts();
        var controller = CreateController(products);

        // Omit the currency arg to hit the default value defined in the controller
        var result = controller.Get(0, 3).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(products[0].Price, result[0].Price);
    }
}