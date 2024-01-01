namespace At.Matus.Instruments.RadiaCode
{
    public class DataPoint
    {
        public int Channel { get; internal set; }
        public int Counts { get; internal set; }
        public double Energy { get; internal set; }      // in keV
        public double Rate { get; internal set; }        // in counts/s
        public double SigmaRate { get; internal set; }   // in counts/s
    }
}
