using System.IO;
using System.Collections.Generic;

namespace PascalCompiler
{
    public class IOModule
    {
        readonly string[] lines;
        public List<string> Errors { get; }
        readonly string outputPath;
        int rowNumber;
        int characterNumber;

        public IOModule(string inputPath, string outputPath)
        {
            lines = File.ReadAllLines(inputPath);
            Errors = new List<string>();
            this.outputPath = outputPath;
            File.WriteAllText(outputPath, string.Empty);
        }

        public bool HasUnreadCharacter() => rowNumber != lines.Length;

        public char ReadNextCharacter()
        {
            if (characterNumber == lines[rowNumber].Length)
            {
                WriteLine();
                rowNumber++;
                characterNumber = 0;
                return ' ';
            }

            return lines[rowNumber][characterNumber++];
        }

        private void WriteLine()
        {
            using (StreamWriter writer = File.AppendText(outputPath))
            {
                writer.WriteLine(lines[rowNumber]);

                if (Errors.Count != 0)
                {
                    foreach (string error in Errors)
                        writer.WriteLine(error);

                    Errors.Clear();
                }
            }
        }
    }
}