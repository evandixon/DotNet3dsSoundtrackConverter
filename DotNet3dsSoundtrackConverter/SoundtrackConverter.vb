Imports MediaToolkit
Imports MediaToolkit.Model
Imports MediaToolkit.Options
Imports System.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Utilities.AsyncFor

Public Class SoundtrackConverter
    Implements IReportProgress

#Region "IReportProgress Support"
    Public Property IsCompleted As Boolean Implements IReportProgress.IsCompleted
        Get
            Return _isCompleted
        End Get
        Private Set
            If Not _isCompleted = Value Then
                _isCompleted = Value
                If _isCompleted Then
                    RaiseEvent Completed(Me, New EventArgs)
                End If
            End If
        End Set
    End Property
    Dim _isCompleted As Boolean

    Public Property IsIndeterminate As Boolean Implements IReportProgress.IsIndeterminate
        Get
            Return _isIndeterminate
        End Get
        Private Set
            If Not _isIndeterminate = Value Then
                _isIndeterminate = Value
                UpdateLoadingStatus()
            End If
        End Set
    End Property
    Dim _isIndeterminate As Boolean

    Public Property Message As String Implements IReportProgress.Message
        Get
            Return _message
        End Get
        Private Set
            If Not _message = Value Then
                _message = Value
                UpdateLoadingStatus()
            End If
        End Set
    End Property
    Dim _message As String

    Public Property Progress As Single Implements IReportProgress.Progress
        Get
            Return _progress
        End Get
        Private Set
            If Not _progress = Value Then
                _progress = Value
                UpdateLoadingStatus()
            End If
        End Set
    End Property
    Dim _progress As Single

    Private Sub UpdateLoadingStatus()
        RaiseEvent ProgressChanged(Me, New ProgressReportedEventArgs With {.IsIndeterminate = IsIndeterminate, .Message = Message, .Progress = Progress})
    End Sub

    Public Event Completed(sender As Object, e As EventArgs) Implements IReportProgress.Completed
    Public Event ProgressChanged(sender As Object, e As ProgressReportedEventArgs) Implements IReportProgress.ProgressChanged
#End Region

    Public Async Function CanConvert(source As String) As Task(Of Boolean)
        Dim selector As New DefinitionSelector
        Return Await selector.SoundtrackDefinitionExists(source)
    End Function

    Public Async Function Convert(source As String, soundtrackDefinitionFilename As String, outputDirectory As String) As Task
        Dim definition = SoundtrackDefinition.FromSoundtrackDefinitionFile(soundtrackDefinitionFilename)
        Await Convert(source, definition, outputDirectory)
    End Function

    Public Async Function Convert(source As String, outputDirectory As String) As Task
        Dim selector As New DefinitionSelector
        Await Convert(source, Await selector.SelectSoundtrackDefinition(source), outputDirectory)
    End Function

    Public Async Function Convert(source As String, soundtrackDefinition As SoundtrackDefinition, outputDirectory As String) As Task
        Using external As New ExternalProgramManager
            Dim sourceDir As String
            If Directory.Exists(source) Then
                'If source is a directory, no additional action needs to be taken
                sourceDir = Path.Combine(source, soundtrackDefinition.SourcePath)
            ElseIf File.Exists(source) Then
                'If source is a file, we must first extract the file
                Dim extractDir = Path.Combine(external.CurrentToolsDir, "extractTemp")
                sourceDir = Path.Combine(extractDir, soundtrackDefinition.SourcePath)
                Using c As New DotNet3dsToolkit.Converter
                    Message = My.Resources.Language.LoadingUnpacking
                    Progress = 0
                    IsCompleted = False
                    IsIndeterminate = False

                    Await c.ExtractAuto(source, extractDir)
                End Using
            Else
                Throw New IOException(String.Format(My.Resources.Language.ErrorCantFindSource, source))
            End If

            If Not Directory.Exists(outputDirectory) Then
                Directory.CreateDirectory(outputDirectory)
            End If

            For Each item In IO.Directory.GetFiles(outputDirectory)
                File.Delete(item)
            Next

            Me.Message = My.Resources.Language.LoadingConvertingSoundtrack
            Me.Progress = 0
            Me.IsCompleted = False
            Me.IsIndeterminate = False

            'Start conversion
            Dim ffmpegPath = Path.Combine(external.CurrentToolsDir, "ffmpeg.exe")

            Dim f As New AsyncFor
            f.BatchSize = Environment.ProcessorCount * 2
            AddHandler f.ProgressChanged, Sub(sender As Object, e As ProgressReportedEventArgs)
                                              Me.Message = My.Resources.Language.LoadingConvertingSoundtrack
                                              Me.Progress = e.Progress
                                          End Sub
            Await f.RunForEach(soundtrackDefinition.Tracks,
                               Async Function(item As SoundtrackTrack) As Task
                                   Dim sourceFile = IO.Path.Combine(sourceDir, item.OriginalName) & "." & soundtrackDefinition.OriginalExtension
                                   Dim destinationWav As String = sourceFile.
                                                                    Replace(sourceDir, outputDirectory).
                                                                    Replace(soundtrackDefinition.OriginalExtension, "wav").
                                                                    Replace(item.OriginalName, item.GetFilename(soundtrackDefinition.MaxTrackNumber))

                                   'Remove bad characters
                                   For Each c In "!?,".ToCharArray
                                       destinationWav = destinationWav.Replace(c, "")
                                   Next
                                   destinationWav = destinationWav.Replace("é", "e")

                                   Dim destinationMp3 = destinationWav.Replace(".wav", ".mp3")

                                   'Create the wav file
                                   Await external.RunVGMStream(sourceFile, destinationWav)

                                   'Check to see if the conversion completed successfully
                                   If IO.File.Exists(destinationWav) Then
                                       'Convert to mp3
                                       Using e As New Engine(ffmpegPath, True)
                                           Dim wav As New MediaFile(destinationWav)
                                           Dim mp3 As New MediaFile(destinationMp3)
                                           Dim options = New ConversionOptions
                                           options.AudioSampleRate = AudioSampleRate.Hz48000
                                           e.Convert(wav, mp3, options)
                                       End Using

                                       IO.File.Delete(destinationWav)

                                       'Add the tag
                                       Dim t As New TagLib.Mpeg.AudioFile(destinationMp3)
                                       With t.Tag
                                           .Album = soundtrackDefinition.AlbumName
                                           .AlbumArtists = {soundtrackDefinition.AlbumArtist}
                                           .Title = item.TrackName
                                           .Track = item.TrackNumber
                                           .Year = soundtrackDefinition.Year
#Disable Warning
                                           'Disabling warning because this tag needs to be set to ensure compatibility, like with Windows Explorer and Windows Media Player.
                                           .Artists = {soundtrackDefinition.AlbumArtist}
#Enable Warning
                                       End With
                                       t.Save()
                                   Else
                                       'Todo: log error somehow
                                   End If
                               End Function)
        End Using
        IsCompleted = True
    End Function

End Class
