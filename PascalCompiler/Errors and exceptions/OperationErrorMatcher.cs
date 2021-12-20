using System.Collections.Generic;

namespace PascalCompiler
{
    public static class OperationErrorMatcher
    {
        private static readonly Dictionary<Operation, int> dictionary = new Dictionary<Operation, int>()
        {
            [Operation.Program] = 8,
            [Operation.Begin] = 9,
            [Operation.End] = 10,
            [Operation.Then] = 11,
            [Operation.Do] = 12,
            [Operation.Assignment] = 13,
            [Operation.Semicolon] = 14,
            [Operation.Colon] = 15,
            [Operation.Point] = 16
        };

        public static int GetErrorCode(Operation operation) => dictionary[operation];
    }
}