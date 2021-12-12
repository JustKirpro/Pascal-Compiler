using System.Collections.Generic;

namespace PascalCompiler
{
    public static class NextTokens
    {
        public static readonly List<Operation> Program = new() { Operation.Var, Operation.Begin, Operation.Point };

        public static readonly List<Operation> SameTypeVariables = new() { Operation.Semicolon, Operation.Begin, Operation.Point };

        public static readonly List<Operation> OperatorsPart1 = new() { Operation.Begin, Operation.Point };

        public static readonly List<Operation> OperatorsPart2 = new() { Operation.Point };

        public static readonly List<Operation> Operator = new() { Operation.End, Operation.Point };

        public static readonly List<Operation> AssignmentOperator = new() { Operation.Semicolon, Operation.End, Operation.Point };

        public static readonly List<Operation> WhileOperator = new() { Operation.Begin, Operation.If, Operation.While, Operation.End, Operation.Point };
    }

    public static class Operations
    {
        public static readonly List<Operation> AdditiveOperatons = new() { Operation.Plus, Operation.Minus, Operation.Or };

        public static readonly List<Operation> MultiplicativeOperations = new() { Operation.Asterisk, Operation.Slash, Operation.Div, Operation.Mod, Operation.And };

        public static readonly List<Operation> LogicalOperations = new() { Operation.Less, Operation.LessOrEqual, Operation.Greater, Operation.GreaterOrEqual, Operation.Equals, Operation.NotEqual, Operation.In };
    }
}
