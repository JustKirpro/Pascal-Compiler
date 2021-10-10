using System.IO;

namespace PascalCompiler
{
    public class IOModule
    {
        readonly string[] lines;
        int rowNumber;
        int characterNumber;

        public IOModule(string path)
        {
            lines = File.ReadAllLines(path);
        }

        public bool HasUnreadCharacter() => rowNumber != lines.Length;

        public char ReadNextCharacter()
        {
            if (characterNumber == lines[rowNumber].Length)
            {
                rowNumber++;
                characterNumber = 0;
                return ' ';
            }

            return lines[rowNumber][characterNumber++];
        }

        public char CheckNextCharacter() => characterNumber == lines[rowNumber].Length ? ' ' : lines[rowNumber][characterNumber];
    }
}