namespace Greggs.Products.Api.Services;

public interface ICurrencyConverter
{
    decimal Convert(decimal amount, string fromCurrency, string toCurrency);
}
