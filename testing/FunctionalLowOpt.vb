Public MustInherit Class FunctionalLowOpt
    Inherits DistanceMinimizer

    Friend d As Integer
    MustOverride Function x0() As Double()

    Sub New(dst As Func(Of Double(), Double(), Double),
            d As Integer,
            p() As Double)
        MyBase.New(dst)
        Me.d = d
        Me.p = p
    End Sub
    Public Overrides Sub aim(x() As Double, ByRef res As Double, obj As Object)
        res = Me.dst(ResultValue(x), Me.p)
    End Sub


    Public Overrides Function StartValue(p() As Double) As Double()
        Return ResultValue(Me.x0())
    End Function
End Class
