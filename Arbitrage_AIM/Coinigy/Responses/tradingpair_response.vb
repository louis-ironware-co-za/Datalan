Imports System.Collections.Generic
Imports System.Runtime.Serialization
Imports Newtonsoft.Json

'Namespace Coinigy.API.Responses
<JsonObject(MemberSerialization:=MemberSerialization.Fields)>
Public Class tradingpair_response
    Inherits baseresponse

    Public success As Boolean
    Public [error] As String
    Public result As List(Of tradingpair)

End Class
'End Namespace
