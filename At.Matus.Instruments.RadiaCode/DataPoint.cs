namespace At.Matus.Instruments.RadiaCode
{
    public class DataPoint
    {
        public int Channel { get; internal set; }
        public int Counts { get; internal set; }
        public double Energy { get; internal set; }      // in keV
        public double Rate { get; internal set; }        // in counts/s
        public double SigmaRate { get; internal set; }   // in counts/s

        public DataPoint()
        {
            Channel = -1;
            Counts = -1;
            Energy = double.NaN;
            Rate = double.NaN;
            SigmaRate = double.NaN;
        }

        public DataPoint(DataPoint other)
        {
            Channel = other.Channel;
            Counts = other.Counts;
            Energy = other.Energy;
            Rate = other.Rate;
            SigmaRate = other.SigmaRate;
        }
    }

}
