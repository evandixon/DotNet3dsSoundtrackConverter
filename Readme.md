# .Net 3DS Soundtrack Converter
.Net 3DS Soundtrack Converter is a tool that can convert the game files of a 3DS game into an MP3 soundtrack.

## Usage
Usage of console app:
```
Usage: SoundtrackConsole.exe <Source> <SoundtrackDefinition> <OutputDirectory>
```
* Source can be either a directory containing the raw files of a 3DS ROM, or a compatible decrypted 3DS ROM.
* SoundtrackDefinition is the path to the soundtrack definition file.
* OutputDirectory is where the soundtrack files will be generated.

## Creating New Soundtrack Definitions
A soundtrack definition file is a text file structured as follows.  Comments are not allowed in the actual file, but will be added below using the `#` character.
```
system=3DS #System of the game this is for
gameid=00040000000(BA8|BA9|950)00 #Regular expression matching the game ID of the game this is for.
album=Pokémon Mystery Dungeon: Gates to Infinity
artist=Spike Chunsoft
year=2013
path=romfs/sound/stream #The directory the raw files are stored in
extension=bcstm #The file extension of the raw files
end #marks the end of the header
BGM_SYS_MENU=001 Main Menu #Converts `romfs/sound/stream/BGM_SYS_MENU.bcstm` to `001 Main Menu.mp3`
#Add more lines like the above
```

## Adding New Soundtrack Definitions
While soundtrack definitions can be supplied externally, automatic selection is currently only available for internal definitions.  Steps to add a soundtrack definition to the program:
1. Add the soundtrack definition file to DotNet3dsSoundtrackConverter/Soundtracks/[culture]/
2. Add the soundtrack definition file to the appropriate SoundtrackDefinitions.resx file corresponding to the culture used in step 1.  The default culture is en-US.
3. Add the soundtrack definition to `DefinitionSelector.Soundtracks` as shown in `DefinitionSelector`'s constructor.

Assuming the soundtrack definition is properly formatted, once the above steps are completed the soundtrack definition can then be used with auto-detecting conversion methods.

## Credits
* All the people who are credited on the [.Net 3DS Toolkit](https://github.com/evandixon/DotNet3dsToolkit)
* kode54 for [vgmstream](https://gitlab.kode54.net/kode54/vgmstream/)
* [AydinAdn](https://github.com/AydinAdn) for MediaToolkit (which I forked to allow asynchronous processing)
* [IC#Code](http://www.icsharpcode.net/) for SharpZipLib 