Imports System.Threading
Imports System.Threading.Tasks
Imports System.IO
Imports System.Collections.Concurrent

Module SimMod
    Sub sim()
        Dim size As New List(Of Integer())
        size.Add({2, 4})
        size.Add({3, 3})
        size.Add({3, 4})
        size.Add({4, 4})
        size.Add({4, 5})
        size.Add({5, 5})



        'RealDataSetsToFile()

        'For Each n In {100, 200, 500, 1000, 2000, 5000, 10000}
        '    Dim n1 As Integer = 5
        '    Dim n2 As Integer = 5
        '    Dim nDir As Integer
        '    nDir = (n1 + n2) * 50
        '    StartMod.PowerAtUniformProducts(n1, n2, n, True, 2000, Function(matrix As Double(,))
        '                                                               Return New RelChangeTest(matrix)
        '                                                           End Function, nDir)
        'Next

        'For Each s In size
        '    Dim n As Integer = (s(0) + s(1)) * 100
        '    Dim eps = 0.2
        '    Dim alpha As Double = 0.05
        '    Dim adjFaktor As Double = 1
        '    PowerAtProducts(s(0), s(1), n, eps * adjFaktor, alpha, False, 0, Function(matrix As Double(,))
        '                                                                         Return New AbsChangeTest(matrix)
        '                                                                     End Function)
        'Next

        'For Each s In size
        '    Dim n As Integer = (s(0) + s(1)) * 100
        '    Dim eps = 0.2
        '    Dim alpha As Double = 0.05
        '    Dim adjFaktor As Double = 1
        '    Dim nDir = (s(0) + s(1)) * 50
        '    PowerAtBondary(s(0), s(1), n, eps, alpha, True, 2000, Function(matrix As Double(,))
        '                                                              Return New RelChangeTest(matrix)
        '                                                          End Function, adjFaktor)
        'Next



        'For Each s In size
        '    Dim n As Integer = (s(0) + s(1)) * 100
        '    Dim n1 = s(0)
        '    Dim n2 = s(1)
        '    Dim nDir = (n1 + n2) * 50
        '    Dim eps As Double = 0.2
        '    Dim af As Double = 1 * 0.9
        '    Dim alpha As Double = 0.05 ' / 2
        '    ParallelPowerAtBondary(n1, n2, n, eps, alpha, True, 2000, Function(matrix As Double(,))
        '                                                                  Return New AbsChangeTest(matrix)
        '                                                              End Function, af, nDir)
        'Next

        For Each s In size
            Dim n As Integer = (s(0) + s(1)) * 100
            Dim n1 = s(0)
            Dim n2 = s(1)
            Dim nDir = (n1 + n2) * 50
            Dim eps As Double = 0.2
            Dim af As Double = 1 * 0.9
            Dim alpha As Double = 0.05 ' / 2
            ParallelPowerAtProducts(n1, n2, n, eps * af, alpha, True, 2000, Function(matrix As Double(,))
                                                                                Return New RelChangeTest(matrix)
                                                                            End Function, nDir)
        Next

    End Sub

    Sub PowerAtUniformProducts(n1 As Integer, n2 As Integer, n As Integer,
                               bootsrap As Boolean, nBootsrapSamples As Integer,
                               table As Func(Of Double(,), ctable),
                               nExteriorPoints As Integer)
        Dim alpha As Double = 0.05
        Dim minPower As Double = 0.9
        Dim nSamples = 10000
        Dim fileName = "result.txt"
        Dim rootTable As ctable = New AbsChangeTest(n1, n2)
        rootTable = table(rootTable.matrix)

        Dim minEps = Research.minEps(rootTable, nSamples, n, alpha, 0, 1,
            minPower, bootsrap, nBootsrapSamples, table, nExteriorPoints, fileName)
        Dim str = Join({n.ToString, minEps.ToString})
        File.AppendAllLines(fileName, {str})
    End Sub
    Sub PowerAtBondary(n1 As Integer, n2 As Integer, n As Integer, eps As Double, alpha As Double,
                       bootsrap As Boolean, nBootsrapSamples As Integer,
                       table As Func(Of Double(,), ctable),
                       Optional adjustFactor As Double = 1,
                       Optional nPoints As Integer = 100,
                       Optional nDirections As Integer = 100)
        Dim nSamples = 10000
        Dim rnd = New MathNet.Numerics.Random.MersenneTwister(10071977)
        Dim rootTable As ctable
        rootTable = New RelChangeTest(n1, n2)
        rootTable = table(rootTable.matrix)

        Dim rawEps = eps * eps * n1 * n2
        Dim fileName As String
        fileName = "boundary " + n1.ToString + "x" + n2.ToString + ".txt"


        For m As Integer = 1 To nPoints
            Dim point = rootTable.RandomLinBoundary(rawEps, n1 * n2, rnd)
            Dim mpoint = rootTable.Vector2Matrix(point)
            Dim distance = rootTable.distance()
            Dim power = Research.power(mpoint, n, nSamples, alpha, eps * adjustFactor,
                        bootsrap, nBootsrapSamples, table, nDirections)

            Dim str = Join({power.ToString})
            File.AppendAllLines(fileName, {str})
        Next
    End Sub


    Sub PowerAtProducts(n1 As Integer, n2 As Integer, n As Integer, eps As Double, alpha As Double,
                        bootsrap As Boolean, nBootsrapSamples As Integer,
                        table As Func(Of Double(,), ctable),
                        Optional nDirections As Integer = 100)

        Dim nSamples = 10000
        Dim nPoints As Integer = 100
        Dim rnd = New MathNet.Numerics.Random.MersenneTwister(10071977)
        Dim fileName As String
        fileName = "product " + n1.ToString + "x" + n2.ToString + ".txt"

        Dim rootTable As ctable = New RelChangeTest(n1, n2)
        rootTable = table(rootTable.matrix)

        For m As Integer = 1 To nPoints
            Dim v = ctable.RandomPoint(n1 * n2, rnd)
            rootTable = rootTable.startPoint(v)
            Dim distance = rootTable.distance()
            Dim power = Research.power(rootTable.matrix, n, nSamples, alpha, eps,
                                       bootsrap, nBootsrapSamples, table, nDirections)

            Dim str = Join({power.ToString})
            File.AppendAllLines(fileName, {str})
        Next
    End Sub

    Function NitrenData() As Double(,)
        Dim vec As Double() = {9, 13, 13, 48, 24, 18, 20, 72}
        Dim table = New AbsChangeTest(2, 4)
        Return table.Vector2Matrix(vec)
    End Function
    Function EyeHairData() As Double(,)
        Dim vec As Double() = {68, 119, 26, 7,
                               20, 84, 17, 94,
                               15, 54, 14, 10,
                                5, 29, 14, 16}
        Dim table = New AbsChangeTest(4, 4)
        Return table.Vector2Matrix(vec)
    End Function

    Function ChildrenIncomeData1() As Double(,)
        Dim vec As Double() = {2161, 3577, 2184, 1636,
                               2755, 5081, 2222, 1052,
                                936, 1753, 640, 306,
                                225, 419, 96, 38,
                                 39, 98, 31, 14}
        Dim table = New AbsChangeTest(5, 4)
        Dim res = table.Vector2Matrix(vec)
        Return res
    End Function

    Function ChildrenIncomeData2() As Double(,)
        Dim vec As Double() = {2161, 3577, 2184, 1636,
                               2755, 5081, 2222, 1052,
                                936, 1753, 640, 306,
                                225 + 39, 419 + 98, 96 + 31, 38 + 14}
        Dim table = New AbsChangeTest(4, 4)
        Return table.Vector2Matrix(vec)
    End Function

    Function ChildrenIncomeData3() As Double(,)
        Dim vec As Double() = {2161, 3577, 2184, 1636,
                               2755, 5081, 2222, 1052,
                                936 + 225 + 39, 1753 + 419 + 98, 640 + 96 + 31, 306 + 38 + 14}
        Dim table = New AbsChangeTest(3, 4)
        Dim res = table.Vector2Matrix(vec)
        Return res
    End Function

    Sub RealDataSets()
        Dim table As ctable
        Dim res As TestResult

        Console.WriteLine("Minimum tolerance parameter, for which test reject non-equivalence")
        Console.WriteLine("at the significance level 0.05")
        Console.WriteLine("---------------------------------------------")
        Console.WriteLine("Nitren Data Set")
        Console.WriteLine("---------------------------------------------")

        table = New RelChangeTest(NitrenData)
        res = table.AsymptTestIndependence(0.05, 0.2)
        Console.WriteLine("relative deviation, asymptotic test:")
        Console.WriteLine(res.minEps)


        Console.WriteLine("---------------------------------------------")
        Console.WriteLine("Eye Hair Data Set")
        Console.WriteLine("---------------------------------------------")

        table = New RelChangeTest(EyeHairData)
        res = table.AsymptTestIndependence(0.05, 0.2)
        Console.WriteLine("relative deviation, asymptotic test:")
        Console.WriteLine(res.minEps)

        Console.WriteLine("---------------------------------------------")
        Console.WriteLine("Children Income Data Set")
        Console.WriteLine("---------------------------------------------")

        table = New RelChangeTest(ChildrenIncomeData1)
        res = table.AsymptTestIndependence(0.05, 0.2)
        Console.WriteLine("relative deviation, asymptotic test:")
        Console.WriteLine(res.minEps)

        Console.ReadKey()
    End Sub

    Sub RealDataSetsToFile()
        Dim table As ctable
        Dim res As TestResult
        Dim fileName As String = "rw_data_sets.txt"
        Dim ls As New List(Of String)
        Dim asymptRelChange As String = "asymptotic test for average relative change:"
        Dim bstRelChange As String = "bootstrap test for average relative change:"
        Dim asymptAbsChange As String = "asymptotic test for average absolute change:"
        Dim bstAbsChange As String = "bootstrap test for average absolute change:"

        ls.Add("-----------------------------------------------")
        ls.Add("nitren")
        table = New RelChangeTest(NitrenData)
        res = table.AsymptTestIndependence(0.05, 0.4)
        ls.Add(asymptRelChange)
        ls.Add(res.minEps.ToString())
        res = table.BootstrapTestIndependence(0.05, 2000)
        ls.Add(bstRelChange)
        ls.Add(res.minEps.ToString())
        table = New AbsChangeTest(NitrenData)
        res = table.AsymptTestIndependence(0.05, 0.03)
        ls.Add(asymptAbsChange)
        ls.Add(res.minEps.ToString())
        res = table.BootstrapTestIndependence(0.05, 2000)
        ls.Add(bstAbsChange)
        ls.Add(res.minEps.ToString())


        ls.Add("-----------------------------------------------")
        ls.Add("EyeHairIncome")
        table = New RelChangeTest(EyeHairData)
        res = table.AsymptTestIndependence(0.05, 0.6)
        ls.Add(asymptRelChange)
        ls.Add(res.minEps.ToString())
        res = table.BootstrapTestIndependence(0.05, 2000)
        ls.Add(bstRelChange)
        ls.Add(res.minEps.ToString())
        table = New AbsChangeTest(EyeHairData)
        res = table.AsymptTestIndependence(0.05, 0.04)
        ls.Add(asymptAbsChange)
        ls.Add(res.minEps.ToString())
        res = table.BootstrapTestIndependence(0.05, 2000)
        ls.Add(bstAbsChange)
        ls.Add(res.minEps.ToString())

        ls.Add("-----------------------------------------------")
        ls.Add("ChildrenIncome 1")
        table = New RelChangeTest(ChildrenIncomeData1)
        res = table.AsymptTestIndependence(0.05, 0.4)
        ls.Add(asymptRelChange)
        ls.Add(res.minEps.ToString())
        res = table.BootstrapTestIndependence(0.05, 2000)
        ls.Add(bstRelChange)
        ls.Add(res.minEps.ToString())
        table = New AbsChangeTest(ChildrenIncomeData1)
        res = table.AsymptTestIndependence(0.05, 0.01)
        ls.Add(asymptAbsChange)
        ls.Add(res.minEps.ToString())
        res = table.BootstrapTestIndependence(0.05, 2000)
        ls.Add(bstAbsChange)
        ls.Add(res.minEps.ToString())

        ls.Add("-----------------------------------------------")
        ls.Add("ChildrenIncomene 2")
        table = New RelChangeTest(ChildrenIncomeData2)
        res = table.AsymptTestIndependence(0.05, 0.4)
        ls.Add(asymptRelChange)
        ls.Add(res.minEps.ToString())
        res = table.BootstrapTestIndependence(0.05, 2000)
        ls.Add(bstRelChange)
        ls.Add(res.minEps.ToString())
        table = New AbsChangeTest(ChildrenIncomeData2)
        res = table.AsymptTestIndependence(0.05, 0.01)
        ls.Add(asymptAbsChange)
        ls.Add(res.minEps.ToString())
        res = table.BootstrapTestIndependence(0.05, 2000)
        ls.Add(bstAbsChange)
        ls.Add(res.minEps.ToString())

        ls.Add("-----------------------------------------------")
        ls.Add("ChildrenIncomene 3")
        table = New RelChangeTest(ChildrenIncomeData3)
        res = table.AsymptTestIndependence(0.05, 0.4)
        ls.Add(asymptRelChange)
        ls.Add(res.minEps.ToString())
        res = table.BootstrapTestIndependence(0.05, 2000)
        ls.Add(bstRelChange)
        ls.Add(res.minEps.ToString())
        table = New AbsChangeTest(ChildrenIncomeData3)
        res = table.AsymptTestIndependence(0.05, 0.01)
        ls.Add(asymptAbsChange)
        ls.Add(res.minEps.ToString())
        res = table.BootstrapTestIndependence(0.05, 2000)
        ls.Add(bstAbsChange)
        ls.Add(res.minEps.ToString())


        File.AppendAllLines(fileName, ls)
    End Sub


    Sub ParallelPowerAtBondary(n1 As Integer, n2 As Integer, n As Integer, eps As Double, alpha As Double,
                       bootsrap As Boolean, nBootsrapSamples As Integer,
                       table As Func(Of Double(,), ctable),
                       Optional adjustFactor As Double = 1,
                       Optional nDirections As Integer = 100)
        Dim nSamples = 10000
        Dim rnd = New MathNet.Numerics.Random.MersenneTwister(10071977)
        Dim rootTable As ctable
        rootTable = New RelChangeTest(n1, n2)
        rootTable = table(rootTable.matrix)
        Dim rawEps = rootTable.rawEps(eps)
        Dim fileName As String
        Dim nPoints As Integer = 100

        fileName = "boundary " + n1.ToString + "x" + n2.ToString + ".txt"


        'generate random boundary elements
        Dim ls As New List(Of Double(,))
        For m As Integer = 1 To nPoints
            Dim point = rootTable.RandomLinBoundary(rawEps, n1 * n2, rnd)
            Dim mpoint = rootTable.Vector2Matrix(point)
            ls.Add(mpoint)
        Next

        Parallel.ForEach(ls, Sub(mpoint)
                                 Dim power = Research.power(mpoint, n, nSamples, alpha, eps * adjustFactor,
                                             bootsrap, nBootsrapSamples, table, nDirections)

                                 Dim str = Join({power.ToString})
                                 File.AppendAllLines(fileName, {str})
                             End Sub)
    End Sub

    Sub ParallelPowerAtProducts(n1 As Integer, n2 As Integer, n As Integer, eps As Double, alpha As Double,
                        bootsrap As Boolean, nBootsrapSamples As Integer,
                        table As Func(Of Double(,), ctable),
                        Optional nDirections As Integer = 100)

        Dim nSamples = 10000
        Dim nPoints As Integer = 100
        Dim rnd = New MathNet.Numerics.Random.MersenneTwister(10071977)
        Dim fileName As String
        fileName = "product " + n1.ToString + "x" + n2.ToString + ".txt"

        Dim rootTable As ctable = New RelChangeTest(n1, n2)
        rootTable = table(rootTable.matrix)
        Dim points As New List(Of Double(,))

        For m As Integer = 1 To nPoints
            Dim v = RelChangeTest.RandomPoint(n1 * n2, rnd)
            rootTable = rootTable.startPoint(v)
            points.Add(rootTable.matrix)
        Next

        Parallel.ForEach(points, Sub(p)
                                     Dim power = Research.power(p, n, nSamples, alpha, eps, bootsrap,
                                     nBootsrapSamples, table, nDirections)
                                     Dim str = Join({power.ToString})
                                     File.AppendAllLines(fileName, {str})
                                 End Sub)
    End Sub

End Module
