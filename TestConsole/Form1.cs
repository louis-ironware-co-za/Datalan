using Coinigy.API.V2.Models;
using Crypto.Coinigy;
using Crypto.Common;
using DevExpress.XtraEditors;
using LunoIntegration;
using LunoIntegration.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestConsole
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        string apiKey = "0448b73a465d3946b00b9e6b7b86bf01";

        string apiSecret = "f2358363acecf50085ade2e13692d0dd";

        string coingApiKey = "8ac9c67734284d4abd23d79a37201a1a";
        string coingApiSecret = "23f02be858e84d47ad0088564cb900d2";



        Luno Luno;
        Crypto.Coinigy.Coinigy Coinigy;

        public Form1()
        {
            InitializeComponent();
            
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            RunCoinigyTest();
        }

        private async void RunCoinigyTest()
        {
            Coinigy = new Crypto.Coinigy.Coinigy(new ApiCredentials() { ApiKey = coingApiKey, ApiSecret = coingApiSecret, SocketKey = apiKey, SocketSecret = apiSecret});
            Coinigy.Initialize();
            //Coinigy.Client.TestSignature();
            //Coinigy.Subscribe("ORDER", "BMEX", "XBT", "USD");
            var res = await Coinigy.Client.ExchangesMarkets.GetMarketsAsync();
            grdMarkets.DataSource = res?.Result;
        }

        private async void RunLunoTest()
        {
            Luno = new Luno(new Crypto.Common.Objects.ApiCredentials(apiKey, apiSecret));
            var res = await Luno.GetMarketsAsync();
            grdMarkets.DataSource = res?.Markets;
        }

        LunoLiveOrderBook liveBook;

        private async void btnSubscribeToOrderBook_Click(object sender, EventArgs e)
        {
            if (gvMarkets.GetFocusedRow() is MarketInfo info)
            {
                var liveBook = await Luno.SubscribeToOrderBook(info);
                liveBook.OrderBookInitialized += LiveBook_OrderBookInitialized;
                liveBook.OrderBookUpdated += LiveBook_OrderBookUpdated;
            }
            else if(gvMarkets.GetFocusedRow() is MarketListEntry entry)
            {
                Exchange ex = new Exchange()
                {
                    ExchName = entry.ExchName,
                    ExchCode = entry.ExchCode,

                };

                try
                {
                    var liveBook = Coinigy.Subscribe(ex, entry);
                    liveBook.TradeUpdated += LiveBook_TradeUpdated;
                    liveBook.OrderBookUpdated += LiveBook_OrderBookUpdated1;
                }
                catch (Exception ex2)
                {
                    XtraMessageBox.Show(ex2.Message);
                }
                
            }
        }

        private void LiveBook_OrderBookUpdated1(CoinigyLiveOrderBook book)
        {
            this.Invoke(new Action(delegate
            {
                grdSubAsks.DataSource = book.OrderBook.Sells;
                grdSubAsks.RefreshDataSource();
                grdSubBuys.DataSource = book.OrderBook.Buys;
                grdSubBuys.RefreshDataSource();
            }));
        }

        private void LiveBook_TradeUpdated(CoinigyLiveOrderBook book)
        {
            this.Invoke(new Action(delegate
            {
                grdTrades.DataSource = book.Trades;
                grdTrades.RefreshDataSource();
            }));
        }

        private void LiveBook_OrderBookUpdated(LunoLiveOrderBook book)
        {
            this.Invoke(new Action(delegate
            {
                grdSubAsks.DataSource = book.OrderBook.Asks;
                grdSubAsks.RefreshDataSource();
                grdSubBuys.DataSource = book.OrderBook.Bids;
                grdSubBuys.RefreshDataSource();
                grpBidsAsks.Text = "Bids (" + book.OrderBook.Bids.Count + ") | Asks (" + book.OrderBook.Asks.Count + ")";
                grdTrades.DataSource = book.Trades;
                grdTrades.RefreshDataSource();
                grpTrades.Text = "Trades (" + book.Trades.Count + ")";

                xtabpageSubscriptionOrders.Text = "Subscription - " + book.Market.MarketId + " - " + book.OrderBook.Sequence;
            }));
        }

        private void LiveBook_OrderBookInitialized(LunoLiveOrderBook book)
        {
            this.Invoke(new Action(delegate
            {
                grdSubAsks.DataSource = book.OrderBook.Asks;
                grdSubBuys.DataSource = book.OrderBook.Bids;
                grdTrades.DataSource = book.Trades;
            }));
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Luno != null)
            {
                await Luno.DisconnectAll();
            }

            if (Coinigy != null)
            {
                Coinigy.DisconnectAll();
            }

        }
    }
}
