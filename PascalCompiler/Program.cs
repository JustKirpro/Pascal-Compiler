
namespace PascalCompiler
{
    class Program
    {
        static void Main()
        {
            SyntaxAnalyzer syntaxAnalyzer = new(@"/Users/kirpro/Desktop/input.pas", @"/Users/kirpro/Desktop/output.txt");
            syntaxAnalyzer.StartAnylysis();
        }
    }
}