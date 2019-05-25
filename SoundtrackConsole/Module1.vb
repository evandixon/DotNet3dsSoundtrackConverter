Imports System.IO
Imports DotNet3dsSoundtrackConverter
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Utilities.AsyncFor

Module Module1

    Sub PrintUsage()
        Console.WriteLine("Usage: SoundtrackConsole.exe <Source> [SoundtrackDefinition] <OutputDirectory>")
        Console.WriteLine("* Source can be either a directory containing the raw files of a 3DS ROM, or a compatible decrypted 3DS ROM.
* SoundtrackDefinition is the path to the soundtrack definition file.
* OutputDirectory is where the soundtrack files will be generated.")
    End Sub

    Sub OnProgressed(sender As Object, e As ProgressReportedEventArgs)
        Console.WriteLine("Progress: " & CInt(e.Progress * 100).ToString().PadLeft(3, "0"c) & "%")
    End Sub

    Sub OnComplete(sender As Object, e As EventArgs)
        Console.WriteLine("Complete!")
    End Sub

    Sub Main()
        Dim args = Environment.GetCommandLineArgs
        Try
            If args.Length > 2 Then
                Dim sourceDir As String
                Dim definitionFile As String
                Dim outputDir As String

                If args.Length > 3 Then
                    sourceDir = args(1)
                    definitionFile = args(2)
                    outputDir = args(3)
                Else
                    sourceDir = args(1)
                    definitionFile = Nothing
                    outputDir = args(2)
                End If

                If Not Path.IsPathRooted(sourceDir) Then
                    sourceDir = Path.Combine(Environment.CurrentDirectory, sourceDir)
                End If

                If definitionFile IsNot Nothing Then
                    If Not Path.IsPathRooted(definitionFile) Then
                        definitionFile = Path.Combine(Environment.CurrentDirectory, definitionFile)
                    End If
                End If

                If Not Path.IsPathRooted(outputDir) Then
                    outputDir = Path.Combine(Environment.CurrentDirectory, outputDir)
                End If

                Console.WriteLine("Converting...")
                Dim c As New SoundtrackConverter
                AddHandler c.ProgressChanged, AddressOf OnProgressed
                AddHandler c.Completed, AddressOf OnComplete

                If definitionFile Is Nothing Then
                    c.Convert(sourceDir, outputDir).Wait()
                Else
                    c.Convert(sourceDir, definitionFile, outputDir).Wait()
                End If

                RemoveHandler c.ProgressChanged, AddressOf OnProgressed
                RemoveHandler c.Completed, AddressOf OnComplete
            Else
                PrintUsage()
            End If
        Catch ex As Exception
            Console.WriteLine(ex.ToString)
#If DEBUG Then
            Debugger.Break()
#End If
        End Try
    End Sub

End Module
