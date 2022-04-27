Imports System.ComponentModel
Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports LunoIntegration
Imports LunoIntegration.Models
Imports LunoIntegration.StreamModels
Imports Newtonsoft.Json
Imports Xunit

'Imports LunoClientBase

Partial Public Class Form1

    Private Const key As String = "g6pa7gwqn35b9"
    Private Const secret As String = "JXI74al98v9OkgjxPG79aIxr5qGJmWhT752N3uhS7Ak"
    Private canceltoken As Boolean = False

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        Dim _lunoBase As New LunoClientBase
        Dim _lunoClient As New LunoIntegration.LunoClient
        Dim _orderbookBase As New OrderBookBase
        Dim _streamBase As New LiveStreamBase

        'Dim task As Task = _streamBase.Can_Get_OrderBook_From_LiveStream()
        'Dim task As Task = Can_Get_OrderUpdate_From_LiveStream()
        Dim task As Task = Can_Get_OrderUpdate_From_LiveStream()

        Console.WriteLine()

    End Sub

    Public Function GetClient() As LunoClient
        Return New LunoClient(key:=key, secret:=secret)
    End Function

    'Private ClientItemsList As New List(Of LiveOrderBook)
    Private ClientItemsList As New List(Of Object)
    Private ClientChannelDataSet As DataSet = Arbitrage_AIM.xfuiUser.ChannelDataSet

    <Fact>
    Public Async Function Can_Get_OrderUpdate_From_LiveStream() As Task

        Dim client = Me.GetClient()
        client.itemslist.Clear()
        clientitemslist = client.itemslist
        GridControl1.DataSource = ClientItemsList

        canceltoken = False
        Dim pair = "XBTZAR"
        'Dim client = Me.GetClient()
        'client.itemslist.Clear()
        'GridControl1.DataSource = client.itemslist

        Dim cancellationTokenSource = New CancellationTokenSource()
        Dim receivedMessage As String = String.Empty
        Await client.ConnectLive(pair:=pair, orderBookReceivedAction:=Nothing, orderUpdateReceivedAction:=Function(message As String)
                                                                                                              'receivedMessage = message
                                                                                                              'itemslist.Add(receivedMessage)
                                                                                                              'newfunct()

                                                                                                              GridControl1.Invoke(New Action(Function()
                                                                                                                                                 GridControl1.RefreshDataSource()
                                                                                                                                             End Function))
                                                                                                              If canceltoken = True Then
                                                                                                                  cancellationTokenSource.Cancel()
                                                                                                              End If
                                                                                                          End Function, disconnectedAction:=Nothing, cancellationToken:=cancellationTokenSource.Token)

        'Dim OrderItm() As OrderItem = orderslist.OrderData.Orders
        'OrderItemsSell = itemslist.Where(Function(x) x.
        'OrderItemsBuy = itemslist.Where(Function(x) x.Ordertype() = TradeType.Buy.ToString).ToList()

        '{"order_id":"BXFNGZWZKRA2YUE","type":"ASK","price":"650394.00000000","volume":"0.20000000"}
        '{"sequence":"784174502","trade_updates":[],"create_update":{"order_id":"BX5BCZY3S7RCMN","type":"BID","price":"647424.00000000","volume":"0.00380600"},"delete_update":null,"status_update":null,"timestamp":1650973144384}
        '{"sequence":"784180433","trade_updates":[],"create_update":{"order_id":"BXEZKPAGVGX9H4X","type":"BID","price":"647516.00000000","volume":"0.00152000"},"delete_update":null,"status_update":null,"timestamp":1650974080118}

        '{"order_id":"BXFNGZWZKRA2YUE",
        '"type":"ASK",
        '"price":"650394.00000000",
        '"volume":"0.20000000"}

        '{"sequence":"784180433",
        '"trade_updates":[],
        '"create_update":{"order_id":"BXEZKPAGVGX9H4X","type":"BID","price":"647516.00000000","volume":"0.00152000"},
        '"delete_update":null,
        '"status_update":null,
        '"timestamp":1650974080118
        '}

        '{"sequence":"784225398",
        '"trade_updates":[{
        '"base":"0.00210000",
        '"counter":"1354.5000000000000000",
        '"maker_order_id":"BXDDRSRE539CZUY",
        '"taker_order_id":"BXB66ZGSDCCF7E5",
        '"order_id":"BXDDRSRE539CZUY"}],
        '"create_update":null,
        '"delete_update":null,"status_update":null,"timestamp":1650977945601}

        Await client.DisconnectLive()
        'WaitHandle.WaitAny({cancellationTokenSource.Token.WaitHandle})
        Assert.NotEqual(String.Empty, receivedMessage)
    End Function

    Private Sub newfunct()

        For Each _item In ClientItemsList
            'Dim clientitem As String = _item.ToString

            'Dim livebooksList As LiveUpdate = JsonConvert.DeserializeObject(Of LiveUpdate)(clientitem)

            'Dim OrderItemsBuy = ClientItemsList.OrderBy(Function(x) x.Price).ThenBy(Function(x) x.Price).ToList()
            'Dim OrderItemsSell = ClientItemsList.OrderBy(Function(x) x.Price).ThenByDescending(Function(x) x.Price).ToList()


            Console.WriteLine()
        Next
        ' deserilaize ClientItemsList

        'Dim livebooksList = JsonConvert.DeserializeObject(Of LiveOrderBook)(clientitem)

        'DataSet
        'MyCustomCallback

        'ClientChannelDataSet = Arbitrage_AIM.xfuiUser.ChannelDataSet

        Dim askMin As Decimal = 0
        Dim bidMax As Decimal = 0

        'SORT
        'ClientItemsList

        'ADD ORDERBOOK DATASET
        'Dim orderbookdata As DataSet = OrderBookDataSet
        Dim tablename As String = "ORDER-LUNO--XBT--ZAR" ' channelname.ToString
        Dim table As DataTable = ClientChannelDataSet.Tables(tablename)

        'ADD CHANNEL TABLES
        'Dim table As DataTable
        table = New DataTable(tablename)
        ClientChannelDataSet.Tables.Add(table)

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
        'For i As Integer = 0 To 19
        '    table.Rows.Add(i, 0.00, "orderside", 0.0, 0.0, channelname, DateTime.Now)

        'Next

        'Dim OrderItm() As OrderItem = orderslist.OrderData.Orders
        'Dim OrderItemsBuy = ClientItemsList.OrderBy(Function(x) x.Price).ThenBy(Function(x) x.Price).ToList()
        'Dim OrderItemsSell = ClientItemsList.OrderBy(Function(x) x.Price).ThenByDescending(Function(x) x.Price).ToList()

        ''UPDATE ARBITRAGE TABLE - mem
        'Dim table As DataTable = ChannelDataSet.Tables(tablename)
        ''UPDATE CHANNEL ORDERS - BIND TABLE
        'If tablename.Contains("BUY") Then
        '    Dim i As Integer = 0
        '    For Each _orderitem As OrderItem In OrderItemsBuy
        '        Dim row As DataRow = table.Rows(i)
        '        row.BeginEdit()
        '        row("Ordertype") = _orderitem.Ordertype
        '        row("Price") = _orderitem.Price
        '        row("Quantity") = _orderitem.Quantity
        '        row("Total") = _orderitem.Total
        '        row.EndEdit()
        '        row.AcceptChanges()
        '        i += 1
        '    Next
        '    table.AcceptChanges()
        '    'TODO
        '    If tablename = "ORDER-BINA--BTC--USDT--BUY" Then
        '        bidGridControl.RefreshDataSource()
        '    End If

        '    'Sell Highest Buy in Buyorders 
        '    askMin = OrderItemsSell.Min(Function(x) x.Price)
        '    Dim qtyBuyMax As Double = Convert.ToDecimal(OrderItemsSell.Max(Function(x) x.Quantity))
        '    Dim minBuyPrice As Double = Convert.ToDecimal(OrderItemsSell.Min(Function(x) x.Price))
        'End If
    End Sub


    Private Sub SimpleButton2_Click(sender As Object, e As EventArgs) Handles SimpleButton2.Click
        canceltoken = True
    End Sub

    '<Fact>
    'Public Async Function Can_Get_OrderBook_From_LiveStream() As Task

    '    'this.socketCancellationToken = CancellationToken;
    '    'this.orderBookReceivedAction = orderBookReceivedAction;
    '    'this.orderUpdateReceivedAction = orderUpdateReceivedAction;
    '    'this.disconnectedAction = disconnectedAction;

    '    'orderUpdateReceivedAction
    '    'Dim lclient As New LunoClient
    '    'lclient.
    '    '    orderUpdateReceivedAction



    '    'Dim pair = "XBTZAR"
    '    'Dim client = Me.GetClient()



    '    'Dim cancellationTokenSource = New CancellationTokenSource()
    '    'Dim receivedMessage As String = String.Empty
    '    'Await client.ConnectLive(pair:=pair, orderBookReceivedAction:=Function(ByVal message As String)
    '    '                                                                  receivedMessage = message
    '    '                                                                  cancellationTokenSource.Cancel()
    '    '                                                              End Function, orderUpdateReceivedAction:=Nothing, disconnectedAction:=Nothing, cancellationToken:=cancellationTokenSource.Token)
    '    'Await client.DisconnectLive()
    '    'WaitHandle.WaitAny({cancellationTokenSource.Token.WaitHandle})
    '    'Assert.NotEqual(String.Empty, receivedMessage)
    'End Function

    '<Fact>
    'Public Async Function Can_Get_LiveOrderBook_From_LiveStream() As Task
    '    canceltoken = False
    '    itemslist.Clear()
    '    GridControl1.DataSource = itemslist



    '    Dim pair = "XBTZAR"
    '    Dim client = Me.GetClient()
    '    Dim cancellationTokenSource = New CancellationTokenSource()
    '    Dim receivedMessage As String = String.Empty
    '    Await client.ConnectLive(pair:=pair, orderBookReceivedAction:=Nothing, orderUpdateReceivedAction:=Function(ByVal message As String)
    '                                                                                                          receivedMessage = message

    '                                                                                                          itemslist.Add(receivedMessage)



    '                                                                                                          'LiveUpdate
    '                                                                                                          'OrdersResponse receivedMessage
    '                                                                                                          '     result = response.Content.ReadAsStringAsync().Result

    '                                                                                                          'livebooksList = JsonConvert.DeserializeObject(Of LiveOrderBook)(message)

    '                                                                                                          'booksList.Add(livebooksList)

    '                                                                                                          GridControl1.Invoke(New Action(Function()
    '                                                                                                                                             GridControl1.RefreshDataSource()
    '                                                                                                                                         End Function))

    '                                                                                                          If canceltoken = True Then
    '                                                                                                              cancellationTokenSource.Cancel()
    '                                                                                                          End If

    '                                                                                                      End Function, disconnectedAction:=Nothing, cancellationToken:=cancellationTokenSource.Token)


    '    Await client.DisconnectLive()
    '    'WaitHandle.WaitAny({cancellationTokenSource.Token.WaitHandle})
    '    Assert.NotEqual(String.Empty, receivedMessage)
    'End Function

    'Private Property OrderItemsBuy() As List(Of Object)
    '    Get
    '        Return m_OrderItemsBuy
    '    End Get
    '    Set
    '        m_OrderItemsBuy = Value
    '    End Set
    'End Property
    'Private m_OrderItemsBuy As List(Of Object)
    'Private Property OrderItemsSell() As List(Of Object)
    '    Get
    '        Return m_OrderItemsSell
    '    End Get
    '    Set
    '        m_OrderItemsSell = Value
    '    End Set
    'End Property
    'Private m_OrderItemsSell As List(Of Object)


    'Public itemslist As New List(Of Object)

    '#Can_Get_OrderUpdate_From_LiveStream

End Class

'Friend Class LunoClient_OnClientConnect
'    Public Sub New()
'    End Sub
'End Class
