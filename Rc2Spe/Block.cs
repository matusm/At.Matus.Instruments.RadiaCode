using System;
using System.Text;

namespace Rc2Spe
{
    public class Block
    {

        public Block(string name)
        {
            blockName = $"${name.Trim()}:";
            CheckLength(blockName);
            sb.AppendLine(blockName);
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

        private const int maxLineLength = 64;
        private string blockName;
        private StringBuilder sb = new StringBuilder();

    }
}
