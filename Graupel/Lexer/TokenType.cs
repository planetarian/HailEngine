namespace Graupel.Lexer
{
    public enum TokenType
    {
        // Ignored by parser

        Invalid,

        WhiteSpace,
        LineEnd,

        LineComment,
        BlockComment,


        Comma,
        Colon,
        SemiColon,
        Question,
        Bang,

        String,
        Integer,
        Float,
        Boolean,

        Identifier,

        Global,
        Scene,
        Template,
        Group,
        Entity,
        Eval,

        Asterisk,
        Slash,
        Percent,
        Caret,
        Plus,
        Minus,

        LeftParen,
        RightParen,
        LeftBrace,
        RightBrace,

        EOF
    }
}
