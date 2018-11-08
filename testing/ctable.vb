''' <summary>
''' the class represents the two-way contingancy tables and
''' provides useful functions to test for the approximate row-column independence.
''' </summary>
Public MustInherit Class ctable

    Public MustOverride Function startPoint(p() As Double) As ctable
    Public MustOverride Function TStartPoint(i As Integer, j As Integer) As Double
    Public MustOverride Overloads Function T(i As Integer, j As Integer) As Double
    Public MustOverride Function dT(i As Integer, j As Integer, k As Integer, m As Integer) As Double

    Friend p() As Double
    Friend lp, lq As Double()

    Friend bound As Double
    Friend n1 As Integer
    Friend n2 As Integer
    Friend n As Double
    Friend R() As Double
    Friend C() As Double
    Friend matrix As Double(,)
    Friend boundaryPoints As List(Of ctable())

    Sub New(_n1 As Integer, _n2 As Integer)
        n1 = _n1 - 1
        n2 = _n2 - 1

        Me.matrix = New Double(n1, n2) {}

        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                matrix(i, j) = 1 / (_n1 * _n2)
            Next
        Next

        n = 0

        rows()
        columns()
    End Sub



    Sub New(matrix As Double(,))
        Me.n1 = matrix.GetUpperBound(0)
        Me.n2 = matrix.GetUpperBound(1)

        'number of samples
        Me.n = 0
        Dim s As Double = 0
        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                s = s + matrix(i, j)
            Next
        Next
        n = Convert.ToInt32(s)

        Me.matrix = New Double(n1, n2) {}
        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                Me.matrix(i, j) = matrix(i, j) / s
            Next
        Next
        rows()
        columns()
    End Sub

    Sub columns()
        'column sums
        C = New Double(n2) {}
        For j As Integer = 0 To n2
            C(j) = 0
            For i As Integer = 0 To n1
                C(j) = C(j) + Me.matrix(i, j)
            Next
        Next
    End Sub

    Sub rows()
        'row sums
        R = New Double(n1) {}
        For i As Integer = 0 To n1
            R(i) = 0
            For j As Integer = 0 To n2
                R(i) = R(i) + Me.matrix(i, j)
            Next
        Next
    End Sub

#Region "helper"

    Public Shared Function Symplex2Space(p() As Double) As Double()
        Dim i, n As Integer
        n = p.Length
        Dim x(n - 1) As Double

        For i = 0 To n - 1
            x(i) = Math.Log(p(i) + 1)
        Next
        Return x
    End Function

    Public Shared Function Space2Symplex(x() As Double) As Double()
        Dim i, n As Integer
        n = x.Length
        Dim p(n - 1) As Double

        For i = 0 To n - 1
            p(i) = Math.Max(Math.Exp(x(i)) - 1, 0)
        Next

        Dim s As Double = 0

        For i = 0 To n - 1
            s = s + p(i)
        Next

        For i = 0 To n - 1
            p(i) = p(i) / s
        Next

        Return p
    End Function

    Public Shared Function RandomPoint(d As Integer,
                       rnd As System.Random
                               ) As Double()
        Dim x(d - 1) As Double
        For i = 0 To d - 1
            x(i) = rnd.NextDouble()
        Next

        Return ctable.Space2Symplex(x)
    End Function

    Public Shared Function LinComb(v1 As Double(), v2 As Double(), alpha As Double) As Double()
        Dim lc(v1.Length - 1) As Double

        For i As Integer = 0 To v1.Length - 1
            lc(i) = v1(i) * alpha + v2(i) * (1 - alpha)
        Next

        Return lc
    End Function

#End Region


#Region "special points"

    Function RandomOuterPoint(lowBound As Double,
                              d As Integer,
                              rnd As System.Random) As Double()
        Dim res() As Double
        Dim max As Double = 0

        Do
            res = ctable.RandomPoint(d, rnd)
        Loop Until (Me.distance(res) > lowBound)

        Return res
    End Function

    Public Function RandomLinBoundary(eps As Double, d As Integer, rnd As System.Random) As Double()

        Dim q = ctable.RandomPoint(d, rnd)

        Do While Me.distance(q) <= eps
            q = ctable.RandomPoint(d, rnd)
        Loop

        Dim p = startPoint(q)

        Return linearBoundaryPoint(p.Matrix2Vector(p.matrix), q, eps)
    End Function

    Function linearBoundaryPoint(p As Double(), q As Double(), eps As Double) As Double()
        Dim f As Func(Of Double, Double)
        f = Function(alpha)
                Dim lc = ctable.LinComb(q, p, alpha)
                Dim dst = Me.distance(lc)
                Return dst - eps
            End Function

        Dim s As Double
        s = MathNet.Numerics.RootFinding.Brent.FindRoot(f, 0, 1, , 1000)
        Return LinComb(q, p, s)
    End Function
#End Region

    Public Function dT(i, j) As Double(,)
        Dim D(n1, n2) As Double

        For k As Integer = 0 To n1
            For m As Integer = 0 To n2
                D(k, m) = dT(i, j, k, m)
            Next
        Next

        Return D
    End Function

    Public Overloads Function ddist() As Double(,)
        Dim D(n1, n2) As Double

        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                D(i, j) = ddist(i, j)
            Next
        Next

        Return D
    End Function

    Public Function AsymptoticStDev(p() As Double, dvec() As Double) As Double

        Dim vnsq_1 As Double = 0
        For j As Integer = 0 To p.Length - 1
            vnsq_1 = vnsq_1 + p(j) * dvec(j) * dvec(j)
        Next

        Dim vnsq_2 As Double = 0
        For j1 As Integer = 0 To p.Length - 1
            For j2 As Integer = 0 To p.Length - 1
                vnsq_2 = vnsq_2 + dvec(j1) * dvec(j2) * p(j1) * p(j2)
            Next
        Next

        Dim vnsq As Double = (vnsq_1 - vnsq_2)
        Return Math.Sqrt(vnsq)
    End Function

    Public Function AsymptoticStDev() As Double
        Dim p = Matrix2Vector(matrix)
        Dim D = ddist()
        Dim dvec = Matrix2Vector(D)

        Return AsymptoticStDev(p, dvec)
    End Function

    Public Function ProductMatrix(p As Double(), q As Double()) As Double(,)
        Dim f(n1, n2) As Double
        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                f(i, j) = p(i) * q(j)
            Next
        Next
        Return f
    End Function

    Public Function Vector2Matrix(v As Double()) As Double(,)
        Dim f(n1, n2) As Double
        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                f(i, j) = v(i * (n2 + 1) + j)
            Next
        Next
        Return f
    End Function

    Public Function Matrix2Vector(f As Double(,)) As Double()
        Dim d As Integer
        d = (n1 + 1) * (n2 + 1)
        Dim v(d - 1) As Double
        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                v(i * (n2 + 1) + j) = f(i, j)
            Next
        Next
        Return v
    End Function


    Public Overloads Function ddist(k As Integer, m As Integer) As Double
        Dim s As Double = 0
        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                s = s + 2 * (T(i, j) - TStartPoint(i, j)) * dT(i, j, k, m)
            Next
        Next
        Return s
    End Function

    Public Overloads Function distance() As Double
        Dim s As Double = 0
        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                s = s + (T(i, j) - TStartPoint(i, j)) ^ 2
            Next
        Next
        Return s
    End Function

    Function reload(v As Double()) As Double()
        Dim _m = Me.matrix

        Me.matrix = Me.Vector2Matrix(v)

        'normalize matrix
        Dim s As Double = 0
        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                s = s + matrix(i, j)
            Next
        Next

        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                matrix(i, j) = matrix(i, j) / s
            Next
        Next


        'claculate rows and columns
        Me.rows()
        Me.columns()

        Return Me.Matrix2Vector(_m)
    End Function

    Public Overloads Function T() As Double(,)
        Dim _T(n1, n2) As Double
        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                _T(i, j) = T(i, j)
            Next
        Next

        Return _T
    End Function

    Public Overloads Function distance(q As Double()) As Double
        Dim s As Double = 0
        Dim myMatrix = reload(q)
        Dim res = Me.distance()
        reload(myMatrix)
        Return res
    End Function

    Public Overloads Function distance(p As Double(), q As Double()) As Double
        Dim s As Double = 0
        Dim myMatrix = reload(p)
        Dim Tp = Me.T
        reload(q)
        Dim Tq = Me.T

        For i As Integer = 0 To n1
            For j As Integer = 0 To n2
                s = s + (Tp(i, j) - Tq(i, j)) ^ 2
            Next
        Next

        reload(myMatrix)
        Return s
    End Function


    Function rawEps(eps As Double) As Double
        Return eps * eps * (n1 + 1) * (n2 + 1)
    End Function

    Function eps(rawEps As Double) As Double
        Return Math.Sqrt(rawEps / ((n1 + 1) * (n2 + 1)))
    End Function

    ''' <summary>
    ''' The asymptotic test is based on the asymptotic distribution of the test statistic. 
    ''' Therefore the asymptotic test need some sufficiently large number of the observations.
    ''' It should be used carefully because the test is approximate 
    ''' and may be anti-conservative at some points. 
    ''' In order to obtain a conservative test reducing of alpha  (usually halving) or
    ''' slight shrinkage of the tolerance parameter epsilon may be appropriate.
    ''' </summary>
    ''' <param name="alpha">significance level</param>
    ''' <param name="eps">Tolerance parameter is optional because
    ''' the minimale eps, for which the test can reject H0, will be calculated at any case.
    ''' </param>
    ''' <returns>
    ''' The function returns the smallest epsilon for which test can reject H_0.
    ''' Additionally it returns the test result, which is true if the test can reject H_0 for given eps. 
    ''' </returns>
    Public Function AsymptTestIndependence(alpha As Double, Optional eps As Double = 0) As TestResult
        Dim vol As Double
        Dim res As New TestResult(False, 0)
        Dim rawEps As Double = Me.rawEps(eps)

        vol = AsymptoticStDev() / Math.Sqrt(n)
        Dim qt = MathNet.Numerics.Distributions.Normal.InvCDF(0, 1, 1 - alpha) * vol
        Dim crit As Double = rawEps - qt

        Dim T As Double
        T = distance()

        res.minEps = Me.eps(T + qt)

        If T < crit Then
            res.result = True
        Else
            res.result = False
        End If

        Return res
    End Function

    Public Function BootstrapTestIndependence(alpha As Double, eps As Double,
                                          ExteriorPoints As List(Of Double()),
                                          nBootstrapSamples As Integer,
                                          rnd As System.Random) As TestResult
        Dim res As New TestResult(False, 1)
        Dim rawEps As Double = Me.rawEps(eps)

        'calculate test statistic and check if the value outside of the H0
        Dim T As Double
        T = distance()

        If T > rawEps Then
            Return res
        End If

        'find nearest boundary point
        Dim p = Me.Matrix2Vector(matrix)

        'go trought all external points
        'and search for the nearest one
        Dim min_dist As Double = -1
        Dim nearestBndPoint = Me.linearBoundaryPoint(p, ExteriorPoints.First, rawEps)
        min_dist = Me.l2(p, nearestBndPoint)

        For Each ext In ExteriorPoints
            Dim q = Me.linearBoundaryPoint(p, ext, rawEps)
            Dim dst = Me.l2(p, q)
            If dst < min_dist Then
                min_dist = dst
                nearestBndPoint = q
            End If
        Next

        'generate bootstrap sample
        Dim bstSample(nBootstrapSamples - 1) As Double
        For i As Integer = 0 To nBootstrapSamples - 1
            Dim sampleNearestBndPoint = Research.sample(nearestBndPoint, n, rnd)
            bstSample(i) = Me.distance(sampleNearestBndPoint)
        Next

        Dim pValue As Double
        For Each sampleT In bstSample
            If T > sampleT Then
                pValue = pValue + 1
            End If
        Next

        pValue = pValue / nBootstrapSamples

        res.result = pValue < alpha
        res.minEps = pValue
        Return res
    End Function

    ''' <summary>
    ''' The bootstrap test is based on the re-sampling method called bootstrap. 
    ''' The bootstrap test is often more precise and reliable than the asymptotic test. 
    ''' However, it should be used carefully because the test is approximate only
    ''' and may be anti-conservative at some points. 
    ''' In order to obtain a conservative test reducing of alpha
    ''' (usually halving) or slight shrinkage of the tolerance parameter epsilon
    ''' may be appropriate. We prefer the slight shrinkage of the tolerance parameter 
    ''' because it is more effective and the significance level remains unchanged.
    ''' </summary>
    ''' <param name="alpha">significance level
    ''' </param>
    ''' <param name="nBootstrapSamples">The number of bootstrap samples is optional.
    ''' The default value is 2000
    ''' </param>
    ''' <param name="eps">Tolerance parameter is optional because
    ''' the minimale eps, for which the test can reject H0,
    ''' will be calculated at any case.
    ''' </param>
    ''' <returns>
    ''' The function returns the smallest epsilon for which test can reject H_0.
    ''' Additionally it returns the test result, which is true 
    ''' if the test can reject H_0 for given eps. 
    ''' </returns>
    Public Function BootstrapTestIndependence(alpha As Double,
                                             Optional nBootstrapSamples As Integer = 2000,
                                             Optional eps As Double = 0) As TestResult
        Dim rnd = New MathNet.Numerics.Random.MersenneTwister(10071977)
        Dim res = Me.AsymptTestIndependence(alpha, 0)
        Dim asympEps = res.minEps * 1.2
        Dim numExteriorPoints As Integer = (n1 + n2 + 2) * 50
        Dim exteriorPoints = getExteriorPoints(numExteriorPoints, asympEps)
        res = Me.BootstrapTestIndependence(alpha, asympEps, exteriorPoints, nBootstrapSamples, rnd)

        Dim f As Func(Of Double, Double)
        f = Function(_eps As Double) As Double
                rnd = New MathNet.Numerics.Random.MersenneTwister(10071977)
                Dim pValue = Me.BootstrapTestIndependence(alpha, _eps, exteriorPoints, nBootstrapSamples, rnd).minEps
                Return pValue - alpha
            End Function

        Try
            res.minEps = MathNet.Numerics.RootFinding.Bisection.FindRoot(f, 0, asympEps)
            res.result = (eps >= res.minEps)
        Catch
            res.minEps = 1 / 0
            res.result = False
        End Try

        Return res
    End Function

    Function getExteriorPoints(nExteriorPoints As Integer, eps As Double) As List(Of Double())
        Dim rnd = New MathNet.Numerics.Random.MersenneTwister(9102018)
        Dim exteriorPoints As New List(Of Double())
        Dim rawEps As Double = Me.rawEps(eps)

        For i As Integer = 1 To nExteriorPoints
            exteriorPoints.Add(Me.RandomOuterPoint(rawEps, (n1 + 1) * (n2 + 1), rnd))
        Next

        Return exteriorPoints
    End Function

    Sub toFile(toPath As String)
        Dim ls As New List(Of String)
        ls.Add("Rows")
        ls.Add(Join(Double2String(R)))
        ls.Add("Columns")
        ls.Add(Join(Double2String(C)))

        ls.Add("Matrix")
        For i As Integer = 0 To n1
            Dim row As New List(Of Double)
            For j As Integer = 0 To n2
                row.Add(matrix(i, j))
            Next
            ls.Add(Join(Double2String(row.ToArray)))
        Next

        IO.File.WriteAllLines(toPath, ls.ToArray)
    End Sub
    Function Double2String(v As Double()) As String()
        Dim ls As New List(Of String)
        For Each entry In v
            ls.Add(entry.ToString("N6"))
        Next
        Return ls.ToArray
    End Function
    Function l2(p() As Double, q() As Double) As Double
        Dim s As Double = 0
        For i As Integer = 0 To p.Length - 1
            s = s + (p(i) - q(i)) ^ 2
        Next
        Return s
    End Function
End Class
