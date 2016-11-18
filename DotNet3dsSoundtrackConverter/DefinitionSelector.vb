Imports System.Text.RegularExpressions
''' <summary>
''' Selects the <see cref="SoundtrackDefinition"/> for a given game.
''' </summary>
Public Class DefinitionSelector

    Public Sub New()
        Dim definitions As New List(Of SoundtrackDefinition)

        definitions.Add(SoundtrackDefinition.FromSoundtrackDefinitionContents(My.Resources.SoundtrackDefinitions.GTI))
        definitions.Add(SoundtrackDefinition.FromSoundtrackDefinitionContents(My.Resources.SoundtrackDefinitions.PSMD))

        Soundtracks = definitions.ToArray
    End Sub

    Public ReadOnly Property Soundtracks As SoundtrackDefinition()

    ''' <summary>
    ''' Selects the <see cref="SoundtrackDefinition"/> for a given game.
    ''' </summary>
    ''' <param name="rom">NDS or decrypted 3DS ROM (either a file or a directory) for which to get the soundtrack.</param>
    ''' <returns>The<see cref="SoundtrackDefinition"/> for the given game, or null if there is no soundtrack for the given game.</returns>
    Public Async Function SelectSoundtrackDefinition(rom As String) As Task(Of SoundtrackDefinition)
        Dim gameId = Await DotNet3dsToolkit.MetadataReader.GetGameID(rom)
        Return Soundtracks.Where(Function(definition) (New Regex(definition.GameID)).IsMatch(gameId)).FirstOrDefault
    End Function

    ''' <summary>
    ''' Determines if a <see cref="SoundtrackDefinition"/> exists for the given game.
    ''' </summary>
    ''' <param name="rom">NDS or decrypted 3DS ROM (either a file or a directory) for which to get the soundtrack.</param>
    ''' <returns>A boolean indicating whether or not a <see cref="SoundtrackDefinition"/> exists for the given game.</returns>
    Public Async Function SoundtrackDefinitionExists(rom As String) As Task(Of Boolean)
        Return (Await SelectSoundtrackDefinition(rom)) IsNot Nothing
    End Function
End Class
