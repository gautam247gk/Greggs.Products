using System;
using System.Collections.Generic;

namespace Greggs.Products.Api.Services;

public class StaticCurrencyConverter : ICurrencyConverter
{
    // Hardcoded values for for current user stories, extendable for furutre currencies from actual providers.
    private static readonly Dictionary<string, decimal> ToGbp = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
    {
        ["GBP"] = 1m,
        ["EUR"] = 1m / 1.11m
    };

    public decimal Convert(decimal amount, string fromCurrency, string toCurrency)
    {
        if (string.Equals(fromCurrency, toCurrency, StringComparison.OrdinalIgnoreCase))
            return Math.Round(amount,2);

        // Check for supported currencies
        var amountInGbp = amount * (ToGbp.TryGetValue(fromCurrency, out var toGbp) ? toGbp : throw new ArgumentException($"Unsupported currency: {fromCurrency}"));

        if (string.Equals(toCurrency, "GBP", StringComparison.OrdinalIgnoreCase))
            return Math.Round(amountInGbp, 2);

        if (string.Equals(toCurrency, "EUR", StringComparison.OrdinalIgnoreCase))
        {
            var eur = amountInGbp * 1.11m;
            return Math.Round(eur, 2);
        }

        throw new ArgumentException($"Unsupported currency: {toCurrency}");
    }
}
