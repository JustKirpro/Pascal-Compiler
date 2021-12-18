using System.Collections.Generic;

namespace PascalCompiler
{
    public static class NextTokens
    {
        public static readonly List<Operation> Program = new() { Operation.Var, Operation.Begin, Operation.Point };

        public static readonly List<Operation> SameTypeVariables = new() { Operation.Semicolon, Operation.Begin, Operation.Point };

        public static readonly List<Operation> OperatorsPartStart = new() { Operation.Begin };

        public static readonly List<Operation> OperatorsPartEnd = new() { Operation.Point };

        public static readonly List<Operation> OperatorStart = new() { Operation.Begin, Operation.If, Operation.While, Operation.End };

        public static readonly List<Operation> OperatorEnd = new() { Operation.Semicolon, Operation.End };
    }

    public static class Operations
    {
        public static readonly List<Operation> AdditiveOperatons = new() { Operation.Plus, Operation.Minus, Operation.Or };

        public static readonly List<Operation> MultiplicativeOperations = new() { Operation.Asterisk, Operation.Slash, Operation.Div, Operation.Mod, Operation.And };

        public static readonly List<Operation> LogicalOperations = new() { Operation.Less, Operation.LessOrEqual, Operation.Greater, Operation.GreaterOrEqual, Operation.Equals, Operation.NotEqual};
    }
}