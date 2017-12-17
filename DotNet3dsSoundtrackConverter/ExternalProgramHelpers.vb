Imports System.IO
Imports SkyEditor.Core.IO

Friend Class ExternalProgramManager
    Implements IDisposable

    Public Sub New()
        ResetToolsDir()
    End Sub

    Public Event ConsoleOutputReceived(sender As Object, e As DataReceivedEventArgs)

    ''' <summary>
    ''' Whether or not to forward console output of child processes to the current process.
    ''' </summary>
    ''' <returns></returns>
    Public Property OutputConsoleOutput As Boolean = True
    Public Property CurrentToolsDir As String
    Public Property VgmStreamPath As String

    Private Async Function RunProgram(program As String, arguments As String) As Task
        Dim handlersRegistered As Boolean = False

        Dim p As New Process
        p.StartInfo.FileName = program
        p.StartInfo.WorkingDirectory = Path.GetDirectoryName(program)
        p.StartInfo.Arguments = arguments
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.StartInfo.CreateNoWindow = True
        p.StartInfo.RedirectStandardOutput = OutputConsoleOutput
        p.StartInfo.RedirectStandardError = p.StartInfo.RedirectStandardOutput
        p.StartInfo.UseShellExecute = False

        If p.StartInfo.RedirectStandardOutput Then
            AddHandler p.OutputDataReceived, AddressOf OnInputRecieved
            AddHandler p.ErrorDataReceived, AddressOf OnInputRecieved
            handlersRegistered = True
        End If

        p.Start()

        p.BeginOutputReadLine()
        p.BeginErrorReadLine()

        Await Task.Run(Sub() p.WaitForExit())

        If handlersRegistered Then
            RemoveHandler p.OutputDataReceived, AddressOf OnInputRecieved
            RemoveHandler p.ErrorDataReceived, AddressOf OnInputRecieved
        End If
    End Function

    Private Sub OnInputRecieved(sender As Object, e As DataReceivedEventArgs)
        If TypeOf sender Is Process AndAlso Not String.IsNullOrEmpty(e.Data) Then
            Console.Write($"[{Path.GetFileNameWithoutExtension(DirectCast(sender, Process).StartInfo.FileName)}] ")
            Console.WriteLine(e.Data)
            RaiseEvent ConsoleOutputReceived(Me, e)
        End If
    End Sub

    Private Function ResetToolsDir() As String
        CurrentToolsDir = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DotNet3dsSoundtrackConverter-" & Guid.NewGuid.ToString)
        If Not IO.Directory.Exists(CurrentToolsDir) Then
            IO.Directory.CreateDirectory(CurrentToolsDir)
        End If
        Return CurrentToolsDir
    End Function

    Private _vgmStreamPathLock As New Object
    Public Function GetVgmStreamPath() As String
        If String.IsNullOrEmpty(VgmStreamPath) OrElse Not File.Exists(VgmStreamPath) Then
            SyncLock _vgmStreamPathLock
                'Ensure the zip exists
                Dim zipPath = IO.Path.Combine(CurrentToolsDir, "vgmstream.zip")
                Dim fullPath = IO.Path.Combine(CurrentToolsDir, "vgmstream")
                If Not IO.File.Exists(zipPath) Then
                    IO.File.WriteAllBytes(zipPath, My.Resources.vgmstream)

                    'Extract the zip
                    SkyEditor.Core.Utilities.Zip.UnzipDir(zipPath, fullPath, New PhysicalIOProvider)
                End If

                VgmStreamPath = IO.Path.Combine(fullPath, "test.exe")
            End SyncLock
        End If
        Return VgmStreamPath
    End Function

    Public Async Function RunVgmStream(arguments As String) As Task
        Await RunProgram(GetVgmStreamPath, arguments).ConfigureAwait(False)
    End Function

    ''' <summary>
    ''' Runs vgmstream's test.exe with the given options.
    ''' Converts supported stream files to .wav files.
    ''' </summary>
    ''' <param name="Input">Input filename.  Must be a format that vgmstream supports.</param>
    ''' <param name="Output">Ouput filename of the wav that will be created.</param>
    ''' <param name="LoopCount">Number of times the sound stream should loop.  Defaults to 2 times.</param>
    ''' <param name="FadeTime">Number of seconds to take when fading.  Defaults to 10 seconds.</param>
    ''' <param name="FadeDelay">Number of seconds to delay before fading.  Defaults to 0 seconds.</param>
    ''' <returns></returns>
    Public Async Function RunVGMStream(Input As String, Output As String, Optional LoopCount As Decimal? = Nothing, Optional FadeTime As Decimal? = Nothing, Optional FadeDelay As Decimal? = Nothing) As Task
        Dim arguments As New Text.StringBuilder

        arguments.Append($"-o ""{Output}"" ")

        If LoopCount IsNot Nothing Then
            arguments.Append($"-l {LoopCount} ")
        End If

        If FadeTime IsNot Nothing Then
            arguments.Append($"-f {FadeTime} ")
        End If

        If FadeDelay IsNot Nothing Then
            arguments.Append($"-d {FadeDelay} ")
        End If

        arguments.Append($"""{Input}""")

        Await RunVgmStream(arguments.ToString)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If Directory.Exists(CurrentToolsDir) Then
                    Directory.Delete(CurrentToolsDir, True)
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
