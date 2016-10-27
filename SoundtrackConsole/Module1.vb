Imports System.IO
Imports DotNet3dsSoundtrackConverter

Module Module1

    Sub PrintUsage()
        Console.WriteLine("Usage: SoundtrackConsole.exe <SourceDirectory> <SoundtrackDefinition> <OutputDirectory>")
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
            If args.Length > 3 Then
                Dim sourceDir = args(1)
                Dim definitionFile = args(2)
                Dim outputDir = args(3)

                If Not Path.IsPathRooted(sourceDir) Then
                    sourceDir = Path.Combine(Environment.CurrentDirectory, sourceDir)
                End If

                If Not Path.IsPathRooted(definitionFile) Then
                    definitionFile = Path.Combine(Environment.CurrentDirectory, definitionFile)
                End If

                If Not Path.IsPathRooted(outputDir) Then
                    outputDir = Path.Combine(Environment.CurrentDirectory, outputDir)
                End If

                Console.WriteLine("Converting...")
                Dim c As New SoundtrackConverter
                AddHandler c.ProgressChanged, AddressOf OnProgressed
                AddHandler c.Completed, AddressOf OnComplete
                c.Convert(sourceDir, definitionFile, outputDir).Wait()
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
