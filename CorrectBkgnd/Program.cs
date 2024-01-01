using At.Matus.Instruments.RadiaCode;
using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace CorrectBkgnd
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            #region file name logic
            string inFilename = @"C:\Users\User\Documents\RadiaCode\Pechblende_01A.xml";
            string outFilename;
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

            Console.WriteLine();
            spec = radiaCode.BackgroundEnergySpectrum;
            if (spec.Type == SpectrumType.Invalid)
            {
                ErrorMessage("No background energy spectrum in file.", false);
            }
            else
            {
                Console.WriteLine("Background energy spectrum");
                Console.WriteLine($"   Name:             {spec.Name}");
                Console.WriteLine($"   Comment:          {spec.Comment}");
                Console.WriteLine($"   Device SN:        {spec.SerialNumber}");
                Console.WriteLine($"   Measurement time: {spec.MeasurementTime} s");
                Console.WriteLine($"   Total counts:     {spec.GetTotalCounts()}");
                Console.WriteLine($"   Total rate:       {spec.GetTotalRate():F4} ± {spec.GetTotalSigmaRate():F4} cps");
            }

            Console.WriteLine();
            spec = SpectralTools.CorrectBackground(radiaCode.EnergySpectrum, radiaCode.BackgroundEnergySpectrum);
            if (spec.Type == SpectrumType.Invalid)
            {
                ErrorMessage("Energy spectrum not corrected, output raw energy spectrum.", false);
                spec = radiaCode.EnergySpectrum;
                outFilename = Path.Combine(fileDir, string.Concat(fileName, "_raw", ".csv"));
            }
            else
            {
                outFilename = Path.Combine(fileDir, string.Concat(fileName, "_corrected", ".csv"));
                Console.WriteLine("Corrected energy spectrum");
                Console.WriteLine($"   Name:             {spec.Name}");
                Console.WriteLine($"   Comment:          {spec.Comment}");
                Console.WriteLine($"   Total rate:       {spec.GetTotalRate():F4} ± {spec.GetTotalSigmaRate():F4} cps");
            }

            Console.WriteLine();
            using (StreamWriter sw = new StreamWriter(outFilename, false))
            {
                Console.WriteLine($"Output to {outFilename}");
                sw.WriteLine($"channel, energy (keV), rate (cps), rate standard deviation (cps)");
                foreach (DataPoint dp in spec.Data)
                {
                    sw.WriteLine($"{dp.Channel}, {dp.Energy}, {dp.Rate}, {dp.SigmaRate}");
                    //Console.WriteLine($"{dp.Channel,4} : {dp.Energy,7:F1} keV   {dp.Rate:F6} ± {dp.SigmaRate:F6}");
                }
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
