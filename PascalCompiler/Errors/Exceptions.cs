using System;

namespace PascalCompiler
{
    public class ExpressionException : Exception
    {
        public ExpressionException() { }
    }

    public class TypeException : Exception
    {
        public TypeException() { }
    }
}