Imports System.IO

Public Class SoundtrackDefinition

    Public Shared Function FromSoundtrackDefinitionLines(lines As IEnumerable(Of String)) As SoundtrackDefinition
        Dim out As New SoundtrackDefinition
        Dim isHeaderMode = True

        For count = 0 To lines.Count - 1
            Dim parts = lines(count).Split("=".ToCharArray, 2)
            If isHeaderMode Then
                Select Case parts(0).ToLower
                    Case "system"
                        out.System = parts(1)
                    Case "gameid"
                        out.GameID = parts(1)
                    Case "album"
                        out.AlbumName = parts(1)
                    Case "artist"
                        out.AlbumArtist = parts(1)
                    Case "year"
                        out.Year = parts(1)
                    Case "path"
                        out.SourcePath = parts(1)
                    Case "extension"
                        out.OriginalExtension = parts(1)
                    Case "end"
                        isHeaderMode = False
                    Case Else
                        'Skip the line, look for "end" to exit
                End Select
            Else
                If parts.Count = 2 Then
                    out.Tracks.Add(New SoundtrackTrack(parts(0), parts(1)))
                End If
            End If
        Next
        Return out
    End Function

    Public Shared Function FromSoundtrackDefinitionFile(filename As String) As SoundtrackDefinition
        Dim contents = File.ReadAllLines(filename)
        Return FromSoundtrackDefinitionLines(contents)
    End Function

    Public Shared Function FromSoundtrackDefinitionContents(contents As String) As SoundtrackDefinition
        Return FromSoundtrackDefinitionLines(contents.Split(vbLf).Select(Function(line) line.Trim))
    End Function

    Public Sub New()
        Tracks = New List(Of SoundtrackTrack)
    End Sub

    ''' <summary>
    ''' The system the soundtrack is for
    ''' </summary>
    Public Property System As String

    ''' <summary>
    ''' A regular expression that matches the ID of a game.
    ''' </summary>
    Public Property GameID As String

    ''' <summary>
    ''' Name of the soundtrack's Album
    ''' </summary>
    Public Property AlbumName As String

    ''' <summary>
    ''' The soundtrack's Album Artist
    ''' </summary>
    Public Property AlbumArtist As String

    ''' <summary>
    ''' The soundtrack's year
    ''' </summary>
    Public Property Year As UInteger

    ''' <summary>
    ''' The tracks in the soundtrack
    ''' </summary>
    Public Property Tracks As List(Of SoundtrackTrack)

    ''' <summary>
    ''' The file extension of the original, unconverted file.  Should not start with a dot.
    ''' </summary>
    Public Property OriginalExtension As String

    ''' <summary>
    ''' Path in the ROM of the soundtrack files, relative to the root of the ROM filesystem.
    ''' </summary>
    ''' <returns></returns>
    Public Property SourcePath As String

    ''' <summary>
    ''' The highest track number
    ''' </summary>
    Public Overridable ReadOnly Property MaxTrackNumber As Integer
        Get
            Return Tracks.Select(Function(x) x.TrackNumber).Max
        End Get
    End Property

End Class
