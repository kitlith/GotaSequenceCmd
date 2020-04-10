# GotaSequenceCmd

Small wrapper around [GotaSequenceLib](https://github.com/Gota7/GotaSequenceLib) that exposes:
 - BRSEQ to/from MIDI functionality
 - BRSEQ disassembly/reassembly
 - Some small misc. programmatic transformations.

## Usage
```
Usage: programname command input [output]

Valid commands include:
 - disassemble: takes a brseq and converts it to a human editable text form
 - assemble: takes a file generated from 'disassemble' and converts it to brseq
 - to_midi: takes a brseq and converts it to midi
 - from_midi: takes a midi and converts it to brseq
 - invert: takes a brseq and outputs a brseq, inverting all notes around note 63

If an output filename is not specified, it will be generated from the input filename.
```

There are also a few batch files that come with the program for the purpose of
drag-n-drop conversion.

## Compiling

You'll need [GotaSequenceLib](https://github.com/Gota7/GotaSequenceLib) and
[GotaSoundIO](https://github.com/Gota7/GotaSoundIO). Grab them all with git,
throw them in VS or MonoDevelop (you may need to tweak some TargetFrameworks)
and you're off to the races. If this is too vauge, sorry, I may expand on this
more another time.
