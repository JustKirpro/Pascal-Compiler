using System.Collections.Generic;

namespace PascalCompiler
{
    public static class Starters
    {
        public static readonly List<Operation> Program = new()
        {
            Operation.Program
        };

        public static readonly List<Operation> Block = new()
        {
            Operation.Var,
            Operation.Begin
        };


    }
}
