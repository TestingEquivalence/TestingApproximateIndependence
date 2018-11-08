Imports System.IO

Module Research
    Function sample(vec As Double(), n As Integer, r As System.Random) As Double()
        Dim res(n - 1) As Integer
        MathNet.Numerics.Distributions.Categorical.Samples(r, res, vec)

        Dim length = vec.GetUpperBound(0)
        Dim counts(length) As Double

        For Each t In res
            counts(t) = counts(t) + 1
        Next

        Return counts
    End Function

    Function power(p As Double(,), n As Integer, nSamples As Integer,
                   alpha As Double, eps As Double,
                   bootsrap As Boolean, nBootsrapSamples As Integer,
                   table As Func(Of Double(,), ctable),
                   nDirections As Integer) As Double
        Dim cnt As Integer = 0
        Dim rootTable As ctable
        rootTable = table(p)
        Dim rnd = New MathNet.Numerics.Random.MersenneTwister(10071977)
        Dim exteriorPoints = rootTable.getExteriorPoints(nDirections, eps)

        For i As Integer = 1 To nSamples
            Dim vec = rootTable.Matrix2Vector(p)
            Dim counts = Research.sample(vec, n, rnd)
            Dim mcounts = rootTable.Vector2Matrix(counts)
            Dim simTable = table(mcounts)

            Dim res As TestResult
            If bootsrap Then
                res = simTable.BootstrapTestIndependence(alpha, eps, exteriorPoints, nBootsrapSamples, rnd)
            Else
                res = simTable.AsymptTestIndependence(alpha, eps)
            End If

            If res.result Then
                cnt = cnt + 1
            End If
        Next

        Return cnt / nSamples
    End Function

    Function power(n As Integer, samples As List(Of Double()),
                   alpha As Double, eps As Double, rootTable As ctable,
                   bootsrap As Boolean, nBootsrapSamples As Integer,
                   table As Func(Of Double(,), ctable),
                   nExteriorPoints As Integer) As Double
        Dim cnt As Integer = 0
        Dim exteriorPoints = rootTable.getExteriorPoints(nExteriorPoints, eps)
        Dim rnd = New MathNet.Numerics.Random.MersenneTwister(10071977)

        For Each counts In samples
            Dim mcounts = rootTable.Vector2Matrix(counts)
            Dim simTable = table(mcounts)
            Dim res As TestResult
            If bootsrap Then
                res = simTable.BootstrapTestIndependence(alpha, eps, exteriorPoints, nBootsrapSamples, rnd)
            Else
                res = simTable.AsymptTestIndependence(alpha, eps)
            End If

            If res.result Then
                cnt = cnt + 1
            End If
        Next

        Return cnt / samples.Count
    End Function

    Function minEps(rootTable As ctable, nSamples As Integer,
                    n As Integer, alpha As Double,
                    lb As Double, ub As Double, target As Double,
                    bootsrap As Boolean, nBootsrapSamples As Integer,
                    table As Func(Of Double(,), ctable),
                    nExPoints As Integer,
                    fileName As String) As Double
        Dim samples As New List(Of Double())
        Dim vec = rootTable.Matrix2Vector(rootTable.matrix)
        Dim rnd = New MathNet.Numerics.Random.MersenneTwister(10071977)

        For i As Integer = 1 To nSamples
            samples.Add(sample(vec, n, rnd))
        Next

        Dim f As Func(Of Double, Double)
        Dim lastEps As Double
        f = Function(eps As Double) As Double
                Dim res = power(n, samples, 0.05, eps, rootTable, bootsrap, nBootsrapSamples, table, nExPoints) - target
                'File.AppendAllLines(fileName, {Join({eps.ToString, res.ToString})})
                lastEps = eps
                Return res
            End Function

        Try
            Return MathNet.Numerics.RootFinding.Bisection.FindRoot(f, lb, ub, 0.001, 15)
        Catch ex As Exception
            Return lastEps
        End Try
    End Function
End Module
