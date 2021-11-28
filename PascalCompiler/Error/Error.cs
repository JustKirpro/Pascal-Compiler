
namespace PascalCompiler
{
    public class Error
    {
        public int ErrorCode { get; }

        public Error(int errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}