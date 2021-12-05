using System.IO;
using System.Collections.Generic;

namespace PascalCompiler
{
    public class IOModule
    {
        private readonly string[] lines;
        private readonly string outputPath;
        private int rowNumber;
        public int CharacterNumber { get; private set; }
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
            if (CharacterNumber == 0 && rowNumber > 0)
                WriteLine();

            if (rowNumber < lines.Length && CharacterNumber < lines[rowNumber].Length)
                return lines[rowNumber][CharacterNumber++];

            rowNumber++;
            CharacterNumber = 0;
            return rowNumber <= lines.Length ? '\n' : '\0';
        }

        public void AddError(int code, int position) => errors.Add(new Error(code, position));

        private void WriteLine()
        {
            using StreamWriter writer = File.AppendText(outputPath);
            writer.WriteLine($"  {rowNumber.ToString().PadLeft(2, '0')}   {lines[rowNumber - 1]}");

            if (errors.Count > 0)
            {
                foreach (Error error in errors)
                {
                    totalErrors++;
                    writer.WriteLine($"**{totalErrors.ToString().PadLeft(2, '0')}**{" ^ ".PadLeft(error.Position + 2)} Код ошибки: {error.Code}");
                    writer.WriteLine($"****** {ErrorMatcher.GetErrorDescription(error.Code)}");
                }

                errors.Clear();
            }

            if (rowNumber == lines.Length)
                writer.WriteLine($"\nВсего ошибок - {totalErrors}");
        }
    }
}