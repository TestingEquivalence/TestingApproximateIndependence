Public Class ZipfsLow
    Inherits FunctionalLowOpt
    Dim norm As Double
    Dim s As Double
    Sub New(dst As Func(Of Double(), Double(), Double),
            d As Integer,
            p() As Double)
        MyBase.New(dst, d, p)

        
    End Sub

    Public Overrides Function ResultValue(x() As Double) As Double()
        Me.norm = 0
        Me.s = x(0)

        For i As Integer = 1 To MyBase.d
            Me.norm = Me.norm + Math.Pow(1 / i, s)
        Next

        Dim p(d - 1) As Double
        For i As Integer = 1 To MyBase.d
            p(i - 1) = Math.Pow(1 / i, s) / norm
        Next

        Return p
    End Function

    
    Public Overrides Function x0() As Double()
        Return {2.0}
    End Function
End Class
