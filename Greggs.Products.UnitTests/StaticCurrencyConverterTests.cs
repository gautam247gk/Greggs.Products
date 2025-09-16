using System;
using Greggs.Products.Api.Services;
using Xunit;

namespace Greggs.Products.UnitTests;

public class StaticCurrencyConverterTests
{
    private readonly StaticCurrencyConverter _converter = new();

    [Theory]
    [InlineData(0, "GBP", "GBP", 0)]
    [InlineData(1, "GBP", "GBP", 1)]
    [InlineData(1.234, "GBP", "GBP", 1.23)]
    public void Convert_GbpToGbp_RoundsToTwoDecimals(decimal amount, string from, string to, decimal expected)
    {
        var result = _converter.Convert(amount, from, to);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, "GBP", "EUR", 1.11)]
    [InlineData(1.2, "GBP", "EUR", 1.33)]
    [InlineData(2.34, "GBP", "EUR", 2.60)]
    public void Convert_GbpToEur_UsesRateAndRounds(decimal amount, string from, string to, decimal expected)
    {
        var result = _converter.Convert(amount, from, to);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1.11, "EUR", "GBP", 1.00)]
    [InlineData(2.22, "EUR", "GBP", 2.00)]
    [InlineData(10, "EUR", "GBP", 9.01)]
    public void Convert_EurToGbp_UsesInverseRateAndRounds(decimal amount, string from, string to, decimal expected)
    {
        var result = _converter.Convert(amount, from, to);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Convert_UnsupportedFromCurrency_Throws()
    {
        Assert.Throws<ArgumentException>(() => _converter.Convert(1, "USD", "GBP"));
    }

    [Fact]
    public void Convert_UnsupportedToCurrency_Throws()
    {
        Assert.Throws<ArgumentException>(() => _converter.Convert(1, "GBP", "USD"));
    }

    [Theory]
    [InlineData("gbp", "eur")]
    [InlineData("GBP", "EUR")]
    [InlineData("Gbp", "eUr")]
    public void Convert_IsCaseInsensitive(string from, string to)
    {
        var result = _converter.Convert(1, from, to);
        Assert.True(result > 0);
    }
}
