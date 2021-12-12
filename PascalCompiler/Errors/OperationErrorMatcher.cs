using System.Collections.Generic;

namespace PascalCompiler
{
    public static class OperationErrorMatcher
    {
        private static readonly Dictionary<Operation, int> dictionary = new()
        {
            [Operation.Program] = 8,
            [Operation.Begin] = 9,
            [Operation.Semicolon] = 10,
            [Operation.Point] = 11,
            [Operation.Colon] = 12,
            [Operation.End] = 13,
            [Operation.Assignment] = 14,
            [Operation.Do] = 15,
        };

        public static int GetErrorCode(Operation operation) => dictionary[operation];
    }
}
