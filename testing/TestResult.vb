Public Class TestResult
    Public result As Boolean
    Public minEps As Double

    Sub New(result As Boolean, minEps As Double)
        Me.result = result
        Me.minEps = minEps
    End Sub
End Class
