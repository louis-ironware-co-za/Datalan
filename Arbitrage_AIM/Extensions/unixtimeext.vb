
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

'Namespace bitxlib.Logic

Public Class unixtimeext
    Public Shared ReadOnly UnixEpoch As New DateTime(1970, 1, 1, 0, 0, 0,
            0, DateTimeKind.Utc)

    Public Shared Function DateTimeUTCFromUnixTime(unixtime As Long) As DateTime
        Dim result = UnixEpoch.AddSeconds(unixtime)
        Return result
    End Function

    Public Shared Function UnixTimeFromDateTimeUTC(dateTimeUTC As DateTime) As Long
        Dim diff = (dateTimeUTC - UnixEpoch)
        Dim result = CLng(diff.TotalSeconds)
        Return result
    End Function

End Class
'End Namespace