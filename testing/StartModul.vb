Module StartModul
    Sub main()
        'This project contains the asymptotic and bootstrap tests,
        'which are developed in the preprint "Testing approximate independence in contingency tables".
        'The real data examples, which are considered in the preprint, are presented below.
        'The d_a based tests are implemented in class "AbsChangeTest"
        'and the d_r based tests are implemented in class "RelChangeTest".
        'Both classes should be initialized with a contingence table,
        'which entries should be event counts. 
        'The number of all events n will be derived from that table.
        'The asymptotic test is then the class method AsymptTestIndependence. 
        'The bootstrap test is the class method BootstrapTestIndependence.

        Dim table As ctable
        Dim res As TestResult
        Dim matrix As Double(,)
        Dim alpha As Double = 0.05
        Dim nOfBootstrapSamples As Integer = 2000

        Console.WriteLine("The minimum tolerance parameter, for which tests can show approximate equivalence")
        Console.WriteLine("at the significance level 0.05.")
        Console.WriteLine("---------------------------------------------")
        Console.WriteLine("---------------------------------------------")

        'Nitren data set:
        Console.WriteLine("Nitren Data Set")
        Console.WriteLine("---------------------------------------------")
        matrix = {{9, 13, 13, 48},
                  {24, 18, 20, 72}}

        table = New RelChangeTest(matrix)
        Console.WriteLine("Asymptotic test for relative distance:")
        res = table.AsymptTestIndependence(alpha)
        Console.WriteLine(res.minEps)

        Console.WriteLine("Bootstrap test for relative distance:")
        res = table.BootstrapTestIndependence(alpha, nOfBootstrapSamples)
        Console.WriteLine(res.minEps)

        table = New AbsChangeTest(matrix)
        Console.WriteLine("Asymptotic test for absolute distance:")
        res = table.AsymptTestIndependence(alpha)
        Console.WriteLine(res.minEps)

        Console.WriteLine("Bootstrap test for absolute distance:")
        res = table.BootstrapTestIndependence(alpha, nOfBootstrapSamples)
        Console.WriteLine(res.minEps)


        'Eye color and Hair color data set:
        Console.WriteLine("---------------------------------------------")
        Console.WriteLine("Eye Color and Hair Color Data Set")
        Console.WriteLine("---------------------------------------------")

        matrix = {{68, 119, 26, 7},
                  {20, 84, 17, 94},
                  {15, 54, 14, 10},
                  {5, 29, 14, 16}}

        table = New RelChangeTest(matrix)
        Console.WriteLine("Asymptotic test for relative distance:")
        res = table.AsymptTestIndependence(alpha)
        Console.WriteLine(res.minEps)

        Console.WriteLine("Bootstrap test for relative distance:")
        res = table.BootstrapTestIndependence(alpha, nOfBootstrapSamples)
        Console.WriteLine(res.minEps)

        table = New AbsChangeTest(matrix)
        Console.WriteLine("Asymptotic test for absolute distance:")
        res = table.AsymptTestIndependence(alpha)
        Console.WriteLine(res.minEps)

        Console.WriteLine("Bootstrap test for absolute distance:")
        res = table.BootstrapTestIndependence(alpha, nOfBootstrapSamples)
        Console.WriteLine(res.minEps)

        'Children 
        Console.WriteLine("---------------------------------------------")
        Console.WriteLine("Children Number and Income Data Set")
        Console.WriteLine("---------------------------------------------")

        matrix = {{2161, 3577, 2184, 1636},
                  {2755, 5081, 2222, 1052},
                  {936, 1753, 640, 306},
                  {225, 419, 96, 38},
                  {39, 98, 31, 14}}

        table = New RelChangeTest(matrix)
        Console.WriteLine("Asymptotic test for relative distance:")
        res = table.AsymptTestIndependence(alpha)
        Console.WriteLine(res.minEps)

        Console.WriteLine("Bootstrap test for relative distance:")
        Console.WriteLine("Please, wait a minute.")
        res = table.BootstrapTestIndependence(alpha, nOfBootstrapSamples)
        Console.WriteLine(res.minEps)

        table = New AbsChangeTest(matrix)
        Console.WriteLine("Asymptotic test for absolute distance:")
        res = table.AsymptTestIndependence(alpha)
        Console.WriteLine(res.minEps)

        Console.WriteLine("Bootstrap test for absolute distance:")
        Console.WriteLine("Please, wait a minute.")
        res = table.BootstrapTestIndependence(alpha, nOfBootstrapSamples)
        Console.WriteLine(res.minEps)


        Console.ReadKey()
    End Sub
End Module
