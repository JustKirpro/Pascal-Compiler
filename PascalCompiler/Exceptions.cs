using System;

namespace PascalCompiler
{
    public class ExpressionException : Exception
    {
        public ExpressionException() { }
    }

    public class OperationException : Exception
    {
        public OperationException() { }
    }

    public class IdentifierException : Exception
    {
        public IdentifierException() { }
    }
}
