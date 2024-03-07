using System;
using System.Text;

namespace Rc2Spe
{
    public class Block
    {
        private const int maxLineLength = 64;

        public Block(string name)
        {
            AddLine($"${name.Trim()}:");
        }

        public void AddLine(string line)
        {
            CheckLength(line);
            sb.AppendLine(line);
        }

        public override string ToString() => sb.ToString().TrimEnd('\r', '\n');

        private void CheckLength(string line)
        {
            if (line.Length >= maxLineLength)
            {
                throw new ArgumentOutOfRangeException(nameof(line), $"Line must not exceed {maxLineLength} characters.");
            }
        }

        private readonly StringBuilder sb = new StringBuilder();
    }
}
