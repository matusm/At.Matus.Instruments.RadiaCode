using System;
using System.Globalization;
using System.Xml;

namespace At.Matus.Instruments.RadiaCode
{
    public class RadiaCode
    {
        private const string dataXPath = "/ResultDataFile/ResultDataList/ResultData/";
        private const string energyNode = "EnergySpectrum/";
        private const string backgroundNode = "BackgroundEnergySpectrum/";

        public RadiaCode(XmlDocument doc)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            XmlData = doc;
            EnergySpectrum = GetEnergySpectrum();
            BackgroundEnergySpectrum = GetBackgroundEnergySpectrum();
        }

        public XmlDocument XmlData { get; }
        public string FormatVersion => GetInnerText("/ResultDataFile/FormatVersion");
        public string DeviceConfigReference => GetInnerText(dataXPath + "DeviceConfigReference/Name");
        public string SampleInfoName => GetInnerText(dataXPath + "SampleInfo/Name");
        public string SampleInfoNote => GetInnerText(dataXPath + "SampleInfo/Note");
        public string BackgroundSpectrumFile => GetInnerText(dataXPath + "BackgroundSpectrumFile");
        public DateTime StartTime => GetStartTime();
        public DateTime EndTime => GetEndTime();
        public Spectrum EnergySpectrum { get; }
        public Spectrum BackgroundEnergySpectrum { get; }

        private string[] GetInnerTexts(string xPath)
        {
            string[] values = new string[0];
            XmlNodeList xmlNodeList = XmlData.DocumentElement.SelectNodes(xPath);
            if (xmlNodeList == null) return values;
            if (xmlNodeList.Count == 0) return values;
            values = new string[xmlNodeList.Count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = xmlNodeList[i].InnerText;
            }
            return values;
        }

        private string GetInnerText(string xPath)
        {
            string[] values = GetInnerTexts(xPath);
            return values.Length == 0 ? string.Empty : values[0];
        }

        private int GetInnerInt(string xPath)
        {
            string token = GetInnerText(xPath);
            if (string.IsNullOrWhiteSpace(token)) return -1;
            return int.TryParse(token, out int value) ? value : -1;
        }

        private int[] GetInnerInts(string xPath)
        {
            var tokens = GetInnerTexts(xPath);
            int[] values = new int[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                values[i] = (int.TryParse(tokens[i], out int value) ? value : -1);
            }
            return values;
        }

        private double[] GetInnerDoubles(string xPath)
        {
            var tokens = GetInnerTexts(xPath);
            double[] values = new double[tokens.Length];
            for (int i = 0; i < tokens.Length; i++)
            {
                values[i] = (double.TryParse(tokens[i], out double value) ? value : double.NaN);
            }
            return values;
        }

        private double GetInnerDouble(string xPath)
        {
            string token = GetInnerText(xPath);
            if (string.IsNullOrWhiteSpace(token)) return double.NaN;
            return double.TryParse(token, out double value) ? value : double.NaN;
        }

        private DateTime GetStartTime()
        {
            string value = GetInnerText("/ResultDataFile/ResultDataList/ResultData/StartTime");
            return DateTime.Parse(value);
        }

        private DateTime GetEndTime()
        {
            string value = GetInnerText("/ResultDataFile/ResultDataList/ResultData/EndTime");
            return DateTime.Parse(value);
        }

        private Spectrum GetEnergySpectrum() => GetSpectrum(dataXPath + energyNode);

        private Spectrum GetBackgroundEnergySpectrum() => GetSpectrum(dataXPath + backgroundNode);

        private Spectrum GetSpectrum(string spectrumNode)
        {
            Spectrum spectrum = new Spectrum();
            spectrum.Type = SpectrumType.Measured;
            spectrum.NumberOfChannels = GetInnerInt(spectrumNode + "NumberOfChannels");
            if (spectrum.NumberOfChannels <= 0)
                spectrum.Type = SpectrumType.Invalid;
            spectrum.ChannelPitch = GetInnerInt(spectrumNode + "ChannelPitch");
            spectrum.Name = GetInnerText(spectrumNode + "SpectrumName");
            spectrum.Comment = GetInnerText(spectrumNode + "Comment");
            spectrum.SerialNumber = GetInnerText(spectrumNode + "SerialNumber");
            spectrum.MeasurementTime = GetInnerInt(spectrumNode + "MeasurementTime");
            // get energy calibration 
            double[] coeff = GetInnerDoubles(spectrumNode + "EnergyCalibration/Coefficients/Coefficient");
            if (coeff.Length == 3)
                spectrum.EnergyCalibration = new EnergyCalibration(coeff[0], coeff[1], coeff[2]);
            else
                spectrum.EnergyCalibration = new EnergyCalibration();
            // get actual spectrum
            int[] counts = GetInnerInts(spectrumNode + "Spectrum/DataPoint");
            DataPoint[] dataPoints = new DataPoint[counts.Length];
            for (int i = 0; i < counts.Length; i++)
            {
                var data = new DataPoint();
                data.Channel = i;
                data.Counts = counts[i];
                data.Rate = (double)counts[i] / spectrum.MeasurementTime;
                data.SigmaRate = Math.Sqrt(counts[i]) / spectrum.MeasurementTime;
                data.Energy = spectrum.EnergyCalibration.Convert(i);
                dataPoints[i] = data;
            }
            spectrum.Data = dataPoints;
            return spectrum;
        }

    }
}
