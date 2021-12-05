using System.Collections.Generic;

namespace PascalCompiler
{
    public static class Folowers
    {
        public static readonly List<Operation> Program = new()
        {
            Operation.Semicolon,
            Operation.Var,
            Operation.Begin
        };

        public static readonly List<Operation> Block = new()
        {
            Operation.Point
        };


    }
}
