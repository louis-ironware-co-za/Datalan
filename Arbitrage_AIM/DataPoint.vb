Public Class DataPoint
    Public Property Argument As DateTime
    Public Property Low As Double
    Public Property High As Double
    Public Property Open As Double
    Public Property Close As Double
    Public Sub New(ByVal argument As DateTime, ByVal low As Double, ByVal high As Double, ByVal open As Double, ByVal close As Double)
        Me.Argument = argument
        Me.Low = low
        Me.High = high
        Me.Open = open
        Me.Close = close
    End Sub
End Class
