using CommandLine;

namespace Rc2Spe
{
    public class Options
    {
        [Option("comment", Default = "", HelpText = "User comment.")]
        public string UserComment { get; set; }

        [Option("ID", Default = "", HelpText = "User set spectrum ID.")]
        public string SpectrumID { get; set; }

        [Option('q', "quiet", HelpText = "Quiet mode. No screen output (except for errors).")]
        public bool BeQuiet { get; set; }

        [Value(0, MetaName = "InputPath", Required = true, HelpText = "Input file-name including path")]
        public string InputPath { get; set; }

        [Value(1, MetaName = "OutputPath", HelpText = "Output file-name including path")]
        public string OutputPath { get; set; }
    }
}
