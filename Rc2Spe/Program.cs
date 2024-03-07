using At.Matus.Instruments.RadiaCode;
using System;
using System.Globalization;
using System.IO;
using System.Xml;
using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Rc2Spe
{
    class Program
    {
        private static Options options = new Options(); // this must be set in Run()

        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Parser parser = new Parser(with => with.HelpWriter = null);
            ParserResult<Options> parserResult = parser.ParseArguments<Options>(args);
            parserResult
                .WithParsed<Options>(options => Run(options))
                .WithNotParsed(errs => DisplayHelp(parserResult, errs));
        }

        private static void Run(Options ops)
        {
            options = ops;
            #region file name logic
            string inFilename;
            string outFilename;
            string path = options.InputPath;
            if (Path.HasExtension(path))
            {
                inFilename = path;
            }
            else
            {
                inFilename = Path.ChangeExtension(path, "xml");
            }

            string fileDir = Path.GetDirectoryName(inFilename);
            string fileName = Path.GetFileNameWithoutExtension(inFilename);
            string fileExt = Path.GetExtension(inFilename);
            Path.Combine(fileDir, string.Concat(fileName, "_", fileExt));

            if (string.IsNullOrWhiteSpace(options.OutputPath))
            {
                outFilename = Path.Combine(fileDir, string.Concat(fileName, ".spe"));
            }
            else
            {
                outFilename = options.OutputPath;
            }
            #endregion

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(inFilename);
            RadiaCode radiaCode = new RadiaCode(xmlDoc);

            Spectrum spec;

            Console.WriteLine($"Input file:          {inFilename}");
            Console.WriteLine($"File format version: {radiaCode.FormatVersion}");
            Console.WriteLine($"Sample info (name):  {radiaCode.SampleInfoName}");
            Console.WriteLine($"Sample info (note):  {radiaCode.SampleInfoNote}");
            Console.WriteLine($"Device Type:         {radiaCode.DeviceConfigReference}");
            Console.WriteLine();
            spec = radiaCode.EnergySpectrum;
            if (spec.Type == SpectrumType.Invalid)
                ErrorMessage("No energy spectrum in file.", true);
            Console.WriteLine("Energy spectrum");
            Console.WriteLine($"   Name:             {spec.Name}");
            Console.WriteLine($"   Comment:          {spec.Comment}");
            Console.WriteLine($"   Device SN:        {spec.SerialNumber}");
            Console.WriteLine($"   Measurement time: {spec.MeasurementTime} s");
            Console.WriteLine($"   Total counts:     {spec.GetTotalCounts()}");
            Console.WriteLine($"   Total rate:       {spec.GetTotalRate():F4} ± {spec.GetTotalSigmaRate():F4} cps");
            Console.WriteLine($"   Maximum value:    {spec.MaximumValue.Rate:F4} cps @ {spec.MaximumValue.Energy:F0} keV");
            Console.WriteLine();

            SbaFormater sba = new SbaFormater(radiaCode);
            if (!string.IsNullOrWhiteSpace(options.UserComment)) sba.UserComment = options.UserComment;
            if (!string.IsNullOrWhiteSpace(options.SpectrumID)) sba.SpectrumID = options.SpectrumID;

            using (StreamWriter sw = new StreamWriter(outFilename, false))
            {
                Console.WriteLine($"Output to {outFilename}");
                sw.WriteLine(sba);
            }
            Console.WriteLine("done.");

        }

        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            HelpText helpText = HelpText.AutoBuild(result, h =>
            {
                h.AutoVersion = false;
                h.AdditionalNewLineAfterOption = false;
                h.AddPreOptionsLine("\nProgram to convert XML files by RadiaCode to SPE files according to IAEA.");
                h.AddPreOptionsLine("");
                h.AddPreOptionsLine($"Usage: {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} InputPath [OutPath] [options]");
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText);
        }

        private static void ErrorMessage(string message, bool exit)
        {
            Console.WriteLine(message);
            if (exit)
            {
                Console.WriteLine("Closing application.");
                Environment.Exit(1);
            }
        }
    }
}
