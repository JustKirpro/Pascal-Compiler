using System.IO;
using System.Collections.Generic;

namespace PascalCompiler
{
    public class IOModule
    {
        public List<string> Errors { get; } = new();
        private int totalErros = 0;
        private readonly string[] lines;
        private readonly string outputPath;
        private int rowNumber;
        private int characterNumber;

        public IOModule(string inputPath, string outputPath)
        {
            this.outputPath = outputPath;
            lines = File.ReadAllLines(inputPath);
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

                    totalErros += Errors.Count;
                    Errors.Clear();
                }

                if (rowNumber == lines.Length - 1)
                    writer.WriteLine($"Всего ошибок - {totalErros}");
            }
        }
    }
}