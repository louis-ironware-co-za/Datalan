Imports System.ComponentModel
Imports System.Text

Imports System
Imports System.Threading
Imports CoinigyWebsocketClient
Imports CoinigyWebsocketClient.Models
Imports CoinigyWebsocketClient.Types
Imports PureSocketCluster

Partial Public Class Form1

#Region "VAR"

		Private Shared ReadOnly api_key As String = "0448b73a465d3946b00b9e6b7b86bf01"
		Private Shared ReadOnly api_secret As String = "f2358363acecf50085ade2e13692d0dd"

		Private Shared ReadOnly ManualResetEvent As ManualResetEvent = New ManualResetEvent(False)
		Private Shared _client As CoinigyWsClient

#End Region 'VAR

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

	Private Property lstBuyC1() As List(Of OrderItem)
			Get
				Return m_lstBuyC1
			End Get
			Set
				m_lstBuyC1 = Value
			End Set
		End Property
		Private m_lstBuyC1 As List(Of OrderItem)
		Private Property lstSellC1() As List(Of OrderItem)
			Get
				Return m_lstSellC1
			End Get
			Set
				m_lstSellC1 = Value
			End Set
		End Property
		Private m_lstSellC1 As List(Of OrderItem)
		Private Property lstBuyC2() As List(Of OrderItem)
			Get
				Return m_lstBuyC2
			End Get
			Set
				m_lstBuyC2 = Value
			End Set
		End Property
		Private m_lstBuyC2 As List(Of OrderItem)
		Private Property lstSellC2() As List(Of OrderItem)
			Get
				Return m_lstSellC2
			End Get
			Set
				m_lstSellC2 = Value
			End Set
		End Property
		Private m_lstSellC2 As List(Of OrderItem)

		Public Property ExchangeItemList() As List(Of ExchangeItem)
			Get
				Return m_ExchangeItemList
			End Get
			Set
				m_ExchangeItemList = Value
			End Set
		End Property
		Private m_ExchangeItemList As List(Of ExchangeItem)

		Public Property ChannelItemList() As List(Of String)
			Get
				Return m_ChannelItemList
			End Get
			Set
				m_ChannelItemList = Value
			End Set
		End Property
		Private m_ChannelItemList As List(Of String)

#End Region 'PROPERTIES

	'What fuckery in C# its Like coding In LISP... lost In stupid parenthesis, and brackets  🫢
	'3 lines of codes And miles And miles of whitespace 🤦‍♂️

	Public Sub New()
		InitializeComponent()
	End Sub

	Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
		SimpleButton1.Enabled = False
		RaiseCoinigy()

	End Sub

	Private Sub RaiseCoinigy()

		_client = New CoinigyWsClient(New CoinigyWebSocketClientOptions With {
			.Creds = New Creds With {
				.apiKey = api_key,
				.apiSecret = api_secret
			},
			.DebugMode = True
		 })

		'EVENTHANDLERS
		AddHandler _client.OnClientReady, AddressOf Client_OnClientReady
		AddHandler _client.OnFatality, AddressOf _client_OnFatality
		AddHandler _client.OnClosed, AddressOf _client_OnClosed
		AddHandler _client.OnStateChanged, AddressOf _client_OnStateChanged

		AddHandler _client.OnOrderMessage, AddressOf _client_OnOrderMessage
		AddHandler _client.OnTradeMessage, AddressOf _client_OnTradeMessage

		'AddHandler _client.OnBlockMessage, AddressOf _client_OnBlockMessage
		AddHandler _client.OnFavoriteMessage, AddressOf _client_OnFavoriteMessage
		AddHandler _client.OnNewsMessage, AddressOf _client_OnNewsMessage
		AddHandler _client.OnNotificationMessage, AddressOf _client_OnNotificationMessage
		AddHandler _client.OnMessage, AddressOf _client_OnMessage

		'_client.OnClientReady += AddressOf Client_OnClientReady
		'_client.OnFatality += AddressOf _client_OnFatality
		'_client.OnClosed += AddressOf _client_OnClosed
		'_client.OnStateChanged += AddressOf _client_OnStateChanged
		'_client.OnOrderMessage += AddressOf _client_OnOrderMessage
		'_client.OnTradeMessage += AddressOf _client_OnTradeMessage
		'_client.OnBlockMessage += AddressOf _client_OnBlockMessage
		'_client.OnFavoriteMessage += AddressOf _client_OnFavoriteMessage
		'_client.OnNewsMessage += AddressOf _client_OnNewsMessage
		'_client.OnNotificationMessage += AddressOf _client_OnNotificationMessage
		'_client.OnMessage += AddressOf _client_OnMessage

		Dim connectionResult = _client.Connect()
		'OutputConsole.WriteLine($"Connected = {connectionResult}", ConsoleColor.Green)
		'ManualResetEvent.WaitOne()

		TradeItemsBuy = New List(Of TradeItem)
		TradeItemssell = New List(Of TradeItem)
		TradeItemsBuyGridcontrol.DataSource = TradeItemsBuy
		TradeItemsSellGridControl.DataSource = TradeItemsSell

		lstBuyC1 = New List(Of OrderItem)().OrderBy(Function(x) x.Price).ToList()
		BuyC1Gridcontrol.DataSource = lstBuyC1

		lstSellC1 = New List(Of OrderItem)().OrderBy(Function(x) x.Price).ToList()
		SellC1Gridcontrol.DataSource = lstSellC1

	End Sub

#Region "SOCKETEVENTS"

	Private Shared Sub _client_OnStateChanged(ByVal newState As System.Net.WebSockets.WebSocketState, ByVal prevState As System.Net.WebSockets.WebSocketState)
		'OutputConsole.WriteLine($"Socket state changed from {prevState} to {newState}", ConsoleColor.Gray)
	End Sub

	Private Shared Sub _client_OnClosed(ByVal reason As System.Net.WebSockets.WebSocketCloseStatus)
		'OutputConsole.WriteLine($"Socket Closed reason: {reason}", ConsoleColor.Red)
	End Sub

	Private Shared Sub _client_OnFatality(ByVal reason As String)
		ManualResetEvent.[Set]()
	End Sub

	Private Shared Sub _client_OnNotificationMessage(ByVal notification As NotificationDataItem)
		'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received notification {notification.Title}", ConsoleColor.Green)
	End Sub

	Private Shared Sub _client_OnNewsMessage(ByVal news As NewsDataItem)
		'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received news item \"{news.Title}\"", ConsoleColor.Green)
	End Sub

	Private Shared Sub _client_OnFavoriteMessage(ByVal favorites As FavoriteDataItem())
		OutputConsole.WriteLine($"{DateTime.UtcNow}: Received favorites count {favorites.Length}", ConsoleColor.Green)
	End Sub

	<Obsolete>
	Private Shared Sub _client_OnBlockMessage(ByVal curr As String, ByVal block As BlockItem)
		'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received new block for {curr} height {block.BlockId}", ConsoleColor.Green)
	End Sub

	Private Sub _client_OnTradeMessage(ByVal exchange As String, ByVal curr1 As String, ByVal curr2 As String, ByVal trade As TradeItem)
		'If InvokeRequired Then
		'	Invoke(New MethodInvoker(AddressOf _client_OnTradeMessage))
		'Else


		'	'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received new trade for {exchange} market {curr1}/{curr2} price {trade.Price}", ConsoleColor.Green)
		'	Dim trade As TradeItem

		'	tradeItemsC1.Add(trade)
		'	Try

		'BuyC1Gridcontrol.RefreshDataSource()

		'SellC1Gridcontrol.RefreshDataSource()

		'	Catch ex As Exception
		'	End Try

		'End If

	End Sub

	Private Shared Sub _client_OnMessage(ByVal messageType As MessageType, ByVal data As String)
		'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received {messageType} message : {data}", ConsoleColor.Yellow)
	End Sub

	Private Sub _client_OnOrderMessage(ByVal exchange As String, ByVal curr1 As String, ByVal curr2 As String, ByVal orders As OrderItem())
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
		'BuyC1Gridcontrol.RefreshDataSource()

		'SellC1Gridcontrol.DataSource = lstSellC1
		'SellC1Gridcontrol.RefreshDataSource()


	End Sub

	Private Sub Client_OnClientReady()

		_client.SubscribeToChannel("TRADE-BMEX--XBT--USD", AddressOf MyCustomCallback)

		_client.SubscribeToChannel("ORDER-BMEX--XBT--USD", AddressOf MyCustomCallback)

		_client.SubscribeToChannel("TRADE-BINA--ZRX--USDT", AddressOf MyCustomCallback)

		_client.SubscribeToChannel("ORDER-BINA--ZRX--USDT", AddressOf MyCustomCallback)

	End Sub

#End Region 'SOCKETEVENTS

	Enum tradetype
		Buy
		Sell
	End Enum


	Public tradeItems() As List(Of TradeItem)

	Public Sub clientontrademessage(ByVal trade As TradeResponse)
		Try
			Dim TradeItm As TradeItem = trade.TradeData.Trade
			If Not TradeItm Is Nothing Then
				If TradeItm.Type.ToLower = tradetype.Buy.ToString.ToLower Then
					TradeItemsBuy.Add(TradeItm)
				End If
				If TradeItm.Type.ToLower = tradetype.Sell.ToString.ToLower Then
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


	Private Delegate Sub StringDelegate(<[ParamArray]()> ByVal params As OrderResponse)
	Public Sub clientonordermessage(ByVal orderslist As OrderResponse)
		If Me.InvokeRequired Then
			Me.Invoke(New StringDelegate(AddressOf clientonordermessage), New OrderResponse() {orderslist})
		Else
			Dim OrderItm() As OrderItem = orderslist.OrderData.Orders
			lstSellC1 = OrderItm.Where(Function(x) x.Ordertype.ToLower() = "sell").ToList()
			lstBuyC1 = OrderItm.Where(Function(x) x.Ordertype.ToLower() = "buy").ToList()

			BuyC1Gridcontrol.DataSource = lstBuyC1
			BuyC1Gridcontrol.RefreshDataSource()

			SellC1Gridcontrol.DataSource = lstSellC1
			SellC1Gridcontrol.RefreshDataSource()
		End If
	End Sub


	'CUSTOMCALLBACK
	Private Sub MyCustomCallback(ByVal messageType As MessageType, ByVal data As String)
		'OutputConsole.WriteLine($"{DateTime.UtcNow}: Received {messageType} message in private channel callback", ConsoleColor.Green)

		Select Case messageType
			Case MessageType.TradeData
				Try
					'OutputConsole.WriteLine($"{DateTime.UtcNow}: We have deserialized a trade in our custom callback and the price in that trade is {deserializedClass.TradeData.Trade.Price}", ConsoleColor.Green)
					Dim deserializedClass As TradeResponse = TradeResponse.FromJson(_client.Options.Serializer, data)
					clientontrademessage(deserializedClass)
				Catch ex As Exception
					'OutputConsole.WriteLine(ex.Message)
				End Try
			Case MessageType.OrderData
				Try
					'OutputConsole.WriteLine($"{DateTime.UtcNow}: We have deserialized an orderbook in our custom callback and the first price in that data is {deserializedClass.OrderData.Orders(0).Price}", ConsoleColor.Green)
					Dim deserializedClass = OrderResponse.FromJson(_client.Options.Serializer, data)
					clientonordermessage(deserializedClass)
				Catch ex As Exception
					OutputConsole.WriteLine(ex.Message)
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

End Class
