using System;
using System.IO;
using System.Linq;
using GotaSoundIO.IO;

namespace GotaSequenceCmd
{
    class MainClass
    {
        public static void Main(string[] args) {
            if (args.Length < 2) {
                Console.WriteLine("Usage: programname command input [output]");
                Console.WriteLine("");
                Console.WriteLine("Valid commands include:");
                Console.WriteLine(" - disassemble: takes a brseq and converts it to a human editable text form");
                Console.WriteLine(" - assemble: takes a file generated from 'disassemble' and converts it to brseq");
                Console.WriteLine(" - to_midi: takes a brseq and converts it to midi"); 
                Console.WriteLine(" - from_midi: takes a midi and converts it to brseq");
                Console.WriteLine(" - invert: takes a brseq and outputs a brseq, inverting all notes around note 63");
                Console.WriteLine("");
                Console.WriteLine("If an output filename is not specified, it will be generated from the input filename.");
                return;
            }
            string cmd = args[0];
            string input = args[1];
            string output;
            if (args.Length > 2) {
                output = args[2];
            } else {
                switch (cmd) {
                    case "disassemble":
                        output = Path.ChangeExtension(input, "rseq");
                        break;
                    case "assemble":
                        output = Path.ChangeExtension(input, "brseq");
                        break;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
                    case "to_midi":
                        output = Path.ChangeExtension(input, "midi");
                        break;
                    case "from_midi":
                        output = Path.ChangeExtension(input, "brseq");
                        break;
                    case "invert":
                        output = Path.ChangeExtension(input, null) + "_inverted.brseq";
                        break;
                    default:
                        Console.WriteLine("Unrecognized command, exiting.");
                        return;
                }

                // TODO: relax this restriction?
                if (File.Exists(output)) {
                    Console.WriteLine("Autogenerated output filename '" + output + "' already exists, exiting.");
                    return;
                }
            }

            var rseq = new RevolutionFileLoader.Sequence();
            switch (cmd) {
                case "disassemble":
                    rseq.Read(input);
                    rseq.Name = Path.GetFileName(input);
                    rseq.ReadCommandData();
                    File.WriteAllLines(output, rseq.ToText());
                    break;
                case "assemble":
                    rseq.FromText(File.ReadAllLines(input).ToList());
                    rseq.WriteCommandData();
                    rseq.Name = Path.GetFileName(output);
                    rseq.Write(output);
                    break;
                case "to_midi":
                    rseq.Read(input);
                    rseq.Name = Path.GetFileName(input);
                    rseq.ReadCommandData();
                    rseq.SaveMIDI(output);
                    break;
                case "from_midi":
                    rseq.FromMIDI(input);
                    rseq.Name = Path.GetFileName(output);
                    rseq.WriteCommandData();
                    rseq.Write(output);
                    break;
                case "invert":
                    rseq.Read(input);
                    rseq.Name = Path.GetFileName(output);
                    rseq.ReadCommandData();

                    foreach (var command in rseq.Commands) {
                        if (command.CommandType == GotaSequenceLib.SequenceCommands.Note) {
                            var param = command.Parameter as GotaSequenceLib.NoteParameter;
                            param.Note = 0x7F - param.Note;
                            command.Parameter = param;
                        }
                    }

                    rseq.WriteCommandData();
                    rseq.Write(output);
                    break;
                default:
                    Console.WriteLine("Unrecognized command, exiting.");
                    return;
            }

        }
    }
}
