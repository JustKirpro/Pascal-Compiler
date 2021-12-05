using System;
using System.IO;

namespace PascalCompiler
{
    class Program
    {
        static void Main()
        {
            LexicalAnalyzer syntacticAnalyzer = new(Path.Combine(Environment.CurrentDirectory, "input.pas"), Path.Combine(Environment.CurrentDirectory, "output.txt"));
            //syntacticAnalyzer.Start();
            Token token = syntacticAnalyzer.GetNextToken();

            while (token != null)
            {
                Console.WriteLine(token);
                token = syntacticAnalyzer.GetNextToken();

            }
        }
    }
}