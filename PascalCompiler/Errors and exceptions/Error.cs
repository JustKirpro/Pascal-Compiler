
namespace PascalCompiler
{
    public class Error
    {
        public int Code { get; }
        public int Position { get; }

        public Error(int code, int position)
        {
            Code = code;
            Position = position;
        }
    }
}