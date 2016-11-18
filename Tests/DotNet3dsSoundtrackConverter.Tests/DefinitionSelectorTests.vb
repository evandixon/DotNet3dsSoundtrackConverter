Imports System.Text
Imports DotNet3dsSoundtrackConverter
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass> Public Class DefinitionSelectorTests

    <TestMethod> Public Sub EnsureCorrectDefinitionFormatting()
        Dim selector As New DefinitionSelector
        'The real test here is to ensure no exceptions are thrown in the constructor.
        'If exceptions are thrown, then one of the definitions is incorrectly formatted and will result in test failure.
        Assert.IsNotNull(selector)
    End Sub

End Class