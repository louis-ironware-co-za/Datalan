Imports System.Runtime.Serialization
Imports Newtonsoft.Json

'Namespace Coinigy.API.Responses
<JsonObject(MemberSerialization:=MemberSerialization.Fields)>
Public Class tradingpair

    Public marketId As String
    Public marketName As String

    Public exchId As String
    Public exchCode As String
    Public exchName As String
    Public baseCurrCode As String
    Public baseCurrId As String

    Public quoteCurrCode As String
    Public quoteCurrId As String
    Public baseCurrName As String
    Public quoteCurrName As String

    Public exchmktId As String
    Public displayName As String
    Public baseIsFiat As String
    Public quoteIsFiat As String

    Public basePricePrecision As String
    Public baseQuantityPrecision As String
    Public quoteQuantityPrecision As String
    Public primaryCurrCode As String
    Public secondaryCurrCode As String

    Public baseHasImage As String
    Public quoteHasImage As String
    Public baseImageName As String
    Public quoteImageName As String

    '    {"success":true,"error":null,"result":[
    '    {"marketId":237,"marketName":"USD/BTC","exchId":4,"exchCode":"BITS","exchName":"Bitstamp","baseCurrCode":"USD","baseCurrId":189,
    '    "quoteCurrCode":"BTC","quoteCurrId":1,"baseCurrName":"United States Dollar","quoteCurrName":"Bitcoin","exchmktId":346,"displayName":"BTC/USD",
    '    "baseIsFiat":true,"quoteIsFiat":false,"basePricePrecision":2,"baseQuantityPrecision":8,"quotePricePrecision":8,"quoteQuantityPrecision":8,
    '    "primaryCurrCode":"USD","secondaryCurrCode":"BTC","baseHasImage":true,"quoteHasImage":true,"baseImageName":"USD","quoteImageName":"BTC"},

End Class
'End Namespace