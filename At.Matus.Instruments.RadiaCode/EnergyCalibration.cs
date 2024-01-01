using System;

namespace At.Matus.Instruments.RadiaCode
{
    public class EnergyCalibration : IEquatable<EnergyCalibration>
    {
        public EnergyCalibration(double a0, double a1, double a2)
        {
            A0 = a0;
            A1 = a1;
            A2 = a2;
        }
        public EnergyCalibration() { }

        public double A0 { get; } = double.NaN;
        public double A1 { get; } = double.NaN;
        public double A2 { get; } = double.NaN;

        public double Convert(int channel)
        {
            if (channel < 0) return double.NaN;
            return A0 + (A1 * channel) + (A2 * channel * channel);
        }

        public bool Equals(EnergyCalibration other)
        {
            if (A0 != other.A0) return false;
            if (A1 != other.A1) return false;
            if (A2 != other.A2) return false;
            return true;
        }
    }
}
