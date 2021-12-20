using System;

namespace PascalCompiler
{
    public abstract class ExpressionException : Exception
    {
        public int ErrorPostion { get; protected set; }
    }

    public class OperatorException : ExpressionException
    {
        public OperatorException(int errorPostion)
        {
            ErrorPostion = errorPostion;
        }
    }

    public class TypeException : ExpressionException
    {
        public TypeException(int errorPostion)
        {
            ErrorPostion = errorPostion;
        }
    }

    public class OperationException : ExpressionException
    {
        public OperationException(int errorPostion)
        {
            ErrorPostion = errorPostion;
        }
    }
}