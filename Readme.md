# .Net 3DS Soundtrack Converter
.Net 3DS Soundtrack Converter is a tool that can convert the game files of a 3DS game into an MP3 soundtrack.

Usage of console app:
```
Usage: SoundtrackConsole.exe <Source> <SoundtrackDefinition> <OutputDirectory>
```
* Source can be either a directory containing the raw files of a 3DS ROM, or a compatible decrypted 3DS ROM.
* SoundtrackDefinition is the path to the soundtrack definition file.
* OutputDirectory is where the soundtrack files will be generated.

Credits:
* All the people who are credited on the [.Net 3DS Toolkit](https://github.com/evandixon/DotNet3dsToolkit)
* kode54 for [vgmstream](https://gitlab.kode54.net/kode54/vgmstream/)
* [AydinAdn](https://github.com/AydinAdn) for MediaToolkit (which I forked to allow asynchronous processing)
* [IC#Code](http://www.icsharpcode.net/) for SharpZipLib 