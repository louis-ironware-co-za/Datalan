using Coinigy.API.V2.Models;
using Coinigy.API.V2.Requests;
using Crypto.Coinigy;
using Crypto.Common;
using Crypto.Common.Master;
using Crypto.Common.Models;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
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

        string apiLunoKey = "mfq7b4v974nqf";

        string apiLunoSecret = "rRooohK_008lA8yDobh77OZo8ufizCMV2gyfntu7ShQ";

        string coingApiKey = "8ac9c67734284d4abd23d79a37201a1a";
        string coingApiSecret = "23f02be858e84d47ad0088564cb900d2";



        Luno Luno;
        Crypto.Coinigy.Coinigy Coinigy;
        MasterExchange masterExchange;

        public Form1()
        {
            InitializeComponent();
            
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            RunCommonMasterTest();
            //RunCoinigyTest();
        }

        private async void RunCommonMasterTest()
        {
            masterExchange = new MasterExchange();
            var coinCred = new ApiCredentials() { ApiKey = coingApiKey, ApiSecret = coingApiSecret, SocketKey = apiKey, SocketSecret = apiSecret };

            masterExchange.AddCryptoExchange(new MasterApiCredentials() { Exchange = Exchanges.Coinigy, ApiKey = coinCred.ApiKey, ApiSecret = coinCred.ApiSecret, SocketKey = coinCred.SocketKey, SocketSecret = coinCred.SocketSecret });
            masterExchange.AddCryptoExchange(new MasterApiCredentials() { Exchange = Exchanges.Luno, ApiKey = apiLunoKey, ApiSecret = apiLunoSecret });

            masterExchange.OrderBookUpdated += MasterExchange_OrderBookUpdated;

            await masterExchange.InitializeAll();
            //grdMarkets.DataSource = await masterExchange.GetAllMarkets();
            grdExchanges.DataSource = masterExchange.Exchanges;
        }

        private void MasterExchange_OrderBookUpdated(MasterLiveOrderBook book)
        {
            this.Invoke(new Action(delegate
            {
                grdSubAsks.DataSource = book.Sells;
                grdSubAsks.RefreshDataSource();
                grdSubBuys.DataSource = book.Buys;
                grdSubBuys.RefreshDataSource();
                grdTrades.DataSource = book.Trades;
                grdTrades.RefreshDataSource();
            }));
        }

        private async void RunNewOrderTest()
        {
            Coinigy = new Crypto.Coinigy.Coinigy(new ApiCredentials() { ApiKey = coingApiKey, ApiSecret = coingApiSecret, SocketKey = apiKey, SocketSecret = apiSecret });
            Coinigy.Initialize();

            NewOrderRequest req = new NewOrderRequest()
            {
                ExchMktId = 13511,
                Side = 2,
                Price = 0.99M,
                Quantity = 20,
                PriceType = 3,
                ExpiresAt = new DateTime(2022, 5, 28),
            };

            int authId = 456582;

            //var result = await Coinigy.Client.Orders.PlaceNewOrder(req, authId.ToString());
            var orders = await Coinigy.Client.Orders.GetOrders();
            var result = await Coinigy.Client.Orders.CancelOrder(authId.ToString(), orders.Result[0].CoinigyOrderId.ToString());
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
            Luno = new Luno(apiKey, apiSecret);
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
            else if(gvMarkets.GetFocusedRow() is CommonMarket commonMarket)
            {
                masterExchange.SubscribeToMarketOrderBook(commonMarket.ExchangeRef, commonMarket);
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

        private async void grpExchanges_CustomButtonClick(object sender, DevExpress.XtraBars.Docking2010.BaseButtonEventArgs e)
        {
            if (e.Button.Properties.Tag is string tag)
            {
                switch (tag)
                {
                    case "Markets":
                        await masterExchange.PopulateMarketsForExchanges(gvExchanges.GetSelectedItems<CommonExchange>());
                        grdMarkets.DataSource = await masterExchange.GetAllMarkets();
                        break;
                    case "Orders":
                        if (gvExchanges.GetFocusedRow() is CommonExchange orderExchange)
                        {
                            var crypto = masterExchange.GetCryptoExchange(orderExchange.ParentExchange);
                            var res = await crypto.ApiClient.GetOrdersAsync(orderExchange);
                            grdOrders.DataSource = res;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        
    }
}
