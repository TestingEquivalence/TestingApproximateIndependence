
This package provides two new equivalence tests for approximate independence in two-way contingency tables.
The package is based on the article:

Vladimir Ostrovski "New Equivalence Tests for Approximate Independence in Contingency Tables".
Stats 2019, 2, 239–246; doi:10.3390/stats2020018
http://www.mdpi.com/journal/stats

The package is written in VB.NET. Tree examples are available in the module "StartMod.vb", which can be run immediately.

The goal is to show that two discrete random variables are approximately independent distributed. 
The approximate independence can be important for some applications or also greatly simplify calculations. 

The approximate independence can be established  using relative or absolute deviation of the given contingency table
and the product measure of the marginal distributions.
The tests for absolute deviation are implemented in class “AbsChangeTest“ 
and the tests for relative deviation are in the class “RelChangeTest”. 
The classes should be initialized  as “AbsChangeTest(table)“ or “RelChangeTest(table)”,
where table should be an integer matrix containing the contingency table, see also examples. 

Each class provides asymptotic and  bootstrap based tests, 
which are implemented as the class methods “AsymptTestIndependence(alpha)“ and “BootstrapTestIndependence(alpha, nOfBootstrapSamples)” respectively. 
The parameter alpha is the significance level and nOfBootstrapSamples is the number of Bootstrap samples. 
Further information on the function can be found in the program comments. 

Both methods return the class  “TestResult”, which contains the following two fields:
- minEps is the minimum tolerance parameter, for which test can show approximate equivalence at the significance level alpha,
- result is true or false, if the test can reject H0. This field is relevant if the optional tolerance parameter eps was provided to the test.

The asymptotic test is based on the asymptotic distribution of the test statistic. 
Therefore the asymptotic test need some sufficiently large number of the observations. 
It should be used carefully because the test is approximate and may be anti conservative at some points. 
In order to obtain a conservative test reducing of alpha  (usually halving) or 
slight shrinkage of the tolerance parameter eps may be appropriate. 

The bootstrap test is based on the re-sampling method called bootstrap. 
The bootstrap test is more precise and reliable than the asymptotic test. 
However, it should be used carefully because the test is approximate and may be anti conservative at some points. 
In order to obtain a conservative test reducing of alpha  (usually halving) or 
slight shrinkage of the tolerance parameter e may be appropriate. 
We prefer the slight shrinkage of the tolerance parameter 
because it is more effective and the significance level remains unchanged. 
The bootstrap test needs considerable computation time. For example, it may need few minutes on the usual office computer.