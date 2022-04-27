using Luno.Net.Clients;
using Luno.Net.Objects;
using Luno.Net.SymbolOrderBooks;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

           
        }

        private void Book_OnOrderBookUpdate((IEnumerable<CryptoExchange.Net.Interfaces.ISymbolOrderBookEntry> Bids, IEnumerable<CryptoExchange.Net.Interfaces.ISymbolOrderBookEntry> Asks) obj)
        {

        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            LunoClient.SetDefaultOptions(new LunoClientOptions()
            {
                LogLevel = Microsoft.Extensions.Logging.LogLevel.Trace,
                ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("sqavxb2baxyu", "hBZznvdRNNqBwZdbtC6EYpS2GmnVgxBDgGBdBH9wOGM"),
                OutputOriginalData = true
            });
            LunoSocketClient.SetDefaultOptions(new LunoSocketClientOptions()
            {
                LogLevel = Microsoft.Extensions.Logging.LogLevel.Trace,
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
            book.OnOrderBookUpdate += Book_OnOrderBookUpdate;
            var startResult = await book.StartAsync().ConfigureAwait(false);
        }
    }
}