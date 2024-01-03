using System;

namespace At.Matus.Instruments.RadiaCode
{
    public class Spectrum
    {
        public SpectrumType Type { get; internal set; } = SpectrumType.Invalid;
        public int NumberOfChannels { get; internal set; } = -1;
        public int ChannelPitch { get; internal set; } = -1;
        public string Name { get; internal set; }
        public string Comment { get; internal set; }
        public string SerialNumber { get; internal set; }
        public int MeasurementTime { get; internal set; } = -1;  // in s
        public EnergyCalibration EnergyCalibration { get; internal set; }
        public DataPoint[] Data { get; internal set; }
        public DataPoint MaximumValue => GetMaximumValue();
        public DataPoint MinimumValue => GetMinimumValue();

        public bool IsCompatible(Spectrum other)
        {
            if (Type == SpectrumType.Invalid) return false;
            if (other.Type == SpectrumType.Invalid) return false;
            if (NumberOfChannels != other.NumberOfChannels) return false;
            if (ChannelPitch != other.ChannelPitch) return false;
            if (SerialNumber != other.SerialNumber) return false;
            if (!(EnergyCalibration.Equals(other.EnergyCalibration))) return false;
            return true;
        }

        public int GetTotalCounts(int fromChannel, int toChannel)
        {
            if (fromChannel > toChannel)
            {
                int temp = fromChannel;
                fromChannel = toChannel;
                toChannel = temp;
            }
            int totalCounts = 0;
            foreach (var dp in Data)
            {
                if (dp.Channel >= fromChannel && dp.Channel <= toChannel)
                    totalCounts += dp.Counts;
            }
            return totalCounts;
        }

        public int GetTotalCounts(double fromEnergy, double toEnergy)
        {
            if (fromEnergy > toEnergy)
            {
                double temp = fromEnergy;
                fromEnergy = toEnergy;
                toEnergy = temp;
            }
            int totalCounts = 0;
            foreach (var dp in Data)
            {
                if (dp.Energy >= fromEnergy && dp.Energy <= toEnergy)
                    totalCounts += dp.Counts;
            }
            return totalCounts;
        }

        public int GetTotalCounts() => GetTotalCounts(0, NumberOfChannels);

        public double GetTotalRate(int fromChannel, int toChannel)
        {
            if (fromChannel > toChannel)
            {
                int temp = fromChannel;
                fromChannel = toChannel;
                toChannel = temp;
            }
            double totalRate = 0;
            foreach (var dp in Data)
            {
                if (dp.Channel >= fromChannel && dp.Channel <= toChannel)
                    totalRate += dp.Rate;
            }
            return totalRate;
        }

        public double GetTotalRate(double fromEnergy, double toEnergy)
        {
            if (fromEnergy > toEnergy)
            {
                double temp = fromEnergy;
                fromEnergy = toEnergy;
                toEnergy = temp;
            }
            double totalRate = 0;
            foreach (var dp in Data)
            {
                if (dp.Energy >= fromEnergy && dp.Energy <= toEnergy)
                    totalRate += dp.Rate;
            }
            return totalRate;
        }

        public double GetTotalRate() => GetTotalRate(0, NumberOfChannels);

        public double GetTotalSigmaRate(int fromChannel, int toChannel)
        {
            if (fromChannel > toChannel)
            {
                int temp = fromChannel;
                fromChannel = toChannel;
                toChannel = temp;
            }
            double totalVariance = 0;
            foreach (var dp in Data)
            {
                if (dp.Channel >= fromChannel && dp.Channel <= toChannel)
                    totalVariance += dp.SigmaRate * dp.SigmaRate;
            }
            return Math.Sqrt(totalVariance);
        }

        public double GetTotalSigmaRate(double fromEnergy, double toEnergy)
        {
            if (fromEnergy > toEnergy)
            {
                double temp = fromEnergy;
                fromEnergy = toEnergy;
                toEnergy = temp;
            }
            double totalVariance = 0;
            foreach (var dp in Data)
            {
                if (dp.Energy >= fromEnergy && dp.Energy <= toEnergy)
                    totalVariance += dp.SigmaRate * dp.SigmaRate;
            }
            return Math.Sqrt(totalVariance);
        }

        public double GetTotalSigmaRate() => GetTotalSigmaRate(0, NumberOfChannels);

        private DataPoint GetMaximumValue()
        {
            double maxRate = double.MinValue;
            int maxChannel = -1;
            for (int i = 0; i < Data.Length; i++)
            {
                DataPoint p = Data[i];
                if (p.Rate > maxRate)
                {
                    maxRate = p.Rate;
                    maxChannel = i;
                }
            }
            return new DataPoint(Data[maxChannel]);
        }

        private DataPoint GetMinimumValue()
        {
            double minRate = double.MaxValue;
            int minChannel = -1;
            for (int i = 0; i < Data.Length; i++)
            {
                DataPoint p = Data[i];
                if (p.Rate < minRate)
                {
                    minRate = p.Rate;
                    minChannel = i;
                }
            }
            return new DataPoint(Data[minChannel]);
        }
    }
}
