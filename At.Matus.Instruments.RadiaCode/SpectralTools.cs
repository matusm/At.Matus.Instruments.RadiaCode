using System;

namespace At.Matus.Instruments.RadiaCode
{
    public static class SpectralTools
    {

        public static Spectrum CorrectBackground(Spectrum signal, Spectrum background)
        {
            Spectrum result = new Spectrum();
            if (!signal.IsCompatible(background))
                return result; // invalid spectrum
            result.Type = SpectrumType.Processed;
            result.NumberOfChannels = signal.NumberOfChannels;
            result.ChannelPitch = signal.ChannelPitch;
            result.SerialNumber = signal.SerialNumber;
            result.EnergyCalibration = signal.EnergyCalibration;
            result.Name = $"({signal.Name}) - ({background.Name})";
            result.Comment = "background corrected";
            DataPoint[] points = new DataPoint[signal.Data.Length];
            for (int i = 0; i < points.Length; i++)
            {
                DataPoint point = new DataPoint();
                point.Channel = signal.Data[i].Channel;
                point.Counts = -1; // not defined
                point.Energy = signal.Data[i].Energy;
                point.Rate = Math.Max(signal.Data[i].Rate - background.Data[i].Rate, 0.0);
                point.Rate = signal.Data[i].Rate - background.Data[i].Rate;
                point.SigmaRate = Quadrature(signal.Data[i].SigmaRate, background.Data[i].SigmaRate);
                points[i] = point;
            }
            result.Data = points;
            return result;
        }

        private static double Quadrature(double x1, double x2) => Math.Sqrt(x1 * x1 + x2 * x2);
    }
}
