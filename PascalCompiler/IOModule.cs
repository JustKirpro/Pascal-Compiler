using System.IO;
using System.Collections.Generic;

namespace PascalCompiler
{
    public class IOModule
    {
        public List<Error> Errors { get; } = new List<Error>();
        private int totalErrors;
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

        public char ReadNextCharacter()
        {
            if (characterNumber != lines[rowNumber].Length)
                return lines[rowNumber][characterNumber++];

            WriteListingLine();
            rowNumber++;
            characterNumber = 0;
            return rowNumber == lines.Length ? '\0' : '\n';
        }

        private void WriteListingLine()
        {
            using StreamWriter writer = File.AppendText(outputPath);
            writer.WriteLine(lines[rowNumber]);

            if (Errors.Count > 0)
            {
                foreach (Error error in Errors)
                {
                    totalErrors++;
                    writer.WriteLine($"*{totalErrors.ToString().PadLeft(3, '0')}* Код ошибки: {error.ErrorCode}");
                    writer.WriteLine(ErrorMatcher.GetErrorDescription(error.ErrorCode));
                }

                Errors.Clear();
            }

            if (rowNumber == lines.Length - 1)
                writer.WriteLine($"\nВсего ошибок - {totalErrors}");
        }
    }
}