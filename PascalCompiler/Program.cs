using System;

namespace PascalCompiler
{
    class Program
    {
        static void Main()
        {
            IOModule io = new(@"/Users/kirpro/Desktop/temp");
            LexicalAnalyzer lexer = new(io);

            while (true)
            {
                Token token = lexer.GetNextToken();

                if (token == null)
                    break;

                Console.WriteLine(token);
            }
        }
    }
}