using Crypto.Common;
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
        Luno Luno;

        public Form1()
        {
            InitializeComponent();
            
        }

        private async void Form1_Shown(object sender, EventArgs e)
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
            await Luno.DisconnectAll();
        }
    }
}
