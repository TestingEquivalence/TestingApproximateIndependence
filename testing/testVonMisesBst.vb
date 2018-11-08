Public Class testVonMisesBst
    'Inherits ConEqTest
    'Public a, b As Double
    'Public Sub New(F As EmpiricalDistr,
    '        G As clsDistribution,
    '        eps As Double,
    '        alpha As Double,
    '        adj_eps As Double,
    '        n_bst As Integer,
    '        a As Double,
    '        b As Double)
    '    MyBase.New(F, G, eps, alpha, adj_eps, n_bst)

    '    Me.d = 1
    'End Sub

    'Public Overrides Function est_Var(X() As Double) As Double
    '    Throw New Exception("no variance estimator available")
    '    Return 0
    'End Function

    'Public Overrides Function norm_T(X() As Double) As Double
    '    Throw New Exception("no variance estimator available")
    '    Return 0
    'End Function


    'Public Overrides Function simple_T(X() As Double) As Double
    '    Dim dst As New vonMisesDist(a, b)
    '    Return dst.calc(F.CDF, G.CDF)
    'End Function

    'Public Overrides Sub AimRandPoint(s() As Double, ByRef res As Double, obj As Object)
    '    Dim DF As Func(Of Double, Double)
    '    DF = Function(t As Double)
    '             Dim sum As Double
    '             Dim w As Double
    '             w = s(0)

    '             sum = w * F.CDF(t) + (1 - w) * G.CDF(t)
    '             Return sum
    '         End Function

    '    Dim DG As Func(Of Double, Double)
    '    DG = Function(t As Double)
    '             Return G.CDF(t)
    '         End Function

    '    Dim dst As New vonMisesDist(a, b)
    '    res = dst.calc(DF, DG)
    'End Sub


    'Public Overrides Function GetQuantilTransform(s() As Double) As System.Func(Of Double, Double)
    '    Return Nothing
    'End Function
End Class
