Imports System.Threading
Imports System.Threading.Tasks
Imports Xunit





Public Class LiveStreamBase
    Inherits LunoClientBase

    'Public Property ItemList() As List(Of Object)
    '    Get
    '        Return m_ItemList
    '    End Get
    '    Set
    '        m_ItemList = Value
    '    End Set
    'End Property
    'Private m_ItemList As List(Of Object)

    ''ExchangeItemList = New List(Of ExchangeItem)
    '' Dim Exchanges As IEnumerable(Of ExchangeItem) = _coinigyclient.GetExchanges()
    '' ExchangeItemList = Exchanges.ToList()

    'Public itemslist As New List(Of Object)

    '<Fact>
    'Public Async Function Can_Get_OrderBook_From_LiveStream() As Task
    '    Dim pair = "XBTZAR"
    '    Dim client = Me.GetClient()
    '    Dim cancellationTokenSource = New CancellationTokenSource()
    '    Dim receivedMessage As String = String.Empty
    '    Await client.ConnectLive(pair:=pair, orderBookReceivedAction:=Function(ByVal message As String)
    '                                                                      receivedMessage = message
    '                                                                      cancellationTokenSource.Cancel()
    '                                                                  End Function, orderUpdateReceivedAction:=Nothing, disconnectedAction:=Nothing, cancellationToken:=cancellationTokenSource.Token)
    '    Await client.DisconnectLive()
    '    WaitHandle.WaitAny({cancellationTokenSource.Token.WaitHandle})
    '    Assert.NotEqual(String.Empty, receivedMessage)
    'End Function




    '<Fact>
    'Public Async Function Can_Get_OrderUpdate_From_LiveStream() As Task
    '    Dim pair = "XBTZAR"
    '    Dim client = Me.GetClient()
    '    Dim cancellationTokenSource = New CancellationTokenSource()
    '    Dim receivedMessage As String = String.Empty
    '    Await client.ConnectLive(pair:=pair, orderBookReceivedAction:=Nothing, orderUpdateReceivedAction:=Function(ByVal message As String)
    '                                                                                                          receivedMessage = message
    '                                                                                                          itemslist.Add(receivedMessage)
    '                                                                                                          'cancellationTokenSource.Cancel()
    '                                                                                                      End Function, disconnectedAction:=Nothing, cancellationToken:=cancellationTokenSource.Token)
    '    Await client.DisconnectLive()
    '    'WaitHandle.WaitAny({cancellationTokenSource.Token.WaitHandle})
    '    Assert.NotEqual(String.Empty, receivedMessage)
    'End Function
End Class