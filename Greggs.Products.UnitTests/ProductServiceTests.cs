using System;
using System.Collections.Generic;
using System.Linq;
using Greggs.Products.Api.DataAccess;
using Greggs.Products.Api.Models;
using Greggs.Products.Api.Services;
using Moq;
using Xunit;

namespace Greggs.Products.UnitTests;

public class ProductServiceTests
{
    private static List<Product> GetSampleProducts() =>
        new()
        {
            new Product { Name = "Sausage Roll", Price = 1m },
            new Product { Name = "Vegan Sausage Roll", Price = 1.1m },
            new Product { Name = "Steak Bake", Price = 1.2m }
        };

    private static (ProductService service, Mock<IDataAccess<Product>> dataMock, Mock<ICurrencyConverter> currencyMock) CreateService(List<Product> products)
    {
        var dataMock = new Mock<IDataAccess<Product>>();
        dataMock.Setup(d => d.List(It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns((int? start, int? size) => products.Skip(start ?? 0).Take(size ?? products.Count).ToList());

        var currencyMock = new Mock<ICurrencyConverter>();

        var service = new ProductService(dataMock.Object, currencyMock.Object);
        return (service, dataMock, currencyMock);
    }

    [Fact]
    public void GetProducts_GbpPassthrough_WhenCurrencyIsGbp()
    {
        var products = GetSampleProducts();
        var (service, dataMock, currencyMock) = CreateService(products);

        var result = service.GetProducts(0, 3, "GBP").ToList();

        Assert.Equal(products.Count, result.Count);
        Assert.Equal(products[0].Price, result[0].Price);
        currencyMock.Verify(c => c.Convert(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void GetProducts_Convert_WhenCurrencyIsEur()
    {
        var products = GetSampleProducts();
        var (service, dataMock, currencyMock) = CreateService(products);
        currencyMock.Setup(c => c.Convert(It.IsAny<decimal>(), "GBP", "EUR"))
                    .Returns<decimal, string, string>((amount, _, _) => Math.Round(amount * 1.11m, 2));

        var result = service.GetProducts(0, 3, "EUR").ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(Math.Round(products[0].Price * 1.11m, 2), result[0].Price);
        currencyMock.Verify(c => c.Convert(products[0].Price, "GBP", "EUR"), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetProducts_NullOrWhitespaceCurrency_DefaultsToGbp(string currency)
    {
        var products = GetSampleProducts();
        var (service, dataMock, currencyMock) = CreateService(products);

        var result = service.GetProducts(0, 3, currency).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(products[1].Price, result[1].Price);
        currencyMock.Verify(c => c.Convert(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
