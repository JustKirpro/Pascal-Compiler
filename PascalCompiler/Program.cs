using System;

namespace PascalCompiler
{
    class Program
    {
        static void Main()
        {
            LexicalAnalyzer lexicalAnalyzer = new(@"/Users/kirpro/Desktop/input.pas", @"/Users/kirpro/Desktop/output.txt");

            while (true)
            {
                Token token = lexicalAnalyzer.GetNextToken();

                if (token == null)
                    break;

                Console.WriteLine(token);
            }
        }
    }
}