using System.IO;
using System.Collections.Generic;

namespace PascalCompiler
{
    public class IOModule
    {
        private readonly string[] lines;
        private readonly string outputPath;
        private int rowNumber;
        private int characterNumber;
        private readonly List<Error> errors = new();
        private int totalErrors;

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

            WriteLine();
            rowNumber++;
            characterNumber = 0;
            return rowNumber == lines.Length ? '\0' : '\n';
        }

        public void AddError(int errorCode)
        {
            errors.Add(new Error(errorCode));
        }

        private void WriteLine()
        {
            using StreamWriter writer = File.AppendText(outputPath);
            writer.WriteLine(lines[rowNumber]);

            if (errors.Count > 0)
            {
                foreach (Error error in errors)
                {
                    totalErrors++;
                    writer.WriteLine($"*{totalErrors.ToString().PadLeft(3, '0')}* Код ошибки: {error.ErrorCode}");
                    writer.WriteLine(ErrorMatcher.GetErrorDescription(error.ErrorCode));
                }

                errors.Clear();
            }

            if (rowNumber == lines.Length - 1)
                writer.WriteLine($"\nВсего ошибок - {totalErrors}");
        }
    }
}