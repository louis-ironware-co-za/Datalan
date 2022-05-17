namespace TestConsole
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions buttonImageOptions1 = new DevExpress.XtraEditors.ButtonsPanelControl.ButtonImageOptions();
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtabpageMarkets = new DevExpress.XtraTab.XtraTabPage();
            this.grdMarkets = new DevExpress.XtraGrid.GridControl();
            this.gvMarkets = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.btnSubscribeToOrderBook = new DevExpress.XtraEditors.SimpleButton();
            this.xtabpageSubscriptionOrders = new DevExpress.XtraTab.XtraTabPage();
            this.grpBidsAsks = new DevExpress.XtraEditors.GroupControl();
            this.grdSubBuys = new DevExpress.XtraGrid.GridControl();
            this.gvSubBuys = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.splitterControl2 = new DevExpress.XtraEditors.SplitterControl();
            this.grdSubAsks = new DevExpress.XtraGrid.GridControl();
            this.gvSubAsks = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.splitterControl1 = new DevExpress.XtraEditors.SplitterControl();
            this.grpTrades = new DevExpress.XtraEditors.GroupControl();
            this.grdTrades = new DevExpress.XtraGrid.GridControl();
            this.gvTrades = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.xtabpageExchanges = new DevExpress.XtraTab.XtraTabPage();
            this.grpExchanges = new DevExpress.XtraEditors.GroupControl();
            this.grdExchanges = new DevExpress.XtraGrid.GridControl();
            this.gvExchanges = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtabpageMarkets.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdMarkets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMarkets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            this.xtabpageSubscriptionOrders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpBidsAsks)).BeginInit();
            this.grpBidsAsks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdSubBuys)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSubBuys)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdSubAsks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSubAsks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpTrades)).BeginInit();
            this.grpTrades.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdTrades)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTrades)).BeginInit();
            this.xtabpageExchanges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grpExchanges)).BeginInit();
            this.grpExchanges.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdExchanges)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvExchanges)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtabpageMarkets;
            this.xtraTabControl1.Size = new System.Drawing.Size(800, 579);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtabpageExchanges,
            this.xtabpageMarkets,
            this.xtabpageSubscriptionOrders});
            // 
            // xtabpageMarkets
            // 
            this.xtabpageMarkets.Controls.Add(this.grdMarkets);
            this.xtabpageMarkets.Controls.Add(this.groupControl1);
            this.xtabpageMarkets.Name = "xtabpageMarkets";
            this.xtabpageMarkets.Size = new System.Drawing.Size(798, 554);
            this.xtabpageMarkets.Text = "Markets";
            // 
            // grdMarkets
            // 
            this.grdMarkets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdMarkets.Location = new System.Drawing.Point(0, 48);
            this.grdMarkets.MainView = this.gvMarkets;
            this.grdMarkets.Name = "grdMarkets";
            this.grdMarkets.Size = new System.Drawing.Size(798, 506);
            this.grdMarkets.TabIndex = 0;
            this.grdMarkets.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvMarkets});
            // 
            // gvMarkets
            // 
            this.gvMarkets.GridControl = this.grdMarkets;
            this.gvMarkets.Name = "gvMarkets";
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.btnSubscribeToOrderBook);
            this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupControl1.Location = new System.Drawing.Point(0, 0);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(798, 48);
            this.groupControl1.TabIndex = 1;
            this.groupControl1.Text = "groupControl1";
            // 
            // btnSubscribeToOrderBook
            // 
            this.btnSubscribeToOrderBook.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnSubscribeToOrderBook.Location = new System.Drawing.Point(2, 23);
            this.btnSubscribeToOrderBook.Name = "btnSubscribeToOrderBook";
            this.btnSubscribeToOrderBook.Size = new System.Drawing.Size(61, 23);
            this.btnSubscribeToOrderBook.TabIndex = 0;
            this.btnSubscribeToOrderBook.Text = "Subscribe";
            this.btnSubscribeToOrderBook.Click += new System.EventHandler(this.btnSubscribeToOrderBook_Click);
            // 
            // xtabpageSubscriptionOrders
            // 
            this.xtabpageSubscriptionOrders.Controls.Add(this.grpBidsAsks);
            this.xtabpageSubscriptionOrders.Controls.Add(this.splitterControl1);
            this.xtabpageSubscriptionOrders.Controls.Add(this.grpTrades);
            this.xtabpageSubscriptionOrders.Name = "xtabpageSubscriptionOrders";
            this.xtabpageSubscriptionOrders.Size = new System.Drawing.Size(798, 554);
            this.xtabpageSubscriptionOrders.Text = "Subscription";
            // 
            // grpBidsAsks
            // 
            this.grpBidsAsks.Controls.Add(this.grdSubBuys);
            this.grpBidsAsks.Controls.Add(this.splitterControl2);
            this.grpBidsAsks.Controls.Add(this.grdSubAsks);
            this.grpBidsAsks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBidsAsks.Location = new System.Drawing.Point(0, 0);
            this.grpBidsAsks.Name = "grpBidsAsks";
            this.grpBidsAsks.Size = new System.Drawing.Size(798, 331);
            this.grpBidsAsks.TabIndex = 2;
            this.grpBidsAsks.Text = "Bids | Asks";
            // 
            // grdSubBuys
            // 
            this.grdSubBuys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdSubBuys.Location = new System.Drawing.Point(2, 23);
            this.grdSubBuys.MainView = this.gvSubBuys;
            this.grdSubBuys.Name = "grdSubBuys";
            this.grdSubBuys.Size = new System.Drawing.Size(384, 306);
            this.grdSubBuys.TabIndex = 5;
            this.grdSubBuys.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvSubBuys});
            // 
            // gvSubBuys
            // 
            this.gvSubBuys.GridControl = this.grdSubBuys;
            this.gvSubBuys.Name = "gvSubBuys";
            // 
            // splitterControl2
            // 
            this.splitterControl2.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterControl2.Location = new System.Drawing.Point(386, 23);
            this.splitterControl2.Name = "splitterControl2";
            this.splitterControl2.Size = new System.Drawing.Size(10, 306);
            this.splitterControl2.TabIndex = 4;
            this.splitterControl2.TabStop = false;
            // 
            // grdSubAsks
            // 
            this.grdSubAsks.Dock = System.Windows.Forms.DockStyle.Right;
            this.grdSubAsks.Location = new System.Drawing.Point(396, 23);
            this.grdSubAsks.MainView = this.gvSubAsks;
            this.grdSubAsks.Name = "grdSubAsks";
            this.grdSubAsks.Size = new System.Drawing.Size(400, 306);
            this.grdSubAsks.TabIndex = 3;
            this.grdSubAsks.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvSubAsks});
            // 
            // gvSubAsks
            // 
            this.gvSubAsks.GridControl = this.grdSubAsks;
            this.gvSubAsks.Name = "gvSubAsks";
            // 
            // splitterControl1
            // 
            this.splitterControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitterControl1.Location = new System.Drawing.Point(0, 331);
            this.splitterControl1.Name = "splitterControl1";
            this.splitterControl1.Size = new System.Drawing.Size(798, 10);
            this.splitterControl1.TabIndex = 1;
            this.splitterControl1.TabStop = false;
            // 
            // grpTrades
            // 
            this.grpTrades.Controls.Add(this.grdTrades);
            this.grpTrades.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.grpTrades.Location = new System.Drawing.Point(0, 341);
            this.grpTrades.Name = "grpTrades";
            this.grpTrades.Size = new System.Drawing.Size(798, 213);
            this.grpTrades.TabIndex = 0;
            this.grpTrades.Text = "Trades";
            // 
            // grdTrades
            // 
            this.grdTrades.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdTrades.Location = new System.Drawing.Point(2, 23);
            this.grdTrades.MainView = this.gvTrades;
            this.grdTrades.Name = "grdTrades";
            this.grdTrades.Size = new System.Drawing.Size(794, 188);
            this.grdTrades.TabIndex = 0;
            this.grdTrades.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvTrades});
            // 
            // gvTrades
            // 
            this.gvTrades.GridControl = this.grdTrades;
            this.gvTrades.Name = "gvTrades";
            // 
            // xtabpageExchanges
            // 
            this.xtabpageExchanges.Controls.Add(this.grpExchanges);
            this.xtabpageExchanges.Name = "xtabpageExchanges";
            this.xtabpageExchanges.Size = new System.Drawing.Size(798, 554);
            this.xtabpageExchanges.Text = "Exchanges";
            // 
            // grpExchanges
            // 
            this.grpExchanges.Controls.Add(this.grdExchanges);
            this.grpExchanges.CustomHeaderButtons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
            new DevExpress.XtraEditors.ButtonsPanelControl.GroupBoxButton("Markets", true, buttonImageOptions1, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, "", -1, true, null, true, false, true, "Markets", -1)});
            this.grpExchanges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpExchanges.Location = new System.Drawing.Point(0, 0);
            this.grpExchanges.Name = "grpExchanges";
            this.grpExchanges.Size = new System.Drawing.Size(798, 554);
            this.grpExchanges.TabIndex = 0;
            this.grpExchanges.Text = "Exchanges";
            this.grpExchanges.CustomButtonClick += new DevExpress.XtraBars.Docking2010.BaseButtonEventHandler(this.grpExchanges_CustomButtonClick);
            // 
            // grdExchanges
            // 
            this.grdExchanges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdExchanges.Location = new System.Drawing.Point(2, 23);
            this.grdExchanges.MainView = this.gvExchanges;
            this.grdExchanges.Name = "grdExchanges";
            this.grdExchanges.Size = new System.Drawing.Size(794, 529);
            this.grdExchanges.TabIndex = 1;
            this.grdExchanges.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvExchanges});
            // 
            // gvExchanges
            // 
            this.gvExchanges.GridControl = this.grdExchanges;
            this.gvExchanges.Name = "gvExchanges";
            this.gvExchanges.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.gvExchanges.OptionsSelection.MultiSelect = true;
            this.gvExchanges.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 579);
            this.Controls.Add(this.xtraTabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtabpageMarkets.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdMarkets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMarkets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            this.xtabpageSubscriptionOrders.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpBidsAsks)).EndInit();
            this.grpBidsAsks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdSubBuys)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSubBuys)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdSubAsks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvSubAsks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grpTrades)).EndInit();
            this.grpTrades.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdTrades)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvTrades)).EndInit();
            this.xtabpageExchanges.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grpExchanges)).EndInit();
            this.grpExchanges.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdExchanges)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvExchanges)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtabpageMarkets;
        private DevExpress.XtraGrid.GridControl grdMarkets;
        private DevExpress.XtraGrid.Views.Grid.GridView gvMarkets;
        private DevExpress.XtraTab.XtraTabPage xtabpageSubscriptionOrders;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.SimpleButton btnSubscribeToOrderBook;
        private DevExpress.XtraEditors.GroupControl grpBidsAsks;
        private DevExpress.XtraGrid.GridControl grdSubBuys;
        private DevExpress.XtraGrid.Views.Grid.GridView gvSubBuys;
        private DevExpress.XtraEditors.SplitterControl splitterControl2;
        private DevExpress.XtraGrid.GridControl grdSubAsks;
        private DevExpress.XtraGrid.Views.Grid.GridView gvSubAsks;
        private DevExpress.XtraEditors.SplitterControl splitterControl1;
        private DevExpress.XtraEditors.GroupControl grpTrades;
        private DevExpress.XtraGrid.GridControl grdTrades;
        private DevExpress.XtraGrid.Views.Grid.GridView gvTrades;
        private DevExpress.XtraTab.XtraTabPage xtabpageExchanges;
        private DevExpress.XtraEditors.GroupControl grpExchanges;
        private DevExpress.XtraGrid.GridControl grdExchanges;
        private DevExpress.XtraGrid.Views.Grid.GridView gvExchanges;
    }
}

