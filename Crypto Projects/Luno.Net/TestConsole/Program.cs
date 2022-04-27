// See https://aka.ms/new-console-template for more information
using Luno.Net.Clients;
using Luno.Net.Objects;
using Luno.Net.SymbolOrderBooks;
ManualResetEvent manualResetEvent = new ManualResetEvent(false);

Console.WriteLine("Hello, World!");
LunoClient.SetDefaultOptions(new LunoClientOptions()
{
    LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
    ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("sqavxb2baxyu", "hBZznvdRNNqBwZdbtC6EYpS2GmnVgxBDgGBdBH9wOGM"),
    OutputOriginalData = true
});
LunoSocketClient.SetDefaultOptions(new LunoSocketClientOptions()
{
    LogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
    ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("sqavxb2baxyu", "hBZznvdRNNqBwZdbtC6EYpS2GmnVgxBDgGBdBH9wOGM"),
    OutputOriginalData = true
});
LunoClient client = new LunoClient();
var res = client.GeneralApi.General.GetExchangeSymbolsAsync(5).Result;

if (!res.Success)
{
    var error = res.Error.ToString();
    Console.Write(error);
    Console.WriteLine("\n\n\n" + res.OriginalData);
}
else
{
    foreach (var item in res.Data.Markets)
    {
        Console.WriteLine(string.Format("Symbol: {0} | Status: {1}", item.Name, item.TradingStatus));
    }
    Console.WriteLine("\n\n\n" + res.OriginalData);
}

LunoSocketClient socketClient = new LunoSocketClient();

LunoSymbolOrderBook book = new LunoSymbolOrderBook(res.Data.Markets.First().Name);
var startResult = book.StartAsync().Result;

manualResetEvent.WaitOne();

if (true)
{

}
