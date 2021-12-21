using System.Collections.Generic;

namespace PascalCompiler
{
    public static class NextTokens
    {
        public static readonly List<Operation> Program = new List<Operation>() { Operation.Var, Operation.Begin, Operation.Point };

        public static readonly List<Operation> SameTypeVariables = new List<Operation>() { Operation.Semicolon, Operation.Begin, Operation.Point };

        public static readonly List<Operation> OperatorsPartStart = new List<Operation>() { Operation.Begin };

        public static readonly List<Operation> OperatorsPartEnd = new List<Operation>() { Operation.Point };

        public static readonly List<Operation> OperatorStart = new List<Operation>() { Operation.Begin, Operation.If, Operation.While, Operation.End };

        public static readonly List<Operation> OperatorEnd = new List<Operation>() { Operation.Semicolon, Operation.End };
    }

    public static class Operations
    {
        public static readonly List<Operation> AdditiveOperatons = new List<Operation>() { Operation.Plus, Operation.Minus, Operation.Or };

        public static readonly List<Operation> MultiplicativeOperations = new List<Operation>() { Operation.Asterisk, Operation.Slash, Operation.Div, Operation.Mod, Operation.And };

        public static readonly List<Operation> LogicalOperations = new List<Operation>() { Operation.Less, Operation.LessOrEqual, Operation.Greater, Operation.GreaterOrEqual, Operation.Equals, Operation.NotEqual };
    }
}
