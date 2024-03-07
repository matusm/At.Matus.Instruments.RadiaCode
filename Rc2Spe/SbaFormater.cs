using At.Matus.Instruments.RadiaCode;
using System.Globalization;
using System.Text;

namespace Rc2Spe
{
    public class SbaFormater
    {

        public SbaFormater(RadiaCode radiaCode, Spectrum spectrum)
        {
            this.radiaCode = radiaCode;
            this.spectrum = spectrum;
            InitComments();
        }

        public SbaFormater(RadiaCode radiaCode) : this(radiaCode, radiaCode.EnergySpectrum) { }

        public string SpectrumID { get; set; }
        public string UserComment { get; set; }

        public override string ToString()
        {
            PopulateBlocks();
            return BlocksToString();
        }

        private void InitComments()
        {
            SpectrumID = $"{radiaCode.SampleInfoName} {radiaCode.SampleInfoNote}";
            UserComment = $"{spectrum.Comment}";
        }

        private string BlocksToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(blckId.ToString());
            sb.AppendLine(blckRem.ToString());
            sb.AppendLine(blckDate.ToString());
            sb.AppendLine(blckDateEnd.ToString());
            sb.AppendLine(blckTime.ToString());
            sb.AppendLine(blckData.ToString());
            sb.AppendLine(blckECal.ToString());
            sb.AppendLine(blckDevice.ToString());
            sb.AppendLine(blckTemp.ToString());
            sb.AppendLine(blckApp.ToString());
            return sb.ToString();
        }

        private void PopulateBlocks()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            /*******************/
            blckData.AddLine($"0 {spectrum.NumberOfChannels - 1}");
            foreach (DataPoint dp in spectrum.Data)
            {
                blckData.AddLine($"{dp.Counts}");
            }
            /*******************/
            blckId.AddLine(SpectrumID);
            /*******************/
            blckTime.AddLine($"{spectrum.MeasurementTime} {spectrum.MeasurementTime}");
            /*******************/
            blckDate.AddLine($"{radiaCode.StartTime.ToString("MM/dd/yyyy HH:mm:ss")}");
            /*******************/
            blckDateEnd.AddLine($"{radiaCode.EndTime.ToString("MM/dd/yyyy HH:mm:ss")}");
            /*******************/
            blckECal.AddLine("3");
            blckECal.AddLine($"{spectrum.EnergyCalibration.A0} {spectrum.EnergyCalibration.A1} {spectrum.EnergyCalibration.A2} ");
            /*******************/
            blckApp.AddLine($"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            /*******************/
            blckDevice.AddLine($"{radiaCode.DeviceConfigReference}"); // device type
            blckDevice.AddLine($"{spectrum.SerialNumber}"); // serial number
            blckDevice.AddLine($"-"); // hardware version
            blckDevice.AddLine($"-"); // firmware version
            /*******************/
            blckRem.AddLine(UserComment);
            /*******************/
            blckTemp.AddLine($"-"); // detector °C
            blckTemp.AddLine($"-"); // MCA °C
        }

        private readonly Block blckData = new Block("DATA");            // NECESSARY BLOCK (SPEDAC Pro User's Manual)
        private readonly Block blckId = new Block("SPEC_ID");           // OPTIONAL BLOCK (SPEDAC Pro User's Manual)
        private readonly Block blckTime = new Block("MEAS_TIM");        // OPTIONAL BLOCK (SPEDAC Pro User's Manual)
        private readonly Block blckDate = new Block("DATE_MEA");        // OPTIONAL BLOCK (SPEDAC Pro User's Manual)
        private readonly Block blckDateEnd = new Block("DATE_END_MEA"); // OPTIONAL BLOCK (SPEDAC Pro User's Manual)
        private readonly Block blckECal = new Block("MCA_CAL");         // OPTIONAL BLOCK (SPEDAC Pro User's Manual)
        private readonly Block blckApp = new Block("APPLICATION_ID");   // OPTIONAL BLOCK (MCA Data Spectral Format)
        private readonly Block blckDevice = new Block("DEVICE_ID");     // OPTIONAL BLOCK (MCA Data Spectral Format)
        private readonly Block blckRem = new Block("SPEC_REM");         // OPTIONAL BLOCK (MCA Data Spectral Format)
        private readonly Block blckTemp = new Block("TEMPERATURE");     // OPTIONAL BLOCK (MCA Data Spectral Format)

        private readonly RadiaCode radiaCode;
        private readonly Spectrum spectrum;
    }
}
