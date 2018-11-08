''' <summary>
''' The class provides two tests for the approximate independence
''' of the rows and columns of a contingency table.
''' The tests are based on the Euclidian norm
''' of the relative deviation between the counting frequencies
''' and the product of the marginal distributions.
''' The class should be initialized with a contingence table,
''' which entries should be event counts. 
''' The number of all events n will be derived from that table.
''' </summary>
Public Class RelChangeTest
    Inherits ctable

    Sub New(n1 As Integer, n2 As Integer)
        MyBase.New(n1, n2)
    End Sub

    Sub New(matrix As Double(,))
        MyBase.New(matrix)
    End Sub


    Public Overrides Function T(i As Integer, j As Integer) As Double
        Return matrix(i, j) / (R(i) * C(j))
    End Function

    Public Overrides Function dT(i As Integer, j As Integer,
                       k As Integer, m As Integer) As Double

        If (i = k) And (j <> m) Then
            Return -matrix(i, j) / (R(i) * R(i) * C(j))
        End If

        If (i <> k) And (m = j) Then
            Return -matrix(i, j) / (R(i) * C(j) * C(j))
        End If

        If (i = k) And (j = m) Then
            Dim Z, N As Double
            Z = R(i) * C(j) - (R(i) + C(j)) * matrix(i, j)
            N = (R(i) * C(j)) ^ 2
            Return Z / N
        End If

        Return 0
    End Function


    Public Overrides Function startPoint(p() As Double) As ctable
        Dim m = Me.Vector2Matrix(p)
        Dim prod = New RelChangeTest(m)
        Dim prodMatrix = ProductMatrix(prod.R, prod.C)
        Return New RelChangeTest(prodMatrix)
    End Function

    Public Overrides Function TStartPoint(i As Integer, j As Integer) As Double
        Return 1
    End Function
End Class
