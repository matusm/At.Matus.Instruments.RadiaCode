using At.Matus.Instruments.RadiaCode;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Rc2Spe
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            #region file name logic
            string inFilename = "";
            string outFilename;
            if (args.Length == 0)
                ErrorMessage("No filename given.", true);
            if (args.Length >= 1)
            {
                string path = args[0];
                if (Path.HasExtension(path))
                {
                    inFilename = path;
                }
                else
                {
                    inFilename = Path.ChangeExtension(path, "xml");
                }
            }
            string fileDir = Path.GetDirectoryName(inFilename);
            string fileName = Path.GetFileNameWithoutExtension(inFilename);
            string fileExt = Path.GetExtension(inFilename);
            Path.Combine(fileDir, string.Concat(fileName, "_", fileExt));
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

            outFilename = Path.Combine(fileDir, string.Concat(fileName, ".spe"));

            SbaFormater sba = new SbaFormater(radiaCode);
            sba.UserComment = "Michael Matus";
            //sba.SpectrumID = "";

            using (StreamWriter sw = new StreamWriter(outFilename, false))
            {
                Console.WriteLine($"Output to {outFilename}");
                sw.WriteLine(sba);
            }
            Console.WriteLine("done.");

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
