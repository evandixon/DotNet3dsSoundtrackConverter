Imports System.Text
Imports DotNet3dsSoundtrackConverter
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass> Public Class DefinitionSelectorTests

    <TestMethod> Public Sub EnsureCorrectDefinitionFormatting()
        Dim selector As New DefinitionSelector

        For Each item In selector.Soundtracks
            Assert.IsNotNull(item.System, "System should not be null.")
            Assert.IsNotNull(item.GameID, "GameID should not be null.")
            Assert.IsNotNull(item.AlbumName, "AlbumName should not be null.")
            Assert.IsNotNull(item.AlbumArtist, "AlbumArtist should not be null.")
            Assert.IsNotNull(item.Year, "Year should not be null.")
            Assert.IsNotNull(item.SourcePath, "SourcePath should not be null.")
            Assert.IsNotNull(item.OriginalExtension, "OriginalExtension should not be null.")
            Assert.IsTrue(item.Tracks.Any, "There should be at least one track in the file.")
            Assert.IsTrue(item.MaxTrackNumber > 0, "Max track number should be greater than 0.")
        Next
    End Sub

End Class