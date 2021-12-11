using System;
using System.IO;

namespace PascalCompiler
{
    class Program
    {
        static void Main()
        {
            Compiler compiler = new(Path.Combine(Environment.CurrentDirectory, "input.pas"), Path.Combine(Environment.CurrentDirectory, "output.txt"));
            compiler.Start();
        }
    }
}