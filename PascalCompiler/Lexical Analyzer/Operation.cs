namespace PascalCompiler
{
    public enum Operation
    {
        If, 
        Do, 
        Or,
        In,
        End,
        Var,
        Div,
        And,
        Not,
        Xor,
        Mod,
        Then,
        Else,
        Type,
        Begin,
        While,
        Program,
        Less, // <
        Greater, // >
        LessOrEqual, // <=
        GreaterOrEqual, // >=
        Assignment, // :=
        Plus, // +
        Minus, // -
        Asterisk, // * 
        Slash, // /
        Equals, // =
        NotEqual, // <>
        LeftParenthesis, // (
        RightParenthesis, // )
        LeftBracket, // {
        RightBracket, // }
        LeftSquareBracket, // [
        RightSquareBracket, // ]
        Point, // .
        TwoPoints, // ..
        Comma, // ,
        Colon, // :
        Semicolon, // ;
        Quote, // '
        LeftComment, // (*
        RightComment, // *)
    }
}