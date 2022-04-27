Option Explicit On
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Threading
Imports DevExpress.Data.Filtering
Imports DevExpress.LookAndFeel
Imports DevExpress.XtraBars
Imports DevExpress.XtraBars.Docking2010
Imports DevExpress.XtraBars.Helpers
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraNavBar
Imports Newtonsoft.Json

Imports CoinigyWebsocketClient
Imports CoinigyWebsocketClient.Models
Imports PureSocketCluster
Imports PureWebSockets
Imports CoinigyWebsocketClient.Types
'Imports CoinigyWebsocketClient.Models

Imports Coinigy.API.CoinigyApi
Imports Coinigy.API.Responses
'Imports System.Reflection

Imports Arbitrage_AIM.Enums
Imports System.Threading.Tasks
Imports System.Net.WebSockets

Imports RestSharp

Imports Coinigy.API
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Net.Http
Imports System.Reflection
Imports DevExpress.XtraCharts

Public Class xfuiUser

#Region "#VAR"

    Private Shared ReadOnly api_key As String = "0448b73a465d3946b00b9e6b7b86bf01"
    Private Shared ReadOnly api_secret As String = "f2358363acecf50085ade2e13692d0dd"

    '0448b73a465d3946b00b9e6b7b86bf01
    'f2358363acecf50085ade2e13692d0dd

    Private Shared ReadOnly ManualResetEvent As ManualResetEvent = New ManualResetEvent(False)

    Private Shared _coinigyclient As CoinigyWsClient
    Private Shared _coinigyAPI As Coinigy.API.CoinigyApi
    Private _scClient As PureSocketClusterSocket
    Public ReadOnly Options As CoinigyWebSocketClientOptions

    'DATASET - mem
    'Private OrderBookTable As New DataTable
    Private OrderBookDataSet As New DataSet
    Private WalletDataSet As New DataSet
    Private ExchangeDataSet As New DataSet
    Public Shared ChannelDataSet As New DataSet

    'TRADEITEMS
    'Public tradeItems() As List(Of TradeItem)

    'CHART
    Private series1 As New Series("Series 1", ViewType.RangeArea)
    Private ariaSeries1 As New Series("Series 1", ViewType.Area)

    'SAVELABELSCLICK
    Private Delegate Sub LabelDelegate(<[ParamArray]()> sender As Object, e As EventArgs)


#End Region '#VAR

#Region "PROPERTIES"

    Private Property TradeItemsBuy() As List(Of TradeItem)
        Get
            Return m_TradeItemsBuy
        End Get
        Set
            m_TradeItemsBuy = Value
        End Set
    End Property
    Private m_TradeItemsBuy As List(Of TradeItem)

    Private Property TradeItemsSell() As List(Of TradeItem)
        Get
            Return m_TradeItemsSell
        End Get
        Set
            m_TradeItemsSell = Value
        End Set
    End Property
    Private m_TradeItemsSell As List(Of TradeItem)

    Private Property OrderItemsBuy() As List(Of OrderItem)
        Get
            Return m_OrderItemsBuy
        End Get
        Set
            m_OrderItemsBuy = Value
        End Set
    End Property
    Private m_OrderItemsBuy As List(Of OrderItem)
    Private Property OrderItemsSell() As List(Of OrderItem)
        Get
            Return m_OrderItemsSell
        End Get
        Set
            m_OrderItemsSell = Value
        End Set
    End Property
    Private m_OrderItemsSell As List(Of OrderItem)
    'Private Property lstBuyC2() As List(Of OrderItem)
    '    Get
    '        Return m_lstBuyC2
    '    End Get
    '    Set
    '        m_lstBuyC2 = Value
    '    End Set
    'End Property
    'Private m_lstBuyC2 As List(Of OrderItem)
    'Private Property lstSellC2() As List(Of OrderItem)
    '    Get
    '        Return m_lstSellC2
    '    End Get
    '    Set
    '        m_lstSellC2 = Value
    '    End Set
    'End Property
    'Private m_lstSellC2 As List(Of OrderItem)

    Public Property ExchangeItemList() As List(Of ExchangeItem)
        Get
            Return m_ExchangeItemList
        End Get
        Set
            m_ExchangeItemList = Value
        End Set
    End Property
    Private m_ExchangeItemList As List(Of ExchangeItem)

    Public Property ChannelItemList() As IEnumerable(Of String)
        Get
            Return m_ChannelItemList
        End Get
        Set
            m_ChannelItemList = Value
        End Set
    End Property
    Private m_ChannelItemList As IEnumerable(Of String)



#End Region 'PROPERTIES

#Region "#INIT"
    Sub New()
        InitSkins()
        InitializeComponent()
        Me.InitSkinGallery()

        Me.Opacity = 0
        Dim ribbontabpage As DevExpress.XtraTab.XtraTabPage = HomePageXtraTab 'LoginPageXtraTab

        PageHeadersPanelXtraTabControl.SelectedTabPage = ribbontabpage
        PageHeadersPanelXtraTabControl.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False

        'PageRibbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide
        PageRibbonControl.Minimized = True

        'InformationGroupControl.Visible = False

    End Sub

    Sub InitSkins()
        DevExpress.Skins.SkinManager.EnableFormSkins()
        DevExpress.UserSkins.BonusSkins.Register()
        Dim DefaultLookAndFeel As String = My.Settings.propLookAndFeelStyle
        UserLookAndFeel.Default.SetSkinStyle(DefaultLookAndFeel)
    End Sub

    Private Sub InitSkinGallery()
        SkinHelper.InitSkinGallery(rgbiSkins, True)
    End Sub

    '#DEFAULT STYLE CHANGED EVENT
    Private Sub Default_StyleChanged() ' ByVal sender As Object, ByVal e As EventArgs
        Dim DefaultLookAndFeel = UserLookAndFeel.Default.SkinName
        My.Settings.propLookAndFeelStyle = DefaultLookAndFeel
        My.Settings.Save()
    End Sub

#End Region '#INIT

#Region "#FORM"

    '#FORM LOAD
    Private Sub xfuiUser_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'ArbitrageTradingDataSet.ExchangeTable' table. You can move, or remove it, as needed.
        'Me.ExchangeTableTableAdapter.Fill(Me.ArbitrageTradingDataSet.ExchangeTable)
        '#DEFAULT STYLE
        AddHandler UserLookAndFeel.Default.StyleChanged, AddressOf Default_StyleChanged

        'Light Dark Mode
        LookAndFeelBarButtonItemEvent()


    End Sub

    'FORM SHOWN
    Private Sub xfuiUser_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        'Me.Opacity = 1


        Dim xflogin As New xfuiLogin
        If xflogin.ShowDialog() = vbOK Then
            Try
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Initializing...")
                'CREATE CHART
                createTotalsChart()
                createVarianceChart()

                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Initializing local channel pairs...")
                'Init - User Channels
                getuserchannels()

                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Initializing local settings...")
                LoadFormSettings()

                '#RIBBON PAGE
                'Dim RibbonPage As DevExpress.XtraBars.Ribbon.RibbonPage = HomeRibbonPage 'HomeRibbonPage
                'PageRibbonControl.SelectedPage = RibbonPage

                ''#XTRATAB PAGE
                'Dim PageXtraTab As DevExpress.XtraTab.XtraTabPage = HomePageXtraTab
                'PageHeadersPanelXtraTabControl.SelectedTabPage = PageXtraTab

                HomeRibbonPage.Category.Color = Color.Goldenrod
                'Me.Text = "AIM Arbitrage - Home Page"

                'RibbonPage = LoginRibbonPage
                'RibbonPage.Visible = False

                'RibbonPage = SettingsRibbonPage
                'RibbonPage.Visible = False

                'PageRibbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Default

                InfoStaticBar.Caption = "AIM Arbitrage 2022 Release 01.00"
            Catch ex As Exception
                InfoStaticBar.Caption = "Login failed: " & ex.Message
            Finally
                Me.Opacity = 1
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
            End Try
        Else
            Me.Close()
            'InfoStaticBar.Caption = "Login failed, for AIM Arbitrage 2022 Release 01.00"
        End If
        'Ticker
        'ticker_response :  Ticker
    End Sub

    '#FORM CLOSING
    Private Sub xfuiUser_Closing(sender As Object, e As CancelEventArgs) Handles MyBase.Closing
        '#TERMINATE SESSION
        InfoStaticBar.Caption = "The application is shutting down, please wait... "
        'shutdown = True

        'Dim DefaultLookAndFeel = UserLookAndFeel.Default.SkinName
        'My.Settings.cfglookandfeelstyle = DefaultLookAndFeel
        'My.Settings.Save()
        'HostServiceStop()

    End Sub

    '#FORM CLOSING - ON EXIT
    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        If e.CloseReason <> CloseReason.WindowsShutDown AndAlso e.CloseReason <> CloseReason.ApplicationExitCall Then
            e.Cancel = True
        End If
        '#SAVE TABLES
        'SaveTables()
        '#HOST SERVICES TIMERS
        'shutdown = True
        'unsubsockets = True

        notifyiconTaskBar.Visible = False
        notifyiconTaskBar.Dispose()

        HostServiceStop()

        '#TIMER
        Timer1.Stop()
        e.Cancel = False
        MyBase.OnFormClosing(e)
    End Sub

#Region "#LOOK AND FEEL CUSTOMDRAW STYLES"

    'TECHNICAL ANALYTICS GRIDVIEW
    Private Sub AnalyticsGridView_CustomDrawCell(sender As Object, e As RowCellCustomDrawEventArgs) Handles AnalyticsGridView.CustomDrawCell
        Dim gridview As DevExpress.XtraGrid.Views.Grid.GridView = TryCast(sender, DevExpress.XtraGrid.Views.Grid.GridView)
        If Not gridview.RowCount > 0 Then Exit Sub
        Dim rowhandle As Integer = e.RowHandle
        Dim row As Object = gridview.GetRow(rowhandle)
        Dim value As Object = gridview.GetRowCellValue(e.RowHandle, "a_channelName")
        If value Is Nothing Or IsDBNull(value) Then Exit Sub
        If Not value.GetType() Is GetType(System.String) Then Exit Sub
        Dim _value As String = CStr(value)
        '#COLOUR
        Try
            If _value.ToLower.Contains("ask") Or _value.ToLower.Contains("sell") Then
                e.Appearance.ForeColor = Color.Red
            ElseIf _value.ToLower.Contains("bid") Or _value.ToLower.Contains("buy") Then
                e.Appearance.ForeColor = colorgreen
            End If
            'e.HighPriority = True   'override any other formatting
        Catch ex As Exception
        End Try

    End Sub

#Region "CUSTOM DRAW STYLES"

    'SELL GRIDVIEW
    Private Sub askGridView_CustomDrawCell(sender As Object, e As RowCellCustomDrawEventArgs) Handles askGridView.CustomDrawCell
        Dim gridview As DevExpress.XtraGrid.Views.Grid.GridView = TryCast(sender, DevExpress.XtraGrid.Views.Grid.GridView)
        If Not gridview.RowCount > 0 Then Exit Sub
        e.Appearance.ForeColor = Color.Red
        'If e.Column.Name = "colaskimg" Then
        '    Try
        '        Dim rowhandle As Integer = e.RowHandle
        '        Dim row As DataRowView = gridview.GetRow(rowhandle)

        '        Dim imageid As String = row("askalarmimg").ToString

        '        If Not String.IsNullOrEmpty(imageid) Then
        '            Dim img As Image = Nothing
        '            img = ribbonImageCollection16.Images("bellred16.png")
        '            If (img IsNot Nothing) Then
        '                Dim rct As New Rectangle(e.Bounds.Left, e.Bounds.Top, e.Bounds.Height, e.Bounds.Height)
        '                e.Graphics.DrawImage(img, rct)
        '            End If
        '        End If
        '    Catch ex As Exception
        '    End Try
        'End If
    End Sub

    'BUY GRIDVIEW
    Private Sub bidGridView_CustomDrawCell(sender As Object, e As RowCellCustomDrawEventArgs) Handles bidGridView.CustomDrawCell
        Dim gridview As DevExpress.XtraGrid.Views.Grid.GridView = TryCast(sender, DevExpress.XtraGrid.Views.Grid.GridView)
        If Not gridview.RowCount > 0 Then Exit Sub
        e.Appearance.ForeColor = colorgreen
    End Sub

    'BUY TRADES GRIDVIEW
    Private Sub TradeItemsBuyGridView_CustomDrawCell(sender As Object, e As RowCellCustomDrawEventArgs) Handles TradeItemsBuyGridView.CustomDrawCell
        Dim gridview As DevExpress.XtraGrid.Views.Grid.GridView = TryCast(sender, DevExpress.XtraGrid.Views.Grid.GridView)
        If Not gridview.RowCount > 0 Then Exit Sub
        e.Appearance.ForeColor = colorgreen
    End Sub
    'SELL TRADES GRIDVIEW
    Private Sub TradeItemsSellGridView_CustomDrawCell(sender As Object, e As RowCellCustomDrawEventArgs) Handles TradeItemsSellGridView.CustomDrawCell
        Dim gridview As DevExpress.XtraGrid.Views.Grid.GridView = TryCast(sender, DevExpress.XtraGrid.Views.Grid.GridView)
        If Not gridview.RowCount > 0 Then Exit Sub
        e.Appearance.ForeColor = Color.Red
    End Sub


#End Region 'CUSTOM DRAW STYLES

#Region "RESIZE UI"

    '#ORDERBOOK RESIZE
    Private Sub orderbookPanelControl_Resize(sender As Object, e As EventArgs) Handles orderbookPanelControl.Resize
        Try
            'spreadLabel.Height = 20
            Dim panelheight As Integer = orderbookPanelControl.Height
            panelheight = panelheight - askvolumeLabelControl.Height
            panelheight = panelheight - bidvolumeLabelControl.Height

            'panelheight = panelheight - spreadLabel.Height   'spreadLabel.Height 
            panelheight = panelheight - (spreadPanelControl.Height + spreadpricePanelControl.Height + spreadmarketPanelControl.Height)      'spreadLabel.Height 

            'panelheight = panelheight - 11  'bidGridControl.Height or askGridControl.Height
            panelheight = panelheight / 2

            askGridControl.Height = panelheight '+ 10
            bidGridControl.Height = panelheight

        Catch ex As Exception
        End Try
    End Sub

#End Region 'RESIZE UI

#Region "FORM RIBBONPAGEBAR"

#Region "#RIBBONPAGE BUTTONS - VIEWS"

    Private Sub ViewSignalsBarCheckItem_CheckedChanged(sender As Object, e As ItemClickEventArgs) Handles ViewSignalsBarCheckItem.CheckedChanged
        Dim IsVisible = SplitPanelVisibility.Both
        RemoveHandler ViewSignalsBarCheckItem.CheckedChanged, AddressOf ViewSignalsBarCheckItem_CheckedChanged
        If IsVisible = SplitPanelVisibility.Both Then
            ViewSignalsBarCheckItem.Checked = True
            SplitContainerControl3.PanelVisibility = SplitPanelVisibility.Panel1
        Else
            ViewSignalsBarCheckItem.Checked = False
            ViewTradesBarCheckItem.Checked = False
            SplitContainerControl3.PanelVisibility = SplitPanelVisibility.Both
        End If
        AddHandler ViewSignalsBarCheckItem.CheckedChanged, AddressOf ViewSignalsBarCheckItem_CheckedChanged
    End Sub

    Private Sub ViewTradesBarCheckItem_CheckedChanged(sender As Object, e As ItemClickEventArgs) Handles ViewTradesBarCheckItem.CheckedChanged
        Dim IsVisible = SplitPanelVisibility.Both
        RemoveHandler ViewTradesBarCheckItem.CheckedChanged, AddressOf ViewTradesBarCheckItem_CheckedChanged
        If IsVisible = SplitPanelVisibility.Both Then
            ViewTradesBarCheckItem.Checked = True
            SplitContainerControl3.PanelVisibility = SplitPanelVisibility.Panel2
        Else
            ViewTradesBarCheckItem.Checked = False
            ViewSignalsBarCheckItem.Checked = False
            SplitContainerControl3.PanelVisibility = SplitPanelVisibility.Both
        End If
        AddHandler ViewTradesBarCheckItem.CheckedChanged, AddressOf ViewTradesBarCheckItem_CheckedChanged
    End Sub

    Private Sub ViewTransactBarCheckItem_CheckedChanged(sender As Object, e As ItemClickEventArgs) Handles ViewTransactBarCheckItem.CheckedChanged

        SplitContainerControl3.PanelVisibility = SplitPanelVisibility.Both

    End Sub

#End Region '"#RIBBONPAGE BUTTONS - VIEWS"

    Private Sub configureBarButtonItem_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles configureBarButtonItem.ItemClick
        Dim PageSelected As DevExpress.XtraBars.Ribbon.RibbonPage = SettingsRibbonPage
        If PageSelected.Visible = True Then
            PageRibbonControl.SelectedPage = HomeRibbonPage
            PageSelected.Visible = False
        Else
            PageSelected.Visible = True
            PageRibbonControl.SelectedPage = PageSelected
        End If
    End Sub

    '#RIBBONBAR MAIN TAB PAGES
    Private Sub ribbonControl_SelectedPageChanged(sender As Object, e As EventArgs) Handles PageRibbonControl.SelectedPageChanged
        RaiseRibbonControlSelectedPageChanged(sender)
    End Sub
    '#RIBBONBAR - RIBBONTAB PAGE CHANGED EVENT
    Private Sub RaiseRibbonControlSelectedPageChanged(sender As Object)

        Dim PageSelected As DevExpress.XtraBars.Ribbon.RibbonPage = sender.SelectedPage


        'Dim _type As String = sender.GetType().ToString

        'If Not String.IsNullOrEmpty(_type) And _type = "DevExpress.XtraBars.Ribbon.RibbonControl" Then
        '    Exit Sub
        'End If

        If PageSelected Is Nothing Then
            PageSelected = HomeRibbonPage
        End If

        Dim PageXtraTab As DevExpress.XtraTab.XtraTabPage = TryCast(sender, DevExpress.XtraTab.XtraTabPage)

        '#CLEAR COLOR
        'Dim PageCategory As DevExpress.XtraBars.Ribbon.RibbonPageCategory = PageSelected.Ribbon.PageCategories(0)
        For Each PageCategory As DevExpress.XtraBars.Ribbon.RibbonPageCategory In PageSelected.Ribbon.PageCategories
            PageCategory.Color = Nothing
        Next
        'Console.WriteLine(PageCategory.Name)
        SettingsRibbonPage.Visible = False
        Select Case PageSelected.Name
            'Case LoginRibbonPage.Name
            '#LOGIN PAGE
            'InformationGroupControl.Visible = False
            'PageXtraTab = LoginPageXtraTab
            'PageHeadersPanelXtraTabControl.SelectedTabPage = LoginPageXtraTab  'PageXtraTab

                'Me.Text = "BitBuyBot - Login Page"
            Case ServiceRibbonPage.Name
                '#SERVICE
                'Dim RibbonPage As DevExpress.XtraBars.Ribbon.RibbonPage = ServiceRibbonPage
                'PageRibbonControl.SelectedPage = RibbonPage
                'HomeRibbonPage.Category.Color = Color.Goldenrod
                Me.Text = "AIM Arbitrage - Host Service"
            Case HomeRibbonPage.Name
                '#HOME PAGE
                InformationGroupControl.Visible = True
                PageXtraTab = HomePageXtraTab
                PageHeadersPanelXtraTabControl.SelectedTabPage = PageXtraTab
                HomeRibbonPage.Category.Color = Color.Goldenrod
                Me.Text = "AIM Arbitrage - Home Page"

            Case AdminRibbonPage.Name
                '#ADMIN PAGE
                InformationGroupControl.Visible = True
                PageXtraTab = AdminPageXtraTab
                PageHeadersPanelXtraTabControl.SelectedTabPage = PageXtraTab
                AdminRibbonPage.Category.Color = Color.Goldenrod
                Me.Text = "AIM Arbitrage - Admininstrator"

            Case RibbonPage4.Name
                'InformationGroupControl.Visible = True
                'ribbontabpage = TryCast(XtraTabPage5, DevExpress.XtraTab.XtraTabPage)
                'ctrl = ribbonpage.Ribbon.PageCategories(4)

            'Case TelegramRibbonPage.Name
                ''#TELEGRAM PAGE
                'InformationGroupControl.Visible = True
                'PageXtraTab = TelegramPageXtraTab
                'PageHeadersPanelXtraTabControl.SelectedTabPage = PageXtraTab
                'TelegramRibbonPage.Category.Color = Color.Goldenrod
                'Me.Text = "AIM Arbitrage - Telegram"

            Case SettingsRibbonPage.Name
                '#INDICATORS view
                SettingsRibbonPage.Visible = True
                InformationGroupControl.Visible = True
                PageXtraTab = SettingsPageXtraTab
                PageHeadersPanelXtraTabControl.SelectedTabPage = PageXtraTab
                SettingsRibbonPage.Category.Color = Color.Goldenrod
                Me.Text = "AIM Arbitrage - Settings"

            Case IndicatorsRibbonPage.Name
                '#SETTINGS
                '#INDICATORS view
                InformationGroupControl.Visible = True
                PageXtraTab = IndicatorsPageXtraTab
                PageHeadersPanelXtraTabControl.SelectedTabPage = PageXtraTab
                IndicatorsRibbonPage.Category.Color = Color.Goldenrod
                Me.Text = "AIM Arbitrage - Indicators"

            Case Else

        End Select

        'ctrl.Color = Color.Goldenrod
        'PageHeadersPanelXtraTabControl.SelectedTabPage = PageXtraTab
        'PageCategory.Color = Color.Goldenrod




        Dim item As String = PageRibbonControl.SelectedPage.Text
        Dim itemtext As String = PageRibbonControl.SelectedPage.Category.Text
        If String.IsNullOrEmpty(Name) Or Name.ToLower.Contains("homeribbonpage") Then Exit Sub
        If itemtext.ToLower.Contains("unassigned") Then Exit Sub
        'currencylistLookUpEdit.EditValue = itemtext 'baseparams.Pair

    End Sub

    '#RIBBONBAR - XTRATABPAGE SELECTED PAGE CHANGED EVENT
    Private Sub PageHeadersPanelXtraTabControl_SelectedPageChanged(sender As Object, e As DevExpress.XtraTab.TabPageChangedEventArgs) Handles PageHeadersPanelXtraTabControl.SelectedPageChanged
        Dim pageControl As DevExpress.XtraTab.XtraTabControl = TryCast(sender, DevExpress.XtraTab.XtraTabControl)
        Dim page As DevExpress.XtraTab.XtraTabPage = e.Page
        If page Is Nothing Then Exit Sub
        Select Case page.Name

            'SplitContainerControl3
            'TradeGridsHomePanelControl
            'TradeGridsAdminPanelControl

            Case HomePageXtraTab.Name
                'USER PAGE - TRADEGRIDS
                SplitContainerControl3.Dock = DockStyle.None
                SplitContainerControl3.Parent = TradeGridsHomePanelControl
                SplitContainerControl3.Dock = DockStyle.Fill
                'TradeTriggerGridView.ExpandAllGroups()

                '#USER PAGE - DOCKING PANEL
                'PageXtraTabControl.Dock = DockStyle.None
                'PageXtraTabControl.Parent = UserPanelControl
                'PageXtraTabControl.Dock = DockStyle.Fill

                '#TELEGRAM
                'TelegramUserPanelControl.Dock = DockStyle.None
                'TelegramUserPanelControl.Parent = TelegramXtraTabPage
                'TelegramUserPanelControl.Dock = DockStyle.Fill
            Case AdminPageXtraTab.Name
                'ADMIN PAGE - TRADEGRIDS
                SplitContainerControl3.Dock = DockStyle.None
                SplitContainerControl3.Parent = TradePairsPanelControl
                SplitContainerControl3.Dock = DockStyle.Fill
                'TradeTriggerGridView.ExpandAllGroups()

                '#ADMIN PAGE - DOCKING PANEL
                'PageXtraTabControl.Dock = DockStyle.None
                'PageXtraTabControl.Parent = AdminPanelControl
                'PageXtraTabControl.Dock = DockStyle.Fill

                '#TELEGRAM
                'TelegramUserPanelControl.Dock = DockStyle.None
                'TelegramUserPanelControl.Parent = TelegramXtraTabPage
                'TelegramUserPanelControl.Dock = DockStyle.Fill
            Case TelegramPageXtraTab.Name
                '#TELEGRAM PAGE
                'TelegramUserPanelControl.Dock = DockStyle.None
                'TelegramUserPanelControl.Parent = TelegramGroupControl
                'TelegramUserPanelControl.Dock = DockStyle.Fill
        End Select
    End Sub

    'Private Sub PageXtraTabControl_SelectedPageChanged(sender As Object, e As DevExpress.XtraTab.TabPageChangedEventArgs) Handles PageXtraTabControl.SelectedPageChanged
    'Dim pageControl As DevExpress.XtraTab.XtraTabControl = TryCast(sender, DevExpress.XtraTab.XtraTabControl)
    'Dim page As DevExpress.XtraTab.XtraTabPage = e.Page

    'Select Case page.Name
    '    Case AssetPriceXtraTabPage.Name

    '        AccountWalletPanelControl.Dock = DockStyle.None
    '        AccountWalletPanelControl.Parent = AssetWalletPanelControl
    '        AccountWalletPanelControl.Dock = DockStyle.Fill

    '    Case OrderBookXtraTabPage.Name

    '        AccountWalletPanelControl.Dock = DockStyle.None
    '        AccountWalletPanelControl.Parent = OrderBookWalletPanelControl
    '        AccountWalletPanelControl.Dock = DockStyle.Fill
    'End Select
    'End Sub

    '#ABOUT
    Private Sub iAbout_ItemClick(sender As Object, e As ItemClickEventArgs) Handles iAbout.ItemClick
        iAbout.Enabled = False
        Dim xfs As New xf_About
        xfs.closeLabel.Visible = True

        xfs.ShowDialog()
        xfs.BringToFront()

        iAbout.Enabled = True

        Dim url As String = "http://www.bitbuybot.com"
        System.Diagnostics.Process.Start(url)

    End Sub

    '#HELP
    Private Sub iHelp_ItemClick(sender As Object, e As ItemClickEventArgs) 'Handles iHelp.ItemClick
        ' Dim url As String = "http://www.aim.za.com/support"
        ' System.Diagnostics.Process.Start(url)
    End Sub

    Private Sub RibbonUpdatePages()
        Try
            Dim dt As DataTable = BaseDataSet.Tables("tickersDataTable") ': dt.Clear()
            For ii As Integer = 0 To dt.Rows.Count - 1
                Dim row As DataRow = dt.Rows(ii)
                Dim exchange As String = row("exchange")
                'If baseparams.Exchange.ToLower = exchange.ToLower Then
                '    Dim isenabled As Boolean = row("isenabled")
                '    Dim pair As String = row("pair")
                '    Dim price As Decimal = row("price")
                '    If isenabled = True Then
                '        Try
                '            Dim isexisting As Boolean = False
                '            For Each item As DevExpress.XtraBars.Ribbon.RibbonPageCategory In ribbonControl.PageCategories
                '                Dim name As String = item.Name
                '                If name = pair & "RibbonPageCategory" Then
                '                    item.Text = pair
                '                    item.Pages(0).Text = CStr(price)
                '                    Exit For
                '                End If
                '            Next
                '        Catch ex As Exception : exerr = ex
                '        End Try
                '    End If
                'End If
            Next
        Catch ex As Exception ': exerr = ex
        End Try
    End Sub

    Private Sub RibbonTogglePages()
        Try
            Dim dt As DataTable = BaseDataSet.Tables("tickersDataTable") ': dt.Clear()
            For ii As Integer = 0 To dt.Rows.Count - 1
                Dim row As DataRow = dt.Rows(ii)
                Dim pair As String = row("pair")
                For Each item As DevExpress.XtraBars.Ribbon.RibbonPageCategory In PageRibbonControl.PageCategories
                    Dim name As String = item.Name
                    If name = pair & "RibbonPageCategory" Then
                        item.Visible = False
                        Exit For
                    End If
                Next
            Next
            For ii As Integer = 0 To dt.Rows.Count - 1
                Dim row As DataRow = dt.Rows(ii)
                Dim exchange As String = row("exchange")
                'If baseparams.Exchange.ToLower = exchange.ToLower Then
                '    Dim isenabled As Boolean = row("isenabled")
                '    Dim pair As String = row("pair")
                '    Dim price As Decimal = row("price")
                '    If isenabled = True Then
                '        Try
                '            Dim isexisting As Boolean = False
                '            For Each item As DevExpress.XtraBars.Ribbon.RibbonPageCategory In ribbonControl.PageCategories
                '                Dim name As String = item.Name
                '                If name = pair & "RibbonPageCategory" Then
                '                    item.Text = pair
                '                    item.Pages(0).Text = CStr(price)
                '                    item.Visible = True
                '                    isexisting = True
                '                    Exit For
                '                End If
                '            Next
                '            If isexisting = False Then
                '                Dim control As New DevExpress.XtraBars.Ribbon.RibbonPageCategory
                '                control.Name = pair & "RibbonPageCategory"
                '                control.Text = pair
                '                control.Color = Color.Goldenrod
                '                control.Expanded = False
                '                control.Visible = True
                '                control.AutoStretchPageHeaders = True
                '                Me.ribbonControl.PageCategories.Add(control)
                '                Dim page As New DevExpress.XtraBars.Ribbon.RibbonPage
                '                page.Name = pair & "RibbonPage"
                '                page.Text = CStr(price)
                '                control.Pages.Add(page)
                '            End If
                '        Catch ex As Exception : exerr = ex
                '        End Try
                '    End If
                'End If
            Next
        Catch ex As Exception ': exerr = ex
        End Try
    End Sub

    'SHOW HIDE RIBBON BAR
    Private Sub ShowRibbonBarButtonItem_ItemClick(sender As Object, e As ItemClickEventArgs) Handles ShowRibbonBarButtonItem.ItemClick
        ShowRibbonBarButtonItem.Enabled = False
        Dim hide As Boolean = PageRibbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide
        If hide = True Then
            PageRibbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Show
            ShowRibbonBarButtonItem.ImageIndex = 108
        Else
            PageRibbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide
            ShowRibbonBarButtonItem.ImageIndex = 109
        End If
        ShowRibbonBarButtonItem.Enabled = True
    End Sub


#End Region 'FORM RIBBONPAGEBAR

#Region "LAYOUT PREFERENCES"



#End Region 'LAYOUT PREFERENCES

#Region "LOOK AND FEEL"
    '#LOOKANDFEEL - DARK LIGHT THEME
    Private Sub LookAndFeelBarButtonItem_ItemClick(sender As Object, e As ItemClickEventArgs) Handles LookAndFeelBarButtonItem.ItemClick, IsWhisperModeBarCheckItem.ItemClick
        LookAndFeelBarButtonItemEvent()
    End Sub
    Private Sub LookAndFeelBarButtonItemEvent()
        Dim DefaultLookAndFeel As String = My.Settings.propLookAndFeelStyle
        If DefaultLookAndFeel = "Basic" Then
            '#DARKMODE
            DefaultLookAndFeel = "Sharp Plus"
            UserLookAndFeel.Default.SetSkinStyle("Sharp Plus")
            LookAndFeelBarButtonItem.ImageIndex = 78
            LookAndFeelBarButtonItem.Caption = "Light Theme"
        Else
            '#LIGHTMODE
            DefaultLookAndFeel = "Basic"
            UserLookAndFeel.Default.SetSkinStyle("Basic")
            LookAndFeelBarButtonItem.ImageIndex = 104
            LookAndFeelBarButtonItem.Caption = "Dark Theme"
        End If

        My.Settings.propLookAndFeelStyle = DefaultLookAndFeel
        My.Settings.Save()

        'DevExpress.Skins.SkinManager.EnableFormSkins()
        'DevExpress.UserSkins.BonusSkins.Register()
        'Dim DefaultLookAndFeel As String = My.Settings.propLookAndFeelStyle
        'UserLookAndFeel.Default.SetSkinStyle(DefaultLookAndFeel)

    End Sub

#End Region ' LOOKANDFEEL

#End Region '#LOOK AND FEEL CUSTOMDRAW STYLES

#Region "#NAVIGATORS - SETTINGS"

    '#NAVBAR TOGGLES
    Private Sub NavBarToggle()
        For Each navgroup As NavBarGroup In ConfigNavBarControl.Groups
            For Each navitem As NavBarItemLink In navgroup.ItemLinks
                If navitem.Item.Enabled = False Then
                    navitem.Item.Enabled = True
                    'Exit For
                End If
            Next
        Next
        Application.DoEvents()
    End Sub
    Private Sub ConfigNavBarControl_Click(sender As Object, e As EventArgs) Handles ConfigNavBarControl.Click
        Dim ActiveGroupIndex = ConfigNavBarControl.Groups.IndexOf(ConfigNavBarControl.PressedGroup)
        If ActiveGroupIndex <> -1 Then
            ConfigNavBarControl.ActiveGroup = ConfigNavBarControl.Groups(ActiveGroupIndex)
            If ConfigNavBarControl.ActiveGroup.Expanded Then
                ConfigNavBarControl.ActiveGroup.Expanded = False
            Else
                ConfigNavBarControl.ActiveGroup.Expanded = True
            End If
        End If
        NavBarToggle()
        'NavigationFrameConfig.SelectedPage = NavPagePlaceholder
        NavBarHeaderLabelControl.Text = String.Empty
    End Sub

    '#WINDOWS SETTINGS - NAVBAR
    Private Sub WindowsNavBarItem_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles WindowsNavBarItem.LinkClicked
        NavBarToggle()
        WindowsNavBarItem.Enabled = False
        NavigationFrameConfig.SelectedPage = NavPageWindows
        NavBarHeaderLabelControl.Text = "Windows Settings"
    End Sub

    '#APPLICATION SETTINGS - NAVBAR
    Private Sub ApplicationNavBarItem_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles ApplicationNavBarItem.LinkClicked
        NavBarToggle()
        ApplicationNavBarItem.Enabled = False
        NavigationFrameConfig.SelectedPage = NavPageApplication
        NavBarHeaderLabelControl.Text = "Application Settings"
    End Sub

    '#ALERTS SETTINGS - NAVBAR
    Private Sub AlertsNavBarItem_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles SoundAlertsNavBarItem.LinkClicked
        NavBarToggle()
        SoundAlertsNavBarItem.Enabled = False
        NavigationFrameConfig.SelectedPage = NavPageAlarms
        NavBarHeaderLabelControl.Text = "Notifications & Alerts"
    End Sub

    '#VOICE SETTINGS - NAVBAR
    'Private Sub VoiceSettingsNavBarItem_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles VoiceSettingsNavBarItem.LinkClicked
    '    NavBarToggle()
    '    VoiceSettingsNavBarItem.Enabled = False
    '    NavigationFrameConfig.SelectedPage = NavPageVoiceSettings
    '    NavBarHeaderLabelControl.Text = "Voice Settings"
    'End Sub

    '#VOICE CONTROL - NAVBAR
    Private Sub VoiceControlNavBarItem_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs)
        'NavBarToggle()
        'NavBarHeaderLabelControl.Text = "Voice Control"
        'VoiceControlNavBarItem.Enabled = False
        'NavigationFrameConfig.SelectedPage = NavPageVoiceControl

    End Sub

    '#LOGIN CONTROL - NAVBAR
    Private Sub LoginNavBarItem_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles LoginNavBarItem.LinkClicked
        NavBarToggle()
        LoginNavBarItem.Enabled = False
        NavigationFrameConfig.SelectedPage = NavPageLogin
        NavBarHeaderLabelControl.Text = "Login Authentication"
    End Sub

    '#EXCHANGE API - NAVBAR
    Private Sub ExchangeApiNavBarItem_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles ExchangeApiNavBarItem.LinkClicked
        NavBarToggle()
        ExchangeApiNavBarItem.Enabled = False
        NavigationFrameConfig.SelectedPage = NavPageExchangeApi
        NavBarHeaderLabelControl.Text = "Exchange Authentication"
    End Sub

    '#TELEGRAM - NAVBAR
    Private Sub TelegramNavBarItem_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles TelegramNavBarItem.LinkClicked
        NavBarToggle()
        TelegramNavBarItem.Enabled = False
        NavigationFrameConfig.SelectedPage = NavPageTelegram
        NavBarHeaderLabelControl.Text = "Telegram Authentication"
    End Sub

    '#SERVER - NAVBAR
    Private Sub HostServerNavBarItem_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles HostServerNavBarItem.LinkClicked
        NavBarToggle()
        HostServerNavBarItem.Enabled = False
        NavigationFrameConfig.SelectedPage = NavPageServer
        NavBarHeaderLabelControl.Text = "Server Connection Settings"
    End Sub

    '#SUBSCRIPTION - NAVBAR
    Private Sub SubscriptionNavBarItem_LinkClicked(sender As Object, e As DevExpress.XtraNavBar.NavBarLinkEventArgs) Handles SubscriptionNavBarItem.LinkClicked
        NavBarToggle()
        SubscriptionNavBarItem.Enabled = False
        NavigationFrameConfig.SelectedPage = NavPageSubscription
        NavBarHeaderLabelControl.Text = "Subscription"
    End Sub

#End Region '#NAVIGATORS - SETTINGS

#Region "#FORM SETTINGS"
    '#FORM SETTINGS
    Private Sub LoadFormSettings()

        'SAVE LABEL CLICK HANDLER
        Dim layoutlabels() As LabelControl =
            {ExchangeListLayoutLabel, AnalyticsLayoutLabel, LocalPairsLayoutLabel, TradeHistoryLayoutLabel, ExchangePairsLayoutLabel, ExchangeWalletLayoutLabel, TradingPairsLayoutLabel}
        For Each layoutlabel As LabelControl In layoutlabels
            AddHandler layoutlabel.Click, AddressOf LayoutLabel_Click
        Next
        layoutlabels = Nothing

        'REFRESH LABEL CLICK HANDLER
        Dim refreshlabels() As LabelControl =
            {ExchangeWalletRefreshLabel, LocalPairsRefreshLabel, TradeHistoryRefreshLabel, ExchangeListRefreshLabel, TradingPairsLabelControl}
        For Each refreshlabel As LabelControl In refreshlabels
            AddHandler refreshlabel.Click, AddressOf RefreshLabel_Click
        Next
        refreshlabels = Nothing

        'LOCAL GRIDVIEWS LAYOUT LOAD HANDLER
        Dim gridviews() As GridView =
            {TradingPairGridView, ExchangePairsGridView, AccountWalletGridView, TradeItemsBuyGridView, TradeItemsSellGridView, AnalyticsGridView, LocalTradingPairsGridView, ExchangeGridView}
        'ADDHANDLER GRIDVIEW LAYOUT CLICK
        For Each view As GridView In gridviews
            loadlocallayout(view)
        Next
        'gridviews = Nothing

        'GRIDVIEW ROWCOUNT CHANGED HANDLER
        gridviews = {TradingPairGridView, ExchangePairsGridView, AccountWalletGridView, LocalTradingPairsGridView, ExchangeGridView}
        For Each view As GridView In gridviews
            AddHandler view.RowCountChanged, AddressOf gridview_rowcounthandler
        Next
        gridviews = Nothing


    End Sub

    'LOAD GRIDVIEW ROW COUNT HANDLERS
    Private Sub gridview_rowcounthandler(sender As Object, e As EventArgs)
        Dim view As GridView = TryCast(sender, GridView)
        Dim cnt As Integer = view.RowCount
        Select Case view.Name
            Case TradingPairGridView.Name
                TradingPairsCountLabelControl.Text = "Trading Pairs <" & CStr(cnt) & ">"

            Case ExchangePairsGridView.Name
                ExchangeCountLabelControl.Text = "Exchanges <" & CStr(cnt) & ">"

            Case LocalTradingPairsGridView.Name
                LocalTradingPairsLabelControl.Text = "Local Trading Pairs <" & CStr(cnt) & ">"

            Case ExchangeGridView.Name
                ExchangeCountLabelControl.Text = "Exchanges <" & CStr(cnt) & ">"


        End Select
    End Sub

    'LOAD LOCAL LAYOUT FILE
    Private Sub loadlocallayout(ByRef view As GridView)
        'ADDHANDLER GRIDVIEW LAYOUT CLICK
        Dim layoutname As String = view.Name.ToLower
        Dim layoutfile As String = LocalFolder("aim" & "." & layoutname & ".dat", "layout")
        If IO.File.Exists(layoutfile) Then
            view.RestoreLayoutFromXml(layoutfile)
        End If
    End Sub

    'SAVE LOCAL LAYOUT
    Private Sub LayoutLabel_Click(sender As Object, e As EventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(New LabelDelegate(AddressOf LoadFormSettings), New Object() {sender, e})
        Else
            Dim _labelcontrol As LabelControl = CType(sender, LabelControl)
            If _labelcontrol Is Nothing Then Exit Sub

            _labelcontrol.Enabled = False
            Select Case _labelcontrol.Name
                Case ExchangeListLayoutLabel.Name
                    SaveLayoutFile(ExchangeGridView)

                Case AnalyticsLayoutLabel.Name
                    SaveLayoutFile(AnalyticsGridView)

                Case LocalPairsLayoutLabel.Name
                    'SaveLayoutFile(TradingPairGridView)
                    SaveLayoutFile(LocalTradingPairsGridView)

                Case TradeHistoryLayoutLabel.Name
                    SaveLayoutFile(TradeItemsBuyGridView)
                    SaveLayoutFile(TradeItemsSellGridView)

                Case ExchangePairsLayoutLabel.Name
                    SaveLayoutFile(ExchangePairsGridView)

                Case ExchangeWalletLayoutLabel.Name
                    SaveLayoutFile(AccountWalletGridView)

                Case TradingPairsLayoutLabel.Name
                    SaveLayoutFile(TradingPairGridView)

            End Select
            _labelcontrol.Enabled = True

        End If
    End Sub

#End Region '#FORM SETTINGS

#Region "LAYOUT SAVE REFRESH - BUTTONS"

    '#GRIDVIEW - CUSTOM MENUBUTTONS
    Private Sub SaveLayoutFile(view As GridView)
        If Not view.RowCount > 0 Then
            InfoStaticBar.Caption = "Failed to saved " & view.Name.ToProperCase & " layout - no data records. "
            Exit Sub
        End If
        Try
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Saving Layout...")
            InfoStaticBar.Caption = "Saving " & view.Name.ToProperCase & " Layout..."

            Dim layoutname As String = view.Name.ToLower
            Dim layoutfile As String = LocalFolder("aim" & "." & layoutname & ".dat", "layout")
            view.SaveLayoutToXml(layoutfile)

            InfoStaticBar.Caption = "Saved " & view.Name.ToProperCase & " Layout."
        Catch ex As Exception
            MsgBox("Failed to save layout." & vbCrLf & ex.Message, MsgBoxStyle.OkCancel, Me.Text)
        Finally
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        End Try

        'End If
    End Sub

#Region "REFRESHLABELS"

    Private Sub RefreshLabel_Click(sender As Object, e As EventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(New LabelDelegate(AddressOf LoadFormSettings), New Object() {sender, e})
        Else
            Dim _labelcontrol As LabelControl = CType(sender, LabelControl)
            If _labelcontrol Is Nothing Then Exit Sub

            Try
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
                _labelcontrol.Enabled = False

                Select Case _labelcontrol.Name
                    Case ExchangeWalletRefreshLabel.Name
                        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Wallet balances...")
                        getuserbalances()
                        loadlocallayout(AccountWalletGridView)

                    Case LocalPairsRefreshLabel.Name
                        'TODO - ADMIN/LOCAL ACCESSLEVEL 
                        getuserchannels()

                    Case TradeHistoryRefreshLabel.Name
                    'TradeItemsBuyGridcontrol

                    Case ExchangeListRefreshLabel.Name
                        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Exchange list...")
                        getuserexchangesv2api()
                        loadlocallayout(ExchangeGridView)

                    Case TradingPairsLabelControl.Name
                        gettradingpairs()
                        loadlocallayout(TradingPairGridView)


                End Select
                _labelcontrol.Enabled = True


            Catch ex As Exception
                InfoStaticBar.Caption = "function failed: " & ex.Message
            Finally
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
            End Try
        End If
    End Sub

#End Region 'REFRESHLABELS

#End Region '#LAYOUT SAVE REFRESH - BUTTONS

#End Region '#FORM

#Region "#INIT SERVICES"
    'INIT START
    Private Sub threadingstartBarButtonItem_ItemClick(sender As Object, e As ItemClickEventArgs) Handles threadingstartBarButtonItem.ItemClick
        threadingstartBarButtonItem.Enabled = False
        RaiseCoinigy()
    End Sub

    Private Sub HostServiceStop()
        On Error Resume Next
        'unsubsockets = True

        '#ORDERBOOK THREAD
        If Not RaiseOrderBookAsyncThread Is Nothing Then
            RaiseOrderBookAsyncThread.Dispose()
        End If

    End Sub

    Private Sub RaiseCoinigy()
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Initializing credentials...")
        Try
            _coinigyclient = New CoinigyWsClient(New CoinigyWebSocketClientOptions With {
                .Creds = New Creds With {
                    .apiKey = api_key,
                    .apiSecret = api_secret
                },
                .DebugMode = True
             })

            'EVENTHANDLERS
            AddHandler _coinigyclient.OnClientReady, AddressOf Client_OnClientReady
            AddHandler _coinigyclient.OnFatality, AddressOf _coinigyclient_OnFatality
            AddHandler _coinigyclient.OnClosed, AddressOf Client_OnClosed
            AddHandler _coinigyclient.OnStateChanged, AddressOf _coinigyclient_OnStateChanged

            AddHandler _coinigyclient.OnOrderMessage, AddressOf _coinigyclient_OnOrderMessage
            AddHandler _coinigyclient.OnTradeMessage, AddressOf _coinigyclient_OnTradeMessage

            'AddHandler _coinigyclient.OnBlockMessage, AddressOf _coinigyclient_OnBlockMessage
            AddHandler _coinigyclient.OnFavoriteMessage, AddressOf _coinigyclient_OnFavoriteMessage
            AddHandler _coinigyclient.OnNewsMessage, AddressOf Client_OnNewsMessage
            AddHandler _coinigyclient.OnNotificationMessage, AddressOf Client_OnNotificationMessage
            AddHandler _coinigyclient.OnMessage, AddressOf _coinigyclient_OnMessage

            'AddHandler _coinigyclient.
            'ConnectLive

            Dim connectionResult = _coinigyclient.Connect()

            If connectionResult Then
                InfoStaticBar.Caption = "Connected."
            Else
                InfoStaticBar.Caption = "Failed to connect."
            End If
            'OutputConsole.WriteLine($"Connected = {connectionResult}", ConsoleColor.Green)
            'ManualResetEvent.WaitOne()

            TradeItemsBuy = New List(Of TradeItem)
            TradeItemsSell = New List(Of TradeItem)
            TradeItemsBuyGridcontrol.DataSource = TradeItemsBuy
            TradeItemsSellGridControl.DataSource = TradeItemsSell

            OrderItemsBuy = New List(Of OrderItem)().OrderBy(Function(x) x.Price).ToList()
            OrderItemsSell = New List(Of OrderItem)().OrderBy(Function(x) x.Price).ToList()
            'bidGridControl.DataSource = OrderItemsBuy
            'askGridControl.DataSource = OrderItemsSell


            ExchangeItemList = New List(Of ExchangeItem)
            'ExchangeGridControl.DataSource = ExchangeItemList

            ChannelItemList = New List(Of String)
            'ChannelGridControl.DataSource = ChannelItemList

        Catch ex As Exception
            MsgBox("Initializing credentials failed." & vbCrLf & ex.Message, MsgBoxStyle.OkCancel, Me.Text)
        Finally

            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        End Try



    End Sub

#End Region '#INIT SERVICES

#Region "#SOCKETEVENTS"

    Private Shared Sub _coinigyclient_OnStateChanged(ByVal newState As System.Net.WebSockets.WebSocketState, ByVal prevState As System.Net.WebSockets.WebSocketState)
        'OutputConsole.WriteLine($"Socket state changed from {prevState} to {newState}", ConsoleColor.Gray)
    End Sub

    Private Sub Client_OnClosed(ByVal reason As System.Net.WebSockets.WebSocketCloseStatus)
        'OutputConsole.WriteLine($"Socket Closed reason: {reason}", ConsoleColor.Red)
        InfoStaticBar.Caption = "Socket Closed reason: " & reason

    End Sub

    Private Shared Sub _coinigyclient_OnFatality(ByVal reason As String)
        ManualResetEvent.[Set]()
        'InfoStaticBar.Caption = "Fatal error: " & reason

    End Sub

    Private Sub Client_OnNotificationMessage(ByVal notification As NotificationDataItem)
        'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received notification {notification.Title}", ConsoleColor.Green)
        InfoStaticBar.Caption = "Received notification: " & notification.Title
    End Sub

    Private Sub Client_OnNewsMessage(ByVal news As NewsDataItem)
        'Private Shared Sub _coinigyclient_OnNewsMessage(ByVal news As NewsDataItem)
        'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received news item \"{news.Title}\"", ConsoleColor.Green)

        InfoStaticBar.Caption = "News notification: " & news.Title

    End Sub

    Private Shared Sub _coinigyclient_OnFavoriteMessage(ByVal favorites As FavoriteDataItem())
        'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received favorites count {favorites.Length}", ConsoleColor.Green)
    End Sub

    <Obsolete>
    Private Shared Sub _coinigyclient_OnBlockMessage(ByVal curr As String, ByVal block As BlockItem)
        'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received new block for {curr} height {block.BlockId}", ConsoleColor.Green)
    End Sub

    Private Sub _coinigyclient_OnTradeMessage(ByVal exchange As String, ByVal curr1 As String, ByVal curr2 As String, ByVal trade As TradeItem)
        'If InvokeRequired Then
        '    Invoke(New MethodInvoker(AddressOf _coinigyclient_OnTradeMessage))
        'Else


        'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received new trade for {exchange} market {curr1}/{curr2} price {trade.Price}", ConsoleColor.Green)
        'Dim trade As TradeItem

        'tradeItemsC1.Add(trade)
        '    Try

        '        TradeItemsBuyGridcontrol.RefreshDataSource()

        '        TradeItemsSellGridControl.RefreshDataSource()

        '    Catch ex As Exception
        '    End Try

        'End If

    End Sub

    Private Shared Sub _coinigyclient_OnMessage(ByVal messageType As MessageType, ByVal data As String)
        'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received {messageType} message : {data}", ConsoleColor.Yellow)
    End Sub

    Private Sub _coinigyclient_OnOrderMessage(ByVal exchange As String, ByVal curr1 As String, ByVal curr2 As String, ByVal orders As OrderItem())
        'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received orders for {exchange} market {curr1}/{curr2} count {orders.Length}", ConsoleColor.Green)

        'If exchange = "BMEX" Then

        '	'TODO Enable - buylabelC1 = "Buy (" + exchange + " " + DateTime.Now.ToString("hh:mm:ss.fff tt") + ")";
        '	'TODO Enable - selllabelC1 = "Sell (" + exchange + ")";

        '	lstSellC1 = orders.Where(Function(x) x.Ordertype.ToLower() = "sell").ToList()
        '	lstBuyC1 = orders.Where(Function(x) x.Ordertype.ToLower() = "buy").ToList()

        '	Dim maxBuy As Decimal = lstBuyC1.Max(Function(x) x.Price)
        '	Dim minBuy As Decimal = lstBuyC1.Min(Function(x) x.Price)

        '	Dim maxSell As Decimal = lstSellC1.Max(Function(x) x.Price)
        '	Dim minSell As Decimal = lstSellC1.Min(Function(x) x.Price)
        '	Dim s As String = "{DateTime.UtcNow.ToString("
        '	'TODO Enable -channel1orders = s;
        'Else

        '	'TODO Enable - selllabelC2 = "Sell (" + exchange + ")";

        '	lstSellC2 = orders.Where(Function(x) x.Ordertype.ToLower() = "sell").ToList()
        '	lstBuyC2 = orders.Where(Function(x) x.Ordertype.ToLower() = "buy").ToList()

        '	Dim maxBuy As Decimal = lstBuyC2.Max(Function(x) x.Price)
        '	Dim minBuy As Decimal = lstBuyC2.Min(Function(x) x.Price)
        '	Dim maxSell As Decimal = lstSellC2.Max(Function(x) x.Price)
        '	'TODO Enable -buylabelC2 = "Buy ("
        '	'TODO Enable -+exchange
        '	'TODO Enable -+" "
        '	'TODO Enable -+DateTime.Now.ToString("hh:mm:ss.fff tt")
        '	'TODO Enable -+")";

        '	'TODO Enable -string s = $"{DateTime.UtcNow.ToString("yyyy/MM/dd hh:mm:ss.fff tt")}: Received orders for {exchange} market {curr1}/{curr2} count {orders.Length}"
        '	'TODO Enable -+" - Buy[↓" + minBuy.ToString()
        '	'TODO Enable -+" - ↑" + maxBuy.ToString()
        '	'TODO Enable -+"] Sell[↓" + minSell.ToString()
        '	'TODO Enable -+" - ↑" + maxSell.ToString()
        '	'TODO Enable -+"]";
        '	'TODO Enable -channel2orders = s;

        '	Dim minSell As Decimal = lstSellC2.Min(Function(x) x.Price)

        '	BuyC1Gridcontrol.Invoke(New Action(Function()
        '										   'TODO - handle this cross threading error and don't set the DataSource.
        '										   'dxDataGrid.Refresh()
        '										   BuyC1Gridcontrol.DataSource = lstBuyC1
        '										   SellC1Gridcontrol.DataSource = lstSellC1



        '									   End Function))

        'End If


        'BuyC1Gridcontrol.DataSource = lstBuyC1
        ''BuyC1Gridcontrol.RefreshDataSource()

        'SellC1Gridcontrol.DataSource = lstSellC1
        ''SellC1Gridcontrol.RefreshDataSource()


    End Sub

    Private Sub Client_OnClientReady()
        DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
        DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Initializing Exchanges...")

        Try
            'EXCHANGE LIST
            ExchangeItemList = New List(Of ExchangeItem)
            Dim Exchanges As IEnumerable(Of ExchangeItem) = _coinigyclient.GetExchanges()
            ExchangeItemList = Exchanges.ToList()
            'ExchangeGridControl.Invoke(New Action(Function()
            '                                          ExchangeGridControl.DataSource = ExchangeItemList
            '                                          ExchangeGridControl.RefreshDataSource()
            '                                      End Function))

            ''Channel ASSET LIST
            'DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Initilizing Channel Pairs...")

            'Dim channels As IEnumerable(Of String) = _coinigyclient.GetChannels()
            'ChannelItemList = channels
            'ExchangePairsGridControl.Invoke(New Action(Function()
            '                                               'GridControl1.DataSource = channels
            '                                               'GridControl1.RefreshDataSource()
            '                                               'ADD CHANNEL TABLES
            '                                               Dim tablename As String = "ChannelDataTable"
            '                                               Dim table As DataTable = New DataTable(tablename)
            '                                               table = BaseDataSet.Tables("ChannelDataTable")
            '                                               For Each chan As String In ChannelItemList
            '                                                   chan = chan.Replace("--", "-")
            '                                                   Dim chans As String() = chan.Split(New Char() {"-"c})
            '                                                   Dim newRow As DataRow = table.NewRow()
            '                                                   newRow.BeginEdit()
            '                                                   newRow("ChanName") = chan
            '                                                   If chans.Length > 1 Then
            '                                                       newRow("ChanMethod") = chans(0)
            '                                                       newRow("ChanExchCode") = chans(1)
            '                                                       newRow("ChanPrimary") = chans(2)
            '                                                       newRow("ChanSecond") = chans(3)
            '                                                   End If
            '                                                   newRow("ChanVal") = chan
            '                                                   newRow.EndEdit()
            '                                                   table.Rows.Add(newRow)
            '                                                   newRow.AcceptChanges()
            '                                               Next
            '                                               table.AcceptChanges()

            '                                           End Function))


            'SUBSCRIBE
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Initilizing Subscription Channels...")
            '_coinigyclient.SubscribeToChannel("TRADE-BMEX--XBT--USDT", AddressOf MyCustomCallback)
            '_coinigyclient.SubscribeToChannel("TRADE-BINA--BTC--USDT", AddressOf MyCustomCallback)

            _coinigyclient.SubscribeToChannel("ORDER-BMEX--XBT--USDT", AddressOf MyCustomCallback)
            _coinigyclient.SubscribeToChannel("ORDER-BINA--BTC--USDT", AddressOf MyCustomCallback)
            _coinigyclient.SubscribeToChannel("ORDER-BIFU--BTC--USDT", AddressOf MyCustomCallback)
            _coinigyclient.SubscribeToChannel("ORDER-BTRX--BTC--USDT", AddressOf MyCustomCallback)

            _coinigyclient.SubscribeToChannel("TRADE-BMEX--XBT--USDT", AddressOf MyCustomCallback)
            _coinigyclient.SubscribeToChannel("TRADE-BINA--BTC--USDT", AddressOf MyCustomCallback)
            _coinigyclient.SubscribeToChannel("TRADE-BIFU--BTC--USDT", AddressOf MyCustomCallback)
            _coinigyclient.SubscribeToChannel("TRADE-BTRX--BTC--USDT", AddressOf MyCustomCallback)

            '_coinigyclient.SubscribeToChannel("TRADE-BINA--ZRX--USDT", AddressOf MyCustomCallback)
            '_coinigyclient.SubscribeToChannel("ORDER-BINA--ZRX--USDT", AddressOf MyCustomCallback)

            'PRIVATE CHANNEL
            '_coinigyclient.SubscribeToChannel("651CD393-726A-F6DC-43EA-213DCA3845FB")

            '_coinigyclient.SubscribeToChannel("NEWS")

            ''TRADE CHANNELS
            '_coinigyclient.SubscribeToChannel("TRADE-BMEX--XBT--USDT", AddressOf MyCustomCallback)
            '_coinigyclient.SubscribeToChannel("TRADE-BINA--XBT--USDT", AddressOf MyCustomCallback)

            ''ORDER CHANNELS
            '_coinigyclient.SubscribeToChannel("ORDER-BINA--XBT--USDT", AddressOf MyCustomCallback)
            '_coinigyclient.SubscribeToChannel("ORDER-BMEX--XBT--USDT", AddressOf MyCustomCallback)

            '_coinigyclient.SubscribeToChannel("TRADE-BINA--ZRX--USDT", AddressOf MyCustomCallback)
            '_coinigyclient.SubscribeToChannel("ORDER-BINA--ZRX--USDT", AddressOf MyCustomCallback)

            'ORDER-BINA--XTZ--BTC


        Catch ex As Exception
            MsgBox("Initializing client failed." & vbCrLf & ex.Message, MsgBoxStyle.OkCancel, Me.Text)
        Finally
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        End Try


    End Sub

    'UNSUB
    Public Function unsub() As Object 'IEnumerable(Of ExchangeItem)
        'If _scClient Is Nothing OrElse _coinigyclient.SocketState <> WebSocketState.Open Then Return Nothing

        '_coinigyclient.Unsubscribe("")

        '_coinigyclient.Unsubscrib("TRADE-BMEX--XBT--USDT")
        '_coinigyclient.Unsubscribe("TRADE-BINA--XBT--USDT")

        ''ORDER CHANNELS
        '_coinigyclient.SubscribeToChannel("ORDER-BINA--XBT--USDT")
        '_coinigyclient.SubscribeToChannel("ORDER-BMEX--XBT--USDT")

        '_scClient.Disconnect()

    End Function

    Public Property User_Agent As String
    Public Property Server_Url As String
    'Private Shared _coinigyAPI As Coinigy.API.CoinigyApi

    Public Function Getapiresults() As Object 'IEnumerable(Of ExchangeItem)


        Dim _datetime As DateTime = DateTime.Now
        _datetime = DateAdd(DateInterval.Hour, -2, _datetime)
        Dim UnixTimeStamp As String = CStr(CInt(unixtimeext.UnixTimeFromDateTimeUTC(_datetime)))

        'UNIX TIME
        'Dim X_API_TIMESTAMP As String = UnixStamp
        'X_API_SIGN = SignSignature(UnixTimeStamp)

        '_coinigyAPI = New CoinigyApi(api_key, X_API_SIGN)

        Dim abla = _coinigyAPI.Exchanges()

        'Dim abla As Responses.exchanges_response = _apiclient.Exchanges()
        'Dim abla As Responses.userInfo_response = _apiclient.UserInfo()

        If Not abla Is Nothing Then
            Console.WriteLine()
        End If
        Console.WriteLine()

    End Function

#Region "SIGNATURE"

    Private BASE_URL As String = "https://api.coinigy.com"
    'Private ENDPOINT As String = "/api/v2/private/exchanges"
    Private X_API_KEY As String = "8d3e1cbe41f747e2ad5d4b9510ecd55f"
    Private X_API_SECRET As String = "12db3e29366b490e8737b53ef7763afc"
    Private Shared X_API_TIMESTAMP As String
    'Private Shared METHOD As Object = "GET"
    'Private Shared X_API_SIGN As String
    'Public Shared SECRET As Object = String.Empty

    'CLASS
    'Module [Module]
    '    Public Shared BASE_URL As Object = "https://api.coinigy.com"
    '    Public Shared ENDPOINT As Object = "/api/v2/private/user/balances"
    '    Public Shared X_API_KEY As Object = "8d3e1cbe41f747e2ad5d4b9510ecd55f"
    '    Public Shared SECRET As Object = "12db3e29366b490e8737b53ef7763afc"
    '    Public Shared METHOD As Object = "GET"
    '    Public Shared UNIXTIME As Object = DateTime.fromtimestamp(time.time(), TimeZone.utc)
    '    Public Shared X_API_TIMESTAMP As Object = Convert.ToInt32(DateTime.timestamp(UNIXTIME)).ToString()
    '    Public Shared query_string As Object = ""
    '    Public Shared msg As Object = X_API_KEY + X_API_TIMESTAMP + METHOD + ENDPOINT
    '    Public Shared signature_bytes As Object = HMAC.[new](SECRET.encode("ascii"), msg.encode("ascii"), digestmod:=hashlib.sha256).digest()
    '    Public Shared signature_hex As Object = map("{:02X}".Format, signature_bytes)
    '    Public Shared X_API_SIGN As Object = "".Join(signature_hex)
    '    Public Shared headers As Object = New Dictionary(Of Object, Object) From {
    '    {"Accept", "application/json"},
    '    {"Content-Type", "application/json"},
    '    {"X-API-SIGN", X_API_SIGN},
    '    {"X-API-TIMESTAMP", X_API_TIMESTAMP},
    '    {"X-API-KEY", X_API_KEY}
    '}
    '    Public Shared r As Object = requests.[get](BASE_URL + ENDPOINT, headers:=headers)
    'End Module



    'URLENCODE
    Private Shared Function UrlEncode(ByVal parameters As IDictionary(Of String, String)) As String
        Dim sb = New StringBuilder()
        For Each Item In parameters
            sb.AppendFormat("{0}={1}&", Item.Key, HttpUtility.UrlEncode(Item.Value))
        Next
        sb.Remove(sb.Length - 1, 1)
        Return sb.ToString()
    End Function

    'CALCULATE SIGNATURE - HMAC
    Public Shared Function CalculateSignature(ByVal text As String, ByVal secretKey As String) As String
        'SIGNATURE
        'Dim hmAcSha = New HMACSHA256(Encoding.ASCII.GetBytes(SECRET))
        'Dim URIbyte = Encoding.ASCII.GetBytes(msg)
        'Dim hashmessage = hmAcSha.ComputeHash(URIbyte)
        'Dim sign = BitConverter.ToString(hashmessage)
        'sign = sign.Replace("-", "")

        'SIGNATURE
        Using hmacsha256 = New System.Security.Cryptography.HMACSHA256(System.Text.Encoding.ASCII.GetBytes(secretKey))
            hmacsha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(text))
            Return String.Concat(hmacsha256.Hash.[Select](Function(b) b.ToString("x2")).ToArray()).ToUpper
        End Using
    End Function


    'POST SIGNATURE REQUEST
    Public Function PostRequestClientAsync(ByVal EndpointUrl As String) As HttpResponseMessage

        Dim response As HttpResponseMessage = New HttpResponseMessage
        Try


            Dim ENDPOINT As String = EndpointUrl

            Dim X_API_KEY As String
            X_API_KEY = "8d3e1cbe41f747e2ad5d4b9510ecd55f"

            Dim SECRET As String
            SECRET = "12db3e29366b490e8737b53ef7763afc"

            Dim METHOD As String
            METHOD = "GET"

            Dim _datetime As DateTime = DateTime.Now
            _datetime = DateAdd(DateInterval.Hour, -2, _datetime)
            Dim UnixTime As Long = unixtimeext.UnixTimeFromDateTimeUTC(_datetime)

            Dim PARAMS As New Dictionary(Of String, String)
            PARAMS.Add("StartDate", "2019-01-01T00,00,00.000Z")
            PARAMS.Add("EndDate", "2019-01-02T00:00:00.000Z")

            Dim BODY As String
            BODY = ""

            X_API_TIMESTAMP = CStr(CInt(UnixTime))

            'URL PARSE
            Dim query_string As String = "?StartDate=" & "'2019-01-01T00:00:00.000Z'" & "EndDate=" & "2019-01-02T00:00:00.000Z"
            Dim query_result = (String.Format("{0}api/1.1/api/v2/private/user", BASE_URL), UrlEncode(PARAMS))
            query_string = query_result.Item2

            'query_string = ""
            'msg = X_API_KEY & METHOD & ENDPOINT '& "?" & query_string & BODY
            Dim msg As String = X_API_KEY & X_API_TIMESTAMP & METHOD & ENDPOINT

            Dim X_API_SIGN As String = CalculateSignature(msg, SECRET)

            Dim postdata As New List(Of KeyValuePair(Of String, String))
            Dim client = New HttpClient()
            client.DefaultRequestHeaders.Add("Accept", "application/json")
            client.DefaultRequestHeaders.Add("X-API-SIGN", X_API_SIGN)
            client.DefaultRequestHeaders.Add("X-API-TIMESTAMP", X_API_TIMESTAMP)
            client.DefaultRequestHeaders.Add("X-API-KEY", X_API_KEY)
            'Dim content = New FormUrlEncodedContent(postdata)
            Server_Url = (BASE_URL + ENDPOINT) & "?Timestamp=" & X_API_TIMESTAMP
            response = client.GetAsync(Server_Url).Result
            'Return If(response.IsSuccessStatusCode, response.Content.ReadAsStringAsync().Result, "ERROR:" & response.StatusCode & " " + response.ReasonPhrase & " | " + response.RequestMessage)
            'End Function

        Catch ex As Exception
            MsgBox("Error: PostRequestClientAsync: " & vbCrLf & ex.Message)
        End Try

        Return response

        Console.WriteLine()

    End Function

    '************************************************
    Public Async Sub SigRoutineWorking()

        'Dim BASE_URL As String
        'BASE_URL = "https://api.coinigy.com"
        'Dim ENDPOINT As String
        Dim ENDPOINT As String = "/api/v2/private/user"
        'ENDPOINT = "/api/v2/private/exchanges"
        'ENDPOINT = "/api/v2/private/exchanges/BINA/markets/BTC/USDT/ohlc/h"
        'ENDPOINT = "/api/v2/private/user/balances"

        Dim X_API_KEY As String
        X_API_KEY = "8d3e1cbe41f747e2ad5d4b9510ecd55f"
        Dim SECRET As String
        SECRET = "12db3e29366b490e8737b53ef7763afc"
        Dim METHOD As String
        METHOD = "GET"
        Dim _datetime As DateTime = DateTime.Now
        _datetime = DateAdd(DateInterval.Hour, -2, _datetime)
        Dim UnixTime As Long = unixtimeext.UnixTimeFromDateTimeUTC(_datetime)
        Dim PARAMS As New Dictionary(Of String, String)
        PARAMS.Add("StartDate", "2019-01-01T00,00,00.000Z")
        PARAMS.Add("EndDate", "2019-01-02T00:00:00.000Z")
        Dim BODY As String
        BODY = ""
        X_API_TIMESTAMP = CStr(CInt(UnixTime))
        Dim query_string As String
        Dim r() As String
        Dim X_API_SIGN As String
        Dim msg As String
        Dim version = 1

        'URL PARSE
        query_string = "?StartDate=" & "'2019-01-01T00:00:00.000Z'" & "EndDate=" & "2019-01-02T00:00:00.000Z"
        Dim result = (String.Format("{0}api/1.1/api/v2/private/user", BASE_URL), UrlEncode(PARAMS))
        query_string = result.Item2

        'query_string = ""
        'msg = X_API_KEY & METHOD & ENDPOINT '& "?" & query_string & BODY
        msg = X_API_KEY & X_API_TIMESTAMP & METHOD & ENDPOINT

        Dim sign As String = CalculateSignature(msg, SECRET)
        X_API_SIGN = sign

        'Dim _datetime As DateTime = DateTime.Now
        '_datetime = DateAdd(DateInterval.Hour, -2, _datetime)
        'Dim UnixTimeStamp As String = CStr(CInt(unixtimeext.UnixTimeFromDateTimeUTC(_datetime)))
        'X_API_SIGN = SignSignature(UnixTimeStamp)
        'PostRequestClient(UnixTimeStamp)


        Dim postdata As New List(Of KeyValuePair(Of String, String))
        Dim client = New HttpClient()
        client.DefaultRequestHeaders.Add("Accept", "application/json")
        client.DefaultRequestHeaders.Add("X-API-SIGN", X_API_SIGN)
        client.DefaultRequestHeaders.Add("X-API-TIMESTAMP", X_API_TIMESTAMP)
        client.DefaultRequestHeaders.Add("X-API-KEY", X_API_KEY)

        Dim content = New FormUrlEncodedContent(postdata)
        Dim nonce = Environment.TickCount

        'Server_Url = "https://api.coinigy.com/api/v2/public/exchanges"
        'Server_Url = "https://api.coinigy.com/api/v2/private/user/balances?Timestamp=" & X_API_TIMESTAMP

        Server_Url = (BASE_URL + ENDPOINT) & "?Timestamp=" & X_API_TIMESTAMP


        'Server_Url = "https://api.coinigy.com/api/v2/private/exchanges?x-api-key=8d3e1cbe41f747e2ad5d4b9510ecd55f&x-api-secret=12db3e29366b490e8737b53ef7763afc&Timestamp=" & "100933953"
        'Server_Url = "https://api.coinigy.com/api/v2/public/exchanges?x-api-key=0448b73a465d3946b00b9e6b7b86bf01&x-api-secret=f2358363acecf50085ade2e13692d0dd&Timestamp=" & "100933953"

        Dim response = client.GetAsync(Server_Url).Result
        'Return If(response.IsSuccessStatusCode, response.Content.ReadAsStringAsync().Result, "ERROR:" & response.StatusCode & " " + response.ReasonPhrase & " | " + response.RequestMessage)
        'End Function

        If response.IsSuccessStatusCode = True Then
            Dim responseresult = response.Content.ReadAsStringAsync().Result
            Console.WriteLine()
        End If

        Console.WriteLine()

    End Sub

    '**************************************************
    'CONNECT
    '_coinigyAPI = New CoinigyApi(api_key, api_secret)
    ''Dim abla = _coinigyAPI.Exchanges()
    ''Dim abla As Responses.exchanges_response = _apiclient.Exchanges()
    'Dim abla As Responses.userInfo_response = _coinigyAPI.UserInfo()
    'If Not abla Is Nothing Then
    '    Console.WriteLine()
    'End If
    'Console.WriteLine()
    'Exit Sub
    '****************************************************
    'PYTHON3
    'BASE_URL = 'https://api.coinigy.com'
    '#ENDPOINT = '/api/v2/private/exchanges/BINA/markets/BTC/USDT/ohlc/h'
    'ENDPOINT = '/api/v2/private/user'
    '#ENDPOINT = '/api/v2/private/user/balances'

    'X_API_KEY = '8d3e1cbe41f747e2ad5d4b9510ecd55f'
    'SECRET = '12db3e29366b490e8737b53ef7763afc'
    'METHOD = 'GET'
    'UNIXTIME = datetime.fromtimestamp(time.time(), timezone.utc)
    'PARAMS = {'StartDate':'2019-01-01T00:00:00.000Z', 'EndDate':'2019-01-02T00:00:00.000Z'}
    'BODY = ''

    'X_API_TIMESTAMP = str(int(datetime.timestamp(UNIXTIME)))
    '#'1649973293'
    'query_string = "?" + "&".join( [ key + '=' + urllib.parse.quote_plus(PARAMS[key]) for key in PARAMS.keys() ] )
    '#'?StartDate=2019-01-01T00%3A00%3A00.000Z&EndDate=2019-01-02T00%3A00%3A00.000Z'
    'msg = X_API_KEY + X_API_TIMESTAMP + METHOD + ENDPOINT + (query_string if len(query_string) > 1 else '') + BODY
    '#'8d3e1cbe41f747e2ad5d4b9510ecd55f1649973293GET/api/v2/private/user?StartDate=2019-01-01T00%3A00%3A00.000Z&EndDate=2019-01-02T00%3A00%3A00.000Z'
    'signature_bytes = hmac.New(SECRET.encode("ascii"), msg.encode("ascii"), digestmod=hashlib.sha256).digest()
    '#b'\x91\xcf\xf4\xd9+\x85\x04(\xd4t\x7fe\xa7\\9\x83\x02\x98\r\xa92\xdb\xfa)b\x15.T\xbf\x11zU'
    'signature_hex = map("{:02X}".format, signature_bytes)
    '#<map object at 0x0000021AD98AE108>
    'X_API_SIGN = ''.join(signature_hex)
    '#'91CFF4D92B850428D4747F65A75C398302980DA932DBFA2962152E54BF117A55'

    'print(X_API_SIGN)

    'headers = {'Accept': 'application/json', 'Content-Type': 'application/json', 'X-API-SIGN': X_API_SIGN, 'X-API-TIMESTAMP' : X_API_TIMESTAMP, 'X-API-KEY': X_API_KEY}

#End Region 'SIGNATURE

#End Region '#SOCKETEVENTS

#Region "##ONCLIENT"

    Dim m_i As Integer = 0
    Dim m_Dictionary As New Dictionary(Of String, List(Of OrderItem))

    Public colorgreen As Color = Color.FromArgb(0, 192, 0)
    Public colorspread As Color = Color.FromArgb(255, 230, 232, 242)
    Private Delegate Sub StringDelegate(<[ParamArray]()> ByVal params As OrderResponse)

    'CLIENTONTRADEMESSAGE
    Public Sub clientontrademessage(ByVal trade As TradeResponse)
        Try
            Dim TradeItm As TradeItem = trade.TradeData.Trade
            If Not TradeItm Is Nothing Then
                If TradeItm.Type.ToLower = TradeType.Buy.ToString.ToLower Then
                    TradeItemsBuy.Add(TradeItm)
                End If
                If TradeItm.Type.ToLower = TradeType.Sell.ToString.ToLower Then
                    TradeItemsSell.Add(TradeItm)
                End If
            End If

            TradeItemsBuyGridcontrol.DataSource = TradeItemsBuy
            TradeItemsSellGridControl.DataSource = TradeItemsSell

            TradeItemsBuyGridcontrol.RefreshDataSource()
            TradeItemsSellGridControl.RefreshDataSource()

        Catch ex As Exception
        End Try
    End Sub

    'CLIENTONORDERMESSAGE
    Public Sub clientonordermessage(ByVal orderslist As OrderResponse)
        If Me.InvokeRequired Then
            Me.Invoke(New StringDelegate(AddressOf clientonordermessage), New OrderResponse() {orderslist})
        Else
            Dim OrderItm() As OrderItem = orderslist.OrderData.Orders
            OrderItemsSell = OrderItm.Where(Function(x) x.Ordertype() = TradeType.Sell.ToString).ToList()
            OrderItemsBuy = OrderItm.Where(Function(x) x.Ordertype() = TradeType.Buy.ToString).ToList()
            'bidGridcontrol.DataSource = OrderItemsBuy
            'askGridcontrol.DataSource = OrderItemsSell
            'bidGridcontrol.RefreshDataSource()
            'askGridcontrol.RefreshDataSource()

            'Buy Lowest Sell in Sellorders 
            Dim ChannnelName As String = orderslist.OrderData.Channel & "--BUY"
            'Dim askMin As Decimal = OrderItemsSell.Min(Function(x) x.Price)
            'askvolumeLabelControl.Text = ChannnelName 'REMOVE UI

            getorderdata(ChannnelName, orderslist)

            'Sell Highest Buy in Buyorders 
            ChannnelName = orderslist.OrderData.Channel & "--SELL"
            'Dim bidMax As Decimal = OrderItemsBuy.Max(Function(x) x.Price)
            'bidvolumeLabelControl.Text = ChannnelName 'REMOVE UI

            getorderdata(ChannnelName, orderslist)

            ''SPREAD PERCENTAGE
            'Dim spread As Decimal = 0
            'Dim percentage As Decimal = 0
            ''If isnotnullorzero(lastask) And isnotnullorzero(lastbid) Then
            'spread = askMin - bidMax
            'percentage = CalculateChange(bidMax, askMin)
            'percentage = (Math.Round(percentage, 4) * 100)
            ''#Percentage Color
            'If percentage <= 0.01 Then
            '    spreadLabel.ForeColor = Color.Red
            'ElseIf percentage > 0.01 Then
            '    spreadLabel.ForeColor = colorgreen
            'Else
            '    spreadLabel.ForeColor = colorspread
            'End If
            'spreadLabel.Text = "Spread " & percentage & "%" & "  " & spread
            ''Me.Text = row("price").ToString & " " & pair & " " & exchange.ToProperCase

        End If
    End Sub

    'CUSTOMCALLBACK
    Private Sub MyCustomCallback(ByVal messageType As MessageType, ByVal data As String)
        'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received {messageType} message in private channel callback", ConsoleColor.Green)

        'OutputConsole.WriteLine(DateTime.UtcNow & " Received " & messageType & " message in private channel callback") ', ConsoleColor.Green

        Select Case messageType
            Case MessageType.TradeData
                Try
                    'OutputConsole.WriteLine($"{DateTime.UtcNow}: We have deserialized a trade in our custom callback and the price in that trade is {deserializedClass.TradeData.Trade.Price}", ConsoleColor.Green)
                    Dim deserializedClass As TradeResponse = TradeResponse.FromJson(_coinigyclient.Options.Serializer, data)
                    clientontrademessage(deserializedClass)
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    InfoStaticBar.Caption = ex.Message
                End Try
            Case MessageType.OrderData
                Try
                    'OutputConsole.WriteLine($"{DateTime.UtcNow}: We have deserialized an orderbook in our custom callback and the first price in that data is {deserializedClass.OrderData.Orders(0).Price}", ConsoleColor.Green)
                    Dim deserializedClass = OrderResponse.FromJson(_coinigyclient.Options.Serializer, data)
                    clientonordermessage(deserializedClass)
                Catch ex As Exception
                    'Console.WriteLine(ex.Message)
                    InfoStaticBar.Caption = ex.Message
                End Try

            Case MessageType.NewsData
            Case MessageType.BlockData
            Case MessageType.FavoriteData
            Case MessageType.NewMarket
            Case MessageType.NotificationData
            Case MessageType.Unknown
            Case Else
                Throw New ArgumentOutOfRangeException(NameOf(messageType), messageType, Nothing)
        End Select
    End Sub

#Region "#ONFUNCTION"
    'Note: row count values # for eval
    'CreateOrderBookTable(ChannnelName, OrderItemsBuy)
    'Note if table does not have 20 rows discard or pause

    'SPREAD PERCENTAGE CALCULATE
    Private Function CalculateChange(ByVal previous As Decimal, ByVal current As Decimal) As Decimal
        If previous = 0 Or current = 0 Then Return 0.0
        If previous = 0 Then Throw New InvalidOperationException()
        Dim change = current - previous
        Return CDec(change) / previous
    End Function

    'EXCHANGES V2 -DEBUG BUG FIXED
    Private Sub getuserexchangesv2api()
        Dim ENDPOINT = "/api/v2/private/exchanges"
        Dim result As String = String.Empty
        Dim response As HttpResponseMessage = PostRequestClientAsync(ENDPOINT)

        If response.IsSuccessStatusCode = True Then
            Console.WriteLine()
            result = response.Content.ReadAsStringAsync().Result
            Dim userexchanges As exchanges_response = JsonConvert.DeserializeObject(Of exchanges_response)(result)
            Console.WriteLine(response.StatusCode)
            Try
                'ADD ORDERBOOK DATASET
                Dim exchangedata As DataSet = ExchangeDataSet
                Dim tablename As String = "ExchangeDataTable"

                'ADD CHANNEL TABLES
                Dim table As New DataTable()
                table = New DataTable(tablename)
                exchangedata.Tables.Add(table)

                Dim col As New DataColumn
                col = New DataColumn : col.ColumnName = "Id"
                col.Caption = "Id"
                col.AutoIncrement = True
                col.AutoIncrementSeed = 0
                col.AutoIncrementStep = 1
                table.Columns.Add(col)
                col.Dispose() : col = Nothing

                Dim userexchange As New Exchange

                Dim value As Object
                Dim T As Type = userexchange.GetType
                For Each fi As System.Reflection.FieldInfo In T.GetFields
                    Dim valuenam As String = fi.Name
                    value = fi.GetValue(userexchange)
                    Dim valuetyp As Type = fi.FieldType
                    table.Columns.Add(New DataColumn(valuenam, valuetyp))
                Next
                Console.WriteLine()
                For Each exch As Exchange In userexchanges.result
                    Dim newRow As DataRow = table.NewRow()
                    newRow.BeginEdit()
                    newRow("exchid") = exch.exchid
                    newRow("exchcode") = exch.exchcode
                    newRow("exchname") = exch.exchname
                    newRow("exchfee") = exch.exchfee
                    newRow("exchurl") = exch.exchurl

                    newRow("exchbalanceenabled") = exch.exchbalanceenabled
                    newRow("exchtradeenabled") = exch.exchtradeenabled

                    newRow.EndEdit()
                    table.Rows.Add(newRow)
                    newRow.AcceptChanges()
                Next
                table.AcceptChanges()

                'reflection
                'https://kellyschronicles.wordpress.com/2008/10/31/iterate-over-all-fields-and-properties-in-an-object-with-vb-net/

                ExchangeGridControl.DataMember = "ExchangeDataTable"
                ExchangeGridControl.DataSource = table 'walletdata
                ExchangeGridControl.RefreshDataSource()

                'Return dt
            Catch ex As Exception
                Console.WriteLine(ex.ToString())
            End Try

        Else
            'ERROR
            Console.WriteLine(response.StatusCode)
            Console.WriteLine(response.ReasonPhrase)
        End If

    End Sub
    'EXCHANGES
    Private Sub getuserexchanges()
        If _coinigyclient Is Nothing OrElse ExchangeItemList Is Nothing Then Return

        'DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
        'DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Updating exchange information...")

        Try

            Console.WriteLine()

            'EXCHANGE LIST
            'ExchangeItemList = New List(Of ExchangeItem)
            Try
                'ADD EXCHANGE DATASET
                Dim exchangedata As DataSet = ExchangeDataSet
                Dim tablename As String = "ExchangeDataTable"

                'ADD CHANNEL TABLES
                Dim table As New DataTable()
                table = New DataTable(tablename)
                exchangedata.Tables.Add(table)

                Dim col As New DataColumn
                col = New DataColumn : col.ColumnName = "Id"
                col.Caption = "Id"
                col.AutoIncrement = True
                col.AutoIncrementSeed = 0
                col.AutoIncrementStep = 1
                table.Columns.Add(col)
                col.Dispose() : col = Nothing

                Dim exch As New ExchangeItem
                For Each [property] As PropertyInfo In exch.[GetType]().GetProperties()
                    Dim valuename As String = [property].Name.ToLower
                    Dim valuetype As Type = [property].PropertyType
                    table.Columns.Add(New DataColumn(valuename, valuetype))
                Next
                For Each exch In ExchangeItemList
                    Dim newRow As DataRow = table.NewRow()
                    newRow.BeginEdit()
                    newRow("ExchId") = exch.ExchId
                    newRow("ExchCode") = exch.ExchCode
                    newRow("ExchName") = exch.ExchName
                    newRow("ExchFee") = exch.ExchFee
                    newRow("ExchBalanceEnabled") = exch.ExchBalanceEnabled
                    newRow("ExchTradeEnabled") = exch.ExchTradeEnabled
                    newRow("ExchUrl") = exch.ExchUrl
                    newRow.EndEdit()
                    table.Rows.Add(newRow)
                    newRow.AcceptChanges()
                Next
                table.AcceptChanges()

                Console.WriteLine()
                'AccountWalletGridControl.DataMember = "WalletDataTable"
                'AccountWalletGridControl.DataSource = table 'walletdata
                'AccountWalletGridControl.RefreshDataSource()

                'Return dt
            Catch ex As Exception
                Console.WriteLine(ex.ToString())
            End Try

            Console.WriteLine()





        Catch ex As Exception
            MsgBox("Exchange list update failed." & vbCrLf & ex.Message, MsgBoxStyle.OkCancel, Me.Text)
        Finally
            'DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        End Try

    End Sub

    'FETCH AND CREATE USER CHANNELS
    Private Sub getuserchannels()
        'EXCHFEE
        'TODO get exchange fee

        'CREATE SAMPLE DATA
        Dim UserChannelList As List(Of String) = New List(Of String)
        UserChannelList.Add("ORDER-BINA--BTC--USDT")
        'UserChannelList.Add("TRADE-BINA--BTC--USDT")
        UserChannelList.Add("ORDER-BMEX--XBT--USDT")
        'UserChannelList.Add("TRADE-BMEX--XBT--USDT")
        UserChannelList.Add("ORDER-BIFU--BTC--USDT")
        UserChannelList.Add("ORDER-BTRX--BTC--USDT")

        'CHANNELS
        Dim tablename As String = "UserChannelDataTable"
        Dim table As DataTable = New DataTable(tablename)
        table = BaseDataSet.Tables("UserChannelDataTable")
        For Each chan As String In UserChannelList
            Dim value As String = chan.Replace("--", "-")
            Dim chans As String() = value.Split(New Char() {"-"c})
            Dim newRow As DataRow = table.NewRow()
            newRow.BeginEdit()
            newRow("UserChanName") = chan
            If chans.Length = 4 Then
                newRow("UserChanMethod") = chans(0)
                newRow("UserChanExch") = chans(1)
                newRow("UserChanPri") = chans(2)
                newRow("UserChanSec") = chans(3)
                newRow("UserChanFee") = 0.025
            End If
            'newRow("UserChanVal") = chan
            newRow.EndEdit()
            table.Rows.Add(newRow)
            newRow.AcceptChanges()
        Next
        table.AcceptChanges()

        'CHANNELDATASET CREATE CHANNEL BUY SELL TABLEs - mem
        Dim tabletype As String() = {"BUY", "SELL"}
        Dim tabletypes As New List(Of String)(tabletype)
        For Each row As DataRow In BaseDataSet.Tables("UserChannelDataTable").Rows
            tablename = CStr(row("UserChanName"))

            For Each _type As String In tabletypes
                Dim _tablename As String = tablename & "--" & _type

                'ADD CHANNEL TABLES
                table = New DataTable()
                table = New DataTable(_tablename)
                ChannelDataSet.Tables.Add(table)

                Dim col As New DataColumn
                col = New DataColumn : col.ColumnName = "Id"
                col.Caption = "Id"
                col.AutoIncrement = True
                col.AutoIncrementSeed = 0
                col.AutoIncrementStep = 1
                table.Columns.Add(col)
                col.Dispose() : col = Nothing

                table.Columns.Add(New DataColumn("Price", Type.GetType("System.Decimal")))
                table.Columns.Add(New DataColumn("OrderType", Type.GetType("System.String")))
                table.Columns.Add(New DataColumn("Quantity", Type.GetType("System.Decimal")))
                table.Columns.Add(New DataColumn("Total", Type.GetType("System.Decimal")))

                'ADD 20 ROWS
                For i As Integer = 0 To 19
                    Dim newRow As DataRow = table.NewRow()
                    newRow.BeginEdit()
                    newRow("price") = 0.00
                    'newRow("OrderType") = "BUY"
                    'newRow("OrderType") = "SELL"
                    newRow("Quantity") = 0.00
                    newRow("Total") = 0.00
                    newRow.EndEdit()
                    table.Rows.Add(newRow)
                    newRow.AcceptChanges()
                Next
                Console.WriteLine()

                'TODO Select user table to display in grid
                If _tablename = "ORDER-BINA--BTC--USDT--BUY" Then
                    bidGridControl.DataMember = _tablename
                    bidGridControl.DataSource = ChannelDataSet 'table 'walletdata
                    bidGridControl.RefreshDataSource()

                    bidGridView.Columns("Id").SortOrder = (DevExpress.Data.ColumnSortOrder.Ascending)

                    bidGridView.Columns("Price").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                    bidGridView.Columns("Price").DisplayFormat.FormatString = "###,###,##0.#0"

                    bidGridView.Columns("Quantity").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                    bidGridView.Columns("Quantity").DisplayFormat.FormatString = "###,###,##0.#######0"

                    bidGridView.Columns("Total").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                    bidGridView.Columns("Total").DisplayFormat.FormatString = "###,###,##0.#######0"

                    'bidGridView.MoveLast()

                End If
                If _tablename = "ORDER-BINA--BTC--USDT--SELL" Then
                    askGridControl.DataMember = _tablename
                    askGridControl.DataSource = ChannelDataSet
                    askGridControl.RefreshDataSource()

                    askGridView.Columns("Id").SortOrder = (DevExpress.Data.ColumnSortOrder.Descending)

                    askGridView.Columns("Price").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                    askGridView.Columns("Price").DisplayFormat.FormatString = "###,###,##0.#0"

                    askGridView.Columns("Quantity").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                    askGridView.Columns("Quantity").DisplayFormat.FormatString = "###,###,##0.#######0"

                    askGridView.Columns("Total").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                    askGridView.Columns("Total").DisplayFormat.FormatString = "###,###,##0.#######0"

                    'askGridView.MoveFirst()

                End If
            Next

            'ADD SPREAD VARIANCEL TABLE
            If Not ChannelDataSet.Tables.Contains("VarianceDataTable") Then
                Dim _tablename As String = "VarianceDataTable"
                table = New DataTable()
                table = New DataTable(_tablename)
                Dim col As New DataColumn
                col = New DataColumn : col.ColumnName = "v_id"
                col.Caption = "Id"
                col.AutoIncrement = True
                col.AutoIncrementSeed = 0
                col.AutoIncrementStep = 1
                table.Columns.Add(col)
                col.Dispose() : col = Nothing
                table.Columns.Add(New DataColumn("v_channelName", Type.GetType("System.String")))
                table.Columns.Add(New DataColumn("v_maxPrice", Type.GetType("System.Decimal")))
                table.Columns.Add(New DataColumn("v_minPrice", Type.GetType("System.Decimal")))
                table.Columns.Add(New DataColumn("v_quantity", Type.GetType("System.Decimal")))
                table.Columns.Add(New DataColumn("v_variance", Type.GetType("System.Decimal")))
                table.Columns.Add(New DataColumn("v_timestamp", Type.GetType("System.DateTime")))

                ChannelDataSet.Tables.Add(table)

                LocalTradingPairsGridControl.DataMember = _tablename
                LocalTradingPairsGridControl.DataSource = ChannelDataSet

                LocalTradingPairsGridView.Columns("v_maxPrice").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                LocalTradingPairsGridView.Columns("v_maxPrice").DisplayFormat.FormatString = "###,###,##0.#0"

                LocalTradingPairsGridView.Columns("v_minPrice").DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric
                LocalTradingPairsGridView.Columns("v_minPrice").DisplayFormat.FormatString = "###,###,##0.#0"


            End If
        Next
        Console.WriteLine(ChannelDataSet.Tables.Count)

        'Where(function(x), x.IsDbNull = false).Max.
        '        If Not IsDBNull(row("minprice")) Then
        '    Dim testmin = table.AsEnumerable().Min(Function(row) row("minprice"))
        '    Dim result As DataRow() = table.[Select]("minprice =  " & testmin).IndexOf

        '    table.AsEnumerable().Min(Function(row) row("minprice"))

    End Sub

    'CONVERT LIST TO TABLE
    Public Shared Function ConvertToDataTable(Of T)(ByVal list As IList(Of T)) As DataTable
        Dim table As New DataTable()
        Dim fields() As FieldInfo = GetType(T).GetFields()
        For Each field As FieldInfo In fields
            table.Columns.Add(field.Name, field.FieldType)
        Next
        For Each item As T In list
            Dim row As DataRow = table.NewRow()
            For Each field As FieldInfo In fields
                row(field.Name) = field.GetValue(item)
            Next
            table.Rows.Add(row)
        Next
        Return table
    End Function

    '#SELECTED TRADING CHANNEL DATA
    Private LastLogWrite As DateTime = DateTime.Now

    Private Sub getorderdata(ByVal tablename As String, ByVal orderslist As OrderResponse)
        Dim askMin As Decimal = 0
        Dim bidMax As Decimal = 0

        'SORT
        Dim OrderItm() As OrderItem = orderslist.OrderData.Orders
        OrderItemsBuy = OrderItemsBuy.OrderBy(Function(x) x.Price).ThenBy(Function(x) x.Price).ToList()
        OrderItemsSell = OrderItemsSell.OrderBy(Function(x) x.Price).ThenByDescending(Function(x) x.Price).ToList()

        'UPDATE ARBITRAGE TABLE - mem
        Dim table As DataTable = ChannelDataSet.Tables(tablename)
        'UPDATE CHANNEL ORDERS - BIND TABLE
        If tablename.Contains("BUY") Then
            Dim i As Integer = 0
            For Each _orderitem As OrderItem In OrderItemsBuy
                Dim row As DataRow = table.Rows(i)
                row.BeginEdit()
                row("Ordertype") = _orderitem.Ordertype
                row("Price") = _orderitem.Price
                row("Quantity") = _orderitem.Quantity
                row("Total") = _orderitem.Total
                row.EndEdit()
                row.AcceptChanges()
                i += 1
            Next
            table.AcceptChanges()
            'TODO
            If tablename = "ORDER-BINA--BTC--USDT--BUY" Then
                bidGridControl.RefreshDataSource()
            End If

            'Sell Highest Buy in Buyorders 
            askMin = OrderItemsSell.Min(Function(x) x.Price)
            Dim qtyBuyMax As Double = Convert.ToDecimal(OrderItemsSell.Max(Function(x) x.Quantity))
            Dim minBuyPrice As Double = Convert.ToDecimal(OrderItemsSell.Min(Function(x) x.Price))
            'Dim minPrice As Double = Convert.ToDecimal(OrderItemsSell.Min(Function(x) x.Price))

            'ADD SPREAD VARIANCEL TABLE
            If ChannelDataSet.Tables.Contains("VarianceDataTable") Then
                'ADD SPREAD VARIANCEL TABLE
                Dim _tablename As String = "VarianceDataTable"
                table = ChannelDataSet.Tables(_tablename)
                Dim vartable As String = tablename.Replace("--BUY", "")
                Dim row As DataRow = table.Select("v_channelName = '" & vartable & "'").FirstOrDefault()
                If row Is Nothing Then
                    Dim newRow As DataRow = table.NewRow()
                    newRow.BeginEdit()
                    newRow("v_channelName") = vartable
                    newRow("v_maxPrice") = askMin 'bidMax
                    newRow("v_minPrice") = minBuyPrice
                    newRow("v_quantity") = qtyBuyMax
                    newRow("v_variance") = 0
                    newRow("v_timestamp") = DateTime.Now
                    newRow.EndEdit()
                    table.Rows.Add(newRow)
                    newRow.AcceptChanges()
                    LocalTradingPairsGridView.RefreshData()
                Else
                    Dim newRow As DataRow = row
                    newRow.BeginEdit()
                    'newRow("v_channelName") = tablename
                    newRow("v_maxPrice") = askMin 'bidMax
                    newRow("v_minPrice") = minBuyPrice
                    newRow("v_quantity") = qtyBuyMax
                    newRow("v_variance") = 0
                    newRow("v_timestamp") = DateTime.Now
                    newRow.EndEdit()
                    newRow.AcceptChanges()
                    table.AcceptChanges()
                    LocalTradingPairsGridView.RefreshData()
                End If
            End If
        End If

        If tablename.Contains("SELL") Then
            Dim i As Integer = 0
            For Each _orderitem As OrderItem In OrderItemsSell
                Dim row As DataRow = table.Rows(i)
                row.BeginEdit()
                row("Ordertype") = _orderitem.Ordertype
                row("Price") = _orderitem.Price
                row("Quantity") = _orderitem.Quantity
                row("Total") = _orderitem.Total
                row.EndEdit()
                row.AcceptChanges()
                i += 1
            Next
            table.AcceptChanges()
            'TODO
            If tablename = "ORDER-BINA--BTC--USDT--SELL" Then
                askGridControl.RefreshDataSource()
            End If

            'Buy Lowest Sell in Sellorders 
            bidMax = OrderItemsBuy.Max(Function(x) x.Price)
            Dim qtySellMax As Double = Convert.ToDecimal(OrderItemsSell.Max(Function(x) x.Quantity))
            Dim maxSellPrice As Double = Convert.ToDecimal(OrderItemsSell.Max(Function(x) x.Price))

            'ADD SPREAD VARIANCEL TABLE
            If ChannelDataSet.Tables.Contains("VarianceDataTable") Then
                'ADD SPREAD VARIANCEL TABLE
                Dim _tablename As String = "VarianceDataTable"
                table = ChannelDataSet.Tables(_tablename)
                Dim vartable As String = tablename.Replace("--SELL", "")
                Dim row As DataRow = table.Select("v_channelName = '" & vartable & "'").FirstOrDefault()
                If row Is Nothing Then
                    Dim newRow As DataRow = table.NewRow()
                    newRow.BeginEdit()
                    newRow("v_channelName") = vartable
                    newRow("v_maxPrice") = maxSellPrice
                    newRow("v_minPrice") = bidMax 'askMin
                    newRow("v_quantity") = qtySellMax
                    newRow("v_variance") = 0
                    newRow("v_timestamp") = DateTime.Now
                    newRow.EndEdit()
                    table.Rows.Add(newRow)
                    newRow.AcceptChanges()
                    LocalTradingPairsGridView.RefreshData()
                Else
                    Dim newRow As DataRow = row
                    newRow.BeginEdit()
                    'newRow("v_channelName") = tablename
                    newRow("v_maxPrice") = maxSellPrice
                    newRow("v_minPrice") = bidMax 'askMin
                    newRow("v_quantity") = qtySellMax
                    newRow("v_variance") = 0
                    newRow("v_timestamp") = DateTime.Now
                    newRow.EndEdit()
                    newRow.AcceptChanges()
                    table.AcceptChanges()
                    LocalTradingPairsGridView.RefreshData()
                End If
            End If

        End If

        'ADD SPREAD VARIANCE TABLE
        If ChannelDataSet.Tables.Contains("VarianceDataTable") Then
            '    'ADD SPREAD VARIANCEL TABLE
            Dim _tablename As String = "VarianceDataTable"
            '    'table = New DataTable()
            '    'table = New DataTable(_tablename)
            table = ChannelDataSet.Tables(_tablename)

            Try


                'Sell Highest Buy in Buyorders 
                Dim minprice As Decimal = CDec(table.AsEnumerable().Min(Function(row) row("v_maxPrice")))
                'Buy Lowest Sell in Sellorders 
                Dim maxprice As Decimal = CDec(table.AsEnumerable().Max(Function(row) row("v_minPrice")))
                Dim qty As Decimal = CDec(table.AsEnumerable().Max(Function(row) row("v_quantity")))

                'If (Not String.IsNullOrEmpty("value")) 

                'If Not IsDBNull(table.AsEnumerable().Min(Function(row) row("minprice"))) Then
                '    minprice = table.AsEnumerable().Min(Function(row) row("minprice"))
                'Else
                '    minpricemin = 0
                'End If

                'Dim testmin = table.AsEnumerable().Min(Function(row) row("minprice"))


                'SPREAD PERCENTAGE
                Dim spread As Decimal = 0
                Dim percentage As Decimal = 0
                'If isnotnullorzero(lastask) And isnotnullorzero(lastbid) Then
                spread = askMin - bidMax
                percentage = CalculateChange(minprice, maxprice)
                percentage = (Math.Round(percentage, 4) * 100)
                '#Percentage Color
                If percentage <= 0.045 Then
                    spreadLabel.ForeColor = Color.Red
                ElseIf percentage > 0.045 Then
                    spreadLabel.ForeColor = colorgreen
                    'logwrite
                    Dim logwritetime As DateTime = DateTime.Now

                    Dim ellapsedtime As Long = DateDiff(DateInterval.Second, LastLogWrite, logwritetime)
                    If ellapsedtime > 3 Then
                        'WRITE STATISTICS TABLE - VARIANCE LOG
                        Dim logtable As DataTable = BaseDataSet.Tables("AnalyticsDataTable")
                        Dim newRow As DataRow = logtable.NewRow()
                        newRow("a_channelName") = tablename
                        newRow("a_maxPrice") = maxprice
                        newRow("a_minPrice") = minprice
                        newRow("a_quantity") = qty
                        newRow("a_variance") = percentage
                        newRow("a_dateTime") = logwritetime
                        LastLogWrite = logwritetime
                        logtable.Rows.Add(newRow)
                        logtable.AcceptChanges()
                        'logtable.Dispose()

                        Dim key As String = Convert.ToDateTime(logwritetime).ToString("HH.mm.ss") + " (" & Convert.ToDouble(percentage).ToString("0.00") + "%)"
                        series1.Points.Add(New SeriesPoint(key, Convert.ToDouble(minprice), Convert.ToDouble(maxprice)))

                        'get the min value in the database
                        Dim minVal As Double = logtable.[Select]("").Min(Function(e) CType(e.ItemArray(3), Double))
                        'get the max value in the database
                        Dim maxVal As Double = logtable.[Select]("").Max(Function(e) CType(e.ItemArray(3), Double))
                        If Not IsNothing(rangeAreaChart.Diagram) Then
                            DirectCast(rangeAreaChart.Diagram, XYDiagram).AxisY.WholeRange.MinValue = minVal - 40
                            DirectCast(rangeAreaChart.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = maxVal + 40
                        End If

                        If Not IsDBNull(qty) Then
                            key = Convert.ToDateTime(logwritetime).ToString("HH.mm.ss") + " (" & Convert.ToDouble(qty).ToString("0.00") + ")"
                            'add a line point
                            Dim sp As New SeriesPoint(key, Convert.ToDouble(qty))
                            If percentage > 0.06 Then
                                sp.Color = Color.Green
                            End If

                            ariaSeries1.Points.Add(sp)
                        End If
                    End If
                Else
                    spreadLabel.ForeColor = colorspread

                End If
                'spreadLabel.Text = "Spread " & percentage & "%" '& "  " & spread
                spreadLabel.Text = " " & percentage & "% "
                'Me.Text = row("price").ToString & " " & pair & " " & exchange.ToProperCase

            Catch ex As Exception

            End Try

            'SPREAD TABLE
        End If
    End Sub

    'TRADING PAIRS
    Private Sub gettradingpairs()
        Dim ENDPOINT = "/api/v2/private/markets"
        Dim result As String = String.Empty
        Dim response As HttpResponseMessage = PostRequestClientAsync(ENDPOINT)
        If response.IsSuccessStatusCode = True Then
            result = response.Content.ReadAsStringAsync().Result
            Dim tradingpairs As tradingpair_response = JsonConvert.DeserializeObject(Of tradingpair_response)(result)
            Try
                'ADD ORDERBOOK DATASET
                Dim exchangedata As DataSet = ExchangeDataSet
                Dim tablename As String = "TradingPairsDataTable"
                'ADD CHANNEL TABLES
                Dim table As New DataTable()
                table = New DataTable(tablename)
                exchangedata.Tables.Add(table)
                Dim col As New DataColumn : col = New DataColumn : col.ColumnName = "Id" : col.Caption = "t_Id" : col.AutoIncrement = True : col.AutoIncrementSeed = 0 : col.AutoIncrementStep = 1
                table.Columns.Add(col) : col.Dispose() : col = Nothing
                Dim _tradingpair As New tradingpair
                Dim value As Object
                Dim T As Type = _tradingpair.GetType
                For Each fi As System.Reflection.FieldInfo In T.GetFields
                    Dim valuenam As String = fi.Name
                    value = fi.GetValue(_tradingpair)
                    Dim valuetyp As Type = fi.FieldType
                    table.Columns.Add(New DataColumn(valuenam, valuetyp))
                Next
                For Each _pair As tradingpair In tradingpairs.result
                    Dim newRow As DataRow = table.NewRow()
                    newRow.BeginEdit()
                    newRow("marketId") = _pair.marketId
                    newRow("marketName") = _pair.marketName
                    newRow("exchId") = _pair.exchId
                    newRow("exchCode") = _pair.exchCode
                    newRow("exchName") = _pair.exchName
                    newRow("baseCurrCode") = _pair.baseCurrCode
                    newRow("baseCurrId") = _pair.baseCurrId

                    newRow("quoteCurrCode") = _pair.quoteCurrCode
                    newRow("quoteCurrId") = _pair.quoteCurrId
                    newRow("baseCurrName") = _pair.baseCurrName
                    newRow("quoteCurrName") = _pair.quoteCurrName

                    newRow("exchmktId") = _pair.exchmktId
                    newRow("displayName") = _pair.displayName
                    newRow("baseIsFiat") = _pair.baseIsFiat
                    newRow("quoteIsFiat") = _pair.quoteIsFiat
                    newRow("basePricePrecision") = _pair.basePricePrecision

                    newRow("baseQuantityPrecision") = _pair.baseQuantityPrecision
                    newRow("quoteQuantityPrecision") = _pair.quoteQuantityPrecision
                    newRow("primaryCurrCode") = _pair.primaryCurrCode
                    newRow("secondaryCurrCode") = _pair.secondaryCurrCode

                    newRow("baseHasImage") = _pair.baseHasImage
                    newRow("quoteHasImage") = _pair.quoteHasImage
                    newRow("baseImageName") = _pair.baseImageName
                    newRow("quoteImageName") = _pair.quoteImageName


                    newRow.EndEdit()
                    table.Rows.Add(newRow)
                    newRow.AcceptChanges()
                Next
                table.AcceptChanges()

                TradingPairGridControl.DataMember = "TradingPairsDataTable"
                TradingPairGridControl.DataSource = table 'walletdata
                TradingPairGridControl.RefreshDataSource()

                'Return dt
            Catch ex As Exception
                Console.WriteLine(ex.ToString())
            End Try
        Else
            'ERROR
            Console.WriteLine(response.StatusCode)
            Console.WriteLine(response.ReasonPhrase)
        End If
    End Sub

    'TRADE ORDER
    Private Sub postsignalSimpleButton_Click(sender As Object, e As EventArgs) Handles postsignalSimpleButton.Click
        postsignalSimpleButton.Enabled = False
        createtradeorder()
        postsignalSimpleButton.Enabled = True
    End Sub

    '/api/v2/private/user/orders/addorder
    Private Sub createtradeorder()
        Dim response As HttpResponseMessage = New HttpResponseMessage
        Try

            Dim ENDPOINT = "/api/v2/private/user/orders/addorder"
            'Dim ENDPOINT = "/api/v2/private/user/orders/addorders"
            'Dim ENDPOINT As String = EndpointUrl

            Dim X_API_KEY As String
            X_API_KEY = "8d3e1cbe41f747e2ad5d4b9510ecd55f"

            Dim SECRET As String
            SECRET = "12db3e29366b490e8737b53ef7763afc"

            Dim METHOD As String
            METHOD = "GET"

            Dim _datetime As DateTime = DateTime.Now
            _datetime = DateAdd(DateInterval.Hour, -2, _datetime)
            Dim UnixTime As Long = unixtimeext.UnixTimeFromDateTimeUTC(_datetime)

            '#not used
            'PARAMS.Add("StartDate", "2019-01-01T00,00,00.000Z")
            'PARAMS.Add("EndDate", "2019-01-02T00:00:00.000Z")

            Dim PARAMS As New Dictionary(Of String, String)
            PARAMS.Add("auth_id", "1234")
            PARAMS.Add("exch_id", "70")
            PARAMS.Add("mkt_id", "6160")
            PARAMS.Add("order_type_id", "2")
            PARAMS.Add("price_type_id", "3")
            PARAMS.Add("limit_price", "0.99")
            PARAMS.Add("order_quantity", "57")

            Dim BODY As String
            BODY = ""

            X_API_TIMESTAMP = CStr(CInt(UnixTime))

            'URL PARSE
            Dim query_string As String = String.Empty '"?StartDate=" & "'2019-01-01T00:00:00.000Z'" & "EndDate=" & "2019-01-02T00:00:00.000Z"
            Dim query_result = (String.Format("{0}/api/v2/private/user/orders/addorder?", BASE_URL), UrlEncode(PARAMS))
            query_string = query_result.Item2
            query_string = "?" & query_string

            '/api/v2/private/user/orders/addorder?auth_id=1134&exch_id=70&mkt_id=6160&order_type_id=2&price_type_id=3&limit_price=0.99&order_quantity=50

            'query_string = ""
            'msg = X_API_KEY & METHOD & ENDPOINT '& "?" & query_string & BODY

            'Dim msg As String = X_API_KEY & X_API_TIMESTAMP & METHOD & ENDPOINT & query_string
            Dim msg As String = X_API_KEY & X_API_TIMESTAMP & METHOD & ENDPOINT & query_string

            Dim X_API_SIGN As String = CalculateSignature(msg, SECRET)
            'Dim X_API_SIGN As String = String.Empty
            'SIGNATURE
            'Using hmacsha256 = New System.Security.Cryptography.HMACSHA256(System.Text.Encoding.ASCII.GetBytes(SECRET))
            '    hmacsha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(msg))
            '    String.Concat(hmacsha256.Hash.[Select](Function(b) b.ToString("x2")).ToArray()).ToUpper()
            'End Using

            Dim postdata As New List(Of KeyValuePair(Of String, String))
            Dim client = New HttpClient()
            client.DefaultRequestHeaders.Add("Accept", "application/json")
            client.DefaultRequestHeaders.Add("Content", "application/json")
            client.DefaultRequestHeaders.Add("X-API-SIGN", X_API_SIGN)
            client.DefaultRequestHeaders.Add("X-API-TIMESTAMP", X_API_TIMESTAMP)
            client.DefaultRequestHeaders.Add("X-API-KEY", X_API_KEY)

            Dim content = New FormUrlEncodedContent(postdata)

            Server_Url = (BASE_URL + ENDPOINT) & query_string & "&Timestamp=" & X_API_TIMESTAMP
            response = client.GetAsync(Server_Url).Result

            'If (response.IsSuccessStatusCode, response.Content.ReadAsStringAsync().Result, "ERROR:" & response.StatusCode & " " + response.ReasonPhrase & " | " + response.RequestMessage) Then
            Dim r1 = response.IsSuccessStatusCode
            Dim r2 = response.Content.ReadAsStringAsync().Result
            Dim r3 = response.StatusCode
            Dim r4 = response.RequestMessage
            'End Function
            Console.WriteLine()
        Catch ex As Exception
            MsgBox("Error: PostRequestClientAsync: " & vbCrLf & ex.Message)
        End Try

        Console.WriteLine()

        'Return response



    End Sub

    'GETTRADEORDERS - ACTIVE ORDERS
    Private Sub gettradeorders()
        Dim response As HttpResponseMessage = New HttpResponseMessage
        Try

            Dim ENDPOINT = "/api/v2/private/user/orders"
            'Dim ENDPOINT = "/api/v2/private/user/orders/addorders"
            'Dim ENDPOINT As String = EndpointUrl

            Dim X_API_KEY As String
            X_API_KEY = "8d3e1cbe41f747e2ad5d4b9510ecd55f"

            Dim SECRET As String
            SECRET = "12db3e29366b490e8737b53ef7763afc"

            Dim METHOD As String
            METHOD = "GET"

            Dim _datetime As DateTime = DateTime.Now
            _datetime = DateAdd(DateInterval.Hour, -2, _datetime)
            Dim UnixTime As Long = unixtimeext.UnixTimeFromDateTimeUTC(_datetime)

            '#not used
            'PARAMS.Add("StartDate", "2019-01-01T00,00,00.000Z")
            'PARAMS.Add("EndDate", "2019-01-02T00:00:00.000Z")

            'Dim PARAMS As New Dictionary(Of String, String)
            'PARAMS.Add("auth_id", "1234")
            'PARAMS.Add("exch_id", "70")
            'PARAMS.Add("mkt_id", "6160")
            'PARAMS.Add("order_type_id", "2")
            'PARAMS.Add("price_type_id", "3")
            'PARAMS.Add("limit_price", "0.99")
            'PARAMS.Add("order_quantity", "57")


            Dim BODY As String
            BODY = ""

            X_API_TIMESTAMP = CStr(CInt(UnixTime))

            'URL PARSE
            Dim query_string As String = String.Empty '"?StartDate=" & "'2019-01-01T00:00:00.000Z'" & "EndDate=" & "2019-01-02T00:00:00.000Z"
            'Dim query_result = (String.Format("{0}api/1.1/api/v2/private/user/orders/addorder", BASE_URL), UrlEncode(PARAMS))
            'query_string = query_result.Item2

            '/api/v2/private/user/orders/addorder?auth_id=1134&exch_id=70&mkt_id=6160&order_type_id=2&price_type_id=3&limit_price=0.99&order_quantity=50

            'query_string = ""
            'msg = X_API_KEY & METHOD & ENDPOINT '& "?" & query_string & BODY
            Dim msg As String = X_API_KEY & X_API_TIMESTAMP & METHOD & ENDPOINT '& "?" & query_string

            Dim X_API_SIGN As String = CalculateSignature(msg, SECRET)
            'Dim X_API_SIGN As String = String.Empty
            'SIGNATURE
            'Using hmacsha256 = New System.Security.Cryptography.HMACSHA256(System.Text.Encoding.ASCII.GetBytes(SECRET))
            '    hmacsha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(msg))
            '    String.Concat(hmacsha256.Hash.[Select](Function(b) b.ToString("x2")).ToArray()).ToUpper()
            'End Using

            Dim postdata As New List(Of KeyValuePair(Of String, String))
            Dim client = New HttpClient()
            client.DefaultRequestHeaders.Add("Accept", "application/json")
            client.DefaultRequestHeaders.Add("Content", "application/json")
            client.DefaultRequestHeaders.Add("X-API-SIGN", X_API_SIGN)
            client.DefaultRequestHeaders.Add("X-API-TIMESTAMP", X_API_TIMESTAMP)
            client.DefaultRequestHeaders.Add("X-API-KEY", X_API_KEY)

            Dim content = New FormUrlEncodedContent(postdata)

            Server_Url = (BASE_URL + ENDPOINT) & "?Timestamp=" & X_API_TIMESTAMP '& query_string
            response = client.GetAsync(Server_Url).Result

            'If (response.IsSuccessStatusCode, response.Content.ReadAsStringAsync().Result, "ERROR:" & response.StatusCode & " " + response.ReasonPhrase & " | " + response.RequestMessage) Then
            Dim r1 = response.IsSuccessStatusCode
            Dim r2 = response.Content.ReadAsStringAsync().Result
            Dim r3 = response.StatusCode
            Dim r4 = response.RequestMessage
            'End Function
            Console.WriteLine()
        Catch ex As Exception
            MsgBox("Error: PostRequestClientAsync: " & vbCrLf & ex.Message)
        End Try

        Console.WriteLine()

        'Return response



    End Sub


#End Region '#ONFUNCTION

#End Region '##ONCLIENT

#Region "#TECHNICAL ANALYTICS - CHART INDICATORS"

    'CREATE QUANTITY VOLUME CHARTS
    Private Sub createTotalsChart()
        'get the data
        Dim DT As DataTable = BaseDataSet.Tables("AnalyticsDataTable")
        Dim minVal, maxVal As Double
        'get the min value in the database
        If (DT.Rows.Count > 0) Then
            minVal = DT.[Select]("").Min(Function(e) CType(e.ItemArray(3), Double))
            'get the max value in the database
            maxVal = DT.[Select]("").Max(Function(e) CType(e.ItemArray(3), Double))
            For Each DR As DataRow In DT.Rows
                Dim key As String = Convert.ToDateTime(DR("a_dateTime")).ToString("HH.mm.ss") + " (" & Convert.ToDouble(DR("a_variance")).ToString("0.00") + "%)"
                Dim sp As New SeriesPoint(key, Convert.ToDouble(DR("a_quantity")))
                ariaSeries1.Points.Add(sp)
            Next
        End If
        ' Add a series to the chart.
        VolumeChart.Series.Add(ariaSeries1)
        ' Access the view-type-specific options of the series.
        DirectCast(ariaSeries1.View, AreaSeriesView).Transparency = 80
        ' Access the type-specific options of the diagram.
        DirectCast(VolumeChart.Diagram, XYDiagram).AxisX.GridLines.Visible = True
        If (DT.Rows.Count > 0) Then
            DirectCast(VolumeChart.Diagram, XYDiagram).AxisY.WholeRange.MinValue = minVal - 40
            DirectCast(VolumeChart.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = maxVal + 40
        End If
        DirectCast(VolumeChart.Diagram, XYDiagram).AxisX.Label.Angle = 270
        DirectCast(VolumeChart.Diagram, XYDiagram).EnableAxisXZooming = True
        DirectCast(VolumeChart.Diagram, XYDiagram).EnableAxisYZooming = True
        ' Hide the legend (if necessary).
        VolumeChart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False
        ' Add a title to the chart (if necessary).
        VolumeChart.Titles.Add(New ChartTitle())
        VolumeChart.Titles(0).Text = "Volume"
        VolumeChart.Titles(0).Font = New Font("Tahoma Sans MS", 12)
        'VolumeChart.Titles(0).Font = New Font("Tahoma Sans MS", 12, FontStyle.Bold Or FontStyle.Underline)
    End Sub

    'CREATE VARIANCE CHART
    Private Sub createVarianceChart()
        'get the data
        Dim DT As DataTable = BaseDataSet.Tables("AnalyticsDataTable")
        Dim minVal, maxVal As Double
        'get the min value in the database
        If (DT.Rows.Count > 0) Then
            minVal = DT.[Select]("").Min(Function(e) CType(e.ItemArray(3), Double))
            'get the max value in the database
            maxVal = DT.[Select]("").Max(Function(e) CType(e.ItemArray(3), Double))
            For Each DR As DataRow In DT.Rows
                Dim key As String = Convert.ToDateTime(DR("a_dateTime")).ToString("HH.mm.ss") + " (" & Convert.ToDouble(DR("a_variance")).ToString("0.00") + "%)"
                Dim sp As New SeriesPoint(key, Convert.ToDouble(DR("a_minPrice")), Convert.ToDouble(DR("a_maxPrice")))
                series1.Points.Add(sp)
            Next
        End If
        ' Add a series to the chart.
        rangeAreaChart.Series.Add(series1)
        ' Access the view-type-specific options of the series.
        DirectCast(series1.View, RangeAreaSeriesView).Transparency = 80
        ' Access the type-specific options of the diagram.
        DirectCast(rangeAreaChart.Diagram, XYDiagram).AxisX.GridLines.Visible = True
        If (DT.Rows.Count > 0) Then
            DirectCast(rangeAreaChart.Diagram, XYDiagram).AxisY.WholeRange.MinValue = minVal - 40
            DirectCast(rangeAreaChart.Diagram, XYDiagram).AxisY.WholeRange.MaxValue = maxVal + 40
        End If
        DirectCast(rangeAreaChart.Diagram, XYDiagram).AxisX.Label.Angle = 270
        DirectCast(rangeAreaChart.Diagram, XYDiagram).EnableAxisXZooming = True
        DirectCast(rangeAreaChart.Diagram, XYDiagram).EnableAxisYZooming = True
        ' Hide the legend (if necessary).
        rangeAreaChart.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False
        ' Add a title to the chart (if necessary).
        rangeAreaChart.Titles.Add(New ChartTitle())
        rangeAreaChart.Titles(0).Text = "Variance"
        rangeAreaChart.Titles(0).Font = New Font("Tahoma Sans MS", 12)
        'VolumeChart.Titles(0).Font = New Font("Tahoma Sans MS", 12, FontStyle.Bold Or FontStyle.Underline)
    End Sub

#End Region '#TECHNICAL ANALYTICS - CHART INDICATORS

#Region "#THREADING"

    '#AGGREGATED PRICE TICKER
    Private RaiseOrderBookAsyncThread As System.Threading.Timer
    Private RaiseOrderBookAsyncThreadCallback As New TimerCallback(AddressOf RaiseOrderBookAsync)

    'INIT THREADING
    Private Function GetExchangesAsync()

        RaiseOrderBookAsync("")


        '#RAISE AGREGATED ORDERBOOK - PRICE
        'RaiseOrderBookAsyncThread = New System.Threading.Timer(RaiseOrderBookAsyncThreadCallback, Nothing, Timeout.Infinite, Timeout.Infinite)
        '#RAISE SINGLE THREAD LOOP
        'RaiseOrderBookAsyncThread = New System.Threading.Timer(RaiseOrderBookAsyncThreadCallback, Nothing, 0, 100)

        'Await SubscribeToSpotUserStreamAsync()

        '#QUEUE THREADING TIMER - VOICE
        'Dim RaiseOrderBookAsyncThread As System.Threading.Thread
        'RaiseOrderBookAsyncThread = New System.Threading.Thread(AddressOf RaiseOrderBookAsync)
        'RaiseOrderBookAsyncThread.IsBackground = True

        'Dim params As MarketStream.BinanceStreamAggregatedTrade
        'params = Data

        'RaiseOrderBookAsyncThreadCallback(params) = New System.Threading.Timer(RaiseOrderBookAsyncThreadCallback, Nothing, 0, 100)

        'RaiseOrderBookAsync(params)
        'RaiseOrderBookAsyncThreadCallback(params) = New System.Threading.Timer(RaiseOrderBookAsyncThreadCallback, Nothing, 0, 0)
        'RaiseOrderBookAsyncThread.Start(params)


    End Function

    '#CREATEORDERBOOKTABLE

    Public Delegate Sub OrderBookDelegate(<[ParamArray]()> ByVal channelname As String) '<[ParamArray]()> ByVal params As List(Of OrderItem), ByVal channelname As String



    Private Sub RaiseOrderBookAsync(ByVal channelname As Object) 'As DataSet 'ByVal params As List(Of OrderItem), ByVal param As String
        If InvokeRequired Then
            'Invoke(New MethodInvoker(AddressOf CreateOrderBookTable))
            Me.Invoke(New OrderBookDelegate(AddressOf RaiseOrderBookAsync), New Object() {channelname})

            'Invoke(New OrderBookDelegate(AddressOf RaiseOrderBookAsync), New Object() {param}) 'New List(Of OrderItem)() {params}, New String() {param}
            'Invoke(New OrderBookDelegate(AddressOf CreateOrderBookTableAsync), channelname, New List(Of OrderItem)() {orderitems})

        Else

            '#QUEUE THREADING TIMER - VOICE
            Dim RaiseOrderBookAsyncThread As System.Threading.Thread
            RaiseOrderBookAsyncThread = New System.Threading.Thread(AddressOf RaiseOrderBookAsync)
            RaiseOrderBookAsyncThread.IsBackground = True

            'Dim params As MarketStream.BinanceStreamAggregatedTrade
            'params = Data

            'If unsubsockets Then socketClient.UnsubscribeAll()

            'EXCHANGE LIST
            ExchangeItemList = New List(Of ExchangeItem)
            Dim Exchanges As IEnumerable(Of ExchangeItem) = _coinigyclient.GetExchanges()
            ExchangeItemList = Exchanges.ToList()
            ExchangeGridControl.Invoke(New Action(Function()
                                                      ExchangeGridControl.DataSource = ExchangeItemList
                                                      ExchangeGridControl.RefreshDataSource()
                                                  End Function))


            'GET EXCHANGE TABLE
            'Write to table


            'EXCHANGE DATA

            Dim ExchangeTable As DataTable = New DataTable
            ExchangeTable = BaseDataSet.Tables("ExchangeDataTable") ': ExchangeTable.Clear()
            For Each _exchange As ExchangeItem In ExchangeItemList
                'Dim ExId As integer
                Dim ExchCode As String = _exchange.ExchCode
                Dim itemfound As Integer = ChannelBindingSource.Find("ExchCode", ExchCode)
                If itemfound > -1 Then
                    'row.BeginEdit()
                    'row("ExchName") = _exchange.ExchName
                    'row("ExchTradeEnabled") = _exchange.ExchTradeEnabled
                    'row("ExchBalanceEnabled") = _exchange.ExchBalanceEnabled
                    'row("ExchFee") = _exchange.ExchFee
                    'row("ExchUrl") = _exchange.ExchUrl
                    'row("ExEnabled") = _exchange.ExEnabled
                    'row("ExActive") = _exchange.ExActive
                    'row("ExDateTime") = _exchange.ExDateTime
                    'row.EndEdit()
                    'row.AcceptChanges()
                Else
                    Dim newRow As DataRow = ExchangeTable.NewRow()
                    newRow.BeginEdit()
                    newRow("ExchName") = _exchange.ExchName
                    newRow("ExchTradeEnabled") = _exchange.ExchTradeEnabled
                    newRow("ExchBalanceEnabled") = _exchange.ExchBalanceEnabled
                    newRow("ExchFee") = _exchange.ExchFee
                    newRow("ExchUrl") = _exchange.ExchUrl
                    'newRow("ExEnabled") = _exchange.ExEnabled
                    'newRow("ExActive") = _exchange.ExActive
                    'newRow("ExDateTime") = _exchange.ExDateTime
                    newRow.EndEdit()
                    newRow.AcceptChanges()
                    ExchangeTable.Rows.Add(newRow)
                End If



            Next

            'COMPARE TABLES

            'Private Sub RaiseAccountInfo(ByVal params As BinanceStreamAccountInfo)
            '    If Me.InvokeRequired Then
            '        Me.Invoke(New StreamAccountInfoDelegate(AddressOf RaiseAccountInfo), New BinanceStreamAccountInfo() {params})
            '    Else
            '        If params Is Nothing Then Return

            '        Dim balances As BinanceStreamBalance = params.Balances
            '        Dim dtAccount As DataTable = New DataTable
            '        dtAccount = BaseDataSet.Tables("accountTable") ': dtAsks.Clear()
            '        For Each row As DataRow In dtAccount.Rows
            '            Dim Asset As String = row("Asset")
            '            Dim itemfound As Integer = accountBindingSource.Find("Asset", Asset)
            '            If itemfound > -1 Then
            '                row.BeginEdit()
            '                row("Asset") = balances.Asset
            '                row("Free") = balances.Free
            '                row("Locked") = balances.Locked
            '                row("Total") = balances.Total
            '                row.EndEdit()
            '                row.AcceptChanges()
            '            Else
            '                Dim newRow As DataRow = dtAccount.NewRow()
            '                row.BeginEdit()
            '                newRow("Asset") = balances.Asset
            '                newRow("Free") = balances.Free
            '                newRow("Locked") = balances.Locked
            '                newRow("Total") = balances.Total
            '                row.EndEdit()
            '                dtAccount.Rows.Add(newRow)
            '            End If
            '        Next
            '        dtAccount.AcceptChanges()
            '        dtAccount.Dispose()
            '    End If
            'End Sub

#Region "INIT LISTOFPAIRS"

            ''#PAIR PARAMETERS
            'ListOfPairs = {} '{"BTCUSDT", "BTCETH", "DOTBTC", "LTCBTC", "XRPBTC"} 'baseparam.pair
            'Dim PairParams As New List(Of PairParametersExt)
            'PairParams = GetBaseParameters()

            ''#PRICE PARAMETERS
            'PriceParams = BuildDictionary(ListOfPairs)
            'dtSymbols = BaseDataSet.Tables("symbolsinfoTable") ': dtSymbols.Clear()

            ''#FEE - MAKER TAKER
            'For Each PairItem As String In ListOfPairs

            'Next

#End Region 'INIT LISTOFPAIRS


            'ADD ORDERBOOK DATASET
            Dim orderbookdata As DataSet = OrderBookDataSet
            Dim tablename As String = channelname.ToString

            'ADD CHANNEL TABLES
            Dim table As DataTable
            table = New DataTable(tablename)
            orderbookdata.Tables.Add(table)

            Dim col As DataColumn

            col = New DataColumn : col.ColumnName = "Id"
            col.Caption = "Id"
            col.AutoIncrement = True
            col.AutoIncrementSeed = 0
            col.AutoIncrementStep = 1
            table.Columns.Add(col)
            col.Dispose() : col = Nothing

            col = New DataColumn : col.ColumnName = "Price" : col.Caption = "Price"
            col.DataType = System.Type.GetType("System.Decimal") : table.Columns.Add(col) : col.Dispose() : col = Nothing
            col = New DataColumn : col.ColumnName = "OrderSide" : col.Caption = "OrderSide"
            col.DataType = System.Type.GetType("System.String") : table.Columns.Add(col) : col.Dispose() : col = Nothing
            col = New DataColumn : col.ColumnName = "Quantity" : col.Caption = "Quantity"
            col.DataType = System.Type.GetType("System.Decimal") : table.Columns.Add(col) : col.Dispose() : col = Nothing
            col = New DataColumn : col.ColumnName = "Total" : col.Caption = "Total"
            col.DataType = System.Type.GetType("System.Decimal") : table.Columns.Add(col) : col.Dispose() : col = Nothing
            col = New DataColumn : col.ColumnName = "ChannelName" : col.Caption = "ChannelName"
            col.DataType = System.Type.GetType("System.String") : table.Columns.Add(col) : col.Dispose() : col = Nothing
            col = New DataColumn : col.ColumnName = "Timestamp" : col.Caption = "Timestamp"
            col.DataType = System.Type.GetType("System.DateTime") : table.Columns.Add(col) : col.Dispose() : col = Nothing

            'Add 20 rows to each table
            For i As Integer = 0 To 19
                table.Rows.Add(i, 0.00, "orderside", 0.0, 0.0, channelname, DateTime.Now)

            Next


            'For i = 1 To 10
            '    row = dt.NewRow
            '    row.Item(1) = "ABC" & i
            '    row.Item(2) = "XYZ" & i
            '    dt.Rows.Add(row)
            'Next

            'Dim j As Integer
            ''Outer loop is used for the rows.
            'For i = 0 To dt.Rows.Count - 1
            '    'Innser loop is used for the columns.
            '    For j = 0 To dt.Columns.Count - 1
            '        System.Console.Write(dt.Rows(i).Item(j))
            '        System.Console.Write(ControlChars.Tab)
            '    Next
            '    System.Console.WriteLine()
            'Next

            'Return orderbookdata
        End If
    End Sub

#End Region '#THREADING

#Region "*obsolete*"

    'FOO
    'Foo()

    'm_Dictionary.Add(bidOrder, OrderItemsBuy)
    'If m_i > 10 Then
    '    Console.WriteLine()
    'End If
    'm_i += 1


#End Region '*obsolete*

#Region "#WALLET"

    'GETUSERBALANCES - WALLET

    'USER BALANCES
    Private Sub getuserbalances()
        Dim ENDPOINT = "/api/v2/private/user/balances"
        Dim result As String = String.Empty
        Dim response As HttpResponseMessage = PostRequestClientAsync(ENDPOINT)

        If response.IsSuccessStatusCode = True Then
            result = response.Content.ReadAsStringAsync().Result
            Dim walletbalances As balances_response = JsonConvert.DeserializeObject(Of balances_response)(result)
            Console.WriteLine(response.StatusCode)
            Try
                'ADD ORDERBOOK DATASET
                Dim walletdata As DataSet = WalletDataSet
                Dim tablename As String = "WalletDataTable"

                'ADD CHANNEL TABLES
                Dim table As New DataTable()
                table = New DataTable(tablename)
                walletdata.Tables.Add(table)

                Dim col As New DataColumn
                col = New DataColumn : col.ColumnName = "Id"
                col.Caption = "Id"
                col.AutoIncrement = True
                col.AutoIncrementSeed = 0
                col.AutoIncrementStep = 1
                table.Columns.Add(col)
                col.Dispose() : col = Nothing

                Dim walletbalance As New Balance
                Dim value As Object
                Dim T As Type = walletbalance.GetType
                For Each fi As System.Reflection.FieldInfo In T.GetFields
                    Dim valuename As String = fi.Name
                    value = fi.GetValue(walletbalance)
                    Dim valuetype As Type = fi.FieldType
                    table.Columns.Add(New DataColumn(valuename, valuetype))
                Next
                Console.WriteLine()
                For Each _balance As Balance In walletbalances.result
                    Dim newRow As DataRow = table.NewRow()
                    newRow.BeginEdit()
                    newRow("balanceAmountAvailable") = _balance.balanceAmountAvailable
                    newRow("balanceAmountTotal") = _balance.balanceAmountTotal
                    newRow.EndEdit()
                    table.Rows.Add(newRow)
                    newRow.AcceptChanges()
                Next
                table.AcceptChanges()

                'reflection
                'https://kellyschronicles.wordpress.com/2008/10/31/iterate-over-all-fields-and-properties-in-an-object-with-vb-net/

                AccountWalletGridControl.DataMember = "WalletDataTable"
                AccountWalletGridControl.DataSource = table 'walletdata
                AccountWalletGridControl.RefreshDataSource()

                'Return dt
            Catch ex As Exception
                Console.WriteLine(ex.ToString())
            End Try

        Else
            'ERROR
            Console.WriteLine(response.StatusCode)
            Console.WriteLine(response.ReasonPhrase)
        End If

    End Sub


    Private Sub RaiseCredentials()
        Try
            DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
            DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Initializing credentials...")

            '	_coinigyclient = New CoinigyWsClient(New CoinigyWebSocketClientOptions() With {
            '	.Creds = New Creds() With {
            '		.apiKey = api_key,
            '		.apiSecret = api_secret
            '	},
            '	.DebugMode = True
            '})
            '	_coinigyclient.Options.MyReconnectStrategy = New ReconnectStrategy(2000, 4000)

            '#CREDENTIALS

            _coinigyclient = New CoinigyWsClient(New CoinigyWebSocketClientOptions() With {
                                                                 .Creds = New Creds() With {
                                                                 .apiKey = api_key,
                                                                 .apiSecret = api_secret},
                                                                 .DebugMode = True,
                                                                 .MyReconnectStrategy = New ReconnectStrategy(2000, 4000)})

        Catch ex As Exception
            MsgBox("Initializing credentials failed." & vbCrLf & ex.Message, MsgBoxStyle.OkCancel, Me.Text)
        Finally
            DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
        End Try
    End Sub



    Private Async Sub WalletGridViewRefresh()
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf WalletGridViewRefresh))
        Else

#Region "#CREDENTIALS"

            RaiseCredentials()

#End Region '#CREDENTIALS

#Region "#ACCOUNT WALLET INIT"

            Try
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(GetType(xfwait))
                DevExpress.XtraSplashScreen.SplashScreenManager.Default.SetWaitFormDescription("Initializing account information...")

                '#ACCOUNT WALLET
                'Dim dtAccount As DataTable = New DataTable
                'dtAccount = BaseDataSet.Tables("accountTable") : dtAccount.Clear()

                'Dim exchangeinfoResult As CryptoExchange.Net.Objects.WebCallResult(Of BinanceExchangeInfo) = Await client.Spot.System.GetExchangeInfoAsync()

                Dim exchangeinfoResult = _coinigyclient.GetExchanges()

            Catch ex As Exception
                MsgBox("Initializing account information failed." & vbCrLf & ex.Message, MsgBoxStyle.OkCancel, Me.Text)
            Finally
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm()
            End Try

        End If

#End Region '#ACCOUNT INIT
    End Sub

    Private exchanges As New List(Of ExchangeItem)
    Public Function GetClient() As CoinigyWsClient
        '_coinigyclient = New CoinigyWsClient(New CoinigyWebSocketClientOptions() With {.Creds = New Creds() With {.apiKey = api_key, .apiSecret = api_secret}, .DebugMode = True})
        Return New CoinigyWsClient(New CoinigyWebSocketClientOptions() With {.Creds = New Creds() With {.apiKey = api_key, .apiSecret = api_secret}, .DebugMode = True})
    End Function

    Public Async Function Can_Get_AccountBalances() As Task
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf Can_Get_AccountBalances))
        Else
            Try
                'If threads.accounts = True Then
                '    Console.WriteLine("Crossthread Can_Get_AccountBalances" & Now)
                '    If Not ThreadingTimerAccountBalances Is Nothing Then ThreadingTimerAccountBalances.Change(3000, config.AccountBalance)
                '    Exit Function
                'End If
                'threads.accounts = True

                'apiwatch.WatcherStart("accountbalance")

                'Dim client = GetClient()
                Dim exchanges = GetvbExchanges()

                'Assert.NotNull(accountBalanceResponse)
                'Assert.NotEmpty(accountBalanceResponse.Balances)
                If Not exchanges Is Nothing Then
                    Dim dt As DataTable = New DataTable


                End If

            Catch ex As Exception
            End Try
        End If
    End Function

#End Region '#WALLET

#Region "#CLICK EVENT - BUTTONS"

    'Private ReadOnly _subscribeCallbacs As List(Of KeyValuePair(Of String, Message)) = New List(Of KeyValuePair(Of String, Message))()
    'Public Event OnClientReady As ClientIsReady
    'Public Event OnStateChanged As StateChanged
    'Public Event OnClosed As Closed
    'Public Event OnError As [Error]
    'Public Event OnFatality As Fatality
    'Public Event OnSendFailed As SendFailed
    'Public Event OnOrderMessage As OrderMessage
    'Public Event OnTradeMessage As TradeMessage
    '<Obsolete>
    'Public Event OnBlockMessage As BlockMessage
    'Public Event OnFavoriteMessage As FavoriteMessage
    'Public Event OnNewMarketMessage As NewMarketMessage
    'Public Event OnNotificationMessage As NotificationMessage
    'Public Event OnNewsMessage As NewsMessage
    'Public Event OnMessage As Message

    'Public Sub New(ByVal options As CoinigyWebSocketClientOptions, ByVal Optional url As String = "wss://sc-02.coinigy.com/socketcluster/")
    '    options = options
    '    _scClient = New PureSocketClusterSocket(url, options)
    '    '_scClient.OnOpened += _scClient_OnOpened
    '    '_scClient.OnMessage += AddressOf _scClient_OnMessage
    '    '_scClient.OnStateChanged += AddressOf _scClient_OnStateChanged
    '    '_scClient.OnClosed += AddressOf _scClient_OnClosed
    '    '_scClient.OnError += AddressOf _scClient_OnError
    '    '_scClient.OnFatality += AddressOf _scClient_OnFatality
    '    '_scClient.OnSendFailed += AddressOf _scClient_OnSendFailed
    'End Sub

    'Private Sub _scClient_OnSendFailed(ByVal data As String, ByVal ex As Exception)
    '    Return OnSendFailed?.Invoke(data, ex)
    'End Sub

    'Private Sub _scClient_OnFatality(ByVal reason As String)
    '    Return OnFatality?.Invoke(reason)
    'End Sub

    'Private Sub _scClient_OnError(ByVal ex As Exception)
    '    Return OnError?.Invoke(ex)
    'End Sub

    'Private Sub _scClient_OnClosed(ByVal reason As WebSocketCloseStatus)
    '    Return OnClosed?.Invoke(reason)
    'End Sub

    'Private Sub _scClient_OnStateChanged(ByVal newState As WebSocketState, ByVal prevState As WebSocketState)
    '    Return OnStateChanged?.Invoke(newState, prevState)
    'End Sub

    'Private Sub _scClient_OnMessage(ByVal message As String)


    'Private cclient As PureSocketClusterSocket

    'Public Sub New(ByVal options As CoinigyWebSocketClientOptions, ByVal Optional url As String = "wss://sc-02.coinigy.com/socketcluster/")
    '    options = options
    '    _scClient = New PureSocketClusterSocket(url, options)
    '    _scClient.OnOpened += _scClient_OnOpened
    '    _scClient.OnMessage += _scClient_OnMessage
    '    _scClient.OnStateChanged += _scClient_OnStateChanged
    '    _scClient.OnClosed += _scClient_OnClosed
    '    _scClient.OnError += _scClient_OnError
    '    _scClient.OnFatality += _scClient_OnFatality
    '    _scClient.OnSendFailed += _scClient_OnSendFailed
    'End Sub

    Public Function GetvbExchanges() As IEnumerable(Of ExchangeItem)

        'Dim url As String = "wss://sc-02.coinigy.com/socketcluster/"
        'Dim options As CoinigyWebSocketClientOptions
        'options = options

        ''Public Sub New(ByVal options As CoinigyWebSocketClientOptions, ByVal Optional url As String = "wss://sc-02.coinigy.com/socketcluster/")
        ''    options = options
        ''    _scClient = New PureSocketClusterSocket(url, options)
        ''    '_scClient.OnOpened += _scClient_OnOpened
        ''    '_scClient.OnMessage += AddressOf _scClient_OnMessage
        ''    '_scClient.OnStateChanged += AddressOf _scClient_OnStateChanged
        ''    '_scClient.OnClosed += AddressOf _scClient_OnClosed
        ''    '_scClient.OnError += AddressOf _scClient_OnError
        ''    '_scClient.OnFatality += AddressOf _scClient_OnFatality
        ''    '_scClient.OnSendFailed += AddressOf _scClient_OnSendFailed
        ''End Sub
        '_coinigyclient = New CoinigyWsClient(New CoinigyWebSocketClientOptions With {
        '    .Creds = New Creds With {
        '        .apiKey = api_key,
        '        .apiSecret = api_secret
        '    },
        '    .DebugMode = True
        ' })
        'options = _coinigyclient.Options

        ''Duplicate Auth Key
        ''We were unable To add this API key, because it matched the hash of an existing key within our system. Keys can be added only once. Please generate a New API key And try again.


        '_scClient = New PureSocketClusterSocket(url, options)

        'Dim connectionResult = _scClient.Connect()

        'If _scClient Is Nothing OrElse _scClient.SocketState <> WebSocketState.Open Then Return Nothing
        'Dim results = New List(Of ExchangeItem)()
        'Dim ackReceived = False
        '_scClient.Emit("exchanges", "", Function(name, [error], data)
        '                                    Dim res = CType(data, List(Of Data))

        '                                    'For Each r As Dictionary(Of String, Object) In res

        '                                    '    Try
        '                                    '        results.Add(New ExchangeItem With {
        '                                    '        .ExchCode = r("exch_code").ToString(),
        '                                    '        .ExchName = r("exch_name").ToString(),
        '                                    '        .ExchBalanceEnabled = Convert.ToInt32(r("exch_balance_enabled")) = 1,
        '                                    '        .ExchTradeEnabled = Convert.ToInt32(r("exch_trade_enabled")) = 1,
        '                                    '        .ExchFee = Convert.ToDecimal(CDbl(r("exch_fee"))),
        '                                    '        .ExchId = Convert.ToInt64(r("exch_id")),
        '                                    '        .ExchUrl = r("exch_url").ToString()
        '                                    '    })
        '                                    '    Catch
        '                                    '    End Try
        '                                    'Next

        '                                    ackReceived = True
        '                                End Function)
        'Dim st = DateTime.UtcNow

        'Do
        '    If ackReceived Then Exit Do
        'Loop While st.AddSeconds(20000) > DateTime.UtcNow

        'Return results
    End Function

    Private Async Sub runsub()

        '("TRADE-BMEX--XBT--USDT", AddressOf MyCustomCallback)

        '        Dim variables As New Dictionary(Of String, Integer)()
        '        variables("MyDynamicVariable") = 10  ' Set the value of the "variable"
        '        Dim value As Integer = variables("MyDynamicVariable")  ' Retrieve the value of the variable

        '        Dim Numbers As New List(Of Integer)

        '        For i As Integer = 0 To 9
        '            Numbers.Add(0)
        '        Next

        '        "Dim data() As String = {"box1", "box2", "box3"}"
        ''Here box1, box2, box3 are Not the names Of the variables, they are string 3 values of the string array data().
        ''You can declare it as
        'Dim data(2) As String
        '        data(0) = "box1"
        '        data(1) = "box2"
        '        data(2) = "box3"


        '        Dim Collection1 As Collection
        '        Collection1 = New Collection
        '        Collection1.Add(Class1, Key1)


        '        Dim ListOfNames = New List(Of String)

        '        ' Add 3 elements to the list.
        '        ListOfNames.Add("Alexandre")
        '        ListOfNames.Add("Maxime")
        '        ListOfNames.Add("Daniel")

        '        ListOfNames.Remove("Maxime")

        '        ListOfNames.Clear();


    End Sub

#End Region '#CLICK EVENT - BUTTONS





    Private Sub TestSimpleButton_Click(sender As Object, e As EventArgs) Handles TestSimpleButton.Click

        '#CREATE TABLE
        RaiseOrderBookAsync("chan")

        'getuserbalances()

        'getuserexchanges()
        'getuserexchangesv2api()

        'GetExchangesAsync()
        'Getapiresults()

        'gettradeorders()

        'createtradeorder()


        'unsub()

        'PostRequestClient()

        'SIGNATURE QUERY
        'ENDPOINT = "/api/v2/private/exchanges"
        'ENDPOINT = "/api/v2/private/exchanges"
        'ENDPOINT = "/api/v2/private/exchanges/BINA/markets/BTC/USDT/ohlc/h"
        'ENDPOINT = "/api/v2/private/user/balances"
        'Dim ENDPOINT As String = "/api/v2/private/user/balances"

    End Sub


End Class


