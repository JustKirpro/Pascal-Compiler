
namespace PascalCompiler
{
    public enum Operation
    {
        If,
        Do,
        Of,
        Or, 
        In,
        To,
        End, 
        Var,
        Div,
        And,
        Not,
        For,
        Xor,
        Mod,
        Nil,
        Set,
        Then,
        Else,
        Case,
        File,
        Goto,
        Type,
        With,
        Begin,
        While,
        Array,
        Const,
        Label,
        Until,
        Downto,
        Packed,
        Record,
        Repeat,
        Program,
        Function,
        Procedure,
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
        LeftBrace, // {
        RightBrace, // }
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
        Circumflex // ^
    }
}